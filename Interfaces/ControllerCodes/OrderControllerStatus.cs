using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.ControllerCodes
{
    public class OrderControllerStatus
    {
        public static ORDER_ERROR _Error { get; set; }
    }

    public enum ORDER_ERROR
    {
        NO_ERROR,
        BAD_REQUEST,
        EXISTS,
        NOT_FOUND,
        EXCEPTION
    }
}
