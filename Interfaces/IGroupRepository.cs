using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Allprimetech.Interfaces
{
    public interface IGroupRepository
    {
        Task<List<Group>> AllGroups(int customerId);
        Task<Group> GetGroup(int id);
        Task AddGroup(Group group, string userId);
        void DeleteGroup(Group group);
        Task<IEnumerable<Group>> SearchGroups(GroupSearchResource searchTerm);
        Task<Customer> GetCustomerByName(string customer);
        Task<Order> GetOrderByNumber(string order);
        Task<Customer> GetCustomerByNumber(string customerNumber);
        Task<Order> GetOrderByOrderNumber(string orderNumber);
        Task<List<Group>> GetGroupsByOrder(int orderID);
        Task<int> GetGroupBatchIdByCustomerName(string customerName);
        Task<int> GetLastBatchNumber(string customer);
        Task<bool> IsGroupExistsWithCustomer(int customerId);
        Task CreateGroupsBulk(string insertQuery);
        Task<List<ReadGroupResource>> ModifyGroup(List<CreateGroupResource> groupsResource, int customerId, string userId);
        Task<List<Group>> RetrieveCreatedGroups(int customerId);
        Task<Customer> GetCustomerById(int customerID);
    }
}
