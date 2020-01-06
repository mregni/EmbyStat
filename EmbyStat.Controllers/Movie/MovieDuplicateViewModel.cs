using System;

namespace EmbyStat.Controllers.Movie
{
    public class MovieDuplicateViewModel
    {
        public int Number { get; set; }
        public MovieDuplicateItemViewModel ItemOne { get; set; }
        public MovieDuplicateItemViewModel ItemTwo { get; set; }
        public string Title { get; set; }
        public string Reason { get; set; }
    }

    public class MovieDuplicateItemViewModel
    {
        public DateTimeOffset? DateCreated { get; set; }
        public string Id { get; set; }
        public string Quality { get; set; }
    }
}
