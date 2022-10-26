using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;
using APRP.Web.ViewModels.DTO;

namespace APRP.Web.Domain.Services
{
    public interface IApplicationUsersService
    {
        Task<ApplicationUsersResponse> UpdateSecurityStamp(UserDTO userDTO);

        Task<ApplicationUsersResponse> GenerateEmailConfirmationToken(UserDTO userDTO);

        Task<ApplicationUsersResponse> ConfirmEmail(UserDTO userDTO);

        Task<ApplicationUsersResponse> FindByNameAsync(string UserName);

        Task<ApplicationUsersResponse> GenerateTwoFactorTokenAsync(ApplicationUser user, string tokenProvider);

        Task<ApplicationUsersResponse> GetPhoneNumberAsync(ApplicationUser user);

        Task<ApplicationUsersResponse> ChangePhoneNumberAsync(ApplicationUser user, string PhoneNumber, string Code);

        Task<ApplicationUsersResponse> FindByIdAsync(string ID);

        Task<ApplicationUsersResponse> FindByEmailAsync(string Email);

        Task<ApplicationUsersResponse> GenerateChangePhoneNumberTokenAsync(ApplicationUser user, string phoneNumber);

        Task<ApplicationUsersResponse> CreateAsync(ApplicationUser user, string Password);

        Task<ApplicationUsersResponse> CheckPasswordAsync(ApplicationUser user, string Password);

        Task<ApplicationUsersResponse> SetTwoFactorEnabledAsync(ApplicationUser user, bool Enabled);

        Task<ApplicationUsersResponse> ResetPasswordAsync(ApplicationUser user, string Code, string Password);

        Task<ApplicationUsersResponse> GeneratePasswordResetTokenAsync(ApplicationUser user);

        Task<ApplicationUsersResponse> IsInRoleAsync(ApplicationUser applicationUser,string role);

        Task<ApplicationUsersResponse> GetRolesAsync(ApplicationUser applicationUser);

        Task<ApplicationUsersResponse> RemoveFromRoleAsync(ApplicationUser applicationUser, string role);

        Task<ApplicationUsersResponse> AddToRoleAsync(ApplicationUser applicationUser, ApplicationRole applicationRole);

        Task<ApplicationUsersResponse> GetAllUsersAsync();

        Task<ApplicationUsersResponse> GetAllUsersAsync(Authority authority);

        Task<ApplicationUsersResponse> UpdateUserAsync(ApplicationUser applicationUser);

        Task<ApplicationUsersResponse> GetUsersInRoleAsync(string roleName);
    }
}
