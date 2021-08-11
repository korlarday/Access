using Allprimetech.Interfaces.Attributes;
using Allprimetech.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class UpdateProductionResource
    {
        public int _ProductionID { get; set; }
        [Required]
        public int _Produced { get; set; }
        [Required]
        public int _OrderID { get; set; }
    }

    public class CreateProductionResource
    {
        [Required]
        public int _Produced { get; set; }

        [Required]
        public int _OrderID { get; set; }

        [Required]
        [IntRange(AllowableValues = new int[] { 1, 2 }, ErrorMessage = "Invalid status value supplied.")]
        public int _ProductType { get; set; } 


        [Required]
        [IntRange(AllowableValues = new int[] { 1, 2, 3, 4, 5 }, ErrorMessage = "Invalid status value supplied.")]
        public int _Status { get; set; }

        [Required]
        public int _ProductID { get; set; } // could be Id of Cylinder or Key

    }
}
