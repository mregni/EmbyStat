using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Net;

namespace EmbyStat.Clients.Base.Converters
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
