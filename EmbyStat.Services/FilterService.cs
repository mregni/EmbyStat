﻿using System;
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

        public FilterValues GetFilterValues(LibraryType type, string field, string[] libraryIds)
        {
            var values = _filterRepository.Get(field, libraryIds);
            return values ?? CalculateFilterValues(type, field, libraryIds);
        }

        public FilterValues CalculateFilterValues(LibraryType type, string field, string[] libraryIds)
        {
            switch (type)
            {
                case LibraryType.Movies: return CalculateMovieFilterValues(field, libraryIds);
                //TODO Add show filters here
                default: return null;
            }
        }

        private FilterValues CalculateMovieFilterValues(string field, string[] libraryIds)
        {
            var values = new FilterValues
            {
                Id = Guid.NewGuid().ToString(),
                Field = field,
                Libraries = libraryIds
            };
            switch (field.ToLowerInvariant())
            {
                case "subtitle":
                    var re = new Regex(@"\ \([0-9a-zA-Z -_]*\)$");
                    values.Values = _movieRepository.CalculateSubtitleFilterValues(libraryIds).ToArray();
                    values.Values.ForEach(x => re.Replace(x.Label, string.Empty));
                    break;
                case "genre":
                    values.Values = _movieRepository.CalculateGenreFilterValues(libraryIds).ToArray();
                    break;
                case "container":
                    values.Values = _movieRepository.CalculateContainerFilterValues(libraryIds).ToArray();
                    break;
                case "collection":
                    values.Values = _movieRepository.CalculateCollectionFilterValues().ToArray();
                    break;
                case "codec":
                    values.Values = _movieRepository.CalculateCodecFilterValues(libraryIds).ToArray();
                    break;
                case "videorange":
                    values.Values = _movieRepository.CalculateVideoRangeFilterValues(libraryIds).ToArray();
                    break;
                default: return null;
            }

            _filterRepository.Insert(values);
            return values;
        }
    }
}
