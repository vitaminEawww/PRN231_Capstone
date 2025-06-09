using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;

namespace PRN231_BE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DailyTaskController : ControllerBase
{
    private readonly IDailyTaskService _dailyTaskService;

    public DailyTaskController(IDailyTaskService dailyTaskService)
    {
        _dailyTaskService = dailyTaskService;
    }

    [HttpGet("phase/{phaseId}")]
    public async Task<IActionResult> GetTasksByPhase(int phaseId)
    {
        var tasks = await _dailyTaskService.GetTasksByPhaseId(phaseId);
        return Ok(tasks);
    }

    [HttpGet("phase/{phaseId}/day/{dayNumber}")]
    public async Task<IActionResult> GetTaskByDayNumber(int phaseId, int dayNumber)
    {
        var task = await _dailyTaskService.GetTaskByDayNumber(phaseId, dayNumber);
        if (task == null)
            return NotFound();

        return Ok(task);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveTasks()
    {
        var tasks = await _dailyTaskService.GetActiveTasks();
        return Ok(tasks);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateTask([FromBody] DailyTask task)
    {
        try
        {
            var createdTask = await _dailyTaskService.CreateDailyTask(task);
            return CreatedAtAction(nameof(GetTaskByDayNumber),
                new { phaseId = createdTask.PhaseId, dayNumber = createdTask.DayNumber },
                createdTask);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] DailyTask task)
    {
        if (id != task.Id)
            return BadRequest("Task ID mismatch");

        try
        {
            var updatedTask = await _dailyTaskService.UpdateDailyTask(task);
            return Ok(updatedTask);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        try
        {
            await _dailyTaskService.DeleteDailyTask(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}