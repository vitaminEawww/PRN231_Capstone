using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using Repositories.IRepositories;

namespace Repositories.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private IDbContextTransaction? _transaction;

        public IGenericRepository<User> Users { get; }
        public IGenericRepository<Customer> Customers { get; }
        public IGenericRepository<Coach> Coaches { get; }
        public IGenericRepository<Post> Posts { get; }
        public IGenericRepository<QuitPlan> QuitPlans { get; }
        public IGenericRepository<SmokingRecord> SmokingRecords { get; }
        public IGenericRepository<DailyProgress> DailyProgresses { get; }
        public IGenericRepository<Badge> Badges { get; }
        public IGenericRepository<UserBadge> UserBadges { get; }
        public IGenericRepository<MembershipPackage> MembershipPackages { get; }
        public IGenericRepository<Payment> Payments { get; }
        public IGenericRepository<Consultation> Consultations { get; }
        public IGenericRepository<Notification> Notifications { get; }

        public IGenericRepository<Message> Messages { get; }

        public IGenericRepository<Conversation> Conversations { get; }

        public IGenericRepository<Leaderboard> Leaderboards { get; }

        public IGenericRepository<CustomerStatistics> CustomerStatistics { get; }

        public IGenericRepository<SystemReport> SystemReports { get; }

        public IGenericRepository<Rating> Ratings { get; }

        public IGenericRepository<MemberShipUsage> MemberShipUsages { get; }

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            Users = new GenericRepository<User>(_dbContext);
            Customers = new GenericRepository<Customer>(_dbContext);
            Coaches = new GenericRepository<Coach>(_dbContext);
            Posts = new GenericRepository<Post>(_dbContext);
            QuitPlans = new GenericRepository<QuitPlan>(_dbContext);
            SmokingRecords = new GenericRepository<SmokingRecord>(_dbContext);
            DailyProgresses = new GenericRepository<DailyProgress>(_dbContext);
            Badges = new GenericRepository<Badge>(_dbContext);
            UserBadges = new GenericRepository<UserBadge>(_dbContext);
            MembershipPackages = new GenericRepository<MembershipPackage>(_dbContext);
            Payments = new GenericRepository<Payment>(_dbContext);
            Consultations = new GenericRepository<Consultation>(_dbContext);
            Notifications = new GenericRepository<Notification>(_dbContext);
            Messages = new GenericRepository<Message>(_dbContext);
            Conversations = new GenericRepository<Conversation>(_dbContext);
            Leaderboards = new GenericRepository<Leaderboard>(_dbContext);
            CustomerStatistics = new GenericRepository<CustomerStatistics>(_dbContext);
            SystemReports = new GenericRepository<SystemReport>(_dbContext);
            Ratings = new GenericRepository<Rating>(_dbContext);
            MemberShipUsages = new GenericRepository<MemberShipUsage>(_dbContext);
        }

        public async Task<int> SaveAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            _transaction = await _dbContext.Database.BeginTransactionAsync();
            return _transaction;
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _dbContext.Dispose();
        }
    }
}