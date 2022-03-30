using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Users;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IMediaServerRepository
    {
        #region MediaServer Status
        Task<MediaServerStatus> GetEmbyStatus();
        Task IncreaseMissedPings();
        Task ResetMissedPings();
        #endregion

        #region MediaServer Plugins
        Task<List<PluginInfo>>  GetAllPlugins();
        Task InsertPlugins(IEnumerable<PluginInfo> plugins);
        Task DeleteAllPlugins();
        #endregion

        #region MediaServer Server Info
        Task<MediaServerInfo> GetServerInfo();
        Task DeleteAndInsertServerInfo(MediaServerInfo entity);
        Task DeleteServerInfo();
        #endregion

        #region MediaServer Users
        Task DeleteAndInsertUsers(IEnumerable<MediaServerUser> users);
        Task<List<MediaServerUser>>  GetAllUsers();
        Task<List<MediaServerUser>>  GetAllAdministrators();
        EmbyUser GetUserById(string id);
        Task DeleteAllUsers();
        #endregion

        #region Devices
        Task<List<Device>> GetAllDevices();
        Task<List<Device>> GetDeviceById(IEnumerable<string> ids);
        Task DeleteAndInsertDevices(IEnumerable<Device> devices);
        Task DeleteAllDevices();
        #endregion

        #region Libraries
        Task<List<Library>> GetAllLibraries();
        Task<List<Library>> GetAllLibraries(LibraryType type);
        Task<List<Library>> GetAllLibraries(LibraryType type, bool synced);
        Task SetLibraryAsSynced(string[] libraryIds, LibraryType type);
        Task DeleteAndInsertLibraries(Library[] libraries);
        Task DeleteAllLibraries();
        Task UpdateLibrarySyncDate(string libraryId, DateTime utcNow);
        #endregion

    }
}
