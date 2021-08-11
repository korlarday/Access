using Allprimetech.Interfaces.Attributes;
using Allprimetech.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class ReadOrderDetailResource
    {
        public int OrderDetailID { get; set; }
        public ProductType _ProductType { get; set; }
        public DateTime _Date { get; set; }
        //public ReadUserResource ByPerson { get; set; }
        public string ByPersonId { get; set; }
        public int OrderID { get; set; }
        public int _ProductID { get; set; }
        public int _OldQty { get; set; }
        public int _NewQty { get; set; }
        public string _Notes { get; set; }
        public string _ProjectName { get; set; }
        public string _OrderNumber { get; set; }
        public string _CreatedBy { get; set; }
    }
    //public class CreateOrderDetailResource
    //{
    //    [Required]
    //    [IntRange(AllowableValues = new int[] { 1, 2 }, ErrorMessage = "Invalid productType value supplied.")]
    //    public int _ProductType { get; set; } 
    //    [Required]
    //    public int _OrderID { get; set; }
    //    [Required]
    //    public string _ProductName { get; set; }
    //    [Required]
    //    public string _ProductNumber { get; set; }
    //    [Required]
    //    public int _CustomerID { get; set; }
    //    public int _NewQty { get; set; }
    //    [Required]
    //    [IntRange(AllowableValues = new int[] { 1, 2 }, ErrorMessage = "Invalid operation code supplied.")]
    //    public OrderDetailOperations _Operation { get; set; }
    //}

    public class CreateOrderDetailResource
    {
        public List<CreateCylinderOrderDetail> _Cylinders { get; set; }
        public List<CreateGroupOrderDetail> _Groups { get; set; }
        [Required]
        public int _OrderID { get; set; }
        [Required]
        public int _CustomerID { get; set; }
    }
}
