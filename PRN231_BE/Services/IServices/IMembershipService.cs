using DataAccess.Common;
using DataAccess.Models.Memberships;

namespace Services.IServices
{
    public interface IMembershipService
    {
        Task<ApiResponse> GetAllMembershipsAsync();
        Task<ApiResponse> GetMembershipByIdAsync(int membershipId);
        Task<ApiResponse> CreateMembershipAsync(MembershipCreateDTO membershipDto);
        Task<ApiResponse> UpdateMembershipAsync(MembershipUpdateDTO membershipDto);
        Task<ApiResponse> DeleteMembershipAsync(int membershipId);
    }
}