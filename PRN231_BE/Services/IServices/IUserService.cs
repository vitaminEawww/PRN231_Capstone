using DataAccess.Common;
using DataAccess.Entities;
using DataAccess.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IUserService
    {
        Task<ApiResponse> GetAllUserAsync(int pageNumber, int pageSize);
        Task<ApiResponse> GetUserByIdAsync(int userId);
        Task<ApiResponse> CreateUserAsync(UserCreateDTO userDto);

        Task<ApiResponse> UpdateUserAsync(UserUpdateDTO userDto);
        Task DeleteUserAsync(int userId);
    }
}
