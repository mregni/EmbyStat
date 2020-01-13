using System.IO;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Extensions;
using FluentAssertions;
using Xunit;

namespace Tests.Unit.Extensions
{
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
    }
}
