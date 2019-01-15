using System;
using System.Collections.Generic;
using AutoMapper;
using EmbyStat.Controllers.ViewModels.Show;
using EmbyStat.Controllers.ViewModels.Stat;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmbyStat.Controllers
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
        public IActionResult GetGeneralStats(IEnumerable<Guid> collectionIds)
        {
            var result = _showService.GetGeneralStats(collectionIds);
            var convert = _mapper.Map<ShowStatViewModel>(result);
            return Ok(convert);
        }

        [HttpGet]
        [Route("graphs")]
        public IActionResult GetGraphs(IEnumerable<Guid> collectionIds)
        {
            var result = _showService.GetGraphs(collectionIds);
            var convert = _mapper.Map<ShowGraphsViewModel>(result);
            return Ok(convert);

        }

        [HttpGet]
        [Route("personstats")]
        public IActionResult GetPersonStats(IEnumerable<Guid> collectionIds)
        {
            var result = _showService.GetPeopleStats(collectionIds);
            return Ok(_mapper.Map<PersonStatsViewModel>(result));
        }

        [HttpGet]
        [Route("collectedlist")]
        public IActionResult GetCollection(IEnumerable<Guid> collectionIds)
        {
            var result = _showService.GetCollectionRows(collectionIds);
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
