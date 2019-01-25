using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Clients.EmbyClient.Model;
using EmbyStat.Common;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using MediaBrowser.Model.Plugins;
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

        public void RemoveAllAndInsertPluginRange(List<PluginInfo> plugins)
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

        public void RemoveAllAndInsertDriveRange(List<Drive> drives)
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
    }
}
