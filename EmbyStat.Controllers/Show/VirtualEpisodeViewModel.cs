using System;
using System.Collections.Generic;

namespace EmbyStat.Controllers.Show;

public class VirtualSeasonViewModel
{
    public int IndexNumber { get; set; }
    public IEnumerable<VirtualEpisodeViewModel> Episodes { get; set; }
}

public class VirtualEpisodeViewModel
{
    public string Id { get; set; }
    public int IndexNumber { get; set; }
    public string Name { get; set; }
    public DateTime? PremiereDate { get; set; }
}