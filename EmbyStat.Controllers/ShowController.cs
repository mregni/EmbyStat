using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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

        public ShowController(IShowService showService)
        {
            _showService = showService;
        }

        [HttpGet]
        [Route("collections")]
        public IActionResult GetCollections()
        {
            var result = _showService.GetShowCollections();
            return Ok(Mapper.Map< IList<CollectionViewModel>>(result));
        }

        [HttpGet]
        [Route("generalstats")]
        public IActionResult GetGeneralStats(IEnumerable<string> collectionIds)
        {
            var result = _showService.GetGeneralStats(collectionIds);
            var convert = Mapper.Map<ShowStatViewModel>(result);
            return Ok(convert);
        }

        [HttpGet]
        [Route("graphs")]
        public IActionResult GetGraphs(IEnumerable<string> collectionIds)
        {
            var result = _showService.GetGraphs(collectionIds);
            var convert = Mapper.Map<ShowGraphsViewModel>(result);
            return Ok(convert);

        }

        [HttpGet]
        [Route("personstats")]
        public IActionResult GetPersonStats(IEnumerable<string> collectionIds)
        {
            var result = _showService.GetPeopleStats(collectionIds);
            return Ok(Mapper.Map<PersonStatsViewModel>(result));
        }

        [HttpGet]
        [Route("collectedlist")]
        public IActionResult GetCollection(IEnumerable<string> collectionIds)
        {
            var result = _showService.GetCollectionRows(collectionIds);
            return Ok(Mapper.Map<IList<ShowCollectionRowViewModel>>(result));
        }

        [HttpGet]
        [Route("showtypepresent")]
        public IActionResult MovieTypeIsPresent()
        {
            return Ok(_showService.ShowTypeIsPresent());
        }
    }
}
