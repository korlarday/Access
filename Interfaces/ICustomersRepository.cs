using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources;
using Allprimetech.Interfaces.Resources.Customer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Allprimetech.Interfaces
{
    public interface ICustomersRepository
    {
        Task<List<Customer>> AllCustomers(string userId);
        Task<Customer> GetCustomer(int id);
        Task AddCustomer(Customer customer, string userId, string partner);
        void DeleteCustomer(Customer customer);
        Task<Customer> GetCustomerByName(string name);
        Task<IEnumerable<ReadCustomerResource>> SearchCustomers(CustomerSearchResource searchTerm, string userId);
        Task<SystemCodeResource> GetNewCustomerSystemCode();
        Task<Customer> GetCustomerBySystemCode(string systemCode);
        Task<Configuration> GetConfiguration();
        Task<LockingPlanInfoResource> GetCustomerLockingPlanInfo(int customerId);
        Task<Customer> GetCustomerByEmail(string name);
    }
}
