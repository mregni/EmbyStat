using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories;
using FluentAssertions;
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
                   var libraryOne = new Library { Id = Guid.NewGuid().ToString(), Name = "Movies", PrimaryImage = "image.png", Type = LibraryType.Movies };
                   var libraryTwo = new Library { Id = Guid.NewGuid().ToString(), Name = "Shows", PrimaryImage = "image.png", Type = LibraryType.TvShow };
                   _libraryRepository.AddOrUpdateRange(new[] { libraryOne, libraryTwo });

                   using (var database = _context.CreateDatabaseContext())
                   {
                       var collection = database.GetCollection<Library>();
                       var libraries = collection.FindAll().OrderBy(x => x.Name).ToList();

                       libraries.Should().NotContainNulls();
                       libraries.Count.Should().Be(2);

                       libraries[0].Id.Should().Be(libraryOne.Id);
                       libraries[0].Name.Should().Be(libraryOne.Name);
                       libraries[0].PrimaryImage.Should().Be(libraryOne.PrimaryImage);
                       libraries[0].Type.Should().Be(libraryOne.Type);

                       libraries[1].Id.Should().Be(libraryTwo.Id);
                       libraries[1].Name.Should().Be(libraryTwo.Name);
                       libraries[1].PrimaryImage.Should().Be(libraryTwo.PrimaryImage);
                       libraries[1].Type.Should().Be(libraryTwo.Type);
                   }
               });
        }

        [Fact]
        public void GetLibrariesByTypes_Should_Return_Only_Library_Of_Certain_Types()
        {
            RunTest(() =>
            {
                var libraryOne = new Library { Id = Guid.NewGuid().ToString(), Name = "Movies", PrimaryImage = "image.png", Type = LibraryType.Movies };
                var libraryTwo = new Library { Id = Guid.NewGuid().ToString(), Name = "Shows", PrimaryImage = "image.png", Type = LibraryType.TvShow };
                var libraryThree = new Library { Id = Guid.NewGuid().ToString(), Name = "Shows2", PrimaryImage = "image2.png", Type = LibraryType.TvShow };
                var libraryFour = new Library { Id = Guid.NewGuid().ToString(), Name = "Folder", PrimaryImage = "image2.png", Type = LibraryType.Folders };

                using (var database = _context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Library>();
                    collection.InsertBulk(new[] {libraryOne, libraryTwo, libraryThree, libraryFour });
                }

                var libraries = _libraryRepository.GetLibrariesByTypes(new[] {LibraryType.TvShow, LibraryType.Movies});
                libraries.Should().NotContainNulls();
                libraries.Count.Should().Be(3);

                libraries[0].Id.Should().Be(libraryOne.Id);
                libraries[0].Name.Should().Be(libraryOne.Name);
                libraries[0].PrimaryImage.Should().Be(libraryOne.PrimaryImage);
                libraries[0].Type.Should().Be(libraryOne.Type);

                libraries[1].Id.Should().Be(libraryTwo.Id);
                libraries[1].Name.Should().Be(libraryTwo.Name);
                libraries[1].PrimaryImage.Should().Be(libraryTwo.PrimaryImage);
                libraries[1].Type.Should().Be(libraryTwo.Type);

                libraries[2].Id.Should().Be(libraryThree.Id);
                libraries[2].Name.Should().Be(libraryThree.Name);
                libraries[2].PrimaryImage.Should().Be(libraryThree.PrimaryImage);
                libraries[2].Type.Should().Be(libraryThree.Type);
            });
        }
    }
}
