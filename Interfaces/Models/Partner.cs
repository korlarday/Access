using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Models
{
    public class Partner
    {
        [Key]
        public int PartnerID { get; set; }
        public string _Name { get; set; }
        public DateTime _CreationDate { get; set; }
        public DateTime _UpdatedDate { get; set; }
        public string _PartnerNumber { get; set; }
        public UserStatus _PartnerStatus { get; set; }
        public ICollection<Customer> _Customers { get; set; }
        public string _Email { get; set; }
        public Partner()
        {
            _Customers = new Collection<Customer>();
        }
    }
}
