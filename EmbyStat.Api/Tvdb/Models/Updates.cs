using System.Collections.Generic;

namespace EmbyStat.Api.Tvdb.Models
{
    public class Updates
    {
        public List<Update> Data { get; set; }
    }

    public class Update
    {
        public int Id { get; set; }
        public int LastUpdated { get; set; }
    }
}
