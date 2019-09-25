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
        [Route("libraries")]
        public IActionResult GetLibraries()
        {
            var result = _movieService.GetMovieLibraries();
            return Ok(_mapper.Map<IList<LibraryViewModel>>(result));
        }

        [HttpGet]
        [Route("statistics")]
        public async Task<IActionResult> GetGeneralStats(List<string> libraryIds)
        {
            var result = await _movieService.GetStatisticsAsync(libraryIds);
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
