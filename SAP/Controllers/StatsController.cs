using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Allprimetech.BL;
using Allprimetech.GeneralUtils;
using Allprimetech.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SAP_Lite_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        #region Declarations
        public StatsBL _StatsBL { get; set; }
        public StatsController(IStatsRepository statsRepository)
        {
            _StatsBL = new StatsBL(statsRepository);
        }
        #endregion

        #region private methods
        private IActionResult GetResult(object response)
        {
            if (response == null)
                return NotFound();

            return Ok(response);
        }

        private string GetUserId()
        {
            return User.Claims.First(c => c.Type == "UserId").Value;
        }

        private IActionResult IsValid()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return null;
        }
        #endregion

        [Route("DashboardStats/{id}/{orderId}")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetDashboardStats(int id, int orderId)
        {
            try
            {
                var result = await Task.Run(() => _StatsBL.GetDashboardStats(GetUserId(), id, orderId));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetDashboardStats.ToString(), ex.Message);
                return GetResult(ex);
            }
        }
    }
}
