using System;
using System.Collections.Generic;

namespace AdminRazorPageV2.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
