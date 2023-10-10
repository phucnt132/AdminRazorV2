using System;
using System.Collections.Generic;

namespace AdminRazorPageV2.Models;

public partial class Episode
{
    public int EpisodeId { get; set; }

    public int MovieId { get; set; }

    public string EpisodeName { get; set; }

    public string Description { get; set; }

    public string MediaContent { get; set; }

    public bool IsActive { get; set; }

    public string MediaLink { get; set; }

    public virtual Movie Movie { get; set; }
}
