using Allprimetech.GeneralUtils;
using Allprimetech.Interfaces;
using Allprimetech.Interfaces.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allprimetech.DAL
{
    public class GroupsInfoRepository : IGroupsInfoRepository
    {
        private ApplicationDbContext _Context { get; set; }
        public GroupsInfoRepository(ApplicationDbContext context)
        {
            _Context = context;
        }
        public async Task AddGroupsInfo(GroupsInfo groupInfo)
        {
            try
            {
                groupInfo._CreationDate = DateTime.UtcNow;
                groupInfo._UpdatedDate = DateTime.UtcNow;
                await _Context.GroupsInfos.AddAsync(groupInfo);
            }
            catch (Exception ex)
            {
                Logs.logError("GroupsInfoRepository", "AddGroupsInfo", ex.Message);
                throw;
            }
        }

        public async Task AddGroupsInfoVerification(GroupInfoVerification groupInfo, bool isGroupInfoExist)
        {
            try
            {
                if(!isGroupInfoExist)
                {
                    groupInfo._CreationDate = DateTime.UtcNow;
                    groupInfo._UpdatedDate = DateTime.UtcNow;
                    await _Context.GroupInfoVerifications.AddAsync(groupInfo);
                }
                else
                {
                    var existingGroupInfo = await _Context.GroupInfoVerifications.Where(x => x.GroupID == groupInfo.GroupID
                                                                                        && x.CustomerID == groupInfo.CustomerID
                                                                                        && x._Slot == groupInfo._Slot
                                                                                        && x._Row == groupInfo._Row)
                                                                                .SingleOrDefaultAsync();
                    if(existingGroupInfo != null)
                    {
                        existingGroupInfo._Value = groupInfo._Value;
                        existingGroupInfo._UpdatedDate = DateTime.UtcNow;
                        _Context.SaveChanges();
                    }
                    else
                    {
                        groupInfo._CreationDate = DateTime.UtcNow;
                        groupInfo._UpdatedDate = DateTime.UtcNow;
                        await _Context.GroupInfoVerifications.AddAsync(groupInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.logError("GroupsInfoRepository", "AddGroupsInfoVerification", ex.Message);
                throw;
            }
        }

        public async Task<List<GroupsInfo>> AllGroupsInfos(int customerId)
        {
            try
            {
                return await _Context.GroupsInfos
                            .Where(x => x.CustomerID == customerId)
                            .Include(x => x.Customer)
                            .ToListAsync();
            }
            catch (Exception ex)
            {
                Logs.logError("GroupsInfoRepository", "AllGroupsInfos", ex.Message);
                throw;
            }
        }

        public void DeleteGroupsInfo(GroupsInfo groupInfo)
        {
            _Context.GroupsInfos.Remove(groupInfo);
        }

        public async Task<GroupsInfo> GetGroupsInfo(int id)
        {
            try
            {
                return await _Context.GroupsInfos.FindAsync(id);
            }
            catch (Exception ex)
            {
                Logs.logError("GroupsInfoRepository", "GetGroupsInfo", ex.Message);
                throw;
            }
        }

        public Task<GroupsInfo> GetGroupsInfoByName(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<Group> GetGroupById(int groupID)
        {
            try
            {
                return await _Context.Groups.FindAsync(groupID);

            }
            catch (Exception ex)
            {
                Logs.logError("GroupsInfoRepository", "GetGroupInfoById", ex.Message);
                throw;
            }
        }

        public async Task<Customer> GetCustomerById(int customerID)
        {
            try
            {
                return await _Context.Customers.FindAsync(customerID);

            }
            catch (Exception ex)
            {
                Logs.logError("GroupsInfoRepository", "GetCustomerById", ex.Message);
                throw;
            }
        }

        public async Task<List<GroupsInfo>> GetGroupsInfo(int numOfItems, int customerId)
        {
            try
            {
                var groupInfos = await _Context.GroupsInfos
                                    .Where(x => x.CustomerID == customerId)
                                    .Include(x => x.Customer)
                                    .Include(x => x.Group)
                                    .OrderBy(x => x.GroupID)
                                    .ToListAsync();
                return groupInfos.Take(numOfItems).ToList();
            }
            catch (Exception ex)
            {
                Logs.logError("GroupsInfoRepository", "GetGroupsInfo", ex.Message);
                return null;
            }
        }

        public async Task<bool> IsGroupInfoExistInVerification(int groupID, int customerID)
        {
            return await _Context.GroupInfoVerifications.AnyAsync(x => x.GroupID == groupID && x.CustomerID == customerID);
        }

        public async Task<bool> DeleteAllGroupInfoVerification(int customerId)
        {
            var groupInfos = await _Context.GroupInfoVerifications.Where(x => x.CustomerID == customerId).ToListAsync();

            if(groupInfos.Count > 0)
            {
                _Context.GroupInfoVerifications.RemoveRange(groupInfos);

                await _Context.SaveChangesAsync();
            }

            return true;
        }
    }
}
