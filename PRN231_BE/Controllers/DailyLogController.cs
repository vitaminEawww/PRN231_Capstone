using DataAccess.Entities;
using DataAccess.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;

namespace PRN231_BE.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DailyLogController : ControllerBase
{
    private readonly IDailyLogService _dailyLogService;

    public DailyLogController(IDailyLogService dailyLogService)
    {
        _dailyLogService = dailyLogService;
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserLogs(int userId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var logs = await _dailyLogService.GetUserLogs(userId, startDate, endDate);
        return Ok(logs);
    }

    [HttpGet("user/{userId}/date/{date:datetime}")]
    public async Task<IActionResult> GetUserLogByDate(int userId, DateTime date)
    {
        var log = await _dailyLogService.GetUserLogByDate(userId, date);
        if (log == null)
            return NotFound();

        return Ok(log);
    }

    [HttpGet("user/{userId}/status/{status}")]
    public async Task<IActionResult> GetUserLogsByStatus(int userId, DailyLogStatus status)
    {
        var logs = await _dailyLogService.GetUserLogsByStatus(userId, status);
        return Ok(logs);
    }

    [HttpPost]
    public async Task<IActionResult> CreateLog([FromBody] DailyLog log)
    {
        try
        {
            var createdLog = await _dailyLogService.CreateDailyLog(log);
            return CreatedAtAction(nameof(GetUserLogByDate),
                new { userId = createdLog.UserId, date = createdLog.LogDate },
                createdLog);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLog(int id, [FromBody] DailyLog log)
    {
        if (id != log.Id)
            return BadRequest("Log ID mismatch");

        try
        {
            var updatedLog = await _dailyLogService.UpdateDailyLog(log);
            return Ok(updatedLog);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLog(int id)
    {
        try
        {
            await _dailyLogService.DeleteDailyLog(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}