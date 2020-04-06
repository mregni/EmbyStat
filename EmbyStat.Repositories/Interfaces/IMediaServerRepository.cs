using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;
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
        List<PluginInfo> GetAllPlugins();
        void RemoveAllAndInsertPluginRange(IEnumerable<PluginInfo> plugins);
        #endregion

        #region MediaServer Server Info
        ServerInfo GetServerInfo();
        void UpsertServerInfo(ServerInfo entity);
        #endregion

        #region MediaServer Users
        void UpsertUsers(IEnumerable<EmbyUser> users);
        List<EmbyUser> GetAllUsers();
        List<EmbyUser> GetAllAdministrators();
        void MarkUsersAsDeleted(IEnumerable<EmbyUser> users);
        EmbyUser GetUserById(string id);

        #endregion

        #region Devices
        List<Device> GetAllDevices();
        List<Device> GetDeviceById(IEnumerable<string> ids);
        void MarkDevicesAsDeleted(IEnumerable<Device> devices);
        void UpsertDevices(IEnumerable<Device> devices);

        #endregion

    }
}
