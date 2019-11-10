using System;
using EmbyStat.Common.Models.Entities;

namespace Tests.Unit.Builders
{
    public class SeasonBuilder
    {
        private readonly Season _season;

        public SeasonBuilder(int id, string showId)
        {
            _season = new Season
            {
                Id = id,
                Path = "path/to/season",
                PremiereDate = new DateTimeOffset(2001, 1, 1, 0, 0, 0, new TimeSpan(0)),
                Name = "Season 01",
                IndexNumber = 1,
                ParentId = showId,
                ProductionYear = 2001,
                SortName = "0001",
                DateCreated = new DateTimeOffset(2001, 1, 1, 0, 0, 0, new TimeSpan(0)),
                Primary = "primary.jpg"
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
