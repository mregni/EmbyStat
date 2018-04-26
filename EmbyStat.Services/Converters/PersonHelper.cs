using System.Linq;
using EmbyStat.Common.Models;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;

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
