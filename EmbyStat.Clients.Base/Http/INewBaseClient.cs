using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Net;
using EmbyStat.Common.SqLite;
using EmbyStat.Common.SqLite.Users;
using MediaBrowser.Model.Querying;
using Newtonsoft.Json.Linq;
using Refit;

namespace EmbyStat.Clients.Base.Http
{
    public interface INewBaseClient
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
        /// <returns><see cref="List{T}"/> where T is of type <see cref="SqlPluginInfo"/></returns>
        [Get("/plugins")]
        Task<List<SqlPluginInfo>> GetPlugins(
            [Header("X-Emby-Token")] string apiKey,
            [Header("X-Emby-Authorization")] string authorization);

        #endregion
        
        #region ServerInfo

        /// <summary>
        /// Fetches all server information
        /// </summary>
        /// <param name="apiKey">Server API key</param>
        /// <param name="authorization">Fixed authentication string</param>
        /// <returns><see cref="ServerInfoDto"/> object holding all the information</returns>
        [Get("/System/Info")]
        Task<ServerInfoDto> GetServerInfo(
            [Header("X-Emby-Token")] string apiKey,
            [Header("X-Emby-Authorization")] string authorization);
        
        #endregion

        #region Users
        
        /// <summary>
        /// Fetches all users from the server
        /// </summary>
        /// <param name="apiKey">Server API key</param>
        /// <param name="authorization">Fixed authentication string</param>
        /// <returns><see cref="List{T}"/> where T is of type <see cref="SqlUser"/></returns>
        [Get("/Users")]
        Task<List<SqlUser>> GetUsers(
            [Header("X-Emby-Token")] string apiKey,
            [Header("X-Emby-Authorization")] string authorization);
        
        #endregion

        #region Devices
        
        /// <summary>
        /// Fetches all devices on the server
        /// </summary>
        /// <param name="apiKey">Server API key</param>
        /// <param name="authorization">Fixed authentication string</param>
        /// <returns><see cref="List{T}"/> where T is of type <see cref="SqlDevice"/></returns>
        [Get("/Devices")]
        Task<ApiResponse<QueryResult<SqlDevice>>> GetDevices(
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
