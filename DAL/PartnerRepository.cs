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
    public class PartnerRepository : IPartnerRepository
    {
        private ApplicationDbContext _Context;

        public PartnerRepository(ApplicationDbContext context)
        {
            _Context = context;
        }
        public void AddPartner(Partner partner)
        {
            partner._CreationDate = DateTime.UtcNow;
            partner._UpdatedDate = DateTime.UtcNow;
            partner._PartnerStatus = UserStatus.Activated;

            _Context.Partners.Add(partner);
        }

        public async Task<IEnumerable<Partner>> AllPartners(int numOfItems, bool includeRelated = false)
        {
            List<Partner> partners = null;

            if (includeRelated)
            {
                return await _Context.Partners.Include(x => x._Customers)
                                            .ToListAsync();
                //return partners.Take(numOfItems);
            }

            return await _Context.Partners.ToListAsync();
            //return partners.Take(numOfItems);
        }

        public async Task<Partner> GetPartner(int id, bool includeRelated = false)
        {
            if (includeRelated)
                return await _Context.Partners.Include(x => x._Customers)
                                            .SingleOrDefaultAsync(x => x.PartnerID == id);

            return await _Context.Partners.SingleOrDefaultAsync(x => x.PartnerID == id);
        }

        public async Task<Partner> GetPartnerByName(string name)
        {
            return await _Context.Partners.Where(x => x._Name == name).FirstOrDefaultAsync();
        }
        public async Task<Partner> GetPartnerByEmail(string email)
        {
            return await _Context.Partners.Where(x => x._Email == email).FirstOrDefaultAsync();
        }

        public async Task<Partner> GetPartnerByPartnerNumber(string partnerNumber)
        {
            return await _Context.Partners.Where(x => x._PartnerNumber == partnerNumber).FirstOrDefaultAsync();
        }

        public void RemovePartner(Partner partner)
        {
            _Context.Partners.Remove(partner);
        }

        public async Task<List<Partner>> SearchPartners(SearchResource model)
        {
            var partners = await _Context.Partners.ToListAsync();
            if (String.IsNullOrEmpty(model._SearchTerm) && (model._StartDate == null || model._EndDate == null))
            {
                return new List<Partner>();
            }
            
            if (!String.IsNullOrEmpty(model._SearchTerm))
            {
                var searchTerm = model._SearchTerm.Trim().ToLower();
                partners = partners.Where(x => x._Name.ToLower().Contains(searchTerm) ||
                                        x._PartnerNumber.ToString().Contains(searchTerm))
                            .ToList();
            }


            if (model._StartDate != null && model._EndDate != null)
            {
                DateTime startDate = (DateTime)model._StartDate;
                DateTime endDate = (DateTime)model._EndDate;
                partners = partners.Where(x => x._CreationDate >= startDate &&
                                                x._CreationDate <= endDate).ToList();
            }

            return partners;
            
        }
    }
}
