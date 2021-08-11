using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.ControllerCodes
{
    public class ProductionControllerStatus
    {
        public static PRODUCTION_ERROR _Error { get; set; }
    }

    public enum PRODUCTION_ERROR
    {
        NO_ERROR,
        BAD_REQUEST,
        EXISTS,
        NOT_FOUND,
        EXCEPTION
    }
}
