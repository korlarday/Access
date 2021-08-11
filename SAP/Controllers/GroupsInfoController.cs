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
    public class GroupsInfoController : ControllerBase
    {
        #region Declarations
        public GroupsInfoBL _GroupsInfosBL { get; set; }
        public GroupsInfoController(
            IGroupsInfoRepository groupInfoRepository,
            IMapper mapper, IUnitOfWork unitOfWork, ISystemAuditRepository auditRepository)
        {
            _GroupsInfosBL = new GroupsInfoBL(groupInfoRepository, mapper, unitOfWork, auditRepository);
        }
        #endregion

        #region private methods
        private IActionResult GetResult(object response)
        {
            GROUPINFO_ERROR status = GroupInfoContrStatus._Error;
            GroupInfoContrStatus._Error = GROUPINFO_ERROR.NO_ERROR;

            switch (status)
            {
                case GROUPINFO_ERROR.BAD_REQUEST:
                    return BadRequest(new TransactionStatus
                    {
                        _ErrorCode = (int)GROUPINFO_ERROR.BAD_REQUEST,
                        _Message = StringStore.BadRequest
                    });
                case GROUPINFO_ERROR.EXISTS:
                    return BadRequest(new TransactionStatus
                    {
                        _ErrorCode = (int)GROUPINFO_ERROR.EXISTS,
                        _Message = StringStore.UserExistsResponse
                    });

                case GROUPINFO_ERROR.NOT_FOUND:
                    return BadRequest(new TransactionStatus
                    {
                        _ErrorCode = (int)GROUPINFO_ERROR.NOT_FOUND,
                        _Message = StringStore.NotFound
                    });

                case GROUPINFO_ERROR.EXCEPTION:
                    return StatusCode(StatusCodes.Status500InternalServerError, new TransactionStatus
                    {
                        _ErrorCode = (int)GROUPINFO_ERROR.EXCEPTION,
                        _Message = StringStore.ExceptionMessage
                    });

                case GROUPINFO_ERROR.NO_ERROR:
                    if (response != null)
                    {
                        return Ok(response);
                    }
                    else
                    {
                        return Ok(new TransactionStatus
                        {
                            _ErrorCode = (int)GROUPINFO_ERROR.NO_ERROR,
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

        [Route("GetGroupInfos/{id}")]
        [Authorize(Policy = "GroupsInfoRead")]
        [HttpGet]
        public async Task<IActionResult> GetGroupsInfos(int id)
        {
            try
            {
                var result = await Task.Run(() => _GroupsInfosBL.AllGroupsInfos(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetGroupsInfos.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("GetGroupInfo/{id}")]
        [Authorize(Policy = "GroupsInfoRead")]
        [HttpGet]
        public async Task<IActionResult> GetGroupsInfo(int id)
        {
            try
            {
                var result = await Task.Run(() => _GroupsInfosBL.GetGroupsInfo(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetGroupsInfo.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("UpdateGroupInfo/{id}")]
        [Authorize(Policy = "GroupsInfoUpdate")]
        [HttpPost]
        public async Task<IActionResult> UpdateGroupsInfo(int id, CreateGroupsInfoResource model)
        {
            IsValid();
            try
            {
                var updatedProduct = await Task.Run(() => _GroupsInfosBL.UpdateGroupsInfo(GetUserId(), id, model));
                return GetResult(updatedProduct);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.UpdateGroupsInfo.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("CreateGroupInfo")]
        [HttpPost]
        [Authorize(Policy = "GroupsInfoCreate")]
        public async Task<IActionResult> CreateGroupsInfo(CreateGroupsInfoResource model)
        {
            IsValid();

            try
            {
                var result = await Task.Run(() => _GroupsInfosBL.CreateGroupsInfo(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.CreateGroupsInfo.ToString(), ex.Message);
                return GetResult(ex);
            }

        }

        [Route("CreateGroupInfos")]
        [HttpPost]
        [Authorize(Policy = "GroupsInfoCreate")]
        public async Task<IActionResult> CreateGroupsInfos(List<CreateGroupsInfoResource> model)
        {
            IsValid();

            try
            {
                var result = await Task.Run(() => _GroupsInfosBL.CreateGroupsInfos(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.CreateGroupsInfo.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("CreateGroupInfoVerification")]
        [HttpPost]
        [Authorize(Policy = "GroupsInfoCreate")]
        public async Task<IActionResult> CreateGroupInfoVerification(List<CreateGroupsInfoResource> model)
        {
            IsValid();

            try
            {
                var result = await Task.Run(() => _GroupsInfosBL.CreateGroupsInfoVerification(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.CreateGroupsInfo.ToString(), ex.Message);
                return GetResult(ex);
            }
        }



        [Route("DeleteGroupInfoVerification/{customerId}")]
        [HttpPost]
        [Authorize(Policy = "GroupsInfoDelete")]
        public async Task<IActionResult> DeleteGroupInfoVerification(int customerId)
        {
            IsValid();

            try
            {
                var result = await Task.Run(() => _GroupsInfosBL.DeleteGroupInfoVerification(GetUserId(), customerId));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.DeleteGroupsInfo.ToString(), ex.Message);
                return GetResult(ex);
            }
        }



        [Route("DeleteGroupInfo/{id}")]
        [Authorize(Policy = "GroupsInfoDelete")]
        [HttpPost]
        public async Task<IActionResult> DeleteGroupsInfo(int id)
        {
            try
            {
                var result = await Task.Run(() => _GroupsInfosBL.DeleteGroupsInfo(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.DeleteGroupsInfo.ToString(), ex.Message);
                return GetResult(ex);
            }
        }


        [Route("GetKeyCodeList/{id}")]
        [Authorize(Policy = "GroupsInfoRead")]
        [HttpGet]
        public async Task<IActionResult> GetKeyCodeList(int id)
        {
            try
            {
                var result = await Task.Run(() => _GroupsInfosBL.GetKeyCodeList(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetKeyCodeList.ToString(), ex.Message);
                return GetResult(ex);
            }
        }


        

    }
}
