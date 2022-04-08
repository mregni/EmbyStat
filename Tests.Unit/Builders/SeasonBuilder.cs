using System;
using System.Collections.Generic;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Shows;
using EmbyStat.Common.Models.Net;

namespace Tests.Unit.Builders
{
    public class SeasonBuilder
    {
        
        private readonly Season _season;

        public SeasonBuilder(string id, string showId)
        {
            _season = new Season
            {
                Id = id,
                Path = "path/to/season",
                PremiereDate = new DateTime(2001, 1, 1, 0, 0, 0),
                Name = "Season 01",
                IndexNumber = 1,
                ShowId = showId,
                ProductionYear = 2001,
                SortName = "0001",
                DateCreated = new DateTime(2001, 1, 1, 0, 0, 0),
                Primary = "primary.jpg",
                Episodes = new List<Episode>(new []
                {
                    new EpisodeBuilder("1", id).Build(), 
                    new EpisodeBuilder("2", id).Build()
                })
            };
        }

        public SeasonBuilder WithIndexNumber(int indexNumber)
        {
            _season.IndexNumber = indexNumber;
            return this;
        }

        public Season Build()
        {
            return _season;
        }
    }
}
