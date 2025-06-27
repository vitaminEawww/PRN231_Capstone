using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;
using DataAccess.Models.DailyProgress;
using System.Security.Claims;

namespace WebAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class DailyProgressController : ControllerBase
{
    private readonly IDailyProgressService _dailyProgressService;
    private readonly ICustomerService _customerService;

    public DailyProgressController(
        IDailyProgressService dailyProgressService,
        ICustomerService customerService)
    {
        _dailyProgressService = dailyProgressService;
        _customerService = customerService;
    }

    /// <summary>
    /// Ghi nhận tiến trình cai thuốc hàng ngày
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateDailyProgress([FromBody] DailyProgressCreateDTO dto)
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        var response = await _dailyProgressService.CreateDailyProgressAsync(customerId.Value, dto);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Lấy dữ liệu theo dõi theo ID
    /// </summary>
    [HttpGet("{progressId}")]
    public async Task<IActionResult> GetDailyProgress(int progressId)
    {
        var response = await _dailyProgressService.GetDailyProgressByIdAsync(progressId);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Lấy dữ liệu theo dõi theo ngày cụ thể
    /// </summary>
    [HttpGet("date/{date:datetime}")]
    public async Task<IActionResult> GetDailyProgressByDate(DateTime date)
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        var response = await _dailyProgressService.GetDailyProgressByDateAsync(customerId.Value, date);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Lấy danh sách dữ liệu theo dõi của khách hàng hiện tại
    /// </summary>
    [HttpGet("my-progress")]
    public async Task<IActionResult> GetMyDailyProgress([FromQuery] ProgressGetListDTO dto)
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        var response = await _dailyProgressService.GetCustomerDailyProgressAsync(customerId.Value, dto);
        return StatusCode((int)response.StatusCode, response.Result);
    }

    /// <summary>
    /// Cập nhật dữ liệu theo dõi hàng ngày
    /// </summary>
    [HttpPut("{progressId}")]
    public async Task<IActionResult> UpdateDailyProgress(int progressId, [FromBody] DailyProgressUpdateDTO dto)
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        if (progressId != dto.Id)
        {
            return BadRequest("ID không khớp");
        }

        var response = await _dailyProgressService.UpdateDailyProgressAsync(customerId.Value, dto);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Xóa dữ liệu theo dõi hàng ngày
    /// </summary>
    [HttpDelete("{progressId}")]
    public async Task<IActionResult> DeleteDailyProgress(int progressId)
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        var response = await _dailyProgressService.DeleteDailyProgressAsync(customerId.Value, progressId);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Lấy thống kê tiến trình cai thuốc tổng quan
    /// </summary>
    [HttpGet("statistics")]
    public async Task<IActionResult> GetProgressStatistics()
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        var response = await _dailyProgressService.GetDailyProgressStatisticsAsync(customerId.Value);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Lấy chuỗi ngày hiện tại không hút thuốc
    /// </summary>
    [HttpGet("current-streak")]
    public async Task<IActionResult> GetCurrentStreak()
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        var response = await _dailyProgressService.GetCurrentStreakAsync(customerId.Value);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Lấy danh sách ngày không hút thuốc
    /// </summary>
    [HttpGet("smoke-free-days")]
    public async Task<IActionResult> GetSmokeFreeeDays(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        var response = await _dailyProgressService.GetSmokeFreeeDaysAsync(customerId.Value, startDate, endDate);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Lấy danh sách ngày có hút thuốc
    /// </summary>
    [HttpGet("smoking-days")]
    public async Task<IActionResult> GetSmokingDays(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        var response = await _dailyProgressService.GetSmokingDaysAsync(customerId.Value, startDate, endDate);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Lấy xu hướng sức khỏe
    /// </summary>
    [HttpGet("health-trends")]
    public async Task<IActionResult> GetHealthTrends([FromQuery] int days = 30)
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        var response = await _dailyProgressService.GetHealthTrendsAsync(customerId.Value, days);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Phân tích mức độ thèm muốn
    /// </summary>
    [HttpGet("craving-analysis")]
    public async Task<IActionResult> GetCravingAnalysis([FromQuery] int days = 30)
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        var response = await _dailyProgressService.GetCravingAnalysisAsync(customerId.Value, days);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Phân tích tác nhân gây thèm thuốc
    /// </summary>
    [HttpGet("triggers-analysis")]
    public async Task<IActionResult> GetTriggersAnalysis([FromQuery] int days = 30)
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        var response = await _dailyProgressService.GetTriggersAnalysisAsync(customerId.Value, days);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Tóm tắt tiến trình hàng tuần
    /// </summary>
    [HttpGet("weekly-summary")]
    public async Task<IActionResult> GetWeeklyProgressSummary([FromQuery] DateTime? weekStartDate = null)
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        var response = await _dailyProgressService.GetWeeklyProgressSummaryAsync(customerId.Value, weekStartDate);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Tóm tắt tiến trình hàng tháng
    /// </summary>
    [HttpGet("monthly-summary")]
    public async Task<IActionResult> GetMonthlyProgressSummary(
        [FromQuery] int? year = null,
        [FromQuery] int? month = null)
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        var response = await _dailyProgressService.GetMonthlyProgressSummaryAsync(customerId.Value, year, month);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Cập nhật lại thống kê khách hàng
    /// </summary>
    [HttpPost("recalculate-statistics")]
    public async Task<IActionResult> RecalculateStatistics()
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        var response = await _dailyProgressService.RecalculateCustomerStatisticsAsync(customerId.Value);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// [Admin] Cập nhật thống kê cho tất cả khách hàng
    /// </summary>
    [HttpPost("update-all-statistics")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateAllCustomerStatistics()
    {
        var response = await _dailyProgressService.UpdateAllCustomerStatisticsAsync();
        return StatusCode((int)response.StatusCode, response);
    }

    private async Task<int?> GetCurrentCustomerId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out var userId))
        {
            return await _customerService.GetCustomerIdByUserId(userId);
        }
        return null;
    }
}