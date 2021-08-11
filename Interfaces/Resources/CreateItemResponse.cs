using Allprimetech.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class CreateItemResponse
    {
        public bool _Succeeded { get; set; }
        public string _Message { get; set; }
        public object _NewItem { get; set; }
    }

    public class ProductionResponse
    {
        public bool _Succeeded { get; set; }
        public string _Message { get; set; }
        public ProductType _ProductType { get; set; }
        public int _ProductID { get; set; }
        public ReadCylinderResource _Cylinder { get; set; }
        public ReadGroupResource _Group { get; set; }
    }
}
