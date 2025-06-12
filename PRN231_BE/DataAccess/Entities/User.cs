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

        // Personal Information
        public DateTime? DateOfBirth { get; set; }
        [MaxLength(10)]
        public string Gender { get; set; } // "Male", "Female", "Other"
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        // Smoking History
        public DateTime? SmokingStartDate { get; set; }
        public int? CigarettesPerDay { get; set; }
        public int? SmokingYears { get; set; }
        public DateTime? QuitDate { get; set; }
        [MaxLength(1000)]
        public string ReasonToQuit { get; set; }
        public decimal? MoneySpentPerDay { get; set; }

        // App Settings
        public bool ReminderEnabled { get; set; } = true;
        public TimeSpan? ReminderTime { get; set; }
        public bool NotificationEnabled { get; set; } = true;
        [MaxLength(10)]
        public string Language { get; set; } = "vi-VN";
        [MaxLength(50)]
        public string TimeZone { get; set; } = "SE Asia Standard Time";

        // Navigation properties
        public virtual ICollection<UserMembership> UserMemberships { get; set; }
        public virtual ICollection<UserPlan> UserPlans { get; set; }
        public virtual ICollection<UserAchievement> UserAchievements { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<TrainerChat> TrainerChats { get; set; }
        public virtual ICollection<DailyLog> DailyLogs { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
