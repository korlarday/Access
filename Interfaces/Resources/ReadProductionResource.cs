using Allprimetech.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class ReadProductionResource
    {
        public int ProductionID { get; set; }
        public DateTime _CreationDate { get; set; }
        public DateTime _UpdatedDate { get; set; }
        public ProductType _ProductType { get; set; }
        public ReadUserResource ByUser { get; set; }
        public string ByUserId { get; set; }
        public ProductionStatus _Status { get; set; }
        public int _ProductID { get; set; } // could be Id of Cylinder or Key
        public int OrderID { get; set; }

    }
}
