using System;
using System.Linq;
using System.Text.RegularExpressions;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Interfaces;
using MoreLinq;

namespace EmbyStat.Services
{
    public class FilterService : IFilterService
    {
        private readonly IFilterRepository _filterRepository;
        private readonly IMovieRepository _movieRepository;

        public FilterService(IFilterRepository filterRepository, IMovieRepository movieRepository)
        {
            _filterRepository = filterRepository;
            _movieRepository = movieRepository;
        }

        public FilterValues GetFilterValues(LibraryType type, string field)
        {
            var values = _filterRepository.Get(field);
            return values ?? CalculateFilterValues(type, field);
        }

        public FilterValues CalculateFilterValues(LibraryType type, string field)
        {
            switch (type)
            {
                case LibraryType.Movies: return CalculateMovieFilterValues(field);
                //TODO Add show filters here
                default: return null;
            }
        }

        private FilterValues CalculateMovieFilterValues(string field)
        {
            var values = new FilterValues
            {
                Id = Guid.NewGuid().ToString(),
                Field = field
            };
            switch (field.ToLowerInvariant())
            {
                case "subtitle":
                    var re = new Regex(@"\ \([0-9a-zA-Z -_]*\)$");
                    values.Values = _movieRepository.CalculateSubtitleFilterValues().ToArray();
                    values.Values.ForEach(x => re.Replace(x.Label, string.Empty));
                    break;
                case "genre":
                    values.Values = _movieRepository.CalculateGenreFilterValues().ToArray();
                    break;
                case "container":
                    values.Values = _movieRepository.CalculateContainerFilterValues().ToArray();
                    break;
                case "collection":
                    values.Values = _movieRepository.CalculateCollectionFilterValues().ToArray();
                    break;
                case "codec":
                    values.Values = _movieRepository.CalculateCodecFilterValues().ToArray();
                    break;
                case "videorange":
                    values.Values = _movieRepository.CalculateVideoRangeFilterValues().ToArray();
                    break;
                default: return null;
            }

            _filterRepository.Insert(values);
            return values;
        }
    }
}
