using MediaBrowser.Model.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emby.ApiClient.Data
{
    public interface IUserRepository
    {
        /// <summary>
        /// Adds the or update.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="user">The user.</param>
        /// <returns>Task.</returns>
        Task AddOrUpdate(string id, UserDto user);
        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task.</returns>
        Task Delete(string id);
        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;UserDto&gt;.</returns>
        Task<UserDto> Get(string id);
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>Task&lt;List&lt;UserDto&gt;&gt;.</returns>
        Task<List<UserDto>> GetAll();
    }
}
