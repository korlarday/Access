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
    public class ProductionsController : ControllerBase
    {
        #region Declarations
        public ProductionBL _ProductionRepo { get; set; }
        public ProductionsController(
            IProductionRepository productionRepository, 
            IMapper mapper, IUnitOfWork unitOfWork, ISystemAuditRepository auditRepository)
        {
            _ProductionRepo = new ProductionBL(productionRepository, mapper, unitOfWork, auditRepository);
        }
        #endregion

        #region private methods
        private IActionResult GetResult(object response)
        {
            PRODUCTION_ERROR status = ProductionControllerStatus._Error;
            ProductionControllerStatus._Error = PRODUCTION_ERROR.NO_ERROR;

            switch (status)
            {
                case PRODUCTION_ERROR.BAD_REQUEST:
                    return BadRequest(new TransactionStatus
                    {
                        _ErrorCode = (int)PRODUCTION_ERROR.BAD_REQUEST,
                        _Message = StringStore.BadRequest,
                        _Response = response
                    });
                case PRODUCTION_ERROR.EXISTS:
                    return BadRequest(new TransactionStatus
                    {
                        _ErrorCode = (int)PRODUCTION_ERROR.EXISTS,
                        _Message = StringStore.UserExistsResponse,
                        _Response = response
                    });

                case PRODUCTION_ERROR.NOT_FOUND:
                    return BadRequest(new TransactionStatus
                    {
                        _ErrorCode = (int)PRODUCTION_ERROR.NOT_FOUND,
                        _Message = StringStore.NotFound,
                        _Response = response
                    });

                case PRODUCTION_ERROR.EXCEPTION:
                    return StatusCode(StatusCodes.Status500InternalServerError, new TransactionStatus
                    {
                        _ErrorCode = (int)PRODUCTION_ERROR.EXCEPTION,
                        _Message = StringStore.ExceptionMessage,
                        _Response = response
                    });

                case PRODUCTION_ERROR.NO_ERROR:
                    if (response != null)
                    {
                        return Ok(response);
                    }
                    else
                    {
                        return Ok(new TransactionStatus
                        {
                            _ErrorCode = (int)PRODUCTION_ERROR.NO_ERROR,
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

        [Route("GetProductions")]
        [Authorize(Policy = "ProductionRead")]
        [HttpGet]
        public async Task<IActionResult> GetProductions()
        {
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetProductions(GetUserId()));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetProductions.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("GetProduction/{id}")]
        [Authorize(Policy = "ProductionRead")]
        [HttpGet]
        public async Task<IActionResult> GetProduction(int id)
        {
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetProduction(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetProduction.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("CreateProduction")]
        [Authorize(Policy = "ProductionCreate")]
        [HttpPost]
        public async Task<IActionResult> CreateProduction(CreateProductionResource model)
        {
            IsValid();
            try
            {
                var updatedProduct = await Task.Run(() => _ProductionRepo.CreateProduction(GetUserId(), model));
                return GetResult(updatedProduct);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.CreateProduction.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("UpdateProduction/{id}")]
        [Authorize(Policy = "ProductionUpdate")]
        [HttpPost]
        public async Task<IActionResult> UpdateProduction(int id, UpdateProductionResource model)
        {
            IsValid();
            try
            {
                var updatedProduct = await Task.Run(() => _ProductionRepo.UpdateProduction(GetUserId(), id, model));
                return GetResult(updatedProduct);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.UpdateProduction.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("DeleteProduction/{id}")]
        [Authorize(Policy = "ProductionDelete")]
        [HttpPost]
        public async Task<IActionResult> DeleteProduction(int id)
        {
            IsValid();
            try
            {
                var updatedProduct = await Task.Run(() => _ProductionRepo.DeleteProduction(GetUserId(), id));
                return GetResult(updatedProduct);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.DeleteProduction.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("SearchProduction")]
        [Authorize(Policy = "ProductionRead")]
        [HttpPost]
        public async Task<IActionResult> SearchProductions(ProductionSearchResource model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _ProductionRepo.SearchProductions(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.SearchProductions.ToString(), ex.Message);
                return GetResult(ex);
            }
        }
        
        //
        [Route("GetCylinderProduction")]
        [Authorize(Policy = "ProductionRead")]
        [HttpPost]
        public async Task<IActionResult> GetCylinderProduction(ProductionProdSearchResource model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _ProductionRepo.SearchCylinderProduction(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetCylinderProduction.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("GetGroupkeyProduction")]
        [Authorize(Policy = "ProductionRead")]
        [HttpPost]
        public async Task<IActionResult> GetGroupkeyProduction(ProductionProdSearchResource model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _ProductionRepo.SearchGroupkeyProduction(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetGroupkeyProduction.ToString(), ex.Message);
                return GetResult(ex);
            }
        }


        [Route("StoreCylinderGroups")]
        [Authorize(Policy = "ProductionCreate")]
        [HttpPost]
        public async Task<IActionResult> StoreCylinderGroups(List<CreateCylinderGroupResource> model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _ProductionRepo.StoreCylinderGroups(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.StoreCylinderGroups.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("ModifyCylinderGroups")]
        [Authorize(Policy = "ProductionCreate")]
        [HttpPost]
        public async Task<IActionResult> ModifyCylinderGroups(List<ModifyCylinderGroup> model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _ProductionRepo.ModifyCylinderGroups(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.StoreCylinderGroups.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("StoreCylinderGroupVerifications")]
        [Authorize(Policy = "ProductionCreate")]
        [HttpPost]
        public async Task<IActionResult> StoreCylinderGroupVerifications(List<CreateCylinderGroupVerification> model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _ProductionRepo.StoreCylinderGroupVerifications(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.StoreCylinderGroups.ToString(), ex.Message);
                return GetResult(ex);
            }
        }



        [Route("GetKeyGroupCylinderPairs/{id}")]
        [Authorize(Policy = "ProductionRead")]
        [HttpGet]
        public async Task<IActionResult> GetKeyGroupCylinderPairs(int id)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetKeyGroupCylinderPairs(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetKeyGroupCylinderPairs.ToString(), ex.Message);
                return GetResult(ex);
            }
        }



        [Route("GetCylinderProductionHistory/{id}")]
        [Authorize(Policy = "ProductionRead")]
        [HttpGet]
        public async Task<IActionResult> GetCylinderProductionHistory(int id)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetCylinderProductionHistory(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetCylinderProductionHistory.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("GetKeyProductionHistory/{id}")]
        [Authorize(Policy = "ProductionRead")]
        [HttpGet]
        public async Task<IActionResult> GetKeyProductionHistory(int id)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetKeyProductionHistory(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetKeyProductionHistory.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("GetKeyGroupMerging")]
        [Authorize(Policy = "ProductionRead")]
        [HttpGet]
        public async Task<IActionResult> GetKeyGroupMerging()
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetKeyGroupMerging(GetUserId()));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetKeyProductionHistory.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("UnZipLockingPlan/{id}")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UnZipLockingPlan(int id)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _ProductionRepo.UnZipLockingPlan(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetKeyProductionHistory.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("GetCylinderRelatedGrouping")]
        [Authorize(Policy = "ProductionRead")]
        [HttpGet]
        public async Task<IActionResult> GetCylinderRelatedGrouping()
        {
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetCylinderRelatedGrouping(GetUserId()));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetKeyProductionHistory.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Authorize(Policy = "GroupsRead")]
        [Route("GetTotalGroups/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetTotalGroups(int id)
        {
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetTotalGroups(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetTotalGroups.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Authorize(Policy = "GroupsRead")]
        [Route("GetCylindersWithGroups/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetCylindersWithGroups(int id)
        {
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetCylindersWithGroups(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetCylindersWithGroups.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Authorize(Policy = "GroupsRead")]
        [Route("GetCylindersWithRelatedGroups/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetCylindersWithRelatedGroups(int id)
        {
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetCylindersWithRelatedGroups(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetCylindersWithRelatedGroups.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        //

        [Authorize(Policy = "GroupsRead")]
        [Route("GetCylinderGroupInfo/{id}/{cylinderId}")]
        [HttpGet]
        public async Task<IActionResult> GetCylinderGroupInfo(int id, int cylinderId)
        {
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetCylinderGroupInfo(GetUserId(), id, cylinderId));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetCylinderGroupInfo.ToString(), ex.Message);
                return GetResult(ex);
            }
        }


        [Authorize(Policy = "GroupsRead")]
        [Route("GetCylindersByGroup/{id}/{groupId}")]
        [HttpGet]
        public async Task<IActionResult> GetCylindersByGroupId(int id, int groupId)
        {
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetCylindersByGroupId(GetUserId(), id, groupId));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetCylindersByGroupId.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Authorize(Policy = "GroupsRead")]
        [Route("GetCylinderGroupsWithRelatedGroups/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetCylinderGroupsWithRelatedGroups(int id)
        {
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetCylinderGroupsWithRelatedGroupsIds(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetCylinderGroupsWithRelatedGroups.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Authorize(Policy = "GroupsRead")]
        [Route("GetCylinderGroupsWithNamedRelatedGroup/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetCylinderGroupsWithNamedRelatedGroup(int id)
        {
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetCylinderGroupsWithNamedRelatedGroup(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetCylinderGroupsWithNamedRelatedGroup.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Authorize(Policy = "GroupsRead")]
        [Route("GetSingleCylGroupWithNamedRelatedGroup/{id}/{groupId}")]
        [HttpGet]
        public async Task<IActionResult> GetSingleCylGroupWithNamedRelatedGroup(int id, int groupId)
        {
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetSingleCylGroupWithNamedRelatedGroup(GetUserId(), id, groupId));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetCylinderGroupsWithNamedRelatedGroup.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Authorize(Policy = "GroupsRead")]
        [Route("GetAllCylinderGroupWithNamedRelatedGroups/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetAllCylinderGroupWithNamedRelatedGroups(int id)
        {
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetAllCylinderGroupWithNamedRelatedGroups(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetCylinderGroupsWithNamedRelatedGroup.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Authorize(Policy = "GroupsRead")]
        [Route("GetRelatedGroupsOfCylinder/{id}/{cylinderId}")]
        [HttpGet]
        public async Task<IActionResult> GetRelatedGroupsOfCylinder(int id, int cylinderId)
        {
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetRelatedGroupsOfCylinder(GetUserId(), id, cylinderId));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetRelatedGroupsOfCylinder.ToString(), ex.Message);
                return GetResult(ex);
            }
        }


        [Authorize(Policy = "GroupsRead")]
        [Route("GetCylinderGroups/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetCylinderGroups(int id)
        {
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetCylinderGroups(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetCylinderGroups.ToString(), ex.Message);
                return GetResult(ex);
            }
        }
        [Authorize(Policy = "GroupsRead")]
        [Route("GetGroupCylinders/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetGroupCylinders(int id)
        {
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetGroupCylinders(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetCylinderGroups.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Authorize(Policy = "GroupsRead")]
        [Route("GetCylinderGroupsWithGroups/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetCylinderGroupsWithGroups(int id)
        {
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetCylinderGroupsWithGroups(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetCylinderGroupsWithGroups.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Authorize(Policy = "GroupsRead")]
        [Route("GetAllCylinderGroupWithRelatedGroups/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetAllCylinderGroupWithRelatedGroups(int id)
        {
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetAllCylinderGroupWithRelatedGroups(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetAllCylinderGroupWithRelatedGroups.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Authorize(Policy = "GroupsRead")]
        [Route("GetCylinderNumbersWithRelatedGroups/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetCylinderNumbersWithRelatedGroups(int id)
        {
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetCylinderNumbersWithRelatedGroups(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetCylinderNumbersWithRelatedGroups.ToString(), ex.Message);
                return GetResult(ex);
            }
        }


        [Authorize(Policy = "GroupsRead")]
        [Route("GetCylinderGroupWithRelatedGroupByGroupId/{id}/{groupId}")]
        [HttpGet]
        public async Task<IActionResult> GetCylinderGroupWithRelatedGroup(int id, int groupId)
        {
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetCylinderGroupWithRelatedGroupByGroupId(GetUserId(), id, groupId));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetCylinderGroupsWithNamedRelatedGroup.ToString(), ex.Message);
                return GetResult(ex);
            }
        }


        [Authorize(Policy = "GroupsRead")]
        [Route("GetRelatedGrpOfCylinder/{id}/{cylinderId}")]
        [HttpGet]
        public async Task<IActionResult> GetRelatedGrpOfCylinder(int id, int cylinderId)
        {
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetRelatedGrpOfCylinder(GetUserId(), id, cylinderId));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetRelatedGroupsOfCylinder.ToString(), ex.Message);
                return GetResult(ex);
            }
        }





        [Authorize(Policy = "GroupsRead")]
        [Route("GetCylindersWithCountByGroup")]
        [HttpPost]
        public async Task<IActionResult> GetCylindersWithCountByGroup(CylinderSearchBy search)
        {
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetCylindersWithCountByGroup(GetUserId(), search));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetCylindersByGroupId.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Authorize(Policy = "GroupsRead")]
        [Route("GetGroupsWithCountByCylinder")]
        [HttpPost]
        public async Task<IActionResult> GetGroupsWithCountByCylinder(GroupSearchBy model)
        {
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetGroupsWithCountByCylinder(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetGroupsWithCountByCylinder.ToString(), ex.Message);
                return GetResult(ex);
            }
        }


        [Route("GetSortedGroups")]
        [Authorize(Policy = "ProductionRead")]
        [HttpPost]
        public async Task<IActionResult> GetSortedGroups(SortResource model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetSortedGroups(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetSortedGroups.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("GetSortedCylinders")]
        [Authorize(Policy = "ProductionRead")]
        [HttpPost]
        public async Task<IActionResult> GetSortedCylinders(SortResource model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetSortedCylinders(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetSortedCylinders.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("GetFilteredLockingPlan")]
        [Authorize(Policy = "ProductionRead")]
        [HttpPost]
        public async Task<IActionResult> GetFilteredLockingPlan(FilterLP model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetFilteredLockingPlan(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetSortedCylinders.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("GetExistingLockingPlan/{customerId}")]
        [Authorize(Policy = "ProductionRead")]
        [HttpPost]
        public async Task<IActionResult> GetFilteredLockingPlan(int customerId)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _ProductionRepo.GetExistingLockingPlan(GetUserId(), customerId));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetSortedCylinders.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("SaveDiscsAndGroupInfo/{id}")]
        [Authorize(Policy = "ProductionRead")]
        [HttpGet]
        public async Task<IActionResult> SaveDiscsAndGroupInfo(int id)
        {
            try
            {
                var result = await Task.Run(() => _ProductionRepo.SaveDiscsAndGroupInfo(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetSortedCylinders.ToString(), ex.Message);
                return GetResult(ex);
            }
        }


    }
}
