using System.Globalization;
using System.Text.Encodings.Web;
using APRP.Web.ViewModels.Account;
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using APRP.Web.ViewModels;
using APRP.Web.ViewModels.DTO;
using APRP.Web.ViewModels.ResponseTypes;
using APRP.Web.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Controllers
{
    [AllowAnonymous]
    public class ApplicationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly IRegisterService _registerService;
        private readonly IAuthenticateService _authenticateService;
        private readonly IManageService _manageService;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IIMService _iMService;
        private readonly ICommunicationService _communicationService;
        private readonly IUserAccessListService _userAccessListService;
        private readonly ILogger _logger;
        private readonly IAfricastingService _africastingService;

        private IConfiguration Configuration { get; }

        private CultureInfo _cultures = new CultureInfo("en-US");
        public ApplicationController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IRegisterService registerService,
            IAuthenticateService authenticateService,
            IManageService manageService,
            IApplicationUsersService applicationUsersService,
            IIMService iMService,
            ICommunicationService communicationService,
            ILogger<ApplicationController> logger,
            IUserAccessListService userAccessListService, IAfricastingService africastingService)
        {

            _userManager = userManager;
            _signInManager = signInManager;
            _registerService = registerService;
            _logger = logger;
            _authenticateService = authenticateService;
            _manageService = manageService;
            _applicationUsersService = applicationUsersService;
            _communicationService = communicationService;
            _iMService = iMService;
            Configuration = configuration;
            _userAccessListService = userAccessListService;
            _africastingService = africastingService;
        }

        #region Register
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public IActionResult Register()
        {
            try
            {
                ViewBag.Title = "Individual Registration Panel";
                Register register = new Register();
                return View(register);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationController.Register Error: {Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "Sign up individual page has reloaded");
                return View();
            }
        }

        // POST: Application/Sign_Up_Individual
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Register([Bind("Username,Email,PhoneNumber,Password,ConfirmPassword")] Register register)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (register == null)
                    {
                        _logger.LogWarning($"Register Error {Environment.NewLine}");
                        ModelState.AddModelError(string.Empty, "You are not allowed to sign you up." +
                        " Contact System Administrator");
                        return View();
                    }
                    //Check if user is in User Access List
                    var userAccessList = await IsUserinAccessList(register).ConfigureAwait(false);
                    if (userAccessList == null)
                    {
                        string msg = "You are not allowed to sign you up." +
                        " Contact System Administrator";
                        _logger.LogWarning($"{msg}: {Environment.NewLine}");
                        ModelState.AddModelError(string.Empty, msg);
                        return View();
                    }
                    //Assign Authority ID
                    register.AuthorityID = userAccessList.AuthorityId;
                    var registerResponse = await _registerService.AddAsync2(register).ConfigureAwait(false);
                    var objectResult = (ObjectResult)registerResponse.IActionResult;

                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {

                        //Get the user
                        var result2 = (OkObjectResult)objectResult;
                        AuthResponse authResponse = (AuthResponse)result2.Value;
                        ApplicationUser applicationUser = authResponse.ApplicationUser;

                        //Get Token
                        LoginModel loginModel = new LoginModel();
                        loginModel.Username = register.Username;
                        loginModel.Password = register.Password;
                        var authenticateResponse = await _authenticateService.LoginAsync2(loginModel).ConfigureAwait(false);
                        objectResult = (ObjectResult)authenticateResponse.IActionResult;
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            //LoginToken loginToken = JsonConvert.DeserializeObject<LoginToken>(result.Value.ToString(_cultures));
                            authResponse = (AuthResponse)result.Value;
                            LoginToken loginToken = authResponse.LoginToken;

                            //set token session here
                            HttpContext.Session.SetString("tokenz", loginToken.token);

                            //Send Code
                            UserDTO userDTO = new UserDTO();
                            userDTO.Id = applicationUser.Id;
                            //Todo: AddPhone2(userDTO) method should be replaced with AddPhone(userDTO) during production
                            var manageResponse = await _manageService.AddPhone2(userDTO).ConfigureAwait(false);
                            objectResult = (ObjectResult)manageResponse.IActionResult;
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                //redirect to verification page for the code
                                //Set Return URL and store in session
                                //HttpContext.Session.SetString("Referer", Request.Headers["Referer"].ToString());

                                return RedirectToAction("Sign_In_Verification", "Application", new { id = applicationUser.Id, reg = "1" });
                            }
                            else
                            {
                                var result3 = (BadRequestObjectResult)objectResult;
                                authResponse = (AuthResponse)result3.Value;
                                //try sending sms via africasting 
                                Africasting africasting = new Africasting();
                                africasting.body = authResponse.Response;
                                africasting.sendto = applicationUser.PhoneNumber;
                                var africastingResp = await _africastingService.SendSMSToAfricasting(africasting).ConfigureAwait(false);
                                if (africastingResp.Success)
                                {
                                    //ToDo:Do something here
                                }

                                //try sending sms via mobilesasa 
                                MobileSasa mobileSasa = new MobileSasa();
                                mobileSasa.message = authResponse.Response;
                                mobileSasa.phone = applicationUser.PhoneNumber;
                                var mobileSasaResp = await _africastingService.SendSMSViaMobileSasa(mobileSasa).ConfigureAwait(false);
                                if (mobileSasaResp.Success)
                                {
                                    //ToDo:Do something here
                                }

                                /*
                                 * Send code to email if Twilio/SMS is dead
                                 */
                                //Send Email
                                string email = applicationUser.Email;
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
                                    return View();
                                }

                                /*
                                 * End
                                 */
                                //Return page with an error message to the user
                                //Set Return URL and store in session
                                //HttpContext.Session.SetString("Referer", Request.Headers["Referer"].ToString());

                                return RedirectToAction("Sign_In_Verification", "Application", new { id = applicationUser.Id, reg = "1" });
                            }
                        }
                        else
                        {

                            var result3 = (BadRequestObjectResult)objectResult;
                            authResponse = (AuthResponse)result3.Value;
                            ModelState.AddModelError(string.Empty, authResponse.Response);
                            return View();
                        }
                    }
                    else
                    {

                        var result2 = (BadRequestObjectResult)objectResult;
                        AuthResponse authResponse = (AuthResponse)result2.Value;
                        if (authResponse.result == null)
                        {
                            ModelState.AddModelError(string.Empty, authResponse.Response);
                        }
                        else
                        {
                            //loop through the error array list
                            foreach (var error in authResponse.result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }


                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "We are unable to sign you up." +
                           " Ensure required entries are entered correctly");
                    return View();
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"Sign_Up_Individual Error {Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "Sign up individual page has reloaded");
                return View();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<UserAccessList> IsUserinAccessList(Register register)
        {
            try
            {
                var userAccessListResponse = await _userAccessListService.FindByEmailAsync(register.Email).ConfigureAwait(false);
                return userAccessListResponse.UserAccessList;
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"Sign_Up_Individual Error {Environment.NewLine}");
                return null;
            }
        }
        #endregion

        #region Verification

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Sign_In_Verification(string id, string reg = null)
        {
            try
            {
                ApplicationUser user = await GetUser(id).ConfigureAwait(false);
                if (user == null)
                {
                    _logger.LogError("ApplicationController.Sign_In_Verification", "We are unable to complete the registration process." +
                        " Try registering a fresh or contact system admin");
                    ModelState.AddModelError(string.Empty, "We are unable to complete the registration process." +
                        " Try registering a fresh or contact system admin");
                    return View();
                }

                UserDTO userDTO = new UserDTO();
                userDTO.Id = id;
                return View(userDTO);
            }
            catch (Exception Ex)
            {
                string msg = Ex.Message;
                _logger.LogError("Sign_In_Verification Error: \r\n" +
                    $" {msg}");
                ModelState.AddModelError(string.Empty, "Sign in verification page has reloaded");
                return View();
            }
        }

        // POST: Application/Sign_Up_Company
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sign_In_Verification([Bind("Id,Code,Registration")] UserDTO userDTO)
        {
            try
            {
                ApplicationUser user = await GetUser(userDTO.Id).ConfigureAwait(false);

                if (user == null)
                {
                    string msg = "We are unable to complete the registration process." +
                        " Try registering a fresh or contact system admin";
                    _logger.LogError($"ApplicationController.Sign_In_Verification: \r\n {msg}");
                    ModelState.AddModelError(string.Empty, msg);
                    return View();
                }

                /*
                 * verify phone number
                 * 
                 * This happens in the case of Registration process either
                 *  Individually or through Company
                 */
                //if (verificationDTO.Registration == null)
                //{ }


                if (user.PhoneNumberConfirmed == false)
                {
                    var manageResponse = await _manageService.VerifyPhone2(userDTO).ConfigureAwait(false);
                    var objectResult = (ObjectResult)manageResponse.IActionResult;

                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        //await _signInManager.SignInAsync(user, isPersistent: false);
                        string msg = "Continue to verify Phone Number";
                        _logger.LogInformation($"ApplicationController.Sign_In_Verification: \r\n {msg}");

                        ////Verify Email
                        if (user.EmailConfirmed == false)
                        {
                            string msg3 = "Redirect to Send_Confirmation_Code_to_Email";
                            _logger.LogInformation($"ApplicationController.Sign_In_Verification: \r\n {msg3}");
                            //Set Return URL and store in session
                            //HttpContext.Session.SetString("Referer", Request.Headers["Referer"].ToString());

                            return RedirectToAction("Send_Confirmation_Code_to_Email", new { id = user.Id });
                        }

                        //Todo:Bug User phone number is not confirmed but email is confirmed
                        /*User phone number is not confirmed but email is confirmed
                         * Is that even possible..if it is then redirect to dashboard
                         */
                        return View();
                    }
                    else
                    {
                        //Display meaningful error to user
                        _logger.LogError($"ApplicationController.Sign_In_Verification API: \r\n {manageResponse.Message}");
                        ModelState.AddModelError(string.Empty, manageResponse.Message);
                        return View();
                    }
                }
                else
                {
                    /*Verify code
                     * See code block below
                     */

                    // The following code protects for brute force attacks against the two factor codes.
                    // If a user enters incorrect codes for a specified amount of time then the user account
                    // will be locked out for a specified amount of time.
                    var result = await _signInManager.TwoFactorSignInAsync("Phone", userDTO.Code, true, false).ConfigureAwait(false);
                    if (result.Succeeded)
                    {
                        ////Verify Email
                        ///
                        if (user.EmailConfirmed == false)
                        {
                            string msg3 = "Redirect to Send_Confirmation_Code_to_Email";
                            _logger.LogInformation($"ApplicationController.Sign_In_Verification: \r\n {msg3}");
                            //Set Return URL and store in session
                            //HttpContext.Session.SetString("Referer", Request.Headers["Referer"].ToString());

                            return RedirectToAction("Send_Confirmation_Code_to_Email", new { id = user.Id });
                        }

                        string msg = "Redirect to user dashboard";
                        _logger.LogInformation($"ApplicationController.Sign_In_Verification: \r\n {msg}");
                        //Set Return URL and store in session
                        //HttpContext.Session.SetString("Referer", Request.Headers["Referer"].ToString());

                        //return LocalRedirect("/UserDashboard/Dashboard");
                        
                        return RedirectToAction("Dashboard", "Home");
                    }
                    if (result.IsLockedOut)
                    {
                        //_logger.LogWarning(7, "User account locked out.");
                        //return View("Lockout");
                        string msg = "User account locked out. Kindly" +
                            " contact system administrator";
                        _logger.LogError($"ApplicationController.Sign_In_Verification: 7 :\r\n {msg}");
                        ModelState.AddModelError(string.Empty, msg);
                        return View();
                    }
                    else
                    {
                        string msg = "Invalid verification code." +
                        " Re-enter code received or try signing in again";
                        _logger.LogError(string.Empty, msg);
                        ModelState.AddModelError(string.Empty, msg);
                        return View();
                    }
                }
            }
            catch (Exception Ex)
            {
                string msg = $"Error Message: {Ex.Message} \r\n" +
                    $"Inner Exception: {Ex.InnerException.ToString()} \r\n" +
                    $"Stack Trace:  {Ex.StackTrace.ToString(_cultures)}";
                _logger.LogError($"ApplicationController.Sign_In_Verification: \r\n {msg}");
                ModelState.AddModelError(string.Empty, "Sign in verification page has reloaded. " +
                    "Please try again or contact systems administrator");
                return View();
            }

        }


        #endregion

        #region Verify_Email
        //Send Confirmation Code to Email
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Send_Confirmation_Code_to_Email(string id)
        {
            try
            {
                ApplicationUser user = await GetUser(id).ConfigureAwait(false);
                UserDTO userDTO = new UserDTO();
                userDTO.Email = user.Email;
                userDTO.Id = id;
                ViewBag.Title = "Verify & Confirm Email";
                return View(userDTO);
            }
            catch (Exception Ex)
            {
                _logger.LogError("Applicationcontroller.Send_Confirmation_Code_to_Email", Ex);
                ModelState.AddModelError(string.Empty, "Verify Email page has reloaded");
                return View();
            }
        }

        // POST: Application/Send_Confirmation_Code_to_Email
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Send_Confirmation_Code_to_Email([Bind("Id,Email")] UserDTO userDTO)
        {
            try
            {
                ApplicationUser user = await GetUser(userDTO.Id).ConfigureAwait(false);

                //Call Code to update user email if the emails are different
                if (user.Email != userDTO.Email)
                {
                    await UpdateUserEmail(user, userDTO).ConfigureAwait(false);
                }


                var result = await UpdateSecurityStamp(userDTO).ConfigureAwait(false);

                if (!result.Succeeded)
                {
                    string msg = "\r\n";

                    foreach (var error in result.Errors)
                    {
                        msg += $" {error.Description}: \r\n";
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    _logger.LogError($"ApplicationController.Send_Confirmation_Code_to_Email: \r\n {msg}");
                    //return View(userDTO);
                }

                var code = await GenerateEmailConfirmationToken(userDTO).ConfigureAwait(false);
                string token = HttpContext.Session.GetString("tokenz");

                var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new
                {
                    area = "Identity",
                    userId = userDTO.Id,
                    code = code,
                    token = token
                },
                protocol: Request.Scheme);

                SendEmailModel sendEmailModel = new SendEmailModel();
                sendEmailModel.Email = user.Email;
                sendEmailModel.Subject = "Confirm Email";

                sendEmailModel.HTMLMessage = $"Please confirm your Email by " +
                    $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";


                var communicationResponse = await _communicationService.SendEmail2(sendEmailModel).ConfigureAwait(false);
                var objectResult = (ObjectResult)communicationResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (OkObjectResult)objectResult;
                    AuthResponse authResponse = (AuthResponse)result2.Value;
                    //Set Return URL and store in session
                    //HttpContext.Session.SetString("Referer", Request.Headers["Referer"].ToString());

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "There was an error while sending the Email");
                    return View();
                }
            }
            catch (ArgumentNullException ANE)
            {
                _logger.LogError("ApplicationController.Send_Confirmation_Code_to_Email \r\n", ANE);
                ModelState.AddModelError(string.Empty, "Redirect to Home page fails");
                return View();
            }
            catch (Exception Ex)
            {
                _logger.LogError("ApplicationController.Send_Confirmation_Code_to_Email \r\n", Ex);
                ModelState.AddModelError(string.Empty, Ex.Message);
                return View();
            }
        }

        private async Task UpdateUserEmail(ApplicationUser user, UserDTO userDTO)
        {
            try
            {
                user.EmailConfirmed = false;
                user.Email = userDTO.Email;

                var registerResponse = await _registerService.UpdateAsync(user).ConfigureAwait(false);
                var objectResult = (ObjectResult)registerResponse.IActionResult;
                if (!objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (BadRequestObjectResult)objectResult;
                    AuthResponse authResponse = (AuthResponse)result2.Value;
                    throw new NotSupportedException("Failed to Update Email Address");
                }
            }
            catch (Exception Ex)
            {
                throw new Exception($"Failed to Update Email Address : {Ex.Message}");
            }
        }
        #endregion

        #region Logout
        public IActionResult Logout()
        {
            ViewBag.Title = "Log Out";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
            HttpContext.Response.Cookies.Delete(".AspNetCore.Cookies");
            //set token session here
            HttpContext.Session.SetString("tokenz", String.Empty);
            //await _signInManager.SignOutAsync().ConfigureAwait(false);
            //_logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                //Set Return URL and store in session
                //HttpContext.Session.SetString("Referer", Request.Headers["Referer"].ToString());

                return RedirectToAction("Index", "Home");
            }
        }

        #endregion

        #region Common

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<ApplicationUser> GetUser(string id)
        {
            try
            {
                ApplicationUser applicationUser = null;

                var applicationUsersResponse = await _applicationUsersService.FindByIdAsync(id).ConfigureAwait(false);
                var objectResult = (ObjectResult)applicationUsersResponse.IActionResult;


                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result = (OkObjectResult)objectResult;
                    applicationUser = (ApplicationUser)result.Value;
                }

                return applicationUser;
            }
            catch (Exception Ex)
            {
                _logger.LogError($"ApplicationController.GetUser \r\n : " +
                    $"{Ex.Message}");
                return null;
            }
        }

        private async Task<IdentityResult> UpdateSecurityStamp(UserDTO userDTO)
        {
            try
            {
                IdentityResult identityResult = null;

                var applicationUsersResponse = await _applicationUsersService.UpdateSecurityStamp(userDTO).ConfigureAwait(false);
                var objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result = (OkObjectResult)objectResult;
                    identityResult = (IdentityResult)result.Value;
                }
                return identityResult;
            }
            catch (Exception Ex)
            {
                _logger.LogError($"ApplicationController.UpdateSecurityStamp \r\n : " +
                    $"{Ex.Message}");
                return null;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<string> GenerateEmailConfirmationToken(UserDTO userDTO)
        {
            try
            {
                string code = null;

                var applicationUsersResponse = await _applicationUsersService.GenerateEmailConfirmationToken(userDTO).ConfigureAwait(false);
                var objectResult = (ObjectResult)applicationUsersResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result = (OkObjectResult)objectResult;
                    code = (string)result.Value;
                }
                return code;
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationController.GenerateEmailConfirmationToken : " +
                    $"{Environment.NewLine}");
                return null;
            }
        }

        #endregion

        #region Communication

        private async Task sendEmail(string Email, string Subject, string Message)
        {
            try
            {
                SendEmailModel _SendEmailModel = new SendEmailModel();
                _SendEmailModel.Email = Email;
                _SendEmailModel.Subject = Subject;
                _SendEmailModel.HTMLMessage = Message;

                var communicationResponse = await _communicationService.SendEmail2(_SendEmailModel).ConfigureAwait(false);
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

                    //Email Sending Failed
                    _logger.LogError($"SendEmail: {authResponse.Response}");
                }

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationController.SendEmail Error{Environment.NewLine}");
                throw;
            }
        }

        #endregion

        #region Activate or Deactivate
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.Administrator.ActivateOrDeactivate)]
        public async Task<JsonResult> ActivateOrDeactivate(string UserId)
        {
            try
            {
                string msg = null;
                bool success = false;

                //Get user
                ApplicationUser applicationUser = null;
                var resp = await _applicationUsersService.FindByIdAsync(UserId).ConfigureAwait(false);

                var objectResult = (ObjectResult)resp.IActionResult;

                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    //Get the user
                    var result = (OkObjectResult)objectResult;
                    applicationUser = (ApplicationUser)result.Value;
                    success = true;
                    msg = "Success";
                } else
                {
                    success = false;
                    msg = "User cannot be found";
                }


                //check user is not null
                if (applicationUser!=null)
                {
                    //Is lockut end enabled, if yes then disable
                    //Else enable
                    if (applicationUser.LockoutEnd==null)
                    {
                        //Deactivate
                        applicationUser.LockoutEnd = DateTime.MaxValue;
                        resp = await _applicationUsersService.UpdateUserAsync(applicationUser).ConfigureAwait(false);
                        success = true;
                        msg = "Success";
                    }
                    else
                    {
                        if (applicationUser.LockoutEnd<DateTime.Now)
                        {
                            //Deactivate
                            applicationUser.LockoutEnd = DateTime.MaxValue;
                        }else
                        {
                            //Activate
                            applicationUser.LockoutEnd = null;
                        }
                        resp = await _applicationUsersService.UpdateUserAsync(applicationUser).ConfigureAwait(false);
                        success = true;
                        msg = "Success";
                    }
                }


                return Json(new
                {
                    Success = success,
                    Message = msg,
                    Href = $"/Users/Index"
                });

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ApplicationController.ActivateOrDeactivate Error {Environment.NewLine}");
                return Json(new
                {
                    Success = true,
                    Message = "Fail",
                    Href = $"/Users/Index"
                });
            }
        }

        #endregion
    }
}