using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Net;
using MediaBrowser.Model.Querying;
using Refit;

namespace EmbyStat.Clients.Base.Http
{
    public interface INewBaseClient
    {
        [Get("/items")]
        Task<QueryResult<BaseItemDto>> GetItems(
            [Header("X-Emby-Token")] string apiKey,
            [Header("X-Emby-Authorization")] string authorization,
            [Query] Dictionary<string, string> param);

        [Get("/items")]
        Task<QueryResult<BaseItemDto>> GetItems(
            [Header("X-Emby-Token")] string apiKey,
            [Header("X-Emby-Authorization")] string authorization,
            [Query] ItemQuery param,
            [Query] string userId);

        [Get("/genres")]
        Task<QueryResult<BaseItemDto>> GetGenres(
            [Header("X-Emby-Token")] string apiKey,
            [Header("X-Emby-Authorization")] string authorization,
            [Query] ItemQuery param);

        [Get("/persons")]
        Task<QueryResult<BaseItemDto>> GetPeople(
            [Header("X-Emby-Token")] string apiKey,
            [Header("X-Emby-Authorization")] string authorization,
            [Query] ItemQuery param);
    }
}
