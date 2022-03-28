using System;
using Xunit;

namespace Tests.Unit.Repository
{
    public class PersonRepositoryTests : BaseRepositoryTester
    {
        private PersonRepository _personRepository;
        private DbContext _context;

        public PersonRepositoryTests() : base("test-data-person-repo.db")
        {
        }

        protected override void SetupRepository()
        {
            _context = CreateDbContext();
            _personRepository = new PersonRepository(_context);
        }

        [Fact]
        public void Insert_Should_Insert_A_Person_Into_The_Database()
        {
            RunTest(() =>
            {
                var person = new Person{ Id = Guid.NewGuid().ToString(), Name = "reggi", IMDB = "t0000001"};
                _personRepository.Upsert(person);

                using var database = _context.LiteDatabase;
                var collection = database.GetCollection<Person>();
                var people = collection.FindAll().ToList();

                people.Should().NotContainNulls();
                people.Count.Should().Be(1);

                people[0].Id.Should().Be(person.Id);
                people[0].Name.Should().Be(person.Name);
                people[0].IMDB.Should().Be(person.IMDB);
            });
        }

        [Fact]
        public void GetPersonByName_Should_Return_Correct_Person()
        {
            RunTest(() =>
            {
                var personOne = new Person { Id = Guid.NewGuid().ToString(), Name = "reggi", IMDB = "t0000001" };
                var personTwo = new Person { Id = Guid.NewGuid().ToString(), Name = "tom", IMDB = "t0000002" };

                using (var database = _context.LiteDatabase)
                {
                    var collection = database.GetCollection<Person>();
                    collection.InsertBulk(new[] {personOne, personTwo});
                }

                var person = _personRepository.GetPersonByName("reggi");
                person.Should().NotBeNull();
                person.Id.Should().Be(person.Id);
                person.Name.Should().Be(person.Name);
                person.IMDB.Should().Be(person.IMDB);
            });
        }
    }
}
