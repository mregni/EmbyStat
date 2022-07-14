using System;
using System.Collections.Generic;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;

namespace Tests.Unit.Builders;

public class LibraryBuilder
{
    private readonly Library _library;

    public LibraryBuilder(int index, LibraryType type)
    {
        _library = new Library
        {
            Id = $"id{index}",
            Name = $"collection{index}",
            Primary = $"image{index}",
            Type = type,
            SyncTypes = new List<LibrarySyncType>
            {
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    SyncType = type,
                    LibraryId = $"id{index}",
                }
            }
        };
    }

    public Library Build()
    {
        return _library;
    }
}