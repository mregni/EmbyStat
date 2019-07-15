using System.Collections.Generic;
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
        [Route("statistics")]
        public IActionResult GetStatistics(List<string> collectionIds)
        {
            var result = _showService.GetStatistics(collectionIds);
            var convert = _mapper.Map<ShowStatisticsViewModel>(result);
            return Ok(convert);
        }

        [HttpGet]
        [Route("collectedlist")]
        public IActionResult GetCollection(List<string> collectionIds)
        {
            var result = _showService.GetCollectionRows(collectionIds);
            return Ok(_mapper.Map<IList<ShowCollectionRowViewModel>>(result));
        }

        [HttpGet]
        [Route("typepresent")]
        public IActionResult ShowTypeIsPresent()
        {
            return Ok(_showService.TypeIsPresent());
        }
    }
}
