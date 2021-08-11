using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.ControllerCodes
{
    public class CustomerControllerStatus
    {
        public static CUSTOMER_ERROR _Error { get; set; }
    }

    public enum CUSTOMER_ERROR
    {
        NO_ERROR,
        BAD_REQUEST,
        EXISTS,
        NOT_FOUND,
        EXCEPTION
    }
}
