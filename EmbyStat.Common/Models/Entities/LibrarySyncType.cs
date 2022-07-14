    using System;
    using EmbyStat.Common.Enums;

    namespace EmbyStat.Common.Models.Entities;

public class LibrarySyncType
{
    public string Id { get; set; }
    public string LibraryId { get; set; }
    public Library Library { get; set; }
    public LibraryType SyncType { get; set; }
    public DateTime? LastSynced { get; set; }
}