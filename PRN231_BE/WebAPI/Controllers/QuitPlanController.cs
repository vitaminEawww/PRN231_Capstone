using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;
using DataAccess.Models.Plans;
using System.Security.Claims;

namespace WebAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class QuitPlanController : ControllerBase
{
    private readonly IPlanService _planService;
    private readonly ICustomerService _customerService;

    public QuitPlanController(IPlanService planService, ICustomerService customerService)
    {
        _planService = planService;
        _customerService = customerService;
    }

    /// <summary>
    /// Tạo kế hoạch cai thuốc mới
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateQuitPlan([FromBody] QuitPlanCreateDTO dto)
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        var response = await _planService.CreateQuitPlanAsync(customerId.Value, dto);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Lấy danh sách kế hoạch cai thuốc của khách hàng
    /// </summary>
    [HttpGet("my-plans")]
    public async Task<IActionResult> GetMyQuitPlans([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        var response = await _planService.GetCustomerQuitPlansAsync(customerId.Value, pageNumber, pageSize);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Lấy chi tiết kế hoạch cai thuốc
    /// </summary>
    [HttpGet("{planId}")]
    public async Task<IActionResult> GetQuitPlan(int planId)
    {
        var response = await _planService.GetQuitPlanByIdAsync(planId);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Cập nhật thông tin kế hoạch cai thuốc
    /// </summary>
    [HttpPut("{planId}")]
    public async Task<IActionResult> UpdateQuitPlan(int planId, [FromBody] QuitPlanUpdateDTO dto)
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        // Đảm bảo planId trong URL khớp với DTO
        dto.Id = planId;
        var response = await _planService.UpdateQuitPlanAsync(customerId.Value, dto);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Xóa kế hoạch cai thuốc
    /// </summary>
    [HttpDelete("{planId}")]
    public async Task<IActionResult> DeleteQuitPlan(int planId)
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        var response = await _planService.DeleteQuitPlanAsync(customerId.Value, planId);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Bắt đầu thực hiện kế hoạch
    /// </summary>
    [HttpPost("{planId}/start")]
    public async Task<IActionResult> StartQuitPlan(int planId)
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        var response = await _planService.StartQuitPlanAsync(customerId.Value, planId);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Tạm dừng kế hoạch
    /// </summary>
    [HttpPost("{planId}/pause")]
    public async Task<IActionResult> PauseQuitPlan(int planId)
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        var response = await _planService.PauseQuitPlanAsync(customerId.Value, planId);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Tiếp tục kế hoạch
    /// </summary>
    [HttpPost("{planId}/resume")]
    public async Task<IActionResult> ResumeQuitPlan(int planId)
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        var response = await _planService.ResumeQuitPlanAsync(customerId.Value, planId);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Đánh dấu hoàn thành kế hoạch
    /// </summary>
    [HttpPost("{planId}/complete")]
    public async Task<IActionResult> CompleteQuitPlan(int planId)
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        var response = await _planService.CompleteQuitPlanAsync(customerId.Value, planId);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Đánh dấu kế hoạch thất bại
    /// </summary>
    [HttpPost("{planId}/fail")]
    public async Task<IActionResult> FailQuitPlan(int planId)
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        var response = await _planService.FailQuitPlanAsync(customerId.Value, planId);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Lấy thống kê chi tiết của kế hoạch
    /// </summary>
    [HttpGet("{planId}/statistics")]
    public async Task<IActionResult> GetPlanStatistics(int planId)
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        var response = await _planService.GetPlanStatisticsAsync(customerId.Value, planId);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Lấy kế hoạch được đề xuất
    /// </summary>
    [HttpGet("recommended")]
    public async Task<IActionResult> GetRecommendedPlan()
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        var response = await _planService.GenerateRecommendedPlanAsync(customerId.Value);
        return StatusCode((int)response.StatusCode, response);
    }

    // ==================== PHASE MANAGEMENT APIs ====================

    /// <summary>
    /// Lấy danh sách giai đoạn của kế hoạch
    /// </summary>
    [HttpGet("{planId}/phases")]
    public async Task<IActionResult> GetPlanPhases(int planId)
    {
        var response = await _planService.GetPlanPhasesAsync(planId);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Cập nhật trạng thái giai đoạn (hoàn thành/chưa hoàn thành)
    /// </summary>
    [HttpPost("phases/{phaseId}/status")]
    public async Task<IActionResult> UpdatePhaseStatus(int phaseId, [FromBody] bool isCompleted)
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        var response = await _planService.UpdatePhaseStatusAsync(customerId.Value, phaseId, isCompleted);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Thêm giai đoạn tùy chỉnh vào kế hoạch
    /// </summary>
    [HttpPost("{planId}/phases")]
    public async Task<IActionResult> AddCustomPhase(int planId, [FromBody] QuitPlanPhaseCreateDTO dto)
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        var response = await _planService.AddCustomPhaseAsync(customerId.Value, planId, dto);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Cập nhật thông tin giai đoạn
    /// </summary>
    [HttpPut("phases/{phaseId}")]
    public async Task<IActionResult> UpdatePhase(int phaseId, [FromBody] QuitPlanPhaseCreateDTO dto)
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        var response = await _planService.UpdatePhaseAsync(customerId.Value, phaseId, dto);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Xóa giai đoạn khỏi kế hoạch
    /// </summary>
    [HttpDelete("phases/{phaseId}")]
    public async Task<IActionResult> DeletePhase(int phaseId)
    {
        var customerId = await GetCurrentCustomerId();
        if (customerId == null)
        {
            return BadRequest("Không thể xác định thông tin khách hàng");
        }

        var response = await _planService.DeletePhaseAsync(customerId.Value, phaseId);
        return StatusCode((int)response.StatusCode, response);
    }

    private async Task<int?> GetCurrentCustomerId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out var userId))
        {
            var customerId = await _customerService.GetCustomerIdByUserId(userId);
            return customerId;
        }
        return null;
    }
}