using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Models
{
    public class Country
    {
        public int CountryID { get; set; }
        [Required]
        [MaxLength(255)]
        public string _CountryName { get; set; }
        [Required]
        [MaxLength(255)]
        public string _TimeZoneName { get; set; }
        public string _TimeZoneDescription { get; set; }
        [MaxLength(255)]
        public string _UTCOffSet { get; set; }
    }
}
