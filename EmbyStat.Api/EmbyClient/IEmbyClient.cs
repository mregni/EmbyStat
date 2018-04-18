using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Api.EmbyClient.Model;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Querying;
using MediaBrowser.Model.System;
using MediaBrowser.Model.Users;

namespace EmbyStat.Api.EmbyClient
{
    public interface IEmbyClient : IDisposable
    {
	    void SetAddressAndUrl(string url, string token);
		Task<AuthenticationResult> AuthenticateUserAsync(string username, string password, string address);
		Task<List<PluginInfo>> GetInstalledPluginsAsync();
		Task<SystemInfo> GetServerInfoAsync();
	    Task<List<Drive>> GetLocalDrivesAsync();
	    Task<string> PingEmbyAsync();
        Task<QueryResult<BaseItemDto>> GetItemsAsync(ItemQuery query, CancellationToken cancellationToken = default(CancellationToken));
        Task<Folder> GetRootFolderAsync(string userId, CancellationToken cancellationToken = default(CancellationToken));
        Task<QueryResult<BaseItemDto>> GetPeopleAsync(PersonsQuery query, CancellationToken cancellationToken = default(CancellationToken));
        Task<QueryResult<BaseItemDto>> GetGenresAsync(ItemsByNameQuery query, CancellationToken cancellationToken = default(CancellationToken));
    }
}
