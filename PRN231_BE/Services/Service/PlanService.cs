using DataAccess.Common;
using DataAccess.Entities;
using DataAccess.Models.Plans;
using Mapster;
using Repositories.IRepositories;
using Services.IServices;

namespace Services.Service
{
    public class PlanService(IUnitOfWork unitOfWork) : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ApiResponse> GetAllPlansAsync()
        {
            try
            {
                var plans = await _unitOfWork.Plans.GetAllPlansWithPhasesAsync();
                var planDtos = plans.Adapt<List<PlanDTO>>();
                return new ApiResponse { IsSuccess = true, Result = planDtos };
            }
            catch (Exception ex)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message] };
            }
        }

        public async Task<ApiResponse> GetPlanByIdAsync(int planId)
        {
            try
            {
                var plan = await _unitOfWork.Plans.GetPlanWithPhasesAsync(planId);
                if (plan == null)
                    return new ApiResponse { IsSuccess = false, ErrorMessages = ["Plan not found"] };

                var planDto = plan.Adapt<PlanDTO>();
                return new ApiResponse { IsSuccess = true, Result = planDto };
            }
            catch (Exception ex)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message] };
            }
        }

        public async Task<ApiResponse> CreatePlanAsync(PlanCreateDTO planDto)
        {
            try
            {
                var plan = planDto.Adapt<Plan>();
                
                // Set required default values after mapping
                plan.CreatedDate = DateTime.UtcNow;
                plan.EndDate = DateTime.UtcNow.AddDays(plan.DurationDays);
                plan.IsActive = true;
                
                await _unitOfWork.Plans.AddAsync(plan);
                await _unitOfWork.SaveAsync();

                if (planDto.Phases != null && planDto.Phases.Any())
                {
                    var phases = planDto.Phases.Adapt<List<Phase>>();
                    await _unitOfWork.Plans.UpdatePlanPhasesAsync(plan.Id, phases);
                    await _unitOfWork.SaveAsync();
                }

                return new ApiResponse { IsSuccess = true, ErrorMessages = ["Plan created IsSuccessfully"] };
            }
            catch (Exception ex)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message] };
            }
        }

        public async Task<ApiResponse> UpdatePlanAsync(PlanUpdateDTO planDto)
        {
            try
            {
                var existingPlan = await _unitOfWork.Plans.GetByIdAsync(planDto.Id);
                if (existingPlan == null)
                    return new ApiResponse { IsSuccess = false, ErrorMessages = ["Plan not found"] };

                planDto.Adapt(existingPlan);
                _unitOfWork.Plans.Update(existingPlan);

                if (planDto.Phases != null)
                {
                    var phases = planDto.Phases.Adapt<List<Phase>>();
                    await _unitOfWork.Plans.UpdatePlanPhasesAsync(planDto.Id, phases);
                }

                await _unitOfWork.SaveAsync();
                return new ApiResponse { IsSuccess = true, ErrorMessages = ["Plan updated IsSuccessfully"] };
            }
            catch (Exception ex)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message] };
            }
        }

        public async Task<ApiResponse> DeletePlanAsync(int planId)
        {
            try
            {
                var plan = await _unitOfWork.Plans.GetByIdAsync(planId);
                if (plan == null)
                    return new ApiResponse { IsSuccess = false, ErrorMessages = ["Plan not found"] };

                _unitOfWork.Plans.Delete(plan);
                await _unitOfWork.SaveAsync();
                return new ApiResponse { IsSuccess = true, ErrorMessages = ["Plan deleted IsSuccessfully"] };
            }
            catch (Exception ex)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message] };
            }
        }

        public async Task<ApiResponse> UpdatePlanProgressAsync(int planId, int phaseId, double progress)
        {
            try
            {
                var plan = await _unitOfWork.Plans.GetPlanWithPhasesAsync(planId);
                if (plan == null)
                    return new ApiResponse { IsSuccess = false, ErrorMessages = ["Plan not found"] };

                var phase = plan.Phases.FirstOrDefault(p => p.Id == phaseId);
                if (phase == null)
                    return new ApiResponse { IsSuccess = false, ErrorMessages = ["Phase not found"] };

                phase.IsCompleted = progress;
                _unitOfWork.Plans.Update(plan);
                await _unitOfWork.SaveAsync();

                return new ApiResponse { IsSuccess = true, ErrorMessages = ["Plan progress updated IsSuccessfully"] };
            }
            catch (Exception ex)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message] };
            }
        }
    }
}