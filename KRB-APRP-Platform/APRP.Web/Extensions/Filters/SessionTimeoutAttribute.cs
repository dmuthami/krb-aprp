using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using APRP.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace APRP.Web.Extensions.Filters
{
    public class SessionTimeoutAttribute : IAsyncActionFilter, IAsyncAuthorizationFilter
    {
        public IConfiguration Configuration { get; }

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAuthenticateService _authenticateService;


        public SessionTimeoutAttribute(
             IConfiguration configuration
             , SignInManager<ApplicationUser> signInManager
            , UserManager<ApplicationUser> userManager,
             IAuthenticateService authenticateService)
        {          
            Configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
            _authenticateService = authenticateService;
        }

        public async Task<int> Check(ActionExecutingContext context)
        {
            CheckToken checktoken = new CheckToken();
            checktoken.Token = context.HttpContext.Session.GetString("tokenz");
            var authenticateResponse = await _authenticateService.CheckToken(checktoken).ConfigureAwait(false);

            var objectResult = (ObjectResult)authenticateResponse.IActionResult;
            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
            {
                return 0;
            }else {
                return -1;
            }
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            //throw new System.NotImplementedException();
            if (context==null)
            {
                context.Result = new RedirectResult("~/Index");
            }
            var user = await _userManager.GetUserAsync(context.HttpContext.User).ConfigureAwait(false);
            //var user2 = context.HttpContext.User
            if (context.HttpContext.User.Identity.IsAuthenticated==false)
            {
                //redirect to Home/signin
                context.Result = new RedirectResult("~/Index");
                return;
            }

            if (_signInManager.IsSignedIn(context.HttpContext.User))
            {

                ////Verify Email
                ///
                if (user.EmailConfirmed == false)
                {
                    //string msg3 = "Redirect to Send_Confirmation_Code_to_Email";
                    //_logger.LogInformation($"ApplicationController.Sign_In_Verification: \r\n {msg3}");
                    context.Result = new RedirectResult($"~/Application/Send_Confirmation_Code_to_Email?id ={ user.Id}");
                    return;
                }
            }
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //if the token is null then it means you have exceeded an idle time of 5 minutes
            // and are you will be required to log back in
           
            string mytoken = context.HttpContext.Session.GetString("tokenz");
            if (mytoken == null)
            {
                context.Result = new RedirectResult("~/Index");
                return;
            }

            int tokenRet = await Check(context).ConfigureAwait(false);
            if (tokenRet != 0)
            {
                /*
                 * Renew Token by signing in
                 */
                context.Result = new RedirectResult("~/Index");
                return;
            }

            // next() calls the action method.
            var resultContext = await next().ConfigureAwait(false);
            // resultContext.Result is set.
            // Do something after the action executes.
        }
    }
}
