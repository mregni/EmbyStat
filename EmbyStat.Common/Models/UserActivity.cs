using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EmbyStat.Common.Models
{
    public class UserActivity
    {
        [Key] public Guid Id { get; set; }
        public DateTime LatestDate { get; set; }
        public int TotalCount { get; set; }
        public int TotalTime { get; set; }
        public string ItemTitle { get; set; }
        public string Client { get; set; }
        public string LastSeen { get; set; }
        public string TotalPlayTime { get; set; }
        public string UserId { get; set; }
    }
}