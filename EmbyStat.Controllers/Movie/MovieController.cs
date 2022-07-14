using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Query;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Core.Movies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MoreLinq;
using Newtonsoft.Json;

namespace EmbyStat.Controllers.Movie;

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
    public async Task<IActionResult> GetMovie(string id)
    {
        var result = await _movieService.GetMovie(id);
        if (result == null)
        {
            return NotFound(id);
        }
        
        var movie = _mapper.Map<MovieViewModel>(result);
        return Ok(movie);
    }

    [HttpGet]
    [Route("libraries")]
    public async Task<IActionResult> GetLibraries()
    {
        var result = await _movieService.GetMovieLibraries();
        var map = _mapper.Map<IList<LibraryViewModel>>(result, opts =>
            opts.AfterMap((src, dest) =>
            {
                dest.ForEach(x => x.Sync = result
                    .Single(y => y.Id == x.Id).SyncTypes
                    .Any(y => y.SyncType == LibraryType.Movies));
            }));
        
        return Ok(map);
    }
        
    [HttpPost]
    [Route("libraries")]
    public async Task<IActionResult> UpdateLibraries([FromBody] string[] libraryIds)
    {
        await _movieService.SetLibraryAsSynced(libraryIds);
        return Ok();
    }

    [HttpGet]
    [Route("typepresent")]
    public IActionResult MovieTypeIsPresent()
    {
        return Ok(_movieService.TypeIsPresent());
    }
}