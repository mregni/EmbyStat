using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EmbyStat.Common.Enums;
using EmbyStat.Common.Models.Entities.Helpers;
using Newtonsoft.Json;

namespace EmbyStat.Common.Models.Entities;

public class FilterValues
{
    [Key]
    public int Id { get; set; }
    public string Field { get; set; }
    public LibraryType Type { get; set; }
        
    /// <summary>
    /// Holds the label value pair values in string format.
    /// Only used for database support. Don't use this property directly. You can use <see cref="Values"/> for that.
    /// </summary>
    public string _Values { get; set; }
        
    [NotMapped]
    public LabelValuePair[] Values {
        get => _Values == null ? null : JsonConvert.DeserializeObject<LabelValuePair[]>(_Values);
        set => _Values = JsonConvert.SerializeObject(value);
    }
}