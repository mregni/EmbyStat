using System;
using System.ComponentModel.DataAnnotations.Schema;
using EmbyStat.Common.Models.Entities.Helpers;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace EmbyStat.Common.Models.Entities;

public class EmbyStatUser : IdentityUser
{
    [Column("RefreshTokens")]
    public string _refreshToken { get; set; }

    [NotMapped]
    public RefreshToken RefreshToken
    {
        get => JsonConvert.DeserializeObject<RefreshToken>(_refreshToken ?? "");
        set => _refreshToken = JsonConvert.SerializeObject(value);
    }

    public bool HasValidRefreshToken(string refreshToken)
    {
        return RefreshToken != null && RefreshToken.Token == refreshToken && RefreshToken.Active;
    }

    public void SetRefreshToken(string token, string userId, string remoteIpAddress, double daysToExpire = 5)
    {
        RefreshToken = new RefreshToken(token, DateTime.UtcNow.AddDays(daysToExpire), userId, remoteIpAddress);
    }
}