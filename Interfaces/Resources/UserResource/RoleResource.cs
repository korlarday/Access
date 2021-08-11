using Allprimetech.Interfaces.Roles;
using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class RoleResource
    {
        public int ApplicationRoleID { get; set; }
        public double _RoleName { get; set; }
    }

    public class RoleDetailsResource : RolesDefinition
    {
        public int ApplicationRoleID { get; set; }
        public new string _Group { get; set; }
    }
}
