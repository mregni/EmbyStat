using System.Linq;
using EmbyStat.Common.Models.Entities;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;

namespace EmbyStat.Common.Converters
{
    public static class PersonHelper
    {
        public static Person ConvertToPerson(BaseItemDto person)
        {
            return new Person
            {
                Id = person.Id,
                Name = person.Name,
                Primary = person.ImageTags?.FirstOrDefault(y => y.Key == ImageType.Primary).Value,
                MovieCount = person.MovieCount ?? 0,
                BirthDate = person.PremiereDate,
                seriesCount = person.SeriesCount ?? 0,
                Etag = person.Etag,
                IMDB = person.ProviderIds?.FirstOrDefault(y => y.Key == "Imdb").Value,
                TMDB = person.ProviderIds?.FirstOrDefault(y => y.Key == "Tmdb").Value,
                OverView = person.Overview,
                SeriesCount = person.SeriesCount ?? 0,
                SortName = person.SortName,
                Synced = true
            };
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
