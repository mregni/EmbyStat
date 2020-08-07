using AutoMapper;
using EmbyStat.Common.Enums;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmbyStat.Controllers.Filters
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class FilterController : Controller
    {
        private readonly IFilterService _filterService;
        private readonly IMapper _mapper;

        public FilterController(IFilterService filterService, IMapper mapper)
        {
            _filterService = filterService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("{type}/{field}")]
        public IActionResult Get(LibraryType type, string field, string[] libraryIds)
        {
            var filters = _filterService.GetFilterValues(type, field, libraryIds);
            var convert = _mapper.Map<FilterValuesViewModel>(filters);
            return Ok(convert);
        }
    }
}