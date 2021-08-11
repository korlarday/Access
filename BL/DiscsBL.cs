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
    public class DiscsBL
    {
        #region Declarations
        private IDiscRepository _DiscRepository { get; set; }
        private IMapper _Mapper { get; set; }
        private IUnitOfWork _UnitOfWork { get; set; }
        public SystemAuditsBL _SystemAuditsBL { get; set; }

        public DiscsBL(IDiscRepository discsRepository, IMapper mapper, IUnitOfWork unitOfWork, ISystemAuditRepository auditRepository)
        {
            _DiscRepository = discsRepository;
            _Mapper = mapper;
            _UnitOfWork = unitOfWork;
            _SystemAuditsBL = new SystemAuditsBL(auditRepository, mapper, unitOfWork);
        }
        #endregion

        private async Task SaveOperation(string operatorId, Operation operation, Source source)
        {
            await _SystemAuditsBL.StoreOperation(operation, source, operatorId);
        }

        public async Task<IEnumerable<ReadDiscResource>> GetDiscs(string userId, int customerId)
        {
            try
            {
                var discs = await _DiscRepository.GetDiscs(StringStore.Limit, customerId);
                var discsResource = _Mapper.Map<List<Disc>, List<ReadDiscResource>>(discs);
                await SaveOperation(userId, Operation.ReadAll, Source.Disc);
                return discsResource;
            }
            catch (Exception ex)
            {
                Logs.logError("DiscsBL", "GetDiscs", ex.Message);
                DiscControllerStatus._Error = DISC_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<ReadDiscResource> GetDisc(string userId, int discId)
        {
            try
            {
                await SaveOperation(userId, Operation.ReadSingle, Source.Disc);

                var disc = await _DiscRepository.GetDisc(discId);

                if (disc == null)
                {
                    DiscControllerStatus._Error = DISC_ERROR.NOT_FOUND;
                    return null;
                }

                var discResource = _Mapper.Map<Disc, ReadDiscResource>(disc);
                return discResource;
            }
            catch (Exception ex)
            {
                Logs.logError("DiscsBL", "GetDisc", ex.Message);
                DiscControllerStatus._Error = DISC_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<ReadDiscResource> UpdateDisc(string userId, int discId, CreateDiscResource model)
        {
            try
            {
                var disc = await _DiscRepository.GetDisc(discId);

                if (disc == null)
                {
                    DiscControllerStatus._Error = DISC_ERROR.NOT_FOUND;
                    return null;
                }

                // update the disc
                _Mapper.Map(model, disc);
                await _UnitOfWork.CompleteAsync();

                await SaveOperation(userId, Operation.Updated, Source.Disc);
                var result = _Mapper.Map<Disc, ReadDiscResource>(disc);
                return result;
            }
            catch (Exception ex)
            {
                Logs.logError("DiscsBL", "UpdateDisc", ex.Message);
                DiscControllerStatus._Error = DISC_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<CreateItemResponse> CreateDisc(string userId, CreateDiscResource model)
        {
            try
            {
                var customer = await _DiscRepository.GetCustomerById(model._CustomerID);
                var cylinder = await _DiscRepository.GetCylinderById(model._CylinderID);
                if(customer == null || cylinder == null)
                {
                    return new CreateItemResponse { _Succeeded = false, _Message = StringStore.InvalidCustomerOrCylinder, _NewItem = null };
                }

                // check if disc exists
                var checkDisc = await _DiscRepository.GetDiscByName(model._Name);
                if (checkDisc != null)
                {
                    // disc exists
                    return new CreateItemResponse { _Succeeded = false, _Message = StringStore.DiscExists, _NewItem = null };
                }

                var disc = _Mapper.Map<CreateDiscResource, Disc>(model);

                //disc.CreatedById = userId;

                await _DiscRepository.AddDisc(disc);
                await _UnitOfWork.CompleteAsync();

                await SaveOperation(userId, Operation.Added, Source.Disc);

                var result = _Mapper.Map<Disc, ReadDiscResource>(disc);

                return new CreateItemResponse { _Succeeded = true, _Message = StringStore._Success, _NewItem = result };
            }
            catch (Exception ex)
            {
                Logs.logError("DiscsBL", "CreateDisc", ex.Message);
                DiscControllerStatus._Error = DISC_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<int?> DeleteDisc(string userId, int discId)
        {
            try
            {
                var disc = await _DiscRepository.GetDisc(discId);

                if (disc == null)
                {
                    DiscControllerStatus._Error = DISC_ERROR.NOT_FOUND;
                    return null;
                }

                // delete the disc
                _DiscRepository.DeleteDisc(disc);
                await _UnitOfWork.CompleteAsync();

                await SaveOperation(userId, Operation.Deleted, Source.Disc);
                return discId;
            }
            catch (Exception ex)
            {
                Logs.logError("DiscsBL", "DeleteDisc", ex.Message);
                DiscControllerStatus._Error = DISC_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<bool> CreateDiscs(string userId, List<CreateDiscResource> model)
        {
            try
            {
                var data = model[0];
                var customer = await _DiscRepository.GetCustomerById(data._CustomerID);
                var cylinder = await _DiscRepository.GetCylinderById(data._CylinderID);
                if (customer == null || cylinder == null)
                {
                    DiscControllerStatus._Error = DISC_ERROR.NOT_FOUND;
                    return false;
                }


                foreach (var discItem in model)
                {
                    var disc = _Mapper.Map<CreateDiscResource, Disc>(discItem);

                    //disc.CreatedById = userId;

                    await _DiscRepository.AddDisc(disc);
                }

                await _UnitOfWork.CompleteAsync();

                await SaveOperation(userId, Operation.Added, Source.Disc);

                return true;
            }
            catch (Exception ex)
            {
                Logs.logError("DiscsBL", "CreateDiscs", ex.Message);
                DiscControllerStatus._Error = DISC_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<List<ReadGroupInfoCodeList>> GetGroupsInfo(string userId, int customerId)
        {
            try
            {
                var groupsInfo = await _DiscRepository.GetGroupsInfo(StringStore.Limit, customerId);
                var result = _Mapper.Map<List<GroupsInfo>, List<ReadGroupInfoCodeList>>(groupsInfo); 
                return result;
            }
            catch (Exception ex)
            {
                Logs.logError("DiscsBL", "GetGroupsInfo", ex.Message);
                DiscControllerStatus._Error = DISC_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<List<ReadGroupInfoCodeList>> GetGroupsInfoBruckner(string userId, int customerId)
        {
            try
            {
                var groupsInfo = await _DiscRepository.GetGroupsInfoBruckner(StringStore.Limit, customerId);
                //var result = _Mapper.Map<List<GroupsInfo>, List<ReadGroupInfoCodeList>>(groupsInfo);
                return groupsInfo;
            }
            catch (Exception ex)
            {
                Logs.logError("DiscsBL", "GetGroupsInfo", ex.Message);
                DiscControllerStatus._Error = DISC_ERROR.EXCEPTION;
                throw;
            }
        }


        public async Task<List<ReadDiscInfo>> GetDiscsInfo(string userId, int customerId)
        {
            try
            {
                var groupsInfo = await _DiscRepository.GetDiscsInfo(StringStore.Limit, customerId);
                return groupsInfo;
            }
            catch (Exception ex)
            {
                Logs.logError("DiscsBL", "GetDiscsInfo", ex.Message);
                DiscControllerStatus._Error = DISC_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<List<ReadDiscWithOccurrence>> GetDiscsTypes(string userId, int customerId)
        {
            try
            {
                var groupsInfo = await _DiscRepository.GetDiscsTypes(StringStore.Limit, customerId);
                return groupsInfo;
            }
            catch (Exception ex)
            {
                Logs.logError("DiscsBL", "GetDiscsTypes", ex.Message);
                DiscControllerStatus._Error = DISC_ERROR.EXCEPTION;
                throw;
            }
        }
        public async Task<List<ReadDiscWithOccurrence>> GetDiscStatistics(string userId, int customerId)
        {
            try
            { 
                var groupsInfo = await _DiscRepository.GetDiscsStatistics(StringStore.Limit, customerId);
                return groupsInfo;
            }
            catch (Exception ex)
            {
                Logs.logError("DiscsBL", "GetDiscStatistics", ex.Message);
                DiscControllerStatus._Error = DISC_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<List<ReadDiscWithOccurrence>> GetDiscStatisticsBruckner(string userId, int customerId)
        {
            try
            {
                var groupsInfo = await _DiscRepository.GetDiscsStatisticsBruckner(StringStore.Limit, customerId);
                return groupsInfo;
            }
            catch (Exception ex)
            {
                Logs.logError("DiscsBL", "GetDiscStatistics", ex.Message);
                DiscControllerStatus._Error = DISC_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<List<ReadDiscInfo>> GetCylinderCodeList(string userId, int customerId)
        {
            try
            {
                var cylinderCodeList = await _DiscRepository.GetCylinderCodeList(customerId);
                return cylinderCodeList;
            }
            catch (Exception ex)
            {
                Logs.logError("DiscsBL", "GetCylinderCodeList", ex.Message);
                DiscControllerStatus._Error = DISC_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<DiscTypeResponse> GetDiscsNumber(string userId, int customerId)
        {
            try
            {
                var discTypes = await _DiscRepository.GetDiscsTypes(StringStore.Limit, customerId);

                var response = new DiscTypeResponse();
                
                var discsA = discTypes.Where(x => x._Name.ToLower().Contains("a")).ToList();
                var discsB = discTypes.Where(x => x._Name.ToLower().Contains("b")).ToList();
                response._DiscsA = discsA;
                response._DiscsB = discsB;
                return response;
            }
            catch (Exception ex)
            {
                Logs.logError("DiscsBL", "GetDiscsNumber", ex.Message);
                DiscControllerStatus._Error = DISC_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<List<CylinderInGrp>> GetCylindersInGroups(string userId, int customerId)
        {
            try
            {
                var cylinderCodeLists = await _DiscRepository.GetCylinderCodeList(customerId);
                List<CylinderInGrp> cylinderInGrps = new List<CylinderInGrp>();

                var groups = cylinderCodeLists.Select(x => x._GroupNumbers).Distinct().ToList();

                for (int i = 0; i < groups.Count(); i++)
                {
                    Console.WriteLine("Group " + (i + 1));
                    var group = groups[i];
                    var cylinderDiscs = cylinderCodeLists.Where(x => x._GroupNumbers == group).ToList();
                    var cylinderIds = cylinderDiscs.Select(x => x.CylinderID).Distinct().ToList();

                    var cylInGrpItem = new CylinderInGrp();
                    cylInGrpItem._Group = group;
                    cylInGrpItem._Quantity = cylinderIds.Count();
                    cylInGrpItem._CylinderDiscs = cylinderDiscs;

                    cylinderInGrps.Add(cylInGrpItem);
                }
                return cylinderInGrps;
            }
            catch (Exception ex)
            {
                Logs.logError("DiscsBL", "GetCylindersInGroups", ex.Message);
                DiscControllerStatus._Error = DISC_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<object> DeleteDiscs(string userId, int customerId)
        {
            try
            {
                await _DiscRepository.DeleteDiscs(customerId);
                return null;

            }
            catch (Exception ex)
            {
                Logs.logError("DiscsBL", "DeleteDiscs", ex.Message);
                DiscControllerStatus._Error = DISC_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<List<CylinderSeiteInfo>> GetCylinderSeiteInfo(string userId, int customerId)
        {
            try
            {
                var cylinderCodeLists = await _DiscRepository.GetCylinderCodeList(customerId);

                List<CylinderSeiteInfo> cylinderSeites = new List<CylinderSeiteInfo>();

                var groups = cylinderCodeLists.Select(x => x._GroupNumbers).Distinct().ToList();

                for (int i = 0; i < groups.Count(); i++)
                {
                    Console.WriteLine("Group " + (i + 1));
                    var group = groups[i];
                    var cylinderDiscs = cylinderCodeLists.Where(x => x._GroupNumbers == group).ToList();
                    var cylinderIds = cylinderDiscs.Select(x => x.CylinderID).Distinct().ToList();

                    var quantity = cylinderIds.Count();
                

                    // write the disc type
                    var cylItem = cylinderDiscs.FirstOrDefault();
                    var cylDiscs = cylinderDiscs.Where(x => x.CylinderID == cylItem.CylinderID).ToList();
                    bool swap = true;

                    var seiteA = new List<string>();
                    var seiteB = new List<string>();

                    for (int j = 1; j <= 6; j++)
                    {
                        var dcs = cylDiscs.Where(x => x._Slot == j).ToList();
                        if (swap)
                        {
                            dcs = dcs.OrderBy(x => x._Type).ToList();
                            foreach (var item in dcs)
                            {
                                seiteA.Add(item._Name);
                            }
                        }
                        else
                        {
                            dcs = dcs.OrderByDescending(x => x._Type).ToList();
                            foreach (var item in dcs)
                            {
                                seiteB.Add(item._Name);
                            }
                        }
                        swap = !swap;
                    }


                    for (int j = 0; j < cylinderIds.Count; j++)
                    {
                        int cylinderId = cylinderIds[j];
                        var cylinder = cylinderDiscs.Where(x => x.CylinderID == cylinderId).FirstOrDefault();

                        var seiteInfo = new CylinderSeiteInfo();

                        seiteInfo._CylinderID = cylinder.CylinderID;
                        seiteInfo._CylinderNumber = cylinder._CylinderNumber;
                        seiteInfo._DoorName = cylinder._DoorName;
                        seiteInfo._LengthInside = cylinder._LengthInside;
                        seiteInfo._LengthOutside = cylinder._LengthOutside;
                        seiteInfo._SeiteA = seiteA;
                        seiteInfo._SeiteB = seiteB;
                        seiteInfo._Quantity = quantity;

                        cylinderSeites.Add(seiteInfo);
                    }


                }
                return cylinderSeites;
            }
            catch (Exception ex)
            {
                Logs.logError("DiscsBL", "GetCylinderSeiteInfo", ex.Message);
                DiscControllerStatus._Error = DISC_ERROR.EXCEPTION;
                throw;
            }
        }
    
    }

    public class DiscTypeResponse
    {
        public List<ReadDiscWithOccurrence> _DiscsA { get; set; }
        public List<ReadDiscWithOccurrence> _DiscsB { get; set; }
    }
    public class CylinderInGrp
    {
        public string _Group { get; set; }
        public int _Quantity { get; set; }
        public List<ReadDiscInfo> _CylinderDiscs { get; set; }
    }

    public class CylinderSeiteInfo
    {
        public int _CylinderID { get; set; }
        public string _CylinderNumber { get; set; }
        public string _DoorName { get; set; }
        public string _LengthInside { get; set; }
        public string _LengthOutside { get; set; }
        public List<string> _SeiteA { get; set; }
        public List<string> _SeiteB { get; set; }
        public int _Quantity { get; set; }
    }
}
