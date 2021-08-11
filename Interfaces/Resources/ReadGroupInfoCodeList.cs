using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class ReadGroupInfoCodeList
    {
        public int GroupsInfoID { get; set; }
        public int _Slot { get; set; }
        public int _Row { get; set; }
        public int _Value { get; set; }
        public string _Customer { get; set; }
        public string _SystemCode { get; set; }
        public string _InstallationCode { get; set; }
        public int CustomerID { get; set; }
        public string _Group { get; set; }
        public string _GroupNumber { get; set; }
        public int _Quantity { get; set; }
        public int GroupID { get; set; }
    }

    public class ReadDiscWithOccurrence
    {
        public int DiscID { get; set; }
        public string _Name { get; set; }
        public int _Number { get; set; }
        public int _Slot { get; set; }
        public int _Type { get; set; }
        public int CylinderID { get; set; }
        public int _Occurrences { get; set; }
        public string _SystemCode { get; set; }
        public string _InstallationCode { get; set; }
        public int _Quantity { get; set; }
    }
}
