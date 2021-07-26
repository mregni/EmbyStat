using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Identity.LiteDB.Models;
using EmbyStat.Common;
using EmbyStat.Common.Helpers;
using EmbyStat.Common.Models.Account;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Logging;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace EmbyStat.Services
{
    public class AccountService : IAccountService
    {
        private readonly SignInManager<EmbyStatUser> _signInManager;
        private readonly UserManager<EmbyStatUser> _userManager;
        private readonly AppSettings _appSettings;
        private readonly Logger _logger;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

        public AccountService(SignInManager<EmbyStatUser> signInManager, UserManager<EmbyStatUser> userManager, 
            ISettingsService settingsService, JwtSecurityTokenHandler jwtSecurityTokenHandler)
        {
            _signInManager = signInManager;
            _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
            _userManager = userManager;
            _userManager.Options.User.RequireUniqueEmail = false;

            _appSettings = settingsService.GetAppSettings();
            _logger = LogFactory.CreateLoggerForType(typeof(AccountService), "ACCOUNT");
        }

        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest login, string remoteIp)
        {
            var result = await _signInManager.PasswordSignInAsync(login.Username, login.Password, login.RememberMe, false);

            if (!result.Succeeded)
            {
                return null;
            }

            var user = await _userManager.FindByNameAsync(login.Username);
            var token = AuthenticationHelper.GenerateAccessToken(user, _appSettings.Jwt, _jwtSecurityTokenHandler);

            var refreshToken = AuthenticationHelper.GenerateRefreshToken();
            user.AddRefreshToken(refreshToken, user.Id, remoteIp);
            await _userManager.UpdateAsync(user);

            return new AuthenticateResponse
            {
                AccessToken = token,
                RefreshToken = refreshToken,
            };
        }

        public async Task<AuthenticateResponse> RefreshToken(string accessToken, string refreshToken, string remoteIp)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Jwt.Key)),
                ValidateLifetime = false
            };

            var principal = _jwtSecurityTokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);
            if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            if (principal == null)
            {
                return null;
            }

            var username = principal.Claims.First(c => c.Type == "sub");
            var user = await _userManager.FindByNameAsync(username.Value);

            if (user != null && user.HasValidRefreshToken(refreshToken))
            {
                var token = AuthenticationHelper.GenerateAccessToken(user, _appSettings.Jwt, _jwtSecurityTokenHandler);
                var newRefreshToken = AuthenticationHelper.GenerateRefreshToken();
                user.RemoveRefreshToken(refreshToken);
                user.AddRefreshToken(newRefreshToken, user.Id, remoteIp);
                await _userManager.UpdateAsync(user);

                return new AuthenticateResponse
                {
                    AccessToken = token,
                    RefreshToken = newRefreshToken,
                };
            }

            return null;
        }

        public async Task Register(AuthenticateRequest login)
        {
            var user = new EmbyStatUser
            {
                UserName = login.Username,
                Roles = new List<string> { Constants.JwtClaims.Admin, Constants.JwtClaims.User  },
                Email = new EmailInfo(),
                EmailConfirmed = false
            };

            await _userManager.CreateAsync(user, login.Password);
        }

        public async Task LogOut()
        {
            await _signInManager.SignOutAsync();
        }

        public bool AnyAdmins()
        {
            var boe = _userManager.Users.ToList();
            return _userManager.Users.Any(x => x.Roles.Contains(Constants.JwtClaims.Admin));
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
                _logger.Warn($"Password update for ${user.UserName} failed with following message \n ${result.Errors.Select(x => x.Code + " - " + x.Description + "\n")}");
            }

            return result.Succeeded;

        }

        public async Task<bool> ChangeUserName(ChangeUserNameRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.SetUserNameAsync(user, request.NewUserName);
            if (!result.Succeeded)
            {
                _logger.Warn($"Username update for ${user.UserName} failed with following message \n ${result.Errors.Select(x => x.Code + " - " + x.Description + "\n")}");
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
                _logger.Info("------------------------------");
                _logger.Info($"Password reset requested for user {user.UserName}");
                _logger.Info($"New Password: {newPassword}");
                _logger.Info("------------------------------");
                return true;
            }

            return false;
        }

        private static string RandomString(int length)
        {
            var random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
