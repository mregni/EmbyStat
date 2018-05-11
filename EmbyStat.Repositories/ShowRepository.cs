using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmbyStat.Common.Models;
using EmbyStat.Repositories.Interfaces;
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

        public void AddRange(List<Show> list)
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

        public void AddRange(List<Season> list)
        {
            using (var context = new ApplicationDbContext())
            {
                context.Seasons.AddRange(list);
                context.SaveChanges();
            }
        }

        public void AddRange(List<Episode> list)
        {
            using (var context = new ApplicationDbContext())
            {
                foreach (var episode in list)
                {
                    try
                    {
                        context.Episodes.Add(episode);
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }
        }
    }
}
