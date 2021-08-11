using Allprimetech.GeneralUtils;
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
    public class CylinderRepository : ICylinderRepository
    {
        private IMapper _Mapper { get; set; }

        private DbContextOptions<ApplicationDbContext> _Options { get; set; }

        private ApplicationDbContext _Context { get; set; }

        public CylinderRepository(ApplicationDbContext context, DbContextOptions<ApplicationDbContext> options, IMapper mapper)
        {
            _Context = context;
            this._Options = options;
            _Mapper = mapper;
        }
        public async Task AddCylinder(Cylinder cylinder, string userId)
        {
            cylinder._CreationDate = DateTime.UtcNow;
            cylinder._UpdatedDate = DateTime.UtcNow;
            
            await _Context.Cylinders.AddAsync(cylinder);
            await _Context.SaveChangesAsync();
        }

        public async Task<List<Cylinder>> AllCylinders(int customerId)
        {
            try
            {
                return await _Context.Cylinders
                            .Where(x => x.CustomerID == customerId)
                            .ToListAsync();
            }
            catch (Exception ex)
            {
                Logs.logError("CylinderRepository", "AllCylinders", ex.Message);
                throw;
            }
        }

        public void DeleteCylinder(Cylinder cylinder) 
        {
            _Context.Cylinders.Remove(cylinder);
        }

        public async Task<Cylinder> GetCylinder(int id)
        {
            try
            {
                return await _Context.Cylinders.FindAsync(id);
            }
            catch (Exception  ex)
            {
                Logs.logError("CylinderRepository", "GetCylinder", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<Cylinder>> SearchCylinders(CylinderSearchResource model)
        {
            try
            {
                CylinderSearchEnum selectedFieldType = CylinderSearchEnum.GCylArtNumber;
                if(!String.IsNullOrWhiteSpace(model._SearchTerm))
                {
                    selectedFieldType = (CylinderSearchEnum)Enum.Parse(typeof(CylinderSearchEnum), model._SearchTerm);
                }
                var from = DateTime.UtcNow;
                var to = DateTime.UtcNow;

                var context = new ApplicationDbContext(_Options);
                List<Cylinder> filteredCylinders = new List<Cylinder>();
                await Task.Run(() => {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {
                        var sqlScript = @"select * from cylinders c inner join orders o on o.OrderID = c.OrderID where 1=1 ";

                        if (!String.IsNullOrWhiteSpace(model._SearchTerm) && selectedFieldType == CylinderSearchEnum.GCylArtNumber)
                        {
                            //sqlScript += " and _ArticleNumber like @searchValue";
                        }
                        else if (!String.IsNullOrWhiteSpace(model._SearchTerm) && selectedFieldType == CylinderSearchEnum.GCylDrName)
                        {
                            sqlScript += " and _DoorName like @searchValue";
                        }
                        else if (!String.IsNullOrWhiteSpace(model._SearchTerm) && selectedFieldType == CylinderSearchEnum.GCylNumber)
                        {
                            sqlScript += " and _CylinderNumber like @searchValue";
                        }
                        else if (!String.IsNullOrWhiteSpace(model._SearchTerm) && selectedFieldType == CylinderSearchEnum.GCylOption)
                        {
                            //sqlScript += " and _Options like @searchValue";
                        }

                        if(model._OrderNumbers != null && model._OrderNumbers.Count() > 0)
                        {
                            if(model._OrderNumbers.Count() == 1)
                            {
                                sqlScript += $" and o._OrderNumber = @orderNumber1";
                                command.Parameters.Add(new MySqlParameter($"@orderNumber1", model._OrderNumbers.FirstOrDefault()));
                            }
                            else
                            {
                                for (int i = 0; i < model._OrderNumbers.Count; i++)
                                {
                                    var item = model._OrderNumbers[i];
                                    var label = i + 1;
                                    if (i == 0)
                                    {
                                        sqlScript += $" and (o._OrderNumber = @orderNumber{label}";
                                    }
                                    else
                                    {
                                        sqlScript += $" or o._OrderNumber = @orderNumber{label}";
                                        if (i == model._OrderNumbers.Count() - 1)
                                        {
                                            sqlScript += ")";
                                        }
                                    }

                                    if (!String.IsNullOrWhiteSpace(item))
                                    {
                                        command.Parameters.Add(new MySqlParameter($"@orderNumber{label}", item));
                                    }
                                }

                            }
                        }
                        

                        sqlScript += " and c.CustomerID = @customerId";

                        if(model._From != null)
                        {
                            from = (DateTime)model._From;
                            sqlScript += " and cast(c._CreationDate as date) >= @from";
                        }
                        if (model._To != null)
                        {
                            to = (DateTime)model._To;
                            sqlScript += " and cast(c._CreationDate as date) <= @to";
                        }

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;

                        command.Parameters.Add(new MySqlParameter("@searchValue", "%" + model._SearchValue + "%"));
                        command.Parameters.Add(new MySqlParameter("@from", from));
                        command.Parameters.Add(new MySqlParameter("@to", to));
                        command.Parameters.Add(new MySqlParameter("@customerId", model._CustomerID));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                Cylinder cylinder = new Cylinder();
                                cylinder.CylinderID = Convert.ToInt32(result["CylinderID"]);
                                cylinder.CustomerID = Convert.ToInt32(result["CustomerID"]);
                                cylinder._DoorName = result["_DoorName"].ToString();
                                cylinder._ArticleNumber = (ArticleNumber)Convert.ToInt32(result["_ArticleNumber"]);
                                cylinder._LengthInside = result["_LengthInside"].ToString();
                                cylinder._LengthOutside = result["_LengthOutside"].ToString();
                                cylinder._CylinderNumber = result["_CylinderNumber"].ToString();
                                cylinder._Color = result["_Color"].ToString();
                                cylinder._Options = (Options)Convert.ToInt32(result["_Options"]);
                                cylinder._Quantity = Convert.ToInt32(result["_Quantity"]);
                                cylinder._QRCodeIssued = Convert.ToInt32(result["_QRCodeIssued"]);
                                cylinder._CreationDate = (DateTime)result["_CreationDate"];
                                cylinder._UpdatedDate = (DateTime)result["_UpdatedDate"];
                                cylinder._Assembled = Convert.ToInt32(result["_Assembled"]);
                                cylinder._Blocked = Convert.ToInt32(result["_Blocked"]);
                                cylinder.OrderID = Convert.ToInt32(result["OrderID"]);

                                filteredCylinders.Add(cylinder);
                            }
                        }
                    }
                });

                if(!String.IsNullOrWhiteSpace(model._SearchTerm) && selectedFieldType == CylinderSearchEnum.GCylArtNumber)
                {
                    filteredCylinders = filteredCylinders.Where(x => x._ArticleNumber.ToString().ToLower().Contains(model._SearchValue.ToLower())).ToList();
                }
                else if (!String.IsNullOrWhiteSpace(model._SearchTerm) && selectedFieldType == CylinderSearchEnum.GCylOption)
                {
                    filteredCylinders = filteredCylinders.Where(x => x._Options.ToString().ToLower().Contains(model._SearchValue.ToLower())).ToList();
                }
                return filteredCylinders;
            }
            catch (Exception ex)
            {
                Logs.logError("CylinderRepository", "SearchCylinders", ex.Message);
                throw;
            }
        }

        public async Task<Customer> GetCustomerByName(string customer)
        {
            try
            {
                return await _Context.Customers.Where(x => x._Name == customer).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Logs.logError("CylinderRepository", "GetCustomerByName", ex.Message);
                throw;
            }
        }

        public async Task<Order> GetOrderByName(string projectName)
        {
            try
            {
                return await _Context.Orders.Where(x => x._ProjectName == projectName).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Logs.logError("CylinderRepository", "GetOrderByName", ex.Message);
                throw;
            }
        }

        public async Task<Order> GetOrderByOrderNumber(string orderNumber)
        {
            try
            {
                return await _Context.Orders.Where(x => x._OrderNumber == orderNumber).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Logs.logError("CylinderRepository", "GetOrderByOrderNumber", ex.Message);
                throw;
            }
        }

        public async Task<List<Cylinder>> GetCylindersByOrderId(int orderID)
        {
            try
            {
                return await _Context.Cylinders.Where(x => x.OrderID == orderID).OrderBy(x => x.OrderID).ToListAsync();
            }
            catch (Exception ex)
            {
                Logs.logError("CylinderRepository", "AllCylindersByOrderId", ex.Message);
                throw;
            }
        }

        public async Task<Order> GetOrderByNumber(string order)
        {
            try
            {
                return await _Context.Orders.Where(x => x._OrderNumber == order).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Logs.logError("CylinderRepository", "GetOrderByNumber", ex.Message);
                throw;
            }
        }

        public async Task<int> GetCylinderBatchIdByCustomerName(string customerName)
        {
            try
            {
                var customer = await GetCustomerByName(customerName);
                if (customer == null)
                    return 0;
                var cylinders = await _Context.Cylinders.Where(x => x.CustomerID == customer.CustomerID).ToListAsync();
                var batches = cylinders.Select(x => x._BatchNumber).Distinct();
                var lastBatch = batches.OrderByDescending(x => x).FirstOrDefault();
                int cylinderIdStart = cylinders.Where(x => x._BatchNumber == lastBatch)
                                        .OrderBy(x => x.CylinderID).FirstOrDefault().CylinderID;
                return cylinderIdStart;
            }
            catch (Exception ex)
            {
                Logs.logError("CylinderRepository", "GetCylinderBatchIdByCustomerName", ex.Message);
                throw;
            }
        }

        public async Task<int> GetLastBatchNumber(string customerName)
        {
            try
            {
                var customer = await GetCustomerByName(customerName);
                if (customer == null) return 0;
                var previousCyls = await _Context.Cylinders.Where(x => x.CustomerID == customer.CustomerID).ToListAsync();
                if (previousCyls == null || previousCyls.Count() < 1) return 0;

                var lastBatchNumber = previousCyls.Select(x => x._BatchNumber).Distinct().OrderByDescending(x => x)
                                                    .FirstOrDefault();
                return lastBatchNumber;
            }
            catch (Exception ex)
            {
                Logs.logError("CylinderRepository", "GetLastBatchNumber", ex.Message);
                throw;
            }
        }

        public async Task<bool> IsCylinderExistsWithCustomer(int customerId)
        {
            try
            {
                bool isExists = await _Context.Cylinders.AnyAsync(x => x.CustomerID == customerId);
                return isExists;
            }
            catch (Exception ex)
            {
                Logs.logError("CylinderRepository", "IsCylinderExistsWithCustomer", ex.Message);
                throw;
            }
        }

        public async Task AddCylinderInBulk(string insertQuery)
        {
            try
            {
                insertQuery = insertQuery.Remove(insertQuery.Length - 1);

                var context = new ApplicationDbContext(_Options);
                List<Cylinder> filteredCylinders = new List<Cylinder>();
                await Task.Run(() => {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {
                        var sqlScript = StringStore.InsertIntoCylinder + insertQuery;

                    

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                            
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Logs.logError("CylinderRepository", "AddCylinderInBulk", ex.Message);
                throw;
            }
        }



        public async Task<List<ReadCylinderResource>> ModifyCylinders(List<CreateCylinderResource> cylinders, int customerId, string userId)
        {
            try
            {
                var createdCylinders = new List<ReadCylinderResource>();
                var newlyCreatedCylinders = new List<Cylinder>();

                var newCylinders = cylinders;
                var cylinder = cylinders.FirstOrDefault();
                int orderId = cylinder._OrderID;
                var existingCylinders = await _Context.Cylinders.Where(x => x.CustomerID == customerId).OrderByDescending(x => x._PositionId).ToListAsync();
                
                var orders = await _Context.Orders.Where(x => x.CustomerID == customerId).OrderByDescending(x => x.OrderID).ToListAsync();
                Order orderToModify = null;
                int orderToModifyId = 0;
                if(orderId != 0)
                {
                    var selectedOrder = orders.Where(x => x.OrderID == orderId).FirstOrDefault();
                    if(selectedOrder != null)
                    {
                        orderToModify = selectedOrder;
                        orderToModifyId = selectedOrder.OrderID;
                    }
                    else
                    {
                        orderToModify = orders.FirstOrDefault();
                        orderToModifyId = orderToModify.OrderID;
                    }
                }
                else
                {
                    orderToModify = orders.FirstOrDefault();
                    orderToModifyId = orderToModify.OrderID;
                }


                for (int i = 0; i < existingCylinders.Count; i++)
                {
                    var existingCyl = existingCylinders[i];
                    var updateCyl = cylinders.Where(x => x.CylinderID == existingCyl.CylinderID).FirstOrDefault();
                    if (updateCyl != null)
                    {
                        newCylinders.Remove(updateCyl);

                        if (updateCyl._Quantity != existingCyl._Quantity)
                        {
                            OrderDetail orderDetail = new OrderDetail
                            {
                                ByPersonId = userId,
                                OrderID = orderToModifyId,
                                _Date = DateTime.UtcNow,
                                _NewQty = updateCyl._Quantity,
                                _OldQty = existingCyl._Quantity,
                                _ProductID = existingCyl.CylinderID,
                                _ProductType = ProductType.Cylinder
                            };
                            _Context.OrderDetails.Add(orderDetail);

                            var newCylinderQty = updateCyl._Quantity - existingCyl._Quantity;

                            existingCyl._Quantity = updateCyl._Quantity;
                            existingCyl.OrderID = orderToModifyId;

                            orderToModify._CylinderQty += newCylinderQty;

                        }

                        existingCyl._CylinderNumber = updateCyl._CylinderNumber;
                        existingCyl._DoorName = updateCyl._DoorName;
                        existingCyl._LengthInside = updateCyl._LengthInside;
                        existingCyl._LengthOutside = updateCyl._LengthOutside;
                        existingCyl._Color = updateCyl._Color;
                        existingCyl._Options = (Options)updateCyl._Options;
                        existingCyl._ArticleNumber = (ArticleNumber)updateCyl._ArticleNumber;
                        existingCyl._PositionId = updateCyl._PositionId;

                        var cylinderResource = _Mapper.Map<Cylinder, ReadCylinderResource>(existingCyl);
                        cylinderResource._ModificationStatus = ModificationStatus.MODIFIED;
                        createdCylinders.Add(cylinderResource);
                    }
                    else
                    {
                        orderToModify._CylinderQty -= existingCyl._Quantity;
                        var relationships = await _Context.CylinderGroups.Where(x => x.CylinderID == existingCyl.CylinderID).ToListAsync();
                        _Context.CylinderGroups.RemoveRange(relationships);
                        _Context.Cylinders.Remove(existingCyl);
                    }
                }

                for (int i = 0; i < newCylinders.Count; i++)
                {
                    var updateCyl = newCylinders[i];

                    var newCylinder = new Cylinder
                    {
                        _CylinderNumber = updateCyl._CylinderNumber,
                        _DoorName = updateCyl._DoorName,
                        _LengthInside = updateCyl._LengthInside,
                        _LengthOutside = updateCyl._LengthOutside,
                        _Options = (Options)updateCyl._Options,
                        _CreationDate = DateTime.UtcNow,
                        _UpdatedDate = DateTime.UtcNow,
                        _ArticleNumber = (ArticleNumber)updateCyl._ArticleNumber,
                        _Color = updateCyl._Color,
                        _Quantity = updateCyl._Quantity,
                        CustomerID = customerId,
                        OrderID = orderToModifyId,
                        _PositionId = updateCyl._PositionId
                    };
                    _Context.Cylinders.Add(newCylinder);
                    orderToModify._CylinderQty += updateCyl._Quantity;

                    newlyCreatedCylinders.Add(newCylinder);
                }


                await _Context.SaveChangesAsync();
                for (int i = 0; i < newlyCreatedCylinders.Count; i++)
                {
                    var newCylinder = newlyCreatedCylinders[i];
                    var cylinderResource = _Mapper.Map<Cylinder, ReadCylinderResource>(newCylinder);
                    cylinderResource._ModificationStatus = ModificationStatus.NEW;
                    createdCylinders.Add(cylinderResource);
                }
                return createdCylinders;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<List<Cylinder>> RetrieveStoredCylinders(int customerId)
        {
            return await _Context.Cylinders.Where(x => x.CustomerID == customerId).ToListAsync();
        }

        public async Task<Customer> GetCustomerById(int customerID)
        {
            return await _Context.Customers.FindAsync(customerID);
        }





        //public async Task<List<ReadCylinderResource>> ModifyCylinders(List<CreateCylinderResource> cylinders, int customerId, string userId)
        //{
        //    var createdCylinders = new List<ReadCylinderResource>();

        //    var existingCylinders = await _Context.Cylinders.Where(x => x.CustomerID == customerId).OrderByDescending(x => x._PositionId).ToListAsync();
        //    int lastPositionId = existingCylinders.FirstOrDefault()._PositionId;
        //    int batchNumber = existingCylinders.FirstOrDefault()._BatchNumber + 1;
        //    var orders = await _Context.Orders.Where(x => x.CustomerID == customerId).OrderByDescending(x => x.OrderID).ToListAsync();
        //    var lastOrderId = orders.FirstOrDefault().OrderID;


        //    for (int i = 1; i < existingCylinders.Count; i++)
        //    {
        //        var existingCyl = existingCylinders[i];
        //        var updateCyl = cylinders.Where(x => x._PositionId == existingCyl._PositionId).FirstOrDefault();
        //        if(updateCyl != null)
        //        {
        //            if(updateCyl._Quantity != existingCyl._Quantity)
        //            {
        //                OrderDetail orderDetail = new OrderDetail
        //                {
        //                    ByPersonId = userId,
        //                    OrderID = lastOrderId,
        //                    _Date = DateTime.UtcNow,
        //                    _NewQty = updateCyl._Quantity,
        //                    _OldQty = existingCyl._Quantity,
        //                    _ProductID = existingCyl.CylinderID,
        //                    _ProductType = ProductType.Cylinder
        //                };
        //                _Context.OrderDetails.Add(orderDetail);

        //                existingCyl._Quantity = updateCyl._Quantity;
        //                existingCyl.OrderID = lastOrderId;

        //            }
        //            existingCyl._CylinderNumber = updateCyl._CylinderNumber;
        //            existingCyl._DoorName = updateCyl._DoorName;
        //            existingCyl._LengthInside = updateCyl._LengthInside;
        //            existingCyl._LengthOutside = updateCyl._LengthOutside;
        //            existingCyl._Options = (Options)updateCyl._Options;
        //        }
        //    }


        //    // save newly added cylinders
        //    var counter = 1;
        //    for (int i = lastPositionId; i < cylinders.Count; i++)
        //    {
        //        var cylinder = cylinders[i];

        //        var newCylinder = new Cylinder(cylinder);
        //        newCylinder.OrderID = lastOrderId;
        //        newCylinder.CustomerID = customerId;
        //        newCylinder._PositionId = lastPositionId + counter;
        //        newCylinder._BatchNumber = batchNumber;
        //        _Context.Cylinders.Add(newCylinder);

        //        createdCylinders.Add(_Mapper.Map<Cylinder, ReadCylinderResource>(newCylinder));

        //        ++counter;
        //    }
        //    await _Context.SaveChangesAsync();
        //    return createdCylinders;
        //}
    }
}
