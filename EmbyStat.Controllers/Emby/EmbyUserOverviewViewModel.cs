using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Controllers.Emby
{
    public class EmbyUserOverviewViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset? LastLoginDate { get; set; }
        public DateTimeOffset? LastActivityDate { get; set; }
        public bool IsAdministrator { get; set; }
        public bool IsHidden { get; set; }
        public bool IsDisabled { get; set; }
        public bool Deleted { get; set; }
        public string PrimaryImageTag { get; set; }
    }
}
