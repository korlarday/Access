using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.ControllerCodes
{
    public class GroupInfoContrStatus
    {
        public static GROUPINFO_ERROR _Error { get; set; }
    }

    public enum GROUPINFO_ERROR
    {
        NO_ERROR,
        BAD_REQUEST,
        EXISTS,
        NOT_FOUND,
        EXCEPTION
    }
}
