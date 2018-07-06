using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EmbyStat.Common.Models.Helpers
{
    public class CollectionId
    {
        [Key]
        public string Id { get; set; }
        public string CId { get; set; }
    }
}
