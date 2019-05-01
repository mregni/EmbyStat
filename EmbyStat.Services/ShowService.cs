using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extentions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Abstract;
using EmbyStat.Services.Converters;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Charts;
using EmbyStat.Services.Models.Show;
using EmbyStat.Services.Models.Stat;
using MediaBrowser.Model.Entities;
using Newtonsoft.Json;

namespace EmbyStat.Services
{
    public class ShowService : MediaService, IShowService
    {
        private readonly IShowRepository _showRepository;
        private readonly ICollectionRepository _collectionRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IPersonService _personService;
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly ISettingsService _settingsService;

        public ShowService(IShowRepository showRepository, ICollectionRepository collectionRepository, IGenreRepository genreRepository,
            IPersonService personService, IJobRepository jobRepository, IStatisticsRepository statisticsRepository, ISettingsService settingsService)
        : base(jobRepository)
        {
            _showRepository = showRepository;
            _collectionRepository = collectionRepository;
            _genreRepository = genreRepository;
            _personService = personService;
            _statisticsRepository = statisticsRepository;
            _settingsService = settingsService;
        }

        public IEnumerable<Collection> GetShowCollections()
        {
            var settings = _settingsService.GetUserSettings();
            return _collectionRepository.GetCollectionByTypes(settings.ShowCollectionTypes);
        }

        public async Task<ShowStat> GetGeneralStats(List<string> collectionIds)
        {
            var statistic = _statisticsRepository.GetLastResultByType(StatisticType.ShowGeneral);

            ShowStat stats;
            if (StatisticsAreValid(statistic, collectionIds))
            {
                stats = JsonConvert.DeserializeObject<ShowStat>(statistic.JsonResult);
            }
            else
            {
                var shows = _showRepository.GetAllShows(collectionIds).ToList();
                stats = new ShowStat
                {
                    ShowCount = TotalShowCount(collectionIds),
                    EpisodeCount = TotalEpisodeCount(collectionIds),
                    MissingEpisodeCount = TotalMissingEpisodeCount(shows),
                    TotalPlayableTime = CalculatePlayableTime(collectionIds),
                    HighestRatedShow = CalculateHighestRatedShow(shows),
                    LowestRatedShow = CalculateLowestRatedShow(shows),
                    OldestPremieredShow = CalculateOldestPremieredShow(shows),
                    ShowWithMostEpisodes = CalculateShowWithMostEpisodes(shows),
                    YoungestAddedShow = CalculateYoungestAddedShow(shows),
                    YoungestPremieredShow = CalculateYoungestPremieredShow(shows)
                };

                var json = JsonConvert.SerializeObject(stats);
                await _statisticsRepository.AddStatistic(json, DateTime.UtcNow, StatisticType.ShowGeneral, collectionIds);
            }

            return stats;
        }

        public async Task<ShowCharts> GetCharts(List<string> collectionIds)
        {
            var statistic = _statisticsRepository.GetLastResultByType(StatisticType.ShowCharts);

            ShowCharts stats;
            if (StatisticsAreValid(statistic, collectionIds))
            {
                stats = JsonConvert.DeserializeObject<ShowCharts>(statistic.JsonResult);
            }
            else
            {
                var shows = _showRepository.GetAllShows(collectionIds, true).ToList();

                stats = new ShowCharts();
                stats.BarCharts.Add(CalculateGenreChart(shows));
                stats.BarCharts.Add(CalculateRatingChart(shows.Select(x => x.CommunityRating)));
                stats.BarCharts.Add(CalculatePremiereYearChart(shows.Select(x => x.PremiereDate)));
                stats.BarCharts.Add(CalculateCollectedRateChart(shows));
                stats.BarCharts.Add(CalculateOfficialRatingChart(shows));
                stats.PieCharts.Add(CalculateShowStateChart(shows));

                var json = JsonConvert.SerializeObject(stats);
                await _statisticsRepository.AddStatistic(json, DateTime.UtcNow, StatisticType.ShowCharts, collectionIds);
            }

            return stats;
        }

        public async Task<PersonStats> GetPeopleStats(List<string> collectionIds)
        {
            var statistic = _statisticsRepository.GetLastResultByType(StatisticType.ShowPeople);

            PersonStats stats;
            if (StatisticsAreValid(statistic, collectionIds))
            {
                stats = JsonConvert.DeserializeObject<PersonStats>(statistic.JsonResult);
            }
            else
            {
                stats = new PersonStats
                {
                    TotalActorCount = TotalTypeCount(collectionIds, PersonType.Actor, Constants.Common.TotalActors),
                    TotalDirectorCount = TotalTypeCount(collectionIds, PersonType.Director, Constants.Common.TotalDirectors),
                    TotalWriterCount = TotalTypeCount(collectionIds, PersonType.Writer, Constants.Common.TotalWriters)
                };


                var json = JsonConvert.SerializeObject(stats);
                await _statisticsRepository.AddStatistic(json, DateTime.UtcNow, StatisticType.ShowPeople, collectionIds);
            }

            return stats;
        }

        public async Task<List<ShowCollectionRow>> GetCollectionRows(List<string> collectionIds)
        {
            var statistic = _statisticsRepository.GetLastResultByType(StatisticType.ShowCollected);

            List<ShowCollectionRow> stats;
            if (StatisticsAreValid(statistic, collectionIds))
            {
                stats = JsonConvert.DeserializeObject<List<ShowCollectionRow>>(statistic.JsonResult);
            }
            else
            {
                var shows = _showRepository.GetAllShows(collectionIds);

                stats = shows.Select(CreateShowCollectionRow).ToList();

                var json = JsonConvert.SerializeObject(stats);
                await _statisticsRepository.AddStatistic(json, DateTime.UtcNow, StatisticType.ShowCollected, collectionIds);
            }

            return stats;
        }

        private ShowCollectionRow CreateShowCollectionRow(Show show)
        {
            var episodeCount = _showRepository.GetEpisodeCountForShow(show.Id);
            var totalEpisodeCount = _showRepository.GetEpisodeCountForShow(show.Id, true);
            var specialCount = totalEpisodeCount - episodeCount;
            var seasonCount = _showRepository.GetSeasonCountForShow(show.Id);

            return new ShowCollectionRow
            {
                Title = show.Name,
                SortName = show.SortName,
                Episodes = episodeCount,
                Seasons = seasonCount,
                Specials = specialCount,
                MissingEpisodes = show.MissingEpisodesCount,
                PremiereDate = show.PremiereDate,
                Status = show.Status == "Continuing"
            };
        }

        public bool ShowTypeIsPresent()
        {
            return _showRepository.Any();
        }

        private async Task<List<PersonPoster>> GetMostFeaturedActorsPerGenreAsync(List<string> collectionIds)
        {
            var shows = _showRepository.GetAllShows(collectionIds);
            var genreIds = _showRepository.GetGenres(collectionIds);
            var genres = _genreRepository.GetListByIds(genreIds);

            var list = new List<PersonPoster>();
            foreach (var genre in genres.OrderBy(x => x.Name))
            {
                var selectedShows = shows.Where(x => x.MediaGenres.Any(y => y.GenreId == genre.Id));
                var episodes = _showRepository.GetAllEpisodesForShows(selectedShows.Select(x => x.Id), true);

                var grouping = episodes
                    .SelectMany(x => x.ExtraPersons)
                    .Where(x => x.Type == PersonType.Actor)
                    .GroupBy(x => x.PersonId)
                    .Select(group => new { Id = group.Key, Count = group.Count() })
                    .OrderByDescending(x => x.Count);

                var personId = grouping
                    .Select(x => x.Id)
                    .FirstOrDefault();
                //Compleet buggy dit! Er moet gekeken worden naar het aantal episodes ipv shows
                //Misschien ExtraPerson weer toevoegen aan Episode type in sync!
                var person = await _personService.GetPersonByIdAsync(personId);
                if(person != null){
                    list.Add(PosterHelper.ConvertToPersonPoster(person, genre.Name));
                }
            }

            return list;
        }

        private async Task<PersonPoster> GetMostFeaturedPersonAsync(IEnumerable<string> collectionIds, string type, string title)
        {
            var personId = _showRepository.GetMostFeaturedPerson(collectionIds, type);
            var person = await _personService.GetPersonByIdAsync(personId);

            return person == null ? new PersonPoster() : PosterHelper.ConvertToPersonPoster(person, title);
        }

        private Card<int> TotalTypeCount(IEnumerable<string> collectionIds, string type, string title)
        {
            return new Card<int>
            {
                Value = _showRepository.GetTotalPersonByType(collectionIds, type),
                Title = title
            };
        }

        private Chart CalculateShowStateChart(IEnumerable<Show> shows)
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
                DataSets = new List<IEnumerable<int>> { list.Select(x => x.Count)}

            };
        }

        private Chart CalculateOfficialRatingChart(IEnumerable<Show> shows)
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

        private Chart CalculateCollectedRateChart(IEnumerable<Show> shows)
        {
            var percentageList = new List<double>();
            foreach (var show in shows)
            {
                var episodeCount = _showRepository.CountEpisodes(show.Id);
                if (episodeCount + show.MissingEpisodesCount == 0)
                {
                    percentageList.Add(0);
                }
                else
                {
                    percentageList.Add((double)episodeCount / (episodeCount + show.MissingEpisodesCount));
                }
            }

            var groupedList = percentageList
                .GroupBy(x => x.RoundToFive())
                .OrderBy(x => x.Key)
                .ToList();

            if (percentageList.Any())
            {
                var j = 0;
                for (var i = 0; i < 20; i++)
                {
                    if (groupedList[j].Key != i * 5)
                    {
                        groupedList.Add(new ChartGrouping<int?, double> { Key = i * 5, Capacity = 0 });
                    }
                    else
                    {
                        j++;
                    }
                }
            }

            var rates = groupedList
                .OrderBy(x => x.Key)
                .Select(x => new { Name = x.Key != 100 ? $"{x.Key}% - {x.Key + 4}%" : $"{x.Key}%", Count = x.Count() })
                .Select(x => new { Name = x.Name, Count = x.Count })
                .ToList();

            return new Chart
            {
                Title = Constants.CountPerCollectedRate,
                Labels = rates.Select(x => x.Name),
                DataSets = new List<IEnumerable<int>> { rates.Select(x => x.Count) }
            };
        }

        private Chart CalculateGenreChart(IEnumerable<Show> shows)
        {
            var genres = _genreRepository.GetAll();
            var genresData = shows.SelectMany(x => x.MediaGenres).GroupBy(x => x.GenreId)
                .Select(x => new { Name = genres.Single(y => y.Id == x.Key).Name, Count = x.Count() })
                .Select(x => new { Name = x.Name, Count = x.Count })
                .OrderBy(x => x.Name)
                .ToList();

            return new Chart
            {
                Title = Constants.CountPerGenre,
                Labels = genresData.Select(x => x.Name),
                DataSets = new List<IEnumerable<int>> { genresData.Select(x => x.Count) }
            };
        }

        private ShowPoster CalculateYoungestPremieredShow(IEnumerable<Show> shows)
        {
            var yougest = shows
                .Where(x => x.PremiereDate.HasValue)
                .OrderByDescending(x => x.PremiereDate)
                .FirstOrDefault();

            if (yougest != null)
            {
                return PosterHelper.ConvertToShowPoster(yougest, Constants.Shows.YoungestPremiered);
            }

            return new ShowPoster();
        }

        private ShowPoster CalculateYoungestAddedShow(IEnumerable<Show> shows)
        {
            var yougest = shows
                .Where(x => x.DateCreated.HasValue)
                .OrderByDescending(x => x.DateCreated)
                .FirstOrDefault();

            if (yougest != null)
            {
                return PosterHelper.ConvertToShowPoster(yougest, Constants.Shows.YoungestAdded);
            }

            return new ShowPoster();
        }

        private ShowPoster CalculateShowWithMostEpisodes(IEnumerable<Show> shows)
        {
            var total = 0;
            Show resultShow = null;
            foreach (var show in shows)
            {
                var episodes = _showRepository.GetAllEpisodesForShow(show.Id).ToArray();
                if (episodes.Length > total)
                {
                    total = episodes.Length;
                    resultShow = show;
                }
            }

            if (resultShow != null)
            {
                return PosterHelper.ConvertToShowPoster(resultShow, Constants.Shows.MostEpisodes);
            }

            return new ShowPoster();
        }

        private ShowPoster CalculateOldestPremieredShow(IEnumerable<Show> shows)
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

        private ShowPoster CalculateHighestRatedShow(IEnumerable<Show> shows)
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

        private ShowPoster CalculateLowestRatedShow(IEnumerable<Show> shows)
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

        private Card<int> TotalShowCount(IEnumerable<string> collectionIds)
        {
            var count = _showRepository.CountShows(collectionIds);
            return new Card<int>
            {
                Title = Constants.Shows.TotalShows,
                Value = count
            };
        }

        private Card<int> TotalEpisodeCount(IEnumerable<string> collectionIds)
        {
            var count = _showRepository.CountEpisodes(collectionIds);
            return new Card<int>
            {
                Title = Constants.Shows.TotalEpisodes,
                Value = count
            };
        }

        private Card<int> TotalMissingEpisodeCount(IEnumerable<Show> shows)
        {
            var count = shows.Sum(x => x.MissingEpisodesCount);
            return new Card<int>
            {
                Title = Constants.Shows.TotalMissingEpisodes,
                Value = count
            };
        }

        private TimeSpanCard CalculatePlayableTime(IEnumerable<string> collectionIds)
        {
            var playLength = new TimeSpan(_showRepository.GetPlayLength(collectionIds));
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
