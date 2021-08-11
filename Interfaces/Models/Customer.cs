using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Models
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; } 
        public Partner Partner { get; set; } 
        public int? PartnerID { get; set; }
        public string _SystemCode { get; set; } 
        public string _InstallationCode { get; set; }
        public string _Name { get; set; }
        public string _ContactPerson { get; set; }
        public DateTime _CreationDate { get; set; }
        public DateTime _UpdatedDate { get; set; }
        public string _CustomerNumber { get; set; }
        public string CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public string _Email { get; set; }
        public bool _EmailVerified { get; set; }
        public UserStatus _CustomerStatus { get; set; }
        public bool _IsDeleted { get; set; }
        public ICollection<Order> _Orders { get; set; }
        public Customer()
        {
            _Orders = new Collection<Order>();
        }
    }
}
