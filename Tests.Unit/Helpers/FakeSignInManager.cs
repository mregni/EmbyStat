using EmbyStat.Common.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Tests.Unit.Helpers;

public class FakeSignInManager : SignInManager<EmbyStatUser>
{
    public FakeSignInManager()
        : base(new FakeUserManager(),
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<EmbyStatUser>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<ILogger<SignInManager<EmbyStatUser>>>().Object,
            new Mock<IAuthenticationSchemeProvider>().Object,
            new Mock<IUserConfirmation<EmbyStatUser>>().Object)
    { }
}