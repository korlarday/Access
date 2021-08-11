using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Allprimetech.BL;
using Allprimetech.GeneralUtils;
using Allprimetech.Interfaces;
using Allprimetech.Interfaces.Constants;
using Allprimetech.Interfaces.ControllerCodes;
using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources;
using Allprimetech.Interfaces.Roles;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NETCore.MailKit.Core;
using Newtonsoft.Json;

namespace SAP_Lite_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IUsersRepository _usersRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UsersBL _UsersBL { get; set; }
        public UsersController(
            IMapper mapper,
            IConfiguration configuration,
            IUsersRepository usersRepository,
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailService emailService)
        {
            _mapper = mapper;
            Configuration = configuration;
            _usersRepository = usersRepository;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _UsersBL = new UsersBL(mapper, configuration, usersRepository, unitOfWork, userManager, emailService);
        }

        public IConfiguration Configuration { get; }

        #region private methods
        private IActionResult GetResult(object response)
        {
            USERS_ERROR status = UserControllerStatus._Error;
            UserControllerStatus._Error = USERS_ERROR.NO_ERROR;

            switch (status)
            {
                case USERS_ERROR.BAD_REQUEST:
                    return BadRequest(new TransactionStatus
                    {
                        _ErrorCode = (int)USERS_ERROR.BAD_REQUEST,
                        _Message = StringStore.BadRequest,
                        _Response = response
                    });
                case USERS_ERROR.EXISTS:
                    return BadRequest(new TransactionStatus
                    {
                        _ErrorCode = (int)USERS_ERROR.EXISTS,
                        _Message = StringStore.UserExistsResponse,
                        _Response = response
                    });

                case USERS_ERROR.NOT_FOUND:
                    return BadRequest(new TransactionStatus
                    {
                        _ErrorCode = (int)USERS_ERROR.NOT_FOUND,
                        _Message = StringStore.NotFound,
                        _Response = response
                    });

                case USERS_ERROR.EXCEPTION:
                    return StatusCode(StatusCodes.Status500InternalServerError, new TransactionStatus
                    {
                        _ErrorCode = (int)USERS_ERROR.EXCEPTION,
                        _Message = StringStore.ExceptionMessage,
                        _Response = response
                    });

                case USERS_ERROR.NO_ERROR:
                    if (response != null)
                    {
                        return Ok(response);
                    }
                    else
                    {
                        return Ok(new TransactionStatus
                        {
                            _ErrorCode = (int)USERS_ERROR.NO_ERROR,
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
            return User.Claims.First(c => c.Type == "UserId")?.Value;
        }

        private IActionResult IsValid()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return null;
        }
        #endregion



        [HttpPost]
        //[Authorize(Policy = "UserManagementCreate")]
        [Route("CreateUser")]
        public async Task<IActionResult> CreateApplicationUser(CreateUserResource model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _UsersBL.CreateApplicationUser(model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.CreateApplicationUser.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginResource model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _UsersBL.Login(model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.Login.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [HttpPost]
        [Route("ChangeUserStatus")]
        public async Task<IActionResult> ChangeUserStatus(ChangeUserStatus model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _UsersBL.ChangeUserStatus(model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.ChangeUserStatus.ToString(), ex.Message);
                return GetResult(ex);
            }
        }


        [HttpPost("VerifyEmail")]
        public async Task<IActionResult> VerifyEmail(VerifyEmailResource verify)
        {
            try
            {
                var result = await Task.Run(() => _UsersBL.VerifyEmail(verify));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.VerifyEmail.ToString(), ex.Message);
                return GetResult(ex);
            }
        }


        [HttpPost("ResendVerificationCode")]
        public async Task<IActionResult> ResendVerificationCode(ResendVerifyCodeResource model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _UsersBL.ResendVerificationCode(model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.ResendVerificationCode.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordResource model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _UsersBL.ForgotPassword(model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.ForgotPassword.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [HttpPost("VerifyResetPassword")]
        public async Task<IActionResult> VerifyResetPassword(ResetPasswordResource model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _UsersBL.VerifyResetPassword(model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.VerifyResetPassword.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [HttpPost("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordResource model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _UsersBL.ChangePassword(GetUserId(), model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.ChangePassword.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [HttpGet("GetRoles")]
        [Authorize(Policy = "AccountRead")]
        public async Task<IActionResult> Roles()
        {
            var roles = await _usersRepository.GetRoles();
            List<RoleDetailsResource> roleDetails = new List<RoleDetailsResource>();
            int[] rolesPositions = (int[])Enum.GetValues(typeof(RolesEnum));

            foreach (RolesEnum item in rolesPositions)
            {
                var result = Definition.GetRoleValue(item);
                int roleId = roles.Where(x => x._RoleName == result._Value).SingleOrDefault().ApplicationRoleID;
                RoleDetailsResource role = new RoleDetailsResource
                {
                    ApplicationRoleID = roleId,
                    _Name = result._Name,
                    _Value = result._Value,
                    _Group = result._Group.ToString(),
                    _ParentValue = result._ParentValue
                };
                roleDetails.Add(role);
            }

            return Ok(roleDetails);
        }

        [Route("GetUser/{id}")]
        [HttpGet]
        [Authorize(Policy = "AccountRead")]
        public async Task<IActionResult> GetUser(string id)
        {
            try
            {
                var result = await Task.Run(() => _UsersBL.GetUser(id));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.GetUser.ToString(), ex.Message);
                return GetResult(ex);
            }
            
        }



        [HttpGet("GetUsers")]
        //[Authorize(Policy = "AccountRead")]
        public async Task<IActionResult> AllUsers()
        {
            try
            {
                var result = await Task.Run(() => _UsersBL.AllUsers());
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.AllUsers.ToString(), ex.Message);
                return GetResult(ex);
            }
            
        }

        //[HttpGet]
        //[Authorize]
        //[Route("last-activities/{id}")]
        //public async Task<IActionResult> LastUserActivities(string id)
        //{
        //    var activities = await _usersRepository.GetUserLastActivities(id);
        //    var result = _mapper.Map<List<UserActivity>, List<UserActivityResource>>(activities);
        //    return Ok(result);
        //}

        [HttpPost("UpdateUser")]
        [Authorize(Policy = "AccountUpdate")]
        public async Task<IActionResult> UpdateUser(UpdateUserResource model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _UsersBL.UpdateUser(model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.UpdateUser.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [HttpPost("UpdateProfile/{id}")]
        [Authorize(Policy = "AccountUpdate")]
        public async Task<IActionResult> UpdateProfile(string id, UpdateUserProfile model)
        {
            IsValid();

            try
            {
                var result = await Task.Run(() => _UsersBL.UpdateProfile(id, model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.UpdateProfile.ToString(), ex.Message);
                return GetResult(ex);
            }

        }

        [HttpPost("VerifyUserEmail")]
        [Authorize]
        public async Task<IActionResult> VerifyUserEmail(VerifyUserEmailResource model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _UsersBL.VerifyUserEmail(model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.UpdateUser.ToString(), ex.Message);
                return GetResult(ex);
            }
        }

        [HttpPost("ChangeUserPassword")]
        [Authorize]
        public async Task<IActionResult> ChangeUserPassword(ChangeUserPasswordResource model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _UsersBL.ChangeUserPassword(model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.UpdateUser.ToString(), ex.Message);
                return GetResult(ex);
            }
        }


        [HttpPost("UpdateDiscCodeListDirectory")]
        [Authorize]
        public async Task<IActionResult> UpdateDiscCodeListDirectory(UpdateDiscCodeListResource model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _UsersBL.UpdateDiscCodeListDirectory(model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.UpdateDiscCodeListDirectory.ToString(), ex.Message);
                return GetResult(ex);
            }
        }


        [HttpPost("ReplaceEmail")]
        [Authorize]
        public async Task<IActionResult> ReplaceEmail(ReplaceEmail model)
        {
            IsValid();
            try
            {
                var result = await Task.Run(() => _UsersBL.ReplaceEmail(model));
                return GetResult(result);
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.SAP_Lite_API_Controllers.ToString(), BLEnums.MethodNames.ReplaceEmail.ToString(), ex.Message);
                return GetResult(ex);
            }
        }


        //[HttpGet("LoadJson")]
        //public void LoadJson()
        //{
        //    using (StreamReader r = new StreamReader("C:\\country.json"))
        //    {
        //        string json = r.ReadToEnd();
        //        dynamic array = JsonConvert.DeserializeObject(json);
        //        var countries = new List<Country>();
        //        foreach (var item in array)
        //        {
        //            Console.WriteLine("{0} {1}", item.temp, item.vcc);
        //            var country = new Country()
        //            {
        //                _CountryName = item.country_name,
        //                _TimeZoneName = item.timezone_name,
        //                _TimeZoneDescription = item.timezone_description,
        //                _UTCOffSet = item.utc_offset
        //            };
        //            countries.Add(country);
        //        }
        //        _usersRepository.SaveCountriesZone(countries);
        //    }
        //}


    }
}
