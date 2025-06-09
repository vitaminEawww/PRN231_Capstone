using DataAccess.Entities;

namespace Repositories.IRepositories
{
    public interface IMembershipRepository : IGenericRepository<Membership>
    {
        Task<Membership> GetMembershipWithFeaturesAsync(int membershipId);
        Task<IEnumerable<Membership>> GetAllMembershipsWithFeaturesAsync();
        Task<bool> UpdateMembershipFeaturesAsync(int membershipId, List<int> featureIds);
    }
}