using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EmbyStat.Common.Models.Entities.Joins;

namespace EmbyStat.Common.Models.Entities
{
    public class Person
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Synced { get; set; }
        public int seriesCount { get; set; }
        public string Etag { get; set; }
        public string HomePageUrl { get; set; }
        public int MovieCount { get; set; }
        public string OverView { get; set; }
        public DateTimeOffset? BirthDate { get; set; }
        public string IMDB { get; set; }
        public string TMDB { get; set; }
        public int SeriesCount { get; set; }
        public string SortName { get; set; }
        public string Primary { get; set; }
        public ICollection<ExtraPerson> ExtraPersons { get; set; }
    }
}