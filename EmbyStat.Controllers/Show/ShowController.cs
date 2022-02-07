using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common.Helpers;
using EmbyStat.Common.Models.Query;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EmbyStat.Controllers.Show
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ShowController : Controller
    {
        private readonly IShowService _showService;
        private readonly IMapper _mapper;

        public ShowController(IShowService showService, IMapper mapper)
        {
            _showService = showService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("libraries")]
        public IActionResult GetLibraries()
        {
            var result = _showService.GetShowLibraries();
            var convert = _mapper.Map<IList<LibraryViewModel>>(result);
            return Ok(convert);
        }

        [HttpGet]
        [Route("statistics")]
        public async Task<IActionResult> GetStatistics(List<string> libraryIds)
        {
            var result = await _showService.GetStatistics(libraryIds);
            var convert = _mapper.Map<ShowStatisticsViewModel>(result);
            return Ok(convert);
        }

        [HttpGet]
        [Route("collectedlist")]
        public async Task<IActionResult> GetCollectedRows(List<string> libraryIds, int page)
        {
            var result = await _showService.GetCollectedRows(libraryIds, page);
            return Ok(_mapper.Map<ListContainer<ShowCollectionRowViewModel>>(result));
        }

        [HttpGet]
        [Route("list")]
        public async Task<IActionResult> GetShowPageList(int skip, int take, string sort, bool requireTotalCount, string filter, List<string> libraryIds)
        {
            var filtersObj = Array.Empty<Filter>();
            if (filter != null)
            {
                filtersObj = JsonConvert.DeserializeObject<Filter[]>(filter);
            }

            var page = await _showService.GetShowPage(skip, take, sort, filtersObj, requireTotalCount, libraryIds);
            var convert = _mapper.Map<PageViewModel<ShowRowViewModel>>(page);
            return Ok(convert);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetShow(string id)
        {
            var show = await _showService.GetShow(id);
            if (show != null)
            {
                var result = _mapper.Map<ShowDetailViewModel>(show);
                return Ok(result);
            }

            return NotFound(id);
        }

        [HttpGet]
        [Route("typepresent")]
        public IActionResult ShowTypeIsPresent()
        {
            var result = _showService.TypeIsPresent();
            return Ok(result);
        }
    }
}
