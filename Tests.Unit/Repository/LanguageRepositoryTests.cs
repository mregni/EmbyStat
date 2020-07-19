using System.Linq;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories;
using FluentAssertions;
using Xunit;

namespace Tests.Unit.Repository
{
    public class LanguageRepositoryTests : BaseRepositoryTester
    {
        private LanguageRepository _languageRepository;
        public LanguageRepositoryTests() : base("test-data-language-repo.db")
        {
        }

        protected override void SetupRepository()
        {
            var context = CreateDbContext();
            using (var database = context.LiteDatabase)
            {
                var collection = database.GetCollection<Language>();
                var languageOne = new Language { Code = "BE", Id = "01", Name = "Dutch" };
                var languageTwo = new Language { Code = "EN", Id = "02", Name = "English" };

                collection.InsertBulk(new[] { languageOne, languageTwo });

            }

            _languageRepository = new LanguageRepository(context);
        }

        [Fact]
        public void GetLanguages_Should_Return_All_Languages()
        {
            RunTest(() =>
            {
                var languages = _languageRepository.GetLanguages().ToList();
                languages.Should().NotContainNulls();
                languages.Count.Should().Be(2);

                languages[0].Id.Should().Be("01");
                languages[0].Code.Should().Be("BE");
                languages[0].Name.Should().Be("Dutch");

                languages[1].Id.Should().Be("02");
                languages[1].Code.Should().Be("EN");
                languages[1].Name.Should().Be("English");
            });
        }
    }
}
