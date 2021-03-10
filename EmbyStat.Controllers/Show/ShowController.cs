using System.Collections.Generic;
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
            return Ok(_mapper.Map<IList<LibraryViewModel>>(result));
        }

        [HttpGet]
        [Route("statistics")]
        public IActionResult GetStatistics(List<string> libraryIds)
        {
            var result = _showService.GetStatistics(libraryIds);
            var convert = _mapper.Map<ShowStatisticsViewModel>(result);
            return Ok(convert);
        }

        [HttpGet]
        [Route("list")]
        public IActionResult GetShowPageList(int skip, int take, string sort, bool requireTotalCount, string filter, List<string> libraryIds)
        {
            var filtersObj = new Filter[0];
            if (filter != null)
            {
                filtersObj = JsonConvert.DeserializeObject<Filter[]>(filter);
            }

            var page = _showService.GetShowPage(skip, take, sort, filtersObj, requireTotalCount, libraryIds);

            var convert = _mapper.Map<PageViewModel<ShowColumnViewModel>>(page);
            return Ok(convert);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetMovie(string id)
        {
            var movie = _showService.GetShow(id);
            if (movie != null)
            {
                return Ok(movie);
            }
            return NotFound(id);
        }

        [HttpGet]
        [Route("collectedlist")]
        public IActionResult GetCollectedRows(List<string> libraryIds, int page)
        {
            var result = _showService.GetCollectedRows(libraryIds, page);
            return Ok(_mapper.Map<ListContainer<ShowCollectionRowViewModel>>(result));
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
