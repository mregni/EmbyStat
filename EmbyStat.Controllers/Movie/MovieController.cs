using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using EmbyStat.Common.Models.Stats;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmbyStat.Controllers.Movie
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
        [Route("getcollections")]
        public IActionResult GetCollections()
        {
            var result = _movieService.GetMovieCollections();
            return Ok(Mapper.Map<IList<CollectionViewModel>>(result));
        }

        [HttpPost]
        [Route("getgeneralstats")]
        public IActionResult GetGeneralStats([FromBody]List<string> collectionIds)
        {
            var result = _movieService.GetGeneralStatsForCollections(collectionIds);
            return Ok(Mapper.Map<MovieStatsViewModel>(result));
        }
    }
}
