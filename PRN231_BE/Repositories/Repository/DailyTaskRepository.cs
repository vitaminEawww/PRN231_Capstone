using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.IRepositories;

namespace Repositories.Repository;

public class DailyTaskRepository(ApplicationDbContext context) : GenericRepository<DailyTask>(context), IDailyTaskRepository
{
    public async Task<IEnumerable<DailyTask>> GetTasksByPhaseId(int phaseId)
    {
        return await _context.DailyTask
            .Where(t => t.PhaseId == phaseId && t.IsActive)
            .OrderBy(t => t.DayNumber)
            .ToListAsync();
    }

    public async Task<DailyTask> GetTaskByDayNumber(int phaseId, int dayNumber)
    {
        return await _context.DailyTask
            .FirstOrDefaultAsync(t => t.PhaseId == phaseId && t.DayNumber == dayNumber && t.IsActive);
    }

    public async Task<IEnumerable<DailyTask>> GetActiveTasks()
    {
        return await _context.DailyTask
            .Where(t => t.IsActive)
            .OrderBy(t => t.PhaseId)
            .ThenBy(t => t.DayNumber)
            .ToListAsync();
    }
}