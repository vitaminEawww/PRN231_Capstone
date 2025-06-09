using DataAccess.Common;
using DataAccess.Models.Features;

namespace Services.IServices
{
    public interface IFeatureService
    {
        Task<ApiResponse> GetAllFeaturesAsync();
        Task<ApiResponse> GetFeatureByIdAsync(int featureId);
        Task<ApiResponse> GetFeaturesByMembershipIdAsync(int membershipId);
        Task<ApiResponse> CreateFeatureAsync(FeatureCreateDTO featureDto);
        Task<ApiResponse> UpdateFeatureAsync(FeatureUpdateDTO featureDto);
        Task<ApiResponse> DeleteFeatureAsync(int featureId);
        Task<ApiResponse> AddFeatureToMembershipAsync(MemFeatureCreateDTO memFeatureDto);
        Task<ApiResponse> RemoveFeatureFromMembershipAsync(int membershipId, int featureId);
    }
}