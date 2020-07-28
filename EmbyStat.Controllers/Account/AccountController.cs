using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Account;
using EmbyStat.Controllers.MediaServer;
using EmbyStat.Logging;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;

namespace EmbyStat.Controllers.Account
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(IAccountService accountService, IHttpContextAccessor httpContextAccessor)
        {
            _accountService = accountService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] AuthenticateRequest login)
        {
            if (login != null)
            {
                var remoteIp = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
                var result = await _accountService.Authenticate(login, remoteIp);

                if (result != null)
                {
                    return Ok(result);
                }
            }

            return BadRequest("Invalid username or password");
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] AuthenticateRequest register)
        {
            if (register == null)
            {
                return Unauthorized("User registration not allowed");
            }

            if (!_accountService.AnyAdmins())
            {
                await _accountService.Register(register);
                return Ok(true);
            }

            var user = Request.HttpContext.User;
            var identities = user?.Identities.ToArray();
            // We need a check if user is an admin here!
            if (identities == null || identities.Length != 1 || !identities[0].IsAuthenticated)
            {
                return Unauthorized("User registration not allowed");
            }

            await _accountService.Register(register);
            return Ok(true);

        }

        [HttpPost]
        [Authorize]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogOut();
            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("refreshtoken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refresh)
        {
            var remoteIp = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            var result = await _accountService.RefreshToken(refresh.AccessToken, refresh.RefreshToken, remoteIp);

            return result != null ? Ok(result) : Ok(null);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("any")]
        public IActionResult AnyAdmins()
        {
            var result = _accountService.AnyAdmins();
            return Ok(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("reset/password/{username}")]
        public async Task<IActionResult> ResetPassword([FromRoute] string username)
        {
            try
            {
                var result = await _accountService.ResetPassword(username);
                return Ok(result);
            }
            catch
            {
                return StatusCode(500);
            }
            
        }

        [HttpPost]
        [Authorize]
        [Route("change/password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var result = await _accountService.ChangePassword(request);
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        [Route("change/username")]
        public async Task<IActionResult> ChangeUsername([FromBody] ChangeUserNameRequest request)
        {
            var result = await _accountService.ChangeUserName(request);
            return Ok(result);
        }
    }
}