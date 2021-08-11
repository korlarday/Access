using Allprimetech.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class ReadDiscResource
    {
        public int DiscID { get; set; }
        public string _Name { get; set; }
        public int _Number { get; set; }
        public int _Slot { get; set; }
        public DiscType _Type { get; set; }
        public Genre _Genre { get; set; }
        //public ReadCylinderResource Cylinder { get; set; }
        public int CylinderID { get; set; }
        public int CustomerID { get; set; }
        public string _Group { get; set; }
    }
}
