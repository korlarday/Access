using Allprimetech.GeneralUtils;
using Allprimetech.Interfaces;
using Allprimetech.Interfaces.Constants;
using Allprimetech.Interfaces.ControllerCodes;
using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources;
using Allprimetech.Interfaces.Resources.Customer;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Allprimetech.BL.Properties;
using Allprimetech.ServiceRestAPI.Proxy;
using Allprimetech.ServiceRestAPI.Metadatas;

namespace Allprimetech.BL
{
    public class CustomersBL
    {
        #region Declarations
        private IPartnerRepository _PartnerRepository { get; set; }
        private ICustomersRepository _CustomerRepository { get; set; }
        private IMapper _Mapper { get; set; }
        private IUnitOfWork _UnitOfWork { get; set; }
        public SystemAuditsBL _SystemAuditsBL { get; set; }

        public CustomersBL(ICustomersRepository customersRepository, IMapper mapper, IUnitOfWork unitOfWork, ISystemAuditRepository auditRepository, IPartnerRepository partnerRepository)
        {
            _CustomerRepository = customersRepository;
            _Mapper = mapper;
            _UnitOfWork = unitOfWork;
            _PartnerRepository = partnerRepository;
            _SystemAuditsBL = new SystemAuditsBL(auditRepository, mapper, unitOfWork);
        }
        #endregion

        private async Task SaveOperation(string operatorId, Operation operation, Source source)
        {
            await _SystemAuditsBL.StoreOperation(operation, source, operatorId);
        }

        public async Task<IEnumerable<ReadCustomerResource>> AllCustomers(string userId, bool includeRelated)
        {
            try
            {
                var customers = await _CustomerRepository.AllCustomers(userId);
                var customersResource = _Mapper.Map<List<Customer>, List<ReadCustomerResource>>(customers);
                await SaveOperation(userId, Operation.ReadAll, Source.Customer);
                return customersResource;
            }
            catch (Exception ex)
            {
                Logs.logError("CustomersBL", "AllCustomers", ex.Message);
                CustomerControllerStatus._Error = CUSTOMER_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<ReadCustomerResource> GetCustomer(string userId, int customerId)
        {
            try
            {
                await SaveOperation(userId, Operation.ReadSingle, Source.Customer);

                var customer = await _CustomerRepository.GetCustomer(customerId);

                if (customer == null)
                {
                    CustomerControllerStatus._Error = CUSTOMER_ERROR.NOT_FOUND;
                    return null;
                }

                var customerResource = _Mapper.Map<Customer, ReadCustomerResource>(customer);
                return customerResource;
            }
            catch (Exception ex)
            {
                Logs.logError("CustomersBL",ex.Message, "GetCustomer", ex);
                CustomerControllerStatus._Error = CUSTOMER_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<ReadCustomerResource> UpdateCustomer(string userId, int customerId, CreateCustomerResource model)
        {
            try
            {
                var customer = await _CustomerRepository.GetCustomer(customerId);

                if (customer == null)
                {
                    CustomerControllerStatus._Error = CUSTOMER_ERROR.NOT_FOUND;
                    return null;
                }

                // update the customer
                _Mapper.Map(model, customer);
                customer._UpdatedDate = DateTime.UtcNow;
                await _UnitOfWork.CompleteAsync();

                await SaveOperation(userId, Operation.Updated, Source.Customer);
                var result = _Mapper.Map<Customer, ReadCustomerResource>(customer);
                return result;
            }
            catch (Exception ex)
            {
                Logs.logError("CustomersBL", "UpdateCustomer", ex.Message);
                CustomerControllerStatus._Error = CUSTOMER_ERROR.EXCEPTION;
                throw;
            }
        }

        //public async Task<CreateItemResponse> CreateCustomer(string userId, CreateCustomerResource model)
        //{
        //    try
        //    {
        //        // first check if customer exists
        //        var checkCustomer = await _CustomerRepository.GetCustomerByName(model._Name);
        //        if(checkCustomer != null)
        //        {
        //            // customer exists
        //            return new CreateItemResponse { _Succeeded = true, _Message = StringStore.CustomerExists, _NewItem = checkCustomer };
        //        }

        //        var customer = _Mapper.Map<CreateCustomerResource, Customer>(model);

        //        //customer.CreatedById = userId;

        //        await _CustomerRepository.AddCustomer(customer, userId, model._Partner);
        //        await _UnitOfWork.CompleteAsync();

        //        await SaveOperation(userId, Operation.Added, Source.Customer);

        //        var result = _Mapper.Map<Customer, ReadCustomerResource>(customer);

        //        return new CreateItemResponse { _Succeeded = true, _Message = "Success", _NewItem = result };
        //    }
        //    catch (Exception ex)
        //    {
        //        Logs.logError("CustomersBL", "CreateCustomer", ex.Message);
        //        CustomerControllerStatus._Error = CUSTOMER_ERROR.EXCEPTION;
        //        throw;
        //    }
        //}


        public async Task<CreateItemResponse> CreateCustomer(string userId, CreateCustomerResource model)
        {
            try
            {
                // first check if customer exists
                var checkCustomer = await _CustomerRepository.GetCustomerByEmail(model._Email);
                if (checkCustomer != null)
                {
                    // customer exists
                    return new CreateItemResponse { _Succeeded = true, _Message = StringStore.CustomerExists, _NewItem = checkCustomer };
                }

                var customer = _Mapper.Map<CreateCustomerResource, Customer>(model);

                //customer.CreatedById = userId;

                await _CustomerRepository.AddCustomer(customer, userId, model._Partner);
                await _UnitOfWork.CompleteAsync();

                string partnerCode = null; 
                if(customer.PartnerID != null)
                {
                    var partner = await _PartnerRepository.GetPartner((int)customer.PartnerID);
                    partnerCode = partner._PartnerNumber;
                }

                await SaveOperation(userId, Operation.Added, Source.Customer);

                var result = _Mapper.Map<Customer, ReadCustomerResource>(customer);
                var config = await _CustomerRepository.GetConfiguration();

                CustomersProxy customersProxy = new CustomersProxy(config.IntegraAdminURI);
                var integraCustomer = new CreateCustomer
                {
                    CompanyAddress = customer._ContactPerson,
                    CustomerCode = customer._SystemCode,
                    PartnerCode = partnerCode,
                    Name = customer._Name,
                    Phone = customer._ContactPerson,
                    Email = customer._Email
                };
                var response = customersProxy.CreateIACustomer(integraCustomer);

                return new CreateItemResponse { _Succeeded = true, _Message = "Success", _NewItem = result };
            }
            catch (Exception ex)
            {
                Logs.logError("CustomersBL", "CreateCustomer", ex.Message);
                CustomerControllerStatus._Error = CUSTOMER_ERROR.EXCEPTION;
                throw;
            }
        }


        public async Task<int?> DeleteCustomer(string userId, int customerId)
        {
            try
            {
                var customer = await _CustomerRepository.GetCustomer(customerId);

                if (customer == null)
                {
                    CustomerControllerStatus._Error = CUSTOMER_ERROR.NOT_FOUND;
                    return null;
                }

                // delete the customer
                customer._IsDeleted = true;
                await _UnitOfWork.CompleteAsync();

                await SaveOperation(userId, Operation.Deleted, Source.Customer);
                return customerId;
            }
            catch (Exception ex)
            {
                Logs.logError("CustomersBL", "DeleteCustomer", ex.Message);
                CustomerControllerStatus._Error = CUSTOMER_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<IEnumerable<ReadCustomerResource>> SearchCustomers(string userId, CustomerSearchResource searchTerm)
        {
            try
            {
                
                var customers = await _CustomerRepository.SearchCustomers(searchTerm, userId);

                await SaveOperation(userId, Operation.Search, Source.Customer);

                //var customersResource = _Mapper.Map<IEnumerable<Customer>, IEnumerable<ReadCustomerResource>>(customers);
                return customers;
            }
            catch (Exception ex)
            {
                Logs.logError("CustomersBL", "SearchCustomer", ex.Message);
                CustomerControllerStatus._Error = CUSTOMER_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<SystemCodeResource> GetNewCustomerSystemCode(string userId)
        {
            try
            {
                var customers = await _CustomerRepository.GetNewCustomerSystemCode();

                await SaveOperation(userId, Operation.Search, Source.Customer);

                //var customersResource = _Mapper.Map<IEnumerable<Customer>, IEnumerable<ReadCustomerResource>>(customers);
                return customers;
            }
            catch (Exception ex)
            {
                Logs.logError("CustomersBL", "GetNewCustomerSystemCode", ex.Message);
                CustomerControllerStatus._Error = CUSTOMER_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<CreateItemResponse> CreateCustomerAdmin(string userId, CreateCustomerAdminResource model)
        {
            try
            {
                // first check if customer exists
                var checkCustomer = await _CustomerRepository.GetCustomerByEmail(model._Email);
                if (checkCustomer != null)
                {
                    // customer exists
                    return new CreateItemResponse { _Succeeded = true, _Message = StringStore.CustomerExists, _NewItem = checkCustomer };
                }

                var partner = await _PartnerRepository.GetPartnerByPartnerNumber(model._PartnerNumber);
                int? partnerId = null;
                if (partner != null)
                    partnerId = partner.PartnerID;

                var customer = new Customer();
                customer._ContactPerson = model._ContactPerson;
                customer._CustomerNumber = model._CustomerNumber;
                customer._Name = model._Name;
                customer._SystemCode = model._SystemCode;
                customer._Email = model._Email;
                customer._CustomerStatus = UserStatus.Deactivated;

                //customer.CreatedById = userId;

                await _CustomerRepository.AddCustomer(customer, userId, model._PartnerNumber);
                customer.PartnerID = partnerId;
                await _UnitOfWork.CompleteAsync();

                await SaveOperation(userId, Operation.Added, Source.Customer);

                var result = _Mapper.Map<Customer, ReadCustomerResource>(customer);

                return new CreateItemResponse { _Succeeded = true, _Message = "Success", _NewItem = result };
            }
            catch (Exception ex)
            {
                Logs.logError("CustomersBL", ex.Message, "CreateCustomerAdmin", ex);
                CustomerControllerStatus._Error = CUSTOMER_ERROR.EXCEPTION;
                throw;
            }
        }

        

        public async Task<ReadCustomerResource> UpdateCustomerAdmin(string userId, CreateCustomerAdminResource model)
        {
            try
            {

                var partner = await _PartnerRepository.GetPartnerByPartnerNumber(model._PartnerNumber);
                int? partnerId = null;
                if (partner != null)
                    partnerId = partner.PartnerID;

                var customer = await _CustomerRepository.GetCustomerBySystemCode(model._SystemCode);
                if(customer == null)
                {
                    CustomerControllerStatus._Error = CUSTOMER_ERROR.NOT_FOUND;
                    return null;
                }
                customer._ContactPerson = model._ContactPerson;
                //customer._CustomerNumber = model._CustomerNumber;
                customer._Name = model._Name;
                customer._Email = model._Email;

                //customer.CreatedById = userId;

                customer.PartnerID = partnerId;
                await _UnitOfWork.CompleteAsync();

                await SaveOperation(userId, Operation.Added, Source.Customer);

                var result = _Mapper.Map<Customer, ReadCustomerResource>(customer);

                return result;
            }
            catch (Exception ex)
            {
                Logs.logError("CustomersBL", "UpdateCustomerAdmin", ex.Message);
                CustomerControllerStatus._Error = CUSTOMER_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<bool> VerifyCustomerEmail(string userId, VerifyCustomerEmail model)
        {
            try
            {
                var customer = await _CustomerRepository.GetCustomerByEmail(model._Email);
                if (customer == null)
                {
                    CustomerControllerStatus._Error = CUSTOMER_ERROR.NOT_FOUND;
                    return false;
                }
                customer._EmailVerified = true;
                customer._CustomerStatus = UserStatus.Activated;
                await _UnitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                Logs.logError("CustomersBL", "VerifyCustomerEmail", ex.Message);
                CustomerControllerStatus._Error = CUSTOMER_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<bool> ChangeCustomerStatus(string userId, ChangeCustomerStatus model)
        {
            try
            {
                var customer = await _CustomerRepository.GetCustomerByEmail(model._Email);
                if (customer == null)
                {
                    CustomerControllerStatus._Error = CUSTOMER_ERROR.NOT_FOUND;
                    return false;
                }
                customer._CustomerStatus = model._Status;
                await _UnitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                Logs.logError("CustomersBL", "ChangeCustomerStatus", ex.Message);
                CustomerControllerStatus._Error = CUSTOMER_ERROR.EXCEPTION;
                throw;
            }
        }

        public async Task<bool> DeleteCustomer(string userId, VerifyCustomerEmail model)
        {
            try
            {
                var customer = await _CustomerRepository.GetCustomerByEmail(model._Email);
                if (customer == null)
                {
                    CustomerControllerStatus._Error = CUSTOMER_ERROR.NOT_FOUND;
                    return false;
                }
                customer._IsDeleted = true;
                await _UnitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                Logs.logError("CustomersBL", "DeleteCustomer", ex.Message);
                CustomerControllerStatus._Error = CUSTOMER_ERROR.EXCEPTION;
                throw;
            }
        }


        public async Task<LockingPlanInfoResource> GetCustomerLockingPlanInfo(string userId, int customerId)
        {
            try
            {
                var result = await _CustomerRepository.GetCustomerLockingPlanInfo(customerId);
                return result;
            }
            catch (Exception ex)
            {
                Logs.logError("CustomersBL", ex.Message, "GetCustomerLockingPlanInfo", ex);
                CustomerControllerStatus._Error = CUSTOMER_ERROR.EXCEPTION;
                throw;
            }
        }
    }
}
