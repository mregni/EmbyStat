using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Controllers.Emby;
using EmbyStat.Controllers.System;
using EmbyStat.Services.Emby;
using EmbyStat.Services.Emby.Models;
using EmbyStat.Services.System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Tests.Unit.Controllers
{
	[Collection("Controller collection")]
	public class SystemControllerTests : IDisposable
    {
	    private readonly SystemController _subject;
	    private readonly Mock<ISystemService> _systemServiceMock;

	    public SystemControllerTests()
	    {
		    _systemServiceMock = new Mock<ISystemService>();
		    _systemServiceMock.Setup(x => x.StartShutdownJob());

		    var loggerMock = new Mock<ILogger<SystemController>>();

		    _subject = new SystemController(_systemServiceMock.Object, loggerMock.Object);
	    }

	    public void Dispose()
	    {
		    _subject?.Dispose();
		}

	    [Fact]
	    public void IsEmbyTokenReturned()
	    {
		    _subject.Shutdown();

		    _systemServiceMock.Verify(x => x.StartShutdownJob(), Times.Once);
		}
	}
}
