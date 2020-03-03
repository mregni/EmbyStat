using System;
using System.Collections.Generic;
using EmbyStat.Clients.Base.Models;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;

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

        public BaseItemDto BuildBaseItemDto()
        {
            return new BaseItemDto
            {
                Id = _season.Id,
                DateCreated = _season.DateCreated,
                ParentId = _season.ParentId,
                Path = _season.Path,
                SortName = _season.SortName,
                IndexNumber = _season.IndexNumber,
                IndexNumberEnd = _season.IndexNumberEnd,
                PremiereDate = _season.PremiereDate,
                ProductionYear = _season.ProductionYear,
                ImageTags = new Dictionary<ImageType, string>
                {
                    {ImageType.Primary, _season.Primary},
                    {ImageType.Thumb, _season.Thumb},
                    {ImageType.Logo, _season.Logo},
                    {ImageType.Banner, _season.Banner}
                }
            };
        }
    }
}
