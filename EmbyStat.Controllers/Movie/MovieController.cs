using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmbyStat.Controllers.Movie
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IMapper _mapper;

        public MovieController(IMovieService movieService, IMapper mapper)
        {
            _movieService = movieService;
            _mapper = mapper;
        }
        
        [HttpGet]
        [Route("collections")]
        public IActionResult GetCollections()
        {
            var result = _movieService.GetMovieCollections();
            return Ok(_mapper.Map<IList<CollectionViewModel>>(result));
        }

        [HttpGet]
        [Route("generalstats")]
        public async Task<IActionResult> GetGeneralStats(List<string> collectionIds)
        {
            var result = await _movieService.GetGeneralStatsForCollections(collectionIds);
            var convert = _mapper.Map<MovieStatsViewModel>(result);
            return Ok(convert);
        }

        [HttpGet]
        [Route("peoplestats")]
        public async Task<IActionResult> GetPersonStats(List<string> collectionIds)
        {
            var result = await _movieService.GetPeopleStatsForCollections(collectionIds);
            return Ok(_mapper.Map<PersonStatsViewModel>(result));
        }

        [HttpGet]
        [Route("suspicious")]
        public async Task<IActionResult> GetDuplicates(List<string> collectionIds)
        {
            var result = await _movieService.GetSuspiciousMovies(collectionIds);
            return Ok(_mapper.Map<SuspiciousTablesViewModel>(result));
        }

        [HttpGet]
        [Route("charts")]
        public async Task<IActionResult> GetCharts(List<string> collectionIds)
        {
            var graphs = await _movieService.GetCharts(collectionIds);
            return Ok(_mapper.Map<MovieChartsViewModel>(graphs));
        }

        [HttpGet]
        [Route("typepresent")]
        public IActionResult MovieTypeIsPresent()
        {
            return Ok(_movieService.TypeIsPresent());
        }
    }
}
