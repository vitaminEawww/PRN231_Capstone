using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    public class UserAchievement
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int AchievementId { get; set; }

        public DateTime DateEarned { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("AchievementId")]
        public virtual Achievement Achievement { get; set; }
    }
}