using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.DAL
{
    public class GroupsOptimization
    {
        public GroupsOptimization()
        {

        }
        public GroupsOptimization(GroupsOptimization item)
        {
            GroupKey = item.GroupKey;
            Cylinders = item.Cylinders;
        }
        public string GroupKey { get; set; }
        public string Cylinders { get; set; }
    }
    public class Occurrence
    {
        public string GroupKey { get; set; }
        public Dictionary<string, int> NumberOccured { get; set; }
    }
    public class VerticalOccurrence
    {
        public string Cylinder { get; set; }
        public Dictionary<string, int> NumberOccured { get; set; }
    }
    public class OccurrenceSummary
    {
        public string GroupKey { get; set; }
        public List<string> Occurrence { get; set; }
        public string VirtualName { get; set; }
    }

    public class VerticalOccurrenceSummary
    {
        public string Cylinder { get; set; }
        public List<string> Occurrence { get; set; }
        public string VirtualName { get; set; }
    }

    public class SubGroups
    {
        public string GroupName { get; set; }
        public string SubGroupName { get; set; }
        public string InnerGroups { get; set; }
        public string Cylinders { get; set; }
        public bool IsSubGroup { get; set; }
    }
    public class CylinderAndGroups
    {
        public string _CylinderID { get; set; }
        public string _RelatedGroups { get; set; }
        public string _DoorName { get; set; }
        public string _CylinderNumber { get; set; }
        public List<RelatedGroup> _FinalGroups { get; set; }

    }
    public class RelatedGroup
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string RelatedGrouping { get; set; }
    }
    public class FinalGrouping
    {
        public string _GroupID { get; set; }
        public string _RelatedGrouping { get; set; }
        public string _GroupName { get; set; }
    }

}
