using System;

namespace EmbyStat.Common.Models.Entities
{
    public class User
    {
        public string Id { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string Name { get; set; }
        public Boolean IsAdmin { get; set; }
	    public Boolean IsHidden { get; set; }
	    public Boolean IsDisabled { get; set; }
    }
}
