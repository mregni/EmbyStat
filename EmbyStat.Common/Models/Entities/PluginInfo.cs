using LiteDB;

namespace EmbyStat.Common.Models.Entities
{
    public class PluginInfo
    {
        [BsonId]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string ConfigurationFileName { get; set; }
        public string Description { get; set; }
        public string ImageTag { get; set; }
    }
}
