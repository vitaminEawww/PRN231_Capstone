using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;

namespace WebAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class BackgroundJobController : ControllerBase
{
    private readonly IBackgroundJobService _backgroundJobService;

    public BackgroundJobController(IBackgroundJobService backgroundJobService)
    {
        _backgroundJobService = backgroundJobService;
    }

    /// <summary>
    /// [Admin] Kích hoạt job cập nhật thống kê thủ công
    /// </summary>
    [HttpPost("trigger-statistics-update")]
    public async Task<IActionResult> TriggerStatisticsUpdate()
    {
        var response = await _backgroundJobService.TriggerStatisticsUpdateJobAsync();
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// [Admin] Kích hoạt job cập nhật leaderboard thủ công
    /// </summary>
    [HttpPost("trigger-leaderboard-update")]
    public async Task<IActionResult> TriggerLeaderboardUpdate()
    {
        var response = await _backgroundJobService.TriggerLeaderboardUpdateJobAsync();
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// [Admin] Lấy trạng thái các background jobs
    /// </summary>
    [HttpGet("status")]
    public async Task<IActionResult> GetJobStatus()
    {
        var response = await _backgroundJobService.GetJobStatusAsync();
        return StatusCode((int)response.StatusCode, response);
    }
}