using Microsoft.AspNetCore.Authorization;
using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using APRP.Web.ViewModels.DTO;
using APRP.Web.Domain.Services;

namespace APRP.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        private readonly IRegisterService _registerService;
        private readonly IApplicationUsersService _applicationUsersService;

        public ConfirmEmailModel(UserManager<ApplicationUser> userManager,
            ILogger<ConfirmEmailModel> logger,
            IRegisterService registerService,
            IApplicationUsersService applicationUsersService)
        {
            _userManager = userManager;
            _logger = logger;
            _registerService = registerService;
            _applicationUsersService = applicationUsersService;
        }

        [TempData]
        public string StatusMessage { get; set; }

        private string _token { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code, string token)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/UserDashboard/Dashboard");
            }

            _token = token;

            var user = await GetUser(userId).ConfigureAwait(false);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            UserDTO userDTO = new UserDTO();
            userDTO.Id = user.Id;
            userDTO.Code = code;
            userDTO.token = token;

            var result = await ConfirmEmail(userDTO).ConfigureAwait(false);
            StatusMessage = result;
            return Page();
        }

        #region Custom
        private async Task<ApplicationUser> GetUser(string id)
        {
            try
            {
                ApplicationUser applicationUser = null;

                var applicationUserResponse = await _applicationUsersService.FindByIdAsync(id).ConfigureAwait(false);
                var objectResult = (ObjectResult)applicationUserResponse.IActionResult;
                var result = (OkObjectResult)objectResult;
                applicationUser = (ApplicationUser)result.Value;
               
                return applicationUser;
            }
            catch (Exception Ex)
            {
                _logger.LogError($"ConfirmEmail.GetUser \r\n : " +
                    $"{Ex.Message}");
                return null;
            }
        }

        private async Task<string> ConfirmEmail(UserDTO userDTO)
        {
            try
            {
                string response = null;
                IdentityResult identityResult = null;

                var applicationUsersResponse = await _applicationUsersService.ConfirmEmail(userDTO);
                var objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200",StringComparison.InvariantCultureIgnoreCase))
                {
                    var result = (OkObjectResult)objectResult;
                    identityResult = (IdentityResult)result.Value;
                    response = "Thank you for confirming your email";
                } else
                {
                    response = "Error confirming your email.";
                }
                
                return response;
            }
            catch (Exception Ex)
            {
                _logger.LogError($"ConfirmEmailModel.ConfirmEmail \r\n : " +
                    $"{Ex.Message}");
                return null;
            }
        }

        #endregion
    }
}
