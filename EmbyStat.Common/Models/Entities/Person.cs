using System;
using System.Collections.Generic;
using EmbyStat.Common.SqLite.Helpers;
using LiteDB;

namespace EmbyStat.Common.Models.Entities
{
    public class Person
    {
        [BsonId]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Etag { get; set; }
        public string HomePageUrl { get; set; }
        public int? MovieCount { get; set; }
        public string OverView { get; set; }
        public DateTime? BirthDate { get; set; }
        public string IMDB { get; set; }
        public string TMDB { get; set; }
        public int? ShowCount { get; set; }
        public string SortName { get; set; }
        public string Primary { get; set; }
        public ICollection<SqlMediaPerson> MoviePeople { get; set; }
    }
}