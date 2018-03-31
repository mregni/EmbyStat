using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Controllers.Plugin;
using EmbyStat.Services.Emby;
using EmbyStat.Services.Plugin;
using FluentAssertions;
using MediaBrowser.Model.Plugins;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Tests.Unit.Controllers
{
	[Collection("Mapper collection")]
	public class PluginControllerTests : IDisposable
	{
		private readonly PluginController _subject;
		private readonly Mock<IPluginService> _pluginServiceMock;
		private readonly List<PluginInfo> _plugins;

		public PluginControllerTests()
		{
			_plugins = new List<PluginInfo>
			{
				new PluginInfo{ Name = "Trakt plugin"},
				new PluginInfo{ Name = "EmbyStat plugin"}
			};

			_pluginServiceMock = new Mock<IPluginService>();
			_pluginServiceMock.Setup(x => x.GetInstalledPlugins()).Returns(_plugins);
			var loggerMock = new Mock<ILogger<PluginController>>();

			_subject = new PluginController(_pluginServiceMock.Object, loggerMock.Object);
		}

		public void Dispose()
		{
			_subject?.Dispose();
		}

		[Fact]
		public void ArePluginsReturned()
		{
			var result = _subject.Get();
			var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
			var list = resultObject.Should().BeOfType<List<EmbyPluginViewModel>>().Subject;

			list.Count.Should().Be(2);
			list[0].Name.Should().Be(_plugins[0].Name);
			list[1].Name.Should().Be(_plugins[1].Name);
			_pluginServiceMock.Verify(x => x.GetInstalledPlugins(), Times.Once);
		}	
	}
}
