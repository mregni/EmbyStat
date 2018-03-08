using MediaBrowser.Model.Sync;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Emby.ApiClient.Data
{
    public interface IFileRepository
    {
        /// <summary>
        /// Gets the file system entries.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;List&lt;DeviceFileInfo&gt;&gt;.</returns>
        Task<List<DeviceFileInfo>> GetFileSystemEntries(string path);

        /// <summary>
        /// Saves the file.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="path">The path.</param>
        /// <returns>Task.</returns>
        Task SaveFile(Stream stream, string path);

#if WINDOWS_UWP
        /// <summary>
        /// Saves the file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="path">The path.</param>
        /// <returns>Task.</returns>
        Task SaveFile(Windows.Storage.IStorageFile file, string path);
#endif

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Task.</returns>
        Task DeleteFile(string path);

        /// <summary>
        /// Deletes the directory.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Task.</returns>
        Task DeleteFolder(string path);

        /// <summary>
        /// Strips invalid characters from a given name
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        string GetValidFileName(string name);

        /// <summary>
        /// Files the exists.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> FileExists(string path);

        /// <summary>
        /// Gets the full local path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        string GetFullLocalPath(IEnumerable<string> path);

        /// <summary>
        /// Gets the parent directory path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        string GetParentDirectoryPath(string path);

        /// <summary>
        /// Gets the file stream.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;Stream&gt;.</returns>
        Task<Stream> GetFileStream(string path);
#if WINDOWS_UWP
        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;File&gt;.</returns>
        Task<Windows.Storage.IStorageFile> GetFile(string path);
#endif
    }
}
