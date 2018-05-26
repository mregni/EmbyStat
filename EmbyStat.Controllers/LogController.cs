using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AutoMapper;
using EmbyStat.Controllers.ViewModels.Logs;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmbyStat.Controllers
{
    [Route("api/[controller]")]
    public class LogController : Controller
    {
        private readonly ILogsService _logService;

        public LogController(ILogsService logService)
        {
            _logService = logService;
        }

        [HttpGet]
        [Produces("application/json")]
        [Route("list")]
        public IActionResult GetLogFileLIst()
        {
            var files = _logService.GetLogFileList();
            return Ok(Mapper.Map<IList<LogFileViewModel>>(files));
        }

        [HttpGet]
        [Route("download/{fileName}")]
        public FileResult GetZipFile(string fileName)
        {
            var stream = _logService.GetLogStream(fileName);
            return File(stream, "application/octet-stream", fileName);
        }
    }
}
