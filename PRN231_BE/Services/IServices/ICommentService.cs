using DataAccess.Common;
using DataAccess.Models.Comments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface ICommentService
    {
        Task<ApiResponse> GetAllAsync();
        Task<ApiResponse> GetByIdAsync(int id);
        Task<ApiResponse> GetByPostIdAsync(int postId);
        Task<ApiResponse> CreateAsync(CommentCreateDTO dto);
        Task<ApiResponse> UpdateAsync(CommentUpdateDTO dto);
        Task<ApiResponse> DeleteAsync(int id);
    }
}
