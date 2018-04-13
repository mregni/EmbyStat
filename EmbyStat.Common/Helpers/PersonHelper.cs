using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Common.Models;
using MediaBrowser.Model.Dto;

namespace EmbyStat.Common.Helpers
{
    public static class PersonHelper
    {
        public static Person ConvertToPerson(BaseItemDto person)
        {
            return new Person
            {
                Id = person.Id,
                Name = person.Name,
                Type = person.Type
            };
        }
    }
}
