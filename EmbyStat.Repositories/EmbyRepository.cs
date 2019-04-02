using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Repositories
{
    public class EmbyRepository : IEmbyRepository
    {
        private readonly ApplicationDbContext _context;

        public EmbyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Emby Status
        public EmbyStatus GetEmbyStatus()
        {
            return new EmbyStatus(_context.EmbyStatus.AsNoTracking());
        }

        public void IncreaseMissedPings()
        {
            var missingPings = _context.EmbyStatus.Single(x => x.Id == Constants.EmbyStatus.MissedPings);
            var value = Convert.ToInt32(missingPings.Value);
            value++;

            missingPings.Value = value.ToString();
            _context.SaveChanges();
        }

        public void ResetMissedPings()
        {
            var missingPings = _context.EmbyStatus.Single(x => x.Id == Constants.EmbyStatus.MissedPings);
            missingPings.Value = "0";
            _context.SaveChanges();
        }
        #endregion

        #region Emby Plugins
        public List<PluginInfo> GetAllPlugins()
        {
            return _context.Plugins.OrderBy(x => x.Name).ToList();
        }

        public void RemoveAllAndInsertPluginRange(IEnumerable<PluginInfo> plugins)
        {
            _context.RemoveRange(_context.Plugins.ToList());
            _context.SaveChanges();

            _context.Plugins.AddRange(plugins);
            _context.SaveChanges();
        }

        #endregion

        #region Emby Server Info
        public ServerInfo GetServerInfo()
        {
            return _context.ServerInfo.SingleOrDefault();
        }

        public void AddOrUpdateServerInfo(ServerInfo entity)
        {
            var settings = _context.ServerInfo.AsNoTracking().SingleOrDefault();

            if (settings != null)
            {
                entity.Id = settings.Id;
                _context.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                _context.ServerInfo.Add(entity);
            }

            _context.SaveChanges();
        }

        #endregion

        #region Emby Users

        public async Task AddOrUpdateUsers(IEnumerable<User> users)
        {
            foreach (var entity in users)
            {
                var user = _context.Users.AsNoTracking().SingleOrDefault(x => x.Id == entity.Id);

                if (user != null)
                {
                    _context.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    _context.Users.Add(entity);
                }
            }

            await _context.SaveChangesAsync();
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public async Task MarkUserAsDeleted(IEnumerable<User> users)
        {
            foreach (var entity in users)
            {
                var user = _context.Users.AsNoTracking().SingleOrDefault(x => x.Id == entity.Id);

                if (user != null)
                {
                    entity.Deleted = true;
                    _context.Entry(entity).State = EntityState.Modified;
                }
            }

            await _context.SaveChangesAsync();
        }

        public User GetUserById(string id)
        {
            return _context.Users
                .Include(x => x.AccessSchedules)
                .SingleOrDefault(x => x.Id == id);
        }

        #endregion


        #region Devices

        public IEnumerable<Device> GetAllDevices()
        {
            return _context.Devices.ToList();
        }

        public Device GetDeviceById(string id)
        {
            return _context.Devices.SingleOrDefault(x => x.Id == id);
        }

        public async Task MarkDeviceAsDeleted(IEnumerable<Device> devices)
        {
            foreach (var entity in devices)
            {
                var device = _context.Users.AsNoTracking().SingleOrDefault(x => x.Id == entity.Id);

                if (device != null)
                {
                    entity.Deleted = true;
                    _context.Entry(entity).State = EntityState.Modified;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task AddOrUpdateDevices(IEnumerable<Device> devices)
        {
            foreach (var entity in devices)
            {
                var device = _context.Devices.AsNoTracking().SingleOrDefault(x => x.Id == entity.Id);

                if (device != null)
                {
                    _context.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    _context.Devices.Add(entity);
                }
            }

            await _context.SaveChangesAsync();
        }

        #endregion
    }
}
