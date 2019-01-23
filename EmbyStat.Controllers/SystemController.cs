using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Controllers.ViewModels.Update;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace EmbyStat.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class SystemController : Controller
    {
        private readonly IUpdateService _updateService;
        private readonly IMapper _mapper;

        public SystemController(IUpdateService updateService, IMapper mapper)
        {
            _updateService = updateService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("checkforupdate")]
        public async Task<ActionResult> CheckForUpdate()
        {
            var result = await _updateService.CheckForUpdate(new CancellationToken());
            return Ok(_mapper.Map<UpdateResultViewModel>(result));
        }

        [HttpPost]
        [Route("startupdate")]
        public async Task<IActionResult> StartUpdate()
        {
            var result = await _updateService.CheckForUpdate(new CancellationToken());
            if (result.IsUpdateAvailable)
            {
                await _updateService.DownloadZip(result);
                await _updateService.UpdateServer();
                return Ok(true);
            }

            return Ok(false);
        }

        [HttpGet]
        [Route("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }
    }
}
