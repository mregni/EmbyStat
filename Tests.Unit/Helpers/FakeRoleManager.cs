using System;
using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Unit.Helpers;

public class FakeRoleManager : RoleManager<IdentityRole>
{
    public FakeRoleManager() 
        : base(new Mock<IRoleStore<IdentityRole>>().Object, 
            Array.Empty<IRoleValidator<IdentityRole>>(),
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<ILogger<RoleManager<IdentityRole>>>().Object
            )
    {
    }
}