using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class CreateUserResource
    {
        public CreateUserResource()
        {
            _Roles = new Collection<int>();
            _CustomerEmails = new List<string>();
        }
        [Required]
        public string _UserName { get; set; }
        [Required]
        public string _Email { get; set; }
        public string _Password { get; set; }
        [Required]
        public string _FirstName { get; set; }
        [Required]
        public string _LastName { get; set; }
        [Required]
        public string _CountryName { get; set; }
        public bool _IsDisabled { get; set; }
        public int _PartnerId { get; set; }
        public string _PartnerNumber { get; set; }
        public string _UserNumber { get; set; }
        public ICollection<int> _Roles { get; set; }
        public List<string> _CustomerEmails { get; set; }
    }
}
