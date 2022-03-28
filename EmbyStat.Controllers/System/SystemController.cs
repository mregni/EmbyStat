using System.Threading.Tasks;
using Aiursoft.XelNaga.Services;
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
        private readonly CannonService _cannonService;

        public SystemController(IUpdateService updateService, IMapper mapper, CannonService cannonService)
        {
            _updateService = updateService;
            _mapper = mapper;
            _cannonService = cannonService;
        }

        [HttpGet]
        [Route("checkforupdate")]
        public ActionResult CheckForUpdate()
        {
            var result = _updateService.CheckForUpdate();
            return Ok(_mapper.Map<UpdateResultViewModel>(result));
        }

        [HttpPost]
        [Route("startupdate")]
        public async Task<IActionResult> StartUpdate()
        {
            var result = await _updateService.CheckForUpdate();
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

        [HttpGet]
        [Route("reset")]
        public IActionResult ResetStatistics()
        {
            _cannonService.FireAsync<ISystemService>(s => s.ResetEmbyStatTables());
            return Ok();
        }
    }
}
