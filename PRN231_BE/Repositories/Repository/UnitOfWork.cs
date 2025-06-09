using DataAccess.Data;
using DataAccess.Entities;
using Repositories.IRepositories;
using Repositories.Repositories;

namespace Repositories.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        public IGenericRepository<User> Users { get; }
        public IMembershipRepository Memberships { get; }
        public IFeatureRepository Features { get; }
        public IPlanRepository Plans { get; }
        public IPhaseRepository Phases { get; }
        public IAchievementRepository Achievements { get; }
        public IUserAchievementRepository UserAchievements { get; }


        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            Users = new GenericRepository<User>(_dbContext);
            Memberships = new MembershipRepository(_dbContext);
            Features = new FeatureRepository(_dbContext);
            Plans = new PlanRepository(_dbContext);
            Phases = new PhaseRepository(_dbContext);
            Achievements = new AchievementRepository(_dbContext);
            UserAchievements = new UserAchievementRepository(_dbContext);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<int> SaveAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
