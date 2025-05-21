using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
    public class Achievement
    {
        [Key]
        public int AchievementId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        //link hình ảnh của huy hiệu đạt được
        [MaxLength(255)]
        public string IconUrl { get; set; }

        public bool Status { get; set; } = true;

        public virtual ICollection<UserAchievement> UserAchievements { get; set; }
    }
}
