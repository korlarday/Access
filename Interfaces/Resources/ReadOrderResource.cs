using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources.Customer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class ReadOrderResource
    {
        public int OrderID { get; set; }
        //public ReadCustomerResource Customer { get; set; }
        public int CustomerID { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public int _GroupKeyQty { get; set; }
        public int _CylinderQty { get; set; }
        public DateTime _CreationDate { get; set; }
        public DateTime _UpdatedDate { get; set; }
        public string _OrderNumber { get; set; }
        public string _ProjectName { get; set; } 
        public string _Description { get; set; }
        public int _Status { get; set; }
        public string _SystemCode { get; set; }
        public string _CustomerName { get; set; }
        public bool _OrderValidated { get; set; }
        public bool _OrderReady { get; set; }
        public ICollection<ReadOrderDetailResource> _OrderDetails { get; set; }
        public ReadOrderResource()
        {
            _OrderDetails = new Collection<ReadOrderDetailResource>();
        }

        public ReadOrderResource(Order order)
        {
            OrderID = order.OrderID;
            CustomerID = order.CustomerID;
            _CylinderQty = order._CylinderQty;
            _GroupKeyQty = order._GroupKeyQty;
            _CreationDate = order._CreationDate;
            _UpdatedDate = order._UpdatedDate;
            _OrderNumber = order._OrderNumber;
            _ProjectName = order._ProjectName;
            _Description = order._Description;
            CreatedById = order.CreatedById;
        }
    }
}
