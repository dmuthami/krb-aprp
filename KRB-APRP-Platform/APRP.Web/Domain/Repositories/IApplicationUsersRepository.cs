using APRP.Web.Domain.Models;
using APRP.Web.ViewModels.DTO;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface IApplicationUsersRepository
    {
        Task<IActionResult> UpdateSecurityStamp(UserDTO userDTO);

        Task<IActionResult> GenerateEmailConfirmationToken(UserDTO userDTO);

        Task<IActionResult> ConfirmEmail(UserDTO userDTO);

        Task<IActionResult> FindByNameAsync(string UserName);

        Task<IActionResult> GenerateTwoFactorTokenAsync(ApplicationUser user, string tokenProvider);

        Task<IActionResult> GetPhoneNumberAsync(ApplicationUser user);

        Task<IActionResult> ChangePhoneNumberAsync(ApplicationUser user, string PhoneNumber, string Code);

        Task<IActionResult> FindByIdAsync(string ID);

        Task<IActionResult> FindByEmailAsync(string Email);

        Task<IActionResult> GenerateChangePhoneNumberTokenAsync(ApplicationUser user, string phoneNumber);

        Task<IActionResult> CreateAsync(ApplicationUser user, string Password);

        Task<IActionResult> CheckPasswordAsync(ApplicationUser user, string Password);

        Task<IActionResult> SetTwoFactorEnabledAsync(ApplicationUser user, bool Enabled);

        Task<IActionResult> ResetPasswordAsync(ApplicationUser user, string Code, string Password);

        Task<IActionResult> GeneratePasswordResetTokenAsync(ApplicationUser user);

        Task<IActionResult> IsInRoleAsync(ApplicationUser applicationUser,string role);

        Task<IActionResult> GetRolesAsync(ApplicationUser applicationUser);

        Task<IActionResult> RemoveFromRoleAsync(ApplicationUser applicationUser, string role);

        Task<IActionResult> AddToRoleAsync(ApplicationUser applicationUser, ApplicationRole applicationRole);

        Task<IActionResult> GetAllUsersAsync();

        Task<IActionResult> GetAllUsersAsync(Authority authority);

        Task<IActionResult> UpdateUserAsync(ApplicationUser user);

        Task<IActionResult> GetUsersInRoleAsync(string roleName);
    }
}
