using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class CylinderWithGroups
    {
        public CylinderWithGroups()
        {

        }
        public CylinderWithGroups(CylinderWithGroups item)
        {
            _CylinderID = item._CylinderID;
            _RelatedGroups = item._RelatedGroups;
        }
        public int _CylinderID { get; set; }
        public string _RelatedGroups { get; set; }
    }

    public class CylindersWithGroups
    {
        public List<int> _CylinderID { get; set; }
        public string _RelatedGroups { get; set; }
    }

    public class CylinderGroupInfo
    {
        public string _GroupID { get; set; }
        public string _GroupName { get; set; }
        public string _GroupNumber { get; set; }
    }

    public class CylinderIdsWithGroups
    {
        public string _CylinderIDs { get; set; }
        public string _RelatedGroups { get; set; }
    }
    public class CylinderIdsNumberWithGroup
    {
        public string _CylinderIDs { get; set; }
        public string _RelatedGroups { get; set; }
        public string _CylinderNumbers { get; set; }
    }
    public class CylindersNamedGroups
    {
        public string _CylinderIDs { get; set; }
        public string _RelatedGroups { get; set; }
        public string _NamedGroup { get; set; }
    }

    public class CylinderWithNameGroups
    {
        public string _CylinderIDs { get; set; }
        public string _NamedGroups { get; set; }
        public string _GroupID { get; set; }
        public string _GroupName { get; set; }
        public string _GroupNumber { get; set; }
    }

    public class CylinderGroupsResource
    {
        public int _CylinderID { get; set; }
        public string _GroupIDs { get; set; }
        public string _GroupNumbers { get; set; }
    }
    public class GroupCylindersResource
    {
        public int _GroupID { get; set; }
        public string _CylinderIDs { get; set; }
        public int _CustomerID { get; set; }
        public int _Count { get; set; }
        public string _GroupName { get; set; }
        public string _DoorName { get; set; }
    }

    public class GroupInfo
    {
        public int _GroupID { get; set; }
        public string _GroupName { get; set; }
        public string _GroupNumber { get; set; }
    }

    public class GroupsInCylinder
    {
        public int _Count { get; set; }
        public List<GroupInfo> _Groups { get; set; }
    }
    public class GroupSortInfo
    {
        public int _GroupID { get; set; }
        public string _GroupName { get; set; }
        public string _GroupNumber { get; set; }
        public int _Count { get; set; }
    }

}
