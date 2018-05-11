using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common;
using EmbyStat.Common.Models;
using EmbyStat.Repositories.Interfaces;
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
            return new ShowStat
            {
                ShowCount = TotalShowCount(collectionIds),
                EpisodeCount = TotalEpisodeCount(collectionIds),
                TotalPlayableTime = CalculatePlayableTime(collectionIds)
            };
        }

        private Card TotalShowCount(List<string> collectionIds)
        {
            var count = _showRepository.CountShows(collectionIds);
            return new Card
            {
                Title = Constants.Shows.TotalShows,
                Value = count.ToString()
            };
        }

        private Card TotalEpisodeCount(List<string> collectionIds)
        {
            var count = _showRepository.CountEpisodes(collectionIds);
            return new Card
            {
                Title = Constants.Shows.TotalEpisodes,
                Value = count.ToString()
            };
        }

        private TimeSpanCard CalculatePlayableTime(List<string> collectionIds)
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
