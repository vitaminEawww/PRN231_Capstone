using System;
using System.Threading.Tasks;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace Repositories.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<Customer> Customers { get; }
        IGenericRepository<Coach> Coaches { get; }
        IGenericRepository<Post> Posts { get; }
        IGenericRepository<QuitPlan> QuitPlans { get; }
        IGenericRepository<SmokingRecord> SmokingRecords { get; }
        IGenericRepository<DailyProgress> DailyProgresses { get; }
        IGenericRepository<Badge> Badges { get; }
        IGenericRepository<UserBadge> UserBadges { get; }
        IGenericRepository<MembershipPackage> MembershipPackages { get; }
        IGenericRepository<Payment> Payments { get; }
        IGenericRepository<Consultation> Consultations { get; }
        IGenericRepository<Notification> Notifications { get; }
        IGenericRepository<Message> Messages { get; }
        IGenericRepository<Conversation> Conversations { get; }
        IGenericRepository<Leaderboard> Leaderboards { get; }
        IGenericRepository<CustomerStatistics> CustomerStatistics { get; }
        IGenericRepository<SystemReport> SystemReports { get; }
        IGenericRepository<Rating> Ratings { get; }
        Task<int> SaveAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
