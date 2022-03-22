using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.SqLite;
using EmbyStat.Common.SqLite.Users;
using Device = EmbyStat.Common.Models.Entities.Device;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IMediaServerRepository
    {
        #region MediaServer Status
        EmbyStatus GetEmbyStatus();
        void IncreaseMissedPings();
        void ResetMissedPings();
        void RemoveAllMediaServerData();
        #endregion

        #region MediaServer Plugins
        Task<List<SqlPluginInfo>>  GetAllPlugins();
        Task InsertPlugins(IEnumerable<SqlPluginInfo> plugins);
        Task DeleteAllPlugins();
        #endregion

        #region MediaServer Server Info
        Task<SqlServerInfo> GetServerInfo();
        Task UpsertServerInfo(SqlServerInfo entity);
        Task DeleteServerInfo();
        #endregion

        #region MediaServer Users
        Task UpsertUsers(IEnumerable<SqlUser> users);
        Task<List<SqlUser>>  GetAllUsers();
        Task<List<SqlUser>>  GetAllAdministrators();
        EmbyUser GetUserById(string id);
        Task DeleteAllUsers();
        #endregion

        #region Devices
        Task<List<SqlDevice>> GetAllDevices();
        Task<List<SqlDevice>> GetDeviceById(IEnumerable<string> ids);
        Task UpsertDevices(IEnumerable<SqlDevice> devices);
        Task DeleteAllDevices();
        #endregion

    }
}
