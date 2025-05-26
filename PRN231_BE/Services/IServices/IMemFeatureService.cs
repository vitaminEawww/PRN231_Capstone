using DataAccess.Common;
using DataAccess.Models.MemFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IMemFeatureService
    {
        Task<ApiResponse> GetAllAsync();
        Task<ApiResponse> GetByMembershipIdAsync(int membershipId);
        Task<ApiResponse> CreateAsync(MemFeatureCreateDTO dto);
        Task<ApiResponse> DeleteAsync(int id);
    }
}
