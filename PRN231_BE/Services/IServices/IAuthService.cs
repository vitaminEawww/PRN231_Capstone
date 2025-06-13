using System;
using DataAccess.Common;
using DataAccess.Models.Auth;

namespace Services.IServices;

public interface IAuthService
{
    Task<ApiResponse> RegisterCustomerAsync(RegisterCustomerRequestDTO request);
    Task<ApiResponse> RegisterCoachAsync(RegisterCoachRequestDTO request);
    Task<ApiResponse> LoginAsync(LoginRequestDTO request);
    Task<ApiResponse> GetUserProfileAsync(int userId);
}
