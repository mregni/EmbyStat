using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common.Models.Query;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        [Route("statistics")]
        public async Task<IActionResult> GetGeneralStats()
        {
            var result = await _movieService.GetStatistics();
            var convert = _mapper.Map<MovieStatisticsViewModel>(result);
            return Ok(convert);
        }

        [HttpGet]
        [Route("list")]
        public async Task<IActionResult> GetMoviePageList(int skip, int take, string sortField, string sortOrder, bool requireTotalCount, string filter)
        {
            var filtersObj = Array.Empty<Filter>();
            if (filter != null)
            {
                filtersObj = JsonConvert.DeserializeObject<Filter[]>(filter);
            }

            var page = await _movieService.GetMoviePage(skip, take, sortField, sortOrder, filtersObj, requireTotalCount);

            var convert = _mapper.Map<PageViewModel<MovieRowViewModel>>(page);
            return Ok(convert);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetMovie(string id)
        {
            var result = _movieService.GetMovie(id);
            if (result != null)
            {
                var movie = _mapper.Map<MovieViewModel>(result);
                return Ok(movie);
            }
            return NotFound(id);
        }

        [HttpGet]
        [Route("libraries")]
        public async Task<IActionResult> GetLibraries()
        {
            var result = await _movieService.GetMovieLibraries();
            return Ok(_mapper.Map<IList<LibraryViewModel>>(result));
        }
        
        [HttpPost]
        [Route("libraries")]
        public async Task<IActionResult> UpdateLibraries([FromBody] string[] libraryIds)
        {
            await _movieService.UpdateLibraries(libraryIds);
            return Ok();
        }

        [HttpGet]
        [Route("typepresent")]
        public IActionResult MovieTypeIsPresent()
        {
            return Ok(_movieService.TypeIsPresent());
        }
    }
}
