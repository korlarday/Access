using Allprimetech.Interfaces.Attributes;
using Allprimetech.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class SearchResource
    {
        public string _SearchTerm { get; set; }
        public DateTime? _StartDate { get; set; }
        public DateTime? _EndDate { get; set; }
    }

    public class CustomerSearchResource
    {
        [Required]
        [StringRange(AllowableValues = new string[] { "GCustName", "GCustNumber", "GCustSystemCode", "GCustPartner" }, ErrorMessage = "Invalid value supplied.")]
        public string _SearchTerm { get; set; }
        [Required]
        public string _SearchValue { get; set; }
    }

    public class OrderSearchResource
    {
        [Required]
        [StringRange(AllowableValues = new string[] { "GOrdNumber", "GOrdProjName", "GCustSystemCode", "GCustName" }, ErrorMessage = "Invalid value supplied.")]
        public string _SearchTerm { get; set; }
        [Required]
        public string _SearchValue { get; set; }
        [Required]
        public int _CustomerID { get; set; }
        public DateTime? _From { get; set; }
        public DateTime? _To { get; set; }
    }

    public class CylinderSearchResource
    {
        [StringRange(AllowableValues = new string[] { "GCylDrName", "GCylArtNumber", "GCylOption", "GCylNumber", "" }, ErrorMessage = "Invalid value supplied.")]
        public string _SearchTerm { get; set; }
        public string _SearchValue { get; set; }
        [Required]
        public int _CustomerID { get; set; }

        public List<string> _OrderNumbers { get; set; }

        public DateTime? _From { get; set; }
        public DateTime? _To { get; set; }
    }

    public class GroupSearchResource
    {
        [StringRange(AllowableValues = new string[] { "GKeyNumber", "GKeyName", "" }, ErrorMessage = "Invalid value supplied.")]
        public string _SearchTerm { get; set; }
        public string _SearchValue { get; set; }
        [Required]
        public int _CustomerID { get; set; }
        public List<string> _OrderNumbers { get; set; }
        public DateTime? _From { get; set; }
        public DateTime? _To { get; set; }
    }

    public class ProductionProdSearchResource
    {
        //[Required]
        //[StringRange(AllowableValues = new string[] { "GProdStatus", "GOrdNum" }, ErrorMessage = "Invalid value supplied.")]
        //[IntRange(AllowableValues = new int[] { 0, 1, 2, 3, 4, 5 }, ErrorMessage = "Invalid production status value supplied.")]
        public int _ProductionStatus { get; set; }
        public List<string> _OrderNumbers { get; set; }
        [Range(1, int.MaxValue)]
        public int _CustomerID { get; set; }

        public DateTime? _From { get; set; }
        public DateTime? _To { get; set; }
    }
    public class OrderDetailSearchResource
    {
        public List<string> _OrderNumbers { get; set; }
        [Range(1, int.MaxValue)]
        public int _CustomerID { get; set; }
        public int? _ProductType { get; set; }
        public DateTime? _From { get; set; }
        public DateTime? _To { get; set; }
    }
}