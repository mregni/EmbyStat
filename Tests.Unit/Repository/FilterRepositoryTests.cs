using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using Xunit;

namespace Tests.Unit.Repository
{
    public class FilterRepositoryTests : BaseRepositoryTester
    {
        private FilterRepository _filterRepository;
        public FilterRepositoryTests() : base("test-data-filters-repo.db")
        {

        }

        protected override void SetupRepository()
        {
            var context = CreateDbContext();
            using (var database = context.LiteDatabase)
            {
                var collection = database.GetCollection<FilterValues>("Filters");
                var filterValuesOne = new FilterValues
                {
                    Field = "subtitle",
                    Id = "10",
                    Libraries = new[] { "1", "2" },
                    Values = new[] { new LabelValuePair { Label = "1", Value = "1" }, new LabelValuePair { Label = "2", Value = "2" } }
                };

                var filterValuesTwo = new FilterValues
                {
                    Field = "subtitle",
                    Id = "11",
                    Libraries = new[] { "1" },
                    Values = new[] { new LabelValuePair { Label = "3", Value = "3" }, new LabelValuePair { Label = "4", Value = "4" } }
                };

                var filterValuesThree = new FilterValues
                {
                    Field = "container",
                    Id = "12",
                    Libraries = new [] { "1" },
                    Values = new[] { new LabelValuePair { Label = "a", Value = "a" }, new LabelValuePair { Label = "b", Value = "b" } }
                };

                collection.InsertBulk(new[] { filterValuesOne, filterValuesTwo, filterValuesThree });

            }

            _filterRepository = new FilterRepository(context);
        }

        [Fact]
        public void Get_Should_return_Correct_Subtitle_Values()
        {
            RunTest(() =>
            {
                var values = _filterRepository.Get("subtitle", new[] { "1", "2" });
                values.Should().NotBeNull();

                values.Id.Should().Be("10");
                values.Field.Should().Be("subtitle");
                values.Libraries.Length.Should().Be(2);
                values.Libraries[0].Should().Be("1");
                values.Libraries[1].Should().Be("2");
                values.Values.Length.Should().Be(2);
                values.Values[0].Label.Should().Be("1");
                values.Values[0].Value.Should().Be("1");
                values.Values[1].Label.Should().Be("2");
                values.Values[1].Value.Should().Be("2");
            });
        }

        [Fact]
        public void Get_Should_return_Null_When_No_Values_Are_Found()
        {
            RunTest(() =>
            {
                var values = _filterRepository.Get("images", new[] { "1", "2" });
                values.Should().BeNull();
            });
        }

        [Fact]
        public void Get_Should_return_Correct_Container_Values()
        {
            RunTest(() =>
            {
                var values = _filterRepository.Get("container", new[] { "1" });
                values.Should().NotBeNull();

                values.Id.Should().Be("12");
                values.Field.Should().Be("container");
                values.Libraries.Length.Should().Be(1);
                values.Libraries[0].Should().Be("1");
                values.Values.Length.Should().Be(2);
                values.Values[0].Label.Should().Be("a");
                values.Values[0].Value.Should().Be("a");
                values.Values[1].Label.Should().Be("b");
                values.Values[1].Value.Should().Be("b");
            });
        }
    }
}
