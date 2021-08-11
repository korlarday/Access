using Allprimetech.GeneralUtils;
using Allprimetech.Interfaces;
using Allprimetech.Interfaces.Constants;
using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources;
using Allprimetech.Interfaces.Resources.Customer;
using Allprimetech.Interfaces.Resources.EnumResource;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allprimetech.DAL
{
    public class CustomerRepository : ICustomersRepository
    {
        private ApplicationDbContext _Context { get; set; }
        private readonly DbContextOptions<ApplicationDbContext> _Options;
        public Random _Random { get; set; }
        public CustomerRepository(ApplicationDbContext context, DbContextOptions<ApplicationDbContext> options)
        {
            _Context = context;
            _Options = options;
            _Random = new Random();
            options = new DbContextOptions<ApplicationDbContext>();
        }
        public async Task AddCustomer(Customer customer, string userId, string partnerName)
        {
            try
            {
                //Partner partner = await _Context.Partners.Where(x => x._Name == partnerName).FirstOrDefaultAsync();
                //if(partner != null)
                //{
                //    customer.PartnerID = partner.PartnerID;
                //}
                var user = await _Context.Users.FindAsync(userId);
                customer.PartnerID = user.PartnerID;
                customer._CreationDate = DateTime.UtcNow;
                customer.CreatedById = userId;
                customer._UpdatedDate = DateTime.UtcNow;
                //customer._SystemCode = GenerateCustomerCode(4);
                customer._InstallationCode = GenerateCustomerCode(8);
                await _Context.Customers.AddAsync(customer);

            }
            catch (Exception ex)
            {
                Logs.logError("CustomerRepository", "AddCustomer", ex.Message);
                throw;
            }
        }

        public async Task<List<Customer>> AllCustomers(string userId)
        {
            try
            {
                var user = await _Context.ApplicationUsers.FindAsync(userId);

                
                //return await _Context.Customers
                //                .Where(x => x.PartnerID == user.PartnerID)
                //                .Include(x => x.Partner)
                //                .Include(x => x.CreatedBy)
                //                .Include(x => x._Orders)
                //                .ToListAsync();
                if (user.PartnerID == null)
                {
                    return await _Context.Customers
                                .Where(x => x._EmailVerified == true
                                    && x._IsDeleted == false
                                    && x._CustomerStatus == UserStatus.Activated)
                                .Include(x => x.Partner)
                                .Include(x => x.CreatedBy)
                                .Include(x => x._Orders)
                                .ToListAsync();
                }
                else
                {
                    // check if user is the main partner
                    var partner = await _Context.Partners.FindAsync(user.PartnerID);
                    if(partner._Email == user.Email)
                    {
                        // if user is the main partner, then get all his customers
                        return await _Context.Customers
                                    .Where(x => x.PartnerID == user.PartnerID 
                                            && x._EmailVerified == true 
                                            && x._IsDeleted == false 
                                            && x._CustomerStatus == UserStatus.Activated)
                                    .Include(x => x.Partner)
                                    .Include(x => x.CreatedBy)
                                    .Include(x => x._Orders)
                                    .ToListAsync();

                    }
                    else
                    {
                        return await _Context.ApplicationUserCustomers.Where(x => x.ApplicationUserID == userId)
                                                                      .Include(x => x.Customer)
                                                                        .ThenInclude(x => x.Partner)
                                                                      .Include(x => x.Customer)
                                                                        .ThenInclude(x => x.CreatedBy)
                                                                      .Include(x => x.Customer)
                                                                        .ThenInclude(x => x._Orders)
                                                                      .Select(x => x.Customer)
                                                                      .ToListAsync();
                    }

                }
            }
            catch (Exception ex)
            {
                Logs.logError("CustomerRepository", "AllCustomers", ex.Message);
                throw;
            }
        }

        public void DeleteCustomer(Customer customer)
        {
            _Context.Customers.Remove(customer);
        }

        public async Task<Customer> GetCustomer(int id)
        {
            try
            {
                return await _Context.Customers
                            .Include(x => x.Partner)
                            .Include(x => x.CreatedBy)
                            .Include(x => x._Orders)
                            .SingleOrDefaultAsync(x => x.CustomerID == id);
            }
            catch (Exception ex)
            {
                Logs.logError("CustomerRepository", "GetCustomer", ex.Message);
                throw;
            }
        }



        ////////////////////////
        ///


        public string GenerateCustomerCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_Random.Next(s.Length)]).ToArray());
            //string path = Path.GetRandomFileName();
            //path = path.Replace(".", ""); // Remove period.
            //return path.Substring(0, 8);
        }
        public string GenerateRandomAlphabet(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_Random.Next(s.Length)]).ToArray());
        }

        public async Task<Customer> GetCustomerByName(string name)
        {
            try
            {
                return await _Context.Customers.Where(x => x._Name == name).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Logs.logError("CustomerRepository", "GetCustomerByName", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<ReadCustomerResource>> SearchCustomers(CustomerSearchResource model, string userId)
        {
            try
            {
                var user = await _Context.Users.FindAsync(userId);
                int? partnerId = null;
                if (user != null)
                    partnerId = user.PartnerID;

                CustomerSearchEnum selectedFieldType = (CustomerSearchEnum)Enum.Parse(typeof(CustomerSearchEnum), model._SearchTerm);

                var context = new ApplicationDbContext(_Options);
                List<ReadCustomerResource> filteredCustomers = new List<ReadCustomerResource>();
                await Task.Run(() => {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {
                        var sqlScript = StringStore.SearchCustomers;

                        if(partnerId != null)
                        {
                            sqlScript += StringStore.SearchWithPartnerId;
                        }

                        if(selectedFieldType == CustomerSearchEnum.GCustName)
                        {
                            sqlScript += StringStore.SearchNameField; 
                        }
                        else if(selectedFieldType == CustomerSearchEnum.GCustNumber)
                        {
                            sqlScript += StringStore.SearchCustomerNumbField; 
                        }
                        else if(selectedFieldType == CustomerSearchEnum.GCustPartner)
                        {
                            sqlScript += StringStore.SearchCustomerPartnerField;
                        }
                        else if(selectedFieldType == CustomerSearchEnum.GCustSystemCode)
                        {
                            sqlScript += StringStore.SearchSystemCodeField;
                        }
                        

                        sqlScript += StringStore.GroupByCustomerId;
                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;

                        command.Parameters.Add(new MySqlParameter("@searchValue", "%" + model._SearchValue + "%"));
                        command.Parameters.Add(new MySqlParameter("@partnerId",  partnerId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                ReadCustomerResource customer = new ReadCustomerResource();
                                customer.CreatedById = result["CreatedById"].ToString();
                                customer.CreatedBy = result["CreatedBy"].ToString();
                                customer.CustomerID = Convert.ToInt32(result["CustomerID"]);
                                customer._Orders = Convert.ToInt32(result["_Orders"]);
                                customer._Partner = result["Partner"].ToString();
                                customer.PartnerID = result["PartnerID"] is DBNull ? (int?)null : Convert.ToInt32(result["PartnerID"]);
                                customer._ContactPerson = result["_ContactPerson"].ToString();
                                customer._CustomerNumber = result["_CustomerNumber"].ToString();
                                customer._CreationDate = (DateTime)result["_CreationDate"];
                                customer._UpdatedDate = (DateTime)result["_UpdatedDate"];
                                customer._InstallationCode = result["_InstallationCode"].ToString();
                                customer._Name = result["_Name"].ToString();
                                customer._SystemCode = result["_SystemCode"].ToString();

                                filteredCustomers.Add(customer);
                            }
                        }
                    }
                });
                return filteredCustomers;
            }
            catch (Exception ex)
            {
                Logs.logError("CustomerRepository", "SearchCustomers", ex.Message);
                throw;
            }
        }

        private bool CheckPartner(Partner partner, string searchTerm)
        {
            try
            {
                if (partner == null) return false;

                return partner._Name.Contains(searchTerm);
            }
            catch (Exception ex)
            {
                Logs.logError("CustomerRepository", "CheckPartner", ex.Message);
                throw;
            }
        }

        public async Task<SystemCodeResource> GetNewCustomerSystemCode()
        {
            try
            {
                var response = new SystemCodeResource();

                #region SystemCode
                var customers = await _Context.Customers.OrderByDescending(x => x._CreationDate).ToListAsync();
                var lastCustomerEntry = customers.FirstOrDefault();
                if(lastCustomerEntry == null || lastCustomerEntry._SystemCode.Length != 4)
                {
                    response._SystemCode = "01AA";
                }
                else
                {
                    var lastCode = lastCustomerEntry._SystemCode;
                    var splittedCode = lastCode.ToCharArray();
                    var count = 0;
                    var num = Convert.ToInt32(splittedCode[0].ToString() + splittedCode[1].ToString());
                    bool changeChar = false;

                    if(num >= 99)
                    {
                        count = 0;
                        changeChar = true;
                    }
                    else
                    {
                        count = num + 1;
                    }

                    var firstAlphabet = Convert.ToInt32(splittedCode[2]);
                    var secondAlphabet = Convert.ToInt32(splittedCode[3]);
                    //65-90

                    if(changeChar)
                    {
                        if(secondAlphabet >= 90)
                        {
                            secondAlphabet = 65;
                            firstAlphabet += 1;
                        }
                        else
                        {
                            secondAlphabet += 1;
                        }
                    }
                    var firstChar = (char)firstAlphabet;
                    var secondChar = (char)secondAlphabet;
                    var newSystemCode = count.ToString("00") + firstChar + secondChar;

                    response._SystemCode = newSystemCode;
                }

                #endregion
                
                // customer number
                var customerCount = customers.Count().ToString("00");
                var rand1 = GenerateRandomAlphabet(3);
                var day = DateTime.UtcNow.Day;
                var month = DateTime.UtcNow.Month.ToString("00");
                var year = DateTime.UtcNow.Year;

                // order Number
                var orderCount = await _Context.Orders.CountAsync();
                var rand2 = GenerateRandomAlphabet(3);

                response._CustomerNumber = customerCount + rand1 + day + month + year;
                response._OrderNumber = orderCount + rand2 + day + month + year;
                return response;
            }
            catch (Exception ex)
            {
                Logs.logError("CustomerRepository", "GetNewCustomerSystemCode", ex.Message);
                throw;
            }
        }

        public async Task<Customer> GetCustomerBySystemCode(string systemCode)
        {
            return await _Context.Customers.Where(x => x._SystemCode == systemCode).FirstOrDefaultAsync();
        }

        public async Task<Configuration> GetConfiguration()
        {
            return await _Context.Configurations.FirstOrDefaultAsync();
        }

        public async Task<LockingPlanInfoResource> GetCustomerLockingPlanInfo(int customerId)
        {
            var cylinders = await _Context.Cylinders.Where(x => x.CustomerID == customerId).CountAsync();
            var groups = await _Context.Groups.Where(x => x.CustomerID == customerId).CountAsync();
            return new LockingPlanInfoResource
            {
                _CustomerID = customerId,
                _CylinderCount = cylinders,
                _GroupCount = groups
            };
        }

        public async Task<Customer> GetCustomerByEmail(string email)
        {
            return await _Context.Customers.Where(x => x._Email == email).FirstOrDefaultAsync();
        }
    }
}
