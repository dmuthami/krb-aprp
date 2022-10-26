using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using APRP.Web.Domain.Services;
using APRP.Web.ViewModels.Account;
using APRP.Web.ViewModels.ResponseTypes;

namespace APRP.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IIMService _iMService;
        private readonly ICommunicationService _communicationService;
        private readonly ILogger _logger;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender,
            IIMService iMService, ILogger<ForgotPasswordModel> logger,
            ICommunicationService communicationService)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _iMService = iMService;
            _logger = logger;
            _communicationService = communicationService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        //public class InputModel
        //{
        //    [Required]
        //    [EmailAddress]
        //    public string Email { get; set; }
        //}

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var iMresponseMessage = await _iMService.ForgotPassword2(Input).ConfigureAwait(false);
                var objectResult = (ObjectResult)iMresponseMessage.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200",StringComparison.InvariantCultureIgnoreCase))
                {
                    var result = (OkObjectResult)objectResult;
                    AuthResponse authResponse = (AuthResponse)result.Value;
                    Input.Code = authResponse.Response;
                    Input.Id = authResponse.ApplicationUser.Id;
                    SendEmailModel sendEmailModel = new SendEmailModel();
                    sendEmailModel.Email = Input.Email;
                    sendEmailModel.Subject = "Reset Password";

                    var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", id = Input.Id, code = Input.Code },
                    protocol: Request.Scheme);
                    sendEmailModel.HTMLMessage = $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";

                    var communicationResponse = await _communicationService.SendEmail2(sendEmailModel).ConfigureAwait(false);
                    objectResult = (ObjectResult)communicationResponse.IActionResult;
                    if (objectResult.StatusCode.ToString().Equals("200",StringComparison.InvariantCultureIgnoreCase))
                    {
                        string msg = "ForgotPasswordcontroller.OnPostAsync : \r\n" +
                        $"Password reset link has been sent to your email";
                        _logger.LogInformation(msg);
                        return RedirectToPage("./ForgotPasswordConfirmation");
                    }
                    else
                    {
                        string msg = "ForgotPasswordcontroller.OnPostAsync : \r\n" +
                            $" There was an error while sending the Email";
                        _logger.LogInformation(msg);
                        ModelState.AddModelError(string.Empty, msg);
                        return Page();
                    }
                }
                else
                {
                    var result = (BadRequestObjectResult)objectResult;
                    AuthResponse authResponse = (AuthResponse)result.Value;
                    ModelState.AddModelError(string.Empty, authResponse.Response);
                    return Page();
                }
            }

            return Page();
        }
    }
}
