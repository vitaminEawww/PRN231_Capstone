using DataAccess.Enums;

namespace DataAccess.Entities;

public partial class Customer
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public Gender Gender { get; set; }
    public DateTime DateOfBirth { get; set; }

    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }

    //* Cài đặt thông báo cho customer 
    public bool IsNotificationEnabled { get; set; } = true;
    public bool IsDailyReminderEnabled { get; set; } = true;
    public bool IsWeeklyReportEnabled { get; set; } = true;

    //* Ghi Audit log
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public virtual User? User { get; set; }
    public virtual ICollection<SmokingRecord> SmokingRecords { get; set; } = new List<SmokingRecord>();
    public virtual ICollection<QuitPlan> QuitPlans { get; set; } = new List<QuitPlan>();
    public virtual ICollection<DailyProgress> DailyProgresses { get; set; } = new List<DailyProgress>();
    public virtual ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public virtual ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();
    public virtual ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();
    public virtual ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual CustomerStatistics? Statistics { get; set; }
    public virtual ICollection<Leaderboard> LeaderboardEntries { get; set; } = new List<Leaderboard>();
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
}
