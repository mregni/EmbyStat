using EmbyStat.Common.Enums;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Cards;
using EmbyStat.Common.Models.DataGrid;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Users;
using EmbyStat.Common.Models.MediaServer;

namespace EmbyStat.Core.MediaServers.Interfaces;

public interface IMediaServerService
{
    #region Server

    Task<IEnumerable<MediaServerUdpBroadcast>> SearchMediaServer(ServerType type);
    Task<MediaServerInfo> GetServerInfo(bool forceReSync);
    Task<bool> TestNewApiKey(string url, string apiKey, ServerType type);
    Task<MediaServerStatus> GetMediaServerStatus();
    Task<bool> PingMediaServer(string url);
    Task<bool> PingMediaServer();
    Task ResetMissedPings();
    Task IncreaseMissedPings();
    Task<Library[]> GetMediaServerLibraries();

    #endregion

    #region Plugins

    Task<List<PluginInfo>> GetAllPlugins();

    #endregion

    #region Users

    Task<MediaServerUserStatistics> GetMediaServerUserStatistics();
    Task<Page<MediaServerUserRow>> GetUserPage(int skip, int take, string sortField, string sortOrder, bool requireTotalCount);
    Task<MediaServerUser[]> GetAllUsers();
    Task<MediaServerUser[]> GetAllAdministrators();
    Task<MediaServerUser> GetUserById(string id);
    Card<int> GetViewedEpisodeCountByUserId(string id);
    Card<int> GetViewedMovieCountByUserId(string id);
    IEnumerable<UserMediaView> GetUserViewPageByUserId(string id, int page, int size);
    Task<int> ProcessViewsForUser(string id);
    Task<MediaServerUserStatistics> CalculateMediaServerUserStatistics();
    
    #endregion

    #region Devices

    Task<List<Device>> GetAllDevices();

    #endregion

    #region JobHelpers

    Task<MediaServerInfo> GetAndProcessServerInfo();
    Task GetAndProcessPluginInfo();
    Task GetAndProcessUsers();
    Task GetAndProcessDevices();
    Task GetAndProcessLibraries();

    #endregion

}