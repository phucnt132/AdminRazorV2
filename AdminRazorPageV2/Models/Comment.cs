using System;
using System.Collections.Generic;

namespace AdminRazorPageV2.Models;

public partial class Comment
{
    public int CommentId { get; set; }

    public int UserId { get; set; }

    public int MovieId { get; set; }

    public string CommentContent { get; set; }

    public DateTime CommentedDate { get; set; }

    public int? Rating { get; set; }

    public bool IsActive { get; set; }

    public virtual Movie Movie { get; set; }

    public virtual User User { get; set; }
}
