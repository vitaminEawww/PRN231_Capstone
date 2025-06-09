using DataAccess.Common;
using DataAccess.Models.Users;

namespace Services.IServices;

public interface IUserService
{
    Task<ApiResponse> GetAllUserAsync(int pageNumber, int pageSize);
    Task<ApiResponse> GetUserByIdAsync(int userId);
    Task<ApiResponse> CreateUserAsync(UserCreateDTO userDto);
    Task<ApiResponse> UpdateUserAsync(UserUpdateDTO userDto);
    Task<ApiResponse> DeleteUserAsync(int userId);
    Task<ApiResponse> LoginAsync(string email, string password);
}
