using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EmbyStat.Common.Models.Query;
using EmbyStat.Controllers.HelperClasses;
using EmbyStat.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EmbyStat.Controllers.Show;

[Produces("application/json")]
[Route("api/[controller]")]
public class ShowController : Controller
{
    private readonly IShowService _showService;
    private readonly IMapper _mapper;

    public ShowController(IShowService showService, IMapper mapper)
    {
        _showService = showService;
        _mapper = mapper;
    }

    [HttpGet]
    [Route("libraries")]
    public async Task<IActionResult> GetLibraries()
    {
        var result = await _showService.GetShowLibraries();
        var convert = _mapper.Map<IList<LibraryViewModel>>(result);
        return Ok(convert);
    }
        
    [HttpPost]
    [Route("libraries")]
    public async Task<IActionResult> UpdateLibraries([FromBody] string[] libraryIds)
    {
        await _showService.SetLibraryAsSynced(libraryIds);
        return Ok();
    }

    [HttpGet]
    [Route("statistics")]
    public async Task<IActionResult> GetStatistics()
    {
        var result = await _showService.GetStatistics();
        var convert = _mapper.Map<ShowStatisticsViewModel>(result);
        return Ok(convert);
    }

    [HttpGet]
    [Route("list")]
    public async Task<IActionResult> GetShowPageList(int skip, int take, string sortField, string sortOrder, bool requireTotalCount, string filter)
    {
        var filtersObj = Array.Empty<Filter>();
        if (filter != null)
        {
            filtersObj = JsonConvert.DeserializeObject<Filter[]>(filter);
        }

        var page = await _showService.GetShowPage(skip, take, sortField, sortOrder, filtersObj, requireTotalCount);
        var convert = _mapper.Map<PageViewModel<ShowRowViewModel>>(page);
        return Ok(convert);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetShow(string id)
    {
        var show = await _showService.GetShow(id);
        if (show != null)
        {
            var result = _mapper.Map<ShowDetailViewModel>(show);
            return Ok(result);
        }

        return NotFound(id);
    }

    [HttpGet]
    [Route("typepresent")]
    public IActionResult ShowTypeIsPresent()
    {
        var result = _showService.TypeIsPresent();
        return Ok(result);
    }
}