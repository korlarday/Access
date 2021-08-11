using Allprimetech.Interfaces.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class SystemAuditSearchResouce
    {
        public string _OperatorID { get; set; }
        public int? _Operation { get; set; }
        public int? _Source { get; set; }
        public DateTime _StartDate { get; set; }
        public DateTime _EndDate { get; set; }
    }
    public class ProductionSearchResource
    {
        [Required]
        public DateTime _StartDate { get; set; }
        [Required]
        public DateTime _EndDate { get; set; }
    }

    public class SortResource
    {
        public int _CustomerID { get; set; }
        [StringRange(AllowableValues = new[] { "ASC", "DESC" }, ErrorMessage = "Sort type must be 'ASC' or 'DESC'.")]
        public string _SortType { get; set; }
    }
    public class FilterLP
    {
        public int _CustomerID { get; set; }
        public bool _FilterTopByGroup { get; set; }
        public bool _FilterTopByCylinder { get; set; }
        public bool _VerificationLP { get; set; }
    }

    public class GroupSearchBy
    {
        public int _CustomerID { get; set; }
        [StringRange(AllowableValues = new[] { "CylinderId", "CylinderNumber" }, ErrorMessage = "")]
        public string _SearchTerm { get; set; }
        
        public string _SearchValue { get; set; }
    }
    public class CylinderSearchBy
    {
        public int _CustomerID { get; set; }
        [StringRange(AllowableValues = new[] { "GroupId", "GroupNumber" }, ErrorMessage = "")]
        public string _SearchTerm { get; set; }
        
        public string _SearchValue { get; set; }
    }
}
