using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using MediaBrowser.Model.Extensions;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace EmbyStat.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        public void AddOrUpdate(Movie movie)
        {
            using (var context = new ApplicationDbContext())
            {
                var peopleToDelete = new List<Guid>();
                foreach (var person in movie.ExtraPersons)
                {
                    var temp = context.People.AsNoTracking().SingleOrDefault(x => x.Id == person.PersonId);
                    if (temp == null)
                    {
                        Log.Warning($"{Constants.LogPrefix.MediaSyncJob}\tWe couldn't find the person with Id {person.PersonId} for movie ({movie.Id}) {movie.Name} in our database. This is because Emby didn't return the actor when we queried the people for the parent id. As a fix we will remove the person from the movie now.");
                        peopleToDelete.Add(person.PersonId);
                    }
                }
                peopleToDelete.ForEach(x => movie.ExtraPersons.Remove(movie.ExtraPersons.SingleOrDefault(y => y.PersonId == x)));

                var genresToDelete = new List<Guid>();
                foreach (var genre in movie.MediaGenres)
                {
                    var temp = context.Genres.AsNoTracking().SingleOrDefault(x => x.Id == genre.GenreId);
                    if (temp == null)
                    {
                        Log.Warning($"{Constants.LogPrefix.MediaSyncJob}\tWe couldn't find the genre with Id {genre.GenreId} for movie ({movie.Id}) {movie.Name} in our database. This is because Emby didn't return the genre when we queried the genres for the parent id. As a fix we will remove the genre from the movie now.");
                        genresToDelete.Add(genre.GenreId);
                    }
                }
                genresToDelete.ForEach(x => movie.MediaGenres.Remove(movie.MediaGenres.SingleOrDefault(y => y.GenreId == x)));

                var dbMovie = context.Movies.Include(x => x.Collections).SingleOrDefault(x => x.Id == movie.Id);
                if (dbMovie == null)
                {
                    context.Movies.Add(movie);
                    context.SaveChanges();
                }
                else
                {
                    dbMovie.Collections.ToList().ForEach(x => movie.Collections.Add(x));
                    context.Movies.Remove(dbMovie);
                    context.SaveChanges();

                    context.Movies.AddRange(movie);
                    context.SaveChanges();
                }
            }
        }

        public int GetTotalPersonByType(List<Guid> collections, string type)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context.Movies.Include(x => x.ExtraPersons).AsQueryable();

                if (collections.Any())
                {
                    query = query.Where(x => collections.Any(y => x.Collections.Any(z => z.CollectionId == y)));
                }

                var extraPerson = query.SelectMany(x => x.ExtraPersons).AsEnumerable();
                var people = extraPerson.DistinctBy(x => x.PersonId);
                return people.Count(x => x.Type == type);
            }
        }

        public Guid GetMostFeaturedPerson(List<Guid> collections, string type)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context.Movies.Include(x => x.ExtraPersons).AsQueryable();

                if (collections.Any())
                {
                    query = query.Where(x => collections.Any(y => x.Collections.Any(z => z.CollectionId == y)));
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

        public List<Movie> GetAll(IEnumerable<Guid> collections, bool inludeSubs = false)
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
                    query = query.Where(x => collections.Any(y => x.Collections.Any(z => z.CollectionId == y)));
                }

                return query.ToList();
            }
        }

        public List<Guid> GetGenres(List<Guid> collections)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context.Movies.Include(x => x.MediaGenres).AsQueryable();

                if (collections.Any())
                {
                    query = query.Where(x => collections.Any(y => x.Collections.Any(z => z.CollectionId == y)));
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
