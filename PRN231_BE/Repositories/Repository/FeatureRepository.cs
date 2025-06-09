using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.IRepositories;

namespace Repositories.Repository
{
    public class FeatureRepository(ApplicationDbContext context) : GenericRepository<Feature>(context), IFeatureRepository
    {
        public async Task<IEnumerable<Feature>> GetFeaturesByMembershipIdAsync(int membershipId)
        {
            return await _dbSet
                .Include(f => f.MemFeatures)
                .Where(f => f.MemFeatures.Any(mf => mf.MembershipId == membershipId))
                .ToListAsync();
        }

        public async Task<bool> AddFeatureToMembershipAsync(int membershipId, int featureId)
        {
            var membership = await _context.Set<Membership>().FindAsync(membershipId);
            var feature = await _dbSet.FindAsync(featureId);

            if (membership == null || feature == null)
                return false;

            var memFeature = new MemFeature
            {
                MembershipId = membershipId,
                FeatureId = featureId
            };

            await _context.Set<MemFeature>().AddAsync(memFeature);
            return true;
        }

        public async Task<bool> RemoveFeatureFromMembershipAsync(int membershipId, int featureId)
        {
            var memFeature = await _context.Set<MemFeature>()
                .FirstOrDefaultAsync(mf => mf.MembershipId == membershipId && mf.FeatureId == featureId);

            if (memFeature == null)
                return false;

            _context.Set<MemFeature>().Remove(memFeature);
            return true;
        }
    }
}