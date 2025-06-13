namespace DataAccess.Entities;

public partial class PostComment
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public int CustomerId { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public virtual Post? Post { get; set; }
    public virtual Customer? Customer { get; set; }
}
