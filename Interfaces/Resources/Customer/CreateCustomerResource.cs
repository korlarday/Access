using Allprimetech.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Resources.Customer
{
    public class CreateCustomerResource
    {
        public int CustomerID { get; set; }
        public string _Partner { get; set; }
        [Required]
        public string _Name { get; set; }
        [Required]
        public string _ContactPerson { get; set; }
        [Required]
        public string _CustomerNumber { get; set; }
        [Required]
        public string _SystemCode { get; set; }
        [Required]
        public string _Email { get; set; }
    }

    public class CreateCustomerAdminResource
    {
        public string _PartnerNumber { get; set; }
        [Required]
        public string _Name { get; set; }
        public string _ContactPerson { get; set; }
        [Required]
        public string _CustomerNumber { get; set; }
        [Required]
        public string _SystemCode { get; set; }
        [Required]
        public string _Email { get; set; }
    }


    public class VerifyCustomerEmail
    {
        [Required]
        public string _Email { get; set; }
    }

    public class ChangeCustomerStatus
    {
        [Required]
        public string _Email { get; set; }
        [Required]
        public UserStatus _Status { get; set; }
    }
}
