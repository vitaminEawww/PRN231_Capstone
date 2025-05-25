using DataAccess.Entities;
using DataAccess.Enums;
using Mapster;
using Repositories.IRepositories;
using Services.IServices;

namespace Services.Services;

public class DailyLogService : IDailyLogService
{
    private readonly IDailyLogRepository _dailyLogRepository;
    private readonly IDailyTaskRepository _dailyTaskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DailyLogService(
        IDailyLogRepository dailyLogRepository,
        IDailyTaskRepository dailyTaskRepository,
        IUnitOfWork unitOfWork)
    {
        _dailyLogRepository = dailyLogRepository;
        _dailyTaskRepository = dailyTaskRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<DailyLog>> GetUserLogs(int userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        return await _dailyLogRepository.GetUserLogs(userId, startDate, endDate);
    }

    public async Task<DailyLog> GetUserLogByDate(int userId, DateTime date)
    {
        return await _dailyLogRepository.GetUserLogByDate(userId, date);
    }

    public async Task<IEnumerable<DailyLog>> GetUserLogsByStatus(int userId, DailyLogStatus status)
    {
        return await _dailyLogRepository.GetUserLogsByStatus(userId, status);
    }

    public async Task<DailyLog> GetUserLogByTaskId(int userId, int taskId)
    {
        return await _dailyLogRepository.GetUserLogByTaskId(userId, taskId);
    }

    public async Task<DailyLog> CreateDailyLog(DailyLog log)
    {
        await ValidateLogData(log);
        await ValidateLogUniqueness(log);

        log.CreatedAt = DateTime.UtcNow;
        log.Status = DailyLogStatus.NotStarted;

        await _dailyLogRepository.AddAsync(log);
        await _unitOfWork.SaveAsync();

        return log;
    }

    public async Task<DailyLog> UpdateDailyLog(DailyLog log)
    {
        var existingLog = await _dailyLogRepository.GetByIdAsync(log.Id);
        if (existingLog == null)
            throw new KeyNotFoundException($"Log with ID {log.Id} not found");

        await ValidateLogData(log);

        log.Adapt(existingLog);

        _dailyLogRepository.Update(existingLog);
        await _unitOfWork.SaveAsync();

        return existingLog;
    }

    public async Task<bool> DeleteDailyLog(int id)
    {
        var log = await _dailyLogRepository.GetByIdAsync(id);
        if (log == null)
            throw new KeyNotFoundException($"Log with ID {id} not found");

        _dailyLogRepository.Delete(log);
        await _unitOfWork.SaveAsync();

        return true;
    }

    private async Task ValidateLogData(DailyLog log)
    {
        if (log.UserId <= 0)
            throw new ArgumentException("Invalid user ID");

        if (log.DailyTaskId <= 0)
            throw new ArgumentException("Invalid task ID");

        if (log.MoodRating.HasValue && (log.MoodRating < 1 || log.MoodRating > 5))
            throw new ArgumentException("Mood rating must be between 1 and 5");

        if (log.CravingsCount.HasValue && log.CravingsCount < 0)
            throw new ArgumentException("Cravings count cannot be negative");

        if (log.CigarettesSmoked.HasValue && log.CigarettesSmoked < 0)
            throw new ArgumentException("Cigarettes smoked count cannot be negative");

        if (log.MinutesOfExercise.HasValue && log.MinutesOfExercise < 0)
            throw new ArgumentException("Exercise minutes cannot be negative");

        if (log.WaterIntake.HasValue && log.WaterIntake < 0)
            throw new ArgumentException("Water intake cannot be negative");
    }

    private async Task ValidateLogUniqueness(DailyLog log)
    {
        // Check if task exists
        var task = await _dailyTaskRepository.GetByIdAsync(log.DailyTaskId);
        if (task == null)
            throw new KeyNotFoundException($"Task with ID {log.DailyTaskId} not found");

        // Check if a log already exists for this user and task
        var existingLog = await _dailyLogRepository.GetUserLogByTaskId(log.UserId, log.DailyTaskId);
        if (existingLog != null)
            throw new InvalidOperationException($"A log already exists for User {log.UserId} and Task {log.DailyTaskId}");
    }
}