using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Clients.Emby.Http;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Events;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Emby;
using FluentAssertions;
using MediaBrowser.Model.IO;
using Moq;
using Xunit;
using PluginInfo = EmbyStat.Common.Models.Entities.PluginInfo;

namespace Tests.Unit.Services
{
    [Collection("Mapper collection")]
    public class EmbyServiceTests
    {
        private readonly EmbyService _subject;
        private readonly Mock<IEmbyClient> _embyClientMock;
        private readonly ServerInfo _serverInfo;
        private List<string> userIds => new List<string> { "915335F2-FDCA-41E4-8CCC-E93C03C23702", "C8DE0A2D-F152-46C5-A5E0-ACF6B4D58C6B" };

        public EmbyServiceTests()
        {
            var plugins = new List<PluginInfo>
            {
                new PluginInfo { Name = "EmbyStat plugin" },
                new PluginInfo { Name = "Trakt plugin" }
            };

            var embyPlugins = new List<MediaBrowser.Model.Plugins.PluginInfo>
            {
                new MediaBrowser.Model.Plugins.PluginInfo {Name = "EmbyStat plugin"},
                new MediaBrowser.Model.Plugins.PluginInfo {Name = "Trakt plugin"}
            };

            _serverInfo = new ServerInfo
            {
                Id = Guid.NewGuid().ToString(),
                HttpServerPortNumber = 8096,
                HttpsPortNumber = 8097
            };

            var embyDrives = new List<FileSystemEntryInfo>
            {
                new FileSystemEntryInfo()
            };

            var systemInfo = new ServerInfo();

            _embyClientMock = new Mock<IEmbyClient>();
            _embyClientMock.Setup(x => x.GetInstalledPluginsAsync()).Returns(Task.FromResult(embyPlugins));
            _embyClientMock.Setup(x => x.AuthenticateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            _embyClientMock.Setup(x => x.GetServerInfoAsync()).Returns(Task.FromResult(systemInfo));
            _embyClientMock.Setup(x => x.GetLocalDrivesAsync()).Returns(Task.FromResult(embyDrives));

            var embyRepositoryMock = new Mock<IEmbyRepository>();
            embyRepositoryMock.Setup(x => x.GetAllPlugins()).Returns(plugins);
            embyRepositoryMock.Setup(x => x.RemoveAllAndInsertPluginRange(It.IsAny<List<PluginInfo>>()));
            embyRepositoryMock.Setup(x => x.AddOrUpdateServerInfo(It.IsAny<ServerInfo>()));
            embyRepositoryMock.Setup(x => x.GetServerInfo()).Returns(_serverInfo);
            embyRepositoryMock.SetupSequence(x => x.GetUserById(It.IsAny<string>()))
                .Returns(new User { Name = "admin" })
                .Returns(new User { Name = "reggi" });


            var settingsServiceMock = new Mock<ISettingsService>();
            settingsServiceMock.Setup(x => x.GetUserSettings()).Returns(new UserSettings());

            var movieRepositoryMock = new Mock<IMovieRepository>();
            var showRepositoryMock = new Mock<IShowRepository>();

            var sessionServiceMock = new Mock<ISessionService>();
            sessionServiceMock.Setup(x => x.GetPlayStatesForUser(userIds[0])).Returns(new List<PlayState>()
            {
                new PlayState {TimeLogged = new DateTime(0).AddHours(10).AddMinutes(25)},
                new PlayState {TimeLogged = new DateTime(0).AddHours(10).AddMinutes(26)},
                new PlayState {TimeLogged = new DateTime(0).AddHours(11).AddMinutes(25)},
            });
            sessionServiceMock.Setup(x => x.GetPlayStatesForUser(userIds[1])).Returns(new List<PlayState>()
            {
                new PlayState {TimeLogged = new DateTime(0).AddHours(11).AddMinutes(25)},
                new PlayState {TimeLogged = new DateTime(0).AddHours(11).AddMinutes(26)},
                new PlayState {TimeLogged = new DateTime(0).AddHours(12).AddMinutes(25)},
            });

            _subject = new EmbyService(_embyClientMock.Object, embyRepositoryMock.Object, sessionServiceMock.Object,
                settingsServiceMock.Object, movieRepositoryMock.Object, showRepositoryMock.Object);
        }


        [Fact]
        public async void GetEmbyTokenWithNoLoginInfo()
        {
            var ex = await Assert.ThrowsAsync<BusinessException>(() => _subject.GetEmbyToken(null));

            ex.Message.Should().Be("TOKEN_FAILED");
            ex.StatusCode.Should().Be(500);
        }

        [Fact]
        public async void GetEmbyTokenWithNoPassword()
        {
            var login = new EmbyLogin
            {
                UserName = "Admin",
                Address = "http://localhost"
            };
            var ex = await Assert.ThrowsAsync<BusinessException>(() => _subject.GetEmbyToken(login));

            ex.Message.Should().Be("TOKEN_FAILED");
            ex.StatusCode.Should().Be(500);
        }

        [Fact]
        public async void GetEmbyTokenWithNoUserName()
        {
            var login = new EmbyLogin
            {
                Password = "AdminPass",
                Address = "http://localhost"
            };
            var ex = await Assert.ThrowsAsync<BusinessException>(() => _subject.GetEmbyToken(login));

            ex.Message.Should().Be("TOKEN_FAILED");
            ex.StatusCode.Should().Be(500);
        }

        [Fact]
        public async void GetEmbyTokenFailedLogin()
        {
            _embyClientMock.Setup(x => x.AuthenticateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception());
            var login = new EmbyLogin
            {
                Password = "AdminPass",
                Address = "http://localhost",
                UserName = "Admin"
            };
            var ex = await Assert.ThrowsAsync<BusinessException>(() => _subject.GetEmbyToken(login));

            ex.Message.Should().Be("TOKEN_FAILED");
            ex.StatusCode.Should().Be(500);
        }

        [Fact]
        public void GenerateHourOfDayGraphForMultipleUsers()
        {
            var graph = _subject.GenerateHourOfDayGraph(userIds);

            graph.Labels.Length.Should().Be(24);
            graph.Title.Should().Be("GRAPH.TITLE");
            graph.DataSets.Count.Should().Be(2);
            graph.DataSets[0].Label.Should().Be("admin");
            graph.DataSets[0].Data.Length.Should().Be(24);
            graph.DataSets[0].Data[9].Should().Be(0);
            graph.DataSets[0].Data[10].Should().Be(33.3);
            graph.DataSets[0].Data[11].Should().Be(16.7);
            graph.DataSets[0].Data[12].Should().Be(0);
            graph.DataSets[1].Label.Should().Be("reggi");
            graph.DataSets[1].Data.Length.Should().Be(24);
            graph.DataSets[1].Data[9].Should().Be(0);
            graph.DataSets[1].Data[10].Should().Be(0);
            graph.DataSets[1].Data[11].Should().Be(33.3);
            graph.DataSets[1].Data[12].Should().Be(16.7);
            graph.DataSets[1].Data[13].Should().Be(0);
        }
    }
}