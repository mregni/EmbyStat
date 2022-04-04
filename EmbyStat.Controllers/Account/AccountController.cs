using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Common.Models.Account;
using EmbyStat.Logging;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmbyStat.Controllers.Account
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly Logger _logger;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
            _logger = LogFactory.CreateLoggerForType(typeof(AccountController), "ACCOUNT");
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
                _logger.Info($"Username {login.Username} was used for login.");
            }

            _logger.Info($"Invalid username or password. ");
            return BadRequest("Invalid username or password");
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] AuthenticateRequest register)
        {
            if (register == null)
            {
                return BadRequest("No authentication request provided");
            }

            if (!await _accountService.AnyAdmins())
            {
                await _accountService.Register(register);
                return Ok(true);
            }

            var user = Request.HttpContext.User;
            var identities = user?.Identities.ToArray();
            // We need a check if user is an admin here!
            if (identities.Length != 1 || !identities[0].IsAuthenticated)
            {
                _logger.Warn("User registration not allowed");
                _logger.Warn("This is because there is already an admin user in the database but the identity is not authenticated. (You can't create a second user for the moment)");
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
        public async Task<IActionResult> AnyAdmins()
        {
            var result = await _accountService.AnyAdmins();
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