using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
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
        [Route("collections")]
        public IActionResult GetCollections()
        {
            var result = _showService.GetShowCollections();
            return Ok(_mapper.Map< IList<CollectionViewModel>>(result));
        }

        [HttpGet]
        [Route("generalstats")]
        public async Task<IActionResult> GetGeneralStats(List<string> collectionIds)
        {
            var result = await _showService.GetGeneralStats(collectionIds);
            var convert = _mapper.Map<ShowStatViewModel>(result);
            return Ok(convert);
        }

        [HttpGet]
        [Route("graphs")]
        public async Task<IActionResult> GetGraphs(List<string> collectionIds)
        {
            var result = await _showService.GetGraphs(collectionIds);
            var convert = _mapper.Map<ShowGraphsViewModel>(result);
            return Ok(convert);

        }

        [HttpGet]
        [Route("personstats")]
        public async Task<IActionResult> GetPersonStats(List<string> collectionIds)
        {
            var result = await _showService.GetPeopleStats(collectionIds);
            return Ok(_mapper.Map<PersonStatsViewModel>(result));
        }

        [HttpGet]
        [Route("collectedlist")]
        public async Task<IActionResult> GetCollection(List<string> collectionIds)
        {
            var result = await _showService.GetCollectionRows(collectionIds);
            return Ok(_mapper.Map<IList<ShowCollectionRowViewModel>>(result));
        }

        [HttpGet]
        [Route("showtypepresent")]
        public IActionResult MovieTypeIsPresent()
        {
            return Ok(_showService.ShowTypeIsPresent());
        }
    }
}
