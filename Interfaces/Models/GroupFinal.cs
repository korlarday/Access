using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Models
{
    public class GroupFinal
    {
        public int GroupFinalID { get; set; }
        public Group Group { get; set; }
        public int GroupID { get; set; }
        public string _RelatedGrouping { get; set; }
        public bool _Validated { get; set; }
        public int _NumOfMatches { get; set; }
        public int CustomerID { get; set; }
        public Customer Customer { get; set; }
    }
}
