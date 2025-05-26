using DataAccess.Common;
using DataAccess.Models.MemberShips;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IMembershipService
    {
        Task<ApiResponse> GetAllAsync();
        Task<ApiResponse> GetByIdAsync(int id);
        Task<ApiResponse> GetPagedAsync(int pageNumber, int pageSize);
        Task<ApiResponse> CreateAsync(MembershipCreateDTO dto);
        Task<ApiResponse> UpdateAsync(MemberShipUpdateDTO dto);
        Task<ApiResponse> DeleteAsync(int id);
    }
}
