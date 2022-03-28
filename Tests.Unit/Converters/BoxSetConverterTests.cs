using System.Collections.Generic;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Net;
using EmbyStat.Logging;
using Xunit;

namespace Tests.Unit.Converters
{
    public class BoxSetConverterTests
    {
        [Fact]
        public void ConvertToBoxSet_Should_Return_BoxSet()
        {
            var baseItem = new BaseItemDto
            {
                Id = "123",
                Name = "Test",
                ParentId = "234",
                OfficialRating = "TV",
                ImageTags = new Dictionary<ImageType, string> { { ImageType.Primary, "primary" } }
            };

            var logger = LogFactory.CreateLoggerForType(typeof(BoxSetConverterTests), string.Empty);

            var boxSet = baseItem.ConvertToBoxSet(logger);
            boxSet.Should().NotBeNull();
            boxSet.Id.Should().Be(baseItem.Id);
            boxSet.Name.Should().Be(baseItem.Name);
            boxSet.ParentId.Should().Be(baseItem.ParentId);
            boxSet.OfficialRating.Should().Be(baseItem.OfficialRating);
            boxSet.Primary.Should().Be("primary");
        }
    }
}
