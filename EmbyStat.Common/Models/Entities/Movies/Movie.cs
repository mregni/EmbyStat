using System;
using System.Collections.Generic;
using EmbyStat.Common.Models.Entities.Helpers;

namespace EmbyStat.Common.Models.Entities.Movies;

public class Movie : Video, ILinkedMedia
{
    public string OriginalTitle { get; set; }

    public override bool Equals(object? other)
    {
        if (other is Movie media)
        {
            return Id == media.Id;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }

    #region Inherited props
    public ICollection<MediaPerson> People { get; set; }
    public ICollection<Genre> Genres { get; set; }
    public Library Library { get; set; }
    #endregion
}