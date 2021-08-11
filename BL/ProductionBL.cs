using Allprimetech.GeneralUtils;
using Allprimetech.Interfaces;
using Allprimetech.Interfaces.Constants;
using Allprimetech.Interfaces.ControllerCodes;
using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allprimetech.BL
{
    public class ProductionBL
    {
        #region Declarations
        private IProductionRepository _ProductionRepository { get; set; }
        private IMapper _Mapper { get; set; }
        private IUnitOfWork _UnitOfWork { get; set; }
        private SystemAuditsBL _SystemAuditBL { get; set; }

        public ProductionBL(IProductionRepository productionRepository, IMapper mapper, IUnitOfWork unitOfWork, ISystemAuditRepository auditRepository)
        {
            _ProductionRepository = productionRepository;
            _Mapper = mapper;
            _UnitOfWork = unitOfWork;
            _SystemAuditBL = new SystemAuditsBL(auditRepository, mapper, unitOfWork);
            ProductionControllerStatus._Error = PRODUCTION_ERROR.NO_ERROR;
        }
        #endregion

        private async Task SaveOperation(string operatorId, Operation operation, Source source)
        {
            await _SystemAuditBL.StoreOperation(operation, source, operatorId);
        }

        public async Task<IEnumerable<object>> GetProductions(string userId)
        {
            try
            {
                var productions = await _ProductionRepository.AllProduction();

                await SaveOperation(userId, Operation.ReadAll, Source.Production);

                //List<object> keyCylinderProductions = new List<object>();

                //foreach (var item in productions)
                //{
                //    var product = await _ProductionRepository.GetProduct(item);
                //    keyCylinderProductions.Add(product);
                //}

                var productionsResource = _Mapper.Map<List<Production>, List<ReadProductionResource>>(productions);

                return productionsResource;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetProductions.ToString(), ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Returns a production object
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productionId"></param>
        /// <returns></returns>
        public async Task<object> GetProduction(string userId, int productionId)
        {
            try
            {
                var production = await _ProductionRepository.GetProduction(productionId);

                if (production == null)
                {
                    ProductionControllerStatus._Error = PRODUCTION_ERROR.NOT_FOUND;
                    return null;
                }

                await SaveOperation(userId, Operation.ReadSingle, Source.Production);

                //var product = await _ProductionRepository.GetProduct(production);

                var productionResource = _Mapper.Map<Production, ReadProductionResource>(production);
                return productionResource;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetProduction.ToString(), ex.Message);
                throw;
            }
        }

        /// <summary>
        /// update production record
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productionId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ReadProductionResource> UpdateProduction(string userId, int productionId, UpdateProductionResource model)
        {
            try
            {
                var production = await _ProductionRepository.GetProduction(productionId);
                var order = await _ProductionRepository.GetOrder(model._OrderID);

                if(production == null || order == null)
                {
                    ProductionControllerStatus._Error = PRODUCTION_ERROR.NOT_FOUND;
                    return null;
                }


                production._UpdatedDate = DateTime.UtcNow;
                production.OrderID = model._OrderID;
            

                await _UnitOfWork.CompleteAsync();

                await SaveOperation(userId, Operation.Updated, Source.Production);

                var productionResource = _Mapper.Map<Production, ReadProductionResource>(production);
                return productionResource;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.UpdateProduction.ToString(), ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Creates production
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ProductionResponse> CreateProduction(string userId, CreateProductionResource model)
        {
            try
            {
                // first check if order exists
                var checkOrder = await _ProductionRepository.GetOrder(model._OrderID);
                if (checkOrder == null)
                {
                    ProductionControllerStatus._Error = PRODUCTION_ERROR.NOT_FOUND;
                    // production for the order does'nt exists
                    return new ProductionResponse { _ProductType = (ProductType)model._ProductType, _ProductID = model._ProductID };
                }

                var production = _Mapper.Map<CreateProductionResource, Production>(model);

                production.ByUserId = userId;

                var response = await _ProductionRepository.CreateProduction(production, model._Produced, userId);
            
                if(response == null)
                {
                    ProductionControllerStatus._Error = PRODUCTION_ERROR.BAD_REQUEST;
                    return new ProductionResponse { _ProductType = (ProductType)model._ProductType, _ProductID = model._ProductID };
                }

                await _UnitOfWork.CompleteAsync();

                await SaveOperation(userId, Operation.Added, Source.Production);

                //var result = _Mapper.Map<Production, ReadProductionResource>(production);

                return response;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.CreateProduction.ToString(), ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Deletes production
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productionId"></param>
        /// <returns></returns>
        public async Task<int?> DeleteProduction(string userId, int productionId)
        {
            try
            {
                var production = await _ProductionRepository.GetProduction(productionId);

                if (production == null)
                {
                    ProductionControllerStatus._Error = PRODUCTION_ERROR.NOT_FOUND;
                    return null;
                }

                _ProductionRepository.DeleteProduction(production);
                await _UnitOfWork.CompleteAsync();

                await SaveOperation(userId, Operation.Deleted, Source.Production);

                return productionId;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.DeleteProduction.ToString(), ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Search production
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ReadProductionResource>> SearchProductions(string userId, ProductionSearchResource model)
        {
            try
            {
                var production = await _ProductionRepository.SearchProductions(model);

                await SaveOperation(userId, Operation.Search, Source.Production);

                var productionResource = _Mapper.Map<IEnumerable<Production>, IEnumerable<ReadProductionResource>>(production);
                return productionResource;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.SearchProductions.ToString(), ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Stores the key and cylinder relationship
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="keyCylindersList"></param>
        /// <returns></returns>
        public async Task<List<ReadCylinderGroupResource>> StoreCylinderGroups(string userId, List<CreateCylinderGroupResource> keyCylindersList)
        {
            try
            {
                var result = await _ProductionRepository.StoreCylinderGroups(keyCylindersList);

                await SaveOperation(userId, Operation.Added, Source.Production);

                return result; 
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.StoreCylinderGroups.ToString(), ex.Message);
                throw;
            }
        }

        public async Task<List<ReadCylinderGroupResource>> ModifyCylinderGroups(string userId, List<ModifyCylinderGroup> keyCylindersList)
        {
            try
            {
                var result = await _ProductionRepository.ModifyCylinderGroups(keyCylindersList);

                await SaveOperation(userId, Operation.Added, Source.Production);

                return result;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.StoreCylinderGroups.ToString(), ex.Message);
                throw;
            }
        }

        public async Task<List<ReadCylinderGroupResource>> StoreCylinderGroupVerifications(string userId, List<CreateCylinderGroupVerification> keyCylindersList)
        {
            try
            {
                var result = await _ProductionRepository.StoreCylinderGroupVerification(keyCylindersList);

                await SaveOperation(userId, Operation.Added, Source.Production);

                return result;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.StoreCylinderGroupVerifications.ToString(), ex.Message);
                throw;
            }
        }


        /// <summary>
        /// Returns the cylinder production history
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<CylinderProduction>> GetCylinderProductionHistory(string userId, int customerId)
        {
            try
            {
                var production = await _ProductionRepository.GetCylinderProductionHistory(customerId);


                await SaveOperation(userId, Operation.ReadSingle, Source.Production);

                //var product = await _ProductionRepository.GetProduct(production);

                //var productionResource = _Mapper.Map<List<CylinderProduction>, List<CylinderProductionResource>>(production);
                return production;

            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetCylinderProductionHistory.ToString(), ex.Message);
                throw;
            }
        }


        /// <summary>
        /// Returns the group key production history
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<KeyProduction>> GetKeyProductionHistory(string userId, int customerId)
        {
            try
            {
                var production = await _ProductionRepository.GetKeyProductionHistory(customerId);

                if (production == null)
                {
                    ProductionControllerStatus._Error = PRODUCTION_ERROR.NOT_FOUND;
                    return null;
                }

                await SaveOperation(userId, Operation.ReadSingle, Source.Production);

                //var product = await _ProductionRepository.GetProduct(production);

                //var productionResource = _Mapper.Map<List<KeyProduction>, List<KeyProductionResource>>(production);
                return production;

            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetKeyProductionHistory.ToString(), ex.Message);
                throw;
            }
        }
        
        
        /// <summary>
        /// Search Groupkey records
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<List<KeyProduction>> SearchGroupkeyProduction(string userId, ProductionProdSearchResource model)
        {
            try
            {
                var production = await _ProductionRepository.SearchKeyProduction(model);

                await SaveOperation(userId, Operation.ReadSingle, Source.Production);

                //var product = await _ProductionRepository.GetProduct(production);

                //var productionResource = _Mapper.Map<List<KeyProduction>, List<KeyProductionResource>>(production);
                return production;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.SearchGroupKeyProduction.ToString(), ex.Message);
                throw;
            }
        }
       
        /// <summary>
        /// Search the Cylinder Production records
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<List<CylinderProduction>> SearchCylinderProduction(string userId, ProductionProdSearchResource model)
        {
            try
            {
                var production = await _ProductionRepository.SearchCylinderProduction(model);

                await SaveOperation(userId, Operation.Search, Source.Production);

                //var product = await _ProductionRepository.GetProduct(production);

                //var productionResource = _Mapper.Map<List<CylinderProduction>, List<CylinderProductionResource>>(production);
                return production;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), "SearchCylinderProduction", ex.Message);
                throw;
            }
        }

        public async Task<object> GetKeyGroupMerging(string userId)
        {
            //var items = await _ProductionRepository.MergeRelationship();
            return null;
        }

        public async Task<object> UnZipLockingPlan(string userId, int customerId)
        {
            try
            {
                var value = await _ProductionRepository.UnZipLockingPlan(customerId);
                return null;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), "UnZipLockingPlan", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Returns the key cylinder relationship
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="customerId">customerId</param>
        /// <returns></returns>
        public async Task<List<ReadCylinderGroupResource>> GetKeyGroupCylinderPairs(string userId, int customerId)
        {
            try
            {
                var keyPairs = await _ProductionRepository.GetKeyGroupCylinderPairs(customerId);

                return keyPairs;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetKeyGroupCylinderPairs.ToString(), ex.Message);
                throw;
            }
        }

        public async Task<object> GetCylinderRelatedGrouping(string userId)
        {
            try
            {
                var cylinderGroups = await _ProductionRepository.GetCylinderRelatedGrouping(74);

                return cylinderGroups;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), "GetCylinderRelatedGrouping", ex.Message);
                throw;
            }
        }

        public async Task<object> GetTotalGroups(string userId, int customerId)
        {
            try
            {
                var totalGrps = await _ProductionRepository.GetTotalGroups(customerId);

                return totalGrps;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetTotalGroups.ToString(), ex.Message);
                throw;
            }
        }

        public async Task<object> GetCylindersWithGroups(string userId, int customerId)
        {
            try
            {
                var cylWithGrps = await _ProductionRepository.GetCylindersWithGroups(customerId);

                return cylWithGrps;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetCylindersWithGroups.ToString(), ex.Message);
                throw;
            }
        }

        public async Task<object> GetCylinderGroupInfo(string userId, int customerId, int cylinderId)
        {
            try
            {
                var groups = await _ProductionRepository.GetCylinderGroupInfo(customerId, cylinderId);
                var groupsResource = _Mapper.Map<List<Group>, List<CylinderGroupInfo>>(groups);
                return groupsResource;

            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetCylinderGroupInfo.ToString(), ex.Message);
                throw;
            }
        }

        public async Task<List<CylinderWithGroups>> GetCylindersWithRelatedGroups(string userId, int customerId)
        {
            try
            {
                var cylWithGrps = await _ProductionRepository.GetCylindersWithRelatedGroups(customerId);

                return cylWithGrps;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetCylindersWithRelatedGroups.ToString(), ex.Message);
                throw;
            }
        }

        public async Task<List<CylindersByGroupResource>> GetCylindersByGroupId(string userId, int customerId, int groupId)
        {
            try
            {
                var cylsByGrp = await _ProductionRepository.GetCylindersByGroup(customerId, groupId);

                return cylsByGrp;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetCylindersByGroupId.ToString(), ex.Message);
                throw;
            }
        }

        public async Task<List<CylinderIdsWithGroups>> GetCylinderGroupsWithRelatedGroupsIds(string userId, int customerId)
        {
            try
            {
                var cylsByGrp = await _ProductionRepository.GetCylinderGroupsWithRelatedGroupsIds(customerId);

                return cylsByGrp;   
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetCylinderGroupsWithRelatedGroupsIds.ToString(), ex.Message);
                throw;
            }
        }

        public async Task<List<CylinderIdsWithGroups>> GetCylinderGroupsWithNamedRelatedGroup(string userId, int customerId)
        {
            try
            {
                var cylsByGrp = await _ProductionRepository.GetCylinderGroupsWithNamedRelatedGroup(customerId);

                return cylsByGrp;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetCylinderGroupsWithNamedRelatedGroup.ToString(), ex.Message);
                throw;
            }
        }

        public async Task<CylinderWithNameGroups> GetSingleCylGroupWithNamedRelatedGroup(string userId, int customerId, int groupId)
        {
            try
            {
                var cylsByGrp = await _ProductionRepository.GetCylinderGroupWithNamedRelatedGroupByGroupId(customerId, groupId);

                return cylsByGrp;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), "GetSingleCylGroupWithNamedRelatedGroup", ex.Message);
                throw;
            }
        }

        public async Task<List<CylinderWithNameGroups>> GetAllCylinderGroupWithNamedRelatedGroups(string userId, int customerId)
        {
            try
            {
                var cylsByGrp = await _ProductionRepository.GetAllCylinderGroupWithNamedRelatedGroups(customerId);

                return cylsByGrp;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetAllCylinderGroupWithRelatedGroups.ToString(), ex.Message);
                throw;
            }
        }

        public async Task<List<CylinderGroupsResource>> GetCylinderGroups(string userId, int customerId)
        {
            try
            {
                var cylinderGroup = await _ProductionRepository.GetCylinderGroups(customerId);

                return cylinderGroup;

            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetCylinderGroups.ToString(), ex.Message);
                throw;
            }
        }
        public async Task<List<GroupCylindersResource>> GetGroupCylinders(string userId, int customerId)
        {
            try
            {
                var cylinderGroup = await _ProductionRepository.GetGroupCylinders(customerId);

                return cylinderGroup;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetGroupCylinders.ToString(), ex.Message);
                throw;
            }
        }

        public async Task<List<CylinderWithNameGroups>> GetRelatedGroupsOfCylinder(string userId, int customerId, int cylinderId)
        {
            try
            {
                var cylinderGrps = await _ProductionRepository.GetRelatedGroupsOfCylinder(customerId, cylinderId);

                return cylinderGrps;

            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetRelatedGroupsOfCylinder.ToString(), ex.Message);
                throw;
            }
        }

        public async Task<List<CylinderIdsWithGroups>> GetCylinderGroupsWithGroups(string userId, int customerId)
        {
            try
            {
                var cylsByGrp = await _ProductionRepository.GetCylinderGroupsWithGroups(customerId);

                return cylsByGrp;

            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetCylinderGroupsWithGroups.ToString(), ex.Message);
                throw;
            }
        }

        public async Task<List<CylinderWithNameGroups>> GetAllCylinderGroupWithRelatedGroups(string userId, int customerId)
        {
            try
            {
                var cylsByGrp = await _ProductionRepository.GetAllCylinderGroupWithRelatedGroups(customerId);

                return cylsByGrp;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetAllCylinderGroupWithRelatedGroups.ToString(), ex.Message);
                throw;
            }
        }

        public async Task<CylinderWithNameGroups> GetCylinderGroupWithRelatedGroupByGroupId(string userId, int customerId, int groupId)
        {
            try
            {
                var cylsByGrp = await _ProductionRepository.GetCylinderGroupWithRelatedGroupByGroupId(customerId, groupId);

                return cylsByGrp;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetCylinderGroupsWithRelatedGroups.ToString(), ex.Message);
                throw;
            }
        }

        public async Task<CylinderIdsWithGroups> GetRelatedGrpOfCylinder(string userId, int customerId, int cylinderId)
        {
            try
            {
                var cylsByGrp = await _ProductionRepository.GetRelatedGrpOfCylinder(customerId, cylinderId);

                return cylsByGrp;

            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetRelatedGroupsOfCylinder.ToString(), ex.Message);
                throw;
            }
        }

        public async Task<CylinderIdsWithGroups> GetCylinderGroupsRelationship(string userId, int customerId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<CylinderIdsNumberWithGroup>> GetCylinderNumbersWithRelatedGroups(string userId, int customerId)
        {
            try
            {
                var cylsByGrp = await _ProductionRepository.GetCylindersWithNamedRelatedGroup(customerId);

                return cylsByGrp;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetCylinderNumbersWithRelatedGroups.ToString(), ex.Message);
                throw;
            }
        }

        public async Task<CylindersInGroup> GetCylindersWithCountByGroup(string userId, CylinderSearchBy search)
        {
            try
            {
                var cylsByGrp = await _ProductionRepository.GetCylindersWithCountByGroup(search);

                return cylsByGrp;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetCylindersWithCountByGroup.ToString(), ex.Message);
                throw;
            }
        }


        public async Task<GroupsInCylinder> GetGroupsWithCountByCylinder(string userId, GroupSearchBy model)
        {
            try
            {
                var cylinderGrps = await _ProductionRepository.GetGroupsWithCountByCylinder(model);

                return cylinderGrps;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetGroupsWithCountByCylinder.ToString(), ex.Message);
                throw;
            }
        }

        public async Task<List<GroupSortInfo>> GetSortedGroups(string userId, SortResource model)
        {
            try
            {
                var sortedGroups = await _ProductionRepository.GetSortedGroups(model._CustomerID, model._SortType);

                return sortedGroups;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetSortedGroups.ToString(), ex.Message);
                throw;
            }
        }

        public async Task<object> SaveDiscsAndGroupInfo(string userId, int customerId)
        {
            try
            {
                await _ProductionRepository.SaveDiscGroupInfo(customerId);
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<CylinderSortInfo>> GetSortedCylinders(string userId, SortResource model)
        {
            try
            {
                var sortedGroups = await _ProductionRepository.GetSortedCylinders(model._CustomerID, model._SortType);

                return sortedGroups;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetSortedCylinders.ToString(), ex.Message);
                throw;
            }
        }

        public async Task<List<GroupCylindersResource>> GetFilteredLockingPlan(string userId, FilterLP model)
        {
            try
            {
                var sortedGroups = await _ProductionRepository.GetFilteredLockingPlan(model);

                return sortedGroups;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), "GetFilteredLockingPlan", ex.Message);
                throw;
            }
        }

        public async Task<List<GroupCylindersResource>> GetExistingLockingPlan(string userId, int customerId)
        {
            try
            {
                var sortedGroups = await _ProductionRepository.GetExistingLockingPlan(customerId);

                return sortedGroups;
            }
            catch (Exception ex)
            {
                ProductionControllerStatus._Error = PRODUCTION_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), "GetExistingLockingPlan", ex.Message);
                throw;
            }
        }
    }
}
