using Allprimetech.GeneralUtils;
using Allprimetech.Interfaces;
using Allprimetech.Interfaces.Constants;
using Allprimetech.Interfaces.ControllerCodes;
using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NETCore.MailKit.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allprimetech.BL
{
    public class UsersBL
    {
        #region Declarations 
        private UserManager<ApplicationUser> _UserManager { get; set; }
        private IEmailService _EmailService { get; set; }
        private IMapper _Mapper { get; set; }
        private IUsersRepository _UsersRepository { get; set; }
        private IUnitOfWork _UnitOfWork { get; set; }
        public IConfiguration Configuration { get; set; }
        public UsersBL(
            IMapper mapper,
            IConfiguration configuration,
            IUsersRepository usersRepository,
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _Mapper = mapper;
            Configuration = configuration;
            _UsersRepository = usersRepository;
            _UnitOfWork = unitOfWork;
            _UserManager = userManager;
            _EmailService = emailService;
        }

        #endregion
        public async Task<UserResponseResource> CreateApplicationUser(CreateUserResource model)
        {
            UserControllerStatus._Error = USERS_ERROR.NO_ERROR;
            try
            {
                var result = await _UsersRepository.CreateUser(model);
                if (result._User != null)
                {
                    UserResponseResource res = new UserResponseResource 
                    {
                        _User = _Mapper.Map<ApplicationUser, ReadUserResource>(result._User),
                        _Result = result._Result,
                        _Status = true
                    };
                    return res;
                }
                UserControllerStatus._Error = USERS_ERROR.BAD_REQUEST;
                return new UserResponseResource { _Result = result._Result, _User = null, _Status = false };
            }
            catch (Exception ex)
            {
                UserControllerStatus._Error = USERS_ERROR.EXCEPTION;

                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.CreateApplicationUser.ToString(), ex.Message);

                return new UserResponseResource { _Result = StringStore._Failed, _User = null, _Status = false };
            }
        }

        public async Task<LoginResponse> Login(LoginResource model)
        {
            UserControllerStatus._Error = USERS_ERROR.NO_ERROR;
            try
            {
                var user = await _UserManager.FindByEmailAsync(model._UserName);

                if (user != null && await _UserManager.CheckPasswordAsync(user, model._Password))
                {
                    // check if account has been disabled
                    //if (user.IsDisabled)
                    //{
                    //    return BadRequest(new { message = "Sorry your account has been locked. Please contact the administrator" });
                    //}


                    // check if email has not been confirmed
                    if (!user.EmailConfirmed)
                    {
                        // send a new token to the user email
                        await SendEmailToken(user);
                        return new LoginResponse { _Token = null, _Succeeded = true, _EmailConfirmed = false, _UserId = user.Id, _Message = StringStore._Success };
                    }


                    // set the user last login date
                    user._LastLogin = DateTime.UtcNow;

                    // save user access information
                    //model.ApplicationUserId = user.Id;
                    //_UsersRepository.SaveUserAccessInformation(model);

                    await _UnitOfWork.CompleteAsync();

                    // get user login token
                    var tokenJson = await _UsersRepository.GetLoginToken(user);

                    // get user roles
                    var appUserRoles = await _UsersRepository.GetUserRoles(user.Id);
                    List<string> roles = appUserRoles.Select(x => x._RoleName.ToString()).ToList();

                    var getUser = await _UsersRepository.GetUserById(user.Id);
                    var userResponse = _Mapper.Map<ApplicationUser, ReadUserResource>(getUser);

                    return new LoginResponse { 
                        _Token = tokenJson, 
                        _Succeeded = true, 
                        _UserId = user.Id, 
                        _EmailConfirmed = true, 
                        _Message = StringStore._Success,
                        _UserRoles = roles,
                        _User = userResponse
                    };
                }
                else
                {
                    return new LoginResponse { _Succeeded = false, _Token = String.Empty, _Message = StringStore._IncorrectLogin };
                }
            }
            catch (Exception ex)
            {
                UserControllerStatus._Error = USERS_ERROR.EXCEPTION;

                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.Login.ToString(), ex.Message);

                return new LoginResponse { _Succeeded = false, _Token = String.Empty, _Message = StringStore._ErrorOccured };
            }

        }

        public async Task<LoginResponse> VerifyEmail(VerifyEmailResource verify)
        {
            UserControllerStatus._Error = USERS_ERROR.NO_ERROR;

            try
            {
                var user = await _UserManager.FindByIdAsync(verify._UserId);

                if (user == null)
                {
                    UserControllerStatus._Error = USERS_ERROR.NOT_FOUND;
                    return null;
                }

                var result = await _UsersRepository.ConfirmEmail(user, verify._Code);

                if (result._Succeeded)
                {
                    user.EmailConfirmed = true;
                    await _UnitOfWork.CompleteAsync();
                    // get user login token
                    var tokenJson = await _UsersRepository.GetLoginToken(user);

                    return new LoginResponse 
                    { 
                        _Succeeded = true, 
                        _EmailConfirmed = true, 
                        _UserId = user.Id, 
                        _Message = StringStore._Success, 
                        _Token = tokenJson 
                    };
                }

                UserControllerStatus._Error = USERS_ERROR.BAD_REQUEST;
                return new LoginResponse(result);
            }
            catch (Exception ex)
            {
                UserControllerStatus._Error = USERS_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.VerifyEmail.ToString(), ex.Message);

                return new LoginResponse { _Succeeded = false };
            }
        }

        public async Task<ReadUserResource> ChangeUserStatus(ChangeUserStatus model)
        {
            UserControllerStatus._Error = USERS_ERROR.NO_ERROR;
            try
            {
                var user = await _UsersRepository.GetUserByUserNumber(model._UserNumber);
                if(user == null)
                {
                    UserControllerStatus._Error = USERS_ERROR.NOT_FOUND;
                    return null;
                }
                user._Status = model._Status;
                await _UnitOfWork.CompleteAsync();

                var userResponse = _Mapper.Map<ApplicationUser, ReadUserResource>(user);
                return userResponse;
            }
            catch (Exception ex)
            {
                UserControllerStatus._Error = USERS_ERROR.EXCEPTION;

                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.CreateApplicationUser.ToString(), ex.Message);

                throw;
            }
        }

        public async Task<ResendVerificationResponse> ResendVerificationCode(ResendVerifyCodeResource model)
        {
            UserControllerStatus._Error = USERS_ERROR.NO_ERROR;
            try
            {
                var user = await _UserManager.FindByIdAsync(model._UserId);
                if (user == null)
                {
                    UserControllerStatus._Error = USERS_ERROR.BAD_REQUEST;
                    return new ResendVerificationResponse { _Succeeded = false, _MinutesCount = 0 }; //bad request
                }

                if (user.EmailConfirmed)
                {
                    UserControllerStatus._Error = USERS_ERROR.BAD_REQUEST;
                    return new ResendVerificationResponse { _Succeeded = false, _EmailVerified = true, _MinutesCount = 0 }; //bad request
                }

                var userLastToken = await _UsersRepository.UserLastToken(user.Id);
                if (userLastToken == null) 
                {
                    UserControllerStatus._Error = USERS_ERROR.BAD_REQUEST;
                    return new ResendVerificationResponse { _Succeeded = false, _MinutesCount = 0 }; //bad request
                }

                var isLastTokenValid = _UsersRepository.IsTokenValid(userLastToken);
                if (!isLastTokenValid)
                {
                    await SendEmailToken(user);
                    return new ResendVerificationResponse { _Succeeded = true, _MinutesCount = 0 };
                }
                else
                {
                    UserControllerStatus._Error = USERS_ERROR.BAD_REQUEST;
                    var minutesLeft = (DateTime.UtcNow - userLastToken.CreatedAt).TotalMinutes;
                    return new ResendVerificationResponse { _Succeeded = false, _MinutesCount = minutesLeft, _Message = StringStore._WaitFor5Minites }; //bad request
                }
            }
            catch (Exception ex)
            {
                UserControllerStatus._Error = USERS_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.ResendVerificationCode.ToString(), ex.Message);
                return null;
            }
        }

        public async Task<object> VerifyResetPassword(ResetPasswordResource model)
        {
            UserControllerStatus._Error = USERS_ERROR.NO_ERROR;

            try
            {
                var user = await _UserManager.FindByIdAsync(model._UserId);

                if (user == null) 
                {
                    UserControllerStatus._Error = USERS_ERROR.NOT_FOUND;
                    return null;
                }

                var result = await _UserManager.ResetPasswordAsync(user, model._Token, model._Password);

                if (result.Succeeded)
                {
                    return new { succeeded = true };
                }

                return new { succeeded = false };
            }
            catch (Exception ex)
            {
                UserControllerStatus._Error = USERS_ERROR.EXCEPTION;

                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.VerifyResetPassword.ToString(), ex.Message);
                
                return null;
            }
        }

        public async Task<object> ChangePassword(string userId, ChangePasswordResource model)
        {
            UserControllerStatus._Error = USERS_ERROR.NO_ERROR;

            try
            {
                var user = await _UserManager.FindByIdAsync(userId);

                if (user == null)
                {
                    UserControllerStatus._Error = USERS_ERROR.BAD_REQUEST;
                    return new { Succeeded = false, Message = "Invalid request" }; // bad request
                }

                var result = await _UserManager.ChangePasswordAsync(user, model._CurrentPassword, model._NewPassword);
                if (result.Succeeded)
                {
                    return new { _Succeeded = true, _Message = "success" };
                }
                else
                {
                    return new { _Succeded = false, _Message = result.Errors.FirstOrDefault().Description };
                }

            }
            catch (Exception ex)
            {
                UserControllerStatus._Error = USERS_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.ChangePassword.ToString(), ex.Message);
                return null;
            }
        }

        public async Task<object> ForgotPassword(ForgotPasswordResource model)
        {
            UserControllerStatus._Error = USERS_ERROR.NO_ERROR;

            try
            {
                var user = await _UserManager.FindByEmailAsync(model._Email);
                if (user == null)
                {
                    UserControllerStatus._Error = USERS_ERROR.NOT_FOUND;
                    return null;
                }

                await SendPasswordResetToken(user);

                return new { Message = StringStore._Success };
            }
            catch (Exception ex)
            {
                UserControllerStatus._Error = USERS_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.ForgotPassword.ToString(), ex.Message);
                return null;
            }
        }

        
        public async Task<ReadUserResource> GetUser(string id)
        {
            UserControllerStatus._Error = USERS_ERROR.NO_ERROR;

            try
            {
                var user = await _UsersRepository.GetUserById(id);
                if (user == null)
                {
                    UserControllerStatus._Error = USERS_ERROR.NOT_FOUND;
                    return null;
                }

                var userResult = _Mapper.Map<ApplicationUser, ReadUserResource>(user);
                return userResult;
            }
            catch (Exception ex)
            {
                UserControllerStatus._Error = USERS_ERROR.EXCEPTION;

                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetUser.ToString(), ex.Message);

                return null;
            }
        }

        public async Task<bool> ChangeUserPassword(ChangeUserPasswordResource model)
        {
            try
            {
                var user = await _UsersRepository.GetUserByEmail(model._Email);
                if (user == null)
                {
                    UserControllerStatus._Error = USERS_ERROR.NOT_FOUND;
                    return false;
                }

                await _UsersRepository.ChangeUserPassword(user, model._Password);
                return true;
            }
            catch (Exception ex)
            {
                UserControllerStatus._Error = USERS_ERROR.EXCEPTION;

                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetUser.ToString(), ex.Message);
                return false;
            }
        }

        public async Task<bool> ReplaceEmail(ReplaceEmail model)
        {
            try
            {
                var user = await _UsersRepository.GetUserByEmail(model._OldEmail);
                if (user != null)
                {
                    user.Email = model._NewEmail;
                    user.NormalizedEmail = model._NewEmail;
                }

                var partner = await _UsersRepository.GetPartnerByEmail(model._OldEmail);
                if (partner != null)
                {
                    partner._Email = model._NewEmail;
                }

                var customer = await _UsersRepository.GetCustomerByEmail(model._OldEmail);
                if(customer != null)
                {
                    customer._Email = model._NewEmail;
                }

                await _UnitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                UserControllerStatus._Error = USERS_ERROR.EXCEPTION;

                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.ReplaceEmail.ToString(), ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateDiscCodeListDirectory(UpdateDiscCodeListResource model)
        {
            try
            {
                var user = await _UsersRepository.GetUserByEmail(model._Email);
                if (user == null)
                {
                    UserControllerStatus._Error = USERS_ERROR.NOT_FOUND;
                    return false;
                }

                user._DiscCodeListDirectory = model._NewDirectory;
                await _UnitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                UserControllerStatus._Error = USERS_ERROR.EXCEPTION;

                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetUser.ToString(), ex.Message);
                return false;
            }
        }

        public async Task<bool> VerifyUserEmail(VerifyUserEmailResource model)
        {
            try
            {
                var user = await _UsersRepository.GetUserByEmail(model._Email);
                if (user == null)
                {
                    UserControllerStatus._Error = USERS_ERROR.NOT_FOUND;
                    return false;
                }

                user.EmailConfirmed = true;
                await _UnitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                UserControllerStatus._Error = USERS_ERROR.EXCEPTION;

                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.GetUser.ToString(), ex.Message);
                return false;
            }
        }


        /// <summary>
        /// ///////////
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task SendEmailToken(ApplicationUser user)
        {
            try
            {
                // send the email
                //var code = await _UserManager.GenerateEmailConfirmationTokenAsync(user);

                var code = _UsersRepository.GenerateConfirmationCode();
                _UsersRepository.StoreEmailToken(user, code);
                await _UnitOfWork.CompleteAsync();

                //var link = Url.Action("Ahome", "Home", new { userId = user.Id, code }, Request.Scheme, Request.Host.ToString());

                //var link = "http://localhost:4200/email-verified/" + user.Id + "/" + code;
                await _EmailService.SendAsync(user.Email, "Verification Token", $"Email Verification token is {code}", true);

            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.BL.ToString(), "SendEmailToken", ex.Message);
                throw;
            }

        }

        public async Task<List<ReadUserResource>> AllUsers()
        {
            UserControllerStatus._Error = USERS_ERROR.NO_ERROR;

            try
            {
                var users = await _UserManager.Users.Include(x => x.Partner)
                                                    .Include(x => x._Roles)
                                                        .ThenInclude(x => x.ApplicationRole)
                                                    .OrderByDescending(x => x._DateRegistered)
                                                    .ToListAsync();

                var result = _Mapper.Map<List<ApplicationUser>, List<ReadUserResource>>(users);
                return result;
            }
            catch (Exception ex)
            {
                UserControllerStatus._Error = USERS_ERROR.EXCEPTION;

                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.AllUsers.ToString(), ex.Message);

                throw;
            }
        }

        private async Task SendPasswordResetToken(ApplicationUser user)
        {
            try
            {
                // send the email
                var code = await _UserManager.GeneratePasswordResetTokenAsync(user);

                var link = Configuration["ApplicationSettings:Client_URL"] + "/reset-password/" + user.Id + "/" + code;
                await _EmailService.SendAsync("reedwan47@gmail.com", "email verify", $"<a href=\"{link}\">Reset Password</a>", true);
        
            }
            catch (Exception ex)
            {
                Logs.logError(BLEnums.Namespaces.BL.ToString(), "SendPasswordResetToken", ex.Message);
                throw;
            }
        }

        public async Task<ReadUserResource> UpdateUser(UpdateUserResource model)
        {
            UserControllerStatus._Error = USERS_ERROR.NO_ERROR;

            try
            {
                var user = await _UsersRepository.GetUserByUserNumber(model._UserNumber);
                if (user == null)
                {
                    UserControllerStatus._Error = USERS_ERROR.NOT_FOUND;
                    return null;
                }

                var partner = await _UsersRepository.GetPartnerByPartnerNumber(model._PartnerNumber);
                var country = await _UsersRepository.GetCountryByName(model._CountryName);
                int? partnerId = 0;
                if (partner != null)
                {
                    partnerId = partner.PartnerID;
                    model._PartnerId = (int)partnerId;
                }
                user.UpdateUserInfo(model);
                user.CountryID = country == null ? user.CountryID : country.CountryID;

                await _UsersRepository.UpdateUserRoles(user, model._Roles);
                await _UsersRepository.UpdateUserCustomers(user, model._CustomerEmails);
                await _UnitOfWork.CompleteAsync();
                 
                var result = _Mapper.Map<ApplicationUser, ReadUserResource>(user);
                return result;
            }
            catch (Exception ex)
            {
                UserControllerStatus._Error = USERS_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.UpdateUser.ToString(), ex.Message);
                throw;
            }
        }

        public async Task<ReadUserResource> UpdateProfile(string id, UpdateUserProfile model)
        {
            UserControllerStatus._Error = USERS_ERROR.NO_ERROR;

            try
            {
                var user = await _UsersRepository.GetUserById(id);

                if (user == null)
                {
                    UserControllerStatus._Error = USERS_ERROR.NOT_FOUND;
                    return null;
                }

                user.UpdateUserProfile(model);
                //_UsersRepository.UpdateUserPhoto(user, model.Photo, model.PhotoName);
                await _UnitOfWork.CompleteAsync();

                var result = _Mapper.Map<ApplicationUser, ReadUserResource>(user);
                return result;
            }
            catch (Exception ex)
            {
                UserControllerStatus._Error = USERS_ERROR.EXCEPTION;
                Logs.logError(BLEnums.Namespaces.BL.ToString(), BLEnums.MethodNames.UpdateProfile.ToString(), ex.Message);
                return null;
            }
        }
    }
}
