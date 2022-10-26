using APRP.Web.ViewModels.Account;
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using APRP.Web.ViewModels;
using APRP.Web.ViewModels.DTO;
using APRP.Web.ViewModels.ResponseTypes;
using APRP.Web.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace APRP.Web.MyPages
{
    public class IndexModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthenticateService _authenticateService;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IManageService _manageService;
        private readonly ICommunicationService _communicationService;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly IAfricastingService _africastingService;
        private IConfiguration Configuration { get; }
        public IndexModel(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager, IConfiguration configuration,
            IAuthenticateService authenticateService,
            IApplicationUsersService applicationUsersService,
            IManageService manageService,
            ICommunicationService communicationService,
             IEmailSender emailSender,
            ILogger<IndexModel> logger, IAfricastingService africastingService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _authenticateService = authenticateService;
            _applicationUsersService = applicationUsersService;
            _logger = logger;
            _manageService = manageService;
            _communicationService = communicationService;
            _emailSender = emailSender;
            Configuration = configuration;
            _africastingService = africastingService;
        }

        [BindProperty]
        public LoginModel LoginModel { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
            //await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme).ConfigureAwait(true);
            HttpContext.Response.Cookies.Delete(".AspNetCore.Cookies");
            return Page();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    /*
                     * STEP 1
                     */
                    // This doesn't count login failures towards account lockout
                    // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                    var result = await _signInManager.PasswordSignInAsync
                    (LoginModel.Username,
                    LoginModel.Password,
                    LoginModel.RememberMe,
                    lockoutOnFailure: true).ConfigureAwait(true);

                    //Get Token               
                    var authenticateResponse = await _authenticateService.LoginAsync2(LoginModel).ConfigureAwait(true);
                    var objectResult = (ObjectResult)authenticateResponse.IActionResult;
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result2 = (OkObjectResult)objectResult;
                        AuthResponse authResponse = (AuthResponse)result2.Value;
                        LoginToken loginToken = authResponse.LoginToken;

                        //set token session here
                        HttpContext.Session.SetString("tokenz", loginToken.token);

                    }
                    else
                    {
                        //The username exists. Return error
                        AuthResponse authResponse = (AuthResponse)objectResult.Value;
                        ModelState.AddModelError(string.Empty, authResponse.Response);
                        return Page();
                    }

                    if (result.Succeeded)
                    {
                        //Get user
                        ApplicationUser user = null;
                        var applicationUsersResponse = await _applicationUsersService.FindByNameAsync(LoginModel.Username).ConfigureAwait(true);
                        objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result2 = (OkObjectResult)objectResult;
                            user = (ApplicationUser)result2.Value;
                        }
                        if (user == null)
                        {
                            string msg = "User couldn't be retrieved.";
                            _logger.LogError($"HomeController.SignIn : {msg}");
                            ModelState.AddModelError(string.Empty, msg);
                            return Page();
                        }

                        //Check user phone number
                        string code = null; string _PhoneNumber = null;
                        applicationUsersResponse = await _applicationUsersService.GetPhoneNumberAsync(user).ConfigureAwait(true);
                        objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result2 = (OkObjectResult)objectResult;
                            _PhoneNumber = (string)result2.Value;
                        }
                        if (_PhoneNumber == null)
                        {
                            string msg = "User telephone couldn't be retrieved.";
                            _logger.LogError($"HomeController.SignIn : {msg}");
                            ModelState.AddModelError(string.Empty, msg);
                            return Page();
                        }

                        //Generate token/code and Send Code
                        if (user.PhoneNumberConfirmed == true)
                        {
                            //Generate the token/code
                            applicationUsersResponse = await _applicationUsersService.GenerateTwoFactorTokenAsync(user, "Phone").ConfigureAwait(false);
                            objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var result2 = (OkObjectResult)objectResult;
                                code = (string)result2.Value;
                            }
                            if (code == null)
                            {
                                string msg = "Failed to generate OTP code";
                                _logger.LogError($"HomeController.SignIn : {msg}");
                                ModelState.AddModelError(string.Empty, msg);
                                return Page();
                            }

                            //send code/token
                            UserDTO userDTO = new UserDTO();
                            userDTO.Id = user.Id;
                            userDTO.Code = code;
                            userDTO.PhoneNumber = _PhoneNumber;

                            var manageRespone = await _manageService.SendCode3(userDTO).ConfigureAwait(true);
                            objectResult = (ObjectResult)manageRespone.IActionResult;
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var result2 = (OkObjectResult)objectResult;
                                AuthResponse authResponse = (AuthResponse)result2.Value;
                                //Twilio/SMS is working
                            }
                            else
                            {
                                var result2 = (BadRequestObjectResult)objectResult;
                                AuthResponse authResponse = (AuthResponse)result2.Value;

                                //check if africasting is true and send
                                bool useAfricasting = false;
                                bool resultAfricasting = bool.TryParse(Configuration.GetSection("AfricastingSettings")["SendSMS"], out useAfricasting);
                                if (resultAfricasting==true)
                                {
                                    Africasting africasting = new Africasting();
                                    africasting.body = authResponse.Response;
                                    africasting.sendto = user.PhoneNumber;
                                    var africastingResp = await _africastingService.SendSMSToAfricasting(africasting).ConfigureAwait(false);
                                    if (africastingResp.Success)
                                    {
                                        //ToDo:Do something here
                                    }
                                }

                                /*
                                 * Send code to email if Twilio/SMS is dead
                                 */
                                //Send Email
                                string email = user.Email;
                                string subject = "Verification Code";
                                //check configuration code
                                bool SendCodeViaEmail = false;
                                bool resultCode = bool.TryParse(Configuration.GetSection("SendGridSettings")["SendCodeViaEmail"], out SendCodeViaEmail);
                                if (SendCodeViaEmail == true)
                                {
                                    await sendEmail(email, subject, authResponse.Response).ConfigureAwait(true);
                                }
                                /*
                                 * End
                                 */
                            }
                            //Set Return URL and store in session
                            //HttpContext.Session.SetString("Referer", Request.Headers["Referer"].ToString());
                            return RedirectToAction("Sign_In_Verification", "Application", new { id = user.Id });
                        }
                        else
                        {
                            //send code
                            UserDTO userDTO = new UserDTO();
                            userDTO.Id = user.Id;
                            userDTO.Code = code;
                            userDTO.PhoneNumber = _PhoneNumber;
                            var manageResponse = await _manageService.AddPhone2(userDTO).ConfigureAwait(false);

                            objectResult = (ObjectResult)manageResponse.IActionResult;
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var result2 = (OkObjectResult)objectResult;
                                AuthResponse authResponse = (AuthResponse)result2.Value;
                            }
                            else
                            {
                                var result2 = (BadRequestObjectResult)objectResult;
                                AuthResponse authResponse = (AuthResponse)result2.Value;

                                //try sending sms via africasting 
                                Africasting africasting = new Africasting();
                                africasting.body = authResponse.Response;
                                africasting.sendto = user.PhoneNumber;
                                var africastingResp = await _africastingService.SendSMSToAfricasting(africasting).ConfigureAwait(false);
                                if (africastingResp.Success)
                                {
                                    //ToDo:Do something here
                                }
                                //try sending sms via mobilesasa 
                                MobileSasa mobileSasa = new MobileSasa();
                                mobileSasa.message = authResponse.Response;
                                mobileSasa.phone = user.PhoneNumber;
                                var mobileSasaResp = await _africastingService.SendSMSViaMobileSasa(mobileSasa).ConfigureAwait(false);
                                if (mobileSasaResp.Success)
                                {
                                    //ToDo:Do something here
                                }
                                /*
                                 * Send code to email if Twilio is dead
                                 */
                                string email = user.Email;
                                string subject = "Verification Code";
                                try
                                {
                                    bool SendCodeViaEmail = false;
                                    bool resultCode = bool.TryParse(Configuration.GetSection("SendGridSettings")["SendCodeViaEmail"], out SendCodeViaEmail);
                                    if (SendCodeViaEmail == true)
                                    {
                                        await sendEmail(email, subject, authResponse.Response).ConfigureAwait(false);
                                    }
                                }
                                catch (Exception Ex)
                                {

                                    string msg = $"Failed to send verification code to your email{Ex.Message}";
                                    _logger.LogError($"Index2Model.OnGetAsync : {msg}");
                                    ModelState.AddModelError(string.Empty, msg);
                                    return Page();
                                }
                            }
                            //Set Return URL aand store in session
                            //HttpContext.Session.SetString("Referer", Request.Headers["Referer"].ToString());

                            return RedirectToAction("Sign_In_Verification", "Application", new { id = user.Id });
                        }

                    }

                    if (result.RequiresTwoFactor)
                    {
                        //Get user
                        ApplicationUser user = null;
                        var applicationUsersResponse = await _applicationUsersService.FindByNameAsync(LoginModel.Username).ConfigureAwait(false);
                        objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result2 = (OkObjectResult)objectResult;
                            user = (ApplicationUser)result2.Value;
                        }
                        if (user == null)
                        {
                            string msg = "User couldn't be retrieved.";
                            _logger.LogError($"HomeController.SignIn : {msg}");
                            ModelState.AddModelError(string.Empty, msg);
                            return Page();
                        }

                        //Check user phone number
                        string code = null; string _PhoneNumber = null;
                        applicationUsersResponse = await _applicationUsersService.GetPhoneNumberAsync(user).ConfigureAwait(false);
                        objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result2 = (OkObjectResult)objectResult;
                            _PhoneNumber = (string)result2.Value;
                        }
                        if (_PhoneNumber == null)
                        {
                            string msg = "User telephone couldn't be retrieved.";
                            _logger.LogError($"HomeController.SignIn : {msg}");
                            ModelState.AddModelError(string.Empty, msg);
                            return Page();
                        }

                        ////Generate token/code and Send Code
                        if (user.PhoneNumberConfirmed == true)
                        {
                            //Generate the token/code 
                            applicationUsersResponse = await _applicationUsersService.GenerateTwoFactorTokenAsync(user, "Phone").ConfigureAwait(false);
                            objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var result2 = (OkObjectResult)objectResult;
                                code = (string)result2.Value;
                            }
                            if (code == null)
                            {
                                string msg = "Failed to generate OTP code";
                                _logger.LogError($"HomeController.SignIn : {msg}");
                                ModelState.AddModelError(string.Empty, msg);
                                return Page();
                            }

                            //send code/token
                            UserDTO userDTO = new UserDTO();
                            userDTO.Id = user.Id;
                            userDTO.Code = code;
                            userDTO.PhoneNumber = _PhoneNumber;

                            var manageRespone = await _manageService.SendCode3(userDTO).ConfigureAwait(false);
                            objectResult = (ObjectResult)manageRespone.IActionResult;
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var result2 = (OkObjectResult)objectResult;
                                AuthResponse authResponse = (AuthResponse)result2.Value;

                                //Twilio/SMS is working
                            }
                            else
                            {
                                var result2 = (BadRequestObjectResult)objectResult;
                                AuthResponse authResponse = (AuthResponse)result2.Value;

                                //try sending sms via africasting 
                                Africasting africasting = new Africasting();
                                africasting.body = authResponse.Response;
                                africasting.sendto = user.PhoneNumber;
                                var africastingResp = await _africastingService.SendSMSToAfricasting(africasting).ConfigureAwait(false);
                                if (africastingResp.Success)
                                {
                                    //ToDo:Do something here
                                }

                                //try sending sms via mobilesasa 
                                MobileSasa mobileSasa = new MobileSasa();
                                mobileSasa.message = authResponse.Response;
                                mobileSasa.phone = user.PhoneNumber;
                                var mobileSasaResp = await _africastingService.SendSMSViaMobileSasa(mobileSasa).ConfigureAwait(false);
                                if (mobileSasaResp.Success)
                                {
                                    //ToDo:Do something here
                                }
                                /*
                                 * Send code to email if Twilio/SMS is dead
                                 */
                                //Send Email
                                string email = user.Email;
                                string subject = "Verification Code";
                                try
                                {
                                    bool SendCodeViaEmail = false;
                                    bool resultCode = bool.TryParse(Configuration.GetSection("SendGridSettings")["SendCodeViaEmail"], out SendCodeViaEmail);
                                    if (SendCodeViaEmail == true)
                                    {
                                        await sendEmail(email, subject, authResponse.Response).ConfigureAwait(false);
                                    }
                                }
                                catch (Exception Ex)
                                {

                                    string msg = $"Failed to send verification code to your email{Ex.Message}";
                                    _logger.LogError($"Index2Model.OnGetAsync : {msg}");
                                    ModelState.AddModelError(string.Empty, msg);
                                    return Page();
                                }

                                /*
                                 * End
                                 */
                            }
                            //Set Return URL aand store in session
                            //HttpContext.Session.SetString("Referer", Request.Headers["Referer"].ToString());

                            return RedirectToAction("Sign_In_Verification", "Application", new { id = user.Id });
                        }
                        else
                        {
                            //send code
                            UserDTO userDTO = new UserDTO();
                            userDTO.Id = user.Id;
                            userDTO.Code = code;
                            userDTO.PhoneNumber = _PhoneNumber;
                            var manageResponse = await _manageService.AddPhone2(userDTO).ConfigureAwait(false);

                            objectResult = (ObjectResult)manageResponse.IActionResult;
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var result2 = (OkObjectResult)objectResult;
                                AuthResponse authResponse = (AuthResponse)result2.Value;
                            }
                            else
                            {
                                var result2 = (BadRequestObjectResult)objectResult;
                                AuthResponse authResponse = (AuthResponse)result2.Value;

                                //try sending sms via africasting 
                                Africasting africasting = new Africasting();
                                africasting.body = authResponse.Response;
                                africasting.sendto = user.PhoneNumber;
                                var africastingResp = await _africastingService.SendSMSToAfricasting(africasting).ConfigureAwait(false);
                                if (africastingResp.Success)
                                {
                                    //ToDo:Do something here
                                }
                                //try sending sms via mobilesasa 
                                MobileSasa mobileSasa = new MobileSasa();
                                mobileSasa.message = authResponse.Response;
                                mobileSasa.phone = user.PhoneNumber;
                                var mobileSasaResp = await _africastingService.SendSMSViaMobileSasa(mobileSasa).ConfigureAwait(false);
                                if (mobileSasaResp.Success)
                                {
                                    //ToDo:Do something here
                                }
                                /*
                                 * Send code to email if Twilio is dead
                                 */
                                string email = user.Email;
                                string subject = "Verification Code";
                                bool SendCodeViaEmail = false;
                                bool resultCode = bool.TryParse(Configuration.GetSection("SendGridSettings")["SendCodeViaEmail"], out SendCodeViaEmail);
                                if (SendCodeViaEmail == true)
                                {
                                    await sendEmail(email, subject, authResponse.Response).ConfigureAwait(false);
                                }
                            }
                            //Set Return URL aand store in session
                            //HttpContext.Session.SetString("Referer", Request.Headers["Referer"].ToString());
                            return RedirectToAction("Sign_In_Verification", "Application", new { id = user.Id });
                        }
                    }

                    if (result.IsLockedOut)
                    {
                        ModelState.AddModelError(string.Empty, "User account locked out.");
                        // _logger.LogWarning(2, "User account locked out.");
                        return Page();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt. Check you supplied the correct username" +
                            " and password");
                        try
                        {
                            // Clear the existing external cookie to ensure a clean login process
                            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
                            HttpContext.Response.Cookies.Delete(".AspNetCore.Cookies");
                            //await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                        }
                        catch (Exception Ex)
                        {

                            ModelState.AddModelError(string.Empty, Ex.Message);
                            return Page();
                        }
                        return Page();
                    }

                }

                // If we got this far, something failed, redisplay form
                //return LocalRedirect(returnUrl);
                return Page();
            }
            catch (Exception Ex)
            {
                string msg = $"Home.Index Error : {Ex.Message}";
                _logger.LogError(msg);
                ModelState.AddModelError(string.Empty, "Invalid login attempt. Please consult system administrator");
                return Page();
            }
        }

        #region Communication

        private async Task sendEmail(string Email, string Subject, string Message)
        {
            try
            {
                SendEmailModel _SendEmailModel = new SendEmailModel();
                _SendEmailModel.Email = Email;
                _SendEmailModel.Subject = Subject;
                _SendEmailModel.HTMLMessage = Message;


                var communicationResponse = await _communicationService.SendEmail2(_SendEmailModel).ConfigureAwait(true);
                var objectResult = (ObjectResult)communicationResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (OkObjectResult)objectResult;
                    AuthResponse authResponse = (AuthResponse)result2.Value;

                    //Email sending succcesfully
                    _logger.LogInformation($"SendEmail: {authResponse.Response}");
                }
                else
                {
                    var result2 = (BadRequestObjectResult)objectResult;
                    AuthResponse authResponse = (AuthResponse)result2.Value;

                    //Email sending succcesfully
                    _logger.LogError($"SendEmail: {authResponse.Response}");
                }

            }
            catch (Exception Ex)
            {
                _logger.LogError($"SendEmail Error{Ex.Message}");
                throw;
            }
        }

        #endregion
    }
}
