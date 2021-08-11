using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class KeyProductionResource
    {
        public int ProductionID { get; set; }
        public string _KeyName { get; set; }
        public string _KeyNumber { get; set; }
        public string _Status { get; set; }
        public DateTime _CreationDate { get; set; }
        public string _OrderNumber { get; set; }
    }
}
