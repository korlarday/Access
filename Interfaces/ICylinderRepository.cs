using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Allprimetech.Interfaces
{
    public interface ICylinderRepository
    {
        Task<List<Cylinder>> AllCylinders(int customerId);
        Task<Cylinder> GetCylinder(int id);
        Task AddCylinder(Cylinder cylinder, string userId);
        void DeleteCylinder(Cylinder cylinder);
        Task<IEnumerable<Cylinder>> SearchCylinders(CylinderSearchResource searchTerm);
        Task<Customer> GetCustomerByName(string customer);
        Task<Order> GetOrderByName(string order);
        Task<Order> GetOrderByOrderNumber(string orderNumber);
        Task<List<Cylinder>> GetCylindersByOrderId(int orderID);
        Task<Order> GetOrderByNumber(string order);
        Task<int> GetCylinderBatchIdByCustomerName(string customerName);
        Task<int> GetLastBatchNumber(string customerName);
        Task<bool> IsCylinderExistsWithCustomer(int customerId);
        Task AddCylinderInBulk(string insertQuery);
        Task<List<ReadCylinderResource>> ModifyCylinders(List<CreateCylinderResource> cylinders, int customerId, string userId);
        Task<List<Cylinder>> RetrieveStoredCylinders(int customerId);
        Task<Customer> GetCustomerById(int customerID);
    }
}
