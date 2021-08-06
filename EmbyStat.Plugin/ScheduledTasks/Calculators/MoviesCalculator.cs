using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Services.Models.Movie;
using EmbyStat.Services.Models.Stat;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Logging;

namespace EmbyStat.Plugin.ScheduledTasks.Calculators
{
    public class MoviesCalculator
    {
        private readonly ILogger _logger;
        private readonly ILibraryManager _libraryManager;

        public MoviesCalculator(ILogger logger, ILibraryManager libraryManager)
        {
            _logger = logger;
            _libraryManager = libraryManager;
        }

        public MovieStatistics CalculateStatistics(BaseItem[] libraries)
        {
            return new MovieStatistics
            {
                Cards = CalculateCards(libraries)
            };
        }

        #region Cards

        private List<Card<string>> CalculateCards(IEnumerable<BaseItem> libraries)
        {
            var internalIds = libraries
                .Select(x => _libraryManager.GetInternalId(x.Id))
                .ToArray();
            var items = _libraryManager.GetItemList(CreateDefaultQuery(internalIds));

            return new List<Card<string>>
            {
                CalculateTotalMovieCount(items),
                CalculateTotalMovieGenres(internalIds),
                CalculateTotalPlayLength(items)
            };
        }

        private static Card<string> CalculateTotalMovieCount(IEnumerable<BaseItem> movies)
        {
            return new Card<string>
            {
                Title = Constants.Movies.TotalMovies,
                Value = movies.Count().ToString(),
                Type = CardType.Text,
                Icon = Constants.Icons.TheatersRoundedIcon
            };
        }

        private Card<string> CalculateTotalMovieGenres(long[] internalIds)
        {
            var genres = _libraryManager.GetGenres(CreateDefaultQuery(internalIds));
            
            return new Card<string>
            {
                Title = Constants.Common.TotalGenres,
                Value = genres.TotalRecordCount.ToString(),
                Type = CardType.Text,
                Icon = Constants.Icons.PoundRoundedIcon
            };
        }

        private Card<string> CalculateTotalPlayLength(IEnumerable<BaseItem> movies)
        {
            var playLengthTicks = movies.Sum(x => x.RunTimeTicks ?? 0);
            var playLength = new TimeSpan(playLengthTicks);

            return new Card<string>
            {
                Title = Constants.Movies.TotalPlayLength,
                Value = $"{playLength.Days}|{playLength.Hours}|{playLength.Minutes}",
                Type = CardType.Time,
                Icon = Constants.Icons.QueryBuilderRoundedIcon
            };
        }

        private Card<string> CalculateTotalDiskSize(IEnumerable<BaseItem> movies)
        {
            var sum = movies.Sum(x => x.Size);
            return new Card<string>
            {
                Value = sum.ToString(CultureInfo.InvariantCulture),
                Title = Constants.Common.TotalDiskSpace,
                Type = CardType.Size,
                Icon = Constants.Icons.StorageRoundedIcon
            };
        }

        #endregion

        private InternalItemsQuery CreateDefaultQuery(long[] internalIds)
        {
            return new InternalItemsQuery
            {
                TopParentIds = internalIds,
                IncludeItemTypes = new[] { "Movie" },
                Recursive = true
            };;
        }
    }
}
