using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources.Customer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class ReadCylinderResource
    {
        public int CylinderID { get; set; }
        public string _DoorName { get; set; }
        public ReadOrderResource Order { get; set; }
        public int OrderID { get; set; }
        public ReadCustomerResource Customer { get; set; }
        public int CustomerID { get; set; }
        public DateTime _CreationDate { get; set; }
        public DateTime _UpdatedDate { get; set; }
        public string _ArticleNumber { get; set; }
        public string _LengthOutside { get; set; }
        public string _LengthInside { get; set; }
        public string _Color { get; set; }
        public int _Options { get; set; }
        public int _Quantity { get; set; }
        public int _QrCodeIssued { get; set; }
        public int _Assembled { get; set; }
        public int _Blocked { get; set; }
        public int _Validated { get; set; }
        public int _PositionId { get; set; }
        public int _Summary { get; set; }
        public string _CylinderNumber { get; set; }
        public int _Reclaimed { get; set; }
        public ModificationStatus _ModificationStatus { get; set; }
    }
}
