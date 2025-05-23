using DataAccess.Common;
using DataAccess.Models.Plans;

namespace Services.IServices
{
    public interface IPhaseService
    {
        Task<ApiResponse> GetAllPhasesAsync();
        Task<ApiResponse> GetPhaseByIdAsync(int phaseId);
        Task<ApiResponse> GetPhasesByPlanIdAsync(int planId);
        Task<ApiResponse> CreatePhaseAsync(PhaseCreateDTO phaseDto);
        Task<ApiResponse> UpdatePhaseAsync(PhaseUpdateDTO phaseDto);
        Task<ApiResponse> DeletePhaseAsync(int phaseId);
        Task<ApiResponse> UpdatePhaseProgressAsync(int phaseId, double progress);
    }
}