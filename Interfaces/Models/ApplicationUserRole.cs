using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Allprimetech.Interfaces.Models
{
    public class ApplicationUserRole
    {
        //[Key]
        //public int ApplicationUserRoleID { get; set; }
        //[ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        //[ForeignKey("ApplicationRole")]
        public int ApplicationRoleID { get; set; }
        
        public ApplicationUser ApplicationUser { get; set; }
        public ApplicationRole ApplicationRole { get; set; }
    }
}
