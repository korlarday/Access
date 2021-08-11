using Allprimetech.BL;
using Allprimetech.GeneralUtils;
using Allprimetech.Interfaces;
using Allprimetech.Interfaces.Constants;
using Allprimetech.Interfaces.ControllerCodes;
using Allprimetech.Interfaces.Resources;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAP_Lite_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController: ControllerBase
    {
        #region Declarations
        public OrdersBL _OrderBL { get; set; }
        public OrdersController(
            IOrdersRepository orderRepository, 
            IMapper mapper, 
            IUnitOfWork unitOfWork,
            IGroupRepository groupRepository,
            ISystemAuditRepository auditRepository,
            ICustomersRepository customersRepository)
        {
            _OrderBL = new OrdersBL(orderRepository, mapper, unitOfWork, groupRepository, auditRepository, customersRepository);
            ProductControllerStatus._Error = PRODUCT_ERROR.NO_ERROR;
        }
        #endregion

        #region private methods
        private IActionResult GetResult(object response)
        {
            ORDER_ERROR status = OrderControllerStatus._Error;
            OrderControllerStatus._Error = ORDER_ERROR.NO_ERROR;

            switch (status)
            {
                case ORDER_ERROR.BAD_REQUEST:
                    return BadRequest(new TransactionStatus
                    {
                        _ErrorCode = (int)ORDER_ERROR.BAD_REQUEST,
                        _Message = StringStore.BadRequest
                    });
                case ORDER_ERROR.EXISTS:
                    return BadRequest(new TransactionStatus
                    {
                        _ErrorCode = (int)ORDER_ERROR.EXISTS,
                        _Message = StringStore.UserExistsResponse
                    });

                case ORDER_ERROR.NOT_FOUND:
                    return BadRequest(new TransactionStatus
                    {
                        _ErrorCode = (int)ORDER_ERROR.NOT_FOUND,
                        _Message = StringStore.NotFound
                    });

                case ORDER_ERROR.EXCEPTION:
                    return StatusCode(StatusCodes.Status500InternalServerError, new TransactionStatus
                    {
                        _ErrorCode = (int)ORDER_ERROR.EXCEPTION,
                        _Message = StringStore.ExceptionMessage
                    });

                case ORDER_ERROR.NO_ERROR:
                    if (response != null)
                    {
                        return Ok(response);
                    }
                    else
                    {
                        return Ok(new TransactionStatus
                        {
                            _ErrorCode = (int)ORDER_ERROR.NO_ERROR,
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


        #region Orders Endpoints

        [Route("GetOrders/{id}")]
        [Authorize(Policy = "OrderRead")]
        [HttpGet]
        public async Task<IActionResult> GetOrders(int id)
        {
            try
            {
                var result = await Task.Run(() => _OrderBL.AllOrders(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetOrders.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("GetOrder/{id}")]
        [Authorize(Policy = "OrderRead")]
        [HttpGet]
        public async Task<IActionResult> GetOrder(int id)
        {
            try
            {
                var result = await Task.Run(() => _OrderBL.GetOrder(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetOrder.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("UpdateOrder/{id}")]
        [Authorize(Policy = "OrderUpdate")]
        [HttpPost]
        public async Task<IActionResult> UpdateOrder(int id, CreateOrderResource model)
        {
            IsValid();
            try
            {
                var updatedProduct = await Task.Run(() => _OrderBL.UpdateOrder(GetUserId(), id, model));
                return GetResult(updatedProduct);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.UpdateOrder.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("CreateOrder")]
        [HttpPost]
        [Authorize(Policy = "OrderCreate")]
        public async Task<IActionResult> CreateOrder(CreateOrderResource model)
        {
            IsValid();

            try
            {
                var result = await Task.Run(() => _OrderBL.CreateOrder(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.CreateOrder.ToString(), ex.Message);
                return GetResult(ex);
            }

        }

        [Route("ModifyOrder")]
        [HttpPost]
        [Authorize(Policy = "OrderCreate")]
        public async Task<IActionResult> ModifyOrder(ModifyOrderResource model)
        {
            IsValid();

            try
            {
                var result = await Task.Run(() => _OrderBL.ModifyOrder(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.CreateOrder.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("DeleteOrder/{id}")]
        [Authorize(Policy = "OrderDelete")]
        [HttpPost]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                var result = await Task.Run(() => _OrderBL.DeleteOrder(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.DeleteOrder.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("SearchOrder")]
        [Authorize(Policy = "OrderRead")]
        [HttpPost]
        public async Task<IActionResult> SearchOrders(OrderSearchResource searchTerm)
        {
            try
            {
                var result = await Task.Run(() => _OrderBL.SearchOrders(GetUserId(), searchTerm));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.SearchOrders.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("ValidateOrder")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ValidateOrder(ValidateOrderResource model)
        {
            try
            {
                var result = await Task.Run(() => _OrderBL.ValidateOrder(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.ValidateOrder.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("SendReadyForPickUpEmail")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendReadyForPickUpEmail(ValidateOrderResource model)
        {
            try
            {
                var result = await Task.Run(() => _OrderBL.SendReadyForPickUpEmail(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.SendReadyForPickUpEmail.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("v1/SendReadyForPickUpEmail")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendReadyForPickUpEmailV1(ValidateOrderResource model)
        {
            try
            {
                var result = await Task.Run(() => _OrderBL.SendReadyForPickUpEmailV1(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.SendReadyForPickUpEmail.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("GetPickUpDetail/{customerId}")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetPickUpDetail(int customerId)
        {
            try
            {
                var result = await Task.Run(() => _OrderBL.GetPickUpDetail(GetUserId(), customerId));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetPickUpDetail.ToString(), ex.Message);
                return GetResult(ex);
            }
        }


        #endregion

        #region Orders Details Endpoints

        [Route("SearchOrderDetail")]
        [Authorize(Policy = "OrderRead")]
        [HttpPost]
        public async Task<IActionResult> SearchOrderDetails(OrderDetailSearchResource searchTerm)
        {
            try
            {
                var result = await Task.Run(() => _OrderBL.SearchOrderDetails(GetUserId(), searchTerm));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.SearchOrders.ToString(), ex.Message);
                return GetResult(ex);
            }
        }


        [Route("GetOrderDetails/{id}")]
        [Authorize(Policy = "OrderRead")]
        [HttpGet]
        public async Task<IActionResult> GetAllOrderDetails(int id)
        {
            try
            {
                var result = await Task.Run(() => _OrderBL.GetAllOrderDetails(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetAllOrderDetails.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("GetOrderDetailsForOrder/{id}")]
        [Authorize(Policy = "OrderRead")]
        [HttpGet]
        public async Task<IActionResult> GetOrderDetailsForAnOrder(int id)
        {
            try
            {
                var result = await Task.Run(() => _OrderBL.GetOrderDetailsForAnOrder(id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetOrderDetailsForAnOrder.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("SingleOrderDetail/{id}")]
        [Authorize(Policy = "OrderRead")]
        [HttpGet]
        public async Task<IActionResult> GetSingleOrderDetail(int id)
        {
            try
            {
                var result = await Task.Run(() => _OrderBL.GetSingleOrderDetail(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetSingleOrderDetail.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("UpdateOrderDetail/{id}")]
        [Authorize(Policy = "OrderUpdate")]
        [HttpPost]
        public async Task<IActionResult> UpdateOrderDetail(int id, CreateOrderDetailResource model)
        {
            IsValid();
            try
            {
                var updatedProduct = await Task.Run(() => _OrderBL.UpdateOrderDetail(GetUserId(), id, model));
                return GetResult(updatedProduct);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.UpdateOrderDetail.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [Route("CreateOrderDetail")]
        [HttpPost]
        [Authorize(Policy = "OrderCreate")]
        public async Task<IActionResult> CreateOrderDetail(CreateOrderDetailResource model)
        {
            IsValid();

            try
            {
                var result = await Task.Run(() => _OrderBL.CreateOrderDetail(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.CreateOrderDetail.ToString(), ex.Message);
                return GetResult(ex);
            }

        }

        [Route("CreateOrderDetailsBulk")]
        [HttpPost]
        [Authorize(Policy = "OrderCreate")]
        public async Task<IActionResult> CreateOrderDetailsBulk(CreateOrderDetailResource model)
        {
            IsValid();

            try
            {
                var result = await Task.Run(() => _OrderBL.CreateOrderDetailsBulk(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.CreateOrderDetail.ToString(), ex.Message);
                return GetResult(ex);
            }

        }

        [Route("DeleteOrderDetail/{id}")]
        [Authorize(Policy = "OrderDelete")]
        [HttpPost]
        public async Task<IActionResult> DeleteOrderDetail(int id)
        {
            try
            {
                var result = await Task.Run(() => _OrderBL.DeleteOrderDetail(GetUserId(), id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.DeleteOrderDetail.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        #endregion

    }
}
