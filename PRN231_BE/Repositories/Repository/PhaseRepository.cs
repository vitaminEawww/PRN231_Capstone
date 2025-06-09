using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.IRepositories;

namespace Repositories.Repository
{
    public class PhaseRepository(ApplicationDbContext context) : GenericRepository<Phase>(context), IPhaseRepository
    {
        public async Task<Phase> GetPhaseWithPlanAsync(int phaseId)
        {
            return await _dbSet
                .Include(p => p.Plan)
                .FirstOrDefaultAsync(p => p.Id == phaseId);
        }

        public async Task<IEnumerable<Phase>> GetPhasesByPlanIdAsync(int planId)
        {
            return await _dbSet
                .Include(p => p.Plan)
                .Where(p => p.PlanId == planId)
                .ToListAsync();
        }

        public async Task<bool> UpdatePhaseProgressAsync(int phaseId, double progress)
        {
            var phase = await _dbSet.FindAsync(phaseId);
            if (phase == null)
                return false;

            phase.IsCompleted = progress;
            _dbSet.Update(phase);
            return true;
        }
    }
}