using EmbyStat.Clients.Base;
using EmbyStat.Clients.Base.Metadata;
using EmbyStat.Common.Enums;

namespace EmbyStat.Clients.TvMaze;

public class TvMazeClientFactory : IMetadataClientFactory
{
    private readonly ITvMazeShowClient _showClient;

    public TvMazeClientFactory(ITvMazeShowClient showClient)
    {
        _showClient = showClient;
    }
    
    public IMetadataClient CreateClient()
    {
        return _showClient;
    }

    public bool AppliesTo(MetadataServerType type)
    {
        return type == MetadataServerType.TvMaze;
    }
}