using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Allprimetech.Interfaces
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
    }
}
