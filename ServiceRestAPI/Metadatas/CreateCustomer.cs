using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.ServiceRestAPI.Metadatas
{
    public class CreateCustomer
    {
        public int Id { get; set; }
        public string CompanyAddress { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string CustomerCode { get; set; }
        public string PartnerCode { get; set; }
        public string Email { get; set; }
    }
}
