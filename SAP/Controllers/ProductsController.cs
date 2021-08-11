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
    public class ProductsController : ControllerBase
    {
        #region Declarations
        public CylinderBL _CylinderBL { get; set; }
        public OrdersBL _OrderBL { get; set; }
        public ProductsController(
            ICylinderRepository cylinderRepository, 
            IMapper mapper, IUnitOfWork unitOfWork, 
            IGroupRepository group, IOrdersRepository orderRepo, ISystemAuditRepository auditRepository, ICustomersRepository customersRepository)
        {
            _CylinderBL = new CylinderBL(cylinderRepository, mapper, unitOfWork, auditRepository);
            _OrderBL = new OrdersBL(orderRepo, mapper, unitOfWork, group, auditRepository, customersRepository);
        }
        #endregion

        #region private methods
        private IActionResult GetResult(object response)
        {
            PRODUCT_ERROR status = ProductControllerStatus._Error;
            ProductControllerStatus._Error = PRODUCT_ERROR.NO_ERROR;

            switch (status)
            {
                case PRODUCT_ERROR.BAD_REQUEST:
                    return BadRequest(new TransactionStatus
                    {
                        _ErrorCode = (int)PRODUCT_ERROR.BAD_REQUEST,
                        _Message = StringStore.BadRequest
                    });
                case PRODUCT_ERROR.EXISTS:
                    return BadRequest(new TransactionStatus
                    {
                        _ErrorCode = (int)PRODUCT_ERROR.EXISTS,
                        _Message = StringStore.UserExistsResponse
                    });

                case PRODUCT_ERROR.NOT_FOUND:
                    return BadRequest(new TransactionStatus
                    {
                        _ErrorCode = (int)PRODUCT_ERROR.NOT_FOUND,
                        _Message = StringStore.NotFound
                    });

                case PRODUCT_ERROR.EXCEPTION:
                    return StatusCode(StatusCodes.Status500InternalServerError, new TransactionStatus
                    {
                        _ErrorCode = (int)PRODUCT_ERROR.EXCEPTION,
                        _Message = StringStore.ExceptionMessage
                    });

                case PRODUCT_ERROR.NO_ERROR:
                    if (response != null)
                    {
                        return Ok(response);
                    }
                    else
                    {
                        return Ok(new TransactionStatus
                        {
                            _ErrorCode = (int)PRODUCT_ERROR.NO_ERROR,
                            _Message = StringStore.SuccessExecutionMessage
                        });
                    }

                default:
                    return BadRequest();
            }

            //if (response == null)
            //    return NotFound();

            //return Ok(response);
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

        #region Cylinder Endpoints
        [Route("GetCylinders/{id}")]
        [Authorize(Policy = "CylinderRead")]
        [HttpGet]
        public async Task<IActionResult> GetCylinders(int id)
        {
            try
            {
                var result = await Task.Run(() => _CylinderBL.AllCylinders(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetCylinders.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("GetCylinder/{id}")]
        [Authorize(Policy = "CylinderRead")]
        [HttpGet]
        public async Task<IActionResult> GetCylinder(int id)
        {
            try
            {
                var result = await Task.Run(() => _CylinderBL.GetCylinder(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetCylinder.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("UpdateCylinder/{id}")]
        [Authorize(Policy = "CylinderUpdate")]
        [HttpPost]
        public async Task<IActionResult> UpdateCylinder(int id, CreateCylinderResource model)
        {
            IsValid();
            try
            {
                var updatedProduct = await Task.Run(() => _CylinderBL.UpdateCylinder(GetUserId(), id, model));
                return GetResult(updatedProduct);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.UpdateCylinder.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("CreateCylinder")]
        [HttpPost]
        [Authorize(Policy = "CylinderCreate")]
        public async Task<IActionResult> CreateCylinder(CreateCylinderResource model)
        {
            IsValid();

            try
            {
                var result = await Task.Run(() => _CylinderBL.CreateCylinder(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.CreateCylinder.ToString(), ex.Message);
                return GetResult(ex);
            }

        }

        [Route("CreateCylinders")]
        [HttpPost]
        [Authorize(Policy = "CylinderCreate")]
        public async Task<IActionResult> CreateCylinders(List<CreateCylinderResource> model)
        {
            IsValid();

            try
            {
                var result = await Task.Run(() => _CylinderBL.CreateCylinders(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.CreateCylinder.ToString(), ex.Message);
                return GetResult(ex);
            }

        }

        [Route("ModifyCylinders")]
        [HttpPost]
        [Authorize(Policy = "CylinderCreate")]
        public async Task<IActionResult> ModifyCylinders(List<CreateCylinderResource> model)
        {
            IsValid();

            try
            {
                var result = await Task.Run(() => _CylinderBL.ModifyCylinders(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.ModifyCylinders.ToString(), ex.Message);
                return GetResult(ex);
            }

        }


        [Route("DeleteCylinder/{id}")]
        [Authorize(Policy = "CylinderDelete")]
        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteCylinder(int id)
        {
            try
            {
                var result = await Task.Run(() => _CylinderBL.DeleteCylinder(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.DeleteCylinder.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("SearchCylinder")]
        [Authorize(Policy = "CylinderRead")]
        [HttpPost]
        public async Task<IActionResult> SearchCylinders(CylinderSearchResource model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _CylinderBL.SearchCylinders(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.SearchOrders.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Authorize]
        [Route("GetCylindersByOrderNumber/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetCylindersByOrder(string id)
        {
            try
            {
                var result = await Task.Run(() => _CylinderBL.GetCylindersByOrder(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetCylindersByOrder.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("GetCylindersStartIndex/{id}")]
        [Authorize(Policy = "CylinderRead")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCylindersStartIndex(string id)
        {
            try
            {
                var result = await Task.Run(() => _CylinderBL.GetCylindersStartIndex(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetCylindersStartIndex.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        #endregion

        #region Groups Endpoints
        [Authorize(Policy = "GroupsRead")]
        [Route("GetGroups/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetGroups(int id)
        {
            try
            {
                var result = await Task.Run(() => _OrderBL.AllGroups(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetGroups.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Authorize(Policy = "GroupsRead")]
        [Route("GetGroup/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetGroup(int id)
        {
            try
            {
                var result = await Task.Run(() => _OrderBL.GetGroup(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetGroup.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Authorize(Policy = "GroupsUpdate")]
        [Route("UpdateGroup/{id}")]
        [HttpPost]
        public async Task<IActionResult> UpdateGroup(int id, CreateGroupResource model)
        {
            IsValid();
            try
            {
                var updatedProduct = await Task.Run(() => _OrderBL.UpdateGroup(GetUserId(), id, model));
                return GetResult(updatedProduct);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.UpdateGroup.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [HttpPost]
        [Route("CreateGroup")]
        [Authorize(Policy = "GroupsCreate")]
        public async Task<IActionResult> r(CreateGroupResource model)
        {
            IsValid();

            try
            {
                var result = await Task.Run(() => _OrderBL.CreateGroup(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.CreateGroup.ToString(), ex.Message);
                return GetResult(ex);
            }

        }

        [HttpPost]
        [Route("CreateGroups")]
        [Authorize(Policy = "GroupsCreate")]
        public async Task<IActionResult> CreateGroups(List<CreateGroupResource> model)
        {
            IsValid();

            try
            {
                var result = await Task.Run(() => _OrderBL.CreateGroups(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.CreateGroup.ToString(), ex.Message);
                return GetResult(ex);
            }

        }

        [HttpPost]
        [Route("ModifyGroups")]
        [Authorize(Policy = "GroupsCreate")]
        public async Task<IActionResult> ModifyGroups(List<CreateGroupResource> model)
        {
            IsValid();

            try
            {
                var result = await Task.Run(() => _OrderBL.ModifyGroups(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.ModifyGroups.ToString(), ex.Message);
                return GetResult(ex);
            }

        }


        [Authorize(Policy = "GroupsDelete")]
        [Route("DeleteGroup/{id}")]
        [HttpPost]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            try
            {
                var result = await Task.Run(() => _OrderBL.DeleteGroup(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.DeleteGroup.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("SearchGroup")]
        [Authorize(Policy = "GroupsRead")]
        [HttpPost]
        public async Task<IActionResult> SearchGroups(GroupSearchResource model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _OrderBL.SearchGroups(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.SearchGroups.ToString(), ex.Message);
                return GetResult(ex);
            }
        }


        [Authorize(Policy = "GroupsRead")]
        [Route("GetGroupsByOrderNumber/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetGroupsByOrder(string id)
        {
            try
            {
                var result = await Task.Run(() => _OrderBL.GetGroupsByOrder(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetGroupsByOrder.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Authorize(Policy = "GroupsRead")]
        [Route("GetGroupsStartIndex/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetGroupsStartIndex(string id)
        {
            try
            {
                var result = await Task.Run(() => _OrderBL.GetGroupsStartIndex(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetGroupsStartIndex.ToString(), ex.Message);
                return GetResult(ex);
            }
        }


        #endregion
    }
}
