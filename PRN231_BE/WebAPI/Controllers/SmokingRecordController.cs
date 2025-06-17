using DataAccess.Common;
using DataAccess.Models.SmokingRecords;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class SmokingRecordController : ControllerBase
    {
        private readonly ISmokingRecordService _smokingRecordService;

        public SmokingRecordController(ISmokingRecordService smokingRecordService)
        {
            _smokingRecordService = smokingRecordService;
        }

        /// <summary>
        /// Tạo hồ sơ hút thuốc mới
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateSmokingRecord([FromBody] SmokingRecordCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var response = new ApiResponse
                {
                    IsSuccess = false,
                    ErrorMessages = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                };
                return BadRequest(response);
            }

            var result = await _smokingRecordService.CreateSmokingRecordAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Lấy hồ sơ hút thuốc hiện tại của khách hàng
        /// </summary>
        [HttpGet("current/{customerId}")]
        public async Task<ActionResult<ApiResponse>> GetCurrentSmokingRecord(int customerId)
        {
            var result = await _smokingRecordService.GetCurrentSmokingRecordAsync(customerId);
            return Ok(result);
        }

        /// <summary>
        /// Lấy lịch sử hút thuốc của khách hàng
        /// </summary>
        [HttpGet("history/{customerId}")]
        public async Task<ActionResult<ApiResponse>> GetSmokingHistory(
            int customerId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var result = await _smokingRecordService.GetSmokingHistoryAsync(customerId, pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin hút thuốc theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse>> GetSmokingRecordById(int id)
        {
            var result = await _smokingRecordService.GetSmokingRecordByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Cập nhật thông tin hút thuốc
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse>> UpdateSmokingRecord(int id, [FromBody] SmokingRecordUpdateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var response = new ApiResponse
                {
                    IsSuccess = false,
                    ErrorMessages = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                };
                return BadRequest(response);
            }

            var result = await _smokingRecordService.UpdateSmokingRecordAsync(id, dto);
            return Ok(result);
        }

        /// <summary>
        /// Xóa bản ghi hút thuốc
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteSmokingRecord(int id)
        {
            var result = await _smokingRecordService.DeleteSmokingRecordAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Đánh dấu bắt đầu cai thuốc
        /// </summary>
        [HttpPatch("{id}/quit")]
        public async Task<ActionResult<ApiResponse>> StartQuitting(int id)
        {
            var updateDto = new SmokingRecordUpdateDTO
            {
                QuitSmokingStartDate = DateTime.Now
            };

            var result = await _smokingRecordService.UpdateSmokingRecordAsync(id, updateDto);
            return Ok(result);
        }
    }
}