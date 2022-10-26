using APRP.Web.Helpers;
using APRP.Web.Models.UserViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace APRP.Web.Controllers
{
    public class ApplicationAPIController : Controller
    {
        public IConfiguration Configuration { get; }
        APRPAPI _aPRPAPI;
        private TokenAPI tokenAPI = new TokenAPI();
        public ApplicationAPIController(IConfiguration configuration)
        {
            _aPRPAPI = new APRPAPI();
            Configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Sign In
        public IActionResult SignIn(string? message)
        {

            LoginModel loginModel = new LoginModel();
            // Clear the existing external cookie to ensure a clean login process
            HttpContext.Session.SetString("tokenz", String.Empty);

            ViewBag.Title = "Sign In";
            if (string.IsNullOrEmpty(message))
            {
                loginModel.message = message;
            }

            return View(loginModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn([Bind("username,password,sendOTPtoEmail")] LoginModel loginModel)
        {
            try
            {
                string returnUrl = Url.Content("~/");

                if (ModelState.IsValid)
                {

                    //get the token
                    string urlToken = "http://localhost:8010/";

                    HttpClient clientToken = _aPRPAPI.InitializeClient(urlToken);
                    var contentToken = new StringContent(JsonConvert.SerializeObject(loginModel), Encoding.UTF8, "application/json");
                    HttpResponseMessage resToken = clientToken.PostAsync("api/user/login/", contentToken).Result;
                    if (resToken.IsSuccessStatusCode)
                    {
                        //process the json
                        var cntToken = await resToken.Content.ReadAsStringAsync();
                        dynamic response = JObject.Parse(cntToken);

                        long userid = response.data.userid;
                        string phone = response.data.phone;
                        return RedirectToAction("Sign_In_Verification", "Application", new
                        {
                            userid = userid,
                            phone = phone
                        });

                    }
                    else
                    {
                        var cnt = await resToken.Content.ReadAsStringAsync();
                        dynamic response = JObject.Parse(cnt);
                        if (resToken.StatusCode == HttpStatusCode.BadRequest)
                        {
                            ModelState.AddModelError(string.Empty, response.message);

                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, $"Response : {response.message}");
                        }
                        return View();
                    }

                }

                // If we got this far, something failed, redisplay form
                //return LocalRedirect(returnUrl);
                return View();
            }
            catch (Exception Ex)
            {
                ModelState.AddModelError(string.Empty, "We couldn't sign you up. Kindly contact the system admin");
                return View();

            }
        }

        #endregion

        #region Sign in Verification
        public IActionResult Sign_In_Verification(long userid, string phone)
        {
            RegistrationDTO verificationDTO = new RegistrationDTO();
            verificationDTO.userid = userid;
            verificationDTO.phone = phone;
            return View(verificationDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sign_In_Verification([Bind("userid,otp,phone")] RegistrationDTO verificationDTO)
        {
            try
            {
                string returnUrl = Url.Content("~/");

                if (ModelState.IsValid)
                {

                    //get the token
                    string urlToken = "http://localhost:8010/";

                    HttpClient clientToken = _aPRPAPI.InitializeClient(urlToken);
                    var contentToken = new StringContent(JsonConvert.SerializeObject(verificationDTO), Encoding.UTF8, "application/json");
                    HttpResponseMessage resToken = clientToken.PostAsync("api/user/validateloginotp/", contentToken).Result;
                    if (resToken.IsSuccessStatusCode)
                    {
                        //process the json
                        var cntToken = await resToken.Content.ReadAsStringAsync();
                        dynamic response = JObject.Parse(cntToken);

                        //set token session here
                        string tkn = response.token;
                        HttpContext.Session.SetString("tokenz", tkn);
                        Console.WriteLine($"token:{tkn}");
                        return RedirectToAction("Index", "Home");

                    }
                    else
                    {
                        var cnt = await resToken.Content.ReadAsStringAsync();
                        dynamic response = JObject.Parse(cnt);
                        if (resToken.StatusCode == HttpStatusCode.BadRequest)
                        {
                            ModelState.AddModelError(string.Empty, response.message);

                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, $"Response : {response.message}");
                        }
                        return View();
                    }

                }
                return View();
            }
            catch (Exception Ex)
            {
                ModelState.AddModelError(string.Empty, "We couldn't sign you up. Kindly contact the system admin");
                return View();

            }
        }
        #endregion

        #region  Registration
        public IActionResult ValidatePhoneSendOTP()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidatePhoneSendOTP([Bind("phone,email,otp")] ValidatePhoneSendOTP validatePhoneSendOTP)
        {
            try
            {
                string returnUrl = Url.Content("~/");

                if (ModelState.IsValid)
                {

                    //get the token
                    string urlToken = "http://localhost:8010/";

                    HttpClient clientToken = _aPRPAPI.InitializeClient(urlToken);
                    var contentToken = new StringContent(JsonConvert.SerializeObject(validatePhoneSendOTP), Encoding.UTF8, "application/json");
                    HttpResponseMessage resToken = clientToken.PostAsync("api/user/validatephonesendotp/", contentToken).Result;
                    if (resToken.IsSuccessStatusCode)
                    {
                        //process the json
                        var cnt = await resToken.Content.ReadAsStringAsync();
                        dynamic response = JObject.Parse(cnt);

                        //Get message, phone and email
                        string message = response.message;
                        string phone = response.detail.mobile_phone;
                        string email = response.email;
                        Console.WriteLine($"message:{message}");
                        return RedirectToAction("ValidateOTP", "Application", new
                        {
                            email = email,
                            phone = phone
                        });

                    }
                    else
                    {
                        var cnt = await resToken.Content.ReadAsStringAsync();
                        dynamic response = JObject.Parse(cnt);
                        if (resToken.StatusCode == HttpStatusCode.BadRequest)
                        {
                            ModelState.AddModelError(string.Empty, response.message);

                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, $"Response : {response.message}");
                        }
                        return View();
                    }

                }
                return View();
            }
            catch (Exception Ex)
            {
                ModelState.AddModelError(string.Empty, "We couldn't sign you up. Kindly contact the system admin");
                return View();

            }
        }


        public IActionResult ValidateOTP(string email, string phone)
        {
            ValidateOTP validateOTP = new ValidateOTP();
            validateOTP.email = email;
            validateOTP.phone = phone;
            return View(validateOTP);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidateOTP([Bind("email,otp,phone")] ValidateOTP validateOTP)
        {
            try
            {
                string returnUrl = Url.Content("~/");

                if (ModelState.IsValid)
                {

                    //get the token
                    string urlToken = "http://localhost:8010/";

                    HttpClient clientToken = _aPRPAPI.InitializeClient(urlToken);
                    var contentToken = new StringContent(JsonConvert.SerializeObject(validateOTP), Encoding.UTF8, "application/json");
                    HttpResponseMessage resToken = clientToken.PostAsync("api/user/validateotp/", contentToken).Result;
                    if (resToken.IsSuccessStatusCode)
                    {
                        //process the json
                        var cnt = await resToken.Content.ReadAsStringAsync();
                        dynamic response = JObject.Parse(cnt);

                        //Get message, phone and email
                        string message = response.message;
                        string phone = response.detail.mobile_phone;
                        string email = validateOTP.email;
                        Console.WriteLine($"message:{message}");
                        return RedirectToAction("Register", "Application", new
                        {
                            email = email,
                            phone = phone
                        });

                    }
                    else
                    {
                        var cnt = await resToken.Content.ReadAsStringAsync();
                        dynamic response = JObject.Parse(cnt);
                        if (resToken.StatusCode == HttpStatusCode.BadRequest)
                        {
                            ModelState.AddModelError(string.Empty, response.message);

                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, $"Response : {response.message}");
                        }
                        return View();
                    }

                }
                return View();
            }
            catch (Exception Ex)
            {
                ModelState.AddModelError(string.Empty, "We couldn't sign you up. Kindly contact the system admin");
                return View();

            }
        }

        public IActionResult Register(string email, string phone)
        {
            Register register = new Register();
            register.email = email;
            register.mobile_phone = phone;
            return View(register);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("email,username,mobile_phone,first_name,last_name,password,ConfirmPassword")] Register register)
        {
            try
            {
                string returnUrl = Url.Content("~/");

                if (ModelState.IsValid)
                {

                    //get the token
                    string urlToken = "http://localhost:8010/";

                    HttpClient clientToken = _aPRPAPI.InitializeClient(urlToken);
                    var contentToken = new StringContent(JsonConvert.SerializeObject(register), Encoding.UTF8, "application/json");
                    HttpResponseMessage resToken = clientToken.PostAsync("api/user/create/", contentToken).Result;
                    if (resToken.IsSuccessStatusCode)
                    {
                        //process the json
                        var cnt = await resToken.Content.ReadAsStringAsync();
                        dynamic response = JObject.Parse(cnt);

                        //Redirect to sign in page
                        return RedirectToAction("SignIn", "Application");

                    }
                    else
                    {
                        var cnt = await resToken.Content.ReadAsStringAsync();
                        dynamic response = JObject.Parse(cnt);
                        if (resToken.StatusCode == HttpStatusCode.BadRequest)
                        {
                            ModelState.AddModelError(string.Empty, response.message);

                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, $"Response : {response.message}");
                        }
                        return View();
                    }

                }
                return View();
            }
            catch (Exception Ex)
            {
                ModelState.AddModelError(string.Empty, "We couldn't sign you up. Kindly contact the system admin");
                return View();

            }
        }

        #endregion

        #region Change Password/Reset Password
        public IActionResult ChangePassword()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword([Bind("old_password,new_password")] ChangePassword changePassword)
        {
            try
            {
                string returnUrl = Url.Content("~/");

                if (ModelState.IsValid)
                {

                    //get the URL and token
                    string url = "http://localhost:8010/";
                    string token = HttpContext.Session.GetString("tokenz");

                    HttpClient client = tokenAPI.InitializeClient(url, token);
                    var contentToken = new StringContent(JsonConvert.SerializeObject(changePassword), Encoding.UTF8, "application/json");
                    HttpResponseMessage resToken = client.PatchAsync("api/user/changepassword/", contentToken).Result;
                    if (resToken.IsSuccessStatusCode)
                    {
                        //process the json
                        var cnt = await resToken.Content.ReadAsStringAsync();
                        dynamic response = JObject.Parse(cnt);
                        string message = response.message;

                        //Redirect to sign in page
                        return RedirectToAction("SignIn", "Application", new
                        {
                            message = message
                        });

                    }
                    else
                    {
                        var cnt = await resToken.Content.ReadAsStringAsync();
                        dynamic response = JObject.Parse(cnt);
                        string message = response.message;
                        if (resToken.StatusCode == HttpStatusCode.BadRequest)
                        {
                            ModelState.AddModelError(string.Empty, message);

                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, $"Response : {message}");
                        }
                        return View();
                    }

                }
                return View();
            }
            catch (Exception Ex)
            {
                ModelState.AddModelError(string.Empty, "We couldn't sign you up. Kindly contact the system admin");
                return View();

            }
        }

        public IActionResult ResetPasswordRequest()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPasswordRequest([Bind("email")] ResetPasswordRequest resetPasswordRequest)
        {
            try
            {
                string returnUrl = Url.Content("~/");

                if (ModelState.IsValid)
                {

                    //get the URL and token
                    string url = Configuration["AuthURI"];
                    //string href2 = Request.GetEncodedUrl();
                    string domain = $"{Request.Scheme}://{Request.Host}" +
                        $"{Url.Action("ResetPasswordConfirm", "Application")}";
                    resetPasswordRequest.url = domain;

                    HttpClient client = _aPRPAPI.InitializeClient(url);
                    var contentToken = new StringContent(JsonConvert.SerializeObject(resetPasswordRequest), Encoding.UTF8, "application/json");
                    HttpResponseMessage resToken = client.PostAsync("api/user/reset_password/", contentToken).Result;
                    if (resToken.IsSuccessStatusCode)
                    {
                        //process the json
                        var cnt = await resToken.Content.ReadAsStringAsync();
                        dynamic response = JObject.Parse(cnt);
                        string message = response.message;

                        //Redirect to sign in page
                        return RedirectToAction("SignIn", "Application", new
                        {
                            message = message
                        });

                    }
                    else
                    {
                        var cnt = await resToken.Content.ReadAsStringAsync();
                        dynamic response = JObject.Parse(cnt);
                        string message = response.message;
                        if (resToken.StatusCode == HttpStatusCode.BadRequest)
                        {
                            ModelState.AddModelError(string.Empty, message);

                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, $"Response : {message}");
                        }
                        return View();
                    }

                }
                return View();
            }
            catch (Exception Ex)
            {
                ModelState.AddModelError(string.Empty, "We couldn't sign you up. Kindly contact the system admin");
                return View();

            }
        }

        public IActionResult ResetPasswordConfirm(string uid, string token)
        {
            ResetPasswordConfirm resetPasswordConfirm = new ResetPasswordConfirm();
            resetPasswordConfirm.uid = uid;
            resetPasswordConfirm.token = token;
            return View(resetPasswordConfirm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPasswordConfirm([Bind("uid,token,password,ConfirmPassword")] ResetPasswordConfirm resetPasswordConfirm)
        {
            try
            {
                string returnUrl = Url.Content("~/");

                if (ModelState.IsValid)
                {

                    //get the URL and token
                    string url = Configuration["AuthURI"];

                    HttpClient client = _aPRPAPI.InitializeClient(url);
                    var contentToken = new StringContent(JsonConvert.SerializeObject(resetPasswordConfirm), Encoding.UTF8, "application/json");
                    HttpResponseMessage resToken = client.PatchAsync($"api/user/reset_password_confirm/?" +
                        $"uid={resetPasswordConfirm.uid}&token={resetPasswordConfirm.token}",
                        contentToken).Result;
                    if (resToken.IsSuccessStatusCode)
                    {
                        //process the json
                        var cnt = await resToken.Content.ReadAsStringAsync();
                        dynamic response = JObject.Parse(cnt);
                        string message = response.message;

                        //Redirect to sign in page
                        return RedirectToAction("SignIn", "Application", new
                        {
                            message = message
                        });

                    }
                    else
                    {
                        var cnt = await resToken.Content.ReadAsStringAsync();
                        dynamic response = JObject.Parse(cnt);
                        string message = response.message;
                        if (resToken.StatusCode == HttpStatusCode.BadRequest)
                        {
                            ModelState.AddModelError(string.Empty, message);

                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, $"Response : {message}");
                        }
                        return View();
                    }

                }
                return View();
            }
            catch (Exception Ex)
            {
                ModelState.AddModelError(string.Empty, "We couldn't sign you up. Kindly contact the system admin");
                return View();

            }
        }

        #endregion
    }
}
