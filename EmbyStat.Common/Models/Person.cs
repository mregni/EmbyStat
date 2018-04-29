using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EmbyStat.Common.Models.Joins;

namespace EmbyStat.Common.Models
{
    public class Person
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Synced { get; set; }
        public int ChildCount { get; set; }
        public int EpisodeCount { get; set; }
        public string Etag { get; set; }
        public string HomePageUrl { get; set; }
        public int MovieCount { get; set; }
        public string OverView { get; set; }
        public DateTime? BirthDate { get; set; }
        public string IMDB { get; set; }
        public string TMDB { get; set; }
        public int SeriesCount { get; set; }
        public string SortName { get; set; }
        public string Primary { get; set; }
        public ICollection<ExtraPerson> ExtraPersons { get; set; }
    }
}