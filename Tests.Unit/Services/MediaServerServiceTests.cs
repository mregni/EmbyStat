using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Clients.Base;
using EmbyStat.Clients.Base.Http;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Net;
using EmbyStat.Common.Models.Settings;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services;
using EmbyStat.Services.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests.Unit.Services
{
    public class MediaServerServiceTests
    {
        private Mock<IMovieRepository> _movieRepositoryMock;
        private Mock<IShowRepository> _showRepositoryMock;
        private Mock<IMediaServerRepository> _embyRepositoryMock;
        private Mock<ISessionService> _sessionServiceMock;
        private readonly Mock<ISettingsService> _settingsServiceMock;
        private Mock<IBaseHttpClient> _httpClientMock;

        public MediaServerServiceTests()
        {
            _settingsServiceMock = new Mock<ISettingsService>();
            _settingsServiceMock.Setup(x => x.GetUserSettings()).Returns(new UserSettings
            {
                MediaServer = new MediaServerSettings
                {
                    ApiKey = "123",
                    Address = "http://localhost:1"
                }
            });

            _settingsServiceMock.Setup(x => x.GetAppSettings()).Returns(new AppSettings()
            {
                Version = "0.0.0.0"
            });
        }

        [Fact]
        public async Task GetServerInfo_Should_Return_Emby_Server_Info()
        {
            _httpClientMock = new Mock<IBaseHttpClient>();
            _embyRepositoryMock = new Mock<IMediaServerRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _showRepositoryMock = new Mock<IShowRepository>();
            _embyRepositoryMock = new Mock<IMediaServerRepository>();
            _sessionServiceMock = new Mock<ISessionService>();

            var serverInfo = new MediaServerInfo {Id = "1234"};
            _embyRepositoryMock
                .Setup(x => x.GetServerInfo())
                .ReturnsAsync(serverInfo);

            var strategy = new Mock<IClientStrategy>();
            strategy.Setup(x => x.CreateHttpClient(It.IsAny<ServerType>())).Returns(_httpClientMock.Object);

            var mapperMock = new Mock<IMapper>();

            var service = new MediaServerService(strategy.Object, _embyRepositoryMock.Object,
                _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object,
                mapperMock.Object);
            var result = await service.GetServerInfo(false);

            result.Should().NotBeNull();
            result.Id.Should().Be(serverInfo.Id);

            _embyRepositoryMock.Verify(x => x.GetServerInfo(), Times.Once);
        }

        [Fact]
        public async Task GetServerInfo_Should_Return_Emby_Server_Info_And_Fetch_It_From_Emby()
        {
            _httpClientMock = new Mock<IBaseHttpClient>();
            _embyRepositoryMock = new Mock<IMediaServerRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _showRepositoryMock = new Mock<IShowRepository>();
            _embyRepositoryMock = new Mock<IMediaServerRepository>();
            _sessionServiceMock = new Mock<ISessionService>();

            _embyRepositoryMock.Setup(x => x.GetServerInfo()).ReturnsAsync((MediaServerInfo) null);
            _embyRepositoryMock.Setup(x => x.DeleteAndInsertServerInfo(It.IsAny<MediaServerInfo>()));

            var serverInfo = new MediaServerInfo {Id = Guid.NewGuid().ToString()};
            _httpClientMock.Setup(x => x.GetServerInfo()).ReturnsAsync(serverInfo);

            var strategy = new Mock<IClientStrategy>();
            strategy.Setup(x => x.CreateHttpClient(It.IsAny<ServerType>())).Returns(_httpClientMock.Object);

            var mapperMock = new Mock<IMapper>();
            var service = new MediaServerService(strategy.Object, _embyRepositoryMock.Object,
                _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object,
                mapperMock.Object);
            var result = await service.GetServerInfo(false);

            result.Should().NotBeNull();
            result.Id.Should().Be(serverInfo.Id);

            _embyRepositoryMock.Verify(x => x.GetServerInfo(), Times.Once);
            _embyRepositoryMock.Verify(x => x.DeleteAndInsertServerInfo(It.IsAny<MediaServerInfo>()), Times.Once);
            _httpClientMock.Verify(x => x.GetServerInfo(), Times.Once);
        }

        [Fact]
        public async Task GetEmbyStatus_Should_Return_Emby_Status_Object()
        {
            _httpClientMock = new Mock<IBaseHttpClient>();
            _embyRepositoryMock = new Mock<IMediaServerRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _showRepositoryMock = new Mock<IShowRepository>();
            _embyRepositoryMock = new Mock<IMediaServerRepository>();
            _sessionServiceMock = new Mock<ISessionService>();

            var embyStatus = new MediaServerStatus() {Id = Guid.NewGuid()};
            _embyRepositoryMock.Setup(x => x.GetEmbyStatus()).ReturnsAsync(embyStatus);

            var strategy = new Mock<IClientStrategy>();
            strategy.Setup(x => x.CreateHttpClient(It.IsAny<ServerType>())).Returns(_httpClientMock.Object);
            var mapperMock = new Mock<IMapper>();

            var service = new MediaServerService(strategy.Object, _embyRepositoryMock.Object,
                _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object,
                mapperMock.Object);
            var result = await service.GetMediaServerStatus();

            result.Should().NotBeNull();
            result.Id.Should().Be(embyStatus.Id);

            _embyRepositoryMock.Verify(x => x.GetEmbyStatus(), Times.Once);
        }

        [Fact]
        public async Task ResetMissedPings_Should_Return_Reset_Missed_Pings()
        {
            _httpClientMock = new Mock<IBaseHttpClient>();
            _embyRepositoryMock = new Mock<IMediaServerRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _showRepositoryMock = new Mock<IShowRepository>();
            _embyRepositoryMock = new Mock<IMediaServerRepository>();
            _sessionServiceMock = new Mock<ISessionService>();

            _embyRepositoryMock.Setup(x => x.ResetMissedPings());

            var strategy = new Mock<IClientStrategy>();
            strategy.Setup(x => x.CreateHttpClient(It.IsAny<ServerType>())).Returns(_httpClientMock.Object);
            var mapperMock = new Mock<IMapper>();

            var service = new MediaServerService(strategy.Object, _embyRepositoryMock.Object,
                _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object,
                mapperMock.Object);
            await service.ResetMissedPings();

            _embyRepositoryMock.Verify(x => x.ResetMissedPings(), Times.Once);
        }

        [Fact]
        public async Task IncreaseMissedPings_Should_Return_Increased_Missed_Pings()
        {
            _httpClientMock = new Mock<IBaseHttpClient>();
            _embyRepositoryMock = new Mock<IMediaServerRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _showRepositoryMock = new Mock<IShowRepository>();
            _embyRepositoryMock = new Mock<IMediaServerRepository>();
            _sessionServiceMock = new Mock<ISessionService>();

            _embyRepositoryMock.Setup(x => x.ResetMissedPings());

            var strategy = new Mock<IClientStrategy>();
            strategy.Setup(x => x.CreateHttpClient(It.IsAny<ServerType>())).Returns(_httpClientMock.Object);
            var mapperMock = new Mock<IMapper>();

            var service = new MediaServerService(strategy.Object, _embyRepositoryMock.Object,
                _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object,
                mapperMock.Object);
            await service.IncreaseMissedPings();

            _embyRepositoryMock.Verify(x => x.IncreaseMissedPings(), Times.Once);
        }

        [Fact]
        public async Task PingEmby_Should_Ping_Emby_Server()
        {
            _httpClientMock = new Mock<IBaseHttpClient>();
            _embyRepositoryMock = new Mock<IMediaServerRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _showRepositoryMock = new Mock<IShowRepository>();
            _embyRepositoryMock = new Mock<IMediaServerRepository>();
            _sessionServiceMock = new Mock<ISessionService>();

            _httpClientMock.Setup(x => x.Ping()).ReturnsAsync(true);

            var strategy = new Mock<IClientStrategy>();
            strategy.Setup(x => x.CreateHttpClient(It.IsAny<ServerType>())).Returns(_httpClientMock.Object);
            var mapperMock = new Mock<IMapper>();

            var service = new MediaServerService(strategy.Object, _embyRepositoryMock.Object,
                _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object,
                mapperMock.Object);
            var result = await service.PingMediaServer("localhost:9000");
            result.Should().BeTrue();

            _httpClientMock.Verify(x => x.Ping(), Times.Once);
        }

        [Fact]
        public async Task TestNewApiKey_Should_Return_True()
        {
            _httpClientMock = new Mock<IBaseHttpClient>();
            _embyRepositoryMock = new Mock<IMediaServerRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _showRepositoryMock = new Mock<IShowRepository>();
            _embyRepositoryMock = new Mock<IMediaServerRepository>();
            _sessionServiceMock = new Mock<ISessionService>();

            _httpClientMock.Setup(x => x.GetServerInfo()).ReturnsAsync(new MediaServerInfo());
            _httpClientMock.SetupSet(x => x.ApiKey = "1234").Verifiable();
            _httpClientMock.SetupSet(x => x.BaseUrl = "localhost:9000").Verifiable();

            _httpClientMock.SetupGet(x => x.ApiKey).Returns("1234");
            _httpClientMock.SetupGet(x => x.BaseUrl).Returns("localhost:9000");

            var strategy = new Mock<IClientStrategy>();
            strategy.Setup(x => x.CreateHttpClient(It.IsAny<ServerType>())).Returns(_httpClientMock.Object);
            var mapperMock = new Mock<IMapper>();

            var service = new MediaServerService(strategy.Object, _embyRepositoryMock.Object,
                _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object,
                mapperMock.Object);
            var result = await service.TestNewApiKey("localhost:9000", "1234", ServerType.Emby);

            result.Should().BeTrue();

            _httpClientMock.VerifyGet(x => x.ApiKey, Times.Once);
            _httpClientMock.VerifyGet(x => x.BaseUrl, Times.Once);

            _httpClientMock.VerifySet(x => x.ApiKey = "1234");
            _httpClientMock.VerifySet(x => x.BaseUrl = "localhost:9000");

            _httpClientMock.Verify(x => x.GetServerInfo(), Times.Once);
        }

        [Fact]
        public async Task TestNewApiKey_Should_Return_False()
        {
            _httpClientMock = new Mock<IBaseHttpClient>();
            _embyRepositoryMock = new Mock<IMediaServerRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _showRepositoryMock = new Mock<IShowRepository>();
            _embyRepositoryMock = new Mock<IMediaServerRepository>();
            _sessionServiceMock = new Mock<ISessionService>();

            _httpClientMock.Setup(x => x.GetServerInfo()).ReturnsAsync((MediaServerInfo) null);
            _httpClientMock.SetupSet(x => x.ApiKey = "1234").Verifiable();
            _httpClientMock.SetupSet(x => x.BaseUrl = "localhost:9000").Verifiable();

            _httpClientMock.SetupGet(x => x.ApiKey).Returns("12345");
            _httpClientMock.SetupGet(x => x.BaseUrl).Returns("localhost:9001");

            var strategy = new Mock<IClientStrategy>();
            strategy
                .Setup(x => x.CreateHttpClient(It.IsAny<ServerType>()))
                .Returns(_httpClientMock.Object);
            var mapperMock = new Mock<IMapper>();

            var service = new MediaServerService(strategy.Object, _embyRepositoryMock.Object,
                _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object,
                mapperMock.Object);
            var result = await service.TestNewApiKey("localhost:9000", "1234", ServerType.Emby);

            result.Should().BeFalse();

            _httpClientMock.VerifyGet(x => x.ApiKey, Times.Once);
            _httpClientMock.VerifyGet(x => x.BaseUrl, Times.Once);

            _httpClientMock.VerifySet(x => x.ApiKey = "1234", Times.Once);
            _httpClientMock.VerifySet(x => x.BaseUrl = "localhost:9000", Times.Once);

            _httpClientMock.VerifySet(x => x.ApiKey = "12345", Times.Once);
            _httpClientMock.VerifySet(x => x.BaseUrl = "localhost:9001", Times.Once);

            _httpClientMock.Verify(x => x.GetServerInfo(), Times.Once);
        }

        [Fact]
        public async Task GetAllPlugins_Should_Return_List_Of_Plugins()
        {
            _httpClientMock = new Mock<IBaseHttpClient>();
            _embyRepositoryMock = new Mock<IMediaServerRepository>();
            _movieRepositoryMock = new Mock<IMovieRepository>();
            _showRepositoryMock = new Mock<IShowRepository>();
            _embyRepositoryMock = new Mock<IMediaServerRepository>();
            _sessionServiceMock = new Mock<ISessionService>();

            var plugins = new List<PluginInfo> {new() {Id = "123"}, new() {Id = "234"}};
            _embyRepositoryMock.Setup(x => x.GetAllPlugins()).ReturnsAsync(plugins);

            var strategy = new Mock<IClientStrategy>();
            strategy.Setup(x => x.CreateHttpClient(It.IsAny<ServerType>())).Returns(_httpClientMock.Object);
            var mapperMock = new Mock<IMapper>();

            var service = new MediaServerService(strategy.Object, _embyRepositoryMock.Object,
                _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object,
                mapperMock.Object);
            var result = await service.GetAllPlugins();

            result.Should().NotContainNulls();
            result.Count.Should().Be(2);

            _embyRepositoryMock.Verify(x => x.GetAllPlugins(), Times.Once);
        }

        //TODO: fix testing after user implementation
        // [Fact]
        // public async Task GetAllUsers_Should_Return_List_Of_Users()
        // {
        //     _httpClientMock = new Mock<IBaseHttpClient>();
        //     _embyRepositoryMock = new Mock<IMediaServerRepository>();
        //     _movieRepositoryMock = new Mock<IMovieRepository>();
        //     _showRepositoryMock = new Mock<IShowRepository>();
        //     _embyRepositoryMock = new Mock<IMediaServerRepository>();
        //     _sessionServiceMock = new Mock<ISessionService>();
        //
        //     var users = new List<EmbyUser>() {new(), new()};
        //     _embyRepositoryMock.Setup(x => x.GetAllUsers()).ReturnsAsync(users);
        //
        //     var strategy = new Mock<IClientStrategy>();
        //     strategy.Setup(x => x.CreateHttpClient(It.IsAny<ServerType>())).Returns(_httpClientMock.Object);
        //     var mapperMock = new Mock<IMapper>();
        //
        //     var service = new MediaServerService(strategy.Object, _embyRepositoryMock.Object,
        //         _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object,
        //         mapperMock.Object);
        //     var result = await service.GetAllUsers();
        //
        //     result.Should().NotContainNulls();
        //     result.Count.Should().Be(2);
        //
        //     _embyRepositoryMock.Verify(x => x.GetAllUsers(), Times.Once);
        // }
        //
        // [Fact]
        // public async Task GetUserById_Should_Return_Correct_User()
        // {
        //     _httpClientMock = new Mock<IBaseHttpClient>();
        //     _embyRepositoryMock = new Mock<IMediaServerRepository>();
        //     _movieRepositoryMock = new Mock<IMovieRepository>();
        //     _showRepositoryMock = new Mock<IShowRepository>();
        //     _embyRepositoryMock = new Mock<IMediaServerRepository>();
        //     _sessionServiceMock = new Mock<ISessionService>();
        //
        //     var users = new List<EmbyUser> {new() {Id = "1"}, new() {Id = "2"}};
        //     _embyRepositoryMock.Setup(x => x.GetUserById("1")).Returns(users[0]);
        //     _embyRepositoryMock.Setup(x => x.GetUserById("2")).Returns(users[1]);
        //
        //     var strategy = new Mock<IClientStrategy>();
        //     strategy.Setup(x => x.CreateHttpClient(It.IsAny<ServerType>())).Returns(_httpClientMock.Object);
        //     var mapperMock = new Mock<IMapper>();
        //
        //     var service = new MediaServerService(strategy.Object, _embyRepositoryMock.Object,
        //         _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object,
        //         mapperMock.Object);
        //     var result = service.GetUserById("1");
        //
        //     result.Should().NotBeNull();
        //     result.Id.Should().Be("1");
        //
        //     _embyRepositoryMock.Verify(x => x.GetUserById("1"), Times.Once);
        // }
        //
        // [Fact]
        // public async Task GetViewedEpisodeCountByUserId_Should_Return_Total_Episode_View_Count()
        // {
        //     _httpClientMock = new Mock<IBaseHttpClient>();
        //     _embyRepositoryMock = new Mock<IMediaServerRepository>();
        //     _movieRepositoryMock = new Mock<IMovieRepository>();
        //     _showRepositoryMock = new Mock<IShowRepository>();
        //     _embyRepositoryMock = new Mock<IMediaServerRepository>();
        //     _sessionServiceMock = new Mock<ISessionService>();
        //
        //     _sessionServiceMock.Setup(x => x.GetMediaIdsForUser(It.IsAny<string>(), It.IsAny<PlayType>()))
        //         .Returns(new[] {"12", "13", "14"});
        //
        //     var strategy = new Mock<IClientStrategy>();
        //     strategy.Setup(x => x.CreateHttpClient(It.IsAny<ServerType>())).Returns(_httpClientMock.Object);
        //     var mapperMock = new Mock<IMapper>();
        //
        //     var service = new MediaServerService(strategy.Object, _embyRepositoryMock.Object,
        //         _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object,
        //         mapperMock.Object);
        //     var result = service.GetViewedEpisodeCountByUserId("1");
        //
        //     result.Should().NotBeNull();
        //     result.Title.Should().Be(Constants.Users.TotalWatchedEpisodes);
        //     result.Value.Should().Be(3);
        //
        //     _sessionServiceMock.Verify(x => x.GetMediaIdsForUser("1", PlayType.Episode), Times.Once);
        // }
        //
        // [Fact]
        // public async Task GetViewedMovieCountByUserId_Should_Return_Total_Movie_View_Count()
        // {
        //     _httpClientMock = new Mock<IBaseHttpClient>();
        //     _embyRepositoryMock = new Mock<IMediaServerRepository>();
        //     _movieRepositoryMock = new Mock<IMovieRepository>();
        //     _showRepositoryMock = new Mock<IShowRepository>();
        //     _embyRepositoryMock = new Mock<IMediaServerRepository>();
        //     _sessionServiceMock = new Mock<ISessionService>();
        //
        //     _sessionServiceMock.Setup(x => x.GetMediaIdsForUser(It.IsAny<string>(), It.IsAny<PlayType>()))
        //         .Returns(new[] {"12", "13", "14"});
        //
        //     var strategy = new Mock<IClientStrategy>();
        //     strategy.Setup(x => x.CreateHttpClient(It.IsAny<ServerType>())).Returns(_httpClientMock.Object);
        //     var mapperMock = new Mock<IMapper>();
        //
        //     var service = new MediaServerService(strategy.Object, _embyRepositoryMock.Object,
        //         _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object,
        //         mapperMock.Object);
        //     var result = service.GetViewedMovieCountByUserId("1");
        //
        //     result.Should().NotBeNull();
        //     result.Title.Should().Be(Constants.Users.TotalWatchedMovies);
        //     result.Value.Should().Be(3);
        //
        //     _sessionServiceMock.Verify(x => x.GetMediaIdsForUser("1", PlayType.Movie), Times.Once);
        // }
        //
        // [Fact]
        // public async Task GetUserViewCount_Should_Return_User_View_Count()
        // {
        //     _httpClientMock = new Mock<IBaseHttpClient>();
        //     _embyRepositoryMock = new Mock<IMediaServerRepository>();
        //     _movieRepositoryMock = new Mock<IMovieRepository>();
        //     _showRepositoryMock = new Mock<IShowRepository>();
        //     _embyRepositoryMock = new Mock<IMediaServerRepository>();
        //     _sessionServiceMock = new Mock<ISessionService>();
        //
        //     _sessionServiceMock.Setup(x => x.GetPlayCountForUser(It.IsAny<string>())).Returns(12);
        //
        //     var strategy = new Mock<IClientStrategy>();
        //     strategy.Setup(x => x.CreateHttpClient(It.IsAny<ServerType>())).Returns(_httpClientMock.Object);
        //     var mapperMock = new Mock<IMapper>();
        //
        //     var libraryRepository = new Mock<ILibraryRepository>();
        //     var service = new MediaServerService(strategy.Object, _embyRepositoryMock.Object,
        //         _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object,
        //         _showRepositoryMock.Object, libraryRepository.Object);
        //     var result = service.GetUserViewCount("1");
        //
        //     result.Should().Be(12);
        //
        //     _sessionServiceMock.Verify(x => x.GetPlayCountForUser("1"), Times.Once);
        // }
        //
        // [Fact]
        // public async Task GetAndProcessServerInfo_Should_Save_Server_Info()
        // {
        //     _httpClientMock = new Mock<IBaseHttpClient>();
        //     _embyRepositoryMock = new Mock<IMediaServerRepository>();
        //     _movieRepositoryMock = new Mock<IMovieRepository>();
        //     _showRepositoryMock = new Mock<IShowRepository>();
        //     _embyRepositoryMock = new Mock<IMediaServerRepository>();
        //     _sessionServiceMock = new Mock<ISessionService>();
        //
        //     var serverInfo = new ServerInfo {Id = "1234"};
        //     _httpClientMock.Setup(x => x.GetServerInfo()).Returns(serverInfo);
        //     _embyRepositoryMock.Setup(x => x.DeleteAndInsertServerInfo(It.IsAny<ServerInfo>()));
        //
        //     var strategy = new Mock<IClientStrategy>();
        //     strategy.Setup(x => x.CreateHttpClient(It.IsAny<ServerType>())).Returns(_httpClientMock.Object);
        //
        //     var libraryRepository = new Mock<ILibraryRepository>();
        //     var mapperMock = new Mock<IMapper>();
        //
        //     var service = new MediaServerService(strategy.Object, _embyRepositoryMock.Object,
        //         _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object,
        //         _showRepositoryMock.Object, libraryRepository.Object);
        //     var result = service.GetAndProcessServerInfo();
        //
        //     result.Should().NotBeNull();
        //     result.Id.Should().Be(serverInfo.Id);
        //
        //     _httpClientMock.Verify(x => x.GetServerInfo(), Times.Once);
        //     _embyRepositoryMock.Verify(x => x.DeleteAndInsertServerInfo(serverInfo), Times.Once);
        // }
        //
        // [Fact]
        // public async Task GetAndProcessPluginInfo_Should_Save_Plugins()
        // {
        //     _httpClientMock = new Mock<IBaseHttpClient>();
        //     _embyRepositoryMock = new Mock<IMediaServerRepository>();
        //     _movieRepositoryMock = new Mock<IMovieRepository>();
        //     _showRepositoryMock = new Mock<IShowRepository>();
        //     _embyRepositoryMock = new Mock<IMediaServerRepository>();
        //     _sessionServiceMock = new Mock<ISessionService>();
        //
        //     var plugins = new List<PluginInfo> {new(), new()};
        //     _httpClientMock.Setup(x => x.GetInstalledPlugins()).Returns(plugins);
        //
        //     _embyRepositoryMock.Setup(x => x.RemoveAllAndInsertPluginRange(plugins));
        //
        //     var strategy = new Mock<IClientStrategy>();
        //     strategy.Setup(x => x.CreateHttpClient(It.IsAny<ServerType>())).Returns(_httpClientMock.Object);
        //
        //     var mapperMock = new Mock<IMapper>();
        //
        //     var service = new MediaServerService(strategy.Object, _embyRepositoryMock.Object,
        //         _sessionServiceMock.Object, _settingsServiceMock.Object, _movieRepositoryMock.Object,
        //         mapperMock.Object);
        //     service.GetAndProcessPluginInfo();
        //
        //     _httpClientMock.Verify(x => x.GetInstalledPlugins(), Times.Once);
        //     _embyRepositoryMock.Verify(x => x.RemoveAllAndInsertPluginRange(plugins), Times.Once);
        // }
    }
}