using System.Collections.Generic;
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
        public IActionResult GetGeneralStats(List<string> collectionIds)
        {
            var result = _movieService.GetGeneralStatsForCollections(collectionIds);
            var convert = _mapper.Map<MovieStatsViewModel>(result);
            return Ok(convert);
        }

        [HttpGet]
        [Route("peoplestats")]
        public IActionResult GetPersonStats(List<string> collectionIds)
        {
            var result = _movieService.GetPeopleStatsForCollections(collectionIds);
            return Ok(_mapper.Map<PersonStatsViewModel>(result));
        }

        [HttpGet]
        [Route("suspicious")]
        public IActionResult GetDuplicates(List<string> collectionIds)
        {
            var result = _movieService.GetSuspiciousMovies(collectionIds);
            return Ok(_mapper.Map<SuspiciousTablesViewModel>(result));
        }

        [HttpGet]
        [Route("charts")]
        public IActionResult GetCharts(List<string> collectionIds)
        {
            var graphs = _movieService.GetCharts(collectionIds);
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
