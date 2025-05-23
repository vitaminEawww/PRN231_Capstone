using DataAccess.Entities;

namespace Repositories.IRepositories
{
    public interface IUserAchievementRepository : IGenericRepository<UserAchievement>
    {
        Task<UserAchievement> GetUserAchievementWithDetailsAsync(int userId, int achievementId);
        Task<IEnumerable<UserAchievement>> GetUserAchievementsByUserIdAsync(int userId);
        Task<IEnumerable<UserAchievement>> GetUserAchievementsByAchievementIdAsync(int achievementId);
    }
}