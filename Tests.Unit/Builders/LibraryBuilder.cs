using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;

namespace Tests.Unit.Builders
{
    public class LibraryBuilder
    {
        private readonly Library _library;

        public LibraryBuilder(int index, LibraryType type)
        {
            _library = new Library
            {
                Id = $"id{index}",
                Name = $"collection{index}",
                PrimaryImage = $"image{index}",
                Type = type
            };
        }

        public Library Build()
        {
            return _library;
        }
    }
}
