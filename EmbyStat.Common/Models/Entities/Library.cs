using System;
using System.Collections.Generic;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Movies;

namespace EmbyStat.Common.Models.Entities;

public class Library
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Primary { get; set; }
    public LibraryType Type { get; set; }
    public ICollection<Movie> Movies { get; set; }
    public ICollection<Shows.Show> Shows { get; set; }
    public ICollection<LibrarySyncType> SyncTypes { get; set; }
}