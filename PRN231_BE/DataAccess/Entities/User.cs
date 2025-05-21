using DataAccess.Enums;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public DateTime JoinDate { get; set; } = DateTime.Now;

        public bool Status { get; set; } = true;

        [Required]
        public UserRole Role { get; set; } = UserRole.User;

        public virtual ICollection<UserMembership> UserMemberships { get; set; }
        public virtual ICollection<UserPlan> UserPlans { get; set; }
        public virtual ICollection<UserAchievement> UserAchievements { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<TrainerChat> TrainerChats { get; set; }
    }
}
