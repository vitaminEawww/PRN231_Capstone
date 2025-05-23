using System;
using System.Threading.Tasks;
using DataAccess.Entities;

namespace Repositories.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> Users { get; }
        IFeatureRepository Features { get; }
        IPlanRepository Plans { get; }
        IPhaseRepository Phases { get; }
        IAchievementRepository Achievements { get; }
        IUserAchievementRepository UserAchievements { get; }
        IMembershipRepository Memberships { get; }
        Task<int> SaveAsync();
    }
}
