using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.IRepositories;
using Repositories.Repository;

namespace Repositories.Repositories
{
    public class AchievementRepository(ApplicationDbContext context) : GenericRepository<Achievement>(context), IAchievementRepository
    {
        public async Task<Achievement?> GetAchievementWithUsersAsync(int achievementId)
        {
            return await _dbSet
                .Include(a => a.UserAchievements)
                    .ThenInclude(ua => ua.User)
                .FirstOrDefaultAsync(a => a.AchievementId == achievementId);
        }

        public async Task<IEnumerable<Achievement>> GetAllAchievementsWithUsersAsync()
        {
            return await _dbSet
                .Include(a => a.UserAchievements)
                    .ThenInclude(ua => ua.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Achievement>> GetAchievementsByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(a => a.UserAchievements.Where(ua => ua.UserId == userId))
                    .ThenInclude(ua => ua.User)
                .Where(a => a.UserAchievements.Any(ua => ua.UserId == userId))
                .ToListAsync();
        }
    }
}