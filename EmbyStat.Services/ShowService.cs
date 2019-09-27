using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Show;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Abstract;
using EmbyStat.Services.Converters;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Charts;
using EmbyStat.Services.Models.Show;
using EmbyStat.Services.Models.Stat;
using MediaBrowser.Model.Entities;
using Newtonsoft.Json;
using LocationType = EmbyStat.Common.Enums.LocationType;

namespace EmbyStat.Services
{
    public class ShowService : MediaService, IShowService
    {
        private readonly IShowRepository _showRepository;
        private readonly ILibraryRepository _libraryRepository;
        private readonly IPersonService _personService;
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly ISettingsService _settingsService;

        public ShowService(IJobRepository jobRepository, IShowRepository showRepository, ILibraryRepository libraryRepository,
            IPersonService personService, IStatisticsRepository statisticsRepository, ISettingsService settingsService) : base(jobRepository)
        {
            _showRepository = showRepository;
            _libraryRepository = libraryRepository;
            _personService = personService;
            _statisticsRepository = statisticsRepository;
            _settingsService = settingsService;
        }

        public IEnumerable<Library> GetShowLibraries()
        {
            var settings = _settingsService.GetUserSettings();
            return _libraryRepository.GetLibrariesByTypes(settings.ShowLibraryTypes);
        }

        public async Task<ShowStatistics> GetStatistics(List<string> libraryIds)
        {
            var statistic = _statisticsRepository.GetLastResultByType(StatisticType.Show, libraryIds);

            if (StatisticsAreValid(statistic, libraryIds))
            {
                return JsonConvert.DeserializeObject<ShowStatistics>(statistic.JsonResult);
            }

            return await CalculateShowStatistics(libraryIds);
        }

        public async Task<ShowStatistics> CalculateShowStatistics(List<string> libraryIds)
        {
            var shows = _showRepository.GetAllShows(libraryIds, true, true).ToList();
            var statistics = new ShowStatistics
            {
                General = CalculateGeneralStatistics(shows),
                Charts = CalculateCharts(shows),
                People = await CalculatePeopleStatistics(shows)
            };

            var json = JsonConvert.SerializeObject(statistics);
            _statisticsRepository.AddStatistic(json, DateTime.UtcNow, StatisticType.Show, libraryIds);

            return statistics;
        }

        private ShowGeneral CalculateGeneralStatistics(IReadOnlyList<Show> shows)
        {
            return new ShowGeneral
            {
                ShowCount = TotalShowCount(shows),
                EpisodeCount = TotalEpisodeCount(shows),
                MissingEpisodeCount = TotalMissingEpisodeCount(shows),
                TotalPlayableTime = CalculatePlayableTime(shows),
                HighestRatedShow = CalculateHighestRatedShow(shows),
                LowestRatedShow = CalculateLowestRatedShow(shows),
                OldestPremieredShow = CalculateOldestPremieredShow(shows),
                ShowWithMostEpisodes = CalculateShowWithMostEpisodes(shows),
                LatestAddedShow = CalculateLatestAddedShow(shows),
                NewestPremieredShow = CalculateNewestPremieredShow(shows),
                TotalDiskSize = CalculateTotalDiskSize(shows.SelectMany(x => x.Episodes).Where(x => x.LocationType == LocationType.Disk))
            };
        }

        private ShowCharts CalculateCharts(IReadOnlyList<Show> shows)
        {
            var stats = new ShowCharts();
            stats.BarCharts.Add(CalculateGenreChart(shows));
            stats.BarCharts.Add(CalculateRatingChart(shows.Select(x => x.CommunityRating)));
            stats.BarCharts.Add(CalculatePremiereYearChart(shows.Select(x => x.PremiereDate)));
            stats.BarCharts.Add(CalculateCollectedRateChart(shows));
            stats.PieCharts.Add(CalculateOfficialRatingChart(shows));
            stats.PieCharts.Add(CalculateShowStateChart(shows));

            return stats;
        }

        private async Task<PersonStats> CalculatePeopleStatistics(IReadOnlyList<Show> shows)
        {
            return new PersonStats
            {
                MostFeaturedActorsPerGenre = await GetMostFeaturedActorsPerGenreAsync(shows)
            };
        }

        public IEnumerable<ShowCollectionRow> GetCollectedRows(List<string> libraryIds)
        {
            var statistic = _statisticsRepository.GetLastResultByType(StatisticType.ShowCollectedRows, libraryIds);

            if (StatisticsAreValid(statistic, libraryIds))
            {
                return JsonConvert.DeserializeObject<List<ShowCollectionRow>>(statistic.JsonResult);
            }

            return CalculateCollectedRows(libraryIds);
        }

        public IEnumerable<ShowCollectionRow> CalculateCollectedRows(List<string> libraryIds)
        {
            var shows = _showRepository.GetAllShows(libraryIds, true, true);

            var stats = shows
                .Select(CreateShowCollectedRow)
                .OrderBy(x => x.SortName);
            var json = JsonConvert.SerializeObject(stats);
            _statisticsRepository.AddStatistic(json, DateTime.UtcNow, StatisticType.ShowCollectedRows, libraryIds);

            return stats;
        }

        private ShowCollectionRow CreateShowCollectedRow(Show show)
        {
            var seasonCount = show.GetNonSpecialSeasonCount();

            return new ShowCollectionRow
            {
                Title = show.Name,
                SortName = show.SortName,
                Episodes = show.CollectedEpisodeCount,
                Seasons = seasonCount,
                Specials = show.SpecialEpisodeCount,
                MissingEpisodeCount = show.GetMissingEpisodeCount(),
                MissingEpisodes = show.GetMissingEpisodes().GroupBy(x => x.SeasonNumber, (index, episodes) => new VirtualSeason { Episodes = episodes, SeasonNumber = index}),
                PremiereDate = show.PremiereDate,
                Status = show.Status == "Continuing",
                Id = show.Id,
                Banner = show.Banner,
                Imdb = show.IMDB,
                Tvdb = show.TVDB,
                Size = show.GetShowSize()
            };
        }

        public bool TypeIsPresent()
        {
            return _showRepository.AnyShows();
        }

        private async Task<List<PersonPoster>> GetMostFeaturedActorsPerGenreAsync(IReadOnlyList<Show> shows)
        {
            var list = new List<PersonPoster>();
            var genres = shows.SelectMany(x => x.Genres).Distinct();

            foreach (var genre in genres.OrderBy(x => x))
            {
                var selectedShows = shows.Where(x => x.Genres.Any(y => y == genre));

                var personName = selectedShows
                    .SelectMany(x => x.People)
                    .Where(x => x.Type == PersonType.Actor)
                    .GroupBy(x => x.Name, (name, people) => new { Name = name, Count = people.Count() })
                    .OrderByDescending(x => x.Count)
                    .Select(x => x.Name)
                    .FirstOrDefault();

                if (personName != null)
                {
                    var person = await _personService.GetPersonByNameAsync(personName);
                    if (person != null)
                    {
                        list.Add(PosterHelper.ConvertToPersonPoster(person, genre));
                    }
                }
            }

            return list;
        }

        private Chart CalculateShowStateChart(IReadOnlyList<Show> shows)
        {
            var list = shows
                .GroupBy(x => x.Status)
                .Select(x => new { Name = x.Key, Count = x.Count() })
                .OrderBy(x => x.Name)
                .ToList();

            return new Chart
            {
                Title = Constants.Shows.ShowStatusChart,
                Labels = list.Select(x => x.Name),
                DataSets = new List<IEnumerable<int>> { list.Select(x => x.Count) }

            };
        }

        private Chart CalculateOfficialRatingChart(IReadOnlyList<Show> shows)
        {
            var ratingData = shows
                .Where(x => !string.IsNullOrWhiteSpace(x.OfficialRating))
                .GroupBy(x => x.OfficialRating.ToUpper())
                .Select(x => new { Name = x.Key, Count = x.Count() })
                .OrderBy(x => x.Name)
                .ToList();

            return new Chart
            {
                Title = Constants.CountPerOfficialRating,
                Labels = ratingData.Select(x => x.Name),
                DataSets = new List<IEnumerable<int>> { ratingData.Select(x => x.Count) }
            };
        }

        private Chart CalculateCollectedRateChart(IReadOnlyList<Show> shows)
        {
            var percentageList = new List<double>();
            foreach (var show in shows)
            {
                var specialSeasonId = show.Seasons.FirstOrDefault(x => x.IndexNumber == 0)?.Id.ToString() ?? "0";
                var episodeCount = show.Episodes.Count(x => x.LocationType == LocationType.Disk && x.ParentId != specialSeasonId);
                var missingEpisodeCount = show.Episodes.Count(x => x.LocationType == LocationType.Virtual && x.ParentId != specialSeasonId);

                if (episodeCount + missingEpisodeCount == 0)
                {
                    percentageList.Add(0);
                }
                else
                {
                    percentageList.Add((double)episodeCount / (episodeCount + missingEpisodeCount));
                }
            }

            var groupedList = percentageList
                .GroupBy(x => x.RoundToFive())
                .ToList();

            for (var i = 0; i < 20; i++)
            {
                if (groupedList.All(x => x.Key != i * 5))
                {
                    groupedList.Add(new ChartGrouping<int?, double> { Key = i * 5, Capacity = 0 });
                }
            }

            var rates = groupedList
                .OrderBy(x => x.Key)
                .Select(x => new { Name = x.Key != 100 ? $"{x.Key}% - {x.Key + 4}%" : $"{x.Key}%", Count = x.Count() })
                .Select(x => new { x.Name, x.Count })
                .ToList();

            return new Chart
            {
                Title = Constants.CountPerCollectedRate,
                Labels = rates.Select(x => x.Name),
                DataSets = new List<IEnumerable<int>> { rates.Select(x => x.Count) }
            };
        }

        private Chart CalculateGenreChart(IReadOnlyList<Show> shows)
        {
            var genresData = shows
                .SelectMany(x => x.Genres)
                .GroupBy(x => x)
                .Select(x => new { Name = x.Key, Count = x.Count() })
                .OrderBy(x => x.Name)
                .ToList();

            return new Chart
            {
                Title = Constants.CountPerGenre,
                Labels = genresData.Select(x => x.Name),
                DataSets = new List<IEnumerable<int>> { genresData.Select(x => x.Count) }
            };
        }

        private ShowPoster CalculateNewestPremieredShow(IReadOnlyList<Show> shows)
        {
            var yougest = shows
                .Where(x => x.PremiereDate.HasValue)
                .OrderByDescending(x => x.PremiereDate)
                .FirstOrDefault();

            if (yougest != null)
            {
                return PosterHelper.ConvertToShowPoster(yougest, Constants.Shows.NewestPremiered);
            }

            return new ShowPoster();
        }

        private ShowPoster CalculateLatestAddedShow(IReadOnlyList<Show> shows)
        {
            var yougest = shows
                .Where(x => x.DateCreated.HasValue)
                .OrderByDescending(x => x.DateCreated)
                .FirstOrDefault();

            if (yougest != null)
            {
                return PosterHelper.ConvertToShowPoster(yougest, Constants.Shows.LatestAdded);
            }

            return new ShowPoster();
        }

        private ShowPoster CalculateShowWithMostEpisodes(IReadOnlyList<Show> shows)
        {
            var total = 0;
            Show resultShow = null;
            foreach (var show in shows)
            {
                var episodes = show.GetNonSpecialEpisodeCount(false);
                if (episodes > total)
                {
                    total = episodes;
                    resultShow = show;
                }
            }

            if (resultShow != null)
            {
                return PosterHelper.ConvertToShowPoster(resultShow, Constants.Shows.MostEpisodes);
            }

            return new ShowPoster();
        }

        private ShowPoster CalculateOldestPremieredShow(IReadOnlyList<Show> shows)
        {
            var oldest = shows
                .Where(x => x.PremiereDate.HasValue)
                .OrderBy(x => x.PremiereDate)
                .FirstOrDefault();

            if (oldest != null)
            {
                return PosterHelper.ConvertToShowPoster(oldest, Constants.Shows.OldestPremiered);
            }

            return new ShowPoster();
        }

        private ShowPoster CalculateHighestRatedShow(IReadOnlyList<Show> shows)
        {
            var highest = shows
                .Where(x => x.CommunityRating.HasValue)
                .OrderByDescending(x => x.CommunityRating)
                .FirstOrDefault();

            if (highest != null)
            {
                return PosterHelper.ConvertToShowPoster(highest, Constants.Shows.HighestRatedShow);
            }

            return new ShowPoster();
        }

        private ShowPoster CalculateLowestRatedShow(IReadOnlyList<Show> shows)
        {
            var lowest = shows
                .Where(x => x.CommunityRating.HasValue)
                .OrderBy(x => x.CommunityRating)
                .FirstOrDefault();

            if (lowest != null)
            {
                return PosterHelper.ConvertToShowPoster(lowest, Constants.Shows.LowestRatedShow);
            }

            return new ShowPoster();
        }

        private Card<int> TotalShowCount(IReadOnlyList<Show> shows)
        {
            return new Card<int>
            {
                Title = Constants.Shows.TotalShows,
                Value = shows.Count
            };
        }

        private Card<int> TotalEpisodeCount(IReadOnlyList<Show> shows)
        {
            return new Card<int>
            {
                Title = Constants.Shows.TotalEpisodes,
                Value = shows.Sum(x => x.GetNonSpecialEpisodeCount(false))
            };
        }

        private Card<int> TotalMissingEpisodeCount(IReadOnlyList<Show> shows)
        {
            var count = shows.Sum(x => x.MissingEpisodesCount);
            return new Card<int>
            {
                Title = Constants.Shows.TotalMissingEpisodes,
                Value = count
            };
        }

        private TimeSpanCard CalculatePlayableTime(IReadOnlyList<Show> shows)
        {
            var playLength = new TimeSpan(shows.Sum(x => x.CumulativeRunTimeTicks ?? 0));
            return new TimeSpanCard
            {
                Title = Constants.Shows.TotalPlayLength,
                Days = playLength.Days,
                Hours = playLength.Hours,
                Minutes = playLength.Minutes
            };
        }
    }
}
