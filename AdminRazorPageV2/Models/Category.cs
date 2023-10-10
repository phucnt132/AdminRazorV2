using System;
using System.Collections.Generic;

namespace AdminRazorPageV2.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; }

    public bool IsActive { get; set; }
}
