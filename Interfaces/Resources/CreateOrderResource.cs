using Allprimetech.Interfaces.Attributes;
using Allprimetech.Interfaces.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class CreateOrderResource
    {
        
        public int _OrderID { get; set; }
        [Required]
        public string _Customer { get; set; }
        public int _KeyQuantity { get; set; }
        public int _CylinderQuantity { get; set; }
        [Required]
        public string _OrderNumber { get; set; }
        [Required]
        public string _ProjectName { get; set; }
        public string _Description { get; set; }

        //[Required]
        //[IntRange(AllowableValues = new int[] { 1, 2 }, ErrorMessage = "Invalid status value supplied.")]
        //public int _ProductType { get; set; }

        //[Required]
        //public int _ProductID { get; set; }

        //[Required]
        //[IntRange(AllowableValues = new int[] { 1, 2, 3 }, ErrorMessage = "Invalid status value supplied.")]
        //public int _Status { get; set; }
    }

    public class ModifyOrderResource
    {
        [Required]
        public string _Customer { get; set; } 
        public int _KeyQuantity { get; set; }
        public int _CylinderQuantity { get; set; }
        [Required]
        public string _OrderNumber { get; set; }
        [Required]
        public string _ProjectName { get; set; }
        public string _Description { get; set; }
        public int _TotalKeys { get; set; }
        public int _TotalCylinders { get; set; }
        public int _CustomerID { get; set; }
        public int _OrderID { get; set; }
    }

    public class UpdateOrderResource
    {
        public int _OrderID { get; set; }
        [Required]
        public int _CustomerID { get; set; }
        public int _KeyQuantity { get; set; }
        public int _CylinderQuantity { get; set; }
        [Required]
        public int _OrderNumber { get; set; }
        [Required]
        public string _ProjectName { get; set; }
        public string _Description { get; set; }
    }
}
