using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using LiteDB;

namespace EmbyStat.Repositories
{
    public class EmbyRepository : IEmbyRepository
    {
        private readonly LiteCollection<EmbyStatus> _embyStatusCollection;
        private readonly LiteCollection<PluginInfo> _pluginCollection;
        private readonly LiteCollection<ServerInfo> _serverInfoCollection;
        private readonly LiteCollection<EmbyUser> _embyUserCollection;
        private readonly LiteCollection<Device> _deviceCollection;

        public EmbyRepository(IDbContext context)
        {
            _embyStatusCollection = context.GetContext().GetCollection<EmbyStatus>();
            _pluginCollection = context.GetContext().GetCollection<PluginInfo>();
            _serverInfoCollection = context.GetContext().GetCollection<ServerInfo>();
            _embyUserCollection = context.GetContext().GetCollection<EmbyUser>();
            _deviceCollection = context.GetContext().GetCollection<Device>();
        }

        #region Emby Status
        public EmbyStatus GetEmbyStatus()
        {
            return _embyStatusCollection.FindOne(Query.All());
        }

        public void IncreaseMissedPings()
        {
            var state = _embyStatusCollection.FindOne(Query.All());
            state.MissedPings++;

            _embyStatusCollection.Update(state);
        }

        public void ResetMissedPings()
        {
            var state = _embyStatusCollection.FindOne(Query.All());
            state.MissedPings = 0;

            _embyStatusCollection.Update(state);
        }
        #endregion

        #region Emby Plugins
        public List<PluginInfo> GetAllPlugins()
        {
            return _pluginCollection.FindAll().OrderBy(x => x.Name).ToList();
        }

        public void RemoveAllAndInsertPluginRange(IEnumerable<PluginInfo> plugins)
        {
            _pluginCollection.DeleteMany(x => true);
            _pluginCollection.Insert(plugins);
        }

        #endregion

        #region Emby Server Info
        public ServerInfo GetServerInfo()
        {
            return _serverInfoCollection.FindOne(Query.All());
        }

        public void UpsertServerInfo(ServerInfo entity)
        {
            _serverInfoCollection.Upsert(entity);
        }

        #endregion

        #region Emby Users

        public void UpsertUsers(IEnumerable<EmbyUser> users)
        {
            _embyUserCollection.Upsert(users);
        }

        public IEnumerable<EmbyUser> GetAllUsers()
        {
            return _embyUserCollection.FindAll();
        }

        public void MarkUsersAsDeleted(IEnumerable<EmbyUser> users)
        {
            foreach (var user in users)
            {
                var obj = _embyUserCollection.FindById(user.Id);
                obj.Deleted = true;
                _embyUserCollection.Update(obj);
            }
        }

        public EmbyUser GetUserById(string id)
        {
            return _embyUserCollection.FindById(id);
        }

        #endregion

        #region Devices

        public IEnumerable<Device> GetAllDevices()
        {
            return _deviceCollection.FindAll();
        }

        public IEnumerable<Device> GetDeviceById(IEnumerable<string> ids)
        {
            var bArray = new BsonArray();
            foreach (var id in ids)
            {
                bArray.Add(id);
            }

            return _deviceCollection.Find(Query.In("_id", bArray));
        }

        public void MarkDevicesAsDeleted(IEnumerable<Device> devices)
        {
            foreach (var device in devices)
            {
                var obj = _deviceCollection.FindById(device.Id);
                obj.Deleted = true;
                _deviceCollection.Update(obj);
            }
        }

        public void UpsertDevices(IEnumerable<Device> devices)
        {
            _deviceCollection.Upsert(devices);
        }

        #endregion
    }
}
