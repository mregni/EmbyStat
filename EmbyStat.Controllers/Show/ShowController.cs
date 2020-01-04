using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common.Helpers;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetStatistics(List<string> libraryIds)
        {
            var result = await _showService.GetStatistics(libraryIds);
            var convert = _mapper.Map<ShowStatisticsViewModel>(result);
            return Ok(convert);
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
