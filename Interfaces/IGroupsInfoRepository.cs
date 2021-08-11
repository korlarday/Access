using Allprimetech.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Allprimetech.Interfaces
{
    public interface IGroupsInfoRepository
    {
        Task<List<GroupsInfo>> AllGroupsInfos(int customerId);
        Task<GroupsInfo> GetGroupsInfo(int id);
        Task AddGroupsInfo(GroupsInfo groupInfo);
        void DeleteGroupsInfo(GroupsInfo groupInfo);
        Task<GroupsInfo> GetGroupsInfoByName(string name);
        Task<Group> GetGroupById(int groupID);
        Task<Customer> GetCustomerById(int customerID);
        Task<List<GroupsInfo>> GetGroupsInfo(int numOfItems, int customerId);
        Task AddGroupsInfoVerification(GroupInfoVerification groupInfo, bool isGroupInfoExist);
        Task<bool> IsGroupInfoExistInVerification(int groupID, int customerID);
        Task<bool> DeleteAllGroupInfoVerification(int customerId);
    }
}
