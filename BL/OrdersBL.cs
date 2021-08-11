using Allprimetech.GeneralUtils;
using Allprimetech.Interfaces;
using Allprimetech.Interfaces.Constants;
using Allprimetech.Interfaces.ControllerCodes;
using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources;
using Allprimetech.ServiceRestAPI.Metadatas;
using Allprimetech.ServiceRestAPI.Proxy;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allprimetech.BL
{
    public class OrdersBL
    {
        #region Declarations
        private ICustomersRepository _CustomersRepository { get; set; }
        private IOrdersRepository _OrderRepository { get; set; }
        public IGroupRepository _GroupRepository { get; set; }
        private IMapper _Mapper { get; set; }
        private IUnitOfWork _UnitOfWork { get; set; }
        private SystemAuditsBL _SystemAuditBL { get; set; }

        public OrdersBL(
            IOrdersRepository cylinderRepository, 
            IMapper mapper, 
            IUnitOfWork unitOfWork,
            IGroupRepository groupRepository,
            ISystemAuditRepository auditRepository,
            ICustomersRepository customersRepository)
        {
            _OrderRepository = cylinderRepository;
            _GroupRepository = groupRepository;
            _CustomersRepository = customersRepository;
            _Mapper = mapper;
            _UnitOfWork = unitOfWork;
            _SystemAuditBL = new SystemAuditsBL(auditRepository, mapper, unitOfWork);
            ProductControllerStatus._Error = PRODUCT_ERROR.NO_ERROR;
            OrderControllerStatus._Error = ORDER_ERROR.NO_ERROR;
        }
        private async Task SaveOperation(string operatorId, Operation operation, Source source)
        {
            await _SystemAuditBL.StoreOperation(operation, source, operatorId);
        }
        #endregion

        #region Orders Crud
        public async Task<IEnumerable<ReadOrderResource>> AllOrders(string userId, int customerId)
        {
            try
            {
                var orders = await _OrderRepository.AllOrders(customerId);

                await SaveOperation(userId, Operation.ReadAll, Source.Order);

                var ordersResource = _Mapper.Map<List<Order>, List<ReadOrderResource>>(orders);
                return ordersResource;
            }
            catch (Exception ex)
            {
                OrderControllerStatus._Error = ORDER_ERROR.EXCEPTION;
                Logs.logError("OrdersBL", "AllOrders", ex.Message);
                throw;
            }
        }

        public async Task<ReadOrderResource> GetOrder(string userId, int orderId)
        {
            try
            {
                var order = await _OrderRepository.GetOrder(orderId);

                if (order == null)
                {
                    OrderControllerStatus._Error = ORDER_ERROR.NOT_FOUND;
                    return null;
                }

                await SaveOperation(userId, Operation.ReadSingle, Source.Order);
                var orderResource = _Mapper.Map<Order, ReadOrderResource>(order);
                return orderResource;
            }
            catch (Exception ex)
            {
                OrderControllerStatus._Error = ORDER_ERROR.EXCEPTION;
                Logs.logError("OrdersBL", "GetOrder", ex.Message);
                throw;
            }
        }

        public async Task<ReadOrderResource> UpdateOrder(string userId, int orderId, CreateOrderResource model)
        {
            try
            {
                var order = await _OrderRepository.GetOrder(orderId);

                if (order == null)
                {
                    OrderControllerStatus._Error = ORDER_ERROR.NOT_FOUND;
                    return null;
                }

                int oldKeyQty = order._GroupKeyQty;
                int oldCylinderQty = order._CylinderQty;

                // update the order
                _Mapper.Map(model, order);
                order._UpdatedDate = DateTime.UtcNow;

                // add orderdetail
                //ProductType productType = (ProductType)model._ProductType;
                //OrderDetail orderDetail = new OrderDetail(productType, userId);

                //if (productType == ProductType.Cylinder)
                //{
                //    orderDetail._NewQty = order._CylinderQty;
                //    orderDetail._OldQty = oldCylinderQty;
                //}
                //else
                //{
                //    orderDetail._NewQty = order._GroupKeyQty;
                //    orderDetail._OldQty = oldKeyQty;
                //}

                //order._OrderDetails.Add(orderDetail);
                //await _UnitOfWork.CompleteAsync();

                await SaveOperation(userId, Operation.Updated, Source.Order);

                var result = _Mapper.Map<Order, ReadOrderResource>(order);
                return result;
            }
            catch (Exception ex)
            {
                OrderControllerStatus._Error = ORDER_ERROR.EXCEPTION;
                Logs.logError("OrdersBL", "UpdateOrder", ex.Message);
                throw;
            }
        }


        public async Task<CreateItemResponse> CreateOrder(string userId, CreateOrderResource model)
        {
            try
            {
                var order = _Mapper.Map<CreateOrderResource, Order>(model);

                var response = await _OrderRepository.AddOrder(order, model._Customer);
                if (!response)
                {
                    return new CreateItemResponse { _Succeeded = false, _Message = StringStore.InvalidCustomer, _NewItem = null };
                }

                order.CreatedById = userId;
                await _UnitOfWork.CompleteAsync();

                await SaveOperation(userId, Operation.Added, Source.Order);

                //var result = _Mapper.Map<Order, ReadOrderResource>(order); 
                var result = new ReadOrderResource(order);

                return new CreateItemResponse { _Succeeded = true, _Message = StringStore._Success, _NewItem = result };
            }
            catch (Exception ex)
            {
                OrderControllerStatus._Error = ORDER_ERROR.EXCEPTION;
                Logs.logError("OrdersBL", "CreateOrder", ex.Message);
                throw;
            }
        }


        public async Task<CreateItemResponse> ModifyOrder(string userId, ModifyOrderResource model)
        {
            try
            {
                var order = _Mapper.Map<ModifyOrderResource, Order>(model); 

                var response = await _OrderRepository.ModifyOrder(order, model, userId); 
                if (!response._Succeeded)
                {
                    return response;
                }

                var orderResponse = (Order)response._NewItem;
                orderResponse.CreatedById = userId;
                await _UnitOfWork.CompleteAsync();

                await SaveOperation(userId, Operation.Added, Source.Order);

                //var result = _Mapper.Map<Order, ReadOrderResource>(orderResponse);
                var result = new ReadOrderResource(orderResponse);

                return new CreateItemResponse { _Succeeded = true, _Message = StringStore._Success, _NewItem = result };
            }
            catch (Exception ex)
            {
                OrderControllerStatus._Error = ORDER_ERROR.EXCEPTION;
                Logs.logError("OrdersBL", "CreateOrder", ex.Message);
                throw;
            }
        }

        public async Task<int?> DeleteOrder(string userId, int orderId)
        {
            try
            {
                var order = await _OrderRepository.GetOrder(orderId);

                if (order == null)
                {
                    OrderControllerStatus._Error = ORDER_ERROR.NOT_FOUND;
                    return null;
                }

                // delete the order
                _OrderRepository.DeleteOrder(order);
                await _UnitOfWork.CompleteAsync();

                await SaveOperation(userId, Operation.Deleted, Source.Order);

                return orderId;
            }
            catch (Exception ex)
            {
                OrderControllerStatus._Error = ORDER_ERROR.EXCEPTION;
                Logs.logError("OrdersBL", "DeleteOrder", ex.Message);
                throw;
            }
        }

        public async Task<List<ReadOrderResource>> SearchOrders(string userId, OrderSearchResource searchTerm)
        {
            try
            {
                var orders = await _OrderRepository.SearchOrders(searchTerm);

                await SaveOperation(userId, Operation.Search, Source.Order);
                //var ordersResource = _Mapper.Map<List<Order>, List<ReadOrderResource>>(orders);
                return orders;
            }
            catch (Exception ex)
            {
                Logs.logError("OrdersBL", "SearchOrders", ex.Message);
                OrderControllerStatus._Error = ORDER_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<bool> SendReadyForPickUpEmail(string userId, ValidateOrderResource model)
        {
            try
            {
                var customer = await _CustomersRepository.GetCustomer(model._CustomerID);
                var order = await _OrderRepository.GetOrder(model._OrderID);
                if (customer == null || order == null)
                {
                    OrderControllerStatus._Error = ORDER_ERROR.NOT_FOUND;
                    return false;
                }

                OrderAvailable orderAvailable = await _OrderRepository.OrderAvailable(model, userId);

                var config = await _CustomersRepository.GetConfiguration();

                var customerProxy = new CustomersProxy(config.IntegraAdminURI);
                var data = new SendOrderConfirmation() 
                { 
                    _CustomerEmail = customer._Email, 
                    _OrderNumber = order._OrderNumber,
                    _CustomerName = customer._Name,
                    _CylinderQuantity = order._CylinderQty,
                    _GroupQuantity = order._GroupKeyQty,
                    _DateValidated = orderAvailable._CreationDate
                };
                var response = customerProxy.SendEmailReadyForPickUp(data);
                return true;
            }
            catch (Exception ex)
            {
                Logs.logError("OrdersBL", ex.Message, "SendReadyForPickUpEmail", ex);
                OrderControllerStatus._Error = ORDER_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<ReadOrderAvailable> GetPickUpDetail(string userId, int customerId)
        {
            try
            {
                var customer = await _CustomersRepository.GetCustomer(customerId);

                if (customer == null)
                {
                    OrderControllerStatus._Error = ORDER_ERROR.NOT_FOUND;
                    return null;
                }

                var pickUpResponse = await _OrderRepository.GetPickUpDetail(customerId);
                if (pickUpResponse == null)
                {
                    OrderControllerStatus._Error = ORDER_ERROR.NOT_FOUND;
                    return null;
                }

                var pickupResource = _Mapper.Map<OrderAvailable, ReadOrderAvailable>(pickUpResponse);
                return pickupResource;
            }
            catch (Exception ex)
            {
                Logs.logError("OrdersBL", ex.Message, "GetPickUpDetail", ex);
                OrderControllerStatus._Error = ORDER_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<PickupResponseResource> SendReadyForPickUpEmailV1(string userId, ValidateOrderResource model)
        {
            try
            {
                var customer = await _CustomersRepository.GetCustomer(model._CustomerID);
                var order = await _OrderRepository.GetOrder(model._OrderID);
                if (customer == null || order == null)
                {
                    OrderControllerStatus._Error = ORDER_ERROR.NOT_FOUND;
                    return null;
                }

                var orderAvailableResponse = await _OrderRepository.OrderAvailableV1(model, userId);

                if(orderAvailableResponse._Succeeded)
                {
                    var config = await _CustomersRepository.GetConfiguration();

                    var customerProxy = new CustomersProxy(config.IntegraAdminURI);
                    var data = new SendOrderConfirmation()
                    {
                        _CustomerEmail = customer._Email,
                        _OrderNumber = order._OrderNumber,
                        _CustomerName = customer._Name,
                        _CylinderQuantity = order._CylinderQty,
                        _GroupQuantity = order._GroupKeyQty,
                        _DateValidated = orderAvailableResponse._OrderAvailable._CreationDate
                    };
                    var response = customerProxy.SendEmailReadyForPickUp(data);

                }
                return new PickupResponseResource { _Succeeded = orderAvailableResponse._Succeeded, _Message = orderAvailableResponse._Message };
            }
            catch (Exception ex)
            {
                Logs.logError("OrdersBL", ex.Message, "SendReadyForPickUpEmail", ex);
                OrderControllerStatus._Error = ORDER_ERROR.EXCEPTION;
                throw;
            }
        }


        public async Task<bool> ValidateOrder(string userId, ValidateOrderResource model)
        {
            try
            {
                var customer = await _CustomersRepository.GetCustomer(model._CustomerID);
                var order = await _OrderRepository.GetOrder(model._OrderID);
                if(customer == null || order == null)
                {
                    OrderControllerStatus._Error = ORDER_ERROR.NOT_FOUND;
                    return false;
                }

                var orderValidated = await _OrderRepository.ValidatedOrder(model, userId);

                var config = await _CustomersRepository.GetConfiguration();

                var customerProxy = new CustomersProxy(config.IntegraAdminURI);
                var data = new SendOrderConfirmation() 
                { 
                    _CustomerEmail = customer._Email, 
                    _OrderNumber = order._OrderNumber,
                    _CustomerName = customer._Name,
                    _CylinderQuantity = order._CylinderQty,
                    _GroupQuantity = order._GroupKeyQty,
                    _DateValidated = orderValidated._DateValidated
                };
                var response = customerProxy.OrderConfirmationEmail(data);
                return true;
            }
            catch (Exception ex)
            {
                Logs.logError("OrdersBL", "SearchOrders", ex.Message);
                OrderControllerStatus._Error = ORDER_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<List<ReadOrderDetailResource>> SearchOrderDetails(string userId, OrderDetailSearchResource searchTerm)
        {
            try
            {
                var orderDetails = await _OrderRepository.SearchOrderDetail(searchTerm);

                await SaveOperation(userId, Operation.Search, Source.Group);
                return orderDetails;
            }
            catch (Exception ex)
            {
                ProductControllerStatus._Error = PRODUCT_ERROR.EXCEPTION;
                Logs.logError("OrdersBL", "SearchOrderDetails", ex.Message);
                throw;
            }
        }
        #endregion

        #region Order Details Crud
        public async Task<List<ReadOrderDetailResource>> GetAllOrderDetails(string userId, int customerId)
        {
            try
            {
                var orderDetails = await _OrderRepository.AllOrderDetails(customerId);
                var result = _Mapper.Map<List<OrderDetail>, List<ReadOrderDetailResource>>(orderDetails);
                return result;
            }
            catch (Exception ex)
            {
                Logs.logError("OrdersBL", "GetAllOrderDetails", ex.Message);
                OrderControllerStatus._Error = ORDER_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<List<ReadOrderDetailResource>> GetOrderDetailsForAnOrder(int id)
        {
            try
            {
                var orderDetails = await _OrderRepository.GetOrderDetailsForAnOrder(id);
                var result = _Mapper.Map<List<OrderDetail>, List<ReadOrderDetailResource>>(orderDetails);
                return result;
            }
            catch (Exception ex)
            {
                Logs.logError("OrdersBL", "GetOrderDEtailsForAnOrder", ex.Message);
                OrderControllerStatus._Error = ORDER_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<ReadOrderDetailResource> GetSingleOrderDetail(string userId, int orderDetailId)
        {
            try
            {
                var orderDetail = await _OrderRepository.GetSingleOrderDetail(orderDetailId);

                if (orderDetail == null)
                {
                    OrderControllerStatus._Error = ORDER_ERROR.NOT_FOUND;
                    return null;
                }

                var orderDetailResource = _Mapper.Map<OrderDetail, ReadOrderDetailResource>(orderDetail);
                return orderDetailResource;
            }
            catch (Exception ex)
            {
                Logs.logError("OrdersBL", "GetSingleOrderDetail", ex.Message);
                OrderControllerStatus._Error = ORDER_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<ReadOrderDetailResource> UpdateOrderDetail(string userId, int orderDetailId, CreateOrderDetailResource model)
        {
            try
            {
                var orderDetail = await _OrderRepository.GetSingleOrderDetail(orderDetailId);

                if (orderDetail == null)
                {
                    OrderControllerStatus._Error = ORDER_ERROR.NOT_FOUND;
                    return null;
                }

                // update the orderDetail
                _Mapper.Map(model, orderDetail);

                orderDetail.ByPersonId = userId;
                await _UnitOfWork.CompleteAsync();

                var result = _Mapper.Map<OrderDetail, ReadOrderDetailResource>(orderDetail);
                return result;
            }
            catch (Exception ex)
            {
                Logs.logError("OrdersBL", "UpdateOrderDetail", ex.Message);
                OrderControllerStatus._Error = ORDER_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<CreateItemResponse> CreateOrderDetailsBulk(string userId, CreateOrderDetailResource detailsList)
        {
            try
            {
                if(detailsList == null || detailsList._Cylinders.Count == 0 && detailsList._Groups.Count == 0)
                {
                    return new CreateItemResponse { _Succeeded = false, _Message = StringStore.EmptyDetailsList, _NewItem = null };
                }
                var response = await _OrderRepository.AddOrderDetailInBulk(detailsList, userId);

                if (response == null)
                {
                    return new CreateItemResponse { _Succeeded = false, _Message = StringStore._Failed, _NewItem = null };
                }

                return response;
            }
            catch (Exception ex)
            {
                Logs.logError("OrdersBL", "CreateOrderDetail", ex.Message);
                OrderControllerStatus._Error = ORDER_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<CreateItemResponse> CreateOrderDetail(string userId, CreateOrderDetailResource model)
        {
            try
            {
                var orderDetail = await _OrderRepository.AddOrderDetail(model, userId);

                if (orderDetail == null)
                {
                    return new CreateItemResponse { _Succeeded = false, _Message = StringStore._Failed, _NewItem = null };
                }

                await _UnitOfWork.CompleteAsync();

                var result = _Mapper.Map<OrderDetail, ReadOrderDetailResource>(orderDetail);

                return new CreateItemResponse { _Succeeded = true, _Message = StringStore._Success, _NewItem = result };
            }
            catch (Exception ex)
            {
                Logs.logError("OrdersBL", "CreateOrderDetail", ex.Message);
                OrderControllerStatus._Error = ORDER_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<int?> DeleteOrderDetail(string userId, int orderDetailId)
        {
            try
            {
                var orderDetail = await _OrderRepository.GetSingleOrderDetail(orderDetailId);

                if (orderDetail == null)
                {
                    OrderControllerStatus._Error = ORDER_ERROR.NOT_FOUND;
                    return null;
                }

                // delete the order
                _OrderRepository.DeleteOrderDetail(orderDetail);
                await _UnitOfWork.CompleteAsync();

                return orderDetailId;
            }
            catch (Exception ex)
            {
                Logs.logError("OrdersBL", "DeleteOrderDetail", ex.Message);
                OrderControllerStatus._Error = ORDER_ERROR.EXCEPTION;
                throw;
            }
        }

        #endregion

        #region Groups Crud
        public async Task<IEnumerable<ReadGroupResource>> AllGroups(string userId, int customerId)
        {
            try
            {
                var groups = await _GroupRepository.AllGroups(customerId);

                await SaveOperation(userId, Operation.ReadAll, Source.Group);
                var groupsResource = _Mapper.Map<List<Group>, List<ReadGroupResource>>(groups);
                return groupsResource;
            }
            catch (Exception ex)
            {
                ProductControllerStatus._Error = PRODUCT_ERROR.EXCEPTION;
                Logs.logError("OrdersBL", "AllGroups", ex.Message);
                throw;
            }
        }

        public async Task<ReadGroupResource> GetGroup(string userId, int groupId)
        {
            try
            {
                var group = await _GroupRepository.GetGroup(groupId);

                if (group == null)
                {
                    ProductControllerStatus._Error = PRODUCT_ERROR.NOT_FOUND;
                    return null;
                }

                await SaveOperation(userId, Operation.ReadSingle, Source.Group);

                var groupResource = _Mapper.Map<Group, ReadGroupResource>(group);
                return groupResource;
            }
            catch (Exception ex)
            {
                ProductControllerStatus._Error = PRODUCT_ERROR.EXCEPTION;
                Logs.logError("OrdersBL", "GetGroup", ex.Message);
                throw;
            }
        }

        public async Task<ReadGroupResource> UpdateGroup(string userId, int groupId, CreateGroupResource model)
        {
            try
            {
                var group = await _GroupRepository.GetGroup(groupId);

                if (group == null)
                {
                    ProductControllerStatus._Error = PRODUCT_ERROR.NOT_FOUND;
                    return null;
                }

                // update the group
                _Mapper.Map(model, group);
                group._UpdatedDate = DateTime.UtcNow;
                await _UnitOfWork.CompleteAsync();

                await SaveOperation(userId, Operation.Updated, Source.Group);

                var result = _Mapper.Map<Group, ReadGroupResource>(group);
                return result;
            }
            catch (Exception ex)
            {
                ProductControllerStatus._Error = PRODUCT_ERROR.EXCEPTION;
                Logs.logError("OrdersBL", "UpdateGroup", ex.Message);
                throw;
            }
        }

        

        public async Task<IEnumerable<ReadGroupResource>> SearchGroups(string userId, GroupSearchResource searchTerm)
        {
            try
            {
                var groups = await _GroupRepository.SearchGroups(searchTerm);

                await SaveOperation(userId, Operation.Search, Source.Group);
                var groupResource = _Mapper.Map<IEnumerable<Group>, IEnumerable<ReadGroupResource>>(groups);
                return groupResource;
            }
            catch (Exception ex)
            {
                ProductControllerStatus._Error = PRODUCT_ERROR.EXCEPTION;
                Logs.logError("OrdersBL", "SearchGroups", ex.Message);
                throw;
            }
        }

        public async Task<ReadGroupResource> CreateGroup(string userId, CreateGroupResource model)
        {
            try
            {
                var group = _Mapper.Map<CreateGroupResource, Group>(model);

                //group.CreatedById = userId;

                await _GroupRepository.AddGroup(group, userId);
                await _UnitOfWork.CompleteAsync();

                await SaveOperation(userId, Operation.Added, Source.Group);

                var result = _Mapper.Map<Group, ReadGroupResource>(group);

                return result;
            }
            catch (Exception ex)
            {
                ProductControllerStatus._Error = PRODUCT_ERROR.EXCEPTION;
                Logs.logError("OrdersBL", "CreateGroup", ex.Message);
                throw;
            }
        }

        public async Task<List<ReadGroupResource>> CreateGroups(string userId, List<CreateGroupResource> groupsResource)
        {
            try
            {
                List<ReadGroupResource> createdGroups = new List<ReadGroupResource>();
                var groupKeyItem = groupsResource[0];
                int customerId = 0;
                int orderId = 0;
                int batchNumber = 1;

                if (groupKeyItem != null)
                {
                    Customer customer = await _GroupRepository.GetCustomerByName(groupKeyItem._Customer);
                    customerId = customer != null ? customer.CustomerID : 0;
                    Order order = await _GroupRepository.GetOrderByNumber(groupKeyItem._Order);
                    orderId = order != null ? order.OrderID : 0;

                    //check if there is an existing group for this customer in database
                    if (order != null && customer != null)
                    {
                        bool groupsExists = await _GroupRepository.IsGroupExistsWithCustomer(customerId);
                        if (groupsExists)
                        {
                            order._GroupKeyQty += groupsResource.Sum(x => x._Quantity);
                            await _UnitOfWork.CompleteAsync();
                        }
                    }

                    int lastBatch = await _GroupRepository.GetLastBatchNumber(groupKeyItem._Customer);
                    batchNumber = lastBatch + 1;
                }

                // 
                string insertQuery = "";
                foreach (var groupItem in groupsResource)
                {
                    var group = _Mapper.Map<CreateGroupResource, Group>(groupItem);

                    group.CustomerID = customerId;

                    group.OrderID = orderId;
                    group._BatchNumber = batchNumber;

                    if(groupItem._Quantity != 0)
                    {
                        insertQuery += String.Format(@"('{0}', '{1}', '{2}', {3}, '{4}', {5}, {6}, {7}, {8}, {9}, {10}, {11}, '{12}', {13}),",
                                DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                                group._Name, group._Quantity, group._KeyNumber, group._Produced, group._Validated, group._PositionId,
                                group.CustomerID, group.OrderID, group._Reclaimed, group._Blocked, group._GroupNumber, group._BatchNumber);

                    }
                    else
                    {
                        insertQuery += String.Format(@"('{0}', '{1}', '{2}', {3}, '{4}', {5}, {6}, {7}, {8}, {9}, {10}, {11}, '{12}', {13}),",
                                DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                                group._Name, group._Quantity, group._KeyNumber, group._Produced, group._Validated, group._PositionId,
                                group.CustomerID, "NULL", group._Reclaimed, group._Blocked, group._GroupNumber, group._BatchNumber);
                    }

                }
                await _GroupRepository.CreateGroupsBulk(insertQuery);

                var addedGroups = await _GroupRepository.RetrieveCreatedGroups(customerId);
                createdGroups = _Mapper.Map<List<Group>, List<ReadGroupResource>>(addedGroups);

                await SaveOperation(userId, Operation.Added, Source.Group);
            

                //group.CreatedById = userId;


                return createdGroups;
            }
            catch (Exception ex)
            {
                ProductControllerStatus._Error = PRODUCT_ERROR.EXCEPTION;
                Logs.logError("OrdersBL", "CreateGroups", ex.Message);
                throw;
            }
        }

        public async Task<List<ReadGroupResource>> ModifyGroups(string userId, List<CreateGroupResource> groupsResource)
        {
            try
            {
                List<ReadGroupResource> createdGroups = new List<ReadGroupResource>();
                var groupKeyItem = groupsResource[0];
                int customerId = 0;

                if (groupKeyItem != null)
                {
                    Customer customer = await _GroupRepository.GetCustomerById(groupKeyItem._CustomerID);
                    customerId = customer != null ? customer.CustomerID : 0;

                }

                if(customerId == 0)
                {
                    ProductControllerStatus._Error = PRODUCT_ERROR.NOT_FOUND;
                    return null;
                }

                    //var result = _Mapper.Map<Group, ReadGroupResource>(group);
                    //createdGroups.Add(result);
                var response = await _GroupRepository.ModifyGroup(groupsResource, customerId, userId);
                await SaveOperation(userId, Operation.Added, Source.Group);


                //group.CreatedById = userId;


                return response;
            }
            catch (Exception ex)
            {
                ProductControllerStatus._Error = PRODUCT_ERROR.EXCEPTION;
                Logs.logError("OrdersBL", "CreateGroups", ex.Message);
                throw;
            }
        }


        public async Task<IEnumerable<ReadGroupResource>> GetGroupsByOrder(string userId, string orderNumber)
        {
            try
            {
                Order order = await _GroupRepository.GetOrderByOrderNumber(orderNumber);
                if(order == null)
                {
                    ProductControllerStatus._Error = PRODUCT_ERROR.NOT_FOUND;
                    return null;
                }
                var groups = await _GroupRepository.GetGroupsByOrder(order.OrderID);

                await SaveOperation(userId, Operation.ReadAll, Source.Group);
                var groupsResource = _Mapper.Map<List<Group>, List<ReadGroupResource>>(groups);
                return groupsResource;
            }
            catch (Exception ex)
            {
                ProductControllerStatus._Error = PRODUCT_ERROR.EXCEPTION;
                Logs.logError("OrdersBL", "GetGroupsByOrder", ex.Message);
                throw;
            }
        }

        public async Task<IndexResponse> GetGroupsStartIndex(string userId, string customerName)
        {
            try
            {
                var index = await _GroupRepository.GetGroupBatchIdByCustomerName(customerName);
                return new IndexResponse { _Index = index };
            }
            catch (Exception ex)
            {
                ProductControllerStatus._Error = PRODUCT_ERROR.EXCEPTION;
                Logs.logError("OrdersBL", "GetGroupsStartIndex", ex.Message);
                throw;
            }
        }

        public async Task<int?> DeleteGroup(string userId, int groupId)
        {
            try
            {
                var group = await _GroupRepository.GetGroup(groupId);

                if (group == null)
                {
                    ProductControllerStatus._Error = PRODUCT_ERROR.NOT_FOUND;
                    return null;
                }

                // delete the group
                _GroupRepository.DeleteGroup(group);
                await _UnitOfWork.CompleteAsync();

                await SaveOperation(userId, Operation.Deleted, Source.Group);

                return groupId;
            }
            catch (Exception ex)
            {
                ProductControllerStatus._Error = PRODUCT_ERROR.EXCEPTION;
                Logs.logError("OrdersBL", "DeleteGroup", ex.Message);
                throw;
            }
        }

        #endregion
    }
}
