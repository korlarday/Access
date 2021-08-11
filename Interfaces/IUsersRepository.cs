using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Allprimetech.Interfaces
{
    public interface IUsersRepository
    {
        Task<ApplicationUser> GetUserById(string userId);
        Task<UserResponse> CreateUser(CreateUserResource model);
        Task<List<ApplicationRole>> GetUserRoles(string userId);
        Task<List<ApplicationRole>> GetRoles();
        //void SaveUserAccessInformation(LoginResource model);
        //void StorePageVisited(PageVisited pageVisited);

        // email token
        void StoreEmailToken(ApplicationUser user, string token);
        Task<VerificationCode> UserLastToken(string userId);
        bool IsTokenValid(VerificationCode userToken);
        Task<ConfirmEmailResource> ConfirmEmail(ApplicationUser user, string token);
        string GenerateConfirmationCode();
        Task<string> GetLoginToken(ApplicationUser user);
        Task<ApplicationUser> GetUserByUserNumber(string userNumber);
        Task<Partner> GetPartnerByPartnerNumber(string partnerNumber);
        Task<ApplicationUser> GetUserByEmail(string email);
        Task ChangeUserPassword(ApplicationUser user, string newPassword);
        void SaveCountriesZone(List<Country> countries);
        Task<Country> GetCountryByName(string countryName);
        Task<Partner> GetPartnerByEmail(string oldEmail);
        Task UpdateUserRoles(ApplicationUser user, ICollection<int> selectedRoles);
        Task UpdateUserCustomers(ApplicationUser user, List<string> customerEmails);
        Task<Customer> GetCustomerByEmail(string oldEmail);


        //Task<List<Country>> GetLocations();

    }
}
