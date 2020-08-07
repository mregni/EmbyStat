using EmbyStat.Common.Enums;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests.Unit.Services
{
    public class FilterServiceTests
    {
        private readonly Mock<IMovieRepository> _movieRepositoryMock;
        private readonly Mock<IFilterRepository> _filterRepositoryMock;

        private FilterValues _filterValues;

        public FilterServiceTests()
        {
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _filterRepositoryMock = new Mock<IFilterRepository>();
        }

        private void SetupMovieRepository(string[] libraryIds)
        {
            _movieRepositoryMock
                .Setup(x => x.CalculateSubtitleFilterValues(libraryIds))
                .Returns(new[] { new LabelValuePair { Label = "6", Value = "6" }, new LabelValuePair { Label = "7", Value = "7" } });
            _movieRepositoryMock
                .Setup(x => x.CalculateGenreFilterValues(libraryIds))
                .Returns(new[] { new LabelValuePair { Label = "8", Value = "8" }, new LabelValuePair { Label = "9", Value = "9" } });
            _movieRepositoryMock
                .Setup(x => x.CalculateContainerFilterValues(libraryIds))
                .Returns(new[] { new LabelValuePair { Label = "10", Value = "10" }, new LabelValuePair { Label = "11", Value = "11" } });
            _movieRepositoryMock
                .Setup(x => x.CalculateCollectionFilterValues())
                .Returns(new[] { new LabelValuePair { Label = "12", Value = "12" }, new LabelValuePair { Label = "13", Value = "13" } });
        }

        private FilterService CreateFilterService(LibraryType type, string field, string[] libraryIds)
        {
            _filterValues = new FilterValues
            {
                Id = "1",
                Field = field,
                Values = new[] { new LabelValuePair { Label = "2", Value = "2" }, new LabelValuePair { Label = "3", Value = "3" } },
                Libraries = libraryIds
            };

            _filterRepositoryMock
                .Setup(x => x.Get(field, libraryIds))
                .Returns(_filterValues);
            _filterRepositoryMock
                .Setup(x => x.Insert(It.IsAny<FilterValues>()));

            return new FilterService(_filterRepositoryMock.Object, _movieRepositoryMock.Object);
        }

        [Fact]
        public void Get_Should_Return_Existing_FilterValues()
        {
            var collectionIds = new[] { "4", "5" };
            var filterService = CreateFilterService(LibraryType.Movies, "subtitle", collectionIds);
            var values = filterService.GetFilterValues(LibraryType.Movies, "subtitle", collectionIds);
            values.Should().NotBeNull();

            values.Id.Should().Be(_filterValues.Id);
            values.Field.Should().Be(_filterValues.Field);
            values.Values.Length.Should().Be(2);
            values.Values[0].Label.Should().Be("2");
            values.Values[0].Value.Should().Be("2");
            values.Values[1].Label.Should().Be("3");
            values.Values[1].Value.Should().Be("3");
            values.Libraries.Length.Should().Be(2);
            values.Libraries[0].Should().Be("4");
            values.Libraries[1].Should().Be("5");

            _filterRepositoryMock.Verify(x => x.Get("subtitle", collectionIds), Times.Once);
        }

        [Fact]
        public void Get_Should_Calculate_Missing_Subtitle_FilterValues()
        {
            var collectionIds = new[] { "4", "5" };
            var usedCollectionIds = new[] { "3" };
            SetupMovieRepository(usedCollectionIds);
            var filterService = CreateFilterService(LibraryType.Movies, "subtitle", collectionIds);

            var values = filterService.GetFilterValues(LibraryType.Movies, "subtitle", usedCollectionIds);

            values.Field.Should().Be(_filterValues.Field);
            values.Values.Length.Should().Be(2);
            values.Values[0].Label.Should().Be("6");
            values.Values[0].Value.Should().Be("6");
            values.Values[1].Label.Should().Be("7");
            values.Values[1].Value.Should().Be("7");
            values.Libraries.Length.Should().Be(1);
            values.Libraries[0].Should().Be("3");

            _filterRepositoryMock.Verify(x => x.Insert(It.IsAny<FilterValues>()));
            _movieRepositoryMock.Verify(x => x.CalculateSubtitleFilterValues(usedCollectionIds), Times.Once);
            _movieRepositoryMock.Verify(x => x.CalculateGenreFilterValues(usedCollectionIds), Times.Never);
            _movieRepositoryMock.Verify(x => x.CalculateContainerFilterValues(usedCollectionIds), Times.Never);
            _movieRepositoryMock.Verify(x => x.CalculateCollectionFilterValues(), Times.Never);
        }

        [Fact]
        public void Get_Should_Calculate_Missing_Genre_FilterValues()
        {
            var collectionIds = new[] { "4", "5" };
            var usedCollectionIds = new[] { "3" };
            SetupMovieRepository(usedCollectionIds);
            var filterService = CreateFilterService(LibraryType.Movies, "genre", collectionIds);

            var values = filterService.GetFilterValues(LibraryType.Movies, "genre", usedCollectionIds);

            values.Field.Should().Be(_filterValues.Field);
            values.Values.Length.Should().Be(2);
            values.Values[0].Value.Should().Be("8");
            values.Values[0].Label.Should().Be("8");
            values.Values[1].Value.Should().Be("9");
            values.Values[1].Label.Should().Be("9");
            values.Libraries.Length.Should().Be(1);
            values.Libraries[0].Should().Be("3");

            _filterRepositoryMock.Verify(x => x.Insert(It.IsAny<FilterValues>()));
            _movieRepositoryMock.Verify(x => x.CalculateSubtitleFilterValues(usedCollectionIds), Times.Never);
            _movieRepositoryMock.Verify(x => x.CalculateGenreFilterValues(usedCollectionIds), Times.Once);
            _movieRepositoryMock.Verify(x => x.CalculateContainerFilterValues(usedCollectionIds), Times.Never);
            _movieRepositoryMock.Verify(x => x.CalculateCollectionFilterValues(), Times.Never);
        }

        [Fact]
        public void Get_Should_Calculate_Missing_Container_FilterValues()
        {
            var collectionIds = new[] { "4", "5" };
            var usedCollectionIds = new[] { "3" };
            SetupMovieRepository(usedCollectionIds);
            var filterService = CreateFilterService(LibraryType.Movies, "container", collectionIds);

            var values = filterService.GetFilterValues(LibraryType.Movies, "container", usedCollectionIds);

            values.Field.Should().Be(_filterValues.Field);
            values.Values.Length.Should().Be(2);
            values.Values[0].Label.Should().Be("10");
            values.Values[0].Value.Should().Be("10");
            values.Values[1].Label.Should().Be("11");
            values.Values[1].Value.Should().Be("11");
            values.Libraries.Length.Should().Be(1);
            values.Libraries[0].Should().Be("3");

            _filterRepositoryMock.Verify(x => x.Insert(It.IsAny<FilterValues>()));
            _movieRepositoryMock.Verify(x => x.CalculateSubtitleFilterValues(usedCollectionIds), Times.Never);
            _movieRepositoryMock.Verify(x => x.CalculateGenreFilterValues(usedCollectionIds), Times.Never);
            _movieRepositoryMock.Verify(x => x.CalculateContainerFilterValues(usedCollectionIds), Times.Once);
            _movieRepositoryMock.Verify(x => x.CalculateCollectionFilterValues(), Times.Never);
        }

        [Fact]
        public void Get_Should_Return_Null_If_Unknown_Field_Is_Given()
        {
            var collectionIds = new[] { "4", "5" };
            var usedCollectionIds = new[] { "3" };
            SetupMovieRepository(usedCollectionIds);
            var filterService = CreateFilterService(LibraryType.Movies, "strangeField", collectionIds);

            var values = filterService.GetFilterValues(LibraryType.Movies, "strangeField", usedCollectionIds);
            values.Should().BeNull();

            _filterRepositoryMock.Verify(x => x.Insert(It.IsAny<FilterValues>()), Times.Never);
            _movieRepositoryMock.Verify(x => x.CalculateSubtitleFilterValues(usedCollectionIds), Times.Never);
            _movieRepositoryMock.Verify(x => x.CalculateGenreFilterValues(usedCollectionIds), Times.Never);
            _movieRepositoryMock.Verify(x => x.CalculateContainerFilterValues(usedCollectionIds), Times.Never);
            _movieRepositoryMock.Verify(x => x.CalculateCollectionFilterValues(), Times.Never);
        }

        [Fact]
        public void Get_Should_Calculate_Missing_Collection_FilterValues()
        {
            var collectionIds = new[] { "4", "5" };
            var usedCollectionIds = new[] { "3" };
            SetupMovieRepository(usedCollectionIds);
            var filterService = CreateFilterService(LibraryType.Movies, "collection", collectionIds);

            var values = filterService.GetFilterValues(LibraryType.Movies, "collection", usedCollectionIds);

            values.Field.Should().Be(_filterValues.Field);
            values.Values.Length.Should().Be(2);
            values.Values[0].Label.Should().Be("12");
            values.Values[0].Value.Should().Be("12");
            values.Values[1].Label.Should().Be("13");
            values.Values[1].Value.Should().Be("13");
            values.Libraries.Length.Should().Be(1);
            values.Libraries[0].Should().Be("3");

            _filterRepositoryMock.Verify(x => x.Insert(It.IsAny<FilterValues>()));
            _movieRepositoryMock.Verify(x => x.CalculateSubtitleFilterValues(usedCollectionIds), Times.Never);
            _movieRepositoryMock.Verify(x => x.CalculateGenreFilterValues(usedCollectionIds), Times.Never);
            _movieRepositoryMock.Verify(x => x.CalculateContainerFilterValues(usedCollectionIds), Times.Never);
            _movieRepositoryMock.Verify(x => x.CalculateCollectionFilterValues(), Times.Once);
        }

        [Fact]
        public void Get_With_Show_Libraries_Should_Return_Null_For_Now()
        {
            var collectionIds = new[] { "4", "5" };
            var usedCollectionIds = new string[0];
            var filterService = CreateFilterService(LibraryType.Movies, "subtitle", collectionIds);
            var values = filterService.GetFilterValues(LibraryType.TvShow, "subtitle", usedCollectionIds);
            values.Should().BeNull();

            _filterRepositoryMock.Verify(x => x.Get("subtitle", usedCollectionIds), Times.Once);
        }
    }
}
