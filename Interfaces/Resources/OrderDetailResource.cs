using Allprimetech.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class OrderDetailResource
    {
        public int OrderDetailID { get; set; }
        public ProductType _ProductType { get; set; }
        public DateTime _Date { get; set; }
        public ReadUserResource ByPerson { get; set; }
        public int ByPersonID { get; set; }
        public int OrderID { get; set; }
    }
}
