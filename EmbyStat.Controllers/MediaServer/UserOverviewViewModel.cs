﻿using System;

namespace EmbyStat.Controllers.MediaServer;

public class UserOverviewViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public DateTimeOffset? LastLoginDate { get; set; }
    public DateTimeOffset? LastActivityDate { get; set; }
    public bool IsAdministrator { get; set; }
    public bool IsHidden { get; set; }
    public bool IsDisabled { get; set; }
    public string PrimaryImageTag { get; set; }
}