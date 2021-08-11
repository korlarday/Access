using Allprimetech.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class ValidateOrderResource
    {
        public int _OrderID { get; set; }
        public int _CustomerID { get; set; }
    }

    public class PickupResponse
    {
        public bool _Succeeded { get; set; }
        public string _Message { get; set; }
        public OrderAvailable _OrderAvailable { get; set; }
    }

    public class PickupResponseResource
    {
        public bool _Succeeded { get; set; }
        public string _Message { get; set; }
    }
}
