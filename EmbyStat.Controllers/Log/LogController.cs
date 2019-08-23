using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmbyStat.Controllers.Log
{
    [Route("api/[controller]")]
    public class LogController : Controller
    {
        private readonly ILogService _logService;
        private readonly IMapper _mapper;

        public LogController(ILogService logService, IMapper mapper)
        {
            _logService = logService;
            _mapper = mapper;
        }

        [HttpGet]
        [Produces("application/json")]
        [Route("list")]
        public IActionResult GetLogFileList()
        {
            var files = _logService.GetLogFileList();
            return Ok(_mapper.Map<IList<LogFileViewModel>>(files));
        }

        [HttpGet]
        [Route("download/{fileName}")]
        public async Task<FileResult> GetZipFile(string fileName, bool anonymous)
        {
            var stream = await _logService.GetLogStream(fileName, anonymous);
            return File(stream, "application/octet-stream", fileName);
        }
    }
}
