using System;
using System.Collections.Generic;
using EmbyStat.Controllers.HelperClasses;

namespace EmbyStat.Controllers.MediaServer
{
    public class UserFullViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ServerId { get; set; }
        public bool IsAdministrator { get; set; }
        public bool Deleted { get; set; }
        public CardViewModel<int> ViewedMovieCount { get; set; }
        public CardViewModel<int> ViewedEpisodeCount { get; set; }
        public IList<UserMediaViewViewModel> LastWatchedMedia { get; set; }
    }
}
