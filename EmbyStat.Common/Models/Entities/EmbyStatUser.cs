using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using EmbyStat.Common.Models.Entities.Helpers;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace EmbyStat.Common.Models.Entities
{
    public class EmbyStatUser : IdentityUser
    {
        [Column("RefreshTokens")]
        public string _refreshToken { get; set; }

        [NotMapped]
        public RefreshToken RefreshTokens
        {
            get => JsonConvert.DeserializeObject<RefreshToken>(_refreshToken);
            set => _refreshToken = JsonConvert.SerializeObject(value);
        }

        public bool HasValidRefreshToken(string refreshToken)
        {
            return RefreshTokens.Token == refreshToken && RefreshTokens.Active;
        }

        public void SetRefreshToken(string token, string userId, string remoteIpAddress, double daysToExpire = 5)
        {
            RefreshTokens = new RefreshToken(token, DateTime.UtcNow.AddDays(daysToExpire), userId, remoteIpAddress);
        }
    }
}
