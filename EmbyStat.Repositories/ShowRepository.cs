using System;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Models.Entities;
using EmbyStat.Repositories.Interfaces;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Extensions;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace EmbyStat.Repositories
{
    public class ShowRepository : IShowRepository
    {
        public void RemoveShows()
        {
            using (var context = new ApplicationDbContext())
            {
                context.Episodes.RemoveRange(context.Episodes.Include(x => x.SeasonEpisodes));
                context.Seasons.RemoveRange(context.Seasons);
                context.Shows.RemoveRange(context.Shows.Include(x => x.MediaGenres).Include(x => x.ExtraPersons));
                context.SaveChanges();
            }
        }

        public void AddRange(IEnumerable<Show> list)
        {
            using (var context = new ApplicationDbContext())
            {
                foreach (var show in list)
                {
                    var peopleToDelete = new List<string>();
                    foreach (var person in show.ExtraPersons)
                    {
                        var temp = context.People.AsNoTracking().SingleOrDefault(x => x.Id == person.PersonId);
                        if (temp == null)
                        {
                            Log.Warning($"We couldn't find the person with Id {person.PersonId} for show ({show.Id}) {show.Name} in the database. This is because Emby didn't return the actor when we queried the people for the parent id. As a fix we will remove the person from the show now.");
                            peopleToDelete.Add(person.PersonId);
                        }
                    }
                    peopleToDelete.ForEach(x => show.ExtraPersons.Remove(show.ExtraPersons.SingleOrDefault(y => y.PersonId == x)));

                    var genresToDelete = new List<string>();
                    foreach (var genre in show.MediaGenres)
                    {
                        var temp = context.Genres.AsNoTracking().SingleOrDefault(x => x.Id == genre.GenreId);
                        if (temp == null)
                        {
                            Log.Warning($"We couldn't find the genre with Id {genre.GenreId} for show ({show.Id}) {show.Name} in the database. This is because Emby didn't return the genre when we queried the genres for the parent id. As a fix we will remove the genre from the show now.");
                            genresToDelete.Add(genre.GenreId);
                        }
                    }
                    genresToDelete.ForEach(x => show.MediaGenres.Remove(show.MediaGenres.SingleOrDefault(y => y.GenreId == x)));

                    context.Shows.Add(show);
                    context.SaveChanges();
                }
            }
        }

        public void UpdateShow(Show show)
        {
            using (var context = new ApplicationDbContext())
            {
                var data = context.Shows.AsNoTracking().SingleOrDefault(x => x.Id == show.Id);
                if (data != null)
                {
                    context.Entry(show).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
        }

        public void AddRange(IEnumerable<Season> list)
        {
            using (var context = new ApplicationDbContext())
            {
                context.Seasons.AddRange(list);
                context.SaveChanges();
            }
        }

        public void AddRange(IEnumerable<Episode> list)
        {
            using (var context = new ApplicationDbContext())
            {
                context.Episodes.AddRange(list);
                context.SaveChanges();
            }
        }

        public IEnumerable<Show> GetAllShows(IEnumerable<string> collections, bool includeSubs = false)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context.Shows.AsNoTracking().AsQueryable();

                if (includeSubs)
                {
                    query = query
                        .Include(x => x.ExtraPersons)
                        .Include(x => x.MediaGenres);
                }

                if (collections.Any())
                {
                    query = query.Where(x => collections.Any(y => x.Collections.Any(z => z.CollectionId == y)));
                }

                return query.ToList();
            }
        }

        public IEnumerable<Season> GetAllSeasonsForShow(string showId, bool includeSubs = false)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context.Seasons.AsQueryable();

                if (includeSubs)
                {
                    query = query
                        .Include(x => x.SeasonEpisodes)
                        .Include(x => x.MediaGenres);
                }

                query = query.Where(x => x.ParentId == showId);
                return query.ToList();
            }
        }

        public IEnumerable<Episode> GetAllEpisodesForShow(string showId, bool includeSubs = false)
        {
            using (var context = new ApplicationDbContext())
            {
                var episodeIds = context.Seasons
                    .Where(x => x.ParentId == showId)
                    .Include(x => x.SeasonEpisodes)
                    .SelectMany(x => x.SeasonEpisodes)
                    .Select(x => x.EpisodeId);

                var query = context.Episodes.AsQueryable();

                if (includeSubs)
                {
                    query = query
                        .Include(x => x.SeasonEpisodes)
                        .Include(x => x.AudioStreams)
                        .Include(x => x.ExtraPersons)
                        .Include(x => x.MediaSources)
                        .Include(x => x.SubtitleStreams)
                        .Include(x => x.VideoStreams)
                        .Include(x => x.MediaGenres);
                }

                query = query.Where(x => episodeIds.Any(y => y == x.Id));
                return query.ToList();
            }
        }

        public IEnumerable<Episode> GetAllEpisodesForShows(IEnumerable<string> showIds, bool includeSubs = false)
        {
            using (var context = new ApplicationDbContext())
            {
                var episodeIds = context.Seasons
                    .Where(x => showIds.Any(y => y == x.ParentId))
                    .Include(x => x.SeasonEpisodes)
                    .SelectMany(x => x.SeasonEpisodes)
                    .Select(x => x.EpisodeId);

                var query = context.Episodes.AsQueryable();

                if (includeSubs)
                {
                    query = query
                        .Include(x => x.ExtraPersons);
                }

                query = query.Where(x => episodeIds.Any(y => y == x.Id));
                return query.ToList();
            }
        }

        public void SetTvdbSynced(string showId)
        {
            using (var context = new ApplicationDbContext())
            {
                var show = context.Shows.Single(x => x.Id == showId);
                show.TvdbSynced = true;

                context.SaveChanges();
            }
        }

        public int CountShows(IEnumerable<string> collectionIds)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context.Shows.AsQueryable();

                if (collectionIds.Any())
                {
                    query = query.Where(x => collectionIds.Any(y => x.Collections.Any(z => z.CollectionId == y)));
                }

                return query.Count();
            }
        }

        public int CountEpisodes(IEnumerable<string> collectionIds)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context.Episodes.AsQueryable();

                if (collectionIds.Any())
                {
                    query = query.Where(x => collectionIds.Any(y => x.Collections.Any(z => z.CollectionId == y)));

                }

                return query.Count();
            }
        }

        public int CountEpisodes(string showId)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Seasons
                    .Where(x => x.ParentId == showId)
                    .Include(x => x.SeasonEpisodes)
                    .SelectMany(x => x.SeasonEpisodes)
                    .Count();
            }
        }

        public long GetPlayLength(IEnumerable<string> collectionIds)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context.Episodes.AsQueryable();

                if (collectionIds.Any())
                {
                    query = query.Where(x => collectionIds.Any(y => x.Collections.Any(z => z.CollectionId == y)));
                }

                return query.Sum(x => x.RunTimeTicks ?? 0);
            }
        }

        public int GetTotalPersonByType(IEnumerable<string> collections, PersonType type)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context.Shows.Include(x => x.ExtraPersons).AsQueryable();

                if (collections.Any())
                {
                    query = query.Where(x => collections.Any(y => x.Collections.Any(z => z.CollectionId == y)));
                }

                var extraPerson = query.SelectMany(x => x.ExtraPersons).AsEnumerable();
                var people = extraPerson.DistinctBy(x => x.PersonId);
                return people.Count(x => x.Type == type);
            }
        }

        public string GetMostFeaturedPerson(IEnumerable<string> collections, PersonType type)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context.Shows.Include(x => x.ExtraPersons).AsQueryable();

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

        public List<string> GetGenres(IEnumerable<string> collections)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context.Shows.Include(x => x.MediaGenres).AsQueryable();

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

        public int GetEpisodeCountForShow(string showId, bool includeSpecials = false)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context.Seasons.AsQueryable();
                query = query.Where(x => x.ParentId == showId);

                if (!includeSpecials)
                {
                    query = query.Where(x => x.IndexNumber != 0);
                }

                var count = query.Include(x => x.SeasonEpisodes)
                     .SelectMany(x => x.SeasonEpisodes)
                     .Count();

                return count;
            }
        }

        public int GetSeasonCountForShow(string showId, bool includeSpecials = false)
        {
            using (var context = new ApplicationDbContext())
            {
                var query = context.Seasons.AsQueryable();
                query = query.Where(x => x.ParentId == showId);

                if (!includeSpecials)
                {
                    query = query.Where(x => x.IndexNumber != 0);
                }
                   
                return query.Count();
            }
        }

        public bool Any()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Shows.Any();
            }
        }
    }
}
