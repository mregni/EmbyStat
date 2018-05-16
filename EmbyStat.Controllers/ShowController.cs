using System;
using System.Collections.Generic;
using System.Text;
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
        public IActionResult GetGeneralStats(List<string> collectionIds)
        {
            var result = _showService.GetGeneralStats(collectionIds);
            var convert = Mapper.Map<ShowStatViewModel>(result);
            return Ok(convert);
        }

        [HttpGet]
        [Route("graphs")]
        public IActionResult GetGraphs(List<string> collectionIds)
        {
            var result = _showService.GetGraphs(collectionIds);
            var convert = Mapper.Map<ShowGraphsViewModel>(result);
            return Ok(convert);

        }
    }
}
