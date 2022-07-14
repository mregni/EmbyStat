using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities;

namespace EmbyStat.Common.Extensions;

public static class LibrarySyncTypeExtensions
{
    public static DateTime? GetLastSyncedDateForLibrary(this IEnumerable<LibrarySyncType> items, string libraryId, LibraryType type)
    {
        return items.FirstOrDefault(x => x.LibraryId == libraryId && x.SyncType == type)?.LastSynced;
    }   
}