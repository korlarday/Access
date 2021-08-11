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
    public class GroupRepository : IGroupRepository
    {
        private DbContextOptions<ApplicationDbContext> _Options;
        private IMapper _Mapper { get; set; }

        private ApplicationDbContext _Context { get; set; }

        public GroupRepository(ApplicationDbContext context, DbContextOptions<ApplicationDbContext> options, IMapper mapper)
        {
            _Context = context;
            this._Options = options;
            _Mapper = mapper;
        }
        public async Task AddGroup(Group group, string userId)
        {
            group._CreationDate = DateTime.UtcNow;
            group._UpdatedDate = DateTime.UtcNow;
            await _Context.Groups.AddAsync(group);
            await _Context.SaveChangesAsync();
        }

        public async Task<List<Group>> AllGroups(int customerId)
        {
            try
            {
                return await _Context.Groups.Where(x => x.CustomerID == customerId).ToListAsync();

            }
            catch (Exception ex)
            {
                Logs.logError("GroupRepository", "AllGroups", ex.Message);
                throw;
            }
        }

        public void DeleteGroup(Group group)
        {
            _Context.Groups.Remove(group);
        }

        public async Task<Group> GetGroup(int id)
        {
            try
            {
                return await _Context.Groups.SingleOrDefaultAsync(x => x.GroupID == id);

            }
            catch (Exception ex)
            {
                Logs.logError("GroupRepository", "GetGroup", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<Group>> SearchGroups(GroupSearchResource model)
        {
            #region Search Groups
            try
            {
                GroupSearchEnum selectedFieldType = GroupSearchEnum.GKeyName;
                if(!String.IsNullOrWhiteSpace(model._SearchTerm))
                {
                    selectedFieldType = (GroupSearchEnum)Enum.Parse(typeof(GroupSearchEnum), model._SearchTerm);
                }


                var from = DateTime.UtcNow;
                var to = DateTime.UtcNow;

                var context = new ApplicationDbContext(_Options);
                List<Group> filteredGroups = new List<Group>();
                await Task.Run(() => {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {
                        var sqlScript = @"select * from groups g inner join orders o on o.OrderID = g.OrderID where 1=1 ";

                        if (!String.IsNullOrWhiteSpace(model._SearchTerm) && selectedFieldType == GroupSearchEnum.GKeyName)
                        {
                            sqlScript += " and g._Name like @searchValue";
                        }
                        else if (!String.IsNullOrWhiteSpace(model._SearchTerm) && selectedFieldType == GroupSearchEnum.GKeyNumber)
                        {
                            sqlScript += " and g._GroupNumber like @searchValue";
                        }

                        sqlScript += " and g.CustomerID = @customerId";



                        if (model._OrderNumbers != null && model._OrderNumbers.Count() > 0)
                        {
                            if(model._OrderNumbers.Count == 1)
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
                                        if(i == model._OrderNumbers.Count() - 1)
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


                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;

                        command.Parameters.Add(new MySqlParameter("@searchValue", "%" + model._SearchValue + "%"));
                        //command.Parameters.Add(new MySqlParameter("@from", from));
                        //command.Parameters.Add(new MySqlParameter("@to", to));
                        command.Parameters.Add(new MySqlParameter("@customerId", model._CustomerID));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                Group group = new Group();
                                group.GroupID = Convert.ToInt32(result["GroupID"]);
                                group._Quantity = Convert.ToInt32(result["_Quantity"]);
                                group._CreationDate = (DateTime)result["_CreationDate"];
                                group._UpdatedDate = (DateTime)result["_UpdatedDate"];
                                group._Blocked = Convert.ToInt32(result["_Blocked"]);
                                group._KeyNumber = result["_KeyNumber"].ToString();
                                group.OrderID = Convert.ToInt32(result["OrderID"]);
                                group._Produced = Convert.ToInt32(result["_Produced"]);
                                group._Validated = Convert.ToInt32(result["_Validated"]);
                                group._PositionId = Convert.ToInt32(result["_PositionId"]);
                                group._Reclaimed = Convert.ToInt32(result["_Reclaimed"]);
                                group._Name = result["_Name"].ToString();
                                group._GroupNumber = result["_GroupNumber"].ToString();
                                filteredGroups.Add(group);
                            }
                        }
                    }
                });
                if (model._From != null)
                {
                    from = (DateTime)model._From;
                    filteredGroups = filteredGroups.Where(x => x._CreationDate.Date >= from.Date).ToList();
                }
                if (model._To != null)
                {
                    to = (DateTime)model._To;
                    filteredGroups = filteredGroups.Where(x => x._CreationDate.Date <= to.Date).ToList();
                }
                return filteredGroups;
            }
            catch (Exception ex)
            {
                Logs.logError("GroupRepository", "SearchGroups", ex.Message);
                throw;
            }
            #endregion
        }

        public async Task<Customer> GetCustomerByName(string customerName)
        {
            try
            {
                return await _Context.Customers.Where(x => x._Name == customerName).FirstOrDefaultAsync();

            }
            catch (Exception ex)
            {
                Logs.logError("GroupRepository", "GetCustomerByName", ex.Message);
                throw;
            }
        }

        public async Task<Order> GetOrderByNumber(string orderNumber)
        {
            try
            {
                return await _Context.Orders.Where(x => x._OrderNumber == orderNumber).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Logs.logError("GroupRepository", "GetOrderByNumber", ex.Message);
                throw;
            }
        }

        public async Task<Customer> GetCustomerByNumber(string customerNumber)
        {
            try
            {
                return await _Context.Customers.Where(x => x._CustomerNumber == customerNumber).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Logs.logError("GroupRepository", "GetCustomerByNumber", ex.Message);
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
                Logs.logError("GroupRepository", "GetOrderByOrderNumber", ex.Message);
                throw;
            }
        }

        public async Task<List<Group>> GetGroupsByOrder(int orderID)
        {
            try
            {
                return await _Context.Groups.Where(x => x.OrderID == orderID).OrderBy(x => x.OrderID).ToListAsync();
            }
            catch (Exception ex)
            {
                Logs.logError("GroupRepository", "GetGroupsByOrder", ex.Message);
                throw;
            }
        }

        public async Task<int> GetGroupBatchIdByCustomerName(string customerName)
        {
            try
            {
                var customer = await GetCustomerByName(customerName);
                if (customer == null)
                    return 0;
                var groups = await _Context.Groups.Where(x => x.CustomerID == customer.CustomerID).ToListAsync();
                var batches = groups.Select(x => x._BatchNumber).Distinct();
                var lastBatch = batches.OrderByDescending(x => x).FirstOrDefault();
                int keyIdStart = groups.Where(x => x._BatchNumber == lastBatch)
                                        .OrderBy(x => x.GroupID).FirstOrDefault().GroupID;
                return keyIdStart;
            }
            catch (Exception ex)
            {
                Logs.logError("GroupRepository", "GetGroupBatchIdByCustomerName", ex.Message);
                throw;
            }
        }

        public async Task<int> GetLastBatchNumber(string customerName)
        {
            try
            {
                var customer = await GetCustomerByName(customerName);
                if (customer == null) return 0;
                var previousGroups = await _Context.Groups.Where(x => x.CustomerID == customer.CustomerID).ToListAsync();
                if (previousGroups == null || previousGroups.Count() < 1) return 0;

                var lastBatchNumber = previousGroups.Select(x => x._BatchNumber).Distinct().OrderByDescending(x => x)
                                                    .FirstOrDefault();
                return lastBatchNumber;
            }
            catch (Exception ex)
            {
                Logs.logError("GroupRepository", "GetLastBatchNumber", ex.Message);
                throw;
            }
        }

        public async Task<bool> IsGroupExistsWithCustomer(int customerId)
        {
            try
            {
                bool isExists = await _Context.Groups.AnyAsync(x => x.CustomerID == customerId);
                return isExists;
            }
            catch (Exception ex)
            {
                Logs.logError("GroupRepository", "IsGroupExistsWithCustomer", ex.Message);
                throw;
            }
        }

        public async Task CreateGroupsBulk(string insertQuery)
        {
            try
            {
                insertQuery = insertQuery.Remove(insertQuery.Length - 1);

                var context = new ApplicationDbContext(_Options);
                List<Cylinder> filteredCylinders = new List<Cylinder>();
                await Task.Run(() => {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {
                        var sqlScript = StringStore.InsertIntoGroup + insertQuery;

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
                Logs.logError("GroupRepository", "CreateGroupsBulk", ex.Message);
                throw;
            }
        }

        public async Task<List<ReadGroupResource>> ModifyGroup(List<CreateGroupResource> groupsResource, int customerId, string userId)
        {
            try
            {
                var createdGroups = new List<ReadGroupResource>();
                List<Group> newlyCreatedGroups = new List<Group>();

                var newGroups = groupsResource;
                var groupItem = groupsResource.FirstOrDefault();
                int orderId = groupItem._OrderID;

                var existingGroups = await _Context.Groups.Where(x => x.CustomerID == customerId).OrderByDescending(x => x._PositionId).ToListAsync();
                var lastSavedGroup = existingGroups.FirstOrDefault();
                //int lastPositionId = lastSavedGroup._PositionId;
                int batchNumber = lastSavedGroup != null ? lastSavedGroup._BatchNumber + 1 : 1;
                var orders = await _Context.Orders.Where(x => x.CustomerID == customerId).OrderByDescending(x => x.OrderID).ToListAsync();

                Order orderToModify = null;
                int orderToModifyId = 0;

                if (orderId != 0)
                {
                    var selectedOrder = orders.Where(x => x.OrderID == orderId).FirstOrDefault();
                    if (selectedOrder != null)
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



                // update the quantities for existing groups
                for (int i = 0; i < existingGroups.Count; i++)
                {
                    var existingGrp = existingGroups[i];
                    var updateGroup = groupsResource.Where(x => x.GroupID == existingGrp.GroupID).FirstOrDefault();
                    if (updateGroup != null)
                    {
                        newGroups.Remove(updateGroup);
                        if (updateGroup._Quantity != existingGrp._Quantity)
                        {

                            OrderDetail orderDetail = new OrderDetail
                            {
                                ByPersonId = userId,
                                OrderID = orderToModifyId,
                                _Date = DateTime.UtcNow,
                                _NewQty = updateGroup._Quantity,
                                _OldQty = existingGrp._Quantity,
                                _ProductID = existingGrp.GroupID,
                                _ProductType = ProductType.Key
                            };
                            _Context.OrderDetails.Add(orderDetail);

                            var newQuantity = updateGroup._Quantity - existingGrp._Quantity;
                            orderToModify._GroupKeyQty += newQuantity;

                            existingGrp._Quantity = updateGroup._Quantity;
                            existingGrp.OrderID = orderToModifyId;

                        }
                        existingGrp._GroupNumber = updateGroup._GroupNumber;
                        existingGrp._Name = updateGroup._KeyName;
                        existingGrp._PositionId = updateGroup._PositionId;

                        var readGroupResource = _Mapper.Map<Group, ReadGroupResource>(existingGrp);
                        readGroupResource._ModificationStatus = ModificationStatus.MODIFIED;
                        createdGroups.Add(readGroupResource);
                    }
                    else
                    {
                        orderToModify._GroupKeyQty -= existingGrp._Quantity;
                        var relationships = await _Context.CylinderGroups.Where(x => x.GroupID == existingGrp.GroupID).ToListAsync();
                        _Context.CylinderGroups.RemoveRange(relationships);
                        _Context.Groups.Remove(existingGrp);
                    }
                }

                for (int i = 0; i < newGroups.Count; i++)
                {
                    var group = newGroups[i];
                    // a new group
                    Group newGroupo = new Group(group);
                    newGroupo.OrderID = orderToModifyId;
                    newGroupo.CustomerID = customerId;
                    newGroupo._PositionId = group._PositionId;
                    newGroupo._BatchNumber = batchNumber;
                    _Context.Groups.Add(newGroupo);

                    orderToModify._GroupKeyQty += group._Quantity;

                    newlyCreatedGroups.Add(newGroupo);
                }


                await _Context.SaveChangesAsync();
                for (int i = 0; i < newlyCreatedGroups.Count; i++)
                {
                    var group = newlyCreatedGroups[i];
                    var groupResource = _Mapper.Map<Group, ReadGroupResource>(group);
                    groupResource._ModificationStatus = ModificationStatus.NEW;
                    createdGroups.Add(groupResource);
                }
                return createdGroups;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<List<Group>> RetrieveCreatedGroups(int customerId)
        {
            return await _Context.Groups.Where(x => x.CustomerID == customerId).ToListAsync();
        }

        public async Task<Customer> GetCustomerById(int customerID)
        {
            return await _Context.Customers.FindAsync(customerID);
        }
    }
}
