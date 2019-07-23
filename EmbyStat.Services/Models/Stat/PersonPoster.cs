using System;

namespace EmbyStat.Services.Models.Stat
{
    public class PersonPoster
    {
        public string MediaId { get; set; }
        public string Name { get; set; }
        public DateTimeOffset? BirthDate { get; set; }
        public int? MovieCount { get; set; }
        public int? ShowCount { get; set; }
        public string Title { get; set; }
        public string Tag { get; set; }

        public PersonPoster(string title)
        {
            Title = title;
        }
    }
}
