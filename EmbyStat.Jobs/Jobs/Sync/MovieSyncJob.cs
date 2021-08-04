using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Clients.Base;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Common;
using EmbyStat.Common.Converters;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Hubs.Job;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Jobs.Jobs.Interfaces;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using Hangfire;
using MoreLinq;

namespace EmbyStat.Jobs.Jobs.Sync
{
    [DisableConcurrentExecution(60 * 60)]
    public class MovieSyncJob : BaseJob, IMovieSyncJob
    {
        private readonly IHttpClient _httpClient;
        private readonly ILibraryRepository _libraryRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly IMovieService _movieService;

        public MovieSyncJob(IJobHubHelper hubHelper, IJobRepository jobRepository,
            ISettingsService settingsService, ILibraryRepository libraryRepository, IClientStrategy clientStrategy,
            IMovieRepository movieRepository, IStatisticsRepository statisticsRepository, 
            IMovieService movieService) 
            : base(hubHelper, jobRepository, settingsService, typeof(MovieSyncJob), Constants.LogPrefix.MovieSyncJob)
        {
            _libraryRepository = libraryRepository;
            _movieRepository = movieRepository;
            _statisticsRepository = statisticsRepository;
            _movieService = movieService;

            var settings = settingsService.GetUserSettings();
            _httpClient = clientStrategy.CreateHttpClient(settings.MediaServer?.ServerType ?? ServerType.Emby);
            Title = jobRepository.GetById(Id).Title;
        }

        public sealed override Guid Id => Constants.JobIds.MovieSyncId;
        public override string Title { get; }
        public override string JobPrefix => Constants.LogPrefix.MovieSyncJob;
        public override async Task RunJobAsync()
        {
            var cancellationToken = new CancellationToken(false);
            if (!Settings.WizardFinished)
            {
                await LogWarning("Media sync task not running because wizard is not yet finished!");
                return;
            }

            if (!IsMediaServerOnline())
            {
                await LogWarning($"Halting task because we can't contact the server on {Settings.MediaServer.FullMediaServerAddress}, please check the connection and try again.");
                return;
            }

            await LogInformation("First delete all existing movie and root media libraries from database so we have a clean start.");
            await LogProgress(3);

            var libraries = await GetLibrariesAsync();
            _libraryRepository.AddOrUpdateRange(libraries);
            await LogInformation($"Found {libraries.Count} root items, getting ready for processing");
            await LogProgress(15);

            await ProcessMoviesAsync(libraries, cancellationToken);
            await LogProgress(55);

            await CalculateStatistics();
            await LogProgress(100);
        }

        private async Task ProcessMoviesAsync(IEnumerable<Library> libraries, CancellationToken cancellationToken)
        {
            await LogInformation("Lets start processing movies");
            _movieRepository.RemoveMovies();

            var neededLibraries = libraries.Where(x => Settings.MovieLibraries.Any(y => y == x.Id)).ToList();
            var logIncrementBase = Math.Round(40 / (double)neededLibraries.Count, 1);
            foreach (var library in neededLibraries)
            {
                var collectionId = library.Id;
                var totalCount = await GetTotalLibraryMovieCount(collectionId);
                if (totalCount == 0)
                {
                    continue; ;
                }
                var increment = logIncrementBase / (totalCount / (double)100);

                await LogInformation($"Found {totalCount} movies in {library.Name} library");
                var processed = 0;
                var j = 0;
                const int limit = 100;
                do
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var movies = await PerformMovieSyncAsync(collectionId, collectionId, j * limit, limit);
                    _movieRepository.UpsertRange(movies.Where(x => x != null && x.MediaType == "Video"));

                    processed += 100;
                    j++;
                    var logProcessed = processed < totalCount ? processed : totalCount;
                    await LogInformation($"Processed { logProcessed } / { totalCount } movies");
                    await LogProgressIncrement(increment);
                } while (processed < totalCount);

            }
        }

        private async Task<int> GetTotalLibraryMovieCount(string parentId)
        {
            var count = _httpClient.GetMovieCount(parentId);
            if (count == 0)
            {
                await LogWarning($"0 movies found in parent with id {parentId}. Probably means something is wrong with the HTTP call.");
            }

            return count;
        }

        private async Task<List<Movie>> PerformMovieSyncAsync(string parentId, string collectionId, int startIndex, int limit)
        {
            try
            {
                return _httpClient.GetMovies(parentId, collectionId, startIndex, limit);
            }
            catch (Exception e)
            {
                await LogError($"Movie error: {e.Message}");
                throw;
            }
        }

        private async Task CalculateStatistics()
        {
            await LogInformation("Calculating movie statistics");
            _statisticsRepository.MarkMovieTypesAsInvalid();
            _movieService.CalculateMovieStatistics(new List<string>(0));

            await LogInformation($"Calculations done");
            await LogProgress(100);
        }


        #region Helpers

        private async Task<List<Library>> GetLibrariesAsync()
        {
            await LogInformation("Asking MediaServer for all movie root folders");
            var rootItems = _httpClient.GetMediaFolders();

            Logger.Debug("Following root items are found:");
            rootItems.Items.ForEach(x => Logger.Debug(x.ToString()));

            return rootItems.Items
                .Where(x => x.CollectionType.ToLibraryType() == LibraryType.Movies)
                .Select(LibraryConverter.ConvertToLibrary)
                .ToList();
        }

        private bool IsMediaServerOnline()
        {
            _httpClient.BaseUrl = Settings.MediaServer.FullMediaServerAddress;
            return _httpClient.Ping();
        }

        #endregion
    }
}
