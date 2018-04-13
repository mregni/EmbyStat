using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Api.EmbyClient.Model
{
    public class PersonsQuery : ItemsByNameQuery
    {
        public string[] PersonTypes { get; set; }
    }
}
