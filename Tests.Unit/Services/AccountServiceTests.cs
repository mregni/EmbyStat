using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using EmbyStat.Common;
using EmbyStat.Common.Models.Account;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Core.Account;
using EmbyStat.Core.Rollbar.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Tests.Unit.Helpers;
using Xunit;

namespace Tests.Unit.Services;

public class AccountServiceTests
{
    private readonly AuthenticateRequest _loginRequest;
    private readonly EmbyStatUser _user;
    private readonly AppSettings _appSettings;

    public AccountServiceTests()
    {
        _loginRequest = new AuthenticateRequest
        {
            Password = "testpassword",
            RememberMe = false,
            Username = "testusername"
        };

        _user = new EmbyStatUser
        {
            UserName = _loginRequest.Username
        };

        _appSettings = new AppSettings
        {
            Jwt = new Jwt
            {
                Key = "sdflksjdflsjdf",
                Audience = "EmbyStat",
                Issuer = "EmbyStat",
                AccessExpireMinutes = 120
            }
        };
    }

    [Fact]
    public async Task Authenticate_Should_Return_Valid_User()
    {
        var signInManagerMock = new Mock<FakeSignInManager>();
        signInManagerMock
            .Setup(x => x.PasswordSignInAsync(_loginRequest.Username, _loginRequest.Password,
                _loginRequest.RememberMe,
                false))
            .ReturnsAsync(SignInResult.Success);

        var userManagerMock = new Mock<FakeUserManager>();
        userManagerMock
            .Setup(x => x.FindByNameAsync(_loginRequest.Username))
            .ReturnsAsync(_user);

        var roleManagerMock = new Mock<FakeRoleManager>();

        var settingsServiceMock = new Mock<IRollbarService>();
        settingsServiceMock
            .Setup(x => x.GetAppSettings())
            .Returns(_appSettings);

        var tokenHandler = new Mock<JwtSecurityTokenHandler>();
        tokenHandler
            .Setup(x => x.WriteToken(It.IsAny<SecurityToken>()))
            .Returns("azerty");

        var logger = new Mock<ILogger<AccountService>>();
        var accountService = new AccountService(signInManagerMock.Object, userManagerMock.Object,
            settingsServiceMock.Object, tokenHandler.Object, roleManagerMock.Object, logger.Object);

        var result = await accountService.Authenticate(_loginRequest, "0.0.0.0");
        result.Should().NotBeNull();

        result.Should().NotBeNull();
        result.AccessToken.Should().Be("azerty");
        result.RefreshToken.Should().NotBeEmpty();

        signInManagerMock
            .Verify(
                x => x.PasswordSignInAsync(_loginRequest.Username, _loginRequest.Password, _loginRequest.RememberMe,
                    false), Times.Once);
        userManagerMock
            .Verify(x => x.FindByNameAsync(_loginRequest.Username), Times.Once);
        userManagerMock
            .Verify(x => x.UpdateAsync(It.IsAny<EmbyStatUser>()), Times.Once);
        tokenHandler
            .Verify(x => x.WriteToken(It.IsAny<SecurityToken>()), Times.Once);
    }

    [Fact]
    public async Task Authenticate_Should_Return_Null_If_Auth_Fails()
    {
        var signInManagerMock = new Mock<FakeSignInManager>();
        signInManagerMock
            .Setup(x => x.PasswordSignInAsync(_loginRequest.Username, _loginRequest.Password,
                _loginRequest.RememberMe,
                false))
            .ReturnsAsync(SignInResult.Failed);

        var userManagerMock = new Mock<FakeUserManager>();
        var settingsServiceMock = new Mock<IRollbarService>();
        var tokenHandler = new Mock<JwtSecurityTokenHandler>();
        var roleManagerMock = new Mock<FakeRoleManager>();
        var logger = new Mock<ILogger<AccountService>>();
        
        var accountService = new AccountService(signInManagerMock.Object, userManagerMock.Object,
            settingsServiceMock.Object, tokenHandler.Object, roleManagerMock.Object, logger.Object);

        var result = await accountService.Authenticate(_loginRequest, "0.0.0.0");
        result.Should().BeNull();

        signInManagerMock
            .Verify(x => x.PasswordSignInAsync(_loginRequest.Username, _loginRequest.Password,
                _loginRequest.RememberMe,
                false), Times.Once);
        tokenHandler.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Register_Should_Register_User()
    {
        var signInManagerMock = new Mock<FakeSignInManager>();
        var userManagerMock = new Mock<FakeUserManager>();
        userManagerMock.Setup(x => 
                x.CreateAsync(It.IsAny<EmbyStatUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        var settingsServiceMock = new Mock<IRollbarService>();
        var tokenHandler = new Mock<JwtSecurityTokenHandler>();
        var roleManagerMock = new Mock<FakeRoleManager>();
        var logger = new Mock<ILogger<AccountService>>();
        
        var accountService = new AccountService(signInManagerMock.Object, userManagerMock.Object,
            settingsServiceMock.Object, tokenHandler.Object, roleManagerMock.Object, logger.Object);

        var register = new AuthenticateRequest {Password = "test", Username = "username"};
        await accountService.Register(register);

        userManagerMock.Verify(x => x.CreateAsync(It.IsAny<EmbyStatUser>(), "test"), Times.Once);
        userManagerMock.Verify(x => x.FindByNameAsync("username"), Times.Once);
        userManagerMock.Verify(x => x.ConfirmEmailAsync(It.IsAny<EmbyStatUser>(), It.IsAny<string>()), Times.Once);
        userManagerMock.Verify(x => x.SetLockoutEnabledAsync(It.IsAny<EmbyStatUser>(), It.IsAny<bool>()), Times.Once);
        userManagerMock.Verify(x => x.AddToRolesAsync(It.IsAny<EmbyStatUser>(), new []{ Constants.Roles.Admin, Constants.Roles.User }), Times.Once);
    }

    [Fact]
    public async Task LogOut_Should_Log_Out_User()
    {
        var signInManagerMock = new Mock<FakeSignInManager>();
        signInManagerMock.Setup(x => x.SignOutAsync());
        var userManagerMock = new Mock<FakeUserManager>();
        var settingsServiceMock = new Mock<IRollbarService>();
        var tokenHandler = new Mock<JwtSecurityTokenHandler>();
        var roleManagerMock = new Mock<FakeRoleManager>();
        var logger = new Mock<ILogger<AccountService>>();
        
        var accountService = new AccountService(signInManagerMock.Object, userManagerMock.Object,
            settingsServiceMock.Object, tokenHandler.Object, roleManagerMock.Object, logger.Object);

        await accountService.LogOut();

        signInManagerMock.Verify(x => x.SignOutAsync(), Times.Once);
    }

    [Fact]
    public async Task ChangePassword_Should_Change_Password()
    {
        var signInManagerMock = new Mock<FakeSignInManager>();
        var userManagerMock = new Mock<FakeUserManager>();
        userManagerMock
            .Setup(x => x.FindByNameAsync("test"))
            .ReturnsAsync(new EmbyStatUser {UserName = "test"});
        userManagerMock
            .Setup(x => x.ChangePasswordAsync(It.IsAny<EmbyStatUser>(), "oldpass", "newpass"))
            .ReturnsAsync(IdentityResult.Success);

        var settingsServiceMock = new Mock<IRollbarService>();
        var tokenHandler = new Mock<JwtSecurityTokenHandler>();
        var roleManagerMock = new Mock<FakeRoleManager>();
        var logger = new Mock<ILogger<AccountService>>();
        
        var accountService = new AccountService(signInManagerMock.Object, userManagerMock.Object,
            settingsServiceMock.Object, tokenHandler.Object, roleManagerMock.Object, logger.Object);

        var request = new ChangePasswordRequest
        {
            NewPassword = "newpass",
            OldPassword = "oldpass",
            UserName = "test"
        };
        var result = await accountService.ChangePassword(request);
        result.Should().BeTrue();

        userManagerMock.Verify(x => x.FindByNameAsync("test"), Times.Once);
        userManagerMock.Verify(x => x.ChangePasswordAsync(It.IsAny<EmbyStatUser>(), "oldpass", "newpass"),
            Times.Once);
    }

    [Fact]
    public async Task ChangePassword_Should_Fail_If_User_Is_Unknown()
    {
        var signInManagerMock = new Mock<FakeSignInManager>();
        var userManagerMock = new Mock<FakeUserManager>();
        userManagerMock
            .Setup(x => x.FindByNameAsync("test"))
            .ReturnsAsync((EmbyStatUser) null);
        var settingsServiceMock = new Mock<IRollbarService>();
        var tokenHandler = new Mock<JwtSecurityTokenHandler>();
        var roleManagerMock = new Mock<FakeRoleManager>();
        var logger = new Mock<ILogger<AccountService>>();
        
        var accountService = new AccountService(signInManagerMock.Object, userManagerMock.Object,
            settingsServiceMock.Object, tokenHandler.Object, roleManagerMock.Object, logger.Object);

        var request = new ChangePasswordRequest
        {
            NewPassword = "newpass",
            OldPassword = "oldpass",
            UserName = "test2"
        };
        var result = await accountService.ChangePassword(request);
        result.Should().BeFalse();

        userManagerMock.Verify(x => x.FindByNameAsync("test2"), Times.Once);
        userManagerMock.Verify(
            x => x.ChangePasswordAsync(It.IsAny<EmbyStatUser>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task ChangePassword_Should_Fail_If_Change_Fails()
    {
        var signInManagerMock = new Mock<FakeSignInManager>();
        var userManagerMock = new Mock<FakeUserManager>();
        userManagerMock
            .Setup(x => x.FindByNameAsync("test"))
            .ReturnsAsync(new EmbyStatUser {UserName = "test"});
        userManagerMock
            .Setup(x => x.ChangePasswordAsync(It.IsAny<EmbyStatUser>(), "oldpass", "newpass"))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError()));

        var settingsServiceMock = new Mock<IRollbarService>();
        var tokenHandler = new Mock<JwtSecurityTokenHandler>();
        var roleManagerMock = new Mock<FakeRoleManager>();
        var logger = new Mock<ILogger<AccountService>>();
        
        var accountService = new AccountService(signInManagerMock.Object, userManagerMock.Object,
            settingsServiceMock.Object, tokenHandler.Object, roleManagerMock.Object, logger.Object);

        var request = new ChangePasswordRequest
        {
            NewPassword = "newpass",
            OldPassword = "oldpass",
            UserName = "test"
        };
        var result = await accountService.ChangePassword(request);
        result.Should().BeFalse();

        userManagerMock.Verify(x => x.FindByNameAsync("test"), Times.Once);
        userManagerMock.Verify(x => x.ChangePasswordAsync(It.IsAny<EmbyStatUser>(), "oldpass", "newpass"),
            Times.Once);
    }

    [Fact]
    public async Task ChangeUserName_Should_Change_UserName()
    {
        var signInManagerMock = new Mock<FakeSignInManager>();
        var userManagerMock = new Mock<FakeUserManager>();
        userManagerMock
            .Setup(x => x.FindByNameAsync("test"))
            .ReturnsAsync(new EmbyStatUser {UserName = "test"});
        userManagerMock
            .Setup(x => x.SetUserNameAsync(It.IsAny<EmbyStatUser>(), "test2"))
            .ReturnsAsync(IdentityResult.Success);

        var settingsServiceMock = new Mock<IRollbarService>();
        var tokenHandler = new Mock<JwtSecurityTokenHandler>();
        var roleManagerMock = new Mock<FakeRoleManager>();
        var logger = new Mock<ILogger<AccountService>>();
        
        var accountService = new AccountService(signInManagerMock.Object, userManagerMock.Object,
            settingsServiceMock.Object, tokenHandler.Object, roleManagerMock.Object, logger.Object);

        var request = new ChangeUserNameRequest
        {
            OldUserName = "test",
            UserName = "test2"
        };
        var result = await accountService.ChangeUserName(request);
        result.Should().BeTrue();

        userManagerMock.Verify(x => x.FindByNameAsync("test"), Times.Once);
        userManagerMock.Verify(x => x.SetUserNameAsync(It.IsAny<EmbyStatUser>(), "test2"), Times.Once);
    }

    [Fact]
    public async Task ChangeUserName_Should_Fail_If_User_Is_Unknown()
    {
        var signInManagerMock = new Mock<FakeSignInManager>();
        var userManagerMock = new Mock<FakeUserManager>();
        userManagerMock
            .Setup(x => x.FindByNameAsync("test"))
            .ReturnsAsync((EmbyStatUser) null);

        var settingsServiceMock = new Mock<IRollbarService>();
        var tokenHandler = new Mock<JwtSecurityTokenHandler>();
        var roleManagerMock = new Mock<FakeRoleManager>();
        var logger = new Mock<ILogger<AccountService>>();
        
        var accountService = new AccountService(signInManagerMock.Object, userManagerMock.Object,
            settingsServiceMock.Object, tokenHandler.Object, roleManagerMock.Object, logger.Object);

        var request = new ChangeUserNameRequest
        {
            OldUserName = "test",
            UserName = "test2"
        };
        var result = await accountService.ChangeUserName(request);
        result.Should().BeFalse();

        userManagerMock.Verify(x => x.FindByNameAsync("test"), Times.Once);
        userManagerMock.Verify(x => x.SetUserNameAsync(It.IsAny<EmbyStatUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ChangeUserName_Should_Fail_If_Change_Fails()
    {
        var signInManagerMock = new Mock<FakeSignInManager>();
        var userManagerMock = new Mock<FakeUserManager>();
        userManagerMock
            .Setup(x => x.FindByNameAsync("test"))
            .ReturnsAsync(new EmbyStatUser {UserName = "test"});
        userManagerMock
            .Setup(x => x.SetUserNameAsync(It.IsAny<EmbyStatUser>(), "test2"))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError()));

        var settingsServiceMock = new Mock<IRollbarService>();
        var tokenHandler = new Mock<JwtSecurityTokenHandler>();
        var roleManagerMock = new Mock<FakeRoleManager>();
        var logger = new Mock<ILogger<AccountService>>();
        
        var accountService = new AccountService(signInManagerMock.Object, userManagerMock.Object,
            settingsServiceMock.Object, tokenHandler.Object, roleManagerMock.Object, logger.Object);

        var request = new ChangeUserNameRequest
        {
            OldUserName = "test",
            UserName = "test2"
        };
        var result = await accountService.ChangeUserName(request);
        result.Should().BeFalse();

        userManagerMock.Verify(x => x.FindByNameAsync("test"), Times.Once);
        userManagerMock.Verify(x => x.SetUserNameAsync(It.IsAny<EmbyStatUser>(), "test2"), Times.Once);
    }
}