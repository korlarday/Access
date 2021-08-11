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
    public class GroupsInfoBL
    {
        #region Declarations
        private IGroupsInfoRepository _GroupInfoRepository { get; set; }
        private IMapper _Mapper { get; set; }
        private IUnitOfWork _UnitOfWork { get; set; }
        public SystemAuditsBL _SystemAuditsBL { get; set; }

        public GroupsInfoBL(IGroupsInfoRepository groupsRepository, IMapper mapper, IUnitOfWork unitOfWork, ISystemAuditRepository auditRepository)
        {
            _GroupInfoRepository = groupsRepository;
            _Mapper = mapper;
            _UnitOfWork = unitOfWork;
            _SystemAuditsBL = new SystemAuditsBL(auditRepository, mapper, unitOfWork);
            GroupInfoContrStatus._Error = GROUPINFO_ERROR.NO_ERROR;
        }
        #endregion

        private async Task SaveOperation(string operatorId, Operation operation, Source source)
        {
            await _SystemAuditsBL.StoreOperation(operation, source, operatorId);
        }

        public async Task<IEnumerable<ReadGroupsInfoResource>> AllGroupsInfos(string userId, int customerId)
        {
            try
            {
                var groupsInfos = await _GroupInfoRepository.AllGroupsInfos(customerId);
                var groupsInfosResource = _Mapper.Map<List<GroupsInfo>, List<ReadGroupsInfoResource>>(groupsInfos);
                await SaveOperation(userId, Operation.ReadAll, Source.GroupsInfo);
                return groupsInfosResource;
            }
            catch (Exception ex)
            {
                Logs.logError("GroupsInfoBL", "AllGroupsInfos", ex.Message);
                GroupInfoContrStatus._Error = GROUPINFO_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<ReadGroupsInfoResource> GetGroupsInfo(string userId, int groupsInfoId)
        {
            try
            {
                await SaveOperation(userId, Operation.ReadSingle, Source.GroupsInfo);

                var groupsInfo = await _GroupInfoRepository.GetGroupsInfo(groupsInfoId);

                if (groupsInfo == null)
                {
                    GroupInfoContrStatus._Error = GROUPINFO_ERROR.NOT_FOUND;
                    return null;
                }

                var groupsInfoResource = _Mapper.Map<GroupsInfo, ReadGroupsInfoResource>(groupsInfo);
                return groupsInfoResource;
            }
            catch (Exception ex)
            {
                Logs.logError("GroupsInfoBL", "GetGroupsInfo", ex.Message);
                GroupInfoContrStatus._Error = GROUPINFO_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<ReadGroupsInfoResource> UpdateGroupsInfo(string userId, int groupsInfoId, CreateGroupsInfoResource model)
        {
            try
            {
                var group = await _GroupInfoRepository.GetGroupById(model.GroupID);
                var customer = await  _GroupInfoRepository.GetCustomerById(model.CustomerID);
                var groupsInfo = await _GroupInfoRepository.GetGroupsInfo(groupsInfoId);
                if (group == null || customer == null || groupsInfo == null)
                {
                    GroupInfoContrStatus._Error = GROUPINFO_ERROR.NOT_FOUND;
                    return null;
                }

                // update the groupsInfo
                _Mapper.Map(model, groupsInfo);
                await _UnitOfWork.CompleteAsync();

                await SaveOperation(userId, Operation.Updated, Source.GroupsInfo);
                var result = _Mapper.Map<GroupsInfo, ReadGroupsInfoResource>(groupsInfo);
                return result;
            }
            catch (Exception ex)
            {
                Logs.logError("GroupsInfoBL", "UpdateGroupsInfo", ex.Message);
                GroupInfoContrStatus._Error = GROUPINFO_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<CreateItemResponse> CreateGroupsInfo(string userId, CreateGroupsInfoResource model)
        {
            try
            {
                var group = await _GroupInfoRepository.GetGroupById(model.GroupID);
                var customer = await  _GroupInfoRepository.GetCustomerById(model.CustomerID);
                if(group == null || customer == null)
                {
                    GroupInfoContrStatus._Error = GROUPINFO_ERROR.NOT_FOUND;
                    return new CreateItemResponse { _Succeeded = false, _Message = StringStore.InvalidCustomerOrGroup, _NewItem = null };
                }
                var groupsInfo = _Mapper.Map<CreateGroupsInfoResource, GroupsInfo>(model);

                await _GroupInfoRepository.AddGroupsInfo(groupsInfo);
                await _UnitOfWork.CompleteAsync();

                await SaveOperation(userId, Operation.Added, Source.GroupsInfo);

                var result = _Mapper.Map<GroupsInfo, ReadGroupsInfoResource>(groupsInfo);

                return new CreateItemResponse { _Succeeded = true, _Message = StringStore._Success, _NewItem = result };
            }
            catch (Exception ex)
            {
                Logs.logError("GroupsInfoBL", "CreateGroupsInfos", ex.Message);
                GroupInfoContrStatus._Error = GROUPINFO_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<int?> DeleteGroupsInfo(string userId, int groupsInfoId)
        {
            try
            {
                var groupsInfo = await _GroupInfoRepository.GetGroupsInfo(groupsInfoId);

                if (groupsInfo == null)
                {
                    GroupInfoContrStatus._Error = GROUPINFO_ERROR.NOT_FOUND;
                    return null;
                }

                // delete the groupsInfo
                _GroupInfoRepository.DeleteGroupsInfo(groupsInfo);
                await _UnitOfWork.CompleteAsync();

                await SaveOperation(userId, Operation.Deleted, Source.GroupsInfo);
                return groupsInfoId;
            }
            catch (Exception ex)
            {
                Logs.logError("GroupsInfoBL", "DeleteGroupsInfos", ex.Message);
                GroupInfoContrStatus._Error = GROUPINFO_ERROR.EXCEPTION;
                throw;
            }
        } 

        public async Task<bool> CreateGroupsInfos(string userId, List<CreateGroupsInfoResource> model)
        {
            try
            {
                var groupInfoItem = model[0];
                var group = await _GroupInfoRepository.GetGroupById(groupInfoItem.GroupID);
                var customer = await _GroupInfoRepository.GetCustomerById(groupInfoItem.CustomerID);
                if (group == null || customer == null)
                {
                    GroupInfoContrStatus._Error = GROUPINFO_ERROR.NOT_FOUND;
                    return false;
                }

                for (int i = 0; i < model.Count; i++)
                {
                    var grpInfoItem = model[i]; 
                    var groupsInfo = _Mapper.Map<CreateGroupsInfoResource, GroupsInfo>(grpInfoItem);

                    await _GroupInfoRepository.AddGroupsInfo(groupsInfo);
                }
                await _UnitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                Logs.logError("GroupsInfoBL", "CreateGroupsInfos", ex.Message);
                GroupInfoContrStatus._Error = GROUPINFO_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<bool> DeleteGroupInfoVerification(string userId, int customerId)
        {
            try
            {
                var groupsInfo = await _GroupInfoRepository.DeleteAllGroupInfoVerification(customerId);


                return true;
            }
            catch (Exception ex)
            {
                Logs.logError("GroupsInfoBL", "DeleteGroupInfoVerification", ex.Message);
                GroupInfoContrStatus._Error = GROUPINFO_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<bool> CreateGroupsInfoVerification(string userId, List<CreateGroupsInfoResource> model)
        {
            try
            {
                var groupInfoItem = model[0]; 
                var group = await _GroupInfoRepository.GetGroupById(groupInfoItem.GroupID);
                var customer = await _GroupInfoRepository.GetCustomerById(groupInfoItem.CustomerID);
                if (group == null || customer == null)
                {
                    GroupInfoContrStatus._Error = GROUPINFO_ERROR.NOT_FOUND;
                    return false;
                }

                bool isGroupInfoExist = await _GroupInfoRepository.IsGroupInfoExistInVerification(groupInfoItem.GroupID, groupInfoItem.CustomerID);

                for (int i = 0; i < model.Count; i++)
                {
                    var grpInfoItem = model[i];
                    var groupsInfo = _Mapper.Map<CreateGroupsInfoResource, GroupInfoVerification>(grpInfoItem);

                    await _GroupInfoRepository.AddGroupsInfoVerification(groupsInfo, isGroupInfoExist);
                }
                await _UnitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                Logs.logError("GroupsInfoBL", ex.Message, "CreateGroupsInfoVerification", ex);
                GroupInfoContrStatus._Error = GROUPINFO_ERROR.EXCEPTION;
                throw;
            }
        }


        public async Task<List<KeyCodeList>> GetKeyCodeList(string userId, int customerId)
        {
            try
            {
                var groupsInfo = await _GroupInfoRepository.GetGroupsInfo(StringStore.Limit, customerId);
                var groupsInfos = _Mapper.Map<List<GroupsInfo>, List<ReadGroupInfoCodeList>>(groupsInfo);

                List<KeyCodeList> keyCodeLists = new List<KeyCodeList>();

                groupsInfos = groupsInfos.OrderBy(x => x._Row).ToList();
                int count = groupsInfos.Select(x => x.GroupID).Distinct().Count();
                var groupIds = groupsInfos.Select(x => x.GroupID).Distinct().ToList();

                for (int i = 0; i < groupIds.Count(); i++)
                {
                    var groupItems = groupsInfos.Where(x => x.GroupID == groupIds[i]).ToList();
                    groupItems = groupItems.OrderBy(x => x._Row).ToList();
                    var keyRow = groupIds.Count < i ? groupItems[i] : groupItems[0];

                    KeyCodeList keyCL = new KeyCodeList();
                    keyCL._GroupNumber = keyRow._GroupNumber;
                    keyCL.Values = new List<CodeListValue>();

                    var slot = groupItems.Select(x => x._Slot).Distinct().ToList();
                    var row = groupItems.Select(x => x._Row).Distinct().ToList();

                    for (int j = 0; j < row.Count; j++)
                    {
                        for (int k = 0; k < slot.Count; k++)
                        {
                            var item = groupItems.Where(x => x._Slot == k + 1 && x._Row == j + 1).SingleOrDefault();
                            CodeListValue codeLV = new CodeListValue();
                            codeLV._Slot = item._Slot;
                            codeLV._Row = item._Row;
                            codeLV._Value = item._Value;

                            keyCL.Values.Add(codeLV);
                        }
                    }

                    keyCodeLists.Add(keyCL);
                }

                return keyCodeLists;
            }
            catch (Exception ex)
            {
                Logs.logError("GroupsInfoBL", "GetKeyCodeList", ex.Message);
                GroupInfoContrStatus._Error = GROUPINFO_ERROR.EXCEPTION;
                throw;
            }
        }
    }

    public class KeyCodeList
    {
        public string _GroupNumber { get; set; }
        public List<CodeListValue> Values { get; set; }
    }
    public class CodeListValue
    {
        public int _Slot { get; set; }
        public int _Row { get; set; }
        public int _Value { get; set; }
    }
}
