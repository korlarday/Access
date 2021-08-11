using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Allprimetech.BL;
using Allprimetech.GeneralUtils;
using Allprimetech.Interfaces;
using Allprimetech.Interfaces.Resources;
using Allprimetech.Interfaces.Resources.Partner;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SAP_Lite_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnersController : ControllerBase
    {
        #region Declarations
        public PartnersBL _PartnersBL { get; set; }
        public PartnersController(
            IPartnerRepository partnerRepository, 
            IMapper mapper, IUnitOfWork unitOfWork, ISystemAuditRepository auditRepository)
        {
            _PartnersBL = new PartnersBL(partnerRepository, mapper, unitOfWork, auditRepository);
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

        [Route("GetPartners")]
        [Authorize(Policy = "PartnerRead")]
        [HttpGet]
        public async Task<IActionResult> GetPartners()
        {
            try
            {
                var result = await Task.Run(() => _PartnersBL.AllPartners(true));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetPartners.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("GetPartner/{id}")]
        [Authorize(Policy = "PartnerRead")]
        [HttpGet]
        public async Task<IActionResult> GetPartner(int id)
        {
            try
            {
                var result = await Task.Run(() => _PartnersBL.GetPartner(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetPartner.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("UpdatePartner")]
        [Authorize(Policy = "PartnerUpdate")]
        [HttpPost]
        public async Task<IActionResult> UpdatePartner(CreatePartnerResource model)
        {
            IsValid();
            try
            {
                var updatedProduct = await Task.Run(() => _PartnersBL.UpdatePartner(GetUserId(), model));
                return GetResult(updatedProduct);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.UpdatePartner.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("CreatePartner")]
        [HttpPost]
        [Authorize(Policy = "PartnerCreate")]
        public async Task<IActionResult> CreatePartner(CreatePartnerResource model)
        {
            IsValid();

            try
            {
                var result = await Task.Run(() => _PartnersBL.CreatePartner(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.CreatePartner.ToString(), ex.Message);
                return GetResult(ex);
            }

        }

        [Route("ChangePartnerStatus")]
        [HttpPost]
        [Authorize(Policy = "PartnerUpdate")]
        public async Task<IActionResult> ChangePartnerStatus(ChangePartnerStatusResource model)
        {
            IsValid();

            try
            {
                var result = await Task.Run(() => _PartnersBL.ChangePartnerStatus(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.ChangePartnerStatus.ToString(), ex.Message);
                return GetResult(ex);
            }

        }

        [Route("DeletePartner/{id}")]
        [Authorize(Policy = "PartnerDelete")]
        [HttpPost]
        public async Task<IActionResult> DeletePartner(int id)
        {
            try
            {
                var result = await Task.Run(() => _PartnersBL.DeletePartner(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.DeletePartner.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("SearchPartner")]
        [Authorize(Policy = "PartnerRead")]
        [HttpPost]
        public async Task<IActionResult> SearchPartners(SearchResource model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _PartnersBL.SearchPartners(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.SearchPartners.ToString(), ex.Message);
                return GetResult(ex);
            }
        }
    }
}
