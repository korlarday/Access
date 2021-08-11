using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Allprimetech.Interfaces
{
    public interface IDiscRepository
    {
        Task<List<Disc>> GetDiscs(int numOfItems, int customerId);
        Task<Disc> GetDisc(int id);
        Task AddDisc(Disc customer);
        void DeleteDisc(Disc customer);
        Task<Disc> GetDiscByName(string name);
        Task<Customer> GetCustomerById(int customerID);
        Task<Cylinder> GetCylinderById(int cylinderID);
        Task<List<GroupsInfo>> GetGroupsInfo(int numOfItems, int customerId);
        Task<List<ReadGroupInfoCodeList>> GetGroupsInfoBruckner(int numOfItems, int customerId);
        Task<List<ReadDiscWithOccurrence>> GetDiscsStatisticsBruckner(int numOfItems, int customerId);
        Task<List<ReadDiscInfo>> GetDiscsInfo(int numOfItems, int customerId);
        Task<List<ReadDiscWithOccurrence>> GetDiscsTypes(int numOfItems, int customerId);
        Task<List<ReadDiscInfo>> GetCylinderCodeList(int customerId);
        Task<List<ReadDiscWithOccurrence>> GetDiscsStatistics(int limit, int customerId);
        Task DeleteDiscs(int customerId);
        //Task<IEnumerable<ReadDiscResource>> SearchDiscs(DiscSearchResource searchTerm);
    }
}
