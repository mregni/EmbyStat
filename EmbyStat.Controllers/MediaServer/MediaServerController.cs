using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Helpers;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmbyStat.Controllers.MediaServer
{
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
        public IActionResult GetServerInfo(bool forceReSync = false)
        {
            var result = _mediaServerService.GetServerInfo(forceReSync);

            var serverInfo = _mapper.Map<ServerInfoViewModel>(result);
            return Ok(serverInfo);
        }

        [HttpPost]
        [Route("server/test")]
        public IActionResult TestApiKey([FromBody] LoginViewModel login)
        {
            var result = _mediaServerService.TestNewApiKey(login.Address, login.ApiKey, login.Type);
            return Ok(result);
        }

        [HttpGet]
        [Route("server/search")]
        public IActionResult SearchMediaServer(int serverType)
        {
            var type = (ServerType)serverType;
            var result = _mediaServerService.SearchMediaServer(type);
            if (result != null)
            {
                return Ok(_mapper.Map<UdpBroadcastViewModel>(result));
            }

            return NoContent();
        }

        [HttpGet]
        [Route("server/status")]
        public IActionResult GetMediaServerStatus()
        {
            var result = _mediaServerService.GetMediaServerStatus();
            return Ok(_mapper.Map<EmbyStatusViewModel>(result));
        }

        [HttpGet]
        [Route("server/libraries")]
        public IActionResult GetMediaServerLibraries()
        {
            var result = _mediaServerService.GetMediaServerLibraries();
            return Ok(_mapper.Map<IList<LibraryViewModel>>(result));
        }

        [HttpPost]
        [Route("server/ping")]
        public IActionResult PingEmby([FromBody]UrlViewModel url)
        {
            var result = _mediaServerService.PingMediaServer(url.Url);
            return Ok(result);
        }

        #endregion

        #region Plugins

        [HttpGet]
        [Route("plugins")]
        public IActionResult GetPlugins()
        {
            var result = _mediaServerService.GetAllPlugins();
            return Ok(_mapper.Map<IList<PluginViewModel>>(result));
        }

        #endregion

        #region Users

        [HttpGet]
        [Route("users")]
        public IActionResult GetUsers()
        {
            var result = _mediaServerService.GetAllUsers();
            return Ok(_mapper.Map<IList<UserOverviewViewModel>>(result));
        }

        [HttpGet]
        [Route("administrators")]
        public IActionResult GetAdministrators()
        {
            var result = _mediaServerService.GetAllAdministrators();
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
        [Route("users/{id}/views/{page}/{size}")]
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
}