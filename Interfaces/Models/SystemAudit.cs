using Allprimetech.Interfaces.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Allprimetech.Interfaces.Models
{
    public class SystemAudit
    {
        [Key]
        public int SystemAuditID { get; set; }
        public DateTime _Date { get; set; }
        public Operation _Operation { get; set; }
        public ApplicationUser Operator { get; set; }
        public string OperatorId { get; set; }
        public string _Description { get; set; }
        public Source _Source { get; set; }



        public SystemAudit()
        {

        }
        public SystemAudit(Operation operation, Source source, string userId)
        {
            _Date = DateTime.UtcNow;
            _Operation = operation;
            OperatorId = userId;
            _Source = source;
            _Description = StringStore.SystemAuditDescription(operation, source); 
        }
    }

    
}
