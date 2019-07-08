using System.Collections.Generic;
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
        void UpsertServerInfo(ServerInfo entity);
        #endregion

        #region Emby Users
        void UpsertUsers(IEnumerable<EmbyUser> users);
        IEnumerable<EmbyUser> GetAllUsers();
        void MarkUsersAsDeleted(IEnumerable<EmbyUser> users);
        EmbyUser GetUserById(string id);

        #endregion

        #region Devices
        IEnumerable<Device> GetAllDevices();
        IEnumerable<Device> GetDeviceById(IEnumerable<string> ids);
        void MarkDevicesAsDeleted(IEnumerable<Device> devices);
        void UpsertDevices(IEnumerable<Device> devices);

        #endregion
    }
}
