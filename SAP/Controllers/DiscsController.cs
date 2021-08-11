using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Allprimetech.BL;
using Allprimetech.GeneralUtils;
using Allprimetech.Interfaces;
using Allprimetech.Interfaces.Constants;
using Allprimetech.Interfaces.ControllerCodes;
using Allprimetech.Interfaces.Resources;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SAP_Lite_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscsController : ControllerBase
    {
        #region Declarations
        public DiscsBL _DiscsBL { get; set; }
        public DiscsController(
            IDiscRepository discRepository,
            IMapper mapper, IUnitOfWork unitOfWork, ISystemAuditRepository auditRepository)
        {
            _DiscsBL = new DiscsBL(discRepository, mapper, unitOfWork, auditRepository);
        }
        #endregion

        #region private methods
        private IActionResult GetResult(object response)
        {
            DISC_ERROR status = DiscControllerStatus._Error;
            DiscControllerStatus._Error = DISC_ERROR.NO_ERROR;

            switch (status)
            {
                case DISC_ERROR.BAD_REQUEST:
                    return BadRequest(new TransactionStatus
                    {
                        _ErrorCode = (int)DISC_ERROR.BAD_REQUEST,
                        _Message = StringStore.BadRequest
                    });
                case DISC_ERROR.EXISTS:
                    return BadRequest(new TransactionStatus
                    {
                        _ErrorCode = (int)DISC_ERROR.EXISTS,
                        _Message = StringStore.UserExistsResponse
                    });

                case DISC_ERROR.NOT_FOUND:
                    return BadRequest(new TransactionStatus
                    {
                        _ErrorCode = (int)DISC_ERROR.NOT_FOUND,
                        _Message = StringStore.NotFound
                    });

                case DISC_ERROR.EXCEPTION:
                    return StatusCode(StatusCodes.Status500InternalServerError, new TransactionStatus
                    {
                        _ErrorCode = (int)DISC_ERROR.EXCEPTION,
                        _Message = StringStore.ExceptionMessage
                    });

                case DISC_ERROR.NO_ERROR:
                    if (response != null)
                    {
                        return Ok(response);
                    }
                    else
                    {
                        return Ok(new TransactionStatus
                        {
                            _ErrorCode = (int)DISC_ERROR.NO_ERROR,
                            _Message = StringStore.SuccessExecutionMessage
                        });
                    }

                default:
                    return BadRequest();
            }

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

        [Route("GetDiscs/{id}")]
        [Authorize(Policy = "DiscRead")]
        [HttpGet]
        public async Task<IActionResult> GetDiscs(int id)
        {
            try
            {
                var result = await Task.Run(() => _DiscsBL.GetDiscs(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetDiscs.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("GetDisc/{id}")]
        [Authorize(Policy = "DiscRead")]
        [HttpGet]
        public async Task<IActionResult> GetDisc(int id)
        {
            try
            {
                var result = await Task.Run(() => _DiscsBL.GetDisc(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetDisc.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("UpdateDisc/{id}")]
        [Authorize(Policy = "DiscUpdate")]
        [HttpPost]
        public async Task<IActionResult> UpdateDisc(int id, CreateDiscResource model)
        {
            IsValid();
            try
            {
                var updatedProduct = await Task.Run(() => _DiscsBL.UpdateDisc(GetUserId(), id, model));
                return GetResult(updatedProduct);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.UpdateDisc.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("CreateDisc")]
        [HttpPost]
        [Authorize(Policy = "DiscCreate")]
        public async Task<IActionResult> CreateDisc(CreateDiscResource model)
        {
            IsValid();

            try
            {
                var result = await Task.Run(() => _DiscsBL.CreateDisc(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.CreateDisc.ToString(), ex.Message);
                return GetResult(ex);
            }

        }

        [Route("CreateDiscs")]
        [HttpPost]
        [Authorize(Policy = "DiscCreate")]
        public async Task<IActionResult> CreateDiscs(List<CreateDiscResource> model)
        {
            IsValid();

            try
            {
                var result = await Task.Run(() => _DiscsBL.CreateDiscs(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.CreateDisc.ToString(), ex.Message);
                return GetResult(ex);
            }

        }


        [Route("DeleteDisc/{id}")]
        [Authorize(Policy = "DiscDelete")]
        [HttpPost]
        public async Task<IActionResult> DeleteDisc(int id)
        {
            try
            {
                var result = await Task.Run(() => _DiscsBL.DeleteDisc(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.DeleteDisc.ToString(), ex.Message);
                return GetResult(ex);
            }
        }


        [Route("GetGroupsInfo/{id}")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetGroupsInfo(int id)
        {
            try
            {
                var result = await Task.Run(() => _DiscsBL.GetGroupsInfo(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetDiscs.ToString(), ex.Message);
                return GetResult(ex);
            }
        }


        [Route("GetGroupsInfoBruckner/{id}")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetGroupsInfoBruckner(int id)
        {
            try
            {
                var result = await Task.Run(() => _DiscsBL.GetGroupsInfoBruckner(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetDiscs.ToString(), ex.Message);
                return GetResult(ex);
            }
        }



        [Route("GetDiscsInfo/{id}")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetDiscsInfo(int id)
        {
            try
            {
                var result = await Task.Run(() => _DiscsBL.GetDiscsInfo(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetDiscs.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("GetDiscsTypes/{id}")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetDiscsTypes(int id)
        {
            try
            {
                var result = await Task.Run(() => _DiscsBL.GetDiscsTypes(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetDiscs.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("GetDiscsStatistics/{id}")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetDiscsStatistics(int id)
        {
            try
            {
                var result = await Task.Run(() => _DiscsBL.GetDiscStatistics(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetDiscs.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("GetDiscsStatisticsBruckner/{id}")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetDiscsStatisticsBruckner(int id)
        {
            try
            {
                var result = await Task.Run(() => _DiscsBL.GetDiscStatisticsBruckner(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetDiscs.ToString(), ex.Message);
                return GetResult(ex);
            }
        }


        [Route("GetCylinderCodeList/{id}")]
        [Authorize(Policy = "DiscRead")]
        [HttpGet]
        public async Task<IActionResult> GetCylinderCodeList(int id)
        {
            try
            {
                var result = await Task.Run(() => _DiscsBL.GetCylinderCodeList(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetCylinderCodeList.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("GetCylinderSeiteInfo/{id}")]
        [Authorize(Policy = "DiscRead")]
        [HttpGet]
        public async Task<IActionResult> GetCylinderSeiteInfo(int id)
        {
            try
            {
                var result = await Task.Run(() => _DiscsBL.GetCylinderSeiteInfo(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetCylinderSeiteInfo.ToString(), ex.Message);
                return GetResult(ex);
            }
        }


        [Route("GetDiscsNumber/{id}")]
        [Authorize(Policy = "DiscRead")]
        [HttpGet]
        public async Task<IActionResult> GetDiscsNumber(int id)
        {
            try
            {
                var result = await Task.Run(() => _DiscsBL.GetDiscsNumber(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetDiscsNumber.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("GetCylindersInGroups/{id}")]
        [Authorize(Policy = "DiscRead")]
        [HttpGet]
        public async Task<IActionResult> GetCylindersInGroups(int id)
        {
            try
            {
                var result = await Task.Run(() => _DiscsBL.GetCylindersInGroups(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetCylindersInGroups.ToString(), ex.Message);
                return GetResult(ex);
            }
        }


        [Route("DeleteDiscs/{id}")]
        [Authorize(Policy = "DiscDelete")]
        [HttpGet]
        public async Task<IActionResult> DeleteDiscs(int id)
        {
            try
            {
                var result = await Task.Run(() => _DiscsBL.DeleteDiscs(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.DeleteDisc.ToString(), ex.Message);
                return GetResult(ex);
            }
        }


        ////GetDiscsTypes
    }
}
