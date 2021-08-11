using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class CylindersByGroupResource
    {
        public int _CylinderID { get; set; }
        public int _GroupID { get; set; }
        public int _CustomerID { get; set; }
        public string _DoorName { get; set; }
        public string _CylinderNumber { get; set; }
        public string _GroupName { get; set; }
        public string _GroupNumber { get; set; }
    }

    public class CylindersByGroup
    {
        public int _CylinderID { get; set; }
        public string _DoorName { get; set; }
        public string _CylinderNumber { get; set; }
    }

    public class CylindersInGroup
    {
        public int _Count { get; set; }
        public List<CylindersByGroup> _Cylinders { get; set; }
    }

    public class CylinderSortInfo
    {
        public int _CylinderID { get; set; }
        public string _DoorName { get; set; }
        public string _CylinderNumber { get; set; }
        public int _Count { get; set; }
    }

}
