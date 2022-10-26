using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace APRP.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordConfirmation : PageModel
    {
        private readonly ILogger _logger;

        public ForgotPasswordConfirmation(ILogger<ForgotPasswordConfirmation> logger)
        {
            _logger = logger;
        }
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            string msg = "ForgotPasswordConfirmation.OnPostAsync : \r\n" +
                $" Redirect to Sign In page is successful";
            _logger.LogInformation(msg);
            return RedirectToPage("~/Index");
        }

    }
}
