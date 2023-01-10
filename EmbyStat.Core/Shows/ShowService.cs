using EmbyStat.Common;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Enums.StatisticEnum;
using EmbyStat.Common.Exceptions;
using EmbyStat.Common.Models.Cards;
using EmbyStat.Common.Models.Charts;
using EmbyStat.Common.Models.DataGrid;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Shows;
using EmbyStat.Common.Models.Query;
using EmbyStat.Core.MediaServers.Interfaces;
using EmbyStat.Core.Shows.Interfaces;
using EmbyStat.Core.Statistics.Interfaces;
using Newtonsoft.Json;

namespace EmbyStat.Core.Shows;

public class ShowService : IShowService
{
    private readonly IShowRepository _showRepository;
    private readonly IMediaServerRepository _mediaServerRepository;
    private readonly IStatisticsService _statisticsService;

    public ShowService(IShowRepository showRepository, IMediaServerRepository mediaServerRepository,
        IStatisticsService statisticsService)
    {
        _showRepository = showRepository;
        _mediaServerRepository = mediaServerRepository;
        _statisticsService = statisticsService;
    }

    public Task<List<Library>> GetShowLibraries()
    {
        return _mediaServerRepository.GetAllLibraries(LibraryType.TvShow);
    }

    public async Task<ShowStatistics> GetStatistics()
    {
        var page = await _statisticsService.GetPage(Constants.StatisticPageIds.ShowPage) ??
                   await _statisticsService.CalculatePage(Constants.StatisticPageIds.ShowPage);

        if (page != null)
        {
            var currentWatchingCard = page.PageCards
                .SingleOrDefault(x => x.StatisticCard.UniqueType == Statistic.ShowTotalCurrentWatchingCount);
            if (currentWatchingCard != null)
            {
                await _statisticsService.CalculateCard(currentWatchingCard.StatisticCard);
            }
            return new ShowStatistics(page);
        }

        throw new NotFoundException($"Page {Constants.StatisticPageIds.ShowPage} is not found");
    }

    public bool TypeIsPresent()
    {
        return _showRepository.Any();
    }

    public async Task<Page<Show>> GetShowPage(int skip, int take, string sortField, string sortOrder, Filter[] filters,
        bool requireTotalCount)
    {
        var list = await _showRepository.GetShowPage(skip, take, sortField, sortOrder, filters);

        var page = new Page<Show>(list);
        if (requireTotalCount)
        {
            page.TotalCount = await _showRepository.Count(filters);
        }

        return page;
    }

    public Task<Show> GetShow(string id)
    {
        return _showRepository.GetShowByIdWithEpisodes(id);
    }

    public async Task SetLibraryAsSynced(string[] libraryIds)
    {
        await _mediaServerRepository.SetLibraryAsSynced(libraryIds, LibraryType.TvShow);
        await _showRepository.RemoveUnwantedShows(libraryIds);
    }
}