using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class LoginResource
    {
        [Required]
        public string _UserName { get; set; }
        [Required]
        public string _Password { get; set; }

        //public string Platform { get; set; }
        //public string City { get; set; }
        //public string Country { get; set; }
        //public string IpAddress { get; set; }
        //public string Browser { get; set; }
        //public string BrowserVersion { get; set; }
        //public string OperatingSystem { get; set; }
        //public string TimeZone { get; set; }
        //public string ApplicationUserId { get; set; }
    }

    public class LoginResponse
    {
        public bool _Succeeded { get; set; }
        public bool _EmailConfirmed { get; set; }
        public string _Message { get; set; }
        public string _Token { get; set; }
        public string _UserId { get; set; }
        public List<string> _UserRoles { get; set; }
        public ReadUserResource _User { get; set; }
        public LoginResponse()
        {

        }
        public LoginResponse(ConfirmEmailResource result)
        {
            _Succeeded = result._Succeeded;
            _Message = result._Message;
            _Token = null;
            _EmailConfirmed = false;
        }
    }
}
