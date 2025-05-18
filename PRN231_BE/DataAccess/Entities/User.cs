using DataAccess.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(50)]
        public string MembershipType { get; set; }

        public DateTime JoinDate { get; set; } = DateTime.Now;

        public bool Status { get; set; } = true;

        [Required]
        public UserRole Role { get; set; } = UserRole.User;

        public ICollection<SmokingLog> SmokingLogs { get; set; }
        public ICollection<QuitPlan> QuitPlans { get; set; }
        public ICollection<ProgressTracking> ProgressTrackings { get; set; }
        public ICollection<Achievement> Achievements { get; set; }
        public ICollection<BlogPost> BlogPosts { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
        public ICollection<TrainerChat> TrainerChats { get; set; }
    }
}
