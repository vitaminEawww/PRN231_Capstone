using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<DailyLog> DailyLogs { get; set; }
    public DbSet<Feature> Features { get; set; }
    public DbSet<Membership> Memberships { get; set; }
    public DbSet<MemFeature> MemFeatures { get; set; }
    public DbSet<Phase> Phases { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<DailyTask> DailyTask { get; set; }
    public DbSet<Trainer> Trainers { get; set; }
    public DbSet<TrainerChat> TrainerChats { get; set; }
    public DbSet<UserAchievement> UserAchievements { get; set; }
    public DbSet<UserPlan> UserPlans { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure one-to-many relationship between User and Post
        modelBuilder.Entity<User>()
            .HasMany(u => u.Posts)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure one-to-many relationship between User and Comment
        modelBuilder.Entity<User>()
            .HasMany(u => u.Comments)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict); // Using Restrict to avoid cascade delete conflict with Posts

        // Configure one-to-many relationship between Post and Comment
        modelBuilder.Entity<Post>()
            .HasMany(p => p.Comments)
            .WithOne(c => c.Post)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure one-to-many relationship between User and TrainerChat
        modelBuilder.Entity<User>()
            .HasMany(u => u.TrainerChats)
            .WithOne(tc => tc.User)
            .HasForeignKey(tc => tc.UserID)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure one-to-many relationship between Trainer and TrainerChat
        modelBuilder.Entity<Trainer>()
            .HasMany<TrainerChat>()
            .WithOne(tc => tc.Trainer)
            .HasForeignKey(tc => tc.TrainerID)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure one-to-many relationship between User and UserAchievement
        modelBuilder.Entity<User>()
            .HasMany(u => u.UserAchievements)
            .WithOne(ua => ua.User)
            .HasForeignKey(ua => ua.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure one-to-many relationship between Achievement and UserAchievement
        modelBuilder.Entity<Achievement>()
            .HasMany(a => a.UserAchievements)
            .WithOne(ua => ua.Achievement)
            .HasForeignKey(ua => ua.AchievementId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure one-to-many relationship between User and UserMembership
        modelBuilder.Entity<User>()
            .HasMany(u => u.UserMemberships)
            .WithOne(um => um.User)
            .HasForeignKey(um => um.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure one-to-many relationship between Membership and UserMembership
        modelBuilder.Entity<Membership>()
            .HasMany<UserMembership>()
            .WithOne(um => um.Membership)
            .HasForeignKey(um => um.MembershipId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure one-to-many relationship between User and UserPlan
        modelBuilder.Entity<User>()
            .HasMany(u => u.UserPlans)
            .WithOne(up => up.User)
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure one-to-many relationship between Plan and UserPlan
        modelBuilder.Entity<Plan>()
            .HasMany(p => p.UserPlans)
            .WithOne(up => up.Plan)
            .HasForeignKey(up => up.PlanId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure one-to-many relationship between Plan and Phase
        modelBuilder.Entity<Plan>()
            .HasMany(p => p.Phases)
            .WithOne(ph => ph.Plan)
            .HasForeignKey(ph => ph.PlanId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure one-to-many relationship between User and DailyLog
        modelBuilder.Entity<User>()
            .HasMany(u => u.DailyLogs)
            .WithOne(dl => dl.User)
            .HasForeignKey(dl => dl.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure one-to-many relationship between DailyTask and DailyLog
        modelBuilder.Entity<DailyTask>()
            .HasMany(dt => dt.DailyLogs)
            .WithOne(dl => dl.DailyTask)
            .HasForeignKey(dl => dl.DailyTaskId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure one-to-many relationship between User and Notification
        modelBuilder.Entity<User>()
            .HasMany(u => u.Notifications)
            .WithOne(n => n.User)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure many-to-many relationship between Membership and Feature through MemFeature
        modelBuilder.Entity<MemFeature>()
            .HasOne(mf => mf.Membership)
            .WithMany()
            .HasForeignKey(mf => mf.MembershipId);

        modelBuilder.Entity<MemFeature>()
            .HasOne(mf => mf.Feature)
            .WithMany()
            .HasForeignKey(mf => mf.FeatureId);

        // Configure unique constraints
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

    }
}
