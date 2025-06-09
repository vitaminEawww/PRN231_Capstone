using DataAccess.Entities;

namespace Repositories.IRepositories
{
    public interface IPlanRepository : IGenericRepository<Plan>
    {
        Task<Plan> GetPlanWithPhasesAsync(int planId);
        Task<IEnumerable<Plan>> GetAllPlansWithPhasesAsync();
        Task<bool> UpdatePlanPhasesAsync(int planId, List<Phase> phases);
    }
}