using DataAccess.Entities;

namespace Repositories.IRepositories
{
    public interface IAchievementRepository : IGenericRepository<Achievement>
    {
        Task<Achievement> GetAchievementWithUsersAsync(int achievementId);
        Task<IEnumerable<Achievement>> GetAllAchievementsWithUsersAsync();
        Task<IEnumerable<Achievement>> GetAchievementsByUserIdAsync(int userId);
    }
}