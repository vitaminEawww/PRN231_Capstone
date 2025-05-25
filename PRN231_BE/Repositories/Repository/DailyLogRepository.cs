using DataAccess.Data;
using DataAccess.Entities;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using Repositories.IRepositories;

namespace Repositories.Repository;

public class DailyLogRepository(ApplicationDbContext context) : GenericRepository<DailyLog>(context), IDailyLogRepository
{
    public async Task<IEnumerable<DailyLog>> GetUserLogs(int userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        IQueryable<DailyLog> query = _context.DailyLogs
            .Where(l => l.UserId == userId)
            .Include(l => l.DailyTask);

        if (startDate.HasValue)
            query = query.Where(l => l.LogDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(l => l.LogDate <= endDate.Value);

        return await query.OrderByDescending(l => l.LogDate).ToListAsync();
    }

    public async Task<DailyLog> GetUserLogByDate(int userId, DateTime date)
    {
        return await _context.DailyLogs
            .Include(l => l.DailyTask)
            .FirstOrDefaultAsync(l => l.UserId == userId && l.LogDate.Date == date.Date);
    }

    public async Task<IEnumerable<DailyLog>> GetUserLogsByStatus(int userId, DailyLogStatus status)
    {
        return await _context.DailyLogs
            .Include(l => l.DailyTask)
            .Where(l => l.UserId == userId && l.Status == status)
            .OrderByDescending(l => l.LogDate)
            .ToListAsync();
    }

    public async Task<DailyLog> GetUserLogByTaskId(int userId, int taskId)
    {
        return await _context.DailyLogs
            .Include(l => l.DailyTask)
            .FirstOrDefaultAsync(l => l.UserId == userId && l.DailyTaskId == taskId);
    }
}