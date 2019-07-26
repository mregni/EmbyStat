using System.Linq;
using EmbyStat.Common.Models.Entities;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;

namespace EmbyStat.Common.Converters
{
    public static class PersonConverter
    {
        public static Person UpdatePerson(Person person, BaseItemDto newPerson)
        {
            person.Name = newPerson.Name;
            person.Primary = newPerson.ImageTags?.FirstOrDefault(y => y.Key == ImageType.Primary).Value;
            person.BirthDate = newPerson.PremiereDate;
            person.Etag = newPerson.Etag;
            person.IMDB = newPerson.ProviderIds?.FirstOrDefault(y => y.Key == "Imdb").Value;
            person.TMDB = newPerson.ProviderIds?.FirstOrDefault(y => y.Key == "Tmdb").Value;
            person.OverView = newPerson.Overview;
            person.SortName = newPerson.SortName;
            person.Synced = true;

            return person;
        }

        public static Person ConvertToSmallPerson(BaseItemDto person)
        {
            return new Person
            {
                Id = person.Id,
                Name = person.Name,
                Synced = false
            };
        }
    }
}
