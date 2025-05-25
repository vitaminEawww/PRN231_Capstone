using DataAccess.Entities;

namespace Repositories.IRepositories;

public interface IDailyTaskRepository : IGenericRepository<DailyTask>
{
    Task<IEnumerable<DailyTask>> GetTasksByPhaseId(int phaseId);
    Task<DailyTask> GetTaskByDayNumber(int phaseId, int dayNumber);
    Task<IEnumerable<DailyTask>> GetActiveTasks();
}