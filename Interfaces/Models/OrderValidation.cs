using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Models
{
    public class OrderValidation
    {
        public int OrderValidationID { get; set; }
        public int OrderID { get; set; }
        public Order Order { get; set; }
        public int CustomerID { get; set; }
        public Customer Customer { get; set; }
        public string ValidatedByID { get; set; }
        public ApplicationUser ValidatedBy { get; set; }
        public DateTime _DateValidated { get; set; }
    }
}
