using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmbyStat.Clients.Emby.Http;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using EmbyStat.Services.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;
using ServerInfo = EmbyStat.Common.Models.Entities.ServerInfo;

namespace Tests.Unit.Services
{
    public class EmbyServiceTests
    {
        private Mock<IMovieRepository> _movieRepositoryMock;
        private Mock<IShowRepository> _showRepositoryMock;
        private Mock<IEmbyRepository> _embyRepositoryMock;
        private Mock<ISessionService> _sessionServiceMock;
        private Mock<ISettingsService> _settingsServiceMock;
        private Mock<IEmbyClient> _embyClientMock;

        public EmbyServiceTests()
        {
            _settingsServiceMock = new Mock<ISettingsService>();
            _settingsServiceMock.Setup(x => x.GetUserSettings()).Returns(new UserSettings
            {
                Emby = new EmbySettings
                {
                    ApiKey = "123",
                    ServerAddress = "localhost",
                    ServerPort = 1,
                    ServerProtocol = ConnectionProtocol.Http
                }
            });
        }

        [Fact]
        public async Task GetServerInfoAsync_Should_Return_Emby_Server_Info()
        {
            _embyClientMock = new Mock<IEmbyClient>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _showRepositoryMock = new Mock<IShowRepository>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _sessionServiceMock = new Mock<ISessionService>();

            var serverInfo = new ServerInfo { Id = "1234" };
            _embyRepositoryMock.Setup(x => x.GetServerInfo()).Returns(serverInfo);

            var service = new EmbyService(_embyClientMock.Object, _embyRepositoryMock.Object, _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object, _showRepositoryMock.Object);
            var result = await service.GetServerInfoAsync();

            result.Should().NotBeNull();
            result.Id.Should().Be(serverInfo.Id);

            _embyRepositoryMock.Verify(x => x.GetServerInfo(), Times.Once);
        }

        [Fact]
        public async Task GetServerInfoAsync_Should_Return_Emby_Server_Info_And_Fetch_It_From_Emby()
        {
            _embyClientMock = new Mock<IEmbyClient>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _showRepositoryMock = new Mock<IShowRepository>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _sessionServiceMock = new Mock<ISessionService>();

            _embyRepositoryMock.Setup(x => x.GetServerInfo()).Returns((ServerInfo)null);
            _embyRepositoryMock.Setup(x => x.UpsertServerInfo(It.IsAny<ServerInfo>()));

            var serverInfo = new ServerInfo { Id = Guid.NewGuid().ToString() };
            _embyClientMock.Setup(x => x.GetServerInfoAsync()).ReturnsAsync(serverInfo);

            var service = new EmbyService(_embyClientMock.Object, _embyRepositoryMock.Object, _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object, _showRepositoryMock.Object);
            var result = await service.GetServerInfoAsync();

            result.Should().NotBeNull();
            result.Id.Should().Be(serverInfo.Id);

            _embyRepositoryMock.Verify(x => x.GetServerInfo(), Times.Once);
            _embyRepositoryMock.Verify(x => x.UpsertServerInfo(It.IsAny<ServerInfo>()), Times.Once);
            _embyClientMock.Verify(x => x.GetServerInfoAsync(), Times.Once);
        }

        [Fact]
        public void GetEmbyStatus_Should_Return_Emby_Status_Object()
        {
            _embyClientMock = new Mock<IEmbyClient>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _showRepositoryMock = new Mock<IShowRepository>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _sessionServiceMock = new Mock<ISessionService>();

            var embyStatus = new EmbyStatus() { Id = Guid.NewGuid() };
            _embyRepositoryMock.Setup(x => x.GetEmbyStatus()).Returns(embyStatus);

            var service = new EmbyService(_embyClientMock.Object, _embyRepositoryMock.Object, _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object, _showRepositoryMock.Object);
            var result = service.GetEmbyStatus();

            result.Should().NotBeNull();
            result.Id.Should().Be(embyStatus.Id);

            _embyRepositoryMock.Verify(x => x.GetEmbyStatus(), Times.Once);
        }

        [Fact]
        public void ResetMissedPings_Should_Return_Reset_Missed_Pings()
        {
            _embyClientMock = new Mock<IEmbyClient>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _showRepositoryMock = new Mock<IShowRepository>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _sessionServiceMock = new Mock<ISessionService>();

            _embyRepositoryMock.Setup(x => x.ResetMissedPings());

            var service = new EmbyService(_embyClientMock.Object, _embyRepositoryMock.Object, _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object, _showRepositoryMock.Object);
            service.ResetMissedPings();

            _embyRepositoryMock.Verify(x => x.ResetMissedPings(), Times.Once);
        }

        [Fact]
        public void IncreaseMissedPings_Should_Return_Increased_Missed_Pings()
        {
            _embyClientMock = new Mock<IEmbyClient>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _showRepositoryMock = new Mock<IShowRepository>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _sessionServiceMock = new Mock<ISessionService>();

            _embyRepositoryMock.Setup(x => x.ResetMissedPings());

            var service = new EmbyService(_embyClientMock.Object, _embyRepositoryMock.Object, _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object, _showRepositoryMock.Object);
            service.IncreaseMissedPings();

            _embyRepositoryMock.Verify(x => x.IncreaseMissedPings(), Times.Once);
        }

        [Fact]
        public async Task PingEmbyAsync_Should_Ping_Emby_Server()
        {
            _embyClientMock = new Mock<IEmbyClient>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _showRepositoryMock = new Mock<IShowRepository>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _sessionServiceMock = new Mock<ISessionService>();

            _embyClientMock.Setup(x => x.PingEmbyAsync()).ReturnsAsync("Emby Server");

            var service = new EmbyService(_embyClientMock.Object, _embyRepositoryMock.Object, _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object, _showRepositoryMock.Object);
            var result = await service.PingEmbyAsync("localhost:9000");
            result.Should().NotBeNull();
            result.Should().Be("Emby Server");

            _embyClientMock.Verify(x => x.PingEmbyAsync(), Times.Once);
        }

        [Fact]
        public async Task TestNewEmbyApiKey_Should_Return_True()
        {
            _embyClientMock = new Mock<IEmbyClient>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _showRepositoryMock = new Mock<IShowRepository>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _sessionServiceMock = new Mock<ISessionService>();

            _embyClientMock.Setup(x => x.GetServerInfoAsync()).ReturnsAsync(new ServerInfo());
            _embyClientMock.SetupSet(x => x.ApiKey = "1234").Verifiable();
            _embyClientMock.SetupSet(x => x.BaseUrl = "localhost:9000").Verifiable();

            _embyClientMock.SetupGet(x => x.ApiKey).Returns("1234");
            _embyClientMock.SetupGet(x => x.BaseUrl).Returns("localhost:9000");

            var service = new EmbyService(_embyClientMock.Object, _embyRepositoryMock.Object, _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object, _showRepositoryMock.Object);
            var result = await service.TestNewEmbyApiKeyAsync("localhost:9000", "1234");

            result.Should().BeTrue();

            _embyClientMock.VerifyGet(x => x.ApiKey, Times.Once);
            _embyClientMock.VerifyGet(x => x.BaseUrl, Times.Once);

            _embyClientMock.VerifySet(x => x.ApiKey = "1234");
            _embyClientMock.VerifySet(x => x.BaseUrl = "localhost:9000");

            _embyClientMock.Verify(x => x.GetServerInfoAsync(), Times.Once);
        }

        [Fact]
        public async Task TestNewEmbyApiKey_Should_Return_False()
        {
            _embyClientMock = new Mock<IEmbyClient>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _showRepositoryMock = new Mock<IShowRepository>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _sessionServiceMock = new Mock<ISessionService>();

            _embyClientMock.Setup(x => x.GetServerInfoAsync()).ReturnsAsync((ServerInfo)null);
            _embyClientMock.SetupSet(x => x.ApiKey = "1234").Verifiable();
            _embyClientMock.SetupSet(x => x.BaseUrl = "localhost:9000").Verifiable();

            _embyClientMock.SetupGet(x => x.ApiKey).Returns("12345");
            _embyClientMock.SetupGet(x => x.BaseUrl).Returns("localhost:9001");

            var service = new EmbyService(_embyClientMock.Object, _embyRepositoryMock.Object, _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object, _showRepositoryMock.Object);
            var result = await service.TestNewEmbyApiKeyAsync("localhost:9000", "1234");

            result.Should().BeFalse();

            _embyClientMock.VerifyGet(x => x.ApiKey, Times.Once);
            _embyClientMock.VerifyGet(x => x.BaseUrl, Times.Once);

            _embyClientMock.VerifySet(x => x.ApiKey = "1234", Times.Once);
            _embyClientMock.VerifySet(x => x.BaseUrl = "localhost:9000", Times.Once);

            _embyClientMock.VerifySet(x => x.ApiKey = "12345", Times.Once);
            _embyClientMock.VerifySet(x => x.BaseUrl = "localhost:9001", Times.Once);

            _embyClientMock.Verify(x => x.GetServerInfoAsync(), Times.Once);
        }

        [Fact]
        public void GetAllPlugins_Should_Return_List_Of_Plugins()
        {
            _embyClientMock = new Mock<IEmbyClient>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _showRepositoryMock = new Mock<IShowRepository>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _sessionServiceMock = new Mock<ISessionService>();

            var plugins = new List<PluginInfo> { new PluginInfo { Id = "123" }, new PluginInfo { Id = "234" } };
            _embyRepositoryMock.Setup(x => x.GetAllPlugins()).Returns(plugins);

            var service = new EmbyService(_embyClientMock.Object, _embyRepositoryMock.Object, _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object, _showRepositoryMock.Object);
            var result = service.GetAllPlugins();

            result.Should().NotContainNulls();
            result.Count.Should().Be(2);

            _embyRepositoryMock.Verify(x => x.GetAllPlugins(), Times.Once);
        }

        [Fact]
        public void GetAllUsers_Should_Return_List_Of_Users()
        {
            _embyClientMock = new Mock<IEmbyClient>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _showRepositoryMock = new Mock<IShowRepository>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _sessionServiceMock = new Mock<ISessionService>();

            var users = new List<EmbyUser>() { new EmbyUser(), new EmbyUser() };
            _embyRepositoryMock.Setup(x => x.GetAllUsers()).Returns(users);

            var service = new EmbyService(_embyClientMock.Object, _embyRepositoryMock.Object, _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object, _showRepositoryMock.Object);
            var result = service.GetAllUsers().ToList();

            result.Should().NotContainNulls();
            result.Count.Should().Be(2);

            _embyRepositoryMock.Verify(x => x.GetAllUsers(), Times.Once);
        }

        [Fact]
        public void GetUserById_Should_Return_Correct_User()
        {
            _embyClientMock = new Mock<IEmbyClient>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _showRepositoryMock = new Mock<IShowRepository>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _sessionServiceMock = new Mock<ISessionService>();

            var users = new List<EmbyUser> { new EmbyUser { Id = "1" }, new EmbyUser { Id = "2" } };
            _embyRepositoryMock.Setup(x => x.GetUserById("1")).Returns(users[0]);
            _embyRepositoryMock.Setup(x => x.GetUserById("2")).Returns(users[1]);

            var service = new EmbyService(_embyClientMock.Object, _embyRepositoryMock.Object, _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object, _showRepositoryMock.Object);
            var result = service.GetUserById("1");

            result.Should().NotBeNull();
            result.Id.Should().Be("1");

            _embyRepositoryMock.Verify(x => x.GetUserById("1"), Times.Once);
        }

        [Fact]
        public void GetViewedEpisodeCountByUserId_Should_Return_Total_Episode_View_Count()
        {
            _embyClientMock = new Mock<IEmbyClient>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _showRepositoryMock = new Mock<IShowRepository>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _sessionServiceMock = new Mock<ISessionService>();

            _sessionServiceMock.Setup(x => x.GetMediaIdsForUser(It.IsAny<string>(), It.IsAny<PlayType>()))
                .Returns(new[] { "12", "13", "14" });

            var service = new EmbyService(_embyClientMock.Object, _embyRepositoryMock.Object, _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object, _showRepositoryMock.Object);
            var result = service.GetViewedEpisodeCountByUserId("1");

            result.Should().NotBeNull();
            result.Title.Should().Be(Constants.Users.TotalWatchedEpisodes);
            result.Value.Should().Be(3);

            _sessionServiceMock.Verify(x => x.GetMediaIdsForUser("1", PlayType.Episode), Times.Once);
        }

        [Fact]
        public void GetViewedMovieCountByUserId_Should_Return_Total_Movie_View_Count()
        {
            _embyClientMock = new Mock<IEmbyClient>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _showRepositoryMock = new Mock<IShowRepository>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _sessionServiceMock = new Mock<ISessionService>();

            _sessionServiceMock.Setup(x => x.GetMediaIdsForUser(It.IsAny<string>(), It.IsAny<PlayType>()))
                .Returns(new[] { "12", "13", "14" });

            var service = new EmbyService(_embyClientMock.Object, _embyRepositoryMock.Object, _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object, _showRepositoryMock.Object);
            var result = service.GetViewedMovieCountByUserId("1");

            result.Should().NotBeNull();
            result.Title.Should().Be(Constants.Users.TotalWatchedMovies);
            result.Value.Should().Be(3);

            _sessionServiceMock.Verify(x => x.GetMediaIdsForUser("1", PlayType.Movie), Times.Once);
        }

        [Fact]
        public void GetUserViewCount_Should_Return_User_View_Count()
        {
            _embyClientMock = new Mock<IEmbyClient>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _showRepositoryMock = new Mock<IShowRepository>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _sessionServiceMock = new Mock<ISessionService>();

            _sessionServiceMock.Setup(x => x.GetPlayCountForUser(It.IsAny<string>())).Returns(12);

            var service = new EmbyService(_embyClientMock.Object, _embyRepositoryMock.Object, _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object, _showRepositoryMock.Object);
            var result = service.GetUserViewCount("1");

            result.Should().Be(12);

            _sessionServiceMock.Verify(x => x.GetPlayCountForUser("1"), Times.Once);
        }

        [Fact]
        public async Task GetAndProcessServerInfoAsync_Should_Save_Server_Info()
        {
            _embyClientMock = new Mock<IEmbyClient>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _showRepositoryMock = new Mock<IShowRepository>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _sessionServiceMock = new Mock<ISessionService>();

            var serverInfo = new ServerInfo { Id = "1234" };
            _embyClientMock.Setup(x => x.GetServerInfoAsync()).ReturnsAsync(serverInfo);
            _embyRepositoryMock.Setup(x => x.UpsertServerInfo(It.IsAny<ServerInfo>()));

            var service = new EmbyService(_embyClientMock.Object, _embyRepositoryMock.Object, _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object, _showRepositoryMock.Object);
            var result = await service.GetAndProcessServerInfoAsync();

            result.Should().NotBeNull();
            result.Id.Should().Be(serverInfo.Id);

            _embyClientMock.Verify(x => x.GetServerInfoAsync(), Times.Once);
            _embyRepositoryMock.Verify(x => x.UpsertServerInfo(serverInfo), Times.Once);

        }

        [Fact]
        public async Task GetAndProcessPluginInfoAsync_Should_Save_Plugins()
        {
            _embyClientMock = new Mock<IEmbyClient>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _showRepositoryMock = new Mock<IShowRepository>();
            _embyRepositoryMock = new Mock<IEmbyRepository>();
            _sessionServiceMock = new Mock<ISessionService>();

            var plugins = new List<PluginInfo> { new PluginInfo(), new PluginInfo() };
            _embyClientMock.Setup(x => x.GetInstalledPluginsAsync()).ReturnsAsync(plugins);

            _embyRepositoryMock.Setup(x => x.RemoveAllAndInsertPluginRange(plugins));

            var service = new EmbyService(_embyClientMock.Object, _embyRepositoryMock.Object, _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object, _showRepositoryMock.Object);
            await service.GetAndProcessPluginInfoAsync();

            _embyClientMock.Verify(x => x.GetInstalledPluginsAsync(), Times.Once);
            _embyRepositoryMock.Verify(x => x.RemoveAllAndInsertPluginRange(plugins), Times.Once);

        }
    }
}