using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Clients.EmbyClient.Model;
using EmbyStat.Common.Models.Entities;
using MediaBrowser.Model.Plugins;

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
        void RemoveAllAndInsertPluginRange(List<PluginInfo> plugins);
        #endregion

        #region Emby Server Info
        ServerInfo GetServerInfo();
        void AddOrUpdateServerInfo(ServerInfo entity);
        #endregion

        #region Emby Drives
        List<Drive> GetAllDrives();
        void RemoveAllAndInsertDriveRange(List<Drive> drives);
        #endregion

        #region Emby Users
        void AddOrUpdateUsers(IEnumerable<User> users);
        IEnumerable<User> GetAllUsers();
        void MarkAsDeleted(IEnumerable<User> users);

        #endregion

    }
}
