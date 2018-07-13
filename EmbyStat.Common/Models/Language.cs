using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EmbyStat.Common.Models
{
    public class Language
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
