using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Users;
using EmbyStat.Common.Models.Net;
using MediaBrowser.Model.Querying;
using Refit;
using MediaServerUser = EmbyStat.Common.Models.Entities.Users.MediaServerUser;

namespace EmbyStat.Clients.Base.Api
{
    public interface IMediaServerApi
    {
        #region Items
        
        /// <summary>
        /// Fetches a list of items filtered by a query object
        /// </summary>
        /// <param name="apiKey">Server API key</param>
        /// <param name="authorization">Fixed authentication string</param>
        /// <param name="param">Filter parameters</param>
        /// <returns><see cref="QueryResult{T}"/> where T is of type <see cref="BaseItemDto"/></returns>
        [Get("/items")]
        Task<QueryResult<BaseItemDto>> GetItems(
            [Header("X-Emby-Token")] string apiKey,
            [Header("X-Emby-Authorization")] string authorization,
            [Query] Dictionary<string, string> param);

        #endregion

        #region Genres
        
        /// <summary>
        /// Fetches a list of all genres filtered by a query object
        /// </summary>
        /// <param name="apiKey">Server API key</param>
        /// <param name="authorization">Fixed authentication string</param>
        /// <param name="param">Filter parameters</param>
        /// <returns><see cref="QueryResult{T}"/> where T is of type <see cref="BaseItemDto"/></returns>
        [Get("/genres")]
        Task<QueryResult<BaseItemDto>> GetGenres(
            [Header("X-Emby-Token")] string apiKey,
            [Header("X-Emby-Authorization")] string authorization,
            [Query] ItemQuery param);

        #endregion
        
        #region People
        
        /// <summary>
        /// Fetches a list of all people filtered by a query object
        /// </summary>
        /// <param name="apiKey">Server API key</param>
        /// <param name="authorization">Fixed authentication string</param>
        /// <param name="param">Filter parameters</param>
        /// <returns><see cref="QueryResult{T}"/> where T is of type <see cref="BaseItemDto"/></returns>
        [Get("/persons")]
        Task<QueryResult<BaseItemDto>> GetPeople(
            [Header("X-Emby-Token")] string apiKey,
            [Header("X-Emby-Authorization")] string authorization,
            [Query] ItemQuery param);
        
        #endregion
        
        #region Plugins

        /// <summary>
        /// Fetches a list of all plugins on the server
        /// </summary>
        /// <param name="apiKey">Server API key</param>
        /// <param name="authorization">Fixed authentication string</param>
        /// <returns><see cref="List{T}"/> where T is of type <see cref="PluginInfo"/></returns>
        [Get("/plugins")]
        Task<List<PluginInfo>> GetPlugins(
            [Header("X-Emby-Token")] string apiKey,
            [Header("X-Emby-Authorization")] string authorization);

        #endregion
        
        #region ServerInfo

        /// <summary>
        /// Fetches all server information
        /// </summary>
        /// <param name="apiKey">Server API key</param>
        /// <param name="authorization">Fixed authentication string</param>
        /// <returns><see cref="MediaServerInfo"/> object holding all the information</returns>
        [Get("/System/Info")]
        Task<ApiResponse<MediaServerInfo>> GetServerInfo(
            [Header("X-Emby-Token")] string apiKey,
            [Header("X-Emby-Authorization")] string authorization);
        
        /// <summary>
        /// Ping the media server. He will reply with a message if online.
        /// </summary>
        /// <param name="apiKey">Server API key</param>
        /// <param name="authorization">Fixed authentication string</param>
        /// <returns><see cref="string"/> that contains the result of the media server</returns>
        [Post("/System/Ping")]
        Task<string> Ping(
            [Header("X-Emby-Token")] string apiKey,
            [Header("X-Emby-Authorization")] string authorization);
        
        #endregion

        #region Users
        
        /// <summary>
        /// Fetches all users from the server
        /// </summary>
        /// <param name="apiKey">Server API key</param>
        /// <param name="authorization">Fixed authentication string</param>
        /// <returns><see cref="List{T}"/> where T is of type <see cref="Common.Models.Entities.Users.MediaServerUser"/></returns>
        [Get("/Users")]
        Task<List<MediaServerUser>> GetUsers(
            [Header("X-Emby-Token")] string apiKey,
            [Header("X-Emby-Authorization")] string authorization);
        
        #endregion

        #region Devices
        
        /// <summary>
        /// Fetches all devices on the server
        /// </summary>
        /// <param name="apiKey">Server API key</param>
        /// <param name="authorization">Fixed authentication string</param>
        /// <returns><see cref="List{T}"/> where T is of type <see cref="Device"/></returns>
        [Get("/Devices")]
        Task<QueryResult<Device>> GetDevices(
            [Header("X-Emby-Token")] string apiKey,
            [Header("X-Emby-Authorization")] string authorization);


        #endregion

        #region MediaFolders

        [Get("/Library/MediaFolders")]
        Task<QueryResult<BaseItemDto>> GetMediaFolders(
            [Header("X-Emby-Token")] string apiKey,
            [Header("X-Emby-Authorization")] string authorization);

        #endregion
    }
}
