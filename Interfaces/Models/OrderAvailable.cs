using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Models
{
    public class OrderAvailable
    {
        public int OrderAvailableID { get; set; }
        public int OrderID { get; set; }
        public Order Order { get; set; }
        public int CustomerID { get; set; }
        public Customer Customer { get; set; }
        public string CreatedByID { get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public DateTime _CreationDate { get; set; }
    }
}
