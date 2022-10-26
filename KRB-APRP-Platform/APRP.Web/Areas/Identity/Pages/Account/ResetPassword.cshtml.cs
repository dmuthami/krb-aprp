using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using APRP.Web.Domain.Services;
using APRP.Web.ViewModels.Account;
using APRP.Web.ViewModels.ResponseTypes;

namespace APRP.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IIMService _iMService;

        public ResetPasswordModel(UserManager<ApplicationUser> userManager,
            ILogger<ResetPasswordModel> logger, IApplicationUsersService applicationUsersService, IIMService iMService)
        {
            _userManager = userManager;
            _logger = logger;
            _applicationUsersService = applicationUsersService;
            _iMService = iMService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            public string Id { get; set; }

            [Required]
            [Display(Name = "Username")]
            public string UserName { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Enter New Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm New Password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string Code { get; set; }
        }

        public async Task< IActionResult> OnGetAsync(string id,string code = null )
        {
            if (code == null)
            {
                string msg = "A code must be supplied for password reset.";
                _logger.LogError("ResetPasswordModel.OnGet Error: \r\n" +
                    $" {msg}");
                return BadRequest(msg);
            }
            else
            {
                //Get user using ID
                ApplicationUser user = null;
               var applicationUsersResponse = await _applicationUsersService.FindByIdAsync(id).ConfigureAwait(false);
                var objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200",StringComparison.InvariantCultureIgnoreCase))
                {
                    var result = (OkObjectResult)objectResult;
                    user = (ApplicationUser)result.Value;
                }

                if (user == null)
                {
                    string msg2 = "User cannot be found.Contact System Admin";
                    _logger.LogError("ResetPasswordModel.OnGet Error: \r\n" +
                        $" {msg2}");
                    return BadRequest(msg2);
                }

                Input = new InputModel
                {
                    //Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                    Code = code,
                     Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName
                };
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                string msg = "Invalid NodelState";
                _logger.LogError("ResetPasswordModel.OnPostAsync Error: \r\n" +
                    $" {msg}");
                return Page();
            }

            
            ApplicationUser user = null;
            var applicationUsersResponse = await _applicationUsersService.FindByEmailAsync(Input.Email);
            var objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
            if (objectResult.StatusCode.ToString().Equals("200",StringComparison.InvariantCultureIgnoreCase))
            {
                var result2 = (OkObjectResult)objectResult;
                user = (ApplicationUser)result2.Value;
            }
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToPage("./ResetPasswordConfirmation");
            }
            ResetModel resetModel = new ResetModel();
            resetModel.Id = Input.Id;
            resetModel.Code = Input.Code;
            resetModel.Email = Input.Email;
            resetModel.Password = Input.Password;
            resetModel.ConfirmPassword = Input.ConfirmPassword;
            resetModel.UserName = Input.UserName;

            var iMResponse = await _iMService.ResetPassword2(resetModel);
            objectResult = (ObjectResult)iMResponse.IActionResult;
            if (objectResult.StatusCode.ToString().Equals("200",StringComparison.InvariantCultureIgnoreCase))
            {
                return RedirectToPage("./ResetPasswordConfirmation");
            }
            else
            {
                var result2 = (BadRequestObjectResult)objectResult;
                AuthResponse authResponse = (AuthResponse)result2.Value;
                foreach (var error in authResponse.result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return Page();
        }
    }
}
