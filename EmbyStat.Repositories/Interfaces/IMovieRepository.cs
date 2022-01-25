using System;
using EmbyStat.Common.Models.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Repositories.Interfaces.Helpers;
using EmbyStat.Common.Models;
using EmbyStat.Common.Models.Query;
using EmbyStat.Common.SqLite;

namespace EmbyStat.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        SqlMovie GetById(string id);
        void RemoveAll();
        Task UpsertRange(IEnumerable<SqlMovie> movies);
        IEnumerable<SqlMovie> GetAll(IReadOnlyList<string> libraryIds);
        IEnumerable<SqlMovie> GetAll(IReadOnlyList<string> libraryIds, bool includeGenres);
        Task<int> Count(IReadOnlyList<string> libraryIds);
        Task<int> Count(Filter[] filters, IReadOnlyList<string> libraryIds);
        bool Any();

        Task<Dictionary<string, int>> GetOfficialRatingChartValues(IReadOnlyList<string> libraryIds);
        Task<Dictionary<string, int>> GetMovieGenreChartValues(IReadOnlyList<string> libraryIds);
        IEnumerable<SqlMovie> GetAllWithImdbId(IReadOnlyList<string> libraryIds);
        long? GetTotalRuntime(IReadOnlyList<string> libraryIds);
        IEnumerable<SqlMovie> GetShortestMovie(IReadOnlyList<string> libraryIds, long toShortMovieTicks, int count);
        IEnumerable<SqlMovie> GetLongestMovie(IReadOnlyList<string> libraryIds, int count);
        double GetTotalDiskSpace(IReadOnlyList<string> libraryIds);
        IEnumerable<SqlMovie> GetToShortMovieList(IReadOnlyList<string> libraryIds, int toShortMovieMinutes);
        IEnumerable<SqlMovie> GetMoviesWithoutImdbId(IReadOnlyList<string> libraryIds);
        IEnumerable<SqlMovie> GetMoviesWithoutPrimaryImage(IReadOnlyList<string> libraryIds);
        Task<IEnumerable<SqlMovie>> GetMoviePage(int skip, int take, string sortField, string sortOrder, Filter[] filters, List<string> libraryIds);
        IEnumerable<LabelValuePair> CalculateSubtitleFilterValues(IReadOnlyList<string> libraryIds);
        IEnumerable<LabelValuePair> CalculateContainerFilterValues(IReadOnlyList<string> libraryIds);
        IEnumerable<LabelValuePair> CalculateGenreFilterValues(IReadOnlyList<string> libraryIds);
        IEnumerable<LabelValuePair> CalculateCollectionFilterValues();
        IEnumerable<LabelValuePair> CalculateCodecFilterValues(IReadOnlyList<string> libraryIds);
        IEnumerable<LabelValuePair> CalculateVideoRangeFilterValues(IReadOnlyList<string> libraryIds);
        IEnumerable<SqlMovie> GetNewestPremieredMedia(IReadOnlyList<string> libraryIds, int count);
        IEnumerable<SqlMovie> GetLatestAddedMedia(IReadOnlyList<string> libraryIds, int count);
        IEnumerable<SqlMovie> GetOldestPremieredMedia(IReadOnlyList<string> libraryIds, int count);
        IEnumerable<SqlMovie> GetHighestRatedMedia(IReadOnlyList<string> libraryIds, int count);
        IEnumerable<SqlMovie> GetLowestRatedMedia(IReadOnlyList<string> libraryIds, int count);
        IEnumerable<float?> GetCommunityRatings(IReadOnlyList<string> libraryIds);
        IEnumerable<DateTime?> GetPremiereYears(IReadOnlyList<string> libraryIds);

        int GetMediaCountForPerson(string name, string genre);
        int GetMediaCountForPerson(string name);
        Task<int> GetGenreCount(IReadOnlyList<string> libraryIds);
        int GetPeopleCount(IReadOnlyList<string> libraryIds, PersonType type);
        IEnumerable<string> GetMostFeaturedPersons(IReadOnlyList<string> libraryIds, PersonType type, int count);
    }
}
