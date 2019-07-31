using System.Linq;
using EmbyStat.Common.Models.Entities;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;

namespace EmbyStat.Common.Converters
{
    public static class PersonConverter
    {
        public static Person Convert(BaseItemDto person)
        {
            return new Person
            {
                Id = person.Id,
                Name = person.Name,
                Primary = person.ImageTags?.FirstOrDefault(y => y.Key == ImageType.Primary).Value,
                BirthDate = person.PremiereDate,
                Etag = person.Etag,
                IMDB = person.ProviderIds?.FirstOrDefault(y => y.Key == "Imdb").Value,
                TMDB = person.ProviderIds?.FirstOrDefault(y => y.Key == "Tmdb").Value,
                OverView = person.Overview,
                SortName = person.SortName
            };
        }
    }
}
