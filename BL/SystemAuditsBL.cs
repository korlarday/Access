using Allprimetech.Interfaces;
using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Allprimetech.BL
{
    public class SystemAuditsBL
    {
        #region Declarations
        private ISystemAuditRepository _SystemAuditRepo { get; set; }
        private IMapper _Mapper { get; set; }
        private IUnitOfWork _UnitOfWork { get; set; }

        public SystemAuditsBL(
            ISystemAuditRepository systemAuditRepo,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _SystemAuditRepo = systemAuditRepo;
            _Mapper = mapper;
            _UnitOfWork = unitOfWork;
        }
        #endregion

        private async Task SaveOperation(string operatorId, Operation operation, Source source)
        {
            await _SystemAuditRepo.StoreOperation(operation, source, operatorId);
        }
        #region SystemAudit Crud
        public async Task<IEnumerable<ReadSystemAuditResource>> AllSystemAudits(string userId)
        {
            var cylinders = await _SystemAuditRepo.AllSystemAudits();
            var cylindersResource = _Mapper.Map<List<SystemAudit>, List<ReadSystemAuditResource>>(cylinders);
            return cylindersResource;
        }

        public async Task<ReadSystemAuditResource> GetSystemAudit(string userId, int customerId)
        {
            var customer = await _SystemAuditRepo.GetSystemAudit(customerId);

            if (customer == null)
                return null;

            var customerResource = _Mapper.Map<SystemAudit, ReadSystemAuditResource>(customer);
            return customerResource;
        }

        public async Task<ReadSystemAuditResource> UpdateSystemAudit(string userId, int customerId, CreateSystemAuditResource model)
        {
            var customer = await _SystemAuditRepo.GetSystemAudit(customerId);

            if (customer == null)
                return null;

            // update the customer
            _Mapper.Map(model, customer);
            customer._Date = DateTime.UtcNow;
            await _UnitOfWork.CompleteAsync();

            var result = _Mapper.Map<SystemAudit, ReadSystemAuditResource>(customer);
            return result;
        }

        public async Task<ReadSystemAuditResource> CreateSystemAudit(string userId, CreateSystemAuditResource model)
        {
            var customer = _Mapper.Map<CreateSystemAuditResource, SystemAudit>(model);

            //customer.CreatedById = userId;

            await _SystemAuditRepo.AddSystemAudit(customer, userId);
            await _UnitOfWork.CompleteAsync();

            var result = _Mapper.Map<SystemAudit, ReadSystemAuditResource>(customer);

            return result;
        }

        public async Task<int?> DeleteSystemAudit(string userId, int customerId)
        {
            var customer = await _SystemAuditRepo.GetSystemAudit(customerId);

            if (customer == null)
                return null;

            // delete the customer
            _SystemAuditRepo.DeleteSystemAudit(customer);
            await _UnitOfWork.CompleteAsync();

            return customerId;
        }

        public async Task StoreOperation(Operation operation, Source source, string userId)
        {
            await _SystemAuditRepo.StoreOperation(operation, source, userId);
            await _UnitOfWork.CompleteAsync();
        }


        public async Task<IEnumerable<ReadSystemAuditResource>> SearchSystemAudits(string userId, SystemAuditSearchResouce searchTerm)
        {
            var customers = await _SystemAuditRepo.SearchSystemAudit(searchTerm);

            await SaveOperation(userId, Operation.Search, Source.SystemAudit);

            var customersResource = _Mapper.Map<IEnumerable<SystemAudit>, IEnumerable<ReadSystemAuditResource>>(customers);
            return customersResource;
        }
        #endregion

    }
}
