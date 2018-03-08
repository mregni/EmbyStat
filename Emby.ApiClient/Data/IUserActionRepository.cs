using MediaBrowser.Model.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emby.ApiClient.Data
{
    public interface IUserActionRepository
    {
        /// <summary>
        /// Creates the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>Task.</returns>
        Task Create(UserAction action);

        /// <summary>
        /// Deletes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>Task.</returns>
        Task Delete(UserAction action);

        /// <summary>
        /// Gets all user actions by serverId
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        Task<IEnumerable<UserAction>> Get(string serverId);
    }
}
