using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class LeaderboardController : ControllerBase
{
    private readonly ILeaderboardService _leaderboardService;

    public LeaderboardController(ILeaderboardService leaderboardService)
    {
        _leaderboardService = leaderboardService;
    }

    /// <summary>
    /// Lấy bảng xếp hạng theo period và type
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetLeaderboard(
        [FromQuery] string period = "Daily",
        [FromQuery] string type = "CurrentStreak",
        [FromQuery] int limit = 100)
    {
        var result = await _leaderboardService.GetLeaderboardAsync(period, type, limit);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Cập nhật tất cả bảng xếp hạng (Admin only)
    /// </summary>
    [HttpPost("update-all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateAllLeaderboards()
    {
        var result = await _leaderboardService.UpdateAllLeaderboardsAsync();
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Cập nhật bảng xếp hạng hàng ngày (Admin only)
    /// </summary>
    [HttpPost("update-daily")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateDailyLeaderboard()
    {
        var result = await _leaderboardService.UpdateDailyLeaderboardAsync();
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Cập nhật bảng xếp hạng hàng tuần (Admin only)
    /// </summary>
    [HttpPost("update-weekly")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateWeeklyLeaderboard()
    {
        var result = await _leaderboardService.UpdateWeeklyLeaderboardAsync();
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Cập nhật bảng xếp hạng hàng tháng (Admin only)
    /// </summary>
    [HttpPost("update-monthly")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateMonthlyLeaderboard()
    {
        var result = await _leaderboardService.UpdateMonthlyLeaderboardAsync();
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}