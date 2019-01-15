using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EmbyStat.Common.Models.Entities.Joins;

namespace EmbyStat.Common.Models.Entities.Helpers
{
    public class Media
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Banner { get; set; }
        public string Logo { get; set; }
        public string Primary { get; set; }
        public string Thumb { get; set; }
        public string Name { get; set; }
        public Guid? ParentId { get; set; }
        public string Path { get; set; }
        public DateTime? PremiereDate { get; set; }
        public int? ProductionYear { get; set; }
        public string SortName { get; set; }
        public ICollection<MediaGenre> MediaGenres { get; set; }
        public ICollection<MediaCollection> Collections { get; set; }

        public Media()
        {
            Collections = new List<MediaCollection>();
        }

    }
}
