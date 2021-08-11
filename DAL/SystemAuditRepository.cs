using Allprimetech.Interfaces;
using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allprimetech.DAL
{
    public class SystemAuditRepository : ISystemAuditRepository
    {
        private ApplicationDbContext _Context { get; set; }

        public SystemAuditRepository(ApplicationDbContext context)
        {
            _Context = context;
        }
        public async Task AddSystemAudit(SystemAudit item, string userId)
        {
            item._Date = DateTime.UtcNow;
            await _Context.SystemAudits.AddAsync(item);
        }

        public async Task<List<SystemAudit>> AllSystemAudits()
        {
            return await _Context.SystemAudits.ToListAsync();
        }

        public void DeleteSystemAudit(SystemAudit item)
        {
            _Context.SystemAudits.Remove(item);
        }

        public async Task<SystemAudit> GetSystemAudit(int id)
        {
            return await _Context.SystemAudits.FindAsync(id);
        }

        public async Task StoreOperation(Operation operation, Source source, string userId)
        {
            SystemAudit systemAudit = new SystemAudit(operation, source, userId);
            await _Context.SystemAudits.AddAsync(systemAudit);
        }

        public async Task<IEnumerable<SystemAudit>> SearchSystemAudit(SystemAuditSearchResouce searchTerm)
        {
            var systemAudits = await _Context.SystemAudits.ToListAsync();
            if (!String.IsNullOrEmpty(searchTerm._OperatorID))
            {
                systemAudits = systemAudits.Where(x => x.OperatorId == searchTerm._OperatorID).ToList();
            }
            if (searchTerm._Operation != null)
            {
                int value = (int)searchTerm._Operation;
                if (Enum.IsDefined(typeof(Operation), value))
                {
                    Operation filterOperation = (Operation)value;
                    systemAudits = systemAudits.Where(x => x._Operation == filterOperation).ToList();
                }
            }
            if (searchTerm._Source != null)
            {
                int value = (int)searchTerm._Source;
                if (Enum.IsDefined(typeof(Source), value))
                {
                    Source filterSource = (Source)value;
                    systemAudits = systemAudits.Where(x => x._Source == filterSource).ToList();
                }
            }
            systemAudits = systemAudits.Where(x => x._Date.Date >= searchTerm._StartDate.Date &&
                                                x._Date.Date <= searchTerm._EndDate.Date)
                                        .ToList();
            return systemAudits;
        }
    }
}
