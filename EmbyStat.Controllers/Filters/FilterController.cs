using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Hubs.Job;
using EmbyStat.Common.Models.Tasks.Enum;
using EmbyStat.Controllers.Movie;
using EmbyStat.Jobs;
using EmbyStat.Services.Interfaces;
using Hangfire;
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