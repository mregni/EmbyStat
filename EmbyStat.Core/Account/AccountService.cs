﻿using System.IdentityModel.Tokens.Jwt;
using System.Text;
using EmbyStat.Common;
using EmbyStat.Common.Models.Account;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Configuration;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Core.Account.Interfaces;
using EmbyStat.Core.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace EmbyStat.Core.Account;

public class AccountService : IAccountService
{
    private readonly SignInManager<EmbyStatUser> _signInManager;
    private readonly UserManager<EmbyStatUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly Config _configuration;
    private readonly ILogger<AccountService> _logger;
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

    public AccountService(SignInManager<EmbyStatUser> signInManager, UserManager<EmbyStatUser> userManager, 
        IConfigurationService configurationService, JwtSecurityTokenHandler jwtSecurityTokenHandler, 
        RoleManager<IdentityRole> roleManager, ILogger<AccountService> logger)
    {
        _signInManager = signInManager;
        _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
        _roleManager = roleManager;
        _userManager = userManager;
        _userManager.Options.User.RequireUniqueEmail = false;
            
        _configuration = configurationService.Get();
        _logger = logger;
    }

    public async Task<AuthenticateResponse?> Authenticate(AuthenticateRequest login, string remoteIp)
    {
        var result = await _signInManager.PasswordSignInAsync(login.Username, login.Password, login.RememberMe, false);
        if (!result.Succeeded)
        {
            return null;
        }

        var user = await _userManager.FindByNameAsync(login.Username);
        var token = AuthenticationHelper.GenerateAccessToken(user, _configuration.SystemConfig.Jwt, _jwtSecurityTokenHandler);

        var refreshToken = AuthenticationHelper.GenerateRefreshToken();
        user.SetRefreshToken(refreshToken, user.Id, remoteIp);
        await _userManager.UpdateAsync(user);

        return new AuthenticateResponse
        {
            AccessToken = token,
            RefreshToken = refreshToken,
        };
    }

    public async Task<AuthenticateResponse?> RefreshToken(string accessToken, string refreshToken, string remoteIp)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.SystemConfig.Jwt.Key)),
            ValidateLifetime = false
        };
        try
        {
            var principal = _jwtSecurityTokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);
            if (!(securityToken is JwtSecurityToken jwtSecurityToken) 
                || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            if (principal == null)
            {
                return null;
            }

            var username = principal.Claims.First(c => c.Type == "sub");
            var user = await _userManager.FindByNameAsync(username.Value);

            if (user == null || !user.HasValidRefreshToken(refreshToken))
            {
                return null;
            }
            
            var token = AuthenticationHelper.GenerateAccessToken(user, _configuration.SystemConfig.Jwt, _jwtSecurityTokenHandler);
            var newRefreshToken = AuthenticationHelper.GenerateRefreshToken();
            user.SetRefreshToken(newRefreshToken, user.Id, remoteIp);
            await _userManager.UpdateAsync(user);

            return new AuthenticateResponse
            {
                AccessToken = token,
                RefreshToken = newRefreshToken,
            };

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error refreshing token");
            return null;
        }
    }

    public async Task Register(AuthenticateRequest login)
    {
        var newUser = new EmbyStatUser
        {
            UserName = login.Username,
            Email = "dummy@example.com"
        };

        var result = await _userManager.CreateAsync(newUser, login.Password);
        if (result.Succeeded)
        {
            var user = await _userManager.FindByNameAsync(newUser.UserName);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _userManager.ConfirmEmailAsync(user, token);
            await _userManager.SetLockoutEnabledAsync(user, false);
            await _userManager.AddToRolesAsync(user,new []{ Constants.Roles.Admin, Constants.Roles.User });
        }
    }

    public async Task LogOut()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<bool> AnyAdmins()
    {
        var users = await _userManager.GetUsersInRoleAsync(Constants.Roles.Admin);
        return users.Any();
    }

    public async Task<bool> ChangePassword(ChangePasswordRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            _logger.LogWarning($"Password update for ${user.UserName} failed with following message \n ${result.Errors.Select(x => x.Code + " - " + x.Description + "\n")}");
        }

        return result.Succeeded;

    }

    public async Task<bool> ChangeUserName(ChangeUserNameRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.OldUserName);
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.SetUserNameAsync(user, request.UserName);
        if (!result.Succeeded)
        {
            _logger.LogWarning($"Username update for ${user.UserName} failed with following message \n ${result.Errors.Select(x => x.Code + " - " + x.Description + "\n")}");
        }

        return result.Succeeded;
    }

    public async Task<bool> ResetPassword(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user != null)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var newPassword = RandomString(15);
            await _userManager.ResetPasswordAsync(user, token, newPassword);
            _logger.LogInformation("------------------------------");
            _logger.LogInformation($"Password reset requested for user {user.UserName}");
            _logger.LogInformation($"New Password: {newPassword}");
            _logger.LogInformation("------------------------------");
            return true;
        }

        return false;
    }

    public async Task CreateRoles()
    {
        if (!await _roleManager.RoleExistsAsync(Constants.Roles.Admin))
        {
            await _roleManager.CreateAsync(new IdentityRole(Constants.Roles.Admin));
        }

        if (!await _roleManager.RoleExistsAsync(Constants.Roles.User))
        {
            await _roleManager.CreateAsync(new IdentityRole(Constants.Roles.User));
        }
    }

    private static string RandomString(int length)
    {
        var random = new Random();
        const string chars = "  abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}