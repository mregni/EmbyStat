using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Show;

namespace EmbyStat.Clients.Base.Metadata;

public interface IMetadataClient
{
    Task<IEnumerable<VirtualEpisode>> GetEpisodesAsync(int? id);
}