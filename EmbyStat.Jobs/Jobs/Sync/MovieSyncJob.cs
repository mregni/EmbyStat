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
using EmbyStat.Common.Models.Settings;
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

            await LogProgress(15);

            await ProcessMoviesAsync(cancellationToken);
            await LogProgress(55);

            await CalculateStatistics();
            await LogProgress(100);
        }

        private async Task ProcessMoviesAsync(CancellationToken cancellationToken)
        {
            await LogInformation("Lets start processing movies");
            await LogInformation($"{Settings.MovieLibraries.Count} libraries are selected, getting ready for processing");

            var logIncrementBase = Math.Round(40 / (double)Settings.MovieLibraries.Count, 1);
            foreach (var library in Settings.MovieLibraries)
            {
                var totalCount = _httpClient.GetMovieCount(library.Id, library.LastSynced);
                if (totalCount == 0)
                {
                    continue;
                }
                var increment = logIncrementBase / (totalCount / (double)100);

                await LogInformation($"Found {totalCount} changed movies since last sync in {library.Name}");
                var processed = 0;
                var j = 0;
                const int limit = 100;
                do
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var movies = await FetchMoviesAsync(library.Id, library, j * limit, limit);
                    _movieRepository.UpsertRange(movies.Where(x => x != null && x.MediaType == "Video"));

                    processed += 100;
                    j++;
                    var logProcessed = processed < totalCount ? processed : totalCount;
                    await LogInformation($"Processed { logProcessed } / { totalCount } movies");
                    await LogProgressIncrement(increment);
                } while (processed < totalCount);
                await SettingsService.UpdateLibrarySyncDate(library.Id, DateTime.UtcNow);
            }
        }

        private async Task<List<Movie>> FetchMoviesAsync(string parentId, LibraryContainer library, int startIndex, int limit)
        {
            try
            {
                return _httpClient.GetMovies(parentId, library.Id, startIndex, limit, library.LastSynced);
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
            await LogProgress(67);
            _movieService.CalculateMovieStatistics(new List<string>(0));

            await LogInformation($"Calculations done");
            await LogProgress(100);
        }


        #region Helpers

        private bool IsMediaServerOnline()
        {
            _httpClient.BaseUrl = Settings.MediaServer.FullMediaServerAddress;
            return _httpClient.Ping();
        }

        #endregion
    }
}
