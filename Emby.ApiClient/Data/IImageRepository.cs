using System.IO;
using System.Threading.Tasks;

namespace Emby.ApiClient.Data
{
    public interface IImageRepository
    {
        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="imageId">The image identifier.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>Task.</returns>
        Task SaveImage(string itemId, string imageId, Stream stream);
        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="imageId">The image identifier.</param>
        /// <returns>Task&lt;Stream&gt;.</returns>
        Task<Stream> GetImage(string itemId, string imageId);
        /// <summary>
        /// Deletes the image.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="imageId">The image identifier.</param>
        /// <returns>Task.</returns>
        Task DeleteImage(string itemId, string imageId);
        /// <summary>
        /// Determines whether the specified item identifier has image.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="imageId">The image identifier.</param>
        Task<bool> HasImage(string itemId, string imageId);
        /// <summary>
        /// Deletes the images.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <returns>Task.</returns>
        Task DeleteImages(string itemId);
    }
}
