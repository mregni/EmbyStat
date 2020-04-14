using System.Collections.Generic;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Net;

namespace Tests.Unit.Builders
{
    public class BoxSetBuilder
    {
        private readonly BoxSet _boxSet;

        public BoxSetBuilder(string parentId)
        {
            _boxSet = new BoxSet
            {
                Id = "123",
                ParentId = parentId,
                Name = "Lord of the rings",
                OfficialRating = "TV-16",
                Primary = "primary"
            };
        }

        public BoxSet Build()
        {
            return _boxSet;
        }
        public BaseItemDto ToBaseItemDto()
        {
            return new BaseItemDto
            {
                Id = _boxSet.Id,
                Name = _boxSet.Name,
                ParentId = _boxSet.ParentId,
                OfficialRating = _boxSet.OfficialRating,
                ImageTags = new Dictionary<ImageType, string>
                {
                    {ImageType.Primary, _boxSet.Primary}
                }
            };
        }
    }
}
