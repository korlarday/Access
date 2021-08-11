using Allprimetech.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class CreateGroupResource
    {
        public int GroupID { get; set; }
        [Required]
        public string _Order { get; set; }
        [Required]
        public string _Customer { get; set; }
        [Required]
        public string _KeyName { get; set; }
        [Required]
        public int _Quantity { get; set; }
        [Required]
        public string _KeyNumber { get; set; } 
        public int _Produced { get; set; }
        public int _Validated { get; set; }
        public int _PositionId { get; set; }
        public string _GroupNumber { get; set; }
        public int _CustomerID { get; set; }
        public int _OrderID { get; set; }
    }

    public class CreateGroupOrderDetail
    {
        public int GroupID { get; set; }
        [Required]
        public string _Customer { get; set; }
        [Required]
        public string _KeyName { get; set; }
        [Required]
        public int _Quantity { get; set; }
        [Required]
        public string _KeyNumber { get; set; }
        public int _PositionId { get; set; }
        public string _GroupNumber { get; set; }
        public OrderDetailOperations _Operation { get; set; }
    }
}
