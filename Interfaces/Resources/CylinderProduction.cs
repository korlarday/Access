using Allprimetech.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class CylinderProduction
    {
        public int ProductionID { get; set; }
        public string _DoorName { get; set; }
        public string _CylinderNumber { get; set; }
        public ProductionStatus _Status { get; set; }
        public DateTime _CreationDate { get; set; }
        public string _OrderNumber { get; set; }
        public string _LengthOutside { get; set; }
        public string _LengthInside { get; set; }
        public string _Color { get; set; }
        public Options _Options { get; set; }
        public string _CreatedBy { get; set; }
        public int _Quantity { get; set; }
    }

}
