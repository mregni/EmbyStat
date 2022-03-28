using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.SqLite;
using EmbyStat.Common.SqLite.Users;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IMediaServerRepository
    {
        #region MediaServer Status
        MediaServerStatus GetEmbyStatus();
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
        Task DeleteAndInsertServerInfo(SqlServerInfo entity);
        Task DeleteServerInfo();
        #endregion

        #region MediaServer Users
        Task DeleteAndInsertUsers(IEnumerable<SqlUser> users);
        Task<List<SqlUser>>  GetAllUsers();
        Task<List<SqlUser>>  GetAllAdministrators();
        EmbyUser GetUserById(string id);
        Task DeleteAllUsers();
        #endregion

        #region Devices
        Task<List<SqlDevice>> GetAllDevices();
        Task<List<SqlDevice>> GetDeviceById(IEnumerable<string> ids);
        Task DeleteAndInsertDevices(IEnumerable<SqlDevice> devices);
        Task DeleteAllDevices();
        #endregion

        #region Libraries
        Task<List<Library>> GetAllLibraries();
        Task<List<Library>> GetAllLibraries(LibraryType type);
        Task<List<Library>> GetAllLibraries(LibraryType type, bool synced);
        Task DeleteAndInsertLibraries(Library[] libraries);
        Task DeleteAllLibraries();
        Task UpdateLibrarySyncDate(string libraryId, DateTime utcNow);
        #endregion

    }
}
