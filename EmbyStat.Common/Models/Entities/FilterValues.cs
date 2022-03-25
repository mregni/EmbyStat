using System.ComponentModel.DataAnnotations;

namespace EmbyStat.Common.Models.Entities
{
    public class FilterValues
    {
        [Key]
        public string Id { get; set; }
        public string Field { get; set; }
        public LabelValuePair[] Values { get; set; }
    }
}
