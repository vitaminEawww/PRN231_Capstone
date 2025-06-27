using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
  #region DbSets
  public DbSet<User> Users { get; set; }
  public DbSet<Customer> Customers { get; set; }
  public DbSet<Coach> Coaches { get; set; }
  public DbSet<SmokingRecord> SmokingRecords { get; set; }
  public DbSet<QuitPlan> QuitPlans { get; set; }
  public DbSet<QuitPlanPhase> QuitPlanPhases { get; set; }
  public DbSet<DailyProgress> DailyProgresses { get; set; }
  public DbSet<Badge> Badges { get; set; }
  public DbSet<UserBadge> UserBadges { get; set; }
  public DbSet<Post> Posts { get; set; }
  public DbSet<PostComment> PostComments { get; set; }
  public DbSet<PostLike> PostLikes { get; set; }
  public DbSet<Payment> Payments { get; set; }
  public DbSet<MembershipPackage> MembershipPackages { get; set; }
  public DbSet<MemberShipUsage> MemberShipUsages { get; set; }
  public DbSet<Consultation> Consultations { get; set; }
  public DbSet<Rating> Ratings { get; set; }
  public DbSet<RatingSummary> RatingSummaries { get; set; }
  public DbSet<Notification> Notifications { get; set; }
  public DbSet<Message> Messages { get; set; }
  public DbSet<Conversation> Conversations { get; set; }
  public DbSet<Leaderboard> Leaderboards { get; set; }
  public DbSet<CustomerStatistics> CustomerStatistics { get; set; }
  public DbSet<SystemReport> SystemReports { get; set; }
  #endregion

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // User Entity Configuration
    modelBuilder.Entity<User>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.UserName).IsRequired().HasMaxLength(100);
      entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
      entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
      entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
      entity.Property(e => e.Role).IsRequired();
      entity.Property(e => e.Status).IsRequired();
      entity.Property(e => e.JoinDate).IsRequired();

      // Unique constraints
      entity.HasIndex(e => e.Email).IsUnique();
      entity.HasIndex(e => e.UserName).IsUnique();
    });

    // Customer Entity Configuration
    modelBuilder.Entity<Customer>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
      entity.Property(e => e.Gender).IsRequired();
      entity.Property(e => e.DateOfBirth).IsRequired();
      entity.Property(e => e.AvatarUrl).HasMaxLength(500);
      entity.Property(e => e.Bio).HasMaxLength(1000);
      entity.Property(e => e.IsNotificationEnabled).HasDefaultValue(true);
      entity.Property(e => e.IsDailyReminderEnabled).HasDefaultValue(true);
      entity.Property(e => e.IsWeeklyReportEnabled).HasDefaultValue(true);
      entity.Property(e => e.CreatedAt).IsRequired();

      // One-to-One relationship with User
      entity.HasOne(c => c.User)
                .WithOne(u => u.Customer)
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

      // One-to-One relationship with CustomerStatistics
      entity.HasOne(c => c.Statistics)
                .WithOne(cs => cs.Customer)
                .HasForeignKey<CustomerStatistics>(cs => cs.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
    });

    // Coach Entity Configuration
    modelBuilder.Entity<Coach>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
      entity.Property(e => e.AvatarUrl).HasMaxLength(500);
      entity.Property(e => e.Bio).IsRequired().HasMaxLength(2000);
      entity.Property(e => e.Specialization).IsRequired().HasMaxLength(500);
      entity.Property(e => e.Qualifications).IsRequired().HasMaxLength(1000);
      entity.Property(e => e.HourlyRate).HasColumnType("decimal(18,2)");
      entity.Property(e => e.IsAvailable).HasDefaultValue(true);
      entity.Property(e => e.Rating).HasDefaultValue(0f);
      entity.Property(e => e.TotalConsultations).HasDefaultValue(0);
      entity.Property(e => e.CreatedAt).IsRequired();

      // One-to-One relationship with User
      entity.HasOne(c => c.User)
                .WithOne(u => u.Coach)
                .HasForeignKey<Coach>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

      // Indexes for performance
      entity.HasIndex(e => e.IsAvailable);
      entity.HasIndex(e => e.Rating);
    });

    // SmokingRecord Entity Configuration
    modelBuilder.Entity<SmokingRecord>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.CigarettesPerDay).IsRequired();
      entity.Property(e => e.CostPerPack).HasColumnType("decimal(18,2)");
      entity.Property(e => e.CigarettesPerPack).HasDefaultValue(20);
      entity.Property(e => e.Frequency).IsRequired();
      entity.Property(e => e.Brands).HasMaxLength(200);
      entity.Property(e => e.Triggers).HasMaxLength(500);
      entity.Property(e => e.RecordDate).IsRequired();
      entity.Property(e => e.CreatedAt).IsRequired();

      // Foreign key relationship
      entity.HasOne(sr => sr.Customer)
                .WithOne(c => c.SmokingRecord)
                .HasForeignKey<SmokingRecord>(sr => sr.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

      // Indexes
      entity.HasIndex(e => e.RecordDate);
      entity.HasIndex(e => new { e.CustomerId, e.RecordDate });
    });

    // QuitPlan Entity Configuration
    modelBuilder.Entity<QuitPlan>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
      entity.Property(e => e.Reasons).IsRequired().HasMaxLength(1000);
      entity.Property(e => e.StartDate).IsRequired();
      entity.Property(e => e.TargetDate).IsRequired();
      entity.Property(e => e.Status).IsRequired();
      entity.Property(e => e.IsSystemGenerated).HasDefaultValue(false);
      entity.Property(e => e.Notes).HasMaxLength(2000);
      entity.Property(e => e.CreatedAt).IsRequired();

      // Foreign key relationship
      entity.HasOne(qp => qp.Customer)
                .WithMany(c => c.QuitPlans)
                .HasForeignKey(qp => qp.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

      // Indexes
      entity.HasIndex(e => e.Status);
      entity.HasIndex(e => e.StartDate);
    });

    // QuitPlanPhase Entity Configuration
    modelBuilder.Entity<QuitPlanPhase>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.PhaseNumber).IsRequired();
      entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
      entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
      entity.Property(e => e.StartDate).IsRequired();
      entity.Property(e => e.EndDate).IsRequired();
      entity.Property(e => e.TargetCigarettesPerDay).IsRequired();
      entity.Property(e => e.IsCompleted).HasDefaultValue(false);

      // Foreign key relationship
      entity.HasOne(qpp => qpp.QuitPlan)
                .WithMany(qp => qp.Phases)
                .HasForeignKey(qpp => qpp.QuitPlanId)
                .OnDelete(DeleteBehavior.Cascade);

      // Unique constraint - each phase number should be unique within a quit plan
      entity.HasIndex(e => new { e.QuitPlanId, e.PhaseNumber }).IsUnique();
    });

    // DailyProgress Entity Configuration
    modelBuilder.Entity<DailyProgress>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Date).IsRequired();
      entity.Property(e => e.CigarettesSmoked).HasDefaultValue(0);
      entity.Property(e => e.MoneySpent).HasColumnType("decimal(18,2)").HasDefaultValue(0);
      entity.Property(e => e.CravingLevel).HasDefaultValue(0);
      entity.Property(e => e.MoodLevel).HasDefaultValue(0);
      entity.Property(e => e.EnergyLevel).HasDefaultValue(0);
      entity.Property(e => e.Notes).HasMaxLength(1000);
      entity.Property(e => e.Triggers).HasMaxLength(500);
      entity.Property(e => e.IsSmokeFree).HasDefaultValue(false);
      entity.Property(e => e.CreatedAt).IsRequired();

      // Foreign key relationship
      entity.HasOne(dp => dp.Customer)
                .WithMany(c => c.DailyProgresses)
                .HasForeignKey(dp => dp.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

      // Unique constraint - one record per customer per date
      entity.HasIndex(e => new { e.CustomerId, e.Date }).IsUnique();

      // Check constraints for level ranges
      entity.HasCheckConstraint("CK_DailyProgress_CravingLevel", "CravingLevel >= 0 AND CravingLevel <= 10");
      entity.HasCheckConstraint("CK_DailyProgress_MoodLevel", "MoodLevel >= 0 AND MoodLevel <= 10");
      entity.HasCheckConstraint("CK_DailyProgress_EnergyLevel", "EnergyLevel >= 0 AND EnergyLevel <= 10");
    });

    // Badge Entity Configuration
    modelBuilder.Entity<Badge>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
      entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
      entity.Property(e => e.IconUrl).IsRequired().HasMaxLength(500);
      entity.Property(e => e.Type).IsRequired();
      entity.Property(e => e.Criteria).IsRequired().HasMaxLength(1000);
      entity.Property(e => e.Points).HasDefaultValue(0);
      entity.Property(e => e.IsActive).HasDefaultValue(true);
      entity.Property(e => e.CreatedAt).IsRequired();

      // Indexes for performance
      entity.HasIndex(e => e.Type);
      entity.HasIndex(e => e.IsActive);
      entity.HasIndex(e => e.Name).IsUnique();
    });

    // UserBadge Entity Configuration
    modelBuilder.Entity<UserBadge>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.EarnedAt).IsRequired();
      entity.Property(e => e.IsShared).HasDefaultValue(false);

      // Foreign key relationships
      entity.HasOne(ub => ub.Customer)
                .WithMany(c => c.UserBadges)
                .HasForeignKey(ub => ub.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(ub => ub.Badge)
                .WithMany(b => b.UserBadges)
                .HasForeignKey(ub => ub.BadgeId)
                .OnDelete(DeleteBehavior.Cascade);

      // Unique constraint - customer chỉ có thể có 1 badge duy nhất
      entity.HasIndex(e => new { e.CustomerId, e.BadgeId }).IsUnique();
    });

    // Post Entity Configuration
    modelBuilder.Entity<Post>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
      entity.Property(e => e.Content).IsRequired().HasMaxLength(5000);
      entity.Property(e => e.Type).IsRequired();
      entity.Property(e => e.ImageUrl).HasMaxLength(500);
      entity.Property(e => e.LikeCount).HasDefaultValue(0);
      entity.Property(e => e.CommentCount).HasDefaultValue(0);
      entity.Property(e => e.Status).IsRequired();
      entity.Property(e => e.CreatedAt).IsRequired();

      // Foreign key relationship
      entity.HasOne(p => p.Customer)
                .WithMany(c => c.Posts)
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

      // Indexes for performance
      entity.HasIndex(e => e.Status);
      entity.HasIndex(e => e.Type);
      entity.HasIndex(e => e.CreatedAt);
    });

    // PostComment Entity Configuration
    modelBuilder.Entity<PostComment>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Content).IsRequired().HasMaxLength(1000);
      entity.Property(e => e.CreatedAt).IsRequired();

      // Foreign key relationships
      entity.HasOne(pc => pc.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(pc => pc.PostId)
                .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(pc => pc.Customer)
                .WithMany(c => c.PostComments)
                .HasForeignKey(pc => pc.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

      // Indexes
      entity.HasIndex(e => e.CreatedAt);
    });

    // PostLike Entity Configuration
    modelBuilder.Entity<PostLike>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.CreatedAt).IsRequired();

      // Foreign key relationships
      entity.HasOne(pl => pl.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(pl => pl.PostId)
                .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(pl => pl.Customer)
                .WithMany(c => c.PostLikes)
                .HasForeignKey(pl => pl.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

      // Unique constraint - customer chỉ có thể like 1 post 1 lần
      entity.HasIndex(e => new { e.PostId, e.CustomerId }).IsUnique();
    });

    // MembershipPackage Entity Configuration
    modelBuilder.Entity<MembershipPackage>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
      entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
      entity.Property(e => e.Price).HasColumnType("decimal(18,2)").IsRequired();
      entity.Property(e => e.DurationInDays).IsRequired();
      entity.Property(e => e.IsActive).HasDefaultValue(true);
      entity.Property(e => e.CreatedAt).IsRequired();

      // Indexes
      entity.HasIndex(e => e.IsActive);
      entity.HasIndex(e => e.Name);
    });

    // MemberShipUsage Entity Configuration
    modelBuilder.Entity<MemberShipUsage>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.StartDate).IsRequired();
      entity.Property(e => e.EndDate).IsRequired();
      entity.Property(e => e.Status).IsRequired();

      // Foreign key relationships
      entity.HasOne(msu => msu.Customer)
                .WithMany()
                .HasForeignKey(msu => msu.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(msu => msu.MembershipPackage)
                .WithMany(mp => mp.MemberShipUsages)
                .HasForeignKey(msu => msu.MembershipPackageId)
                .OnDelete(DeleteBehavior.Restrict);

      // Indexes
      entity.HasIndex(e => e.Status);
      entity.HasIndex(e => new { e.CustomerId, e.Status });
    });

    // Payment Entity Configuration
    modelBuilder.Entity<Payment>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Amount).HasColumnType("decimal(18,2)").IsRequired();
      entity.Property(e => e.Type).IsRequired();
      entity.Property(e => e.Method).IsRequired();
      entity.Property(e => e.Status).IsRequired();
      entity.Property(e => e.TransactionId).HasMaxLength(100);
      entity.Property(e => e.Description).HasMaxLength(500);
      entity.Property(e => e.PaymentDate).IsRequired();
      entity.Property(e => e.CreatedAt).IsRequired();

      // Foreign key relationships
      entity.HasOne(p => p.Customer)
                .WithMany(c => c.Payments)
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

      entity.HasOne(p => p.MembershipPackage)
                .WithMany(mp => mp.Payments)
                .HasForeignKey(p => p.MembershipPackageId)
                .OnDelete(DeleteBehavior.SetNull);

      entity.HasOne(p => p.Consultation)
                .WithMany()
                .HasForeignKey(p => p.ConsultationId)
                .OnDelete(DeleteBehavior.SetNull);

      // Indexes
      entity.HasIndex(e => e.Status);
      entity.HasIndex(e => e.PaymentDate);
      entity.HasIndex(e => e.TransactionId);
    });

    // Consultation Entity Configuration
    modelBuilder.Entity<Consultation>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
      entity.Property(e => e.Description).HasMaxLength(1000);
      entity.Property(e => e.ScheduledAt).IsRequired();
      entity.Property(e => e.DurationMinutes).HasDefaultValue(60);
      entity.Property(e => e.Status).IsRequired();
      entity.Property(e => e.Type).IsRequired();
      entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
      entity.Property(e => e.Notes).HasMaxLength(2000);
      entity.Property(e => e.Feedback).HasMaxLength(1000);
      entity.Property(e => e.CreatedAt).IsRequired();

      // Foreign key relationships
      entity.HasOne(c => c.Customer)
                .WithMany(cu => cu.Consultations)
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

      entity.HasOne(c => c.Coach)
                .WithMany(co => co.Consultations)
                .HasForeignKey(c => c.CoachId)
                .OnDelete(DeleteBehavior.Restrict);

      // Indexes
      entity.HasIndex(e => e.Status);
      entity.HasIndex(e => e.ScheduledAt);

      // Check constraint for rating
      entity.HasCheckConstraint("CK_Consultation_Rating", "Rating IS NULL OR (Rating >= 1 AND Rating <= 5)");
    });

    // Rating Entity Configuration
    modelBuilder.Entity<Rating>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.TargetType).IsRequired();
      entity.Property(e => e.TargetId).IsRequired();
      entity.Property(e => e.Score).IsRequired();
      entity.Property(e => e.Comment).HasMaxLength(1000);
      entity.Property(e => e.IsAnonymous).HasDefaultValue(false);
      entity.Property(e => e.Status).IsRequired();
      entity.Property(e => e.CreatedAt).IsRequired();

      // Foreign key relationship
      entity.HasOne(r => r.Customer)
                .WithMany(c => c.Ratings)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

      // Indexes
      entity.HasIndex(e => new { e.TargetType, e.TargetId });
      entity.HasIndex(e => e.Status);

      // Check constraint for score
      entity.HasCheckConstraint("CK_Rating_Score", "Score >= 1 AND Score <= 5");
    });

    // RatingSummary Entity Configuration
    modelBuilder.Entity<RatingSummary>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.TargetType).IsRequired();
      entity.Property(e => e.TargetId).IsRequired();
      entity.Property(e => e.AverageScore).HasColumnType("decimal(3,2)");
      entity.Property(e => e.TotalRatings).HasDefaultValue(0);
      entity.Property(e => e.FiveStarCount).HasDefaultValue(0);
      entity.Property(e => e.FourStarCount).HasDefaultValue(0);
      entity.Property(e => e.ThreeStarCount).HasDefaultValue(0);
      entity.Property(e => e.TwoStarCount).HasDefaultValue(0);
      entity.Property(e => e.OneStarCount).HasDefaultValue(0);
      entity.Property(e => e.LastUpdated).IsRequired();

      // Indexes
      entity.HasIndex(e => new { e.TargetType, e.TargetId }).IsUnique();

      // Ignore calculated properties
      entity.Ignore(rs => rs.FiveStarPercentage);
      entity.Ignore(rs => rs.FourStarPercentage);
      entity.Ignore(rs => rs.ThreeStarPercentage);
      entity.Ignore(rs => rs.TwoStarPercentage);
      entity.Ignore(rs => rs.OneStarPercentage);
    });

    // Notification Entity Configuration
    modelBuilder.Entity<Notification>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
      entity.Property(e => e.Message).IsRequired().HasMaxLength(1000);
      entity.Property(e => e.Type).IsRequired();
      entity.Property(e => e.IsRead).HasDefaultValue(false);
      entity.Property(e => e.ScheduledAt).IsRequired();
      entity.Property(e => e.CreatedAt).IsRequired();

      // Foreign key relationship
      entity.HasOne(n => n.Customer)
                .WithMany(c => c.Notifications)
                .HasForeignKey(n => n.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

      // Indexes
      entity.HasIndex(e => e.IsRead);
      entity.HasIndex(e => e.ScheduledAt);
      entity.HasIndex(e => e.CreatedAt);
    });

    // Conversation Entity Configuration
    modelBuilder.Entity<Conversation>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.CreatedAt).IsRequired();
      entity.Property(e => e.Status).IsRequired();

      // Foreign key relationships
      entity.HasOne(c => c.Customer)
                .WithMany(c => c.Conversations)
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

      entity.HasOne(c => c.Coach)
                .WithMany(co => co.Conversations)
                .HasForeignKey(c => c.CoachId)
                .OnDelete(DeleteBehavior.Restrict);

      // Indexes
      entity.HasIndex(e => e.Status);
      entity.HasIndex(e => new { e.CustomerId, e.CoachId });
    });

    // Message Entity Configuration
    modelBuilder.Entity<Message>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Content).IsRequired().HasMaxLength(2000);
      entity.Property(e => e.SenderType).IsRequired();
      entity.Property(e => e.Type).IsRequired();
      entity.Property(e => e.IsRead).HasDefaultValue(false);
      entity.Property(e => e.SentAt).IsRequired();

      // Foreign key relationship
      entity.HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

      // Indexes
      entity.HasIndex(e => e.SentAt);
      entity.HasIndex(e => e.IsRead);
    });

    // Leaderboard Entity Configuration
    modelBuilder.Entity<Leaderboard>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Type).IsRequired();
      entity.Property(e => e.Period).IsRequired();
      entity.Property(e => e.Score).IsRequired();
      entity.Property(e => e.Rank).IsRequired();
      entity.Property(e => e.PeriodStart).IsRequired();
      entity.Property(e => e.PeriodEnd).IsRequired();
      entity.Property(e => e.LastUpdated).IsRequired();
      entity.Property(e => e.IsActive).HasDefaultValue(true);

      // Foreign key relationship
      entity.HasOne(l => l.Customer)
                .WithMany(c => c.LeaderboardEntries)
                .HasForeignKey(l => l.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

      // Indexes
      entity.HasIndex(e => new { e.Type, e.Period, e.Rank });
      entity.HasIndex(e => e.IsActive);

      // Unique constraint for customer per type per period
      entity.HasIndex(e => new { e.CustomerId, e.Type, e.Period, e.PeriodStart }).IsUnique();
    });

    // CustomerStatistics Entity Configuration
    modelBuilder.Entity<CustomerStatistics>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.TotalSmokeFreesDays).HasDefaultValue(0);
      entity.Property(e => e.CurrentStreak).HasDefaultValue(0);
      entity.Property(e => e.LongestStreak).HasDefaultValue(0);
      entity.Property(e => e.TotalMoneySaved).HasColumnType("decimal(18,2)").HasDefaultValue(0);
      entity.Property(e => e.AverageDailySaving).HasColumnType("decimal(18,2)").HasDefaultValue(0);
      entity.Property(e => e.TotalCigarettesAvoided).HasDefaultValue(0);
      entity.Property(e => e.TotalPacksAvoided).HasDefaultValue(0);
      entity.Property(e => e.AverageMoodLevel).HasDefaultValue(0f);
      entity.Property(e => e.AverageEnergyLevel).HasDefaultValue(0f);
      entity.Property(e => e.AverageCravingLevel).HasDefaultValue(0f);
      entity.Property(e => e.TotalPosts).HasDefaultValue(0);
      entity.Property(e => e.TotalLikesReceived).HasDefaultValue(0);
      entity.Property(e => e.TotalCommentsReceived).HasDefaultValue(0);
      entity.Property(e => e.TotalBadgesEarned).HasDefaultValue(0);
      entity.Property(e => e.TotalPoints).HasDefaultValue(0);
      entity.Property(e => e.LastCalculated).IsRequired();
      entity.Property(e => e.CreatedAt).IsRequired();

      // Indexes
      entity.HasIndex(e => e.TotalSmokeFreesDays);
      entity.HasIndex(e => e.CurrentStreak);
      entity.HasIndex(e => e.TotalPoints);
      entity.HasIndex(e => e.LastCalculated);
    });

    // SystemReport Entity Configuration
    modelBuilder.Entity<SystemReport>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
      entity.Property(e => e.Type).IsRequired();
      entity.Property(e => e.StartDate).IsRequired();
      entity.Property(e => e.EndDate).IsRequired();
      entity.Property(e => e.Data).IsRequired().HasColumnType("nvarchar(max)");
      entity.Property(e => e.GeneratedAt).IsRequired();

      // Indexes
      entity.HasIndex(e => e.Type);
      entity.HasIndex(e => e.GeneratedAt);
      entity.HasIndex(e => new { e.StartDate, e.EndDate });
    });
  }
}