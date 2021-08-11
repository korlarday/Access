using Allprimetech.Interfaces;
using Allprimetech.Interfaces.Constants;
using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources;
using Allprimetech.Interfaces.Resources.Partner;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Allprimetech.BL
{
    public class PartnersBL
    {
        #region Declarations
        private IPartnerRepository _PartnerRepository { get; set; }
        private IMapper _Mapper { get; set; }
        private IUnitOfWork _UnitOfWork { get; set; }
        private SystemAuditsBL _SystemAuditBL { get; set; }

        public PartnersBL(IPartnerRepository partnerRepository, IMapper mapper, IUnitOfWork unitOfWork, ISystemAuditRepository auditRepository)
        {
            _PartnerRepository = partnerRepository;
            _Mapper = mapper;
            _UnitOfWork = unitOfWork;
            _SystemAuditBL = new SystemAuditsBL(auditRepository, mapper, unitOfWork);
        }
        #endregion

        private async Task SaveOperation(string operatorId, Operation operation, Source source)
        {
            await _SystemAuditBL.StoreOperation(operation, source, operatorId);
        }

        public async Task<IEnumerable<ReadPartnerResource>> AllPartners(bool includeRelated)
        {
            var partners = await _PartnerRepository.AllPartners(StringStore.Limit, includeRelated);

            //await SaveOperation(userId, Operation.ReadAll, Source.Partner); 
            var partnersResource = _Mapper.Map<IEnumerable<Partner>, IEnumerable<ReadPartnerResource>>(partners);
            return partnersResource;
        }

        public async Task<ReadPartnerResource> GetPartner(string userId, int partnerId)
        {
            var partner = await _PartnerRepository.GetPartner(partnerId, true);

            if (partner == null)
                return null;

            await SaveOperation(userId, Operation.ReadSingle, Source.Partner);

            var partnerResource = _Mapper.Map<Partner, ReadPartnerResource>(partner);
            return partnerResource;
        }

        public async Task<ReadPartnerResource> UpdatePartner(string userId, CreatePartnerResource model)
        {
            var partner = await _PartnerRepository.GetPartnerByPartnerNumber(model._PartnerNumber);

            if (partner == null)
                return null;

            // update the partner
            partner._Name = model._Name;
            partner._UpdatedDate = DateTime.UtcNow;
            await _UnitOfWork.CompleteAsync();

            await SaveOperation(userId, Operation.Updated, Source.Partner);

            var result = _Mapper.Map<Partner, ReadPartnerResource>(partner);
            return result;
        }

        public async Task<CreateItemResponse> CreatePartner(string userId, CreatePartnerResource model)
        {
            var existingPartner = await _PartnerRepository.GetPartnerByEmail(model._Email);
            if(existingPartner != null)
            {
                return new CreateItemResponse { _Succeeded = false, _Message = StringStore.PartnerExists, _NewItem = null };
            }

            var partner = _Mapper.Map<CreatePartnerResource, Partner>(model);

            //partner.CreatedById = userId;

            _PartnerRepository.AddPartner(partner);
            await _UnitOfWork.CompleteAsync();

            await SaveOperation(userId, Operation.Added, Source.Partner);

            var result = _Mapper.Map<Partner, ReadPartnerResource>(partner);

            return new CreateItemResponse { _Succeeded = true, _Message = StringStore._Success, _NewItem = result };
        }

        public async Task<int?> DeletePartner(string userId, int partnerId)
        {
            var partner = await _PartnerRepository.GetPartner(partnerId);

            if (partner == null)
                return null;

            await SaveOperation(userId, Operation.Deleted, Source.Partner);

            // delete the partner
            _PartnerRepository.RemovePartner(partner);
            await _UnitOfWork.CompleteAsync();

            return partnerId;
        }

        public async Task<List<ReadPartnerResource>> SearchPartners(string userId, SearchResource searchTerm)
        {
            var partners = await _PartnerRepository.SearchPartners(searchTerm);

            await SaveOperation(userId, Operation.Search, Source.Partner);
            var partnersResource = _Mapper.Map<List<Partner>, List<ReadPartnerResource>>(partners);
            return partnersResource;
        }

        public async Task<ReadPartnerResource> ChangePartnerStatus(string userId, ChangePartnerStatusResource model)
        {
            var partner = await _PartnerRepository.GetPartnerByPartnerNumber(model._PartnerNumber);

            if (partner == null)
                return null;

            partner._PartnerStatus = model._PartnerStatus;
            await _UnitOfWork.CompleteAsync();

            await SaveOperation(userId, Operation.ReadSingle, Source.Partner);

            var partnerResource = _Mapper.Map<Partner, ReadPartnerResource>(partner);
            return partnerResource;
        }
    }
}
