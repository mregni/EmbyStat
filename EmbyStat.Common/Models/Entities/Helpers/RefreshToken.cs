using System;

namespace EmbyStat.Common.Models.Entities.Helpers
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; }
        public DateTime Expires { get; }
        public string UserId { get; }
        public bool Active => DateTime.UtcNow <= Expires;
        public string RemoteIpAddress { get; }

        public RefreshToken()
        {
            
        }
        
        public RefreshToken(string token, DateTime expires, string userId, string remoteIpAddress)
        {
            Token = token;
            Expires = expires;
            UserId = userId;
            RemoteIpAddress = remoteIpAddress;
        }
    }
}
