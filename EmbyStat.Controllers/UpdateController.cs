using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmbyStat.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UpdateController : Controller
    {
        private readonly IUpdateService _updateService;

        public UpdateController(IUpdateService updateService)
        {
            _updateService = updateService;
        }

        [HttpGet]
        [Route("checkforupdate")]
        public async Task<ActionResult> CheckForUpdate()
        {
            await _updateService.CheckForUpdate(new CancellationToken());
            _updateService.UpdateServer();

            return Ok();
        }

        [HttpPost]
        [Route("startupdate")]
        public async Task<IActionResult> StartUpdate()
        {
            _updateService.UpdateServer();
            return Ok();
        }
    }
}
