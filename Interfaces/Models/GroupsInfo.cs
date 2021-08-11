using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Models
{
    public class GroupsInfo
    {
        public int GroupsInfoID { get; set; }
        public int _Slot { get; set; }
        public int _Row { get; set; }
        public int _Value { get; set; }
        public Customer Customer { get; set; }
        public int CustomerID { get; set; }
        public Group Group { get; set; }
        public int GroupID { get; set; }
        public DateTime _CreationDate { get; set; }
        public DateTime _UpdatedDate { get; set; }
    }
}
