using Allprimetech.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class KeyProduction
    {
        public int ProductionID { get; set; }
        public string _KeyName { get; set; }
        public string _KeyNumber { get; set; }
        public ProductionStatus _Status { get; set; }
        public DateTime _CreationDate { get; set; }
        public string _OrderNumber { get; set; }
        public string _CreatedBy { get; set; }
        public int _Quantity { get; set; }
    }
}
