using Allprimetech.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Allprimetech.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _Context { get; set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _Context = context;
        }
        public async Task CompleteAsync()
        {
            await _Context.SaveChangesAsync();
        }
    }
}
