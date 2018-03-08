using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Dlna;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Sync;
using MediaBrowser.Model.Users;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Emby.ApiClient.Model;

namespace Emby.ApiClient.Data
{
    public interface ILocalAssetManager
    {
        /// <summary>
        /// Records the user action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>Task.</returns>
        Task RecordUserAction(UserAction action);

        /// <summary>
        /// Deletes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>Task.</returns>
        Task Delete(UserAction action);

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Task.</returns>
        Task Delete(LocalItem item);

        /// <summary>
        /// Gets all user actions by serverId
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        Task<IEnumerable<UserAction>> GetUserActions(string serverId);

        /// <summary>
        /// Adds the or update.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Task.</returns>
        Task AddOrUpdate(LocalItem item);

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Task&lt;List&lt;ItemFileInfo&gt;&gt;.</returns>
        Task<List<ItemFileInfo>> GetFiles(LocalItem item);

        /// <summary>
        /// Deletes the specified file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Task.</returns>
        Task DeleteFile(string path);

        /// <summary>
        /// Saves the subtitles.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="format">The format.</param>
        /// <param name="item">The item.</param>
        /// <param name="language">The language.</param>
        /// <param name="isForced">if set to <c>true</c> [is forced].</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        Task<string> SaveSubtitles(Stream stream,
            string format,
            LocalItem item,
            string language,
            bool isForced);

        /// <summary>
        /// Saves the media.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="localItem">The local item.</param>
        /// <param name="server">The server.</param>
        /// <returns>Task.</returns>
        Task SaveMedia(Stream stream, LocalItem localItem, ServerInfo server);
#if WINDOWS_UWP
        /// <summary>
        /// Saves the media.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="localItem">The local item.</param>
        /// <param name="server">The server.</param>
        /// <returns>Task.</returns>
        Task SaveMedia(Windows.Storage.IStorageFile file, LocalItem localItem, ServerInfo server);
#endif
        /// <summary>
        /// Creates the local item.
        /// </summary>
        /// <param name="libraryItem">The library item.</param>
        /// <param name="server">The server.</param>
        /// <param name="syncJobItemId">The synchronize job item identifier.</param>
        /// <param name="originalFileName">Name of the original file.</param>
        /// <returns>LocalItem.</returns>
        LocalItem CreateLocalItem(BaseItemDto libraryItem, ServerInfo server, string syncJobItemId, string originalFileName);
        /// <summary>
        /// Gets the local item.
        /// </summary>
        /// <param name="localId">The local identifier.</param>
        /// <returns>Task&lt;LocalItem&gt;.</returns>
        Task<LocalItem> GetLocalItem(string localId);
        /// <summary>
        /// Gets the local item.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <returns>Task&lt;LocalItem&gt;.</returns>
        Task<LocalItem> GetLocalItem(string serverId, string itemId);
        /// <summary>
        /// Files the exists.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> FileExists(string path);
        /// <summary>
        /// Gets the server item ids.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <returns>Task&lt;List&lt;System.String&gt;&gt;.</returns>
        Task<List<string>> GetServerItemIds(string serverId);
        /// <summary>
        /// Gets the file stream.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>Task&lt;Stream&gt;.</returns>
        Task<Stream> GetFileStream(StreamInfo info);
        /// <summary>
        /// Gets the file stream.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;Stream&gt;.</returns>
        Task<Stream> GetFileStream(string path);
        /// <summary>
        /// Saves the offline user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Task.</returns>
        Task SaveOfflineUser(UserDto user);
        /// <summary>
        /// Deletes the offline user.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task.</returns>
        Task DeleteOfflineUser(string id);
        /// <summary>
        /// Saves the user image.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>Task.</returns>
        Task SaveImage(UserDto user, Stream stream);
        /// <summary>
        /// Gets the user image.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Task&lt;Stream&gt;.</returns>
        Task<Stream> GetImage(UserDto user);
        /// <summary>
        /// Deletes the user image.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Task.</returns>
        Task DeleteImage(UserDto user);
        /// <summary>
        /// Determines whether the specified user has image.
        /// </summary>
        /// <param name="user">The user.</param>
        Task<bool> HasImage(UserDto user);
        /// <summary>
        /// Saves the item image.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="imageId">The image identifier.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>Task.</returns>
        Task SaveImage(string serverId, string itemId, string imageId, Stream stream);
        /// <summary>
        /// Determines whether the specified server identifier has image.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="imageId">The image identifier.</param>
        Task<bool> HasImage(string serverId, string itemId, string imageId);
        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="imageId">The image identifier.</param>
        /// <returns>Task&lt;Stream&gt;.</returns>
        Task<Stream> GetImage(string serverId, string itemId, string imageId);
        /// <summary>
        /// Determines whether the specified item has image.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="imageId">The image identifier.</param>
        Task<bool> HasImage(BaseItemDto item, string imageId);
        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="imageId">The image identifier.</param>
        /// <returns>Task&lt;Stream&gt;.</returns>
        Task<Stream> GetImage(BaseItemDto item, string imageId);
        /// <summary>
        /// Gets the views.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Task&lt;List&lt;BaseItemDto&gt;&gt;.</returns>
        Task<List<BaseItemDto>> GetViews(string serverId, string userId);
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="parentItem">The parent item.</param>
        /// <returns>Task&lt;List&lt;BaseItemDto&gt;&gt;.</returns>
        Task<List<BaseItemDto>> GetItems(UserDto user, BaseItemDto parentItem);
        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;UserDto&gt;.</returns>
        Task<UserDto> GetUser(string id);
    }
}