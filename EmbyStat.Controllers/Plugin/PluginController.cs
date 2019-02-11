using System.Collections.Generic;
using AutoMapper;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmbyStat.Controllers.Plugin
{
	[Produces("application/json")]
	[Route("api/[controller]")]
	public class PluginController : Controller
	{
		private readonly IEmbyService _embyService;
	    private readonly IMapper _mapper;

	    public PluginController(IEmbyService embyService, IMapper mapper)
	    {
            _embyService = embyService;
	        _mapper = mapper;
	    }

		[HttpGet]
		public IActionResult Get()
		{
			var result = _embyService.GetAllPlugins();
			return Ok(_mapper.Map<IList<EmbyPluginViewModel>>(result));
		}
    }
}
