using System;
using System.Linq;
using EmbyStat.Common;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmbyStat.Repositories
{
    public class EmbyStatusRepository: IEmbyStatusRepository
    {
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
    }
}
