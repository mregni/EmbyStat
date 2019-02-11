using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EmbyStat.Clients.EmbyClient;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using EmbyStat.Services.Interfaces;
using FluentAssertions;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.System;
using Moq;
using Xunit;
using PluginInfo = EmbyStat.Common.Models.Entities.PluginInfo;

namespace Tests.Unit.Services
{
	[Collection("Services collection")]
	public class PluginServiceTests
    {
	    private readonly EmbyService _subject;
	    private readonly List<PluginInfo> _plugins;
	    private readonly Mock<IEmbyRepository> _embyRepositoryMock;
		public PluginServiceTests()
	    {
			_plugins = new List<PluginInfo>
			{
				new PluginInfo { Name = "EmbyStat plugin" },
				new PluginInfo { Name = "Trakt plugin" }
			};

		    _embyRepositoryMock = new Mock<IEmbyRepository>();
		    _embyRepositoryMock.Setup(x => x.GetAllPlugins()).Returns(_plugins);

            var _mapperMock = new Mock<IMapper>();
            _subject = new EmbyService(new Mock<IEmbyClient>().Object, new Mock<ISettingsService>().Object, _embyRepositoryMock.Object, _mapperMock.Object);
		}

	    [Fact]
	    public void GetPluginsFromDatabase()
	    {
		    var plugins = _subject.GetAllPlugins();

		    plugins.Should().NotBeNull();
		    plugins.Count.Should().Be(2);
		    plugins.First().Name.Should().Be(_plugins.First().Name);
		    plugins.Skip(1).First().Name.Should().Be(_plugins.Skip(1).First().Name);

		    _embyRepositoryMock.Verify(x => x.GetAllPlugins(), Times.Once);
	    }
	}
}
