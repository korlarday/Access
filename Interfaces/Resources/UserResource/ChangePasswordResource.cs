using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class ChangePasswordResource
    {
        //[Required]
        //public string Id { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string _CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string _NewPassword { get; set; }


        [DataType(DataType.Password)]
        [Compare("_NewPassword", ErrorMessage = "Password and Confirm Password must match")]
        public string _ConfirmPassword { get; set; }
    }

    public class ResetPasswordResource
    {
        public string _UserId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string _Password { get; set; }


        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Confirm Password must match")]
        public string _ConfirmPassword { get; set; }

        public string _Token { get; set; }
    }

    public class ForgotPasswordResource
    {
        [Required]
        public string _Email { get; set; }
    }

}
