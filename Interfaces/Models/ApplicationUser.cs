using Allprimetech.Interfaces.Resources;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Allprimetech.Interfaces.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string _FirstName { get; set; }
        public string _LastName { get; set; }
        public DateTime _DateRegistered { get; set; }
        public DateTime? _LastLogin { get; set; }
        public string RegisteredById { get; set; }
        public ApplicationUser RegisteredBy { get; set; }
        public int? PartnerID { get; set; }
        public Partner Partner { get; set; }
        public ICollection<ApplicationUserRole> _Roles { get; set; }
        public ICollection<ApplicationUserCustomer> _Customers { get; set; }
        public DateTime _UpdatedDate { get; set; }
        public UserStatus _Status { get; set; }
        public string _UserNumber { get; set; }
        public string _DiscCodeListDirectory { get; set; }
        public int CountryID { get; set; }
        public Country Country { get; set; }


        public ApplicationUser()
        {
            _Roles = new Collection<ApplicationUserRole>();
            _Customers = new Collection<ApplicationUserCustomer>();
        }
        public ApplicationUser(CreateUserResource model)
        {
            UserName = model._UserName;
            Email = model._Email;
            _FirstName = model._FirstName;
            _LastName = model._LastName;
            _DateRegistered = DateTime.UtcNow;
            _LastLogin = null;
            if(model._PartnerId == 0)
            {
                PartnerID = null;
            }else
            {
                PartnerID = model._PartnerId;
            }
            _UserNumber = model._UserNumber;
            _Status = UserStatus.Activated;
            _DiscCodeListDirectory = "C:\\";
        }

        public void UpdateUserInfo(UpdateUserResource model)
        {
            //UserName = model._UserName;
            Email = model._Email;
            NormalizedEmail = model._Email.ToUpper();
            _FirstName = model._FirstName;
            _LastName = model._LastName;
            if (model._PartnerId == 0)
            {
                PartnerID = null;
            }
            else
            {
                PartnerID = model._PartnerId;
            }
        }

        //public void UpdateUserProfile(UpdateUserProfile model)
        //{
        //    UserName = model.UserName;
        //    FirstName = model.FirstName;
        //    LastName = model.LastName;
        //}

        

        public void UpdateUserProfile(UpdateUserProfile model)
        {
            UserName = model._UserName;
            _FirstName = model._FirstName;
            _LastName = model._LastName;
        }
        //public ICollection<UserAccessInfo> UserAccessInfos { get; set; }
        //public ICollection<PageVisited> PagesVisited { get; set; }
    }
}
