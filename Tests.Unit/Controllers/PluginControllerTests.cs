using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Controllers.MediaServer;
using EmbyStat.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Tests.Unit.Controllers;

public class PluginControllerTests : IDisposable
{
    private readonly MediaServerController _subject;
    private readonly Mock<IMediaServerService> _embyServiceMock;
    private readonly List<PluginInfo> _plugins;

    public PluginControllerTests()
    {
        _plugins = new List<PluginInfo>
        {
            new() {Name = "Trakt plugin"},
            new() {Name = "EmbyStat plugin"}
        };

        _embyServiceMock = new Mock<IMediaServerService>();
        _embyServiceMock.Setup(x => x.GetAllPlugins()).ReturnsAsync(_plugins);

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(x => x.Map<IList<PluginViewModel>>(It.IsAny<List<PluginInfo>>())).Returns(
            new List<PluginViewModel> {new() {Name = "Trakt plugin"}, new() {Name = "EmbyStat plugin"}});
        _subject = new MediaServerController(_embyServiceMock.Object, mapperMock.Object);
    }

    public void Dispose()
    {
        _subject?.Dispose();
    }

    [Fact]
    public async Task ArePluginsReturned()
    {
        var result = await _subject.GetPlugins();
        var resultObject = result.Should().BeOfType<OkObjectResult>().Subject.Value;
        var list = resultObject.Should().BeOfType<List<PluginViewModel>>().Subject;

        list.Count.Should().Be(2);
        list[0].Name.Should().Be(_plugins[0].Name);
        list[1].Name.Should().Be(_plugins[1].Name);
        _embyServiceMock.Verify(x => x.GetAllPlugins(), Times.Once);
        _embyServiceMock.VerifyNoOtherCalls();
    }
}