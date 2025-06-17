using DataAccess.Common;
using DataAccess.Models.MembershipPackage;
using DataAccess.Models.MemberShipPackage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IMembershipPackageService
    {
        Task<ApiResponse> GetAllMembershipPackagesAsync();
        Task<ApiResponse> GetMembershipPackageByIdAsync(int id);

        Task<ApiResponse> GetPagedMembershipPackagesAsync(int pageNumber, int pageSize);

        Task<ApiResponse> CreateMembershipPackageAsync( CreateMembershipPackageDTO packageDto);

        Task<ApiResponse> UpdateMembershipPackageAsync(int id, UpdateMembershipPackageDTO packageDto);


        Task<ApiResponse> DeleteMembershipPackageAsync(int id);

    }
}
