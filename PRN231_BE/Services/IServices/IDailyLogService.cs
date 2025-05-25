using DataAccess.Entities;
using DataAccess.Enums;

namespace Services.IServices;

public interface IDailyLogService
{
    Task<IEnumerable<DailyLog>> GetUserLogs(int userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<DailyLog> GetUserLogByDate(int userId, DateTime date);
    Task<IEnumerable<DailyLog>> GetUserLogsByStatus(int userId, DailyLogStatus status);
    Task<DailyLog> GetUserLogByTaskId(int userId, int taskId);
    Task<DailyLog> CreateDailyLog(DailyLog log);
    Task<DailyLog> UpdateDailyLog(DailyLog log);
    Task<bool> DeleteDailyLog(int id);
}