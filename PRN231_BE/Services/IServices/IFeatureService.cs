using DataAccess.Common;
using DataAccess.Models.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IFeatureService
    {
        Task<ApiResponse> GetAllAsync();
        Task<ApiResponse> GetByIdAsync(int id);

        Task<ApiResponse> GetPagedAsync(int pageNumber, int pageSize);

        Task<ApiResponse> CreateAsync(FeatureCreateDTO dto);
        Task<ApiResponse> UpdateAsync(FeatureUpdateDTO dto);
        Task<ApiResponse> DeleteAsync(int id);
    }
}
