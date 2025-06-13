namespace DataAccess.Entities;

public partial class PostLike
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public int CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual Post? Post { get; set; }
    public virtual Customer? Customer { get; set; }
}
