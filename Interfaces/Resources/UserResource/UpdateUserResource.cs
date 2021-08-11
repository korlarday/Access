using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class UpdateUserResource
    {
        //[Required]
        //public string _UserName { get; set; }
        [Required]
        public string _Email { get; set; }
        [Required]
        public string _FirstName { get; set; }
        [Required]
        public string _LastName { get; set; }
        public ICollection<int> _Roles { get; set; }
        public List<string> _CustomerEmails { get; set; }
        public int _PartnerId { get; set; }
        public string _PartnerNumber { get; set; }
        public string _UserNumber { get; set; }
        public string _CountryName { get; set; }

        public UpdateUserResource()
        {
            _Roles = new Collection<int>();
            _CustomerEmails = new List<string>();
        }
    }
    public class VerifyUserEmailResource
    {
        public string _Email { get; set; }
    }
    public class ChangeUserPasswordResource
    {
        public string _Email { get; set; }
        public string _Password { get; set; }
    }
    public class UpdateDiscCodeListResource
    {
        public string _Email { get; set; }
        public string _NewDirectory { get; set; }
    }
    public class ReplaceEmail
    {
        public string _OldEmail { get; set; }
        public string _NewEmail { get; set; }
    }
}
