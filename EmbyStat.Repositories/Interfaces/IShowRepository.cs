using System;
using System.Collections.Generic;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces.Helpers;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IShowRepository : IMediaRepository<Show>
    {
        void UpsertShow(Show show);
        void UpdateShow(Show show);
        List<Show> GetAllShows(IReadOnlyList<string> libraryIds, bool includeSeasons, bool includeEpisodes);
        Episode GetEpisodeById(int id);
        Season GetSeasonById(int id);
        List<Episode> GetAllEpisodesForShow(int showId);
        Show GetShowById(int showId);
        void RemoveShowsThatAreNotUpdated(DateTime startTime);
        void AddSeason(Season season);
        void AddEpisode(Episode episode);
    }
}
