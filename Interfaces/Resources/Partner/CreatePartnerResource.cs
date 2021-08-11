using Allprimetech.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Resources.Partner
{
    public class CreatePartnerResource
    {
        //public int Id { get; set; }
        [Required]
        public string _Name { get; set; }
        [Required]
        public string _PartnerNumber { get; set; }
        public string _Email { get; set; }
    }

    public class ChangePartnerStatusResource
    {
        [Required]
        public UserStatus _PartnerStatus { get; set; }
        [Required]
        public string _PartnerNumber { get; set; }
    }
}
