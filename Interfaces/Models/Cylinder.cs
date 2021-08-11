using Allprimetech.Interfaces.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Models
{
    public class Cylinder
    {
        public Cylinder()
        {

        }
        public Cylinder(CreateCylinderResource cylinder)
        {
            _DoorName = cylinder._DoorName;
            _CreationDate = DateTime.UtcNow;
            _UpdatedDate = DateTime.UtcNow;
            _ArticleNumber = (ArticleNumber)cylinder._ArticleNumber;
            _LengthInside = cylinder._LengthInside;
            _LengthOutside = cylinder._LengthOutside;
            _Color = cylinder._Color;
            _Options = (Options)cylinder._Options;
            _Quantity = cylinder._Quantity;
            _CylinderNumber = cylinder._CylinderNumber;
        }

        public int CylinderID { get; set; }
        public string _DoorName { get; set; }
        public Order Order { get; set; }
        public int? OrderID { get; set; }
        public Customer Customer { get; set; }
        public int CustomerID { get; set; }
        public DateTime _CreationDate { get; set; }
        public DateTime _UpdatedDate { get; set; }
        public ArticleNumber _ArticleNumber { get; set; }
        public string _LengthOutside { get; set; }
        public string _LengthInside { get; set; }
        public string _Color { get; set; }
        public Options _Options { get; set; }
        public int _Quantity { get; set; }
        public int _QRCodeIssued { get; set; }
        public int _Assembled { get; set; }
        public int _Blocked { get; set; }
        public int _Validated { get; set; }
        public int _Reclaimed { get; set; }
        public int _PositionId { get; set; }
        public string _CylinderNumber { get; set; }
        public int _BatchNumber { get; set; }
    }

}
