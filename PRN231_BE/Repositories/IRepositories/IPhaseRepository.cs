using DataAccess.Entities;

namespace Repositories.IRepositories
{
    public interface IPhaseRepository : IGenericRepository<Phase>
    {
        Task<Phase> GetPhaseWithPlanAsync(int phaseId);
        Task<IEnumerable<Phase>> GetPhasesByPlanIdAsync(int planId);
        Task<bool> UpdatePhaseProgressAsync(int phaseId, double progress);
    }
}