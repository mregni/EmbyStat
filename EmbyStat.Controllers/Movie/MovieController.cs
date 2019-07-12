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
        [Route("statistics")]
        public async Task<IActionResult> GetGeneralStats(List<string> collectionIds)
        {
            var result = await _movieService.GetMovieStatistics(collectionIds);
            var convert = _mapper.Map<MovieStatisticsViewModel>(result);
            return Ok(convert);
        }

        [HttpGet]
        [Route("typepresent")]
        public IActionResult MovieTypeIsPresent()
        {
            return Ok(_movieService.TypeIsPresent());
        }
    }
}
