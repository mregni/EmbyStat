using System;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using Xunit;

namespace Tests.Unit.Repository
{
    public class LibraryRepositoryTests : BaseRepositoryTester
    {
        private LibraryRepository _libraryRepository;
        private DbContext _context;

        public LibraryRepositoryTests() : base("test-data-library-repo.db")
        {

        }

        protected override void SetupRepository()
        {
            _context = CreateDbContext();
            _libraryRepository = new LibraryRepository(_context);
        }

        [Fact]
        public void AddOrUpdateRange_Should_Add_New_Libraries()
        {
            RunTest(() =>
               {
                   var libraryOne = new Library { Id = Guid.NewGuid().ToString(), Name = "Movies", Primary = "image.png", Type = LibraryType.Movies };
                   var libraryTwo = new Library { Id = Guid.NewGuid().ToString(), Name = "Shows", Primary = "image.png", Type = LibraryType.TvShow };
                   _libraryRepository.AddOrUpdateRange(new[] { libraryOne, libraryTwo });

                   using var database = _context.LiteDatabase;
                   var collection = database.GetCollection<Library>();
                   var libraries = collection.FindAll().OrderBy(x => x.Name).ToList();

                   libraries.Should().NotContainNulls();
                   libraries.Count.Should().Be(2);

                   libraries[0].Id.Should().Be(libraryOne.Id);
                   libraries[0].Name.Should().Be(libraryOne.Name);
                   libraries[0].Primary.Should().Be(libraryOne.Primary);
                   libraries[0].Type.Should().Be(libraryOne.Type);

                   libraries[1].Id.Should().Be(libraryTwo.Id);
                   libraries[1].Name.Should().Be(libraryTwo.Name);
                   libraries[1].Primary.Should().Be(libraryTwo.Primary);
                   libraries[1].Type.Should().Be(libraryTwo.Type);
               });
        }

        [Fact]
        public void GetLibrariesByTypes_Should_Return_Only_Library_Of_Certain_Types()
        {
            RunTest(() =>
            {
                var libraryOne = new Library { Id = Guid.NewGuid().ToString(), Name = "Movies", Primary = "image.png", Type = LibraryType.Movies };
                var libraryTwo = new Library { Id = Guid.NewGuid().ToString(), Name = "Shows", Primary = "image.png", Type = LibraryType.TvShow };
                var libraryThree = new Library { Id = Guid.NewGuid().ToString(), Name = "Shows2", Primary = "image2.png", Type = LibraryType.TvShow };
                var libraryFour = new Library { Id = Guid.NewGuid().ToString(), Name = "Folder", Primary = "image2.png", Type = LibraryType.Folders };

                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<Library>();
                    collection.InsertBulk(new[] { libraryOne, libraryTwo, libraryThree, libraryFour });
                }

                var libraries = _libraryRepository.GetLibrariesById(new[] { libraryOne.Id, libraryTwo.Id, libraryThree.Id });
                libraries.Should().NotContainNulls();
                libraries.Count.Should().Be(3);

                libraries[0].Id.Should().Be(libraryOne.Id);
                libraries[0].Name.Should().Be(libraryOne.Name);
                libraries[0].Primary.Should().Be(libraryOne.Primary);
                libraries[0].Type.Should().Be(libraryOne.Type);

                libraries[1].Id.Should().Be(libraryTwo.Id);
                libraries[1].Name.Should().Be(libraryTwo.Name);
                libraries[1].Primary.Should().Be(libraryTwo.Primary);
                libraries[1].Type.Should().Be(libraryTwo.Type);

                libraries[2].Id.Should().Be(libraryThree.Id);
                libraries[2].Name.Should().Be(libraryThree.Name);
                libraries[2].Primary.Should().Be(libraryThree.Primary);
                libraries[2].Type.Should().Be(libraryThree.Type);
            });
        }
    }
}
