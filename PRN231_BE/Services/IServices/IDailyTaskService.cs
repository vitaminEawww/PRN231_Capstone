using DataAccess.Entities;

namespace Services.IServices;

public interface IDailyTaskService
{
    Task<IEnumerable<DailyTask>> GetTasksByPhaseId(int phaseId);
    Task<DailyTask> GetTaskByDayNumber(int phaseId, int dayNumber);
    Task<IEnumerable<DailyTask>> GetActiveTasks();
    Task<DailyTask> CreateDailyTask(DailyTask task);
    Task<DailyTask> UpdateDailyTask(DailyTask task);
    Task<bool> DeleteDailyTask(int id);
}