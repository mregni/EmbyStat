using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common;
using EmbyStat.Common.Models;
using EmbyStat.Repositories.Interfaces;
using MediaBrowser.Model.Extensions;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace EmbyStat.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        public void Add(Movie movie)
        {
            using (var context = new ApplicationDbContext())
            {
                var peopleToDelete = new List<string>();
                foreach (var person in movie.ExtraPersons)
                {
                    var temp = context.People.AsNoTracking().SingleOrDefault(x => x.Id == person.PersonId);
                    if (temp == null)
                    {
                        Log.Warning($"We couldn't find the person with Id {person.PersonId} for movie ({movie.Id}) {movie.Name} in our database. This is because Emby didn't return the actor when we queried the people for the parent id. As a fix we will remove the person from the movie now.");
                        peopleToDelete.Add(person.PersonId);
                    }
                }
                peopleToDelete.ForEach(x => movie.ExtraPersons.Remove(movie.ExtraPersons.SingleOrDefault(y => y.PersonId == x)));

                var genresToDelete = new List<string>();
                foreach (var genre in movie.MediaGenres)
                {
                    var temp = context.Genres.AsNoTracking().SingleOrDefault(x => x.Id == genre.GenreId);
                    if (temp == null)
                    {
                        Log.Warning($"We couldn't find the genre with Id {genre.GenreId} for movie ({movie.Id}) {movie.Name} in our database. This is because Emby didn't return the genre when we queried the genres for the parent id. As a fix we will remove the genre from the movie now.");
                        genresToDelete.Add(genre.GenreId);
                    }
                }
                genresToDelete.ForEach(x => movie.MediaGenres.Remove(movie.MediaGenres.SingleOrDefault(y => y.GenreId == x)));

                context.Movies.Add(movie);
                context.SaveChanges();
            }
        }

        public int GetTotalPersonByType(List<string> collections, string type)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context.Movies.Include(x => x.ExtraPersons).AsQueryable();

                if (collections.Any())
                {
                    query = query.Where(x => collections.Any(y => x.CollectionId == y));
                }

                var extraPerson = query.SelectMany(x => x.ExtraPersons).AsEnumerable();
                var people = extraPerson.DistinctBy(x => x.PersonId);
                return people.Count(x => x.Type == type);
            }
        }

        public string GetMostFeaturedPerson(List<string> collections, string type)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context.Movies.Include(x => x.ExtraPersons).AsQueryable();

                if (collections.Any())
                {
                    query = query.Where(x => collections.Any(y => x.CollectionId == y));
                }

                var person = query
                    .SelectMany(x => x.ExtraPersons)
                    .AsEnumerable()
                    .Where(x => x.Type == type)
                    .GroupBy(x => x.PersonId)
                    .Select(group => new { Id = group.Key, Count = group.Count() })
                    .OrderByDescending(x => x.Count)
                    .Select(x => x.Id);
                return person.FirstOrDefault();
            }
        }

        public List<Movie> GetAll(IEnumerable<string> collections, bool inludeSubs = false)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context.Movies.AsNoTracking().AsQueryable();

                if (inludeSubs)
                {
                    query = query
                        .Include(x => x.ExtraPersons)
                        .Include(x => x.MediaGenres)
                        .Include(x => x.VideoStreams);
                }

                if (collections.Any())
                {
                    query = query.Where(x => collections.Any(y => x.CollectionId == y));
                }

                return query.ToList();
            }
        }

        public List<string> GetGenres(List<string> collections)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context.Movies.Include(x => x.MediaGenres).AsQueryable();

                if (collections.Any())
                {
                    query = query.Where(x => collections.Any(y => x.CollectionId == y));
                }

                var genres = query
                    .SelectMany(x => x.MediaGenres)
                    .Select(x => x.GenreId)
                    .Distinct();

                return genres.ToList();
            }
        }

        public bool Any()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Movies.Any();
            }
        }

        public void RemoveMovies()
        {
            using (var context = new ApplicationDbContext())
            {
                context.Movies.RemoveRange(context.Movies.
                    Include(x => x.ExtraPersons)
                    .Include(x => x.AudioStreams)
                    .Include(x => x.MediaGenres)
                    .Include(x => x.MediaSources)
                    .Include(x => x.SubtitleStreams)
                    .Include(x => x.VideoStreams));
                context.SaveChanges();
            }
        }
    }
}
