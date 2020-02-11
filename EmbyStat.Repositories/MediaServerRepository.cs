using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Extensions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using LiteDB;

namespace EmbyStat.Repositories
{
    public class MediaServerRepository : BaseRepository, IMediaServerRepository
    {
        public MediaServerRepository(IDbContext context) : base(context)
        {

        }

        #region MediaServer Status
        public EmbyStatus GetEmbyStatus()
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<EmbyStatus>();
                    return collection.FindOne(Query.All());
                }
            });
        }

        public void IncreaseMissedPings()
        {
            ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<EmbyStatus>();
                    var state = collection.FindOne(Query.All());

                    state.MissedPings++;
                    collection.Upsert(state);
                }
            });
        }

        public void ResetMissedPings()
        {
            ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<EmbyStatus>();
                    var state = collection.FindOne(Query.All());

                    state.MissedPings = 0;
                    collection.Upsert(state);
                }
            });
        }
        
        #endregion

        #region MediaServer Plugins
        public List<PluginInfo> GetAllPlugins()
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<PluginInfo>();
                    return collection.FindAll().OrderBy(x => x.Name).ToList();
                }
            });
        }

        public void RemoveAllAndInsertPluginRange(IEnumerable<PluginInfo> plugins)
        {
            ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<PluginInfo>();
                    collection.Delete(Query.All());
                    collection.Insert(plugins);
                }
            });
            
        }

        #endregion

        #region MediaServer Server Info
        public ServerInfo GetServerInfo()
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<ServerInfo>();
                    return collection.FindOne(Query.All());
                }
            });
        }

        public void UpsertServerInfo(ServerInfo entity)
        {
            ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<ServerInfo>();
                    collection.Upsert(entity);
                }
            });
        }

        #endregion

        #region MediaServer Users

        public void UpsertUsers(IEnumerable<EmbyUser> users)
        {
            ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<EmbyUser>();
                    collection.Upsert(users);
                }
            });
        }

        public IEnumerable<EmbyUser> GetAllUsers()
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<EmbyUser>();
                    return collection.FindAll().OrderBy(x => x.Name).ToList();
                }
            });
        }

        public IEnumerable<EmbyUser> GetAllAdministrators()
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<EmbyUser>();
                    return collection.Find(x => x.IsAdministrator).OrderBy(x => x.Name);
                }
            });
        }

        public void MarkUsersAsDeleted(IEnumerable<EmbyUser> users)
        {
            ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<EmbyUser>();
                    foreach (var user in users)
                    {
                        var obj = collection.FindById(user.Id);
                        obj.Deleted = true;
                        collection.Update(obj);
                    }
                }
            });
        }

        public EmbyUser GetUserById(string id)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<EmbyUser>();
                    return collection.FindById(id);
                }
            });
        }

        #endregion

        #region Devices

        public List<Device> GetAllDevices()
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Device>();
                    return collection.FindAll().OrderBy(x => x.Name).ToList();
                }
            });
        }

        public List<Device> GetDeviceById(IEnumerable<string> ids)
        {
            return ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Device>();
                    return collection.Find(Query.In("_id", ids.ConvertToBsonArray())).ToList();
                }
            });
        }

        public void MarkDevicesAsDeleted(IEnumerable<Device> devices)
        {
            ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Device>();
                    foreach (var device in devices)
                    {
                        var obj = collection.FindById(device.Id);
                        obj.Deleted = true;
                        collection.Update(obj);
                    }
                }
            });
        }

        public void UpsertDevices(IEnumerable<Device> devices)
        {
            ExecuteQuery(() =>
            {
                using (var database = Context.CreateDatabaseContext())
                {
                    var collection = database.GetCollection<Device>();
                    collection.Upsert(devices);
                }
            });
        }

        #endregion
    }
}
