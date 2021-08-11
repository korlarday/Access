using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class ReadDiscInfo
    {
        public int DiscID { get; set; }
        public string _Name { get; set; }
        public int _Number { get; set; }
        public int _Slot { get; set; }
        public int _Type { get; set; }
        public int CylinderID { get; set; }
        public string _DoorName { get; set; }
        public int CustomerID { get; set; }
        public int _Quantity { get; set; }
        public string _LengthInside { get; set; }
        public string _LengthOutside { get; set; }
        public string _Color { get; set; }
        public int _Options { get; set; }
        public string _SystemCode { get; set; }
        public string _InstallationCode { get; set; }
        public string _CylinderNumber { get; set; }
        public string _GroupNumbers { get; set; }
    }
}
