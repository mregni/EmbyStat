using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using FluentAssertions;
using Tests.Unit.Builders;
using Xunit;

namespace Tests.Unit.Extensions;

public class IntExtensionTests
{
    [Theory]
    [InlineData(1, "Season 1")]
    [InlineData(2, "Season 2")]
    [InlineData(0, "Special")]
    public void ConvertToVirtualSeason_Should_Work(int index, string namePrefix)
    {
        var show = new ShowBuilder("1").Build();

        var result = index.ConvertToVirtualSeason(show);
        result.Should().NotBeNull();
        result.Id.Should().NotBeNull();
        result.Name.Should().Be($"{namePrefix}");
        result.ShowId.Should().Be(show.Id);
        result.Path.Should().BeEmpty();
        result.DateCreated.Should().BeNull();
        result.IndexNumber.Should().Be(index);
        result.IndexNumberEnd.Should().Be(index);
        result.PremiereDate.Should().BeNull();
        result.ProductionYear.Should().BeNull();
        result.SortName.Should().Be(index.ToString("0000"));
        result.LocationType.Should().Be(LocationType.Virtual);
        result.Episodes.Should().BeEmpty();
    }
}