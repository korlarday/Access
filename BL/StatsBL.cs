using Allprimetech.Interfaces;
using Allprimetech.Interfaces.Resources;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Allprimetech.BL
{
    public class StatsBL
    { 
        #region Declarations
        private IStatsRepository _StatsRepository { get; set; }

        public StatsBL(IStatsRepository statsRepository)
        {
            _StatsRepository = statsRepository;
        }
        #endregion

        public async Task<DashboardResource> GetDashboardStats(string userId, int customerId, int orderId)
        {
            var stats = await _StatsRepository.DashboardStats(userId, customerId, orderId); 
            return stats;
        }

    }
}
