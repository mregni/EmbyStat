using System;
using System.IO;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using FluentAssertions;
using Xunit;

namespace Tests.Unit.Extensions;

public class StringExtensionTests
{
    [Theory]
    [InlineData("0.1.0-dev.0", "0.1.0.0")]
    [InlineData("0.1-beta.0.0", "0.1.0.0")]
    [InlineData("0.1.0.0", "0.1.0.0")]
    public void ToCleanVersionString_Should_Remove_Version_Tags(string input, string output)
    {
        var result = input.ToCleanVersionString();
        result.Should().Be(output);
    }

    [Fact]
    public void GetLocalPath_Should_Add_Current_Directory_As_Prefix()
    {
        var path = "temp";
        var fullPath = path.GetLocalPath();

        fullPath.Should().Be(Path.Combine(Directory.GetCurrentDirectory(), path));
    }
    
    [Fact]
    public void GetLocalFilePath_Should_Add_Current_Directory_As_Prefix()
    {
        var path = "temp";
        var fileName = "log.txt";
        var fullPath = path.GetLocalFilePath(fileName);

        fullPath.Should().Be(Path.Combine(Directory.GetCurrentDirectory(), path, fileName));
    }

    [Theory]
    [InlineData("movies", LibraryType.Movies)]
    [InlineData("tvshows", LibraryType.TvShow)]
    [InlineData("music", LibraryType.Music)]
    [InlineData("musicvideos", LibraryType.MusicVideos)]
    [InlineData("trailers", LibraryType.Trailers)]
    [InlineData("homevideos", LibraryType.HomeVideos)]
    [InlineData("books", LibraryType.Books)]
    [InlineData("photos", LibraryType.Photos)]
    [InlineData("games", LibraryType.Games)]
    [InlineData("livetv", LibraryType.LiveTv)]
    [InlineData("playlists", LibraryType.Playlists)]
    [InlineData("folders", LibraryType.Folders)]
    [InlineData("boxsets", LibraryType.BoxSets)]
    [InlineData("random", LibraryType.Other)]
    public void ToCollectionType_Should_Return_Proper_Library_Type(string input, LibraryType output)
    {
        var result = input.ToLibraryType();
        result.Should().Be(output);
    }

    [Fact]
    public void GetLocalPath_Should_Return_Path()
    {
        var localPath = "/show";
        var fullPath = localPath.GetLocalPath();

        fullPath.Should().NotBeNull();
        fullPath.Should().Be(Path.Combine(Directory.GetCurrentDirectory(), localPath));
    }

    [Fact]
    public void FirstCharToUpper_Should_Work()
    {
        var text = "movies";
        var result = text.FirstCharToUpper();

        result.Should().Be("Movies");
    }
        
    [Fact]
    public void FirstCharToUpper_Should_Throw_Exception_When_Null()
    {
        var action = () => ((string)null).FirstCharToUpper();
        action.Should().Throw<ArgumentNullException>();
    }
        
    [Fact]
    public void FirstCharToUpper_Should_Throw_Exception_When_Empty()
    {
        var action = () => ((string)null).FirstCharToUpper();
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("3,2|0,5", "0.5", "3.2")]
    [InlineData("0,5|3,2", "0.5", "3.2")]
    [InlineData("3.2|0.5", "0.5", "3.2")]
    [InlineData("0.5|3.2", "0.5", "3.2")]
    public void FormatInputValue_Should_Format_Input(string input, string left, string right)
    {
        var value = input;
        var result = value.FormatInputValue();

        result.Should().NotBeNull();
        result.Length.Should().Be(2);
        result[0].Should().Be(left);
        result[1].Should().Be(right);
    }
        
    [Theory]
    [InlineData("3,2", "3.2", 1)]
    [InlineData("3,2", "6.4", 2)]
    [InlineData("", "0", 1)]
    public void FormatInputValue_Should_Return_Single_Value(string input, string resultValue, int multiplier)
    {
        var value = input;
        var result = value.FormatInputValue(multiplier);

        result.Should().NotBeNull();
        result.Length.Should().Be(1);
        result[0].Should().Be(resultValue);
    }

    [Theory]
    [InlineData("migration", 0, "migration")]
    [InlineData("migration", 1, "migration")]
    [InlineData("migration", 2, "migrations")]
    [InlineData("migration", 3, "migrations")]
    public void MakePlural_Should_Return_Correct_Value(string input, int count, string output)
    {
        var result = input.MakePlural(count);
        result.Should().Be(output);
    }
}