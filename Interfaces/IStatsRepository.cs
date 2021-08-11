using Allprimetech.Interfaces.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Allprimetech.Interfaces
{
    public interface IStatsRepository
    {
        Task<DashboardResource> DashboardStats(string userId, int customerId, int orderId);
    }
}
