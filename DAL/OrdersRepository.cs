using Allprimetech.Interfaces;
using Allprimetech.Interfaces.Constants;
using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources;
using Allprimetech.Interfaces.Resources.EnumResource;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allprimetech.DAL
{
    public class OrdersRepository : IOrdersRepository
    {
        private IMapper _Mapper { get; set; }

        private DbContextOptions<ApplicationDbContext> _Options { get; set; }

        private ApplicationDbContext _Context { get; set; }

        public OrdersRepository(ApplicationDbContext context, DbContextOptions<ApplicationDbContext> options, IMapper mapper)
        {
            _Context = context;
            _Options = options;
            _Mapper = mapper;
        }
        public async Task<bool> AddOrder(Order order, string customerName)
        {
            try
            {
                var customer = await _Context.Customers.Where(x => x._Name == customerName).FirstOrDefaultAsync();
                if(customer == null)
                {
                    return false;
                }

                order.CustomerID = customer.CustomerID;
                order._Status = Status.NotStarted;
                order._CreationDate = DateTime.UtcNow;
                order._UpdatedDate = DateTime.UtcNow;


                await _Context.Orders.AddAsync(order);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<CreateItemResponse> ModifyOrder(Order order, ModifyOrderResource model, string userId)
        {
            try
            {
                var customer = await _Context.Customers.FindAsync(model._CustomerID);
                if (customer == null)
                {
                    return new CreateItemResponse { _Succeeded = false, _Message = "Invalid Customer" };
                }

                if(model._OrderID != 0)
                {
                    var existingOrder = await _Context.Orders.FindAsync(model._OrderID);
                    if(existingOrder == null)
                    {
                        return new CreateItemResponse { _Succeeded = false, _Message = "Invalid Order" };
                    }
                    return new CreateItemResponse { _Succeeded = true, _Message = StringStore._Success, _NewItem = existingOrder };
                }
                else
                {
                    var orders = await _Context.Orders.Where(x => x.CustomerID == customer.CustomerID).OrderBy(x => x._CreationDate).ToListAsync();
                    var cylinders = await _Context.Cylinders.Where(x => x.CustomerID == customer.CustomerID).ToListAsync();
                    var groups = await _Context.Groups.Where(x => x.CustomerID == customer.CustomerID).ToListAsync();
                    int cylindersCount = cylinders.Sum(x => x._Quantity);
                    int groupsCount = groups.Sum(x => x._Quantity);

                    if (cylindersCount == model._CylinderQuantity && groupsCount == model._KeyQuantity)
                    {
                        return new CreateItemResponse { _Succeeded = false, _Message = "No new cylinders and keys in new order" };
                    }

                    var user = await _Context.Users.Include(x => x.Country).SingleOrDefaultAsync(x => x.Id == userId);


                    var lastOrder = orders.FirstOrDefault();

                    var currentUtcTime = DateTime.UtcNow;
                    TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById(user.Country._TimeZoneName);
                    DateTime dateNow = TimeZoneInfo.ConvertTimeFromUtc(currentUtcTime, cstZone);


                    string orderNumber = lastOrder == null ? "" : lastOrder._OrderNumber.Split("/")[0];
                    bool isNew = false;
                    if (model._OrderNumber != orderNumber)
                    {
                        if (String.IsNullOrEmpty(orderNumber))
                        {
                            orderNumber = model._OrderNumber;
                            isNew = true;
                        }
                        else
                        {
                            orderNumber = orderNumber + "/" + model._OrderNumber;
                            isNew = false;
                        }
                        //orderNumber = String.IsNullOrEmpty(orderNumber) ? model._OrderNumber : orderNumber + "/" + model._OrderNumber;
                    }

                    var newOrderNumber = isNew ? orderNumber : orderNumber + "/" +
                                            dateNow.Day.ToString("00") + dateNow.Month.ToString("00") +
                                            dateNow.Year.ToString() + "/" + orders.Count();

                    order._CylinderQty = 0;
                    order._GroupKeyQty = 0;
                    order.CustomerID = customer.CustomerID;
                    order._Status = Status.NotStarted;
                    order._CreationDate = DateTime.UtcNow;
                    order._UpdatedDate = DateTime.UtcNow;
                    order._OrderNumber = newOrderNumber;



                    await _Context.Orders.AddAsync(order);
                    await _Context.SaveChangesAsync();
                    return new CreateItemResponse { _Succeeded = true, _Message = StringStore._Success, _NewItem = order };

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Order>> AllOrders(int customerId)
        {
            return await _Context.Orders
                                .Where(x => x.CustomerID == customerId)
                                .Include(x => x.CreatedBy)
                                .Include(x => x.Customer)
                                .Include(x => x._OrderValidations)
                                .Include(x => x._OrderAvailables)
                                .ToListAsync();
        }

        public void DeleteOrder(Order order)
        {
            _Context.Orders.Remove(order);
        }

        public async Task<Order> GetOrder(int id)
        {
            return await _Context.Orders.Include(x => x._OrderDetails).Include(x => x.CreatedBy).SingleOrDefaultAsync(x => x.OrderID == id);
        }


        #region Order Detail Endpoints
        #endregion
        public async Task<List<OrderDetail>> AllOrderDetails(int customerId)
        {
            return await _Context.OrderDetails
                                .Include(x => x.ByPerson)
                                .Include(x => x.Order)
                                    .Where(x => x.Order.CustomerID == customerId)
                                .ToListAsync();
        }

        public async Task<List<OrderDetail>> GetOrderDetailsForAnOrder(int orderId)
        {
            var order = await _Context.OrderDetails
                                        .Where(x => x.OrderID == orderId)
                                        .Include(x => x.ByPerson)
                                        .ToListAsync();
            return order;
        }

        public async Task<OrderDetail> GetSingleOrderDetail(int orderDetailId)
        {
            return await _Context.OrderDetails
                                    .Where(x => x.OrderDetailID == orderDetailId)
                                    .Include(x => x.ByPerson)
                                    .Include(x => x.Order)
                                    .SingleOrDefaultAsync();
        }

        

        public void DeleteOrderDetail(OrderDetail orderDetail)
        {
            _Context.OrderDetails.Remove(orderDetail);
        }

        public async Task<List<ReadOrderResource>> SearchOrders(OrderSearchResource model)
        {
            try
            {

                OrderSearchEnum selectedFieldType = (OrderSearchEnum)Enum.Parse(typeof(OrderSearchEnum), model._SearchTerm);

                var from = DateTime.UtcNow;
                var to = DateTime.UtcNow;

                var context = new ApplicationDbContext(_Options);
                List<ReadOrderResource> filteredOrders = new List<ReadOrderResource>();
                await Task.Run(() => {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {
                        var sqlScript = @"Select o.OrderID, o.CustomerID, o.CreatedById, concat(u._FirstName, ' ', u._LastName) as CreatedBy,
                                            o._GroupKeyQty, o._CylinderQty, o._CreationDate, o._UpdatedDate, o._OrderNumber,
                                            o._ProjectName, o._Description, o._Status, c._SystemCode, c._Name
                                            from orders o 
                                            inner join customers c on c.CustomerID = o.CustomerID
                                            inner join aspnetusers u on o.CreatedById = u.Id
                                            where 1 = 1";

                        if (selectedFieldType == OrderSearchEnum.GCustName)
                        {
                            sqlScript += " and c._Name like @searchValue";
                        }
                        else if (selectedFieldType == OrderSearchEnum.GCustSystemCode)
                        {
                            sqlScript += " and c._SystemCode like @searchValue";
                        }
                        else if (selectedFieldType == OrderSearchEnum.GOrdNumber)
                        {
                            sqlScript += " and o._OrderNumber like @searchValue";
                        }
                        else if (selectedFieldType == OrderSearchEnum.GOrdProjName)
                        {
                            sqlScript += " and o._ProjectName like @searchValue";
                        }

                        if(model._From != null)
                            {
                            from = (DateTime)model._From;
                            sqlScript += " and cast(o._CreationDate as date) >= @from";
                        }
                        if (model._To != null)
                        {
                            to = (DateTime)model._To;
                            sqlScript += " and cast(o._CreationDate as date) <= @to";
                        }

                        sqlScript += " and c.CustomerID = @customerId";

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;

                        command.Parameters.Add(new MySqlParameter("@searchValue", "%" + model._SearchValue + "%"));
                        command.Parameters.Add(new MySqlParameter("@customerId", model._CustomerID));
                        command.Parameters.Add(new MySqlParameter("@from", from));
                        command.Parameters.Add(new MySqlParameter("@to", to));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                ReadOrderResource customer = new ReadOrderResource();
                                customer.CreatedById = result["CreatedById"].ToString();
                                customer.CreatedBy = result["CreatedBy"].ToString();
                                customer.CustomerID = Convert.ToInt32(result["CustomerID"]);
                                customer._GroupKeyQty = Convert.ToInt32(result["_GroupKeyQty"]);
                                customer._CylinderQty = Convert.ToInt32(result["_CylinderQty"]);
                                customer._CustomerName = result["_Name"].ToString();
                                customer._CreationDate = (DateTime)result["_CreationDate"];
                                customer._UpdatedDate = (DateTime)result["_UpdatedDate"];
                                customer._CustomerName = result["_Name"].ToString();
                                customer._SystemCode = result["_SystemCode"].ToString();
                                customer._OrderNumber = result["_OrderNumber"].ToString();
                                customer._ProjectName = result["_ProjectName"].ToString();
                                customer._Description = result["_Description"].ToString();
                                customer._Status = Convert.ToInt32(result["_Status"]);
                                customer.OrderID = Convert.ToInt32(result["OrderID"]);

                                filteredOrders.Add(customer);
                            }
                        }
                    }
                });
                return filteredOrders;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Order> GetOrderByOrderNumber(string orderNumber)
        {
            return await _Context.Orders.Where(x => x._OrderNumber == orderNumber).FirstOrDefaultAsync();
        }

        public async Task<CreateItemResponse> AddOrderDetailInBulk(CreateOrderDetailResource orderDetails, string userId)
        {
            try
            {
                var responseList = new List<ReadOrderDetailResource>();

                int orderId = orderDetails._OrderID;
                var customerId = orderDetails._CustomerID;

                var order = await _Context.Orders.FindAsync(orderId);
                var lastOrder = await _Context.Orders.Where(x => x.CustomerID == customerId).OrderByDescending(x => x._CreationDate).FirstOrDefaultAsync();

                var modifiedCylIds = new List<int>();
                var modifiedGrpIds = new List<int>();
                bool createNewOrder = false;


                // modify the cylinders request
                for (int i = 0; i < orderDetails._Cylinders.Count; i++)
                {
                    var cylinderItem = orderDetails._Cylinders[i];
                    if(cylinderItem._Operation == OrderDetailOperations.UPDATE)
                    {
                        var cylinder = await _Context.Cylinders.Where(x => x._CylinderNumber == cylinderItem._CylinderNumber
                                                                        && x._DoorName == cylinderItem._DoorName).FirstOrDefaultAsync();
                        if(cylinder != null)
                        {
                            if(cylinder._Quantity != cylinderItem._Quantity)
                            {
                                OrderDetail orderDetail = new OrderDetail();
                                orderDetail.ByPersonId = userId;
                                orderDetail._NewQty = cylinderItem._Quantity;
                                orderDetail._Date = DateTime.UtcNow;
                                orderDetail._OldQty = cylinder._Quantity;
                                orderDetail._ProductID = cylinder.CylinderID;
                                orderDetail._ProductType = ProductType.Cylinder;
                                orderDetail.OrderID = lastOrder.OrderID;

                                _Context.OrderDetails.Add(orderDetail);
                                cylinder._Quantity = cylinderItem._Quantity;
                                modifiedCylIds.Add(cylinder.CylinderID);
                                createNewOrder = true;
                            }
                        }
                    }
                    else if(cylinderItem._Operation == OrderDetailOperations.DELETE)
                    {
                        var customerCylinders = await _Context.Cylinders.Where(x => x.CustomerID == customerId)
                                                                        .OrderBy(x => x._PositionId)
                                                                        .ToListAsync();
                        var cylinder = customerCylinders.Where(x => x._CylinderNumber == cylinderItem._CylinderNumber
                                                                        && x._DoorName == cylinderItem._DoorName).FirstOrDefault();
                        if (cylinder != null)
                        {
                            var cylPosIdToBeModified = customerCylinders.Where(x => x._PositionId > cylinder._PositionId)
                                                                        .OrderBy(x => x._PositionId).ToList();
                            for (int j = 0; j < cylPosIdToBeModified.Count; j++)
                            {
                                var item = cylPosIdToBeModified[j];
                                item._PositionId -= 1;
                            }

                            if(cylinder._Quantity > 0)
                            {
                                createNewOrder = true;
                            }

                            _Context.Cylinders.Remove(cylinder);
                        }
                        await _Context.SaveChangesAsync();
                    }
                    else if(cylinderItem._Operation == OrderDetailOperations.ADD)
                    {
                        var customerCylinders = await _Context.Cylinders.Where(x => x.CustomerID == customerId)
                                                                        .OrderBy(x => x._PositionId)
                                                                        .ToListAsync();
                        var cylinder = new Cylinder()
                        {
                            CustomerID = customerId,
                            _CreationDate = DateTime.UtcNow,
                            _UpdatedDate = DateTime.UtcNow,
                            _DoorName = cylinderItem._DoorName,
                            _ArticleNumber = (ArticleNumber)cylinderItem._ArticleNumber,
                            _Color = cylinderItem._Color,
                            _CylinderNumber = cylinderItem._CylinderNumber,
                            _Quantity = cylinderItem._Quantity,
                            _PositionId = cylinderItem._PositionId,
                            _Options = (Options)cylinderItem._Options
                        };

                        if(cylinder._PositionId <= customerCylinders.Count)
                        {
                            var cylPosIdToBeModified = customerCylinders.Where(x => x._PositionId >= cylinder._PositionId)
                                                                        .OrderBy(x => x._PositionId).ToList();
                            for (int j = 0; j < cylPosIdToBeModified.Count; j++)
                            {
                                var item = cylPosIdToBeModified[j];
                                item._PositionId += 1;
                            }

                        }

                        _Context.Cylinders.Add(cylinder);
                        await _Context.SaveChangesAsync();

                        if (cylinder._Quantity > 0)
                        {
                            modifiedCylIds.Add(cylinder.CylinderID);
                            createNewOrder = true;
                        }
                    }
                }

                // modify the groups request
                for (int i = 0; i < orderDetails._Groups.Count; i++)
                {
                    var groupItem = orderDetails._Groups[i];
                    if (groupItem._Operation == OrderDetailOperations.UPDATE)
                    {
                        var group = await _Context.Groups.Where(x => x._GroupNumber == groupItem._GroupNumber
                                                                        && x._Name == groupItem._KeyName).FirstOrDefaultAsync();
                        if (group != null)
                        {
                            if (group._Quantity != groupItem._Quantity)
                            {
                                OrderDetail orderDetail = new OrderDetail();
                                orderDetail.ByPersonId = userId;
                                orderDetail._NewQty = groupItem._Quantity;
                                orderDetail._Date = DateTime.UtcNow;
                                orderDetail._OldQty = group._Quantity;
                                orderDetail._ProductID = group.GroupID;
                                orderDetail._ProductType = ProductType.Key;
                                orderDetail.OrderID = lastOrder.OrderID;

                                _Context.OrderDetails.Add(orderDetail);

                                group._Quantity = groupItem._Quantity;
                                modifiedGrpIds.Add(group.GroupID);
                                createNewOrder = true;
                            }
                        }
                    }
                    else if (groupItem._Operation == OrderDetailOperations.DELETE)
                    {
                        var customerGroups = await _Context.Groups.Where(x => x.CustomerID == customerId)
                                                                        .OrderBy(x => x._PositionId)
                                                                        .ToListAsync();
                        var group = customerGroups.Where(x => x._GroupNumber == groupItem._GroupNumber
                                                                        && x._Name == groupItem._KeyName).FirstOrDefault();
                        if (group != null)
                        {
                            var cylPosIdToBeModified = customerGroups.Where(x => x._PositionId > group._PositionId)
                                                                        .OrderBy(x => x._PositionId).ToList();
                            for (int j = 0; j < cylPosIdToBeModified.Count; j++)
                            {
                                var item = cylPosIdToBeModified[j];
                                item._PositionId -= 1;
                            }

                            if (group._Quantity > 0)
                            {
                                createNewOrder = true;
                            }

                            _Context.Groups.Remove(group);
                        }
                        await _Context.SaveChangesAsync();
                    }
                    else if (groupItem._Operation == OrderDetailOperations.ADD)
                    {
                        var customerGroups = await _Context.Groups.Where(x => x.CustomerID == customerId)
                                                                        .OrderBy(x => x._PositionId)
                                                                        .ToListAsync();
                        var group = new Group()
                        {
                            CustomerID = customerId,
                            _CreationDate = DateTime.UtcNow,
                            _UpdatedDate = DateTime.UtcNow,
                            _KeyNumber = groupItem._PositionId.ToString(),
                            _PositionId = groupItem._PositionId,
                            _GroupNumber = groupItem._GroupNumber,
                            _Quantity = groupItem._Quantity,
                            _Name = groupItem._KeyName,
                        };

                        if (group._PositionId <= customerGroups.Count)
                        {
                            var grpPosIdToBeModified = customerGroups.Where(x => x._PositionId >= group._PositionId)
                                                                        .OrderBy(x => x._PositionId).ToList();
                            for (int j = 0; j < grpPosIdToBeModified.Count; j++)
                            {
                                var item = grpPosIdToBeModified[j];
                                item._PositionId += 1;
                            }

                        }

                        _Context.Groups.Add(group);
                        await _Context.SaveChangesAsync();

                        if (group._Quantity > 0)
                        {
                            modifiedGrpIds.Add(group.GroupID);
                            createNewOrder = true;
                        }
                    }
                }

                if (createNewOrder)
                {
                    var cylQty = await _Context.Cylinders.SumAsync(x => x._Quantity);
                    var grpQty = await _Context.Groups.SumAsync(x => x._Quantity);

                    var orders = await _Context.Orders.Where(x => x.CustomerID == customerId).CountAsync();
                    var newOrderNumber = lastOrder._OrderNumber.Split("-")[0] + "-" +
                                            DateTime.UtcNow.Day.ToString("00") + DateTime.UtcNow.Month.ToString("00") +
                                            DateTime.UtcNow.Year.ToString() + "-" + orders;
                    var newOrder = new Order
                    {
                        CreatedById = userId,
                        _CreationDate = DateTime.UtcNow,
                        _UpdatedDate = DateTime.UtcNow,
                        _CylinderQty = cylQty,
                        _GroupKeyQty = grpQty,
                        CustomerID = customerId,
                        _Description = lastOrder._Description,
                        _ProjectName = lastOrder._ProjectName,
                        _OrderNumber = newOrderNumber,
                        _Status = Status.NotStarted
                    };
                    _Context.Orders.Add(newOrder);
                    await _Context.SaveChangesAsync();

                    for (int i = 0; i < modifiedCylIds.Count; i++)
                    {
                        var cylinderId = modifiedCylIds[i];
                        var cylinder = await _Context.Cylinders.FindAsync(cylinderId);
                        cylinder.OrderID = newOrder.OrderID;
                    }
                    for (int i = 0; i < modifiedGrpIds.Count; i++)
                    {
                        var groupId = modifiedGrpIds[i];
                        var group = await _Context.Groups.FindAsync(groupId);
                        group.OrderID = newOrder.OrderID;
                    }
                    await _Context.SaveChangesAsync();
                }
                return new CreateItemResponse { _Succeeded = true, _Message = StringStore._Success, _NewItem = responseList };

            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<OrderDetail> AddOrderDetail(CreateOrderDetailResource orderDetail, string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ReadOrderDetailResource>> SearchOrderDetail(OrderDetailSearchResource model)
        {
            try
            {

                var from = DateTime.UtcNow;
                var to = DateTime.UtcNow;
                var context = new ApplicationDbContext(_Options);
                List<ReadOrderDetailResource> filteredOrders = new List<ReadOrderDetailResource>();
                await Task.Run(() => {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {
                        var sqlScript = @"select o.OrderDetailID, o.ByPersonId, o._ProductType, o._ProductID, 
                                        o._Date, o.OrderID, o._NewQty, o._OldQty,
                                        o._Notes, concat(u._FirstName, ' ', u._LastName) as _CreatedBy,
                                        od._ProjectName, od._OrderNumber, od.CustomerID from orderdetails o
                                        inner join aspnetusers u on u.Id = o.ByPersonId
                                        inner join orders od on od.OrderID = o.OrderID
                                        where 1=1";

                        if(model._ProductType != null && model._ProductType != 0)
                        {
                            sqlScript += $" and o._ProductType = @productType";
                        }

                        if (model._OrderNumbers != null && model._OrderNumbers.Count > 0)
                        {
                            if(model._OrderNumbers.Count == 1)
                            {
                                sqlScript += $" and od._OrderNumber = @orderNumber1";
                            }
                            else
                            {
                                for (int i = 0; i < model._OrderNumbers.Count; i++)
                                {
                                    var item = model._OrderNumbers[i];
                                    var label = i + 1;
                                    if (i == 0)
                                    {
                                        sqlScript += $" and (od._OrderNumber = @orderNumber{label}";
                                    }
                                    else
                                    {
                                        sqlScript += $" or od._OrderNumber = @orderNumber{label}";
                                        if (i == model._OrderNumbers.Count() - 1)
                                        {
                                            sqlScript += ")";
                                        }
                                    }
                                }
                            }
                        }



                        sqlScript += " and od.CustomerID = @customerId";

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;

                        if (model._OrderNumbers != null && model._OrderNumbers.Count > 0)
                        {
                            //sqlScript += StringStore.SearchOrderNumber;
                            for (int i = 0; i < model._OrderNumbers.Count; i++)
                            {
                                var item = model._OrderNumbers[i];
                                var label = i + 1;
                                if (!String.IsNullOrWhiteSpace(item))
                                {
                                    command.Parameters.Add(new MySqlParameter($"@orderNumber{label}", item));
                                }
                            }
                        }

                        //command.Parameters.Add(new MySqlParameter("@orderNumber", "%" + model._OrderNumbers + "%"));
                        command.Parameters.Add(new MySqlParameter("@productType", model._ProductType));
                        command.Parameters.Add(new MySqlParameter("@customerId", model._CustomerID));
                        command.Parameters.Add(new MySqlParameter("@from", from));
                        command.Parameters.Add(new MySqlParameter("@to", to));



                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                ReadOrderDetailResource customer = new ReadOrderDetailResource();
                                customer.OrderID = Convert.ToInt32(result["OrderID"]);
                                customer._ProductID = Convert.ToInt32(result["_ProductID"]);
                                customer._CreatedBy = result["_CreatedBy"].ToString();
                                customer.OrderDetailID = Convert.ToInt32(result["OrderDetailID"]);
                                customer._ProductType = (ProductType)Convert.ToInt32(result["_ProductType"]);
                                customer._OldQty = Convert.ToInt32(result["_OldQty"]);
                                customer._NewQty = Convert.ToInt32(result["_NewQty"]);
                                customer._Notes = result["_Notes"].ToString();
                                customer.ByPersonId = result["ByPersonId"].ToString();
                                customer._Date = (DateTime)result["_Date"];
                                customer._OrderNumber = result["_OrderNumber"].ToString();
                                customer._ProjectName = result["_ProjectName"].ToString();

                                filteredOrders.Add(customer);
                            }
                        }
                    }
                });
                if (model._From != null)
                {
                    from = (DateTime)model._From;
                    filteredOrders = filteredOrders.Where(x => x._Date.Date >= from.Date).ToList();
                }
                if (model._To != null)
                {
                    to = (DateTime)model._To;
                    filteredOrders = filteredOrders.Where(x => x._Date.Date <= to.Date).ToList();
                }
                return filteredOrders;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<OrderValidation> ValidatedOrder(ValidateOrderResource model, string userId)
        {
            try
            {
                var time = await GetUserTimeZone(userId);
                var orderValidated = await _Context.OrderValidations.Where(x => x.CustomerID == model._CustomerID
                                                                            && x.OrderID == model._OrderID).FirstOrDefaultAsync();
                if(orderValidated == null)
                {
                    var newOrderValidation = new OrderValidation()
                    {
                        _DateValidated = time,
                        CustomerID = model._CustomerID,
                        OrderID = model._OrderID,
                        ValidatedByID = userId
                    };
                    _Context.OrderValidations.Add(newOrderValidation);
                    await _Context.SaveChangesAsync();
                    return newOrderValidation;
                }
                else
                {
                    orderValidated._DateValidated = time;
                    orderValidated.ValidatedByID = userId;
                    await _Context.SaveChangesAsync();
                    return orderValidated;
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<DateTime> GetUserTimeZone(string userId)
        {
            var user = await _Context.Users.Include(x => x.Country).SingleOrDefaultAsync(x => x.Id == userId);


            var currentUtcTime = DateTime.UtcNow;
            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById(user.Country._TimeZoneName);
            DateTime dateNow = TimeZoneInfo.ConvertTimeFromUtc(currentUtcTime, cstZone);
            return dateNow;
        }

        public async Task<OrderAvailable> OrderAvailable(ValidateOrderResource model, string userId)
        {
            try
            {

                var time = await GetUserTimeZone(userId);

                var orderAvailable = await _Context.OrderAvailables.Where(x => x.CustomerID == model._CustomerID
                                                                            && x.OrderID == model._OrderID).FirstOrDefaultAsync();
                if (orderAvailable == null)
                {
                    var newOrderAvailable = new OrderAvailable()
                    {
                        _CreationDate = time,
                        CustomerID = model._CustomerID,
                        OrderID = model._OrderID,
                        CreatedByID = userId
                    };
                    _Context.OrderAvailables.Add(newOrderAvailable);
                    await _Context.SaveChangesAsync();
                    return newOrderAvailable;
                }
                else
                {
                    orderAvailable._CreationDate = time;
                    orderAvailable.CreatedByID = userId;
                    await _Context.SaveChangesAsync();
                    return orderAvailable;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PickupResponse> OrderAvailableV1(ValidateOrderResource model, string userId)
        {
            try
            {
                // check if customers cylinders and keys has all been validated
                var response = await CheckCustomerProduction(model._CustomerID);
                if (!response._Succeeded)
                {
                    return response;
                }

                var time = await GetUserTimeZone(userId);

                var orderAvailable = await _Context.OrderAvailables.Where(x => x.CustomerID == model._CustomerID
                                                                            && x.OrderID == model._OrderID).FirstOrDefaultAsync();
                if (orderAvailable == null)
                {
                    var newOrderAvailable = new OrderAvailable()
                    {
                        _CreationDate = time,
                        CustomerID = model._CustomerID,
                        OrderID = model._OrderID,
                        CreatedByID = userId
                    };
                    _Context.OrderAvailables.Add(newOrderAvailable);
                    await _Context.SaveChangesAsync();
                    response._OrderAvailable = newOrderAvailable;
                    return response;
                }
                else
                {
                    orderAvailable._CreationDate = time;
                    orderAvailable.CreatedByID = userId;
                    await _Context.SaveChangesAsync();
                    response._OrderAvailable = orderAvailable;
                    return response;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        private async Task<PickupResponse> CheckCustomerProduction(int customerId)
        {
            var response = new PickupResponse() { _Succeeded = false };

            var cylinders = await _Context.Cylinders.Where(x => x.CustomerID == customerId).ToListAsync();
            var keys = await _Context.Groups.Where(x => x.CustomerID == customerId).ToListAsync();

            bool cylinderProductionPending = cylinders.Any(x => x._Validated != 1);
            bool keyProductionPending = keys.Any(x => x._Validated != 1);

            if(cylinderProductionPending && keyProductionPending)
            {
                response._Message = StringStore.CylinderAndKeyProductionPending;
            }
            else if(!cylinderProductionPending && keyProductionPending)
            {
                response._Message = StringStore.KeyProductionPending;
            }
            else if (cylinderProductionPending && !keyProductionPending)
            {
                response._Message = StringStore.CylinderProductionPending;
            }
            else
            {
                response._Message = StringStore._Success;
                response._Succeeded = true;
            }
            return response;
        }

        public async Task<OrderAvailable> GetPickUpDetail(int customerId)
        {
            try
            {
                return await _Context.OrderAvailables.Where(x => x.CustomerID == customerId)
                                                    .Include(x => x.CreatedBy)
                                                    .FirstOrDefaultAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }

    
}
