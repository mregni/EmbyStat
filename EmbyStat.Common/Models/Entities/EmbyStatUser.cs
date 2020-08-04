using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AspNetCore.Identity.LiteDB.Models;
using EmbyStat.Common.Models.Entities.Helpers;

namespace EmbyStat.Common.Models.Entities
{
    public class EmbyStatUser : ApplicationUser
    {
        public List<RefreshToken> RefreshTokens { get; set; }

        public bool HasValidRefreshToken(string refreshToken)
        {
            return RefreshTokens.Any(rt => rt.Token == refreshToken && rt.Active);
        }

        public void AddRefreshToken(string token, string userId, string remoteIpAddress, double daysToExpire = 5)
        {
            RefreshTokens.Add(new RefreshToken(token, DateTime.UtcNow.AddDays(daysToExpire), userId, remoteIpAddress));
        }

        public void RemoveRefreshToken(string refreshToken)
        {
            RefreshTokens.Remove(RefreshTokens.First(t => t.Token == refreshToken));
        }

        public EmbyStatUser()
        {
            RefreshTokens = new List<RefreshToken>();
        }
    }
}
