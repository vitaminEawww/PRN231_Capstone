using DataAccess.Entities;

namespace Repositories.IRepositories
{
    public interface IFeatureRepository : IGenericRepository<Feature>
    {
        Task<IEnumerable<Feature>> GetFeaturesByMembershipIdAsync(int membershipId);
        Task<bool> AddFeatureToMembershipAsync(int membershipId, int featureId);
        Task<bool> RemoveFeatureFromMembershipAsync(int membershipId, int featureId);
    }
}