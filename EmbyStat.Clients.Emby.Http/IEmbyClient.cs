using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Clients.Emby.Http.Model;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Querying;
using MediaBrowser.Model.Users;
using Newtonsoft.Json.Linq;
using ServerInfo = EmbyStat.Common.Models.Entities.ServerInfo;

namespace EmbyStat.Clients.Emby.Http
{
    public interface IEmbyClient : IDisposable
    {
        void SetDeviceInfo(string clientName, string authorizationScheme, string applicationVersion, string deviceId);
        void SetAddressAndUser(string url, string token, string userId);
		Task<AuthenticationResult> AuthenticateUserAsync(string username, string password, string address);
		Task<List<PluginInfo>> GetInstalledPluginsAsync();
		Task<ServerInfo> GetServerInfoAsync();
	    Task<List<FileSystemEntryInfo>> GetLocalDrivesAsync();
        Task<JArray> GetEmbyUsersAsync();
        Task<JObject> GetEmbyDevicesAsync();
        Task<string> PingEmbyAsync(CancellationToken cancellationToken);
        Task<QueryResult<BaseItemDto>> GetItemsAsync(ItemQuery query, CancellationToken cancellationToken = default(CancellationToken));
        Task<BaseItemDto> GetPersonByNameAsync(string personName, CancellationToken cancellationToken);
        Task<QueryResult<BaseItemDto>> GetPeopleAsync(PersonsQuery query, CancellationToken cancellationToken = default(CancellationToken));
        Task<QueryResult<BaseItemDto>> GetGenresAsync(ItemsByNameQuery query, CancellationToken cancellationToken = default(CancellationToken));
        Task<QueryResult<BaseItemDto>> GetMediaFoldersAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
