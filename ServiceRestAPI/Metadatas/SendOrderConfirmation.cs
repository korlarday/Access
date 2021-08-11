using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.ServiceRestAPI.Metadatas
{
    public class SendOrderConfirmation
    {
        public string _CustomerEmail { get; set; }
        public string _OrderNumber { get; set; }
        public string _CustomerName { get; set; }
        public int _CylinderQuantity { get; set; }
        public int _GroupQuantity { get; set; }
        public DateTime _DateValidated { get; set; }
    }
}
