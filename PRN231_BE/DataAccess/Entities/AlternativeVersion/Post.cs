using System;
using DataAccess.Enums;

namespace DataAccess.Entities.AlternativeVersion;

public partial class Post
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public PostType Type { get; set; }
    public string? ImageUrl { get; set; }
    public int LikeCount { get; set; } = 0;
    public int CommentCount { get; set; } = 0;
    public PostStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual Customer Customer { get; set; } = null!;
    public virtual ICollection<PostComment> Comments { get; set; } = new List<PostComment>();
    public virtual ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
}
