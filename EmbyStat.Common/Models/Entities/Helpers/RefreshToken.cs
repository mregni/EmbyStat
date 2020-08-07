using System;
using LiteDB;

namespace EmbyStat.Common.Models.Entities.Helpers
{
    public class RefreshToken
    {
        public string Token { get; }
        public DateTime Expires { get; }
        public string UserId { get; }
        [BsonIgnore]
        public bool Active => DateTime.UtcNow <= Expires;
        public string RemoteIpAddress { get; }

        public RefreshToken(string token, DateTime expires, string userId, string remoteIpAddress)
        {
            Token = token;
            Expires = expires;
            UserId = userId;
            RemoteIpAddress = remoteIpAddress;
        }
    }
}
