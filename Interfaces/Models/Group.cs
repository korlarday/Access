using Allprimetech.Interfaces.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Models
{
    public class Group
    {
        public Group()
        {

        }
        public Group(CreateGroupResource group)
        {
            _CreationDate = DateTime.UtcNow;
            _UpdatedDate = DateTime.UtcNow;
            _Name = group._KeyName;
            _Quantity = group._Quantity;
            _KeyNumber = group._KeyNumber;
            _GroupNumber = group._GroupNumber;
        }

        [Key]
        public int GroupID { get; set; }
        public DateTime _CreationDate { get; set; }
        public DateTime _UpdatedDate { get; set; }
        public Order Order { get; set; }
        public int? OrderID { get; set; }
        public Customer Customer { get; set; }
        public int CustomerID { get; set; }
        public string _Name { get; set; }
        public int _Quantity { get; set; }
        public string _KeyNumber { get; set; }
        public int _Produced { get; set; }
        public int _Validated { get; set; }
        public int _PositionId { get; set; }
        public int _Reclaimed { get; set; }
        public int _Blocked { get; set; }
        public string _GroupNumber { get; set; }
        public int _BatchNumber { get; set; }
    }
}
