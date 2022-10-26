using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using APRP.Web.ViewModels.DTO;
using System.Globalization;

namespace APRP.Web.Services
{
    public class ApplicationUsersService : IApplicationUsersService
    {
        private readonly IApplicationUsersRepository _applicationUsersRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        private CultureInfo _cultures = new CultureInfo("en-US");

        public ApplicationUsersService(IApplicationUsersRepository applicationUsersRepository, IUnitOfWork unitOfWork
            ,ILogger<ApplicationUsersService> logger)
        {
            _applicationUsersRepository = applicationUsersRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ApplicationUsersResponse> AddToRoleAsync(ApplicationUser applicationUser, ApplicationRole applicationRole)
        {
            try
            {
                var iActionResult = await _applicationUsersRepository.AddToRoleAsync(applicationUser, applicationRole).ConfigureAwait(false);

                return new ApplicationUsersResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationUsers.AddToRoleAsync Error: {Environment.NewLine}");
                return new ApplicationUsersResponse($"Error occured while saving the road record : {Ex.Message}");
            }

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ApplicationUsersResponse> ChangePhoneNumberAsync(ApplicationUser user, string PhoneNumber, string Code)
        {
            try
            {
                var iActionResult = await _applicationUsersRepository.ChangePhoneNumberAsync(user, PhoneNumber, Code).ConfigureAwait(false);

                return new ApplicationUsersResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationUsersService.ChangePhoneNumberAsync {Environment.NewLine}");
                return new ApplicationUsersResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general Exception types", Justification = "<Pending>")]
        public async Task<ApplicationUsersResponse> CheckPasswordAsync(ApplicationUser user, string Password)
        {
            try
            {
                var iActionResult = await _applicationUsersRepository.CheckPasswordAsync(user, Password).ConfigureAwait(false);
                return new ApplicationUsersResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationUsersService.CheckPasswordAsync {Environment.NewLine}");
                return new ApplicationUsersResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ApplicationUsersResponse> ConfirmEmail(UserDTO userDTO)
        {
            try
            {
                var iActionResult = await _applicationUsersRepository.ConfirmEmail(userDTO).ConfigureAwait(false);
                return new ApplicationUsersResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationUsersService.ConfirmEmail {Environment.NewLine}");
                return new ApplicationUsersResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ApplicationUsersResponse> CreateAsync(ApplicationUser user, string Password)
        {
            try
            {
                var iActionResult = await _applicationUsersRepository.CreateAsync(user, Password).ConfigureAwait(false);
                return new ApplicationUsersResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationUsersService.CreateAsync {Environment.NewLine}");
                return new ApplicationUsersResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ApplicationUsersResponse> FindByEmailAsync(string Email)
        {
            try
            {
                var iActionResult = await _applicationUsersRepository.FindByEmailAsync(Email).ConfigureAwait(false);
                return new ApplicationUsersResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationUsersService.FindByEmailAsync {Environment.NewLine}");
                return new ApplicationUsersResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ApplicationUsersResponse> FindByIdAsync(string ID)
        {
            try
            {
                var iActionResult = await _applicationUsersRepository.FindByIdAsync(ID).ConfigureAwait(false);
                return new ApplicationUsersResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationUsersService.FindByIdAsync {Environment.NewLine}");
                return new ApplicationUsersResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ApplicationUsersResponse> FindByNameAsync(string UserName)
        {
            try
            {
                var iActionResult = await _applicationUsersRepository.FindByNameAsync(UserName).ConfigureAwait(false);
                return new ApplicationUsersResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationUsersService.FindByNameAsync {Environment.NewLine}");
                return new ApplicationUsersResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ApplicationUsersResponse> GenerateChangePhoneNumberTokenAsync(ApplicationUser user, string phoneNumber)
        {
            try
            {
                var iActionResult = await _applicationUsersRepository.GenerateChangePhoneNumberTokenAsync(user, phoneNumber).ConfigureAwait(false);
                return new ApplicationUsersResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationUsersService.GenerateChangePhoneNumberTokenAsync {Environment.NewLine}");
                return new ApplicationUsersResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ApplicationUsersResponse> GenerateEmailConfirmationToken(UserDTO userDTO)
        {
            try
            {
                var iActionResult = await _applicationUsersRepository.GenerateEmailConfirmationToken(userDTO).ConfigureAwait(false);
                //await _unitOfWork.CompleteAsync();

                return new ApplicationUsersResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationUsersService.GenerateEmailConfirmationToken {Environment.NewLine}");
                return new ApplicationUsersResponse($"Error occured while generating email confirmation token" +
                    $" : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ApplicationUsersResponse> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            try
            {
                var iActionResult = await _applicationUsersRepository.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);

                return new ApplicationUsersResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationUsersService.GeneratePasswordResetTokenAsync {Environment.NewLine}");
                return new ApplicationUsersResponse($"Error occured while generating email confirmation token" +
                    $" : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general Exception types", Justification = "<Pending>")]
        public async Task<ApplicationUsersResponse> GenerateTwoFactorTokenAsync(ApplicationUser user, string tokenProvider)
        {
            try
            {
                var iActionResult = await _applicationUsersRepository.GenerateTwoFactorTokenAsync(user, tokenProvider).ConfigureAwait(false);
                return new ApplicationUsersResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationUsersService.GenerateTwoFactorTokenAsync {Environment.NewLine}");
                return new ApplicationUsersResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ApplicationUsersResponse> GetAllUsersAsync()
        {
            try
            {
                var iActionResult = await _applicationUsersRepository.GetAllUsersAsync().ConfigureAwait(false);
                return new ApplicationUsersResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationUsersService.GetAllUsersAsync {Environment.NewLine}");
                return new ApplicationUsersResponse($"Error occured while getting all users : {Ex.Message}");
            }
        }

        public async Task<ApplicationUsersResponse> GetAllUsersAsync(Authority authority)
        {
            try
            {
                var iActionResult = await _applicationUsersRepository.GetAllUsersAsync(authority).ConfigureAwait(false);
                return new ApplicationUsersResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationUsersService.GetAllUsersAsync {Environment.NewLine}");
                return new ApplicationUsersResponse($"Error occured while getting all users : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ApplicationUsersResponse> GetPhoneNumberAsync(ApplicationUser user)
        {
            try
            {
                var iActionResult = await _applicationUsersRepository.GetPhoneNumberAsync(user).ConfigureAwait(false);
                return new ApplicationUsersResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationUsersService.GenerateTwoFactorTokenAsync {Environment.NewLine}");
                return new ApplicationUsersResponse($"Error occured while getting user phone number : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ApplicationUsersResponse> GetRolesAsync(ApplicationUser applicationUser)
        {
            try
            {
                var iActionResult = await _applicationUsersRepository.GetRolesAsync(applicationUser).ConfigureAwait(false);
                return new ApplicationUsersResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationUsersService.GetUserRoles {Environment.NewLine}");
                return new ApplicationUsersResponse($"Error occured while getting user phone number : {Ex.Message}");
            }

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ApplicationUsersResponse> GetUsersInRoleAsync(string roleName)
        {
            try
            {
                var iActionResult = await _applicationUsersRepository.GetUsersInRoleAsync(roleName).ConfigureAwait(false);
                return new ApplicationUsersResponse(iActionResult); //Successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationUsersService.GetUsersInRoleAsync {Environment.NewLine}");
                return new ApplicationUsersResponse($"Error occured while getting user phone number : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ApplicationUsersResponse> IsInRoleAsync(ApplicationUser applicationUser, string role)
        {
            try
            {
                var iActionResult = await _applicationUsersRepository.IsInRoleAsync(applicationUser,role).ConfigureAwait(false);
                return new ApplicationUsersResponse(iActionResult); //Successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationUsersService.IsInRoleAsync {Environment.NewLine}");
                return new ApplicationUsersResponse($"Error occured while getting user phone number : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ApplicationUsersResponse> RemoveFromRoleAsync(ApplicationUser applicationUser, string role)
        {
            try
            {
                var iActionResult = await _applicationUsersRepository.RemoveFromRoleAsync(applicationUser, role).ConfigureAwait(false);
                //await _unitOfWork.CompleteAsync();

                return new ApplicationUsersResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationUsersService.RemoveFromRoleAsync Err:{Environment.NewLine}");
                return new ApplicationUsersResponse($"Error occured while getting user phone number : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ApplicationUsersResponse> ResetPasswordAsync(ApplicationUser user, string Code, string Password)
        {
            try
            {
                var iActionResult = await _applicationUsersRepository.ResetPasswordAsync(user, Code, Password).ConfigureAwait(false);
                return new ApplicationUsersResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationUsersService.ResetPasswordTokenAsync {Environment.NewLine}");
                return new ApplicationUsersResponse($"Error occured while getting user phone number : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ApplicationUsersResponse> SetTwoFactorEnabledAsync(ApplicationUser user, bool Enabled)
        {
            try
            {
                var iActionResult = await _applicationUsersRepository.SetTwoFactorEnabledAsync(user, Enabled).ConfigureAwait(false);
                return new ApplicationUsersResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationUsersService.SetTwoFactorEnabledAsync {Environment.NewLine}");
                return new ApplicationUsersResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ApplicationUsersResponse> UpdateSecurityStamp(UserDTO userDTO)
        {
            try
            {
                var iActionResult = await _applicationUsersRepository.UpdateSecurityStamp(userDTO).ConfigureAwait(false);                
                return new ApplicationUsersResponse(iActionResult); //successful
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationUsersService.UpdateSecurityStamp {Environment.NewLine}");
                return new ApplicationUsersResponse($"Error occured while saving the road record : {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ApplicationUsersResponse> UpdateUserAsync(ApplicationUser applicationUser)
        {
            try
            {
                var iActionResult = await _applicationUsersRepository.UpdateUserAsync(applicationUser).ConfigureAwait(false);
                return new ApplicationUsersResponse(iActionResult);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationUsersService.UpdateUserAsync {Environment.NewLine}");
                return new ApplicationUsersResponse($"Error occured while updating the user : {Ex.Message}");
            }
        }
    }
}
