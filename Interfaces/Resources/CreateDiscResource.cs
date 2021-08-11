using Allprimetech.Interfaces.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class CreateDiscResource
    {
        public int DiscID { get; set; }
        [Required]
        public string _Name { get; set; }
        public int _Number { get; set; }
        public int _Slot { get; set; }
        [Required]
        [IntRange(AllowableValues = new int[] { 1, 2 }, ErrorMessage = "Invalid status value supplied.")]
        public int _Type { get; set; }
        [Required]
        [IntRange(AllowableValues = new int[] { 1, 2, 3, 4 }, ErrorMessage = "Invalid status value supplied.")]
        public int _Genre { get; set; }
        //public Cylinder Cylinder { get; set; }
        public int _CylinderID { get; set; }
        public int _CustomerID { get; set; }
        public string _Group { get; set; }
    }
}
