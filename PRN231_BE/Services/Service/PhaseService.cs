using DataAccess.Common;
using DataAccess.Entities;
using DataAccess.Models.Plans;
using Mapster;
using Repositories.IRepositories;
using Services.IServices;

namespace Services.Service
{
    public class PhaseService(IUnitOfWork unitOfWork) : IPhaseService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<ApiResponse> GetAllPhasesAsync()
        {
            try
            {
                var phases = await _unitOfWork.Phases.GetAllAsync();
                var phaseDtos = phases.Adapt<List<PhaseDTO>>();
                return new ApiResponse { IsSuccess = true, Result = phaseDtos };
            }
            catch (Exception ex)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message] };
            }
        }

        public async Task<ApiResponse> GetPhaseByIdAsync(int phaseId)
        {
            try
            {
                var phase = await _unitOfWork.Phases.GetPhaseWithPlanAsync(phaseId);
                if (phase == null)
                    return new ApiResponse { IsSuccess = false, ErrorMessages = ["Phase not found"] };

                var phaseDto = phase.Adapt<PhaseDTO>();
                return new ApiResponse { IsSuccess = true, Result = phaseDto };
            }
            catch (Exception ex)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message] };
            }
        }

        public async Task<ApiResponse> GetPhasesByPlanIdAsync(int planId)
        {
            try
            {
                var phases = await _unitOfWork.Phases.GetPhasesByPlanIdAsync(planId);
                var phaseDtos = phases.Adapt<List<PhaseDTO>>();
                return new ApiResponse { IsSuccess = true, Result = phaseDtos };
            }
            catch (Exception ex)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message] };
            }
        }

        public async Task<ApiResponse> CreatePhaseAsync(PhaseCreateDTO phaseDto)
        {
            try
            {
                var phase = phaseDto.Adapt<Phase>();
                phase.CreatedDate = DateTime.UtcNow;
                phase.EndDate = phase.CreatedDate.AddDays(phase.DurationDays);
                phase.IsCompleted = 0;

                await _unitOfWork.Phases.AddAsync(phase);
                await _unitOfWork.SaveAsync();

                return new ApiResponse { IsSuccess = true, ErrorMessages = ["Phase created IsSuccessfully"] };
            }
            catch (Exception ex)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message] };
            }
        }

        public async Task<ApiResponse> UpdatePhaseAsync(PhaseUpdateDTO phaseDto)
        {
            try
            {
                var existingPhase = await _unitOfWork.Phases.GetByIdAsync(phaseDto.Id);
                if (existingPhase == null)
                    return new ApiResponse { IsSuccess = false, ErrorMessages = ["Phase not found"] };

                phaseDto.Adapt(existingPhase);
                _unitOfWork.Phases.Update(existingPhase);
                await _unitOfWork.SaveAsync();

                return new ApiResponse { IsSuccess = true, ErrorMessages = ["Phase updated IsSuccessfully"] };
            }
            catch (Exception ex)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message] };
            }
        }

        public async Task<ApiResponse> DeletePhaseAsync(int phaseId)
        {
            try
            {
                var phase = await _unitOfWork.Phases.GetByIdAsync(phaseId);
                if (phase == null)
                    return new ApiResponse { IsSuccess = false, ErrorMessages = ["Phase not found"] };

                _unitOfWork.Phases.Delete(phase);
                await _unitOfWork.SaveAsync();

                return new ApiResponse { IsSuccess = true, ErrorMessages = ["Phase deleted IsSuccessfully"] };
            }
            catch (Exception ex)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message] };
            }
        }

        public async Task<ApiResponse> UpdatePhaseProgressAsync(int phaseId, double progress)
        {
            try
            {
                var IsSuccess = await _unitOfWork.Phases.UpdatePhaseProgressAsync(phaseId, progress);
                if (!IsSuccess)
                    return new ApiResponse { IsSuccess = false, ErrorMessages = ["Phase not found"] };

                await _unitOfWork.SaveAsync();
                return new ApiResponse { IsSuccess = true, ErrorMessages = ["Phase progress updated IsSuccessfully"] };
            }
            catch (Exception ex)
            {
                return new ApiResponse { IsSuccess = false, ErrorMessages = [ex.Message] };
            }
        }
    }
}