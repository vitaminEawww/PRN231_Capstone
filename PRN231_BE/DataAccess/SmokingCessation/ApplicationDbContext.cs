using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.SmokingCessation
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<MembershipPlan> MembershipPlans { get; set; }
        public DbSet<SmokingLog> SmokingLogs { get; set; }
        public DbSet<QuitPlan> QuitPlans { get; set; }
        public DbSet<ProgressTracking> ProgressTrackings { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<TrainerChat> TrainerChats { get; set; }
    }
}
