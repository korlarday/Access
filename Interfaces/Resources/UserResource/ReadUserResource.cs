using Allprimetech.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class ReadUserResource
    {
        public string Id { get; set; }
        public string _UserName { get; set; }
        public string _Email { get; set; }
        public DateTime _DateRegistered { get; set; } 
        public DateTime? _LastLogin { get; set; }
        public string RegisteredBy { get; set; }
        public string _FirstName { get; set; }
        public string _LastName { get; set; }
        public int? PartnerID { get; set; }
        public string _Partner { get; set; }
        //public string PhotoPath { get; set; }
        public List<RoleResource> _Roles { get; set; }
        public List<int> _RoleIds { get; set; }
        public UserStatus _Status { get; set; }
        public string _DiscCodeListDirectory { get; set; }

        public ReadUserResource()
        {
            _RoleIds = new List<int>();
            _Roles = new List<RoleResource>();
        }

        public ReadUserResource(ApplicationUser user, bool isActive)
        {
            Id = user.Id;
            _UserName = user.UserName;
            _Email = user.Email;
            _DateRegistered = user._DateRegistered;
            _LastLogin = user._LastLogin;
            RegisteredBy = user.RegisteredById;
            _FirstName = user._FirstName;
            _LastName = user._LastName;
            PartnerID = user.PartnerID;
            //IsActive = isActive;
        }
    }

}
