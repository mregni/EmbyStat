﻿using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Configuration.Interfaces;
using EmbyStat.Core.Filters.Interfaces;
using EmbyStat.Core.Genres.Interfaces;
using EmbyStat.Core.Hubs;
using EmbyStat.Core.MediaServers.Interfaces;
using EmbyStat.Core.Movies.Interfaces;
using EmbyStat.Core.People.Interfaces;
using EmbyStat.Core.Shows.Interfaces;
using EmbyStat.Core.Statistics.Interfaces;
using EmbyStat.Core.System;
using Moq;
using Tests.Unit.Builders;
using Xunit;

namespace Tests.Unit.Services;

public class SystemServiceTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ResetEmbyStatTables_Should_Reset_All_Tables(bool result)
    {
        var movieRepository = new Mock<IMovieRepository>();
        var showRepository = new Mock<IShowRepository>();
        var statisticsRepository = new Mock<IStatisticsRepository>();
        var mediaServerRepository = new Mock<IMediaServerRepository>();
        var genreRepository = new Mock<IGenreRepository>();
        var personRepository = new Mock<IPersonRepository>();
        var hub = new Mock<IHubHelper>();
        var settingsService = new Mock<IConfigurationService>();
        
        var config = new ConfigBuilder()
            .WithMediaServerAddress("localhost:8000")
            .WithMediaServerApiKey("AZER")
            .WithMediaServerType(ServerType.Emby)
            .Build();
        settingsService
            .Setup(x => x.Get())
            .Returns(config);
        var mediaServerService = new Mock<IMediaServerService>();
        mediaServerService
            .Setup(x => x.TestNewApiKey(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ServerType>()))
            .ReturnsAsync(result);
        var filterRepository = new Mock<IFilterRepository>();

        var service = new SystemService(movieRepository.Object, showRepository.Object, statisticsRepository.Object,
            mediaServerRepository.Object, genreRepository.Object, personRepository.Object, hub.Object,
            settingsService.Object, mediaServerService.Object, filterRepository.Object);

        await service.ResetEmbyStatTables();

        movieRepository.Verify(x => x.DeleteAll());
        movieRepository.VerifyNoOtherCalls();

        showRepository.Verify(x => x.DeleteAll());
        showRepository.VerifyNoOtherCalls();

        statisticsRepository.Verify(x => x.DeleteStatistics());
        statisticsRepository.VerifyNoOtherCalls();

        mediaServerRepository.Verify(x => x.DeleteAllPlugins());
        mediaServerRepository.Verify(x => x.DeleteAllDevices());
        mediaServerRepository.Verify(x => x.DeleteServerInfo());
        mediaServerRepository.Verify(x => x.DeleteAllLibraries());
        mediaServerRepository.Verify(x => x.DeleteAllUsers());
        mediaServerRepository.VerifyNoOtherCalls();

        genreRepository.Verify(x => x.DeleteAll());
        genreRepository.VerifyNoOtherCalls();

        personRepository.Verify(x => x.DeleteAll());
        personRepository.VerifyNoOtherCalls();

        settingsService.Verify(x => x.Get());
        settingsService.VerifyNoOtherCalls();

        hub.Verify(x => x.BroadcastResetLogLine("Deleting statistics"));
        hub.Verify(x => x.BroadcastResetLogLine("Deleting server info"));
        hub.Verify(x => x.BroadcastResetLogLine("Deleting show data"));
        hub.Verify(x => x.BroadcastResetLogLine("Deleting movie data"));
        hub.Verify(x => x.BroadcastResetLogLine("Deleting genres"));
        hub.Verify(x => x.BroadcastResetLogLine("Deleting people"));
        hub.Verify(x => x.BroadcastResetLogLine("Deleting filters"));
        hub.Verify(x => x.BroadcastResetLogLine("Testing new media server info"));
        hub.Verify(x => x.BroadcastResetFinished());

        if (result)
        {
            hub.Verify(x => x.BroadcastResetLogLine("Setting new libraries"));
            mediaServerService.Verify(x => x.GetAndProcessLibraries());
        }

        hub.VerifyNoOtherCalls();
        
        mediaServerService.Verify(x =>
            x.TestNewApiKey(config.UserConfig.MediaServer.Address, config.UserConfig.MediaServer.ApiKey, config.UserConfig.MediaServer.Type));
        mediaServerService.VerifyNoOtherCalls();
        
        filterRepository.Verify(x => x.DeleteAll());
        filterRepository.VerifyNoOtherCalls();
        
        
    }
}