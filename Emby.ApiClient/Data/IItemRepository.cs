using MediaBrowser.Model.Sync;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emby.ApiClient.Data
{
    /// <summary>
    /// Interface IItemRepository
    /// </summary>
    public interface IItemRepository
    {
        /// <summary>
        /// Adds the or update.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Task.</returns>
        Task AddOrUpdate(LocalItem item);

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;BaseItemDto&gt;.</returns>
        Task<LocalItem> Get(string id);

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task.</returns>
        Task Delete(string id);

        /// <summary>
        /// Gets the server item ids.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <returns>Task&lt;List&lt;System.String&gt;&gt;.</returns>
        Task<List<string>> GetServerItemIds(string serverId);

        /// <summary>
        /// Queries all items for a server Id and returns a list of unique item types.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Task&lt;List&lt;System.String&gt;&gt;.</returns>
        Task<List<string>> GetItemTypes(string serverId, string userId);

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Task&lt;List&lt;LocalItem&gt;&gt;.</returns>
        Task<List<LocalItem>> GetItems(LocalItemQuery query);

        /// <summary>
        /// Gets a list of unique AlbumArtist values
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Task&lt;List&lt;System.String&gt;&gt;.</returns>
        Task<List<LocalItemInfo>> GetAlbumArtists(string serverId, string userId);

        /// <summary>
        /// Gets a list of unique series, by id
        /// Name = Album property
        /// Id = SeriesId property
        /// PrimaryImageTag = SeriesPrimaryImageTag
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Task&lt;List&lt;LocalItemInfo&gt;&gt;.</returns>
        Task<List<LocalItemInfo>> GetTvSeries(string serverId, string userId);

        /// <summary>
        /// Gets a list of unique photo albums, by Id
        /// Name = Album property
        /// Id = AlbumId property
        /// PrimaryImageTag = AlbumPrimaryImageTag
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Task&lt;List&lt;LocalItemInfo&gt;&gt;.</returns>
        Task<List<LocalItemInfo>> GetPhotoAlbums(string serverId, string userId);
    }
}
