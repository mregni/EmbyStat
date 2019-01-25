using System.Collections.Generic;
using System.Linq;
using EmbyStat.Clients.EmbyClient;
using EmbyStat.Repositories;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using FluentAssertions;
using MediaBrowser.Model.Plugins;
using Moq;
using Xunit;

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

			_subject = new EmbyService(new Mock<IEmbyClient>().Object, new Mock<IConfigurationRepository>().Object, _embyRepositoryMock.Object);
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
