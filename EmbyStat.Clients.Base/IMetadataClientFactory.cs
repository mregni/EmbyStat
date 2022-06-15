using EmbyStat.Clients.Base.Metadata;
using EmbyStat.Common.Enums;

namespace EmbyStat.Clients.Base;

public interface IMetadataClientFactory
{
    IMetadataClient CreateClient();
    bool AppliesTo(MetadataServerType type);
}