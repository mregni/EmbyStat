using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmbyStat.Controllers.System
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
            var result = await _updateService.CheckForUpdateAsync(new CancellationToken());
            return Ok(_mapper.Map<UpdateResultViewModel>(result));
        }

        [HttpPost]
        [Route("startupdate")]
        public async Task<IActionResult> StartUpdate()
        {
            var result = await _updateService.CheckForUpdateAsync(new CancellationToken());
            if (result.IsUpdateAvailable)
            {
                await _updateService.DownloadZipAsync(result);
                await _updateService.UpdateServerAsync();
                return Ok(true);
            }

            return Ok(false);
        }

        [HttpGet]
        [Route("ping")]
        public IActionResult Ping()
        {
            return Ok();
        }
    }
}
