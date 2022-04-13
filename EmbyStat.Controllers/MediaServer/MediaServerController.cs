using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Helpers;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MoreLinq;

namespace EmbyStat.Controllers.MediaServer;

[Produces("application/json")]
[Route("api/[controller]")]
public class MediaServerController : Controller
{
    private readonly IMediaServerService _mediaServerService;
    private readonly IMapper _mapper;

    public MediaServerController(IMediaServerService mediaServerService, IMapper mapper)
    {
        _mediaServerService = mediaServerService;
        _mapper = mapper;
    }

    #region Server

    [HttpGet]
    [Route("server/info")]
    public async Task<IActionResult> GetServerInfo(bool forceReSync = false)
    {
        var result = await _mediaServerService.GetServerInfo(forceReSync);
        var serverInfo = _mapper.Map<ServerInfoViewModel>(result);

        var users = await _mediaServerService.GetAllUsers();
        serverInfo.ActiveUserCount = users.Count(x => x.LastActivityDate > DateTime.Now.AddMonths(-6));
        serverInfo.IdleUserCount = users.Count(x => x.LastActivityDate <= DateTime.Now.AddMonths(-6));
        var devices = await _mediaServerService.GetAllDevices();
        serverInfo.ActiveDeviceCount = devices.Count(x => x.DateLastActivity > DateTime.Now.AddMonths(-6));
        serverInfo.IdleDeviceCount = devices.Count(x => x.DateLastActivity <= DateTime.Now.AddMonths(-6));
            
        return Ok(serverInfo);
    }

    [HttpPost]
    [Route("server/test")]
    public async Task<IActionResult> TestApiKey([FromBody] LoginViewModel login)
    {
        var result = await _mediaServerService.TestNewApiKey(login.Address, login.ApiKey, login.Type);
        return Ok(result);
    }

    [HttpGet]
    [Route("server/search")]
    public async Task<IActionResult> SearchMediaServer(int serverType)
    {
        var type = (ServerType)serverType;
        var result = await _mediaServerService.SearchMediaServer(type);
        if (result != null)
        {
            var mapped = _mapper
                .Map<IList<UdpBroadcastViewModel>>(result, opt =>
                    opt.AfterMap((src, dest) => dest.ForEach<UdpBroadcastViewModel>(y => y.Type = (int)type)));
            return Ok(mapped);
        }

        return NoContent();
    }

    [HttpGet]
    [Route("server/status")]
    public async Task<IActionResult> GetMediaServerStatus()
    {
        var result = await _mediaServerService.GetMediaServerStatus();
        return Ok(_mapper.Map<EmbyStatusViewModel>(result));
    }

    [HttpGet]
    [Route("server/libraries")]
    public async Task<IActionResult> GetMediaServerLibraries()
    {
        var result = await _mediaServerService.GetMediaServerLibraries();
        return Ok(_mapper.Map<IList<LibraryViewModel>>(result));
    }

    [HttpPost]
    [Route("server/ping")]
    public IActionResult PingEmby([FromBody] UrlViewModel url)
    {
        var result = _mediaServerService.PingMediaServer(url.Url);
        return Ok(result);
    }

    #endregion

    #region Plugins

    [HttpGet]
    [Route("plugins")]
    public async Task<IActionResult> GetPlugins()
    {
        var result = await _mediaServerService.GetAllPlugins();
        return Ok(_mapper.Map<IList<PluginViewModel>>(result));
    }

    #endregion

    #region Users

    [HttpGet]
    [Route("users")]
    public async Task<IActionResult> GetUsers()
    {
        var result = await _mediaServerService.GetAllUsers();
        return Ok(_mapper.Map<IList<UserOverviewViewModel>>(result));
    }

    [HttpGet]
    [Route("administrators")]
    public async Task<IActionResult> GetAdministrators()
    {
        var result = await _mediaServerService.GetAllAdministrators();
        return Ok(_mapper.Map<IList<UserOverviewViewModel>>(result));
    }

    [HttpGet]
    [Route("users/{id}")]
    public IActionResult GetUserById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        var user = _mediaServerService.GetUserById(id);
        if (user == null)
        {
            return NotFound();
        }

        var mappedUser = _mapper.Map<UserFullViewModel>(user);

        var viewedEpisodeCount = _mediaServerService.GetViewedEpisodeCountByUserId(id);
        var viewedMovieCount = _mediaServerService.GetViewedMovieCountByUserId(id);
        var lastWatchedMedia = _mediaServerService.GetUserViewPageByUserId(id, 0,10).ToList();

        mappedUser.ViewedEpisodeCount = _mapper.Map<CardViewModel<int>>(viewedEpisodeCount);
        mappedUser.ViewedMovieCount = _mapper.Map<CardViewModel<int>>(viewedMovieCount);
        mappedUser.LastWatchedMedia = _mapper.Map<IList<UserMediaViewViewModel>>(lastWatchedMedia);

        return Ok(mappedUser);
    }

    [HttpGet]
    [Route("users/{id}/views/{page:int}/{size:int}")]
    public IActionResult GetUserViews(string id, int page, int size)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return NotFound();
        }

        var views = _mediaServerService.GetUserViewPageByUserId(id, page, size);
        var list = _mapper.Map<IList<UserMediaViewViewModel>>(views);

        var count = _mediaServerService.GetUserViewCount(id);
        var container = new ListContainer<UserMediaViewViewModel>
        {
            Data = list.ToList(),
            TotalCount = count
        };
        return Ok(container);
    }

    [HttpGet]
    [Route("users/ids")]
    public IActionResult GetUserIdList()
    {
        var users = _mediaServerService.GetAllUsers();
        return Ok(_mapper.Map<IList<UserIdViewModel>>(users));
    }
    #endregion
}