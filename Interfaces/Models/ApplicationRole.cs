using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Models
{
    public class ApplicationRole
    {
        [Key]
        public int ApplicationRoleID { get; set; }
        public double _RoleName { get; set; }

        public ICollection<ApplicationUserRole> _Users { get; set; }
        public ApplicationRole()
        {
            _Users = new Collection<ApplicationUserRole>();
        }
    }
}
