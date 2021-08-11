using Allprimetech.Interfaces.Attributes;
using Allprimetech.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class CreateCylinderResource
    {
        public int CylinderID { get; set; }

        [Required]
        public string _Order { get; set; }
        [Required]
        public string _Customer { get; set; }
        [Required]
        public string _DoorName { get; set; }
        [Required]
        [IntRange(AllowableValues = new int[] { 1,2,3,4,5,6,7,8,9,10,11,12,13 })]
        public int _ArticleNumber { get; set; }

        //[Required]
        //[IntRange(AllowableValues = new int[] { 27,31,36,41,46,51,56,61 })]
        public string _LengthOutside { get; set; }

        //[Required]
        //[IntRange(AllowableValues = new int[] { 27, 31, 36, 41, 46, 51, 56, 61 })]
        public string _LengthInside { get; set; }

        //[Required]
        //[IntRange(AllowableValues = new int[] { 1,2 })] 
        public string _Color { get; set; }

        [Required]
        [IntRange(AllowableValues = new int[] { 1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16 })]
        public int _Options { get; set; }
        public int _Quantity { get; set; }
        public int _QRCodeIssued { get; set; }
        public int _PositionId { get; set; }
        public string _CylinderNumber { get; set; }
        public int _CustomerID { get; set; }
        public int _OrderID { get; set; }
    }

    public class CreateCylinderOrderDetail
    {
        public int CylinderID { get; set; }
        [Required]
        public string _Customer { get; set; }
        [Required]
        public string _DoorName { get; set; }
        [Required]
        [IntRange(AllowableValues = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 })]
        public int _ArticleNumber { get; set; }
        public string _LengthOutside { get; set; }
        public string _LengthInside { get; set; }
        public string _Color { get; set; }

        [Required]
        [IntRange(AllowableValues = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 })]
        public int _Options { get; set; }
        public int _Quantity { get; set; }
        public int _QRCodeIssued { get; set; }
        public int _PositionId { get; set; }
        public string _CylinderNumber { get; set; }
        public OrderDetailOperations _Operation { get; set; }
    }
}
