using DataAccess.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IMembershipUsage
    {
        Task<ApiResponse> GetMemberShipUsageAsync(int userId);
        Task<ApiResponse> GetMyMembershipStatusAsync(int customerId);
    }
}
