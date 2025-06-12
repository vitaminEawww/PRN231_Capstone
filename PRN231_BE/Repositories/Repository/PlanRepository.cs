using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.IRepositories;

namespace Repositories.Repository
{
    public class PlanRepository(ApplicationDbContext context) : GenericRepository<Plan>(context), IPlanRepository
    {
        public async Task<Plan> GetPlanWithPhasesAsync(int planId)
        {
            return await _dbSet
                .Include(p => p.Phases)
                .FirstOrDefaultAsync(p => p.Id == planId);
        }

        public async Task<IEnumerable<Plan>> GetAllPlansWithPhasesAsync()
        {
            return await _dbSet
                .Include(p => p.Phases)
                .ToListAsync();
        }

        public async Task<bool> UpdatePlanPhasesAsync(int planId, List<Phase> phases)
        {
            var plan = await _dbSet
                .Include(p => p.Phases)
                .FirstOrDefaultAsync(p => p.Id == planId);

            if (plan == null)
                return false;

            // Remove existing phases
            var existingPhases = plan.Phases.ToList();
            foreach (var phase in existingPhases)
            {
                _context.Set<Phase>().Remove(phase);
            }

            // Add new phases
            foreach (var phase in phases)
            {
                phase.PlanId = planId;
                phase.CreatedDate = DateTime.UtcNow;
                phase.EndDate = phase.CreatedDate.AddDays(phase.DurationDays);
                phase.IsCompleted = 0; // Set default progress
                await _context.Set<Phase>().AddAsync(phase);
            }

            return true;
        }
    }
}