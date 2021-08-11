using Allprimetech.GeneralUtils;
using Allprimetech.Interfaces;
using Allprimetech.Interfaces.Constants;
using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources;
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
    public class DiscRepository : IDiscRepository
    {
        private DbContextOptions<ApplicationDbContext> _Options;
        private IProductionRepository _ProductionRepo;

        private ApplicationDbContext _Context { get; set; }

        public DiscRepository(
            ApplicationDbContext context, 
            DbContextOptions<ApplicationDbContext> options,
            IProductionRepository productionRepository)
        {
            _Context = context;
            _Options = options;
            _ProductionRepo = productionRepository;
        }
        public async Task AddDisc(Disc disc)
        {
            disc._CreationDate = DateTime.Now;
            disc._UpdatedDate = DateTime.Now;
            await _Context.Discs.AddAsync(disc);
        }

        public void DeleteDisc(Disc disc)
        {
            _Context.Discs.Remove(disc);
        }

        public async Task<Disc> GetDisc(int id)
        {
            try
            {
                return await _Context.Discs.FindAsync(id);
            }
            catch (Exception ex)
            {
                Logs.logError("DiscRepository", "GetDisc", ex.Message);
                throw;
            }
        }

        public async Task<Disc> GetDiscByName(string name)
        {
            try
            {
                return await _Context.Discs.Where(x => x._Name == name).SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Logs.logError("DiscRepository", "GetDiscByName", ex.Message);
                return null;
            }
        }

        public async Task<List<Disc>> GetDiscs(int numOfItems, int customerId)
        {
            try
            {
                var discs = await _Context.Discs.Where(x => x.CustomerID == customerId).ToListAsync();
                return discs.Take(numOfItems).ToList();
            }
            catch (Exception ex)
            {
                Logs.logError("DiscRepository", "GetDiscs", ex.Message);
                return null;
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
                Logs.logError("DiscRepository", "GetCustomerById", ex.Message);
                return null;
            }
        }

        public async Task<Cylinder> GetCylinderById(int cylinderID)
        {
            try
            {
                return await _Context.Cylinders.FindAsync(cylinderID);
            }
            catch (Exception ex)
            {
                Logs.logError("DiscRepository", "GetCylinderById", ex.Message);
                return null;
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
                //return groupInfos.Take(numOfItems).ToList();
                return groupInfos.ToList();
            }
            catch (Exception ex)
            {
                Logs.logError("DiscRepository", "GetGroupsInfo", ex.Message);
                return null;
            }
        }

        public async Task<List<ReadGroupInfoCodeList>> GetGroupsInfoBruckner(int numOfItems, int customerId)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                List<ReadGroupInfoCodeList> readDiscInfos = new List<ReadGroupInfoCodeList>();
                #region Cylinders Related Groups

                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {
                        var sqlScript = StringStore.GetGroupInfo;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {
                            while (result.Read())
                            {
                                ReadGroupInfoCodeList groupInfo = new ReadGroupInfoCodeList();
                                groupInfo.GroupsInfoID = Convert.ToInt32(result["GroupsInfoID"]);
                                groupInfo.GroupID = Convert.ToInt32(result["GroupID"]);
                                groupInfo.CustomerID = Convert.ToInt32(result["CustomerID"]);
                                groupInfo._Quantity = Convert.ToInt32(result["_Quantity"]);
                                groupInfo._Row = Convert.ToInt32(result["_Row"]);
                                groupInfo._Slot = Convert.ToInt32(result["_Slot"]);
                                groupInfo._Value = Convert.ToInt32(result["_Value"]);

                                groupInfo._SystemCode = result["_SystemCode"].ToString();
                                groupInfo._InstallationCode = result["_InstallationCode"].ToString();

                                groupInfo._GroupNumber = result["_GroupNumber"].ToString();
                                groupInfo._Group = result["_Group"].ToString();
                                groupInfo._Customer = result["_Group"].ToString();

                                readDiscInfos.Add(groupInfo);
                            }

                        }
                    }


                });
                return readDiscInfos.ToList();
                #endregion
            }
            catch (Exception ex)
            {
                Logs.logError("DiscRepository", "GetGroupsInfo", ex.Message);
                return null;
            }
        }

        public async Task<List<ReadDiscInfo>> GetDiscsInfo(int numOfItems, int customerId)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                List<ReadDiscInfo> readDiscInfos = new List<ReadDiscInfo>();
                #region Cylinders Related Groups

                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {
                        var sqlScript = StringStore.GetDiscInfo;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {
                            while (result.Read())
                            {
                                ReadDiscInfo discInfo = new ReadDiscInfo();
                                discInfo.DiscID = Convert.ToInt32(result["DiscID"]);
                                discInfo._Number = Convert.ToInt32(result["_Number"]);
                                discInfo._Slot = Convert.ToInt32(result["_Slot"]);
                                discInfo._Type = Convert.ToInt32(result["_Type"]);
                                discInfo.CylinderID = Convert.ToInt32(result["CylinderID"]);
                                discInfo.CustomerID = Convert.ToInt32(result["CustomerID"]);
                                discInfo._Quantity = Convert.ToInt32(result["_Quantity"]);
                                discInfo._Options = Convert.ToInt32(result["_Options"]);
                                discInfo._Color = result["_Color"].ToString();
                                discInfo._DoorName = result["_DoorName"].ToString();
                                discInfo._Name = result["_Name"].ToString();
                                discInfo._LengthOutside = result["_LengthOutside"].ToString();
                                discInfo._LengthInside = result["_LengthInside"].ToString();
                                discInfo._InstallationCode = result["_InstallationCode"].ToString();
                                discInfo._SystemCode = result["_SystemCode"].ToString();
                                discInfo._CylinderNumber = result["_CylinderNumber"].ToString();

                                readDiscInfos.Add(discInfo);
                            }

                        }
                    }


                });
                return readDiscInfos.Take(numOfItems).ToList();
                #endregion
            }
            catch (Exception ex)
            {
                Logs.logError("DiscRepository", "GetDiscsInfo", ex.Message);
                return null;
            }
        }

        public async Task<List<ReadDiscWithOccurrence>> GetDiscsTypes(int numOfItems, int customerId)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                List<ReadDiscWithOccurrence> readDiscInfos = new List<ReadDiscWithOccurrence>();
                #region Get Discs Types

                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {
                        var sqlScript = StringStore.GetDiscsTypes;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {
                            while (result.Read())
                            {
                                ReadDiscWithOccurrence discInfo = new ReadDiscWithOccurrence();
                                discInfo.DiscID = Convert.ToInt32(result["DiscID"]);
                                discInfo._Number = Convert.ToInt32(result["_Number"]);
                                discInfo._Slot = Convert.ToInt32(result["_Slot"]);
                                discInfo._Type = Convert.ToInt32(result["_Type"]);
                                discInfo._Occurrences = Convert.ToInt32(result["_Occurrences"]);
                                discInfo.CylinderID = Convert.ToInt32(result["CylinderID"]);
                                discInfo._Name = result["_Name"].ToString();
                                discInfo._InstallationCode = result["_InstallationCode"].ToString();
                                discInfo._SystemCode = result["_SystemCode"].ToString();

                                readDiscInfos.Add(discInfo);
                            }

                        }
                        var hold = readDiscInfos.Where(x => x._Name == "29B" || x._Name == "27B").ToList();
                    }


                });
                return readDiscInfos.Take(numOfItems).ToList();
                #endregion
            }
            catch (Exception ex)
            {
                Logs.logError("DiscRepository", "GetDiscsTypes", ex.Message);
                return null;
            }
        }

        public async Task<List<ReadDiscWithOccurrence>> GetDiscsStatistics(int numOfItems, int customerId)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                List<ReadDiscWithOccurrence> readDiscInfos = new List<ReadDiscWithOccurrence>();
                #region Get Discs Types

                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {
                        var sqlScript = StringStore.GetDiscStatistics;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text; 
                        command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {
                            while (result.Read())
                            {
                                ReadDiscWithOccurrence discInfo = new ReadDiscWithOccurrence();
                                discInfo.DiscID = Convert.ToInt32(result["DiscID"]);
                                discInfo._Number = Convert.ToInt32(result["_Number"]);
                                discInfo._Slot = Convert.ToInt32(result["_Slot"]);
                                discInfo._Type = Convert.ToInt32(result["_Type"]);
                                discInfo._Quantity = Convert.ToInt32(result["_Quantity"]);
                                discInfo._Occurrences = 0;
                                discInfo.CylinderID = Convert.ToInt32(result["CylinderID"]);
                                discInfo._Name = result["_Name"].ToString();
                                discInfo._InstallationCode = result["_InstallationCode"].ToString();
                                discInfo._SystemCode = result["_SystemCode"].ToString();

                                readDiscInfos.Add(discInfo);
                            }

                        }
                    }


                });
                return readDiscInfos.Take(numOfItems).ToList();
                #endregion
            }
            catch (Exception ex)
            {
                Logs.logError("DiscRepository", "GetDiscsTypes", ex.Message);
                return null;
            }
        }

        public async Task<List<ReadDiscWithOccurrence>> GetDiscsStatisticsBruckner(int numOfItems, int customerId)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                List<ReadDiscWithOccurrence> readDiscInfos = new List<ReadDiscWithOccurrence>();
                #region Get Discs Types

                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {
                        var sqlScript = StringStore.GetDiscStatisticsBruckner;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {
                            while (result.Read())
                            {
                                ReadDiscWithOccurrence discInfo = new ReadDiscWithOccurrence();
                                discInfo.DiscID = Convert.ToInt32(result["DiscID"]);
                                discInfo._Number = Convert.ToInt32(result["_Number"]);
                                discInfo._Slot = Convert.ToInt32(result["_Slot"]);
                                discInfo._Type = Convert.ToInt32(result["_Type"]);
                                discInfo._Quantity = Convert.ToInt32(result["_Quantity"]);
                                discInfo._Occurrences = 0;
                                discInfo.CylinderID = Convert.ToInt32(result["CylinderID"]);
                                discInfo._Name = result["_Name"].ToString();
                                discInfo._InstallationCode = result["_InstallationCode"].ToString();
                                discInfo._SystemCode = result["_SystemCode"].ToString();

                                readDiscInfos.Add(discInfo);
                            }

                        }
                    }


                });
                return readDiscInfos.Take(numOfItems).ToList();
                #endregion
            }
            catch (Exception ex)
            {
                Logs.logError("DiscRepository", "GetDiscsTypes", ex.Message);
                return null;
            }
        }


        public async Task<List<ReadDiscInfo>> GetCylinderCodeList(int customerId)
        {
            try
            {
                var cylinderGroups = await _ProductionRepo.GetCylinderGroups(customerId);
                var discsInfo = await GetDiscsInfo(StringStore.Limit, customerId);
                for (int i = 0; i < discsInfo.Count; i++)
                {
                    var disc = discsInfo[i];
                    var group = cylinderGroups.Where(x => x._CylinderID == disc.CylinderID).FirstOrDefault();
                    disc._GroupNumbers = group._GroupNumbers;
                }
                return discsInfo;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteDiscs(int customerId)
        {
            try
            {
                var discs = await _Context.Discs.Where(x => x.CustomerID == customerId).ToListAsync();
                var groupInfos = await _Context.GroupsInfos.Where(x => x.CustomerID == customerId).ToListAsync();
                
                _Context.RemoveRange(discs);
                _Context.RemoveRange(groupInfos);

                await _Context.SaveChangesAsync();

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
