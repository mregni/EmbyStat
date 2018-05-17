using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Controllers.ViewModels.Graph;
using EmbyStat.Controllers.ViewModels.Movie;
using EmbyStat.Controllers.ViewModels.Stat;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmbyStat.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet]
        [Route("collections")]
        public IActionResult GetCollections()
        {
            var result = _movieService.GetMovieCollections();
            return Ok(Mapper.Map<IList<CollectionViewModel>>(result));
        }

        [HttpGet]
        [Route("generalstats")]
        public IActionResult GetGeneralStats(List<string> collectionIds)
        {
            var result = _movieService.GetGeneralStatsForCollections(collectionIds);
            var convert = Mapper.Map<MovieStatsViewModel>(result);
            return Ok(convert);
        }

        [HttpGet]
        [Route("personstats")]
        public async Task<IActionResult> GetPersonStats(List<string> collectionIds)
        {
            var result = await _movieService.GetPeopleStatsForCollections(collectionIds);
            return Ok(Mapper.Map<PersonStatsViewModel>(result));
        }

        [HttpGet]
        [Route("suspicious")]
        public IActionResult GetDuplicates(List<string> collectionIds)
        {
            var result = _movieService.GetSuspiciousMovies(collectionIds);
            return Ok(Mapper.Map<SuspiciousTablesViewModel>(result));
        }

        [HttpGet]
        [Route("graphs")]
        public IActionResult GetGraphs(List<string> collectionIds)
        {
            var graphs = _movieService.GetGraphs(collectionIds);
            return Ok(Mapper.Map<MovieGraphsViewModel>(graphs));
        }
    }
}
