using Allprimetech.Interfaces.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Models
{
    public class OrderDetail
    {

        [Key]
        public int OrderDetailID { get; set; }
        public ProductType _ProductType { get; set; }
        public DateTime _Date { get; set; }
        public ApplicationUser ByPerson { get; set; }
        public string ByPersonId { get; set; }
        public int OrderID { get; set; }
        public Order Order { get; set; }
        public int _ProductID { get; set; }
        public int _OldQty { get; set; }
        public int _NewQty { get; set; }
        public string _Notes { get; set; }


        public OrderDetail()
        {

        }
        public OrderDetail(ProductType selectedType, string byPersonId)
        {
            _ProductType = selectedType;
            _Date = DateTime.UtcNow;
            ByPersonId = byPersonId;
        }
    }
}
