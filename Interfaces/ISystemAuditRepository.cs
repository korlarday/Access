using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Allprimetech.Interfaces
{
    public interface ISystemAuditRepository
    {
        Task<List<SystemAudit>> AllSystemAudits();
        Task<SystemAudit> GetSystemAudit(int id);
        Task AddSystemAudit(SystemAudit item, string userId);
        void DeleteSystemAudit(SystemAudit item);
        Task StoreOperation(Operation operation, Source source, string userId);
        Task<IEnumerable<SystemAudit>> SearchSystemAudit(SystemAuditSearchResouce searchTerm);
    }
}
