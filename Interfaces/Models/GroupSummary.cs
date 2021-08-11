using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Models
{
    public class GroupSummary
    {
        public int GroupSummaryID { get; set; }
        public string GroupName { get; set; }
        public string SubGroupName { get; set; }
        public string InnerGroups { get; set; }
        public string Cylinders { get; set; }
        public bool IsSubGroup { get; set; }
        public int CustomerID { get; set; }
    }
}
