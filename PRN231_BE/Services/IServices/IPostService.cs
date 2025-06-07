using DataAccess.Common;
using DataAccess.Models.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IPostService
    {
        Task<ApiResponse> GetAllAsync();
        Task<ApiResponse> GetByIdAsync(int id);
        Task<ApiResponse> CreateAsync(PostCreateDTO dto);
        Task<ApiResponse> UpdateAsync(PostUpdateDTO dto);
        Task<ApiResponse> DeleteAsync(int id);
    }
}
