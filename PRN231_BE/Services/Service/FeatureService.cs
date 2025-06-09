using DataAccess.Common;
using DataAccess.Entities;
using DataAccess.Models.Features;
using Mapster;
using Repositories.IRepositories;
using Services.IServices;

namespace Services.Service
{
    public class FeatureService(IUnitOfWork unitOfWork) : IFeatureService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ApiResponse> GetAllFeaturesAsync()
        {
            try
            {
                var features = await _unitOfWork.Features.GetAllAsync();
                var featureDtos = features.Adapt<List<FeatureDTO>>();
                return new ApiResponse { IsSuccess = true, Result = featureDtos };
            }
            catch (Exception ex)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message] };
            }
        }

        public async Task<ApiResponse> GetFeatureByIdAsync(int featureId)
        {
            try
            {
                var feature = await _unitOfWork.Features.GetByIdAsync(featureId);
                if (feature == null)
                    return new ApiResponse { IsSuccess = false, ErrorMessages = ["Feature not found"] };

                var featureDto = feature.Adapt<FeatureDTO>();
                return new ApiResponse { IsSuccess = true, Result = featureDto };
            }
            catch (Exception ex)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message] };
            }
        }

        public async Task<ApiResponse> GetFeaturesByMembershipIdAsync(int membershipId)
        {
            try
            {
                var features = await _unitOfWork.Features.GetFeaturesByMembershipIdAsync(membershipId);
                var featureDtos = features.Adapt<List<FeatureDTO>>();
                return new ApiResponse { IsSuccess = true, Result = featureDtos };
            }
            catch (Exception ex)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message] };
            }
        }

        public async Task<ApiResponse> CreateFeatureAsync(FeatureCreateDTO featureDto)
        {
            try
            {
                var feature = featureDto.Adapt<Feature>();
                await _unitOfWork.Features.AddAsync(feature);
                await _unitOfWork.SaveAsync();
                return new ApiResponse { IsSuccess = true, ErrorMessages = ["Feature created IsSuccessfully"] };
            }
            catch (Exception ex)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message] };
            }
        }

        public async Task<ApiResponse> UpdateFeatureAsync(FeatureUpdateDTO featureDto)
        {
            try
            {
                var existingFeature = await _unitOfWork.Features.GetByIdAsync(featureDto.Id);
                if (existingFeature == null)
                    return new ApiResponse { IsSuccess = false, ErrorMessages = ["Feature not found"] };

                featureDto.Adapt(existingFeature);
                _unitOfWork.Features.Update(existingFeature);
                await _unitOfWork.SaveAsync();
                return new ApiResponse { IsSuccess = true, ErrorMessages = ["Feature updated IsSuccessfully"] };
            }
            catch (Exception ex)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message] };
            }
        }

        public async Task<ApiResponse> DeleteFeatureAsync(int featureId)
        {
            try
            {
                var feature = await _unitOfWork.Features.GetByIdAsync(featureId);
                if (feature == null)
                    return new ApiResponse { IsSuccess = false, ErrorMessages = ["Feature not found"] };

                _unitOfWork.Features.Delete(feature);
                await _unitOfWork.SaveAsync();
                return new ApiResponse { IsSuccess = true, ErrorMessages = ["Feature deleted IsSuccessfully"] };
            }
            catch (Exception ex)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message] };
            }
        }

        public async Task<ApiResponse> AddFeatureToMembershipAsync(MemFeatureCreateDTO memFeatureDto)
        {
            try
            {
                var IsSuccess = await _unitOfWork.Features.AddFeatureToMembershipAsync(
                    memFeatureDto.MembershipId,
                    memFeatureDto.FeatureId);

                if (!IsSuccess)
                    return new ApiResponse { IsSuccess = false, ErrorMessages = ["Failed to add feature to membership"] };

                await _unitOfWork.SaveAsync();
                return new ApiResponse { IsSuccess = true, ErrorMessages = ["Feature added to membership IsSuccessfully"] };
            }
            catch (Exception ex)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message] };
            }
        }

        public async Task<ApiResponse> RemoveFeatureFromMembershipAsync(int membershipId, int featureId)
        {
            try
            {
                var IsSuccess = await _unitOfWork.Features.RemoveFeatureFromMembershipAsync(membershipId, featureId);
                if (!IsSuccess)
                    return new ApiResponse { IsSuccess = false, ErrorMessages = ["Failed to remove feature from membership"] };

                await _unitOfWork.SaveAsync();
                return new ApiResponse { IsSuccess = true, ErrorMessages = ["Feature removed from membership IsSuccessfully"] };
            }
            catch (Exception ex)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message] };
            }
        }
    }
}