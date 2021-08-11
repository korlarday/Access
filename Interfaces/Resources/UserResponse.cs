using Allprimetech.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    
    public class UserResponse
    {
        public bool _IsSucceeded { get; set; }
        public string _Result { get; set; }
        public ApplicationUser _User { get; set; }
    }

    public class UserResponseResource
    {
        public string _Result { get; set; }
        public ReadUserResource _User { get; set; }
        public bool _Status { get; set; }
    }

    public class ChangeUserStatus
    {
        public string _UserNumber { get; set; }
        public UserStatus _Status { get; set; }
    }
}
