using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Models
{
    public class KeyGroupCylinderDetail
    {
        public int KeyGroupCylinderDetailID { get; set; }
        public int GroupID { get; set; }
        public string _Cylinders { get; set; }
        public int CustomerID { get; set; }
        public Customer Customer { get; set; }
    }
}
