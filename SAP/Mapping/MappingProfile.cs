using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources;
using Allprimetech.Interfaces.Resources.Customer;
using Allprimetech.Interfaces.Resources.Partner;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAP_Lite_API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // From Domain object to Resource

            CreateMap<ApplicationUser, ReadUserResource>()
                .ForMember(p => p._Roles, opt => opt.MapFrom(pr => pr._Roles.Select(x => x.ApplicationRole)))
                .ForMember(p => p._RoleIds, opt => opt.MapFrom(pr => pr._Roles.Select(x => x.ApplicationRole.ApplicationRoleID)))
                .ForMember(p => p._Partner, opt => opt.MapFrom(pr => pr.Partner != null ? pr.Partner._Name : ""));

            //CreateMap<Customer, ReadCustomerResource>()
            //    .ForMember(c => c.Products, opt => opt.MapFrom(cr => cr.Products.Select(x => x.Product)));
            CreateMap<Partner, ReadPartnerResource>();

            CreateMap<Disc, ReadDiscResource>();
            CreateMap<GroupsInfo, ReadGroupsInfoResource>();

            CreateMap<Cylinder, ReadCylinderResource>()
                .ForMember(c => c._Summary, opt => opt.MapFrom(pr => (pr._Quantity - pr._Assembled)));

            CreateMap<Group, ReadGroupResource>()
                .ForMember(c => c._KeyName, opt => opt.MapFrom(pr => pr._Name))
                .ForMember(c => c._Summary, opt => opt.MapFrom(pr => (pr._Quantity - pr._Produced)));


            CreateMap<Order, ReadOrderResource>()
                .ForMember(c => c.CreatedBy, opt => opt.MapFrom(pr => pr.CreatedBy != null ? pr.CreatedBy._FirstName + " " + pr.CreatedBy._FirstName : ""))
                .ForMember(c => c._SystemCode, opt => opt.MapFrom(pr => pr.Customer != null ? pr.Customer._SystemCode : ""))
                .ForMember(c => c._CustomerName, opt => opt.MapFrom(pr => pr.Customer != null ? pr.Customer._Name : ""))
                .ForMember(c => c._OrderValidated, opt => opt.MapFrom(pr => pr._OrderValidations != null && pr._OrderValidations.Count() > 0 ? true : false))
                .ForMember(c => c._OrderReady, opt => opt.MapFrom(pr => pr._OrderAvailables != null && pr._OrderAvailables.Count() > 0 ? true : false));

            CreateMap<Group, CylinderGroupInfo>()
                .ForMember(c => c._GroupID, opt => opt.MapFrom(pr => pr.GroupID))
                .ForMember(c => c._GroupName, opt => opt.MapFrom(pr => pr._Name))
                .ForMember(c => c._GroupNumber, opt => opt.MapFrom(pr => pr._GroupNumber));


            CreateMap<GroupsInfo, ReadGroupInfoCodeList>()
                .ForMember(c => c._Customer, opt => opt.MapFrom(pr => pr.Customer != null ? pr.Customer._Name : ""))
                .ForMember(c => c._SystemCode, opt => opt.MapFrom(pr => pr.Customer != null ? pr.Customer._SystemCode : ""))
                .ForMember(c => c._Group, opt => opt.MapFrom(pr => pr.Group != null ? pr.Group._Name : ""))
                .ForMember(c => c._InstallationCode, opt => opt.MapFrom(pr => pr.Customer != null ? pr.Customer._InstallationCode : ""))
                .ForMember(c => c._GroupNumber, opt => opt.MapFrom(pr => pr.Group != null ? pr.Group._GroupNumber : ""))
                .ForMember(c => c._Quantity, opt => opt.MapFrom(pr => pr.Group != null ? pr.Group._Quantity : 0));


            CreateMap<CylinderProduction, CylinderProductionResource>()
                .ForMember(c => c._Description, opt => opt.MapFrom(pr => pr._Options.ToString() + " " + pr._LengthInside + " " + pr._Color.ToString()));

            CreateMap<KeyProduction, KeyProductionResource>();
                //.ForMember(c => c._Status, opt => opt.MapFrom(pr => pr._Status));


            CreateMap<OrderDetail, ReadOrderDetailResource>()
                .ForMember(c => c._ProjectName, opt => opt.MapFrom(pr => pr.Order != null ? pr.Order._ProjectName : ""))
                .ForMember(c => c._OrderNumber, opt => opt.MapFrom(pr => pr.Order != null ? pr.Order._OrderNumber : ""))
                .ForMember(c => c._CreatedBy, opt => opt.MapFrom(pr => pr.ByPerson != null ? pr.ByPerson._FirstName + " " + pr.ByPerson._LastName : ""));

            CreateMap<Customer, ReadCustomerResource>()
                .ForMember(c => c._Partner, opt => opt.MapFrom(pr => pr.Partner != null ? pr.Partner._Name : ""))
                .ForMember(c => c.CreatedBy, opt => opt.MapFrom(pr => pr.CreatedBy != null ? pr.CreatedBy._FirstName + " " + pr.CreatedBy._LastName : ""))
                .ForMember(c => c._OrderList, opt => opt.MapFrom(pr => pr._Orders))
                .ForMember(c => c._Orders, opt => opt.MapFrom(pr => pr._Orders.Count()));

            CreateMap<ApplicationRole, RoleResource>();
            CreateMap<UserResponse, UserResponseResource>();
            CreateMap<SystemAudit, ReadSystemAuditResource>();
            CreateMap<Production, ReadProductionResource>();
            CreateMap<OrderAvailable, ReadOrderAvailable>()
                .ForMember(c => c.CreatedBy, opt => opt.MapFrom(pr => pr.CreatedBy != null ? pr.CreatedBy._FirstName + " " + pr.CreatedBy._LastName : ""));


            //// From Resource to Domain object
            CreateMap<CreateOrderDetailResource, OrderDetail>()
                .ForMember(p => p.OrderDetailID, opt => opt.Ignore());

            CreateMap<CreateCylinderResource, Cylinder>()
                .ForMember(p => p.CylinderID, opt => opt.Ignore());

            CreateMap<CreateDiscResource, Disc>()
                .ForMember(p => p.DiscID, opt => opt.Ignore())
                .ForMember(x => x.CylinderID, opt => opt.MapFrom(x => x._CylinderID))
                .ForMember(x => x.CustomerID, opt => opt.MapFrom(x => x._CustomerID));


            CreateMap<CreateGroupResource, Group>()
                .ForMember(p => p.GroupID, opt => opt.Ignore())
                .ForMember(x => x._Name, opt => opt.MapFrom(x => x._KeyName));

            CreateMap<CreateOrderResource, Order>()
                .ForMember(p => p.OrderID, opt => opt.Ignore())
                .ForMember(x => x._CylinderQty, opt => opt.MapFrom(x => x._CylinderQuantity))
                .ForMember(x => x._GroupKeyQty, opt => opt.MapFrom(x => x._KeyQuantity));

            CreateMap<ModifyOrderResource, Order>()
                .ForMember(p => p.OrderID, opt => opt.Ignore())
                .ForMember(x => x._CylinderQty, opt => opt.MapFrom(x => x._CylinderQuantity))
                .ForMember(x => x._GroupKeyQty, opt => opt.MapFrom(x => x._KeyQuantity));

            //CreateMap<UpdateOrderResource, Order>()
            //    .ForMember(p => p.OrderID, opt => opt.Ignore())
            //    .ForMember(x => x.CustomerID, opt => opt.MapFrom(x => x._CustomerID));

            CreateMap<CreateCustomerResource, Customer>()
                .ForMember(p => p.CustomerID, opt => opt.Ignore());
            CreateMap<CreateGroupsInfoResource, GroupsInfo>()
                .ForMember(p => p.GroupsInfoID, opt => opt.Ignore());
            CreateMap<CreateGroupsInfoResource, GroupInfoVerification>()
                .ForMember(p => p.GroupInfoVerificationID, opt => opt.Ignore());


            CreateMap<CreatePartnerResource, Partner>()
                .ForMember(p => p.PartnerID, opt => opt.Ignore());

            CreateMap<CreateSystemAuditResource, SystemAudit>()
                .ForMember(p => p.SystemAuditID, opt => opt.Ignore());

            CreateMap<CreateProductionResource, Production>()
                .ForMember(x => x.OrderID, opt => opt.MapFrom(x => x._OrderID))
                .ForMember(x => x._Quantity, opt => opt.MapFrom(x => x._Produced));
        }

    }

}
