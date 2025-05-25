using DataAccess.Entities;
using Mapster;
using Repositories.IRepositories;
using Services.IServices;

namespace Services.Services;

public class DailyTaskService : IDailyTaskService
{
    private readonly IDailyTaskRepository _dailyTaskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DailyTaskService(IDailyTaskRepository dailyTaskRepository, IUnitOfWork unitOfWork)
    {
        _dailyTaskRepository = dailyTaskRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<DailyTask>> GetTasksByPhaseId(int phaseId)
    {
        return await _dailyTaskRepository.GetTasksByPhaseId(phaseId);
    }

    public async Task<DailyTask> GetTaskByDayNumber(int phaseId, int dayNumber)
    {
        return await _dailyTaskRepository.GetTaskByDayNumber(phaseId, dayNumber);
    }

    public async Task<IEnumerable<DailyTask>> GetActiveTasks()
    {
        return await _dailyTaskRepository.GetActiveTasks();
    }

    public async Task<DailyTask> CreateDailyTask(DailyTask task)
    {
        // Validate task data
        if (string.IsNullOrEmpty(task.Title))
            throw new ArgumentException("Task title is required");

        if (string.IsNullOrEmpty(task.Description))
            throw new ArgumentException("Task description is required");

        if (task.DayNumber <= 0)
            throw new ArgumentException("Day number must be greater than 0");

        // Check if a task already exists for this phase and day
        var existingTask = await _dailyTaskRepository.GetTaskByDayNumber(task.PhaseId, task.DayNumber);
        if (existingTask != null)
            throw new InvalidOperationException($"A task already exists for Phase {task.PhaseId}, Day {task.DayNumber}");

        task.CreatedAt = DateTime.UtcNow;
        task.IsActive = true;

        await _dailyTaskRepository.AddAsync(task);
        await _unitOfWork.SaveAsync();

        return task;
    }

    public async Task<DailyTask> UpdateDailyTask(DailyTask task)
    {
        var existingTask = await _dailyTaskRepository.GetByIdAsync(task.Id);
        if (existingTask == null)
            throw new KeyNotFoundException($"Task with ID {task.Id} not found");

        // Validate task data
        if (string.IsNullOrEmpty(task.Title))
            throw new ArgumentException("Task title is required");

        if (string.IsNullOrEmpty(task.Description))
            throw new ArgumentException("Task description is required");

        if (task.DayNumber <= 0)
            throw new ArgumentException("Day number must be greater than 0");

        // Check if updating day number would conflict with another task
        if (task.DayNumber != existingTask.DayNumber)
        {
            var conflictingTask = await _dailyTaskRepository.GetTaskByDayNumber(task.PhaseId, task.DayNumber);
            if (conflictingTask != null)
                throw new InvalidOperationException($"A task already exists for Phase {task.PhaseId}, Day {task.DayNumber}");
        }

        task.Adapt(existingTask);

        _dailyTaskRepository.Update(existingTask);
        await _unitOfWork.SaveAsync();

        return existingTask;
    }

    public async Task<bool> DeleteDailyTask(int id)
    {
        var task = await _dailyTaskRepository.GetByIdAsync(id);
        if (task == null)
            throw new KeyNotFoundException($"Task with ID {id} not found");

        _dailyTaskRepository.Delete(task);
        await _unitOfWork.SaveAsync();

        return true;
    }
}