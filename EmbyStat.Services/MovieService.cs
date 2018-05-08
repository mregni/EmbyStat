using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmbyStat.Api.EmbyClient;
using EmbyStat.Api.EmbyClient.Model;
using EmbyStat.Common;
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

        public MovieService(IMovieRepository movieRepository, ICollectionRepository collectionRepository, IGenreRepository genreRepository, IPersonService personService)
        {
            _movieRepository = movieRepository;
            _collectionRepository = collectionRepository;
            _genreRepository = genreRepository;
            _personService = personService;
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

        public async Task<MoviePersonStats> GetPeopleStatsForCollections(List<string> collectionsIds)
        {
            return new MoviePersonStats
            {
                TotalActorCount = TotalActorCount(collectionsIds),
                TotalDirectorCount = TotalDirectorCount(collectionsIds),
                TotalWriterCount = TotalWriterCount(collectionsIds),
                MostFeaturedActor = await GetMostFeaturedActor(collectionsIds),
                MostFeaturedDirector = await GetMostFeaturedDirector(collectionsIds),
                MostFeaturedWriter = await GetMostFeaturedWriter(collectionsIds),
                MostFeaturedActorsPerGenre = await GetMostFeaturedActorsPerGenre(collectionsIds)
            };
        }

        public List<MovieDuplicate> GetDuplicates(List<string> collectionIds)
        {
            var movies = _movieRepository.GetAll(collectionIds);
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

        public MovieGraphs GetGraphs(List<string> collectionIds)
        {
            var movies = _movieRepository.GetAll(collectionIds);

            var graphs = new MovieGraphs();
            graphs.BarGraphs.Add(CalculateGenreGraph(movies));
            graphs.BarGraphs.Add(CalculateRatingGraph(movies));
            graphs.BarGraphs.Add(CalculatePremiereYearGraph(movies));
            graphs.BarGraphs.Add(CalculateRatingGraph(movies));

            return graphs;
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

        private Poster HighestRatedMovie(List<string> collectionIds)
        {
            var movie = _movieRepository.GetHighestRatedMovie(collectionIds);
            if (movie != null)
            {
                return PosterHelper.ConvertToPoster(movie, Constants.Movies.HighestRated);
            }

            return new Poster();
        }

        private Poster LowestRatedMovie(List<string> collectionIds)
        {
            var movie = _movieRepository.GetLowestRatedMovie(collectionIds);
            if (movie != null)
            {
                return PosterHelper.ConvertToPoster(movie, Constants.Movies.LowestRated);
            }

            return new Poster();
        }

        private Poster OldestPremieredMovie(List<string> collectionIds)
        {
            var movie = _movieRepository.GetOlderPremieredMovie(collectionIds);
            if (movie != null)
            {
                return PosterHelper.ConvertToPoster(movie, Constants.Movies.OldestPremiered);
            }

            return new Poster();
        }

        private Poster YoungestPremieredMovie(List<string> collectionIds)
        {
            var movie = _movieRepository.GetYoungestPremieredMovie(collectionIds);
            if (movie != null)
            {
                return PosterHelper.ConvertToPoster(movie, Constants.Movies.YoungestPremiered);
            }

            return new Poster();
        }

        private Poster ShortestMovie(List<string> collectionIds)
        {
            var movie = _movieRepository.GetShortestMovie(collectionIds);
            if (movie != null)
            {
                return PosterHelper.ConvertToPoster(movie, Constants.Movies.Shortest);
            }

            return new Poster();
        }

        private Poster LongestMovie(List<string> collectionIds)
        {
            var movie = _movieRepository.GetLongestMovie(collectionIds);
            if (movie != null)
            {
                return PosterHelper.ConvertToPoster(movie, Constants.Movies.Longest);
            }

            return new Poster();
        }

        private Poster YoungestAddedMovie(List<string> collectionIds)
        {
            var movie = _movieRepository.GetYoungestAddedMovie(collectionIds);
            if (movie != null)
            {
                return PosterHelper.ConvertToPoster(movie, Constants.Movies.YoungestAdded);
            }

            return new Poster();
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

        private Card TotalActorCount(List<string> collectionsIds)
        {
            return new Card
            {
                Value = _movieRepository.GetTotalActors(collectionsIds).ToString(),
                Title = Constants.Movies.TotalActors
            };
        }

        private Card TotalDirectorCount(List<string> collectionsIds)
        {
            return new Card
            {
                Value = _movieRepository.GetTotalDirectors(collectionsIds).ToString(),
                Title = Constants.Movies.TotalDirectors
            };
        }

        private Card TotalWriterCount(List<string> collectionsIds)
        {
            return new Card
            {
                Value = _movieRepository.GetTotalWriters(collectionsIds).ToString(),
                Title = Constants.Movies.TotalWriters
            };
        }

        private async Task<PersonPoster> GetMostFeaturedActor(List<string> collectionIds)
        {
            var personId = _movieRepository.GetMostFeaturedPerson(collectionIds, Constants.Actor);

            var person = await _personService.GetPersonById(personId);
            return PosterHelper.ConvertToPersonPoster(person, Constants.Movies.MostFeaturedActor);
        }

        private async Task<PersonPoster> GetMostFeaturedDirector(List<string> collectionIds)
        {
            var personId = _movieRepository.GetMostFeaturedPerson(collectionIds, Constants.Director);

            var person = await _personService.GetPersonById(personId);
            return PosterHelper.ConvertToPersonPoster(person, Constants.Movies.MostFeaturedDirector);
        }

        private async Task<PersonPoster> GetMostFeaturedWriter(List<string> collectionIds)
        {
            var personId = _movieRepository.GetMostFeaturedPerson(collectionIds, Constants.Writer);

            var person = await _personService.GetPersonById(personId);
            return PosterHelper.ConvertToPersonPoster(person, Constants.Movies.MostFeaturedWriter);
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
                .Select(x => new {Name = genres.Single(y => y.Id == x.Key).Name, Count = x.Count()})
                .Select(x => new SimpleGraphValue{Name = x.Name, Value = x.Count})
                .OrderBy(x => x.Name)
                .ToList();

            return new Graph<SimpleGraphValue>
            {
                Title = Constants.CountPerGenre,
                Data = genresData
            };
        }

        private Graph<SimpleGraphValue> CalculateRatingGraph(List<Movie> movies)
        {
            var ratingData = movies.GroupBy(x => RoundRating(x.CommunityRating))
                .Select(x => new { Name = x.Key?.ToString() ?? Constants.Unknown , Count = x.Count() })
                .Select(x => new SimpleGraphValue { Name = x.Name, Value = x.Count })
                .OrderBy(x => x.Name)
                .ToList();

            return new Graph<SimpleGraphValue>
            {
                Title = Constants.CountPerCommunityRating,
                Data = ratingData
            };
        }

        private Graph<SimpleGraphValue> CalculatePremiereYearGraph(List<Movie> movies)
        {
            var yearDataList = movies
                .Select(x => x.PremiereDate)
                .GroupBy(RoundYear)
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

        #region Helpers

        private double? RoundRating(float? rating)
        {
            if (rating.HasValue)
            {
                return Math.Round((double)rating.Value * 2, MidpointRounding.AwayFromZero) / 2;
            }

            return null;
        }

        private int? RoundYear(DateTime? date)
        {
            if (date.HasValue)
            {
                var result = (int)Math.Floor((double)date.Value.Year / 5) * 5;
                return result;
            }

            return null;
        }
        #endregion
    }
}
