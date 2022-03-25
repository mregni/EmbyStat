using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Common.Models.Entities.Helpers;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using MoreLinq;

namespace EmbyStat.Services
{
    public class FilterService : IFilterService
    {
        private readonly IFilterRepository _filterRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IShowRepository _showRepository;

        public FilterService(IFilterRepository filterRepository, IMovieRepository movieRepository,
            IShowRepository showRepository)
        {
            _filterRepository = filterRepository;
            _movieRepository = movieRepository;
            _showRepository = showRepository;
        }

        public async Task<FilterValues> GetFilterValues(LibraryType type, string field)
        {
            var values = await _filterRepository.Get(type, field);
            return values ?? CalculateFilterValues(type, field);
        }

        public FilterValues CalculateFilterValues(LibraryType type, string field)
        {
            var values = new FilterValues
            {
                Field = field,
                Type = type
            };

            switch (type)
            {
                case LibraryType.Movies:
                    values.Values = CalculateMovieFilterValues(field).ToArray();
                    break;
                case LibraryType.TvShow:
                    values.Values = CalculateShowFilterValues(field).ToArray();
                    break;
                default:
                    return null;
            }

            _filterRepository.Insert(values);
            return values;
        }

        private IEnumerable<LabelValuePair> CalculateShowFilterValues(string field)
        {
            return field.ToLowerInvariant() switch
            {
                "genre" => _showRepository.CalculateGenreFilterValues(),
                _ => null
            };
        }

        private IEnumerable<LabelValuePair> CalculateMovieFilterValues(string field)
        {
            switch (field.ToLowerInvariant())
            {
                case "subtitle":
                    var re = new Regex(@"\ \([0-9a-zA-Z -_]*\)$");
                    var values = _movieRepository.CalculateSubtitleFilterValues().ToArray();
                    values.ForEach(x => re.Replace(x.Label, string.Empty));
                    return values;
                case "genre":
                    return _movieRepository.CalculateGenreFilterValues().ToArray();
                case "container":
                    return _movieRepository.CalculateContainerFilterValues().ToArray();
                case "codec":
                    return _movieRepository.CalculateCodecFilterValues().ToArray();
                case "videorange":
                    return _movieRepository.CalculateVideoRangeFilterValues().ToArray();
                default: return null;
            }
        }
    }
}