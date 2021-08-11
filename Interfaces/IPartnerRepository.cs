using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Allprimetech.Interfaces
{
    public interface IPartnerRepository
    {
        Task<IEnumerable<Partner>> AllPartners(int numOfItems, bool includeRelated = false);
        Task<Partner> GetPartner(int id, bool includeRelated = false);
        void AddPartner(Partner partner);
        void RemovePartner(Partner partner);
        Task<List<Partner>> SearchPartners(SearchResource searchTerm);
        Task<Partner> GetPartnerByName(string name);
        Task<Partner> GetPartnerByEmail(string email);
        Task<Partner> GetPartnerByPartnerNumber(string partnerNumber);
    }
}
