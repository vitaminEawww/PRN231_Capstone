using DataAccess.Common;
using DataAccess.Models.Plans;

namespace Services.IServices
{
    public interface IPlanService
    {
        Task<ApiResponse> GetAllPlansAsync();
        Task<ApiResponse> GetPlanByIdAsync(int planId);
        Task<ApiResponse> CreatePlanAsync(PlanCreateDTO planDto);
        Task<ApiResponse> UpdatePlanAsync(PlanUpdateDTO planDto);
        Task<ApiResponse> DeletePlanAsync(int planId);
        Task<ApiResponse> UpdatePlanProgressAsync(int planId, int phaseId, double progress);
    }
}