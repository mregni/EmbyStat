using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Services.Models.Movie
{
    public class MovieDuplicate
    {
        public int Number { get; set; }
        public string Title { get; set; }
        public string Reason { get; set; }
        public MovieDuplicateItem ItemOne { get; set; }
        public MovieDuplicateItem ItemTwo { get; set; }
    }

    public class MovieDuplicateItem
    {
        public DateTime? DateCreated { get; set; }
        public string Id { get; set; }
        public string Quality { get; set; }
    }
}
