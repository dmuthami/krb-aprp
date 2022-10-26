using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using APRP.Web.ViewModels.ResponseTypes;
using APRP.Web.ViewModels.DTO;
using APRP.Web.Extensions;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class ApplicationUsersRepository : BaseRepository, IApplicationUsersRepository
    {
        private IConfiguration Configuration { get; }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        private readonly ISmsSender _smsSender;

        private CultureInfo _cultures = new CultureInfo("en-US");
        public ApplicationUsersRepository(AppDbContext context, UserManager<ApplicationUser> userManager,
             IConfiguration configuration, ILogger<ApplicationUsersRepository> logger,
             ISmsSender smsSender) : base(context)
        {
            _userManager = userManager;
            Configuration = configuration;
            _logger = logger;
            _smsSender = smsSender;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> UpdateSecurityStamp(UserDTO userDTO)
        {
            AuthResponse authResponse = new AuthResponse();
            try
            {

                ApplicationUser applicationUser = await _userManager.FindByIdAsync(userDTO.Id).ConfigureAwait(false);
                if (applicationUser == null)
                {
                    return NotFound();
                }

                var result = await _userManager.UpdateSecurityStampAsync(applicationUser).ConfigureAwait(false);

                return Ok(result);


            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationUsersRepository.UpdateSecurityStamp Error: {Environment.NewLine}");
                authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                authResponse.Response = Ex.Message;
                return BadRequest(authResponse);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> GenerateEmailConfirmationToken(UserDTO userDTO)
        {
            AuthResponse authResponse = new AuthResponse();
            try
            {
                ApplicationUser applicationUser = await _userManager.FindByIdAsync(userDTO.Id).ConfigureAwait(false);
                if (applicationUser == null)
                {
                    return NotFound();
                }

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser).ConfigureAwait(false);

                return Ok(code);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ApplicationUsersRepository.GenerateEmailConfirmationToken  Error{Environment.NewLine}");
                authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                authResponse.Response = Ex.Message;
                return BadRequest(authResponse);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ConfirmEmail(UserDTO userDTO)
        {
            try
            {
                ApplicationUser applicationUser = await _userManager.FindByIdAsync(userDTO.Id).ConfigureAwait(false);
                if (applicationUser == null)
                {
                    return NotFound();
                }

                var result = await _userManager.ConfirmEmailAsync(applicationUser, userDTO.Code).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ApplicationUsers.ConfirmEmail Error: {Environment.NewLine}");
                return BadRequest();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FindByNameAsync(string UserName)
        {
            AuthResponse authResponse = new AuthResponse();
            try
            {
                ApplicationUser applicationUser = await _userManager.FindByNameAsync(UserName)
                    .ConfigureAwait(false);
                if (applicationUser == null)
                {
                    authResponse.StatusCode = NotFound().StatusCode.ToString(_cultures);
                    authResponse.Response = "User not found";
                    authResponse.ApplicationUser = null;
                    return NotFound(authResponse);
                }

                return Ok(applicationUser);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ApplicationUsersRepository.GenerateChangePhoneNumberTokenAsync  Error{Environment.NewLine}");
                authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                authResponse.Response = Ex.Message;
                return BadRequest(authResponse);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> GenerateTwoFactorTokenAsync(ApplicationUser user, string tokenProvider)
        {
            AuthResponse authResponse = new AuthResponse();
            try
            {
                string code = await _userManager.GenerateTwoFactorTokenAsync(user, "Phone").ConfigureAwait(false);
                if (code == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(code);
                }
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ApplicationUsersRepository.GenerateTwoFactorTokenAsync  Error{Environment.NewLine}");
                authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                authResponse.Response = Ex.Message;
                return BadRequest(authResponse);
            }

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> GetPhoneNumberAsync(ApplicationUser user)
        {
            AuthResponse authResponse = new AuthResponse();
            try
            {
                string telephoneNumber = await _userManager.GetPhoneNumberAsync(user).ConfigureAwait(false);
                if (telephoneNumber == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(telephoneNumber);
                }
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ApplicationUsersRepository.GetPhoneNumberAsync  Error{Environment.NewLine}");
                authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                authResponse.Response = Ex.Message;
                return BadRequest(authResponse);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FindByIdAsync(string ID)
        {
            try
            {
                AuthResponse authResponse = new AuthResponse();
                ApplicationUser applicationUser = await _userManager.FindByIdAsync(ID).ConfigureAwait(false);
                if (applicationUser == null)
                {
                    return NotFound();
                }

                return Ok(applicationUser);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ApplicationUsersRepository.FindByIdAsync Error: {Environment.NewLine}");
                return BadRequest();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FindByEmailAsync(string Email)
        {

            try
            {
                ApplicationUser applicationUser = await _userManager.FindByEmailAsync(Email).ConfigureAwait(false);
                if (applicationUser == null)
                {
                    return NotFound();
                }

                return Ok(applicationUser);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ApplicationUsersRepository.FindByEmailAsync Error: {Environment.NewLine}");
                return BadRequest();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> GenerateChangePhoneNumberTokenAsync(ApplicationUser user, string phoneNumber)
        {
            AuthResponse authResponse = new AuthResponse();
            try
            {
                string code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber).ConfigureAwait(false);
                if (code == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(code);
                }
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ApplicationUsersRepository.GenerateChangePhoneNumberTokenAsync  Error{Environment.NewLine}");
                authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                authResponse.Response = Ex.Message;
                return BadRequest(authResponse);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> CreateAsync(ApplicationUser user, string Password)
        {
            AuthResponse authResponse = new AuthResponse();
            try
            {
                var result = await _userManager.CreateAsync(user, Password).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ApplicationUsersRepository.CreateAsync  Error{Environment.NewLine}");
                authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                authResponse.Response = Ex.Message;
                return BadRequest(authResponse);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> SetTwoFactorEnabledAsync(ApplicationUser user, bool Enabled)
        {
            AuthResponse authResponse = new AuthResponse();
            try
            {
                var result = await _userManager.SetTwoFactorEnabledAsync(user, true).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    authResponse.result = result;
                    authResponse.StatusCode = Ok().StatusCode.ToString(_cultures);
                    return Ok(authResponse);
                }
                else
                {
                    authResponse.result = result;
                    authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                    return BadRequest(authResponse);
                }
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ApplicationUsersRepository.SetTwoFactorEnabledAsync  Error{Environment.NewLine}");
                authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                authResponse.Response = Ex.Message;
                return BadRequest(authResponse);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> CheckPasswordAsync(ApplicationUser user, string Password)
        {
            AuthResponse authResponse = new AuthResponse();
            try
            {
                var isValidPwd = await _userManager.CheckPasswordAsync(user, Password).ConfigureAwait(false);
                if (isValidPwd == true)
                {
                    authResponse.Success = true;
                    authResponse.StatusCode = Ok().StatusCode.ToString(_cultures);
                    return Ok(authResponse);
                }
                else
                {
                    authResponse.Success = false;
                    authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                    return BadRequest(authResponse);
                }
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ApplicationUsersRepository.CreateAsync  Error{Environment.NewLine}");
                authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                authResponse.Response = Ex.Message;
                return BadRequest(authResponse);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ChangePhoneNumberAsync(ApplicationUser user, string PhoneNumber, string Code)
        {
            AuthResponse authResponse = new AuthResponse();
            try
            {
                var result = await _userManager.ChangePhoneNumberAsync(user, PhoneNumber, Code).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    authResponse.Success = true;
                    authResponse.StatusCode = Ok().StatusCode.ToString(_cultures);
                    authResponse.result = result;
                    return Ok(authResponse);
                }
                else
                {
                    authResponse.Success = false;
                    authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                    authResponse.result = result;
                    return BadRequest(authResponse);
                }
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ApplicationUsersRepository.ChangePhoneNumberAsync  Error{Environment.NewLine}");
                authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                authResponse.Response = Ex.Message;
                return BadRequest(authResponse);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ResetPasswordAsync(ApplicationUser user, string Code, string Password)
        {
            AuthResponse authResponse = new AuthResponse();
            try
            {
                //_userManager.IsInRoleAsync
                var result = await _userManager.ResetPasswordAsync(user, Code, Password).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    authResponse.Success = true;
                    authResponse.StatusCode = Ok().StatusCode.ToString(_cultures);
                    authResponse.result = result;
                    return Ok(authResponse);
                }
                else
                {
                    authResponse.Success = false;
                    authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                    authResponse.result = result;
                    return BadRequest(authResponse);
                }
            }
            catch (Exception Ex)

            {
                _logger.LogError(Ex, $"ApplicationUsersRepository.ResetPasswordAsync  Error{Environment.NewLine}");
                authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                authResponse.Response = Ex.Message;
                return BadRequest(authResponse);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            AuthResponse authResponse = new AuthResponse();
            try
            {
                var code = await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
                authResponse.Success = true;
                authResponse.StatusCode = Ok().StatusCode.ToString(_cultures);
                authResponse.Response = code;
                return Ok(authResponse);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ApplicationUsersRepository.ResetPasswordAsync  Error{Environment.NewLine}");
                authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                authResponse.Response = Ex.Message;
                return BadRequest(authResponse);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> IsInRoleAsync(ApplicationUser applicationUser, string role)
        {
            AuthResponse authResponse = new AuthResponse();
            try
            {

                bool result = await _userManager.IsInRoleAsync(applicationUser, role).ConfigureAwait(false);
                authResponse.Success = result;
                authResponse.StatusCode = Ok().StatusCode.ToString(_cultures);
                authResponse.Response = "";
                return Ok(authResponse);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ApplicationUsersRepository.IsInRoleAsync  Error:{Environment.NewLine}");
                authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                authResponse.Response = Ex.Message;
                return BadRequest(authResponse);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> GetRolesAsync(ApplicationUser applicationUser)
        {
            AuthResponse authResponse = new AuthResponse();
            try
            {

                IList<string> myRoles = await _userManager.GetRolesAsync(applicationUser).ConfigureAwait(false);
                return Ok(myRoles);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ApplicationUsersRepository.GetRolesAsync  Error:{Environment.NewLine}");
                authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                authResponse.Response = Ex.Message;
                return BadRequest(authResponse);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> RemoveFromRoleAsync(ApplicationUser applicationUser, string role)
        {
            AuthResponse authResponse = new AuthResponse();
            try
            {

                IdentityResult result = await _userManager.RemoveFromRoleAsync(applicationUser, role).ConfigureAwait(false);
                return Ok(result);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ApplicationUsersRepository.RemoveFromRoleAsync  Error:{Environment.NewLine}");
                authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                authResponse.Response = Ex.Message;
                return BadRequest(authResponse);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddToRoleAsync(ApplicationUser applicationUser, ApplicationRole applicationRole)
        {
            AuthResponse authResponse = new AuthResponse();
            try
            {

                IdentityResult result = await _userManager.AddToRoleAsync(applicationUser, applicationRole.Name).ConfigureAwait(false);
                return Ok(result);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ApplicationUsersRepository.AddToRoleAsync  Error:{Environment.NewLine}");
                authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                authResponse.Response = Ex.Message;
                return BadRequest(authResponse);
            }
        }

        public async Task<IActionResult> GetAllUsersAsync()
        {
            AuthResponse authResponse = new AuthResponse();
            try
            {
                List<ApplicationUser> users = await _context.Users.ToListAsync().ConfigureAwait(false);

                return Ok(users);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ApplicationUsersRepository.AddToRoleAsync  Error:{Environment.NewLine}");
                authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                authResponse.Response = Ex.Message;
                return BadRequest(authResponse);
            }
        }

        public async Task<IActionResult> GetAllUsersAsync(Authority authority)
        {
            AuthResponse authResponse = new AuthResponse();
            try
            {
                List<ApplicationUser> users = await _context.Users
                    .Where(s => s.AuthorityId == authority.ID).ToListAsync().ConfigureAwait(false);

                return Ok(users);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ApplicationUsersRepository.AddToRoleAsync  Error:{Environment.NewLine}");
                authResponse.StatusCode = BadRequest().StatusCode.ToString(_cultures);
                authResponse.Response = Ex.Message;
                return BadRequest(authResponse);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> UpdateUserAsync(ApplicationUser user)
        {
            try
            {
                IdentityResult result = await _userManager.UpdateAsync(user).ConfigureAwait(false);

                return Ok(result);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ApplicationUsersRepository.AddToRoleAsync  Error:{Environment.NewLine}");
                return BadRequest(Ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> GetUsersInRoleAsync(string roleName)
        {
            try
            {
                IList<ApplicationUser> applicationUsers = await _userManager
                    .GetUsersInRoleAsync(roleName)                    
                    .ConfigureAwait(false);

                foreach (ApplicationUser user in applicationUsers)
                {
                    await _context.Entry(user)
                        .Reference(x => x.Authority)
                        .LoadAsync().ConfigureAwait(false);
                }


                return Ok(applicationUsers);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ApplicationUsersRepository.GetUsersInRoleAsync  Error:{Environment.NewLine}");
                return BadRequest(Ex);
            }
        }
    }
}

