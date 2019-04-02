using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Entities;
using Device = EmbyStat.Common.Models.Entities.Device;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IEmbyRepository
    {
        #region Emby Status
        EmbyStatus GetEmbyStatus();
        void IncreaseMissedPings();
        void ResetMissedPings();
        #endregion

        #region Emby Plugins
        List<PluginInfo> GetAllPlugins();
        void RemoveAllAndInsertPluginRange(IEnumerable<PluginInfo> plugins);
        #endregion

        #region Emby Server Info
        ServerInfo GetServerInfo();
        void AddOrUpdateServerInfo(ServerInfo entity);
        #endregion

        #region Emby Users
        Task AddOrUpdateUsers(IEnumerable<User> users);
        IEnumerable<User> GetAllUsers();
        Task MarkUserAsDeleted(IEnumerable<User> users);
        User GetUserById(string id);

        #endregion

        #region Devices
        IEnumerable<Device> GetAllDevices();
        Device GetDeviceById(string id);
        Task MarkDeviceAsDeleted(IEnumerable<Device> devices);
        Task AddOrUpdateDevices(IEnumerable<Device> devices);

        #endregion
    }
}
