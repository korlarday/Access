using Allprimetech.Interfaces.Resources.Partner;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Allprimetech.Interfaces.Resources.Customer
{
    public class ReadCustomerResource
    {
        public int CustomerID { get; set; }
        public string _Partner { get; set; }
        public int? PartnerID { get; set; }
        public string _SystemCode { get; set; }
        public string _InstallationCode { get; set; }
        public string _Name { get; set; }
        public string _ContactPerson { get; set; }
        public DateTime _CreationDate { get; set; }
        public DateTime _UpdatedDate { get; set; }
        public string _CustomerNumber { get; set; }
        public string CreatedById { get; set; }
        public string CreatedBy { get; set; } 
        public int _Orders { get; set; }
        public string _Email { get; set; }
        public ICollection<ReadOrderResource> _OrderList { get; set; }
        public ReadCustomerResource()
        {
            _OrderList = new Collection<ReadOrderResource>();
        }
    }

    public class LockingPlanInfoResource
    {
        public int _CylinderCount { get; set; }
        public int _GroupCount { get; set; }
        public int _CustomerID { get; set; }
    }
}
