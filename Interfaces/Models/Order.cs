using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Models
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }
        public Customer Customer { get; set; }
        public int CustomerID { get; set; }
        public int _GroupKeyQty { get; set; }
        public int _CylinderQty { get; set; }
        public DateTime _CreationDate { get; set; }
        public DateTime _UpdatedDate { get; set; }
        public string _OrderNumber { get; set; }
        public string _ProjectName { get; set; }
        public string _Description { get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public Status _Status { get; set; }
        public ICollection<OrderValidation> _OrderValidations { get; set; }
        public ICollection<OrderAvailable> _OrderAvailables { get; set; }
        public ICollection<OrderDetail> _OrderDetails { get; set; }
        public ICollection<Cylinder> _Cylinders { get; set; }
        public ICollection<Group> _Groups { get; set; }
        public Order()
        {
            _OrderDetails = new Collection<OrderDetail>();
            _Cylinders = new Collection<Cylinder>();
            _Groups = new Collection<Group>();
            _OrderAvailables = new Collection<OrderAvailable>();
            _OrderValidations = new Collection<OrderValidation>();
        }
    }

    
}
