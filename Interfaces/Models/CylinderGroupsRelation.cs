using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Models
{
    public class CylinderGroupsRelation
    {
        public int CylinderGroupsRelationID { get; set; }
        public int CylinderID { get; set; }
        public int GroupID { get; set; }
        public string _GroupName { get; set; }
        public string _RelatedGrouping { get; set; }
        public int CustomerID { get; set; }
        public Customer Customer { get; set; }
    }
}
