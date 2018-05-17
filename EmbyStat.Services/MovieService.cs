using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Api.EmbyClient;
using EmbyStat.Api.EmbyClient.Model;
using EmbyStat.Common;
using EmbyStat.Common.Converters;
using EmbyStat.Common.Extentions;
using EmbyStat.Common.Models;
using EmbyStat.Repositories.Interfaces;
using EmbyStat.Services.Converters;
using EmbyStat.Services.Interfaces;
using EmbyStat.Services.Models.Graph;
using EmbyStat.Services.Models.Movie;
using EmbyStat.Services.Models.Stat;
using MediaBrowser.Model.Entities;
using Microsoft.EntityFrameworkCore.Query.Internal;
using CollectionType = EmbyStat.Common.Models.CollectionType;
using Constants = EmbyStat.Common.Constants;

namespace EmbyStat.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ICollectionRepository _collectionRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IPersonService _personService;
        private readonly IConfigurationRepository _configurationRepository;

        public MovieService(IMovieRepository movieRepository, ICollectionRepository collectionRepository, IGenreRepository genreRepository, IPersonService personService, IConfigurationRepository configurationRepository)
        {
            _movieRepository = movieRepository;
            _collectionRepository = collectionRepository;
            _genreRepository = genreRepository;
            _personService = personService;
            _configurationRepository = configurationRepository;
        }

        public List<Collection> GetMovieCollections()
        {
            return _collectionRepository.GetCollectionByType(CollectionType.Movies).ToList();
        }

        public MovieStats GetGeneralStatsForCollections(List<string> collectionIds)
        {
            return new MovieStats
            {
                MovieCount = TotalMovieCount(collectionIds),
                GenreCount = TotalMovieGenres(collectionIds),
                TotalPlayableTime = TotalPlayLength(collectionIds),
                HighestRatedMovie = HighestRatedMovie(collectionIds),
                LowestRatedMovie = LowestRatedMovie(collectionIds),
                OldestPremieredMovie = OldestPremieredMovie(collectionIds),
                YoungestPremieredMovie = YoungestPremieredMovie(collectionIds),
                ShortestMovie = ShortestMovie(collectionIds),
                LongestMovie = LongestMovie(collectionIds),
                YoungestAddedMovie = YoungestAddedMovie(collectionIds)
            };
        }

        public async Task<PersonStats> GetPeopleStatsForCollections(List<string> collectionsIds)
        {
            return new PersonStats
            {
                TotalActorCount = TotalTypeCount(collectionsIds, Constants.Actor, Constants.Common.TotalActors),
                TotalDirectorCount = TotalTypeCount(collectionsIds, Constants.Director, Constants.Common.TotalDirectors),
                TotalWriterCount = TotalTypeCount(collectionsIds, Constants.Writer, Constants.Common.TotalWriters),
                MostFeaturedActor = await GetMostFeaturedPerson(collectionsIds, Constants.Actor, Constants.Common.MostFeaturedActor),
                MostFeaturedDirector = await GetMostFeaturedPerson(collectionsIds, Constants.Director, Constants.Common.MostFeaturedDirector),
                MostFeaturedWriter = await GetMostFeaturedPerson(collectionsIds, Constants.Writer, Constants.Common.MostFeaturedWriter),
                MostFeaturedActorsPerGenre = await GetMostFeaturedActorsPerGenre(collectionsIds)
            };
        }

        public MovieGraphs GetGraphs(List<string> collectionIds)
        {
            var movies = _movieRepository.GetAll(collectionIds);

            var graphs = new MovieGraphs();
            graphs.BarGraphs.Add(CalculateGenreGraph(movies));
            graphs.BarGraphs.Add(CalculateRatingGraph(movies));
            graphs.BarGraphs.Add(CalculatePremiereYearGraph(movies));
            graphs.BarGraphs.Add(CalculateOfficialRatingGraph(movies));

            return graphs;
        }

        public SuspiciousTables GetSuspiciousMovies(List<string> collectionsIds)
        {
            var movies = _movieRepository.GetAll(collectionsIds);

            return new SuspiciousTables
            {
                Duplicates = GetDuplicates(movies),
                Shorts = GetShortMovies(movies)
            };
        }

        private List<ShortMovie> GetShortMovies(List<Movie> movies)
        {
            var configuration = _configurationRepository.GetSingle();
            var shortMovies = movies
                .Where(x => x.RunTimeTicks != null)
                .Where(x => new TimeSpan(x.RunTimeTicks ?? 0).TotalMinutes < configuration.ToShortMovie)
                .OrderBy(x => x.SortName)
                .Select((t, i) => new ShortMovie
                {
                    Number = i++,
                    Duration = Math.Floor(new TimeSpan(t.RunTimeTicks ?? 0).TotalMinutes),
                    Title = t.Name,
                    MediaId = t.Id
                })
                .ToList();

            return shortMovies;
        }

        private List<MovieDuplicate> GetDuplicates(List<Movie> movies)
        {
            var list = new List<MovieDuplicate>();

            var duplicatesByImdb = movies.Where(x => !string.IsNullOrWhiteSpace(x.IMDB)).GroupBy(x => x.IMDB).Select(x => new {x.Key, Count = x.Count()}).Where(x => x.Count > 1).ToList();
            for (var i = 0; i < duplicatesByImdb.Count; i++)
            {
                var duplicateMovies = movies.Where(x => x.IMDB == duplicatesByImdb[i].Key).OrderBy(x => x.Id).ToList();
                var itemOne = duplicateMovies.First();
                var itemTwo = duplicateMovies.ElementAt(1);

                list.Add(new MovieDuplicate
                {
                    Number = i,
                    Title = itemOne.Name,
                    Reason= Constants.ByImdb,
                    ItemOne = new MovieDuplicateItem { DateCreated = itemOne.DateCreated, Id = itemOne.Id, Quality = String.Join(",", itemOne.VideoStreams.Select(x => QualityHelper.ConvertToQualityString(x.Width)))},
                    ItemTwo = new MovieDuplicateItem { DateCreated = itemTwo.DateCreated, Id = itemTwo.Id, Quality = String.Join(",", itemTwo.VideoStreams.Select(x => QualityHelper.ConvertToQualityString(x.Width)))}
                });
            }

            var duplicateIds = list.Select(x => x.ItemOne.Id).ToList();
            duplicateIds.AddRange(list.Select(x => x.ItemTwo.Id).ToList());

            var duplicatesByName = movies
                .Where(x => duplicateIds.All(y => y != x.Id))
                .GroupBy(x => x.Name).Select(x => new { x.Key, Count = x.Count() }).Where(x => x.Count > 1).ToList();
            for (var i = 0; i < duplicatesByName.Count; i++)
            {
                var duplicateMovies = movies.Where(x => x.Name == duplicatesByName[i].Key).OrderBy(x => x.Id).ToList();
                var itemOne = duplicateMovies.First();
                var itemTwo = duplicateMovies.ElementAt(1);

                list.Add(new MovieDuplicate
                {
                    Number = i,
                    Title = itemOne.Name,
                    Reason = Constants.ByTitle,
                    ItemOne = new MovieDuplicateItem { DateCreated = itemOne.DateCreated, Id = itemOne.Id, Quality = String.Join(",", itemOne.VideoStreams.Select(x => QualityHelper.ConvertToQualityString(x.Width))) },
                    ItemTwo = new MovieDuplicateItem { DateCreated = itemTwo.DateCreated, Id = itemTwo.Id, Quality = String.Join(",", itemTwo.VideoStreams.Select(x => QualityHelper.ConvertToQualityString(x.Width))) }
                });
            }

            return list.OrderBy(x => x.Title).ToList();
        }

        #region StatCreators

        private Card TotalMovieCount(List<string> collectionIds)
        {
            return new Card
            {
                Title = Constants.Movies.TotalMovies,
                Value = _movieRepository.GetMovieCount(collectionIds).ToString()
            };
        }

        private Card TotalMovieGenres(List<string> collectionIds)
        {
            return new Card
            {
                Title = Constants.Movies.TotalGenres,
                Value = _movieRepository.GetGenreCount(collectionIds).ToString()
            };
        }

        private MoviePoster HighestRatedMovie(List<string> collectionIds)
        {
            var movie = _movieRepository.GetHighestRatedMovie(collectionIds);
            if (movie != null)
            {
                return PosterHelper.ConvertToMoviePoster(movie, Constants.Movies.HighestRated);
            }

            return new MoviePoster();
        }

        private MoviePoster LowestRatedMovie(List<string> collectionIds)
        {
            var movie = _movieRepository.GetLowestRatedMovie(collectionIds);
            if (movie != null)
            {
                return PosterHelper.ConvertToMoviePoster(movie, Constants.Movies.LowestRated);
            }

            return new MoviePoster();
        }

        private MoviePoster OldestPremieredMovie(List<string> collectionIds)
        {
            var movie = _movieRepository.GetOlderPremieredMovie(collectionIds);
            if (movie != null)
            {
                return PosterHelper.ConvertToMoviePoster(movie, Constants.Movies.OldestPremiered);
            }

            return new MoviePoster();
        }

        private MoviePoster YoungestPremieredMovie(List<string> collectionIds)
        {
            var movie = _movieRepository.GetYoungestPremieredMovie(collectionIds);
            if (movie != null)
            {
                return PosterHelper.ConvertToMoviePoster(movie, Constants.Movies.YoungestPremiered);
            }

            return new MoviePoster();
        }

        private MoviePoster ShortestMovie(List<string> collectionIds)
        {
            var movie = _movieRepository.GetShortestMovie(collectionIds);
            if (movie != null)
            {
                return PosterHelper.ConvertToMoviePoster(movie, Constants.Movies.Shortest);
            }

            return new MoviePoster();
        }

        private MoviePoster LongestMovie(List<string> collectionIds)
        {
            var movie = _movieRepository.GetLongestMovie(collectionIds);
            if (movie != null)
            {
                return PosterHelper.ConvertToMoviePoster(movie, Constants.Movies.Longest);
            }

            return new MoviePoster();
        }

        private MoviePoster YoungestAddedMovie(List<string> collectionIds)
        {
            var movie = _movieRepository.GetYoungestAddedMovie(collectionIds);
            if (movie != null)
            {
                return PosterHelper.ConvertToMoviePoster(movie, Constants.Movies.YoungestAdded);
            }

            return new MoviePoster();
        }

        private TimeSpanCard TotalPlayLength(List<string> collectionIds)
        {
            var playLength = new TimeSpan(_movieRepository.GetPlayLength(collectionIds));
            return new TimeSpanCard
            {
                Title = Constants.Movies.TotalPlayLength,
                Days = playLength.Days,
                Hours = playLength.Hours,
                Minutes = playLength.Minutes
            };
        }

        private Card TotalTypeCount(List<string> collectionsIds, string type, string title)
        {
            return new Card
            {
                Value = _movieRepository.GetTotalPersonByType(collectionsIds, type).ToString(),
                Title = title
            };
        }

        private async Task<PersonPoster> GetMostFeaturedPerson(List<string> collectionIds, string type, string title)
        {
            var personId = _movieRepository.GetMostFeaturedPerson(collectionIds, type);

            var person = await _personService.GetPersonById(personId);
            return PosterHelper.ConvertToPersonPoster(person, title);
        }

        private async Task<List<PersonPoster>> GetMostFeaturedActorsPerGenre(List<string> collectionIds)
        {
            var movies = _movieRepository.GetAll(collectionIds);
            var genreIds = _movieRepository.GetGenres(collectionIds);
            var genres = _genreRepository.GetListByIds(genreIds);

            var list = new List<PersonPoster>();
            foreach (var genre in genres.OrderBy(x => x.Name))
            {
                var selectedMovies = movies.Where(x => x.MediaGenres.Any(y => y.GenreId == genre.Id));
                var personId = selectedMovies
                    .SelectMany(x => x.ExtraPersons)
                    .Where(x => x.Type == Constants.Actor)
                    .GroupBy(x => x.PersonId)
                    .Select(group => new { Id = group.Key, Count = group.Count() })
                    .OrderByDescending(x => x.Count)
                    .Select(x => x.Id)
                    .FirstOrDefault();

                var person = await _personService.GetPersonById(personId);
                list.Add(PosterHelper.ConvertToPersonPoster(person, genre.Name));
            }

            return list;
        }

        private Graph<SimpleGraphValue> CalculateGenreGraph(List<Movie> movies)
        {
            var genres = _genreRepository.GetAll();
            var genresData = movies.SelectMany(x => x.MediaGenres).GroupBy(x => x.GenreId)
                .Select(x => new { Name = genres.Single(y => y.Id == x.Key).Name, Count = x.Count()})
                .Select(x => new SimpleGraphValue{Name = x.Name, Value = x.Count})
                .OrderBy(x => x.Name)
                .ToList();

            return new Graph<SimpleGraphValue>
            {
                Title = Constants.CountPerGenre,
                Data = genresData
            };
        }

        private Graph<SimpleGraphValue> CalculatePremiereYearGraph(List<Movie> movies)
        {
            var yearDataList = movies
                .Select(x => x.PremiereDate)
                .GroupBy(x => x.RoundToFive())
                .OrderBy(x => x.Key)
                .ToList();

            var lowestYear = yearDataList.Where(x => x.Key.HasValue).Min(x => x.Key);
            var highestYear = yearDataList.Where(x => x.Key.HasValue).Max(x => x.Key);

            var j = 0;
            for (var i = lowestYear.Value; i < highestYear; i += 5)
            {
                if (yearDataList[j].Key != i)
                {
                    yearDataList.Add(new GraphGrouping<int?, DateTime?> { Key = i, Capacity = 0 });
                }
                else
                {
                    j++;
                }
            }
            
            var yearData = yearDataList
                .Select(x => new { Name = x.Key != null ? $"{x.Key} - {x.Key + 4}" : Constants.Unknown, Count = x.Count() })
                .Select(x => new SimpleGraphValue { Name = x.Name, Value = x.Count })
                .OrderBy(x => x.Name)
                .ToList();

            return new Graph<SimpleGraphValue>
            {
                Title = Constants.CountPerPremiereYear,
                Data = yearData
            };
        }

        private Graph<SimpleGraphValue> CalculateRatingGraph(List<Movie> movies)
        {
            var ratingData = movies.GroupBy(x => x.CommunityRating.RoundToHalf())
                .Select(x => new { Name = x.Key?.ToString() ?? Constants.Unknown, Count = x.Count() })
                .Select(x => new SimpleGraphValue { Name = x.Name, Value = x.Count })
                .OrderBy(x => x.Name)
                .ToList();

            return new Graph<SimpleGraphValue>
            {
                Title = Constants.CountPerCommunityRating,
                Data = ratingData
            };
        }

        private Graph<SimpleGraphValue> CalculateOfficialRatingGraph(List<Movie> movies)
        {
            var ratingData = movies
                .Where(x => !string.IsNullOrWhiteSpace(x.OfficialRating))
                .GroupBy(x => x.OfficialRating.ToUpper())
                .Select(x => new { Name = x.Key, Count = x.Count()})
                .Select(x => new SimpleGraphValue { Name = x.Name, Value = x.Count })
                .OrderBy(x => x.Name)
                .ToList();
            
            return new Graph<SimpleGraphValue>
            {
                Title = Constants.CountPerOfficialRating,
                Data = ratingData
            };
        }


        #endregion
    }
}
