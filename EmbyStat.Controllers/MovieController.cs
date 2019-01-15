using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
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
        public IActionResult GetGeneralStats(List<Guid> collectionIds)
        {
            var result = _movieService.GetGeneralStatsForCollections(collectionIds);
            var convert = _mapper.Map<MovieStatsViewModel>(result);
            return Ok(convert);
        }

        [HttpGet]
        [Route("personstats")]
        public async Task<IActionResult> GetPersonStats(List<Guid> collectionIds)
        {
            var result = await _movieService.GetPeopleStatsForCollections(collectionIds);
            return Ok(_mapper.Map<PersonStatsViewModel>(result));
        }

        [HttpGet]
        [Route("suspicious")]
        public IActionResult GetDuplicates(List<Guid> collectionIds)
        {
            var result = _movieService.GetSuspiciousMovies(collectionIds);
            return Ok(_mapper.Map<SuspiciousTablesViewModel>(result));
        }

        [HttpGet]
        [Route("graphs")]
        public IActionResult GetGraphs(List<Guid> collectionIds)
        {
            var graphs = _movieService.GetGraphs(collectionIds);
            return Ok(_mapper.Map<MovieGraphsViewModel>(graphs));
        }

        [HttpGet]
        [Route("movietypepresent")]
        public IActionResult MovieTypeIsPresent()
        {
            return Ok(_movieService.MovieTypeIsPresent());
        }
    }
}
