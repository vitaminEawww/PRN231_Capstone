using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Models.Conversation;
using DataAccess.Models.Message;
using DataAccess.Common;
using System.Security.Claims;
using System.Collections.Generic;
using System.Net;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationService _conversationService;

        public ConversationController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        // GET /api/conversations - Lấy danh sách conversation của user
        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetUserConversations()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                if (userId == null || role == null)
                {
                    return Unauthorized(new ApiResponse
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        IsSuccess = false,
                        ErrorMessages = new List<string> { "User is not authenticated." }
                    });
                }
                var result = await _conversationService.GetUserConversationsAsync(int.Parse(userId), role);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Result = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message }
                });
            }
        }

        // GET /api/conversations/{id}/messages - Lấy tin nhắn của 1 conversation
        [HttpGet("{id}/messages")]
        public async Task<ActionResult<ApiResponse>> GetMessagesByConversationId(int id)
        {
            try
            {
                var result = await _conversationService.GetMessagesByConversationIdAsync(id);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Result = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message }
                });
            }
        }

        // POST /api/messages - Lưu tin nhắn mới
        [HttpPost("/api/messages")]
        public async Task<ActionResult<ApiResponse>> CreateMessage([FromBody] CreateMessageDTO dto)
        {
            try
            {
                var result = await _conversationService.CreateMessageAsync(dto);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Result = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message }
                });
            }
        }

        // GET /api/conversations/by-consultation/{consultationId}
        [HttpGet("by-consultation/{consultationId}")]
        public async Task<ActionResult<ApiResponse>> GetOrCreateConversationByConsultation(int consultationId)
        {
            try
            {
                var result = await _conversationService.GetOrCreateConversationByConsultationAsync(consultationId);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Result = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message }
                });
            }
        }
    }
}
 