using System;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models;
using FluentAssertions;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Querying;
using Xunit;

namespace Tests.Unit.Extensions;

public class ItemQueryExtensionTests
{
    [Fact]
    public void ConvertToStringDictionary_Should_Convert_All()
    {
        var query = new ItemQuery
        {
            Fields = new[] {ItemFields.Bitrate, ItemFields.Chapters},
            UserId = "1234",
            EnableTotalRecordCount = true,
            EnableImageTypes = new[] {ImageType.Banner, ImageType.Box},
            LocationTypes = new[] {LocationType.Virtual, LocationType.FileSystem},
            ExcludeLocationTypes = new[] {LocationType.Virtual, LocationType.FileSystem},
            MinDateLastSaved = new DateTime(2020, 1, 1),
            ParentId = "12345",
            StartIndex = 1,
            Limit = 100,
            Recursive = true,
            EnableImages = true,
            MediaTypes = new[] {"Video", "BoxSet"},
            IncludeItemTypes = new[] {"Movies", "Episodes"},
            Ids = new[] { "12", "13"}
        };

        var result = query.ConvertToStringDictionary();
        result.Count.Should().Be(16);
        result.ContainsKey("Ids").Should().BeTrue();
        result["Ids"].Should().Be(string.Join(',', query.Ids));
        result.ContainsKey("UserId").Should().BeTrue();
        result["UserId"].Should().Be(query.UserId);
        result.ContainsKey("EnableTotalRecordCount").Should().BeTrue();
        result["EnableTotalRecordCount"].Should().Be(query.EnableTotalRecordCount.ToString());
        result.ContainsKey("Fields").Should().BeTrue();
        result["Fields"].Should().Be(string.Join(',', query.Fields));
        result.ContainsKey("EnableImageTypes").Should().BeTrue();
        result["EnableImageTypes"].Should().Be(string.Join(',', query.EnableImageTypes));
        result.ContainsKey("LocationTypes").Should().BeTrue();
        result["LocationTypes"].Should().Be(string.Join(',', query.LocationTypes));
        result.ContainsKey("ExcludeLocationTypes").Should().BeTrue();
        result["ExcludeLocationTypes"].Should().Be(string.Join(',', query.ExcludeLocationTypes));
        result.ContainsKey("MinDateLastSaved").Should().BeTrue();
        var date = query.MinDateLastSaved?.ToString("O");
        result["MinDateLastSaved"].Should().Be(date);
        result.ContainsKey("MinDateLastSavedForUser").Should().BeTrue();
        result["MinDateLastSavedForUser"].Should().Be(date);
        result.ContainsKey("ParentId").Should().BeTrue();
        result["ParentId"].Should().Be(query.ParentId);
        result.ContainsKey("StartIndex").Should().BeTrue();
        result["StartIndex"].Should().Be(query.StartIndex.ToString());
        result.ContainsKey("Limit").Should().BeTrue();
        result["Limit"].Should().Be(query.Limit.ToString());
        result.ContainsKey("Recursive").Should().BeTrue();
        result["Recursive"].Should().Be(query.Recursive.ToString());
        result.ContainsKey("EnableImages").Should().BeTrue();
        result["EnableImages"].Should().Be(query.EnableImages.ToString());
        result.ContainsKey("MediaTypes").Should().BeTrue();
        result["MediaTypes"].Should().Be(string.Join('|', query.MediaTypes));
        result.ContainsKey("IncludeItemTypes").Should().BeTrue();
        result["IncludeItemTypes"].Should().Be(string.Join('|', query.IncludeItemTypes));
    }
    
}