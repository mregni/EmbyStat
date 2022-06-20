using EmbyStat.Clients.Base;
using EmbyStat.Clients.Base.Metadata;
using EmbyStat.Common.Enums;

namespace EmbyStat.Clients.Tmdb;

public class TmdbClientFactory : IMetadataClientFactory
{
    private readonly ITmdbClient _client;

    public TmdbClientFactory(ITmdbClient client)
    {
        _client = client;
    }

    public IMetadataClient CreateClient()
    {
        return _client;
    }

    public bool AppliesTo(MetadataServerType type)
    {
        return type == MetadataServerType.Tmdb;
    }
}