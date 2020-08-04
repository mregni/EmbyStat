using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common.Models.Query;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using DevExtreme.AspNet.Data;
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
        public IActionResult GetGeneralStats(List<string> libraryIds)
        {
            var result = _movieService.GetStatistics(libraryIds);
            var convert = _mapper.Map<MovieStatisticsViewModel>(result);
            return Ok(convert);
        }

        [HttpGet]
        [Route("list")]
        public IActionResult GetMoviePageList(int skip, int take, string sort, bool requireTotalCount, string filter, List<string> libraryIds)
        {
            var filtersObj = new Filter[0];
            if (filter != null)
            {
                filtersObj = JsonConvert.DeserializeObject<Filter[]>(filter);
            }

            var page = _movieService.GetMoviePage(skip, take, sort, filtersObj, requireTotalCount, libraryIds);

            var convert = _mapper.Map<PageViewModel<MovieColumnViewModel>>(page);
            return Ok(convert);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetMovie(string id)
        {
            var movie = _movieService.GetMovie(id);
            if (movie != null)
            {
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
