using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.IRepositories;

namespace Repositories.Repository
{
    public class MembershipRepository(ApplicationDbContext context) : GenericRepository<Membership>(context), IMembershipRepository
    {
        public async Task<Membership> GetMembershipWithFeaturesAsync(int membershipId)
        {
            return await _dbSet
                .Include(m => m.MemFeatures)
                    .ThenInclude(mf => mf.Feature)
                .FirstOrDefaultAsync(m => m.PlanID == membershipId);
        }

        public async Task<IEnumerable<Membership>> GetAllMembershipsWithFeaturesAsync()
        {
            return await _dbSet
                .Include(m => m.MemFeatures)
                    .ThenInclude(mf => mf.Feature)
                .ToListAsync();
        }

        public async Task<bool> UpdateMembershipFeaturesAsync(int membershipId, List<int> featureIds)
        {
            var membership = await _dbSet
                .Include(m => m.MemFeatures)
                .FirstOrDefaultAsync(m => m.PlanID == membershipId);

            if (membership == null)
                return false;

            // Remove existing features
            var existingFeatures = membership.MemFeatures.ToList();
            foreach (var feature in existingFeatures)
            {
                _context.Set<MemFeature>().Remove(feature);
            }

            // Add new features
            foreach (var featureId in featureIds)
            {
                await _context.Set<MemFeature>().AddAsync(new MemFeature
                {
                    MembershipId = membershipId,
                    FeatureId = featureId
                });
            }

            return true;
        }
    }
}