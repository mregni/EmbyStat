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
        #region Emby Status
        public EmbyStatus GetEmbyStatus()
        {
            using (var context = new ApplicationDbContext())
            {
                return new EmbyStatus(context.EmbyStatus.AsNoTracking());
            }
        }

        public void IncreaseMissedPings()
        {
            using (var context = new ApplicationDbContext())
            {
                var missingPings = context.EmbyStatus.Single(x => x.Id == Constants.EmbyStatus.MissedPings);
                var value = Convert.ToInt32(missingPings.Value);
                value++;

                missingPings.Value = value.ToString();
                context.SaveChanges();
            }
        }

        public void ResetMissedPings()
        {
            using (var context = new ApplicationDbContext())
            {
                var missingPings = context.EmbyStatus.Single(x => x.Id == Constants.EmbyStatus.MissedPings);
                missingPings.Value = "0";
                context.SaveChanges();
            }
        }
        #endregion

        #region Emby Plugins
        public List<PluginInfo> GetAllPlugins()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Plugins.OrderBy(x => x.Name).ToList();
            }
        }

        public void RemoveAllAndInsertPluginRange(IEnumerable<PluginInfo> plugins)
        {
            using (var context = new ApplicationDbContext())
            {
                context.RemoveRange(context.Plugins.ToList());
                context.SaveChanges();

                context.Plugins.AddRange(plugins);
                context.SaveChanges();
            }
        }

        #endregion

        #region Emby Server Info
        public ServerInfo GetServerInfo()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.ServerInfo.SingleOrDefault();
            }
        }

        public void AddOrUpdateServerInfo(ServerInfo entity)
        {
            using (var context = new ApplicationDbContext())
            {
                var settings = context.ServerInfo.AsNoTracking().SingleOrDefault();

                if (settings != null)
                {
                    entity.Id = settings.Id;
                    context.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    context.ServerInfo.Add(entity);
                }

                context.SaveChanges();
            }
        }

        #endregion

        #region Emby Drives
        public List<Drive> GetAllDrives()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Drives.ToList();
            }
        }

        public void RemoveAllAndInsertDriveRange(IEnumerable<Drive> drives)
        {
            using (var context = new ApplicationDbContext())
            {
                context.RemoveRange(context.Drives.ToList());
                context.SaveChanges();

                context.AddRange(drives);
                context.SaveChanges();
            }
        }

        #endregion

        #region Emby Users

        public async Task AddOrUpdateUsers(IEnumerable<User> users)
        {
            using (var context = new ApplicationDbContext())
            {
                foreach (var entity in users)
                {
                    var user = context.Users.AsNoTracking().SingleOrDefault(x => x.Id == entity.Id);

                    if (user != null)
                    {
                        context.Entry(entity).State = EntityState.Modified;
                    }
                    else
                    {
                        context.Users.Add(entity);
                    }
                }

                await context.SaveChangesAsync();
            }
        }

        public IEnumerable<User> GetAllUsers()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Users.ToList();
            }
        }

        public async Task MarkUserAsDeleted(IEnumerable<User> users)
        {
            using (var context = new ApplicationDbContext())
            {
                foreach (var entity in users)
                {
                    var user = context.Users.AsNoTracking().SingleOrDefault(x => x.Id == entity.Id);

                    if (user != null)
                    {
                        entity.Deleted = true;
                        context.Entry(entity).State = EntityState.Modified;
                    }
                }

                await context.SaveChangesAsync();
            }
        }

        #endregion


        #region Devices

        public IEnumerable<Device> GetAllDevices()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Devices.ToList();
            }
        }

        public async Task MarkDeviceAsDeleted(IEnumerable<Device> devices)
        {
            using (var context = new ApplicationDbContext())
            {
                foreach (var entity in devices)
                {
                    var device = context.Users.AsNoTracking().SingleOrDefault(x => x.Id == entity.Id);

                    if (device != null)
                    {
                        entity.Deleted = true;
                        context.Entry(entity).State = EntityState.Modified;
                    }
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task AddOrUpdateDevices(IEnumerable<Device> devices)
        {
            using (var context = new ApplicationDbContext())
            {
                foreach (var entity in devices)
                {
                    var device = context.Devices.AsNoTracking().SingleOrDefault(x => x.Id == entity.Id);

                    if (device != null)
                    {
                        context.Entry(entity).State = EntityState.Modified;
                    }
                    else
                    {
                        context.Devices.Add(entity);
                    }
                }

                await context.SaveChangesAsync();
            }
        }

        #endregion
    }
}
