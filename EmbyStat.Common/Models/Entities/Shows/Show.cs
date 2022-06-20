using System.Collections.Generic;
using EmbyStat.Common.Models.Entities.Helpers;

namespace EmbyStat.Common.Models.Entities.Shows;

public class Show : Extra, ILinkedMedia
{
    public long? CumulativeRunTimeTicks { get; set; }
    public string Status { get; set; }
    public bool ExternalSynced { get; set; }
    public double SizeInMb { get; set; }
    public ICollection<Season> Seasons { get; set; }

    #region Inherited props

    public ICollection<MediaPerson> People { get; set; }
    public ICollection<Genre> Genres { get; set; }
    public Library Library { get; set; }
    public string LibraryId { get; set; }

    #endregion
}