﻿using System;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Users;

namespace EmbyStat.Common.Models.Entities;

public class MediaServerUserView
{
    public string UserId { get; set; }
    public MediaServerUser User { get; set; }
    public MediaType MediaType { get; set; }
    public string MediaId { get; set; }
    public int PlayCount { get; set; }
    public DateTime LastPlayedDate { get; set; }
}