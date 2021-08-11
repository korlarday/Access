using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class TransactionStatus
    {
        public int _ErrorCode { get; set; }
        public string _Message { get; set; }
        public object _Response { get; set; }
    }
}
