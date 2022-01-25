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
        public async Task<IActionResult> GetGeneralStats(List<string> libraryIds)
        {
            var result = await _movieService.GetStatistics(libraryIds);
            var convert = _mapper.Map<MovieStatisticsViewModel>(result);
            return Ok(convert);
        }

        [HttpGet]
        [Route("list")]
        public async Task<IActionResult> GetMoviePageList(int skip, int take, string sortField, string sortOrder, bool requireTotalCount, string filter, List<string> libraryIds)
        {
            var filtersObj = Array.Empty<Filter>();
            if (filter != null)
            {
                filtersObj = JsonConvert.DeserializeObject<Filter[]>(filter);
            }

            var page = await _movieService.GetMoviePage(skip, take, sortField, sortOrder, filtersObj, requireTotalCount, libraryIds);

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
        public IActionResult GetLibraries()
        {
            var result = _movieService.GetMovieLibraries();
            return Ok(_mapper.Map<IList<LibraryViewModel>>(result));
        }

        [HttpGet]
        [Route("typepresent")]
        public IActionResult MovieTypeIsPresent()
        {
            return Ok(_movieService.TypeIsPresent());
        }
    }
}
