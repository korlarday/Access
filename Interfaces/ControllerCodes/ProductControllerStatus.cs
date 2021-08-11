using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.ControllerCodes
{
    public class ProductControllerStatus
    {
        public static PRODUCT_ERROR _Error { get; set; }
    }

    public enum PRODUCT_ERROR
    {
        NO_ERROR,
        BAD_REQUEST,
        EXISTS,
        NOT_FOUND,
        EXCEPTION
    }
}
