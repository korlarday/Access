using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources.Customer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Allprimetech.Interfaces.Resources.Partner
{
    public class ReadPartnerResource
    {
        public int PartnerID { get; set; }
        public string _Name { get; set; }
        public DateTime _CreationDate { get; set; }
        public DateTime _UpdatedDate { get; set; }
        public string _PartnerNumber { get; set; }
        public UserStatus _PartnerStatus { get; set; }
        public string _Email { get; set; }
        public ICollection<ReadCustomerResource> Customers { get; set; }
        public ReadPartnerResource()
        {
            Customers = new Collection<ReadCustomerResource>();
        }
    }
}
