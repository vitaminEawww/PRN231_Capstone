using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities;

public class Comment
{
    [Key]
    public int Id { get; set; }

    public int PostId { get; set; }

    public int UserId { get; set; }

    [Required]
    public string Content { get; set; }

    public DateTime CreatedDate { get; set; }

    public int LikesCount { get; set; }

    public bool IsDeleted { get; set; }

    // Navigation properties
    [ForeignKey("PostId")]
    public virtual Post Post { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; }
}