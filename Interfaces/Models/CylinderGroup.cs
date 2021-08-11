using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Models
{
    public class CylinderGroup
    {
        [Key]
        public int CylinderGroupID { get; set; }
        public int CylinderID { get; set; }
        public int GroupID { get; set; }
        public int CustomerID { get; set; }

        public Customer Customer { get; set; }
        public Cylinder Cylinder { get; set; }
        public Group Group { get; set; }
    }
}
