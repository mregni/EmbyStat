﻿using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Users;
using EmbyStat.Common.Models.MediaServer;

namespace EmbyStat.Core.MediaServers.Interfaces;

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
    Task<IEnumerable<MediaServerUserRow>> GetUserPage(int skip, int take, string sortField, string sortOrder);
    Task<MediaServerUser[]> GetAllUsers();
    Task<MediaServerUser[]>  GetAllAdministrators();
    Task<MediaServerUser?> GetUserById(string id);
    Task DeleteAllUsers();
    Task InsertOrUpdateUserViews(List<MediaServerUserView> views);
    Task<int> GetUserCount();
    int GetUserViewsForType(MediaType type);
    Task<int> GetMediaServerViewsForUser(string id, MediaType type);

    #endregion

    #region Devices
    Task<List<Device>> GetAllDevices();
    Task DeleteAndInsertDevices(IEnumerable<Device> devices);
    Task DeleteAllDevices();
    #endregion

    #region Libraries
    Task<List<Library>> GetAllLibraries();
    Task<List<Library>> GetAllLibraries(LibraryType type);
    Task<List<Library>> GetAllSyncedLibraries(LibraryType type);
    Task SetLibraryAsSynced(string[] libraryIds, LibraryType type);
    Task DeleteAndInsertLibraries(Library[] libraries);
    Task DeleteAllLibraries();
    Task UpdateLibrarySyncDate(string libraryId, DateTime date, LibraryType type);
    #endregion
}