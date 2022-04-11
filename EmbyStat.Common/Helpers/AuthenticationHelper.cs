﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using EmbyStat.Common.Models.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace EmbyStat.Common.Helpers;

public static class AuthenticationHelper
{
    public static string GenerateAccessToken(IdentityUser user, Jwt jwt, JwtSecurityTokenHandler tokenHandler)
    {
        var identity = GenerateClaimsIdentity(user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(jwt.IssuedAt).ToString(), ClaimValueTypes.Integer64)
        };

        claims.AddRange(identity.Claims);

        var token = GenerateToken(claims, jwt);
        return tokenHandler.WriteToken(token);
    }

    public static string GenerateRefreshToken(int size = 128)
    {
        var randomNumber = new byte[size];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private static ClaimsIdentity GenerateClaimsIdentity(IdentityUser user)
    {
        var claimsIdentity = new ClaimsIdentity(
            new GenericIdentity(user.UserName, "AccessToken"), new[]
            {
                new Claim(Constants.JwtClaimIdentifiers.Id, user.Id)
            });

        //TODO: fix this!!
        // var roleClaims = user.Roles.Select(x => new Claim(Constants.JwtClaimIdentifiers.Roles, x));
        // claimsIdentity.AddClaims(roleClaims);

        return claimsIdentity;
    }

    private static JwtSecurityToken GenerateToken(IEnumerable<Claim> claims, Jwt jwt)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        return new JwtSecurityToken(
            jwt.Issuer,
            jwt.Audience,
            claims,
            jwt.NotBefore,
            jwt.Expiration,
            creds
        );
    }

    private static long ToUnixEpochDate(DateTime date)
        => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
}