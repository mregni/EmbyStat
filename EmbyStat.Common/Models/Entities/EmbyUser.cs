namespace EmbyStat.Common.Models.Entities
{
    public class EmbyUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ServerId { get; set; }
        public bool IsAdministrator { get; set; }
        public bool Deleted { get; set; }
    }
}
