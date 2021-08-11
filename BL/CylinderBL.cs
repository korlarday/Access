using Allprimetech.GeneralUtils;
using Allprimetech.Interfaces;
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
    public class CylinderBL
    {
        #region Declarations
        private ICylinderRepository _CylinderRepository { get; set; }
        private IMapper _Mapper { get; set; }
        private IUnitOfWork _UnitOfWork { get; set; }
        private SystemAuditsBL _SystemAuditBL { get; set; }

        public CylinderBL(ICylinderRepository cylinderRepository, IMapper mapper, IUnitOfWork unitOfWork, ISystemAuditRepository auditRepository)
        {
            _CylinderRepository = cylinderRepository;
            _Mapper = mapper;
            _UnitOfWork = unitOfWork;
            _SystemAuditBL = new SystemAuditsBL(auditRepository, mapper, unitOfWork);
            ProductControllerStatus._Error = PRODUCT_ERROR.NO_ERROR;
        }
        #endregion

        private async Task SaveOperation(string operatorId, Operation operation, Source source)
        {
            await _SystemAuditBL.StoreOperation(operation, source, operatorId);
        }

        public async Task<IEnumerable<ReadCylinderResource>> AllCylinders(string userId, int customerId)
        {
            try
            {
                var cylinders = await _CylinderRepository.AllCylinders(customerId);
                await SaveOperation(userId, Operation.ReadAll, Source.Cylinder);
                var cylindersResource = _Mapper.Map<List<Cylinder>, List<ReadCylinderResource>>(cylinders);
                return cylindersResource;
            }
            catch (Exception ex)
            {
                ProductControllerStatus._Error = PRODUCT_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), "AllCylinders", ex.Message);
                throw;
            }
        }

        public async Task<ReadCylinderResource> GetCylinder(string userId, int cylinderId)
        {
            try
            {
                var cylinder = await _CylinderRepository.GetCylinder(cylinderId);

                if (cylinder == null)
                {
                    ProductControllerStatus._Error = PRODUCT_ERROR.NOT_FOUND;
                    return null;
                }
                await SaveOperation(userId, Operation.ReadSingle, Source.Cylinder);
                var cylinderResource = _Mapper.Map<Cylinder, ReadCylinderResource>(cylinder);
                return cylinderResource;
            }
            catch (Exception)
            {
                ProductControllerStatus._Error = PRODUCT_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<ReadCylinderResource> UpdateCylinder(string userId, int cylinderId, CreateCylinderResource model)
        {
            try
            {
                var cylinder = await _CylinderRepository.GetCylinder(cylinderId);

                if (cylinder == null)
                {
                    ProductControllerStatus._Error = PRODUCT_ERROR.NOT_FOUND;
                    return null;
                }

                // update the cylinder
                _Mapper.Map(model, cylinder);
                cylinder._UpdatedDate = DateTime.UtcNow;
                await _UnitOfWork.CompleteAsync();

                await SaveOperation(userId, Operation.Updated, Source.Cylinder);

                var result = _Mapper.Map<Cylinder, ReadCylinderResource>(cylinder);
                return result;
            }
            catch (Exception ex)
            {
                ProductControllerStatus._Error = PRODUCT_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), "UpdateCylinder", ex.Message);
                throw;
            }
        }

        public async Task<ReadCylinderResource> CreateCylinder(string userId, CreateCylinderResource model)
        {
            try
            {
                var cylinderItem = model;
                int customerId = 0;
                int orderId = 0;
                if (cylinderItem != null)
                {
                    Customer customer = await _CylinderRepository.GetCustomerByName(cylinderItem._Customer);
                    customerId = customer != null ? customer.CustomerID : 0;
                    Order order = await _CylinderRepository.GetOrderByNumber(cylinderItem._Order);
                    orderId = order != null ? order.OrderID : 0;
                }
                var cylinder = _Mapper.Map<CreateCylinderResource, Cylinder>(model);

                //cylinder. = userId;
                cylinder.CustomerID = customerId;
                cylinder.OrderID = orderId;
                await _CylinderRepository.AddCylinder(cylinder, userId);
                await _UnitOfWork.CompleteAsync();

                await SaveOperation(userId, Operation.Added, Source.Cylinder);

                var result = _Mapper.Map<Cylinder, ReadCylinderResource>(cylinder);

                return result;
            }
            catch (Exception ex)
            {
                ProductControllerStatus._Error = PRODUCT_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.CreateCylinder.ToString(), ex.Message);
                return null;
            }
        }

        public async Task<List<ReadCylinderResource>> CreateCylinders(string userId, List<CreateCylinderResource> cylinders)
        {
            try
            {
                List<ReadCylinderResource> createdCylinders = new List<ReadCylinderResource>();
                var cylinderItem = cylinders[0];
                int customerId = 0;
                int orderId = 0; 
                int batchNumber = 1;
                if (cylinderItem != null)
                {
                    Customer customer = await _CylinderRepository.GetCustomerByName(cylinderItem._Customer);
                    customerId = customer != null ? customer.CustomerID : 0;
                    Order order = await _CylinderRepository.GetOrderByNumber(cylinderItem._Order);
                    orderId = order != null ? order.OrderID : 0;

                    //check if there is an existing cylinder for this customer in database
                    if (order != null && customer != null)
                    {
                        bool cylindersExists = await _CylinderRepository.IsCylinderExistsWithCustomer(customerId);
                        if (cylindersExists)
                        {
                            order._CylinderQty += cylinders.Sum(x => x._Quantity);
                            await _UnitOfWork.CompleteAsync();
                        }
                    }


                    int lastBatch = await _CylinderRepository.GetLastBatchNumber(cylinderItem._Customer);
                    batchNumber = lastBatch + 1;
                }
            
                string insertQuery = "";
                foreach (var item in cylinders)
                {
                    var cylinder = _Mapper.Map<CreateCylinderResource, Cylinder>(item);

                    
                    cylinder.CustomerID = customerId;
                    
                        cylinder.OrderID = orderId;
                    cylinder._BatchNumber = batchNumber;

                    if(item._Quantity != 0)
                    {
                        insertQuery += String.Format(@"('{0}', '{1}', '{2}', {3}, '{4}', '{5}', '{6}', {7}, {8}, {9}, {10}, 
                                                        {11}, {12}, {13}, {14}, {15}, {16}, '{17}', {18}),", 
                            cylinder._DoorName, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), (int)cylinder._ArticleNumber, cylinder._LengthOutside,
                            cylinder._LengthInside, cylinder._Color, (int)cylinder._Options, cylinder._Quantity, cylinder._QRCodeIssued,
                            cylinder._Assembled, cylinder._Blocked, cylinder._Validated, cylinder._PositionId, cylinder.CustomerID,
                            cylinder.OrderID, cylinder._Reclaimed, cylinder._CylinderNumber, batchNumber);

                    }
                    else
                    {
                        insertQuery += String.Format(@"('{0}', '{1}', '{2}', {3}, '{4}', '{5}', '{6}', {7}, {8}, {9}, {10}, 
                                                        {11}, {12}, {13}, {14}, {15}, {16}, '{17}', {18}),",
                            cylinder._DoorName, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), (int)cylinder._ArticleNumber, cylinder._LengthOutside,
                            cylinder._LengthInside, cylinder._Color, (int)cylinder._Options, cylinder._Quantity, cylinder._QRCodeIssued,
                            cylinder._Assembled, cylinder._Blocked, cylinder._Validated, cylinder._PositionId, cylinder.CustomerID,
                            "NULL", cylinder._Reclaimed, cylinder._CylinderNumber, batchNumber);
                    }

                }
                await _CylinderRepository.AddCylinderInBulk(insertQuery);

                var storedCylinders = await _CylinderRepository.RetrieveStoredCylinders(customerId);
                createdCylinders = _Mapper.Map<List<Cylinder>, List<ReadCylinderResource>>(storedCylinders);
                //await _UnitOfWork.CompleteAsync();

                await SaveOperation(userId, Operation.Added, Source.Cylinder);

                //var result = _Mapper.Map<Cylinder, ReadCylinderResource>(cylinder);

                return createdCylinders;
            }
            catch (Exception ex)
            {
                ProductControllerStatus._Error = PRODUCT_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.CreateCylinders.ToString(), ex.Message);
                return null;
            }
        }

        public async Task<List<ReadCylinderResource>> ModifyCylinders(string userId, List<CreateCylinderResource> cylinders)
        {
            try
            {
                List<ReadCylinderResource> createdCylinders = new List<ReadCylinderResource>();
                var cylinderItem = cylinders[0];
                int customerId = 0;
                if (cylinderItem != null)
                {
                    Customer customer = await _CylinderRepository.GetCustomerById(cylinderItem._CustomerID);
                    customerId = customer != null ? customer.CustomerID : 0;
                }

                if(customerId == 0)
                {
                    ProductControllerStatus._Error = PRODUCT_ERROR.NOT_FOUND;
                    return null;
                }

                var response = await _CylinderRepository.ModifyCylinders(cylinders, customerId, userId);
                //await _UnitOfWork.CompleteAsync();

                await SaveOperation(userId, Operation.Added, Source.Cylinder);

                //var result = _Mapper.Map<Cylinder, ReadCylinderResource>(cylinder);

                return response;
            }
            catch (Exception ex)
            {
                ProductControllerStatus._Error = PRODUCT_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.CreateCylinders.ToString(), ex.Message);
                return null;
            }
        }


        //public async Task<List<ReadCylinderResource>> CreateCylinders(string userId, List<CreateCylinderResource> cylinders)
        //{
        //    List<ReadCylinderResource> createdCylinders = new List<ReadCylinderResource>();
        //    var cylinderItem = cylinders[0];
        //    int customerId = 0;
        //    int orderId = 0;
        //    int batchNumber = 1;
        //    if(cylinderItem != null)
        //    {
        //        Customer customer = await _CylinderRepository.GetCustomerByName(cylinderItem._Customer);
        //        customerId = customer != null ? customer.CustomerID : 0;
        //        Order order = await _CylinderRepository.GetOrderByNumber(cylinderItem._Order);
        //        orderId = order != null ? order.OrderID : 0;

        //        //check if there is an existing cylinder for this customer in database
        //        if(order != null && customer != null)
        //        {
        //            bool cylindersExists = await _CylinderRepository.IsCylinderExistsWithCustomer(customerId);
        //            if (cylindersExists)
        //            {
        //                order._CylinderQty += cylinders.Sum(x => x._Quantity);
        //                await _UnitOfWork.CompleteAsync();
        //            }
        //        }


        //        int lastBatch = await _CylinderRepository.GetLastBatchNumber(cylinderItem._Customer);
        //        batchNumber = lastBatch + 1;
        //    }
        //    try
        //    {
        //        foreach (var item in cylinders)
        //        {
        //            var cylinder = _Mapper.Map<CreateCylinderResource, Cylinder>(item);

        //            //cylinder. = userId;

        //            cylinder.CustomerID = customerId;
        //            cylinder.OrderID = orderId;
        //            cylinder._BatchNumber = batchNumber;
        //            await _CylinderRepository.AddCylinder(cylinder, userId);

        //            createdCylinders.Add(_Mapper.Map<Cylinder, ReadCylinderResource>(cylinder));
        //        }
        //        await _UnitOfWork.CompleteAsync();

        //        await SaveOperation(userId, Operation.Added, Source.Cylinder);

        //        //var result = _Mapper.Map<Cylinder, ReadCylinderResource>(cylinder);

        //        return createdCylinders;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.CreateCylinder.ToString(), ex.Message);
        //        return null;
        //    }
        //}


        public async Task<int?> DeleteCylinder(string userId, int cylinderId)
        {
            try
            {
                var cylinder = await _CylinderRepository.GetCylinder(cylinderId);

                if (cylinder == null)
                {
                    ProductControllerStatus._Error = PRODUCT_ERROR.NOT_FOUND;
                    return null;
                }

                // delete the cylinder
                _CylinderRepository.DeleteCylinder(cylinder);
                await _UnitOfWork.CompleteAsync();

                await SaveOperation(userId, Operation.Deleted, Source.Cylinder);

                return cylinderId;
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.BL.ToString(), "DeleteCylinder", ex.Message);
                ProductControllerStatus._Error = PRODUCT_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<IEnumerable<ReadCylinderResource>> SearchCylinders(string userId, CylinderSearchResource searchTerm)
        {
            try
            {
                var cylinders = await _CylinderRepository.SearchCylinders(searchTerm);

                await SaveOperation(userId, Operation.Search, Source.Customer); 

                var cylindersResource = _Mapper.Map<IEnumerable<Cylinder>, IEnumerable<ReadCylinderResource>>(cylinders);
                return cylindersResource;
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.BL.ToString(), "SearchCylinders", ex.Message);
                ProductControllerStatus._Error = PRODUCT_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<IEnumerable<ReadCylinderResource>> GetCylindersByOrder(string userId, string orderNumber)
        {
            try
            {
                Order order = await _CylinderRepository.GetOrderByOrderNumber(orderNumber);
                if (order == null)
                {
                    ProductControllerStatus._Error = PRODUCT_ERROR.NOT_FOUND;
                    return null;
                }

                var cylinders = await _CylinderRepository.GetCylindersByOrderId(order.OrderID);

                await SaveOperation(userId, Operation.ReadAll, Source.Cylinder);
                var cylindersResource = _Mapper.Map<List<Cylinder>, List<ReadCylinderResource>>(cylinders);
                return cylindersResource;
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.BL.ToString(), "GetCylindersByOrder", ex.Message);
                ProductControllerStatus._Error = PRODUCT_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<IndexResponse> GetCylindersStartIndex(string userId, string customerName)
        {
            try
            {
                var index = await _CylinderRepository.GetCylinderBatchIdByCustomerName(customerName);
                return new IndexResponse { _Index = index };
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.BL.ToString(), "GetCylindersStartIndex", ex.Message);
                ProductControllerStatus._Error = PRODUCT_ERROR.EXCEPTION;
                throw;
            }
        }
    }
}
