using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class CreateSystemAuditResource
    {
        public int SystemAuditID { get; set; } 
        public string _Operation { get; set; }
        public string _Description { get; set; }
        public string _Action { get; set; }
    }
}
