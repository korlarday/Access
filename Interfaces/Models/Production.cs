using Allprimetech.Interfaces.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Models
{
    public class Production
    {
        [Key]
        public int ProductionID { get; set; }
        public DateTime _CreationDate { get; set; }
        public DateTime _UpdatedDate { get; set; }
        public ProductType _ProductType { get; set; }
        public ApplicationUser ByUser { get; set; }
        public string ByUserId { get; set; }
        public Order Order { get; set; }
        public int OrderID { get; set; }
        public ProductionStatus _Status { get; set; }
        public int _ProductID { get; set; } // could be Id of Cylinder or Key
        public int _Quantity { get; set; }
        public Production()
        {

        }
        public Production(int orderID, string userId, ProductType selectedType, int quantity)
        {
            _CreationDate = DateTime.UtcNow;
            _UpdatedDate = DateTime.UtcNow;
            OrderID = orderID;
            _ProductType = selectedType;
        }

    }

    
}
