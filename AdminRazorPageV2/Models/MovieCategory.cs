using System;
using System.Collections.Generic;

namespace AdminRazorPageV2.Models;

public partial class MovieCategory
{
    public int MovieId { get; set; }

    public int CategoryId { get; set; }

    public virtual Category Category { get; set; }

    public virtual Movie Movie { get; set; }
}
