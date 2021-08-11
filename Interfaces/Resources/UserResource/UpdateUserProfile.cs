using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class UpdateUserProfile
    {
        [Required]
        public string _UserName { get; set; }
        //[Required]
        //public string Email { get; set; }
        [Required]
        public string _FirstName { get; set; }
        [Required]
        public string _LastName { get; set; }
        //public string Photo { get; set; }
        //public string PhotoName { get; set; }
    }
}
