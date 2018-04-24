using EmbyStat.Common.Models;
using MediaBrowser.Model.Dto;

namespace EmbyStat.Services.Converters
{
    public static class PersonHelper
    {
        public static Person ConvertToPerson(BaseItemDto person)
        {
            return new Person
            {
                Id = person.Id,
                Name = person.Name
            };
        }
    }
}
