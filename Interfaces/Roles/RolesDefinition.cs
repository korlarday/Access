using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Roles
{
    public class RolesDefinition
    {
        public string _Name { get; set; }
        public double _Value { get; set; }
        public RoleGroup _Group { get; set; }
        public double _ParentValue { get; set; }
    }

    public class Definition
    {
        public static RolesDefinition GetRoleValue(RolesEnum role)
        {
            switch (role)
            {
                case RolesEnum.AccountManagement:
                    return new RolesDefinition { _Name = "Account Management", _Value = 1, _Group = RoleGroup.Parent, _ParentValue = 1 };
                case RolesEnum.AccountCreate:
                    return new RolesDefinition { _Name = "Create", _Value = 1.1, _Group = RoleGroup.Child, _ParentValue = 1 };
                case RolesEnum.AccountRead:
                    return new RolesDefinition { _Name = "Read", _Value = 1.2, _Group = RoleGroup.Child, _ParentValue = 1 };
                case RolesEnum.AccountUpdate:
                    return new RolesDefinition { _Name = "Update", _Value = 1.3, _Group = RoleGroup.Child, _ParentValue = 1 };
                case RolesEnum.AccountDelete:
                    return new RolesDefinition { _Name = "Delete", _Value = 1.4, _Group = RoleGroup.Child, _ParentValue = 1 };

                case RolesEnum.CustomerManagement:
                    return new RolesDefinition { _Name = "Customer Management", _Value = 20, _Group = RoleGroup.Parent, _ParentValue = 20 };
                case RolesEnum.CustomerCreate:
                    return new RolesDefinition { _Name = "Create", _Value = 20.1, _Group = RoleGroup.Child, _ParentValue = 20 };
                case RolesEnum.CustomerRead:
                    return new RolesDefinition { _Name = "Read", _Value = 20.2, _Group = RoleGroup.Child, _ParentValue = 20 };
                case RolesEnum.CustomerUpdate:
                    return new RolesDefinition { _Name = "Update", _Value = 20.3, _Group = RoleGroup.Child, _ParentValue = 20 };
                case RolesEnum.CustomerDelete:
                    return new RolesDefinition { _Name = "Delete", _Value = 20.4, _Group = RoleGroup.Child, _ParentValue = 20 };

                case RolesEnum.OrderManagement:
                    return new RolesDefinition { _Name = "Order Management", _Value = 40, _Group = RoleGroup.Parent, _ParentValue = 40 };
                case RolesEnum.OrderCreate:
                    return new RolesDefinition { _Name = "Create", _Value = 40.1, _Group = RoleGroup.Child, _ParentValue = 40 };
                case RolesEnum.OrderRead:
                    return new RolesDefinition { _Name = "Read", _Value = 40.2, _Group = RoleGroup.Child, _ParentValue = 40 };
                case RolesEnum.OrderUpdate:
                    return new RolesDefinition { _Name = "Update", _Value = 40.3, _Group = RoleGroup.Child, _ParentValue = 40 };
                case RolesEnum.OrderDelete:
                    return new RolesDefinition { _Name = "Delete", _Value = 40.4, _Group = RoleGroup.Child, _ParentValue = 40 };

                case RolesEnum.PartnerManagement:
                    return new RolesDefinition { _Name = "Partner Management", _Value = 60, _Group = RoleGroup.Parent, _ParentValue = 60 };
                case RolesEnum.PartnerCreate:
                    return new RolesDefinition { _Name = "Create", _Value = 60.1, _Group = RoleGroup.Child, _ParentValue = 60 };
                case RolesEnum.PartnerRead:
                    return new RolesDefinition { _Name = "Read", _Value = 60.2, _Group = RoleGroup.Child, _ParentValue = 60 };
                case RolesEnum.PartnerUpdate:
                    return new RolesDefinition { _Name = "Update", _Value = 60.3, _Group = RoleGroup.Child, _ParentValue = 60 };
                case RolesEnum.PartnerDelete:
                    return new RolesDefinition { _Name = "Delete", _Value = 60.4, _Group = RoleGroup.Child, _ParentValue = 60 };

                case RolesEnum.CylinderManagement:
                    return new RolesDefinition { _Name = "Cylinder Management", _Value = 80, _Group = RoleGroup.Parent, _ParentValue = 80 };
                case RolesEnum.CylinderCreate:
                    return new RolesDefinition { _Name = "Create", _Value = 80.1, _Group = RoleGroup.Child, _ParentValue = 80 };
                case RolesEnum.CylinderRead:
                    return new RolesDefinition { _Name = "Read", _Value = 80.2, _Group = RoleGroup.Child, _ParentValue = 80 };
                case RolesEnum.CylinderUpdate:
                    return new RolesDefinition { _Name = "Update", _Value = 80.3, _Group = RoleGroup.Child, _ParentValue = 80 };
                case RolesEnum.CylinderDelete:
                    return new RolesDefinition { _Name = "Delete", _Value = 80.4, _Group = RoleGroup.Child, _ParentValue = 80 };

                case RolesEnum.GroupsManagement:
                    return new RolesDefinition { _Name = "Groups Management", _Value = 100, _Group = RoleGroup.Parent, _ParentValue = 100 };
                case RolesEnum.GroupsCreate:
                    return new RolesDefinition { _Name = "Create", _Value = 100.1, _Group = RoleGroup.Child, _ParentValue = 100 };
                case RolesEnum.GroupsRead:
                    return new RolesDefinition { _Name = "Read", _Value = 100.2, _Group = RoleGroup.Child, _ParentValue = 100 };
                case RolesEnum.GroupsUpdate:
                    return new RolesDefinition { _Name = "Update", _Value = 100.3, _Group = RoleGroup.Child, _ParentValue = 100 };
                case RolesEnum.GroupsDelete:
                    return new RolesDefinition { _Name = "Delete", _Value = 100.4, _Group = RoleGroup.Child, _ParentValue = 100 };

                case RolesEnum.SystemAuditManagement:
                    return new RolesDefinition { _Name = "SystemAudit Management", _Value = 120, _Group = RoleGroup.Parent, _ParentValue = 120 };
                case RolesEnum.SystemAuditCreate:
                    return new RolesDefinition { _Name = "Create", _Value = 120.1, _Group = RoleGroup.Child, _ParentValue = 120 };
                case RolesEnum.SystemAuditRead:
                    return new RolesDefinition { _Name = "Read", _Value = 120.2, _Group = RoleGroup.Child, _ParentValue = 120 };
                case RolesEnum.SystemAuditUpdate:
                    return new RolesDefinition { _Name = "Update", _Value = 120.3, _Group = RoleGroup.Child, _ParentValue = 120 };
                case RolesEnum.SystemAuditDelete:
                    return new RolesDefinition { _Name = "Delete", _Value = 120.4, _Group = RoleGroup.Child, _ParentValue = 120 };


                case RolesEnum.DiscManagement:
                    return new RolesDefinition { _Name = "Disc Management", _Value = 150, _Group = RoleGroup.Parent, _ParentValue = 150 };
                case RolesEnum.DiscCreate:
                    return new RolesDefinition { _Name = "Create", _Value = 150.1, _Group = RoleGroup.Child, _ParentValue = 150 };
                case RolesEnum.DiscRead:
                    return new RolesDefinition { _Name = "Read", _Value = 150.2, _Group = RoleGroup.Child, _ParentValue = 150 };
                case RolesEnum.DiscUpdate:
                    return new RolesDefinition { _Name = "Update", _Value = 150.3, _Group = RoleGroup.Child, _ParentValue = 150 };
                case RolesEnum.DiscDelete:
                    return new RolesDefinition { _Name = "Delete", _Value = 150.4, _Group = RoleGroup.Child, _ParentValue = 150 };

                case RolesEnum.ProductionManagement:
                    return new RolesDefinition { _Name = "Production Management", _Value = 170, _Group = RoleGroup.Parent, _ParentValue = 170 };
                case RolesEnum.ProductionCreate:
                    return new RolesDefinition { _Name = "Create", _Value = 170.1, _Group = RoleGroup.Child, _ParentValue = 170 };
                case RolesEnum.ProductionRead:
                    return new RolesDefinition { _Name = "Read", _Value = 170.2, _Group = RoleGroup.Child, _ParentValue = 170 };
                case RolesEnum.ProductionUpdate:
                    return new RolesDefinition { _Name = "Update", _Value = 170.3, _Group = RoleGroup.Child, _ParentValue = 170 };
                case RolesEnum.ProductionDelete:
                    return new RolesDefinition { _Name = "Delete", _Value = 170.4, _Group = RoleGroup.Child, _ParentValue = 170 };


                case RolesEnum.GroupsInfoManagement:
                    return new RolesDefinition { _Name = "GroupsInfo Management", _Value = 200, _Group = RoleGroup.Parent, _ParentValue = 200 };
                case RolesEnum.GroupsInfoCreate:
                    return new RolesDefinition { _Name = "Create", _Value = 200.1, _Group = RoleGroup.Child, _ParentValue = 200 };
                case RolesEnum.GroupsInfoRead:
                    return new RolesDefinition { _Name = "Read", _Value = 200.2, _Group = RoleGroup.Child, _ParentValue = 200 };
                case RolesEnum.GroupsInfoUpdate:
                    return new RolesDefinition { _Name = "Update", _Value = 200.3, _Group = RoleGroup.Child, _ParentValue = 200 };
                case RolesEnum.GroupsInfoDelete:
                    return new RolesDefinition { _Name = "Delete", _Value = 200.4, _Group = RoleGroup.Child, _ParentValue = 200 };

                default:
                    return new RolesDefinition { _Name = "", _Value = 0 };
            }
        }

    }

}
