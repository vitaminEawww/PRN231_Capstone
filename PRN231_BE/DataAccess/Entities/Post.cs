using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities;

public class Post
{
    [Key]
    public int PostId { get; set; }

    [Required]
    [ForeignKey("User")]
    public int UserId { get; set; }

    public int Rating { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public DateTime CreateDate { get; set; } = DateTime.Now;

    public bool Status { get; set; } = true;

    public User User { get; set; }

    public virtual ICollection<Comment> Comments { get; set; }

}
