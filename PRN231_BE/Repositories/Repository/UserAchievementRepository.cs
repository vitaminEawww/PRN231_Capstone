using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.IRepositories;
using Repositories.Repository;

namespace Repositories.Repositories
{
    public class UserAchievementRepository(ApplicationDbContext context) : GenericRepository<UserAchievement>(context), IUserAchievementRepository
    {
        public async Task<UserAchievement?> GetUserAchievementWithDetailsAsync(int userId, int achievementId)
        {
            return await _dbSet
                .Include(ua => ua.User)
                .Include(ua => ua.Achievement)
                .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AchievementId == achievementId);
        }

        public async Task<IEnumerable<UserAchievement>> GetUserAchievementsByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(ua => ua.Achievement)
                .Where(ua => ua.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserAchievement>> GetUserAchievementsByAchievementIdAsync(int achievementId)
        {
            return await _dbSet
                .Include(ua => ua.User)
                .Where(ua => ua.AchievementId == achievementId)
                .ToListAsync();
        }
    }
}