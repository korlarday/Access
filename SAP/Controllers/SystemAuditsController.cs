using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Allprimetech.BL;
using Allprimetech.GeneralUtils;
using Allprimetech.Interfaces;
using Allprimetech.Interfaces.Resources;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SAP_Lite_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemAuditsController : ControllerBase
    {
        #region Declarations
        public SystemAuditsBL _SystemAuditBL { get; set; }
        public SystemAuditsController(
            ISystemAuditRepository systemAuditRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _SystemAuditBL = new SystemAuditsBL(systemAuditRepository, mapper, unitOfWork);
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


        #region SystemAudits Endpoints
        [Route("GetSystemAudits")]
        [Authorize(Policy = "SystemAuditRead")]
        [HttpGet]
        public async Task<IActionResult> GetSystemAudits()
        {
            try
            {
                var result = await Task.Run(() => _SystemAuditBL.AllSystemAudits(GetUserId()));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetSystemAudits.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("GetSystemAudit/{id}")]
        [Authorize(Policy = "SystemAuditRead")]
        [HttpGet]
        public async Task<IActionResult> GetSystemAudit(int id)
        {
            try
            {
                var result = await Task.Run(() => _SystemAuditBL.GetSystemAudit(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetSystemAudit.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        //[Route("UpdateSystemAudit/{id}")]
        //[Authorize(Policy = "SystemAuditUpdate")]
        //[HttpPost]
        //public async Task<IActionResult> UpdateSystemAudit(int id, CreateSystemAuditResource model)
        //{
        //    IsValid();
        //    try
        //    {
        //        var updatedProduct = await Task.Run(() => _SystemAuditBL.UpdateSystemAudit(GetUserId(), id, model));
        //        return GetResult(updatedProduct);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.UpdateSystemAudit.ToString(), ex.Message);
        //        return GetResult(ex);
        //    }
        //}

        //[Route("CreateSystemAudit")]
        //[HttpPost]
        //[Authorize(Policy = "SystemAuditCreate")]
        //public async Task<IActionResult> CreateSystemAudit(CreateSystemAuditResource model)
        //{
        //    IsValid();

        //    try
        //    {
        //        var result = await Task.Run(() => _SystemAuditBL.CreateSystemAudit(GetUserId(), model));
        //        return GetResult(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.CreateSystemAudit.ToString(), ex.Message);
        //        return GetResult(ex);
        //    }

        //}

        //[Route("DeleteSystemAudit")]
        //[Authorize(Policy = "SystemAuditDelete")]
        //[HttpPost("{id}")]
        //public async Task<IActionResult> DeleteSystemAudit(int id)
        //{
        //    try
        //    {
        //        var result = await Task.Run(() => _SystemAuditBL.DeleteSystemAudit(GetUserId(), id));
        //        return GetResult(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.DeleteSystemAudit.ToString(), ex.Message);
        //        return GetResult(ex);
        //    }
        //}


        [Route("SearchSystemAudit")]
        [Authorize(Policy = "OrderRead")]
        [HttpPost]
        public async Task<IActionResult> SearchSystemAudits(SystemAuditSearchResouce model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _SystemAuditBL.SearchSystemAudits(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.SearchSystemAudits.ToString(), ex.Message);
                return GetResult(ex);
            }
        }


        #endregion


    }
}
