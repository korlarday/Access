using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Roles
{
    public enum RolesEnum
    {
        AccountManagement = 1,
        AccountCreate,
        AccountRead,
        AccountUpdate,
        AccountDelete,
        CustomerManagement,
        CustomerCreate,
        CustomerRead,
        CustomerUpdate,
        CustomerDelete,
        OrderManagement,
        OrderCreate,
        OrderRead,
        OrderUpdate,
        OrderDelete,
        PartnerManagement,
        PartnerCreate,
        PartnerRead,
        PartnerUpdate,
        PartnerDelete,
        CylinderManagement,
        CylinderCreate,
        CylinderRead,
        CylinderUpdate,
        CylinderDelete,
        GroupsManagement,
        GroupsCreate,
        GroupsRead,
        GroupsUpdate,
        GroupsDelete,
        SystemAuditManagement,
        SystemAuditCreate,
        SystemAuditRead,
        SystemAuditUpdate,
        SystemAuditDelete,
        DiscManagement,
        DiscCreate,
        DiscRead,
        DiscUpdate,
        DiscDelete,
        GroupsInfoManagement,
        GroupsInfoCreate,
        GroupsInfoRead,
        GroupsInfoUpdate,
        GroupsInfoDelete,
        ProductionManagement,
        ProductionCreate,
        ProductionRead,
        ProductionUpdate,
        ProductionDelete
    }

    public enum RoleGroup
    {
        Parent,
        Child,
        Isolated
    }
}
