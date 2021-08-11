using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Allprimetech.BL;
using Allprimetech.DAL;
using Allprimetech.GeneralUtils;
using Allprimetech.Interfaces;
using Allprimetech.Interfaces.Constants;
using Allprimetech.Interfaces.ControllerCodes;
using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources;
using Allprimetech.Interfaces.Resources.Customer;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace SAP_Lite_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private DbContextOptions<ApplicationDbContext> _Options;
        #region Declarations
        public CustomersBL _CustomersBL { get; set; }
        public CustomersController(
            ICustomersRepository customerRepository, 
            IMapper mapper, IUnitOfWork unitOfWork, ISystemAuditRepository auditRepository,
            DbContextOptions<ApplicationDbContext> options,
            IPartnerRepository partnerRepository)
        {
            _CustomersBL = new CustomersBL(customerRepository, mapper, unitOfWork, auditRepository, partnerRepository);
            this._Options = options;
        }
        #endregion

        #region private methods
        private IActionResult GetResult(object response)
        {
            CUSTOMER_ERROR status = CustomerControllerStatus._Error;
            CustomerControllerStatus._Error = CUSTOMER_ERROR.NO_ERROR;

            switch (status)
            {
                case CUSTOMER_ERROR.BAD_REQUEST:
                    return BadRequest(new TransactionStatus
                    {
                        _ErrorCode = (int)CUSTOMER_ERROR.BAD_REQUEST,
                        _Message = StringStore.BadRequest
                    });
                case CUSTOMER_ERROR.EXISTS:
                    return BadRequest(new TransactionStatus
                    {
                        _ErrorCode = (int)CUSTOMER_ERROR.EXISTS,
                        _Message = StringStore.UserExistsResponse
                    });

                case CUSTOMER_ERROR.NOT_FOUND:
                    return BadRequest(new TransactionStatus
                    {
                        _ErrorCode = (int)CUSTOMER_ERROR.NOT_FOUND,
                        _Message = StringStore.NotFound
                    });

                case CUSTOMER_ERROR.EXCEPTION:
                    return StatusCode(StatusCodes.Status500InternalServerError, new TransactionStatus
                    {
                        _ErrorCode = (int)CUSTOMER_ERROR.EXCEPTION,
                        _Message = StringStore.ExceptionMessage
                    });

                case CUSTOMER_ERROR.NO_ERROR:
                    if (response != null)
                    {
                        return Ok(response);
                    }
                    else
                    {
                        return Ok(new TransactionStatus
                        {
                            _ErrorCode = (int)CUSTOMER_ERROR.NO_ERROR,
                            _Message = StringStore.SuccessExecutionMessage
                        });
                    }

                default:
                    return BadRequest();
            }

            //if (response == null)
            //    return NotFound();

            //return Ok(response);
        }

        private string GetUserId()
        {
            return User.Claims.First(c => c.Type == "UserId").Value;
        }

        private IActionResult IsValid()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return null;
        }
        #endregion

        [Route("GetCustomers")]
        [Authorize(Policy = "CustomerRead")]
        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            try
            {
                var result = await Task.Run(() => _CustomersBL.AllCustomers(GetUserId(), true));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetCustomers.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        //[Authorize(Policy = "CustomerRead")]
        [Authorize]
        [Route("GetCustomer/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetCustomer(int id)
        {
            try
            {
                var result = await Task.Run(() => _CustomersBL.GetCustomer(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetCustomer.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("UpdateCustomer/{id}")]
        [Authorize(Policy = "CustomerUpdate")]
        [HttpPost]
        public async Task<IActionResult> UpdateCustomer(int id, CreateCustomerResource model)
        {
            IsValid();
            try
            {
                var updatedProduct = await Task.Run(() => _CustomersBL.UpdateCustomer(GetUserId(), id, model));
                return GetResult(updatedProduct);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.UpdateCustomer.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("CreateCustomer")]
        [HttpPost]
        [Authorize(Policy = "CustomerCreate")]
        public async Task<IActionResult> CreateCustomer(CreateCustomerResource model)
        {
            IsValid();

            try
            {
                var result = await Task.Run(() => _CustomersBL.CreateCustomer(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.CreateCustomer.ToString(), ex.Message);
                return GetResult(ex);
            }

        }

        [Route("GetNewCustomerSystemCode")]
        [HttpGet]
        [Authorize(Policy = "CustomerCreate")]
        public async Task<IActionResult> GetNewCustomerSystemCode()
        {

            try
            {
                var result = await Task.Run(() => _CustomersBL.GetNewCustomerSystemCode(GetUserId()));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetNewCustomerSystemCode.ToString(), ex.Message);
                return GetResult(ex);
            }

        }


        [Route("DeleteCustomer/{id}")]
        [Authorize(Policy = "CustomerDelete")]
        [HttpPost]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                var result = await Task.Run(() => _CustomersBL.DeleteCustomer(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.DeleteCustomer.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("SearchCustomer")]
        [Authorize(Policy = "CustomerRead")]
        [HttpPost]
        public async Task<IActionResult> SearchCustomers(CustomerSearchResource model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _CustomersBL.SearchCustomers(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.SearchCustomers.ToString(), ex.Message);
                return GetResult(ex);
            }
        }


        [Route("CreateCustomerAdmin")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateCustomerAdmin(CreateCustomerAdminResource model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _CustomersBL.CreateCustomerAdmin(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.CreateCustomer.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("UpdateCustomerAdmin")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateCustomerAdmin(CreateCustomerAdminResource model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _CustomersBL.UpdateCustomerAdmin(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.CreateCustomer.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("VerifyCustomerEmail")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> VerifyCustomerEmail(VerifyCustomerEmail model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _CustomersBL.VerifyCustomerEmail(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.VerifyCustomerEmail.ToString(), ex.Message);
                return GetResult(ex);
            }
        }


        [Route("GetCustomerLockingPlanInfo/{id}")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> GetCustomerLockingPlanInfo(int id)
        {
            try
            {
                var result = await Task.Run(() => _CustomersBL.GetCustomerLockingPlanInfo(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetCustomerLockingPlanInfo.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("ChangeCustomerStatus")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangeCustomerStatus(ChangeCustomerStatus model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _CustomersBL.ChangeCustomerStatus(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.ChangeCustomerStatus.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("DeleteCustomer")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteCustomer(VerifyCustomerEmail model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _CustomersBL.DeleteCustomer(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.DeleteCustomer.ToString(), ex.Message);
                return GetResult(ex);
            }
        }



        //[Route("Populate")]
        //[Authorize]
        //[HttpGet]
        //public async Task<IActionResult> Populate()
        //{
        //    for (int i = 2; i < 70; i++)
        //    {
        //        Order order = await _context.Orders.FindAsync(i);
        //        Production cylinder = new Production()
        //        {
        //            _ProductID = i,
        //            _CreationDate = DateTime.UtcNow,
        //            _UpdatedDate = DateTime.UtcNow,
        //            OrderID = order.OrderID,
        //            _Status = ProductionStatus.Validated,
        //            _ProductType = ProductType.Cylinder,
        //            ByUserId = GetUserId()
        //        };
        //        _context.Productions.Add(cylinder);
        //        await _context.SaveChangesAsync();
        //    }
        //    return Ok();
        //}

        [Route("test")]
        [HttpGet]
        public async Task<ActionResult> Test()
        {
            List<string> roles = new List<string> { "1.3", "1.3", "2" };
            if (roles.Contains("2"))
            {
                return Ok("Contains 2");
            }
            else
            {
                return Ok("Does not contain");
            }
        }



    }


    public class Item
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
    }
}