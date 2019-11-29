using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Clients.Emby.Http.Model;
using EmbyStat.Common.Models.Entities;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Querying;
using Newtonsoft.Json.Linq;
using ServerInfo = EmbyStat.Common.Models.Entities.ServerInfo;

namespace EmbyStat.Clients.Emby.Http
{
    public interface IEmbyClient
    {
        void SetDeviceInfo(string deviceName, string authorizationScheme, string applicationVersion, string deviceId);
        string BaseUrl { get; set; }
        string ApiKey { get; set; }
        Task<List<PluginInfo>> GetInstalledPluginsAsync();
		Task<ServerInfo> GetServerInfoAsync();
	    Task<List<FileSystemEntryInfo>> GetLocalDrivesAsync();
        Task<JArray> GetEmbyUsersAsync();
        Task<JObject> GetEmbyDevicesAsync();
        Task<string> PingEmbyAsync();
        Task<QueryResult<BaseItemDto>> GetItemsAsync(ItemQuery query);
        Task<BaseItemDto> GetPersonByNameAsync(string personName);
        Task<QueryResult<BaseItemDto>> GetMediaFoldersAsync();
    }
}
