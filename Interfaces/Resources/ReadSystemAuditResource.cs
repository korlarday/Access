using Allprimetech.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class ReadSystemAuditResource
    {
        public int SystemAuditID { get; set; }
        public string OperatorId { get; set; }
        public DateTime _Date { get; set; }
        public Operation _Operation { get; set; }
        public string _Description { get; set; }
        public Source _Source { get; set; }
    }
}
