using System.Collections.Generic;
using EmbyStat.Common.Models.Entities.Helpers;

namespace EmbyStat.Common.Models.Entities;

public class Person
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Primary { get; set; }
    public ICollection<MediaPerson> MediaPeople { get; set; }
}