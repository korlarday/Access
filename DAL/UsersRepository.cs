using Allprimetech.GeneralUtils;
using Allprimetech.Interfaces;
using Allprimetech.Interfaces.Constants;
using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Allprimetech.DAL
{
    public class UsersRepository : IUsersRepository
    {
        private IConfiguration _Configuration { get; set; }
        private ApplicationDbContext _Context { get; set; }
        private UserManager<ApplicationUser> _UserManager { get; set; }
        public UsersRepository(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _Context = context;
            _UserManager = userManager;
            _Configuration = configuration;
        }

        public async Task<ConfirmEmailResource> ConfirmEmail(ApplicationUser user, string token)
        {
            try
            {
                var userToken = await UserLastToken(user.Id);
                if (userToken == null)
                    return new ConfirmEmailResource { _Succeeded = false, _Message = "Invalid Token, please generate new token" };

                if (!IsTokenValid(userToken))
                    return new ConfirmEmailResource { _Succeeded = false, _Message = "Token has expired, generate new token" };

                if (userToken.Token != token)
                {
                    ++userToken.RetryCount;
                    await _Context.SaveChangesAsync();
                    return new ConfirmEmailResource { _Succeeded = false, _Message = "Invalid Token Submitted" };
                }

                return new ConfirmEmailResource { _Succeeded = true, _Message = "Success" };
            }
            catch (Exception ex)
            {
                Logs.logError("UsersRepository", "ConfirmEmail", ex.Message);
                throw;
            }
        }

        public async Task<UserResponse> CreateUser(CreateUserResource model)
        {
            try
            {
                var partner = await _Context.Partners.Where(x => x._PartnerNumber == model._PartnerNumber).FirstOrDefaultAsync();
                int? partnerId = 0;
                if(partner != null)
                {
                    partnerId = partner.PartnerID;
                    model._PartnerId = (int)partnerId;
                }
                var country = await _Context.Countries.Where(x => x._CountryName == model._CountryName).FirstOrDefaultAsync();

                var user = new ApplicationUser(model);
                user.CountryID = country == null ? 15 : country.CountryID;

                user._Status = UserStatus.Activated;
                user._Customers = new Collection<ApplicationUserCustomer>();

                for (int i = 0; i < model._CustomerEmails.Count; i++)
                {
                    var customer = await _Context.Customers.Where(x => x._Email == model._CustomerEmails[i]).FirstOrDefaultAsync();
                    if(customer != null)
                    {
                        user._Customers.Add(new ApplicationUserCustomer { CustomerID = customer.CustomerID });
                    }
                }
                var result = await _UserManager.CreateAsync(user, model._Password);
                if (result.Succeeded)
                {
                    // add roles
                    foreach (var roleId in model._Roles)
                    {
                        _Context.ApplicationUserRoles.Add(
                            new ApplicationUserRole { ApplicationUserId = user.Id, ApplicationRoleID = roleId }
                        );
                    }
                    _Context.SaveChanges();
                    var createdUser = await _Context.ApplicationUsers.Where(x => x.Id == user.Id)
                                                                    .Include(x => x._Roles)
                                                                        .ThenInclude(x => x.ApplicationRole)
                                                                    .Include(x => x.Partner)
                                                                    .SingleOrDefaultAsync();

                    return new UserResponse { _Result = StringStore._Success, _User = createdUser, _IsSucceeded = true };
                }
                // something went wrong
                return new UserResponse { _Result = StringStore.SomethingWentWrong };
            }
            catch (Exception ex)
            {
                Logs.logError("UsersRepository", "CreateUser", ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public string GenerateConfirmationCode()
        {
            Random generator = new Random();
            string r = generator.Next(0, 1000000).ToString("D5");
            if (r.Distinct().Count() == 1)
            {
                r = GenerateConfirmationCode();
            }
            return r;
        }

        public async Task<string> GetLoginToken(ApplicationUser user)
        {
            try
            {
                //Get role assigned to the user
                var roles = await GetUserRoles(user.Id);

                // add roles to claim
                var customClaims = new List<Claim>();
                foreach (var role in roles)
                {
                    var newClaim = new Claim(ClaimTypes.Role, role._RoleName.ToString());
                    customClaims.Add(newClaim);
                }

                //var claimIdentity = new ClaimsIdentity(customClaims);
                customClaims.Add(new Claim("UserId", user.Id.ToString()));
                customClaims.Add(new Claim("Username", user.UserName.ToString()));
            
                customClaims.Add(new Claim("EmailVerified", user.EmailConfirmed ? "verified" : "unverified"));


                var secretBytes = Encoding.UTF8.GetBytes(_Configuration["ApplicationSettings:Secret"]);
                var key = new SymmetricSecurityKey(secretBytes);
                var algorithm = SecurityAlgorithms.HmacSha256;

                var signingCredentials = new SigningCredentials(key, algorithm);

                var token = new JwtSecurityToken(
                    _Configuration["ApplicationSettings:Issuer"],
                    _Configuration["ApplicationSettings:Audiance"],
                    customClaims,
                    notBefore: DateTime.UtcNow,
                    expires: DateTime.UtcNow.AddYears(1),
                    signingCredentials);

                return new JwtSecurityTokenHandler().WriteToken(token);

            }
            catch (Exception ex)
            {
                Logs.logError("UsersRepository", "GetLoginToken", ex.Message);
                throw;
            }
        }

        public async Task<List<ApplicationRole>> GetRoles()
        {
            try
            {
                return await _Context.ApplicationRoles.ToListAsync();
            }
            catch (Exception ex)
            {
                Logs.logError("UsersRepository", "GetRoles", ex.Message);
                throw;
            }
        }

        public async Task<ApplicationUser> GetUserById(string userId)
        {
            try
            {
                return await _Context.Users.Include(x => x.Partner)
                                            .Include(x => x._Roles)
                                            .ThenInclude(x => x.ApplicationRole)
                                            .SingleOrDefaultAsync(x => x.Id == userId);
            }
            catch (Exception ex)
            {
                Logs.logError("UsersRepository", "GetUserById", ex.Message);
                throw;
            }
        }

        public async Task<List<ApplicationRole>> GetUserRoles(string userId)
        {
            try
            {
                var userRoles = await _Context.ApplicationUserRoles.Where(x => x.ApplicationUserId == userId)
                                                            .Include(x => x.ApplicationRole)
                                                            .Select(x => x.ApplicationRole)
                                                            .ToListAsync();
                return userRoles;
            }
            catch (Exception ex)
            {
                Logs.logError("UsersRepository", "GetUserRoles", ex.Message);
                throw;
            }
        }

        public bool IsTokenValid(VerificationCode userToken)
        {
            try
            {
                if (userToken.Expired)
                    return false;

                if (userToken.RetryCount > 5)
                    return false;

                // get the minutes between the time the token was created to now
                var minutes = (DateTime.UtcNow - userToken.CreatedAt).TotalMinutes;

                return minutes < 5;
            }
            catch (Exception ex)
            {
                Logs.logError("UsersRepository", "IsTokenValid", ex.Message);
                throw;
            }
        }

        //public void SaveUserAccessInformation(LoginResource model)
        //{
        //    UserAccessInfo info = new UserAccessInfo(model);
        //    _Context.UserAccessInfos.Add(info);
        //}

        public void StoreEmailToken(ApplicationUser user, string token)
        {
            VerificationCode code = new VerificationCode(user.Id, token);
            _Context.VerificationCodes.Add(code);
        }

        //public void StorePageVisited(PageVisited pageVisited)
        //{
        //    pageVisited.DateVisited = DateTime.UtcNow;
        //    _Context.PagesVisited.Add(pageVisited);
        //}

        public async Task UpdateUserRoles(ApplicationUser user, ICollection<int> selectedRoles)
        {
            try
            {
                var allRoles = await _Context.ApplicationRoles.ToListAsync();
                foreach (var role in allRoles)
                {
                    if (selectedRoles.Contains(role.ApplicationRoleID))
                    {
                        if (!_Context.ApplicationUserRoles.Any(x => x.ApplicationUserId == user.Id && x.ApplicationRoleID == role.ApplicationRoleID))
                        {
                            user._Roles.Add(new ApplicationUserRole { ApplicationRoleID = role.ApplicationRoleID });
                        }
                    }
                    else
                    {
                        if (_Context.ApplicationUserRoles.Any(x => x.ApplicationUserId == user.Id && x.ApplicationRoleID == role.ApplicationRoleID))
                        {
                            var removeRole = _Context.ApplicationUserRoles.SingleOrDefault(x => x.ApplicationUserId == user.Id && x.ApplicationRoleID == role.ApplicationRoleID);
                            user._Roles.Remove(removeRole);
                        }
                    }
                }
        
            }
            catch (Exception ex)
            {
                Logs.logError("UsersRepository", "UpdateUserRoles", ex.Message);
                throw;
            }
        }

        public async Task UpdateUserCustomers(ApplicationUser user, List<string> customerEmails)
        {
            try
            {
                var allCustomers = await _Context.Customers.Where(x => x.PartnerID == user.PartnerID).ToListAsync();
                foreach (var customer in allCustomers)
                {
                    if (customerEmails.Contains(customer._Email))
                    {
                        if (!_Context.ApplicationUserCustomers.Any(x => x.ApplicationUserID == user.Id && x.CustomerID == customer.CustomerID))
                        {
                            user._Customers.Add(new ApplicationUserCustomer { CustomerID = customer.CustomerID });
                        }
                    }
                    else
                    {
                        if (_Context.ApplicationUserCustomers.Any(x => x.ApplicationUserID == user.Id && x.CustomerID == customer.CustomerID))
                        {
                            var removeCustomer = _Context.ApplicationUserCustomers.SingleOrDefault(x => x.ApplicationUserID == user.Id && x.CustomerID == customer.CustomerID);
                            _Context.ApplicationUserCustomers.Remove(removeCustomer);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logs.logError("UsersRepository", "UpdateUserRoles", ex.Message);
                throw;
            }
        }

        public async Task<VerificationCode> UserLastToken(string userId)
        {
            try
            {
                return await _Context.VerificationCodes.Where(x => x.ApplicationUserId == userId)
                                        .OrderByDescending(x => x.CreatedAt)
                                        .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Logs.logError("UsersRepository", "UserLastToken", ex.Message);
                throw;
            }
        }

        public async Task<ApplicationUser> GetUserByUserNumber(string userNumber)
        {
            try
            {
                return await _Context.Users.Include(x => x.Partner)
                                            .Include(x => x._Roles)
                                            .ThenInclude(x => x.ApplicationRole)
                                            .SingleOrDefaultAsync(x => x._UserNumber == userNumber);
            }
            catch (Exception ex)
            {
                Logs.logError("UsersRepository", "GetUserByUserNumber", ex.Message);
                throw;
            }
        }

        public async Task<Partner> GetPartnerByPartnerNumber(string partnerNumber)
        {
            try
            {
                return await _Context.Partners.SingleOrDefaultAsync(x => x._PartnerNumber == partnerNumber);
            }
            catch (Exception ex)
            {
                Logs.logError("UsersRepository", "GetPartnerByPartnerNumber", ex.Message);
                throw;
            }
        }

        public async Task<ApplicationUser> GetUserByEmail(string email)
        {
            return await _Context.ApplicationUsers.Where(x => x.Email == email).FirstOrDefaultAsync();
        }

        public async Task ChangeUserPassword(ApplicationUser user, string newPassword)
        {
            var token = await _UserManager.GeneratePasswordResetTokenAsync(user);
            await _UserManager.ResetPasswordAsync(user, token, newPassword);
        }

        public void SaveCountriesZone(List<Country> countries)
        {
            foreach (var item in countries)
            {
                _Context.Countries.Add(item);
                _Context.SaveChanges();
            }
        }

        public async Task<Country> GetCountryByName(string countryName)
        {
            return await _Context.Countries.Where(x => x._CountryName == countryName).FirstOrDefaultAsync();
        }

        public async Task<Partner> GetPartnerByEmail(string email)
        {
            return await _Context.Partners.Where(x => x._Email == email).FirstOrDefaultAsync();
        }

        public async Task<Customer> GetCustomerByEmail(string oldEmail)
        {
            return await _Context.Customers.Where(x => x._Email == oldEmail).FirstOrDefaultAsync();
        }
    }
}
