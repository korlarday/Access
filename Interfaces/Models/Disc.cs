using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Models
{
    public class Disc
    {
        [Key]
        public int DiscID { get; set; }
        public string _Name { get; set; }
        public int _Number { get; set; }
        public int _Slot { get; set; }
        public DiscType _Type { get; set; }
        public Genre _Genre { get; set; }
        public Cylinder Cylinder { get; set; }
        public int CylinderID { get; set; }
        public Customer Customer { get; set; }
        public int CustomerID { get; set; }
        public string _Group { get; set; }
        public DateTime _CreationDate { get; set; }
        public DateTime _UpdatedDate { get; set; }
    }
}
