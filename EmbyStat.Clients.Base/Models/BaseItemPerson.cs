using EmbyStat.Common.Enums;

namespace EmbyStat.Clients.Base.Models
{
    public class BaseItemPerson
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public PersonType Type { get; set; }
    }
}
