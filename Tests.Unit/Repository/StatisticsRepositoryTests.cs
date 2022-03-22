using System;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories;
using FluentAssertions;
using Xunit;

namespace Tests.Unit.Repository
{
    public class StatisticsRepositoryTests : BaseRepositoryTester
    {
        private StatisticsRepository _statisticsRepository;
        private DbContext _context;
        public StatisticsRepositoryTests() : base("test-data-stat-repo.db")
        {
        }

        protected override void SetupRepository()
        {
            _context = CreateDbContext();
            _statisticsRepository = new StatisticsRepository(_context);
        }

        [Fact]
        public void GetLastResultByType_Should_Return_Last_Valid_Statistics_For_Type()
        {
           RunTest(() =>
           {
               var statisticOne = new Statistic { Id = Guid.NewGuid(), Type = StatisticType.Movie, CalculationDateTime = DateTime.Now, JsonResult = "", IsValid = false, CollectionIds = Array.Empty<string>() };
               var statisticTwo = new Statistic { Id = Guid.NewGuid(), Type = StatisticType.Movie, CalculationDateTime = DateTime.Now, JsonResult = "", IsValid = true, CollectionIds = Array.Empty<string>() };
               var statisticThree = new Statistic { Id = Guid.NewGuid(), Type = StatisticType.Show, CalculationDateTime = DateTime.Now, JsonResult = "", IsValid = true, CollectionIds = Array.Empty<string>() };
               var statisticFour = new Statistic { Id = Guid.NewGuid(), Type = StatisticType.Movie, CalculationDateTime = DateTime.Now, JsonResult = "", IsValid = true, CollectionIds = new[] { "1" } };

               using (var database = _context.LiteDatabase)
               {
                   var collection = database.GetCollection<Statistic>();
                   collection.InsertBulk(new[] { statisticOne, statisticTwo, statisticThree, statisticFour });
               }

               var statistic = _statisticsRepository.GetLastResultByType(StatisticType.Movie, Array.Empty<string>());
               statistic.Should().NotBeNull();
               statistic.Id.Should().Be(statisticTwo.Id);
               statistic.CalculationDateTime.Should().BeCloseTo(statisticTwo.CalculationDateTime, new TimeSpan(0, 0, 1));
               statistic.CollectionIds.Should().BeEquivalentTo(statisticTwo.CollectionIds);
               statistic.IsValid.Should().BeTrue();
               statistic.Type.Should().Be(statisticTwo.Type);
           });
        }

        [Fact]
        public void AddStatistic_Should_Add_New_Statistic()
        {
            RunTest(() =>
            {
                _statisticsRepository.AddStatistic("statistics", DateTime.Now, StatisticType.Movie, Array.Empty<string>());

                using var database = _context.LiteDatabase;
                var collection = database.GetCollection<Statistic>();
                var statistics = collection.FindAll().ToList();

                statistics.Should().NotContainNulls();
                statistics.Count.Should().Be(1);

                statistics[0].IsValid.Should().BeTrue();
                statistics[0].CalculationDateTime.Should().BeCloseTo(DateTime.Now, new TimeSpan(0,0,1));
                statistics[0].CollectionIds.Count().Should().Be(0);
                statistics[0].JsonResult.Should().Be("statistics");
                statistics[0].Type.Should().Be(StatisticType.Movie);
            });
        }

        [Fact]
        public void AddStatistic_Should_Add_New_Statistic_And_Disable_Previous_One()
        {
            RunTest(() =>
            {
                _statisticsRepository.AddStatistic("statistics", DateTime.Now, StatisticType.Movie, Array.Empty<string>());
                _statisticsRepository.AddStatistic("statistics", DateTime.Now.AddDays(1), StatisticType.Movie, Array.Empty<string>());

                using var database = _context.LiteDatabase;
                var collection = database.GetCollection<Statistic>();
                var statistics = collection.FindAll().OrderBy(x => x.CalculationDateTime).ToList();

                statistics.Should().NotContainNulls();
                statistics.Count.Should().Be(2);

                statistics[0].IsValid.Should().BeFalse();
                statistics[0].CalculationDateTime.Should().BeCloseTo(DateTime.Now, new TimeSpan(0, 0, 2));
                statistics[0].CollectionIds.Count().Should().Be(0);
                statistics[0].JsonResult.Should().Be("statistics");
                statistics[0].Type.Should().Be(StatisticType.Movie);

                statistics[1].IsValid.Should().BeTrue();
                statistics[1].CalculationDateTime.Should().BeCloseTo(DateTime.Now.AddDays(1), new TimeSpan(0, 0, 2));
                statistics[1].CollectionIds.Count().Should().Be(0);
                statistics[1].JsonResult.Should().Be("statistics");
                statistics[1].Type.Should().Be(StatisticType.Movie);
            });
        }

        [Fact]
        public void CleanupStatistics_Should_Delete_All_Invalid_Statistics()
        {
            RunTest(() =>
            {
                var statisticOne = new Statistic { Id = Guid.NewGuid(), Type = StatisticType.Movie, CalculationDateTime = DateTime.Now.AddDays(-1), JsonResult = "", IsValid = false, CollectionIds = Array.Empty<string>() };
                var statisticTwo = new Statistic { Id = Guid.NewGuid(), Type = StatisticType.Movie, CalculationDateTime = DateTime.Now, JsonResult = "", IsValid = true, CollectionIds = Array.Empty<string>() };
                var statisticThree = new Statistic { Id = Guid.NewGuid(), Type = StatisticType.Show, CalculationDateTime = DateTime.Now.AddDays(1), JsonResult = "", IsValid = true, CollectionIds = Array.Empty<string>() };
                var statisticFour = new Statistic { Id = Guid.NewGuid(), Type = StatisticType.Movie, CalculationDateTime = DateTime.Now.AddDays(2), JsonResult = "", IsValid = true, CollectionIds = new[] { "1" } };

                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<Statistic>();
                    collection.InsertBulk(new[] { statisticOne, statisticTwo, statisticThree, statisticFour });
                    var statistics = collection.FindAll().ToList();

                    statistics.Count.Should().Be(4);
                }

                _statisticsRepository.DeleteStatistics();

                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<Statistic>();
                    var statistics = collection.FindAll().OrderBy(x => x.CalculationDateTime).ToList();

                    statistics.Count.Should().Be(3);
                    statistics[0].Should().NotBeNull();
                    statistics[0].Id.Should().Be(statisticTwo.Id);
                    statistics[0].CalculationDateTime.Should().BeCloseTo(statisticTwo.CalculationDateTime, new TimeSpan(0, 0, 1));
                    statistics[0].CollectionIds.Should().BeEquivalentTo(statisticTwo.CollectionIds);
                    statistics[0].IsValid.Should().BeTrue();
                    statistics[0].Type.Should().Be(statisticTwo.Type);

                    statistics[1].Should().NotBeNull();
                    statistics[1].Id.Should().Be(statisticThree.Id);
                    statistics[1].CalculationDateTime.Should().BeCloseTo(statisticThree.CalculationDateTime, new TimeSpan(0, 0, 1));
                    statistics[1].CollectionIds.Should().BeEquivalentTo(statisticThree.CollectionIds);
                    statistics[1].IsValid.Should().BeTrue();
                    statistics[1].Type.Should().Be(statisticThree.Type);

                    statistics[2].Should().NotBeNull();
                    statistics[2].Id.Should().Be(statisticFour.Id);
                    statistics[2].CalculationDateTime.Should().BeCloseTo(statisticFour.CalculationDateTime, new TimeSpan(0, 0, 1));
                    statistics[2].CollectionIds.Should().BeEquivalentTo(statisticFour.CollectionIds);
                    statistics[2].IsValid.Should().BeTrue();
                    statistics[2].Type.Should().Be(statisticFour.Type);
                }
            });
        }

        [Fact]
        public void MarkMovieTypesAsInvalid_Should_Invalidate_All_Statistics_With_Type_Movie()
        {
            RunTest(() =>
            {
                var statisticOne = new Statistic { Id = Guid.NewGuid(), Type = StatisticType.Movie, CalculationDateTime = DateTime.Now.AddDays(-1), JsonResult = "", IsValid = false, CollectionIds = Array.Empty<string>() };
                var statisticTwo = new Statistic { Id = Guid.NewGuid(), Type = StatisticType.Movie, CalculationDateTime = DateTime.Now, JsonResult = "", IsValid = true, CollectionIds = Array.Empty<string>() };
                var statisticThree = new Statistic { Id = Guid.NewGuid(), Type = StatisticType.Show, CalculationDateTime = DateTime.Now.AddDays(1), JsonResult = "", IsValid = true, CollectionIds = Array.Empty<string>() };
                var statisticFour = new Statistic { Id = Guid.NewGuid(), Type = StatisticType.Movie, CalculationDateTime = DateTime.Now.AddDays(2), JsonResult = "", IsValid = true, CollectionIds = new[] { "1" } };

                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<Statistic>();
                    collection.InsertBulk(new[] { statisticOne, statisticTwo, statisticThree, statisticFour });
                    var statistics = collection.FindAll().ToList();

                    statistics.Count.Should().Be(4);
                }

                _statisticsRepository.MarkMovieTypesAsInvalid();

                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<Statistic>();
                    var statistics = collection.FindAll().OrderBy(x => x.CalculationDateTime).ToList();

                    statistics.Count.Should().Be(4);
                    statistics.Count(x => x.Type == StatisticType.Movie && !x.IsValid).Should().Be(3);
                    statistics.Count(x => x.Type == StatisticType.Show && x.IsValid).Should().Be(1);
                }
            });
        }

        [Fact]
        public void MarkShowTypesAsInvalid_Should_Invalidate_All_Statistics_With_Type_Show()
        {
            RunTest(() =>
            {
                var statisticOne = new Statistic { Id = Guid.NewGuid(), Type = StatisticType.Show, CalculationDateTime = DateTime.Now.AddDays(-1), JsonResult = "", IsValid = false, CollectionIds = Array.Empty<string>() };
                var statisticTwo = new Statistic { Id = Guid.NewGuid(), Type = StatisticType.Show, CalculationDateTime = DateTime.Now, JsonResult = "", IsValid = true, CollectionIds = Array.Empty<string>() };
                var statisticThree = new Statistic { Id = Guid.NewGuid(), Type = StatisticType.Movie, CalculationDateTime = DateTime.Now.AddDays(1), JsonResult = "", IsValid = true, CollectionIds = Array.Empty<string>() };
                var statisticFour = new Statistic { Id = Guid.NewGuid(), Type = StatisticType.Show, CalculationDateTime = DateTime.Now.AddDays(2), JsonResult = "", IsValid = true, CollectionIds = new[] { "1" } };

                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<Statistic>();
                    collection.InsertBulk(new[] { statisticOne, statisticTwo, statisticThree, statisticFour });
                    var statistics = collection.FindAll().ToList();

                    statistics.Count.Should().Be(4);
                }

                _statisticsRepository.MarkShowTypesAsInvalid();

                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<Statistic>();
                    var statistics = collection.FindAll().OrderBy(x => x.CalculationDateTime).ToList();

                    statistics.Count.Should().Be(4);
                    statistics.Count(x => x.Type == StatisticType.Show && !x.IsValid).Should().Be(3);
                    statistics.Count(x => x.Type == StatisticType.Movie && x.IsValid).Should().Be(1);
                }
            });
        }
    }
}
