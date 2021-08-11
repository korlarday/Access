using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class ReadOrderAvailable
    {
        public int OrderAvailableID { get; set; }
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime _CreationDate { get; set; }
    }
}
