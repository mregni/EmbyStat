using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Emby;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace EmbyStat.Controllers.Emby
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class EmbyController : Controller
    {
        private readonly IEmbyService _embyService;
        private readonly IMapper _mapper;

        public EmbyController(IEmbyService embyService, IMapper mapper)
        {
            _embyService = embyService;
            _mapper = mapper;
        }

        #region Server

        [HttpPost]
        [Route("server/token")]
        public async Task<IActionResult> GenerateToken([FromBody] EmbyLoginViewModel login)
        {
            var result = await _embyService.GetEmbyToken(_mapper.Map<EmbyLogin>(login));
            return Ok(_mapper.Map<EmbyTokenViewModel>(result));
        }

        [HttpGet]
        [Route("server/info")]
        public async Task<IActionResult> GetServerInfo()
        {
            var result = await _embyService.GetServerInfoAsync();

            var serverInfo = _mapper.Map<ServerInfoViewModel>(result);
            return Ok(serverInfo);
        }

        [HttpGet]
        [Route("server/search")]
        public IActionResult SearchEmby()
        {
            var result = _embyService.SearchEmby();
            return Ok(_mapper.Map<EmbyUdpBroadcastViewModel>(result));
        }

        [HttpGet]
        [Route("server/status")]
        public IActionResult GetEmbyStatus()
        {
            var result = _embyService.GetEmbyStatus();
            return Ok(_mapper.Map<EmbyStatusViewModel>(result));
        }

        #endregion

        #region Plugins

        [HttpGet]
        [Route("plugins")]
        public IActionResult GetPlugins()
        {
            var result = _embyService.GetAllPlugins();
            return Ok(_mapper.Map<IList<EmbyPluginViewModel>>(result));
        }

        #endregion

        #region Users

        [HttpGet]
        [Route("users")]
        public IActionResult GetUsers()
        {
            var result = _embyService.GetAllUsers();
            return Ok(_mapper.Map<IList<EmbyUserOverviewViewModel>>(result));
        }

        [HttpGet]
        [Route("users/{id}")]
        public IActionResult GetUserById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var user = _embyService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            var mappedUser = _mapper.Map<EmbyUserFullViewModel>(user);

            var viewedEpisodeCount = _embyService.GetViewedEpisodeCountByUserId(id);
            var viewedMovieCount = _embyService.GetViewedMovieCountByUserId(id);
            var lastWatchedMedia = _embyService.GetUserViewPageByUserId(id, 0,10).ToList();

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

            var views = _embyService.GetUserViewPageByUserId(id, page, size);
            var list = _mapper.Map<IList<UserMediaViewViewModel>>(views);

            var count = _embyService.GetUserViewCount(id);
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
            var users = _embyService.GetAllUsers();
            return Ok(_mapper.Map<IList<UserIdViewModel>>(users));
        }
        #endregion
    }
}