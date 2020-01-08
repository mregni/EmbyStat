using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common.Helpers;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmbyStat.Controllers.Emby
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
        public IActionResult GetServerInfo()
        {
            var result = _mediaServerService.GetServerInfo();

            var serverInfo = _mapper.Map<ServerInfoViewModel>(result);
            return Ok(serverInfo);
        }

        [HttpPost]
        [Route("server/test")]
        public IActionResult TestApiKey([FromBody] LoginViewModel login)
        {
            var result = _mediaServerService.TestNewApiKey(login.Address, login.ApiKey);
            return Ok(result);
        }

        [HttpGet]
        [Route("server/search")]
        public IActionResult SearchEmby()
        {
            var result = _mediaServerService.SearchEmby();
            return Ok(_mapper.Map<UdpBroadcastViewModel>(result));
        }

        [HttpGet]
        [Route("server/status")]
        public IActionResult GetEmbyStatus()
        {
            var result = _mediaServerService.GetEmbyStatus();
            return Ok(_mapper.Map<EmbyStatusViewModel>(result));
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