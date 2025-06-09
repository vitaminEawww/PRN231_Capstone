using DataAccess.Common;
using DataAccess.Entities;
using DataAccess.Models.Memberships;
using Mapster;
using Repositories.IRepositories;
using Services.IServices;

namespace Services.Service
{
    public class MembershipService(IUnitOfWork unitOfWork) : IMembershipService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ApiResponse> GetAllMembershipsAsync()
        {
            try
            {
                var memberships = await _unitOfWork.Memberships.GetAllMembershipsWithFeaturesAsync();
                var membershipDtos = memberships.Adapt<List<MembershipDTO>>();
                return new ApiResponse { IsSuccess = true, Result = membershipDtos };
            }
            catch (Exception ex)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message] };
            }
        }

        public async Task<ApiResponse> GetMembershipByIdAsync(int membershipId)
        {
            try
            {
                var membership = await _unitOfWork.Memberships.GetMembershipWithFeaturesAsync(membershipId);
                if (membership == null)
                    return new ApiResponse { IsSuccess = false, ErrorMessages = ["Membership not found"] };

                var membershipDto = membership.Adapt<MembershipDTO>();
                return new ApiResponse { IsSuccess = true, Result = membershipDto };
            }
            catch (Exception ex)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message] };
            }
        }

        public async Task<ApiResponse> CreateMembershipAsync(MembershipCreateDTO membershipDto)
        {
            try
            {
                var membership = membershipDto.Adapt<Membership>();
                await _unitOfWork.Memberships.AddAsync(membership);
                await _unitOfWork.SaveAsync();

                if (membershipDto.FeatureIds != null && membershipDto.FeatureIds.Any())
                {
                    await _unitOfWork.Memberships.UpdateMembershipFeaturesAsync(membership.PlanID, membershipDto.FeatureIds);
                    await _unitOfWork.SaveAsync();
                }

                return new ApiResponse { IsSuccess = true, ErrorMessages = ["Membership created IsSuccessfully"] };
            }
            catch (Exception ex)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message] };
            }
        }

        public async Task<ApiResponse> UpdateMembershipAsync(MembershipUpdateDTO membershipDto)
        {
            try
            {
                var existingMembership = await _unitOfWork.Memberships.GetByIdAsync(membershipDto.PlanID);
                if (existingMembership == null)
                    return new ApiResponse { IsSuccess = false, ErrorMessages = ["Membership not found"] };

                membershipDto.Adapt(existingMembership);
                _unitOfWork.Memberships.Update(existingMembership);

                if (membershipDto.FeatureIds != null)
                {
                    await _unitOfWork.Memberships.UpdateMembershipFeaturesAsync(membershipDto.PlanID, membershipDto.FeatureIds);
                }

                await _unitOfWork.SaveAsync();
                return new ApiResponse { IsSuccess = true, ErrorMessages = ["Membership updated IsSuccessfully"] };
            }
            catch (Exception ex)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message] };
            }
        }

        public async Task<ApiResponse> DeleteMembershipAsync(int membershipId)
        {
            try
            {
                var membership = await _unitOfWork.Memberships.GetByIdAsync(membershipId);
                if (membership == null)
                    return new ApiResponse { IsSuccess = false, ErrorMessages = ["Membership not found"] };

                _unitOfWork.Memberships.Delete(membership);
                await _unitOfWork.SaveAsync();
                return new ApiResponse { IsSuccess = true, ErrorMessages = ["Membership deleted IsSuccessfully"] };
            }
            catch (Exception ex)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message] };
            }
        }
    }
}