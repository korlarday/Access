using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Allprimetech.Interfaces
{
    public interface IOrdersRepository
    {
        Task<List<Order>> AllOrders(int customerId);
        Task<Order> GetOrder(int id);
        Task<bool> AddOrder(Order order, string customerName);
        void DeleteOrder(Order order);
        Task<List<OrderDetail>> AllOrderDetails(int customerId);
        Task<List<OrderDetail>> GetOrderDetailsForAnOrder(int id);
        Task<OrderDetail> GetSingleOrderDetail(int orderDetailId);
        Task<OrderDetail> AddOrderDetail(CreateOrderDetailResource orderDetail, string userId);
        void DeleteOrderDetail(OrderDetail orderDetail);

        Task<List<ReadOrderResource>> SearchOrders(OrderSearchResource searchTerm);
        Task<Order> GetOrderByOrderNumber(string orderNumber);
        Task<CreateItemResponse> ModifyOrder(Order order, ModifyOrderResource model, string userId);
        Task<CreateItemResponse> AddOrderDetailInBulk(CreateOrderDetailResource model, string userId);
        Task<List<ReadOrderDetailResource>> SearchOrderDetail(OrderDetailSearchResource searchTerm);
        Task<OrderValidation> ValidatedOrder(ValidateOrderResource model, string userId);
        Task<OrderAvailable> OrderAvailable(ValidateOrderResource model, string userId);
        Task<PickupResponse> OrderAvailableV1(ValidateOrderResource model, string userId);
        Task<OrderAvailable> GetPickUpDetail(int customerId);
    }
}
