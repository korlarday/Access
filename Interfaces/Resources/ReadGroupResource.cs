using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources.Customer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class ReadGroupResource
    {
        public int GroupID { get; set; }
        public DateTime _CreationDate { get; set; }
        public DateTime _UpdatedDate { get; set; }
        public ReadOrderResource Order { get; set; }
        public int OrderID { get; set; }
        public ReadCustomerResource Customer { get; set; }
        public int CustomerID { get; set; }
        public string _KeyName { get; set; }
        public int _Quantity { get; set; }
        public string _KeyNumber { get; set; }
        public int _Produced { get; set; }
        public int _Validated { get; set; }
        public int _PositionId { get; set; }
        public int _Summary { get; set; }
        public int _Reclaimed { get; set; }
        public int _Blocked { get; set; }
        public string _GroupNumber { get; set; }
        public ModificationStatus _ModificationStatus { get; set; }
    }
}
