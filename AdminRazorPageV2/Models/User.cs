using System;
using System.Collections.Generic;

namespace AdminRazorPageV2.Models;

public partial class User
{
    public int UserId { get; set; }

    public int RoleId { get; set; }

    public string FullName { get; set; }

    public string Address { get; set; }

    public string PhoneNumber { get; set; }

    public string Email { get; set; }

    public DateTime RegistedDate { get; set; }

    public string Avatar { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();

    public virtual Role Role { get; set; }
}
