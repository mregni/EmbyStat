using System;
using System.Collections.Generic;
using System.Text;
using EmbyStat.Services.Models.Movie;

namespace EmbyStat.Controllers.ViewModels.Movie
{
    public class MovieDuplicateViewModel
    {
        public int Number { get; set; }
        public MovieDuplicateItemViewModel ItemOne { get; set; }
        public MovieDuplicateItemViewModel ItemTwo { get; set; }
    }

    public class MovieDuplicateItemViewModel
    {
        public string Title { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Id { get; set; }
        public string Quality { get; set; }
    }
}
