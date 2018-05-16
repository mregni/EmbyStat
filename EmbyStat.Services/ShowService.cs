using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common;
using EmbyStat.Common.Models;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Converters;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Show;
using EmbyStat.Services.Models.Stat;

namespace EmbyStat.Services
{
    public class ShowService : IShowService
    {
        private readonly IShowRepository _showRepository;
        private readonly ICollectionRepository _collectionRepository;

        public ShowService(IShowRepository showRepository, ICollectionRepository collectionRepository)
        {
            _showRepository = showRepository;
            _collectionRepository = collectionRepository;
        }

        public IEnumerable<Collection> GetShowCollections()
        {
            return _collectionRepository.GetCollectionByType(CollectionType.TvShow);
        }

        public ShowStat GetGeneralStats(List<string> collectionIds)
        {
            var shows = _showRepository.GetAllShows(collectionIds).ToList();
            return new ShowStat
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
                return PosterHelper.ConvertToShowPoster(resultShow, Constants.Shows.MostEpisodes, total.ToString());
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

        private Card TotalShowCount(IEnumerable<string> collectionIds)
        {
            var count = _showRepository.CountShows(collectionIds);
            return new Card
            {
                Title = Constants.Shows.TotalShows,
                Value = count.ToString()
            };
        }

        private Card TotalEpisodeCount(IEnumerable<string> collectionIds)
        {
            var count = _showRepository.CountEpisodes(collectionIds);
            return new Card
            {
                Title = Constants.Shows.TotalEpisodes,
                Value = count.ToString()
            };
        }

        private Card TotalMissingEpisodeCount(IEnumerable<Show> shows)
        {
            var count = shows.Sum(x => x.MissingEpisodesCount);
            return new Card
            {
                Title = Constants.Shows.TotalMissingEpisodes,
                Value = count.ToString()
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
