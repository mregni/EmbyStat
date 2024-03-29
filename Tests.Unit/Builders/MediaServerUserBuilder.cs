﻿using System;
using MediaServerUser = EmbyStat.Common.Models.Entities.Users.MediaServerUser;

namespace Tests.Unit.Builders;

public class MediaServerUserBuilder
{
    private readonly MediaServerUser _user;

    public MediaServerUserBuilder(string id)
    {
        _user = new MediaServerUser
        {
            Id = id,
            Name = "userName",
            LastActivityDate = DateTime.Today.AddDays(-2)
        };
    }

    public MediaServerUserBuilder AddLastActivityDate(DateTime date)
    {
        _user.LastActivityDate = date;
        return this;
    }

    public MediaServerUser Build()
    {
        return _user;
    }
}