using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Models
{
    public class ApplicationUserCustomer
    {
        public int ApplicationUserCustomerID { get; set; }
        public int CustomerID { get; set; }
        public string ApplicationUserID { get; set; }
        public Customer Customer { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
