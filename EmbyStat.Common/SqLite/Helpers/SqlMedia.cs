using System;

namespace EmbyStat.Common.SqLite.Helpers
{
    public abstract class SqlMedia
    {
        public string Id { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Banner { get; set; }
        public string Logo { get; set; }
        public string Primary { get; set; }
        public string Thumb { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime? PremiereDate { get; set; }
        public int? ProductionYear { get; set; }
        public string SortName { get; set; }
        public string CollectionId { get; set; }
    }
}
