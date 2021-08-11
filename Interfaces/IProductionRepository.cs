using Allprimetech.Interfaces.Constants;
using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Allprimetech.Interfaces
{
    public interface IProductionRepository
    {
        Task<List<Production>> AllProduction();
        Task<Production> GetProduction(int? productionId);
        Task<Order> GetOrder(int orderId);
        Task<ProductionResponse> CreateProduction(Production production, int produced, string userId);
        void DeleteProduction(Production production);
        Task<IEnumerable<Production>> SearchProductions(ProductionSearchResource model);
        Task<List<ReadCylinderGroupResource>> StoreCylinderGroups(List<CreateCylinderGroupResource> model);
        Task<List<ReadCylinderGroupResource>> ModifyCylinderGroups(List<ModifyCylinderGroup> model);
        Task<object> GetProduct(Production item);
        Task<List<CylinderProduction>> GetCylinderProductionHistory(int customerId);
        Task<List<KeyProduction>> GetKeyProductionHistory(int customerId);
        //Task<object> SearchProductionType(ProductionProdSearchResource model);
        Task<List<CylinderProduction>> SearchCylinderProduction(ProductionProdSearchResource model);
        Task<List<KeyProduction>> SearchKeyProduction(ProductionProdSearchResource model);
        Task<List<ReadCylinderGroupResource>> GetKeyGroupCylinderPairs(int customerId);
        Task<object> UnZipLockingPlan(int customerId);
        Task<object> GetCylinderRelatedGrouping(int customerId);
        Task<object> GetTotalGroups(int customerId);
        Task<List<CylinderIdsWithGroups>> GetCylindersWithGroups(int customerId);
        Task<List<CylinderWithGroups>> GetCylindersWithRelatedGroups(int customerId);
        Task<List<Group>> GetCylinderGroupInfo(int customerId, int cylinderId);
        Task<List<CylindersByGroupResource>> GetCylindersByGroup(int customerId, int groupId);
        Task<List<CylinderIdsWithGroups>> GetCylinderGroupsWithRelatedGroupsIds(int customerId);
        Task<List<CylinderIdsWithGroups>> GetCylinderGroupsWithNamedRelatedGroup(int customerId);
        Task<CylinderWithNameGroups> GetCylinderGroupWithNamedRelatedGroupByGroupId(int customerId, int groupId);
        Task<List<CylinderWithNameGroups>> GetAllCylinderGroupWithNamedRelatedGroups(int customerId);
        Task<List<CylinderGroupsResource>> GetCylinderGroups(int customerId);
        Task<List<CylinderWithNameGroups>> GetRelatedGroupsOfCylinder(int customerId, int cylinderId);
        Task<List<CylinderIdsWithGroups>> GetCylinderGroupsWithGroups(int customerId);
        Task<List<CylinderWithNameGroups>> GetAllCylinderGroupWithRelatedGroups(int customerId);
        Task<CylinderWithNameGroups> GetCylinderGroupWithRelatedGroupByGroupId(int customerId, int groupId);
        Task<CylinderIdsWithGroups> GetRelatedGrpOfCylinder(int customerId, int cylinderId);
        Task<List<GroupCylindersResource>> GetGroupCylinders(int customerId);
        Task<List<ReadCylinderGroupResource>> StoreCylinderGroupVerification(List<CreateCylinderGroupVerification> keyCylindersList);
        Task<List<CylinderIdsNumberWithGroup>> GetCylindersWithNamedRelatedGroup(int customerId);
        Task<CylindersInGroup> GetCylindersWithCountByGroup(CylinderSearchBy search);
        Task<GroupsInCylinder> GetGroupsWithCountByCylinder(GroupSearchBy model);
        Task<List<GroupSortInfo>> GetSortedGroups(int customerID, string sortType);
        Task<List<CylinderSortInfo>> GetSortedCylinders(int customerID, string sortType);
        Task<List<GroupCylindersResource>> GetFilteredLockingPlan(FilterLP model);
        Task<List<GroupCylindersResource>> GetExistingLockingPlan(int customerId);

        Task SaveDiscGroupInfo(int customerId);
        //
    }
}
