using DataAccess.Common;
using DataAccess.Models.MembershipPackage;
using DataAccess.Models.MemberShipPackage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;
using System.Net;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembershipPackageController : ControllerBase
    {
        private readonly IMembershipPackageService _membershipPackageService;

        public MembershipPackageController(IMembershipPackageService membershipPackageService)
        {
            _membershipPackageService = membershipPackageService;
        }

        /// <summary>
        /// Lấy tất cả gói membership
        /// </summary>
        /// <returns>Danh sách tất cả gói membership</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllMembershipPackages()
        {
            var response = await _membershipPackageService.GetAllMembershipPackagesAsync();

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Lấy gói membership theo ID
        /// </summary>
        /// <param name="id">ID của gói membership</param>
        /// <returns>Thông tin gói membership</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMembershipPackageById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "ID phải lớn hơn 0" });
            }

            var response = await _membershipPackageService.GetMembershipPackageByIdAsync(id);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Tạo gói membership mới
        /// </summary>
        /// <param name="createmembershipDTO">Thông tin gói membership mới</param>
        /// <returns>Gói membership đã được tạo</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreatePackage([FromBody] CreateMembershipPackageDTO packageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _membershipPackageService.CreateMembershipPackageAsync(packageDto);

            if (!response.IsSuccess)
            {
                return StatusCode((int)response.StatusCode, response);
            }

            return StatusCode((int)HttpStatusCode.Created, new ApiResponse
            {
                StatusCode = HttpStatusCode.Created,
                IsSuccess = true,
                ErrorMessages = response.ErrorMessages,
                Result = response.Result 
            });
        }

        /// <summary>
        /// Lấy danh sách gói membership theo phân trang
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedPackages(int pageNumber = 1, int pageSize = 10)
        {
            var response = await _membershipPackageService.GetPagedMembershipPackagesAsync(pageNumber, pageSize);
            if (!response.IsSuccess)
            {
                return StatusCode((int)response.StatusCode, response);
            }

            return Ok(response.Result);
        }

        /// <summary>
        /// Cập nhật gói membership theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="packageDto"></param>
        /// <returns></returns>
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateMembershipPackage(int id, [FromBody] UpdateMembershipPackageDTO packageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _membershipPackageService.UpdateMembershipPackageAsync(id, packageDto);

            if (!response.IsSuccess)
            {
                return StatusCode((int)response.StatusCode, response);
            }

            return Ok(response); 
        }


        /// <summary>
        /// Xóa gói membership theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteMembershipPackage(int id)
        {
            var response = await _membershipPackageService.DeleteMembershipPackageAsync(id);

            if (!response.IsSuccess)
            {
                return StatusCode((int)response.StatusCode, response); 
            }

            return Ok(response.Result); 
        }



        /// <summary>
        /// Nâng cấp gói membership cho người dùng
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPackageId"></param>
        /// <returns></returns>
        [HttpPost("upgrade")]
        public async Task<IActionResult> UpgradePackage(int userId, int newPackageId)
        {
            var response = await _membershipPackageService.UpgradePackageAsync(userId, newPackageId);
            if (!response.IsSuccess)
            {
                return StatusCode((int)response.StatusCode, response);
            }

            return StatusCode((int)HttpStatusCode.OK, response.Result);
        }


        /// <summary>
        /// Mở rộng gói membership cho người dùng bằng cách thêm ngày
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="additionalDays"></param>
        /// <returns></returns>
        [HttpPost("extend")]
        public async Task<IActionResult> ExtendPackage(int userId, int additionalDays)
        {
            var response = await _membershipPackageService.ExtendPackageAsync(userId, additionalDays);
            if (!response.IsSuccess)
            {
                return StatusCode((int)response.StatusCode, response);
            }

            return StatusCode((int)HttpStatusCode.OK, response.Result);
        }



    }
}
