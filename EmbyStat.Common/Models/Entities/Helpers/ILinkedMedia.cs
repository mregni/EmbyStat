using System.Collections.Generic;

namespace EmbyStat.Common.Models.Entities.Helpers
{
    public interface ILinkedMedia
    {
        ICollection<MediaPerson> People { get; set; }
        ICollection<Genre> Genres { get; set; }
        Library Library { get; set; }
    }
}
