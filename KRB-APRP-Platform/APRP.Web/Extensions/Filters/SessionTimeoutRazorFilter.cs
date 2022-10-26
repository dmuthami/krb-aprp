using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using APRP.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace APRP.Web.Extensions.Filters
{
    public class SessionTimeoutRazorFilter : IAsyncPageFilter, IAsyncAuthorizationFilter
    {
     
        public IConfiguration Configuration { get; }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthenticateService _authenticateService;

        public SessionTimeoutRazorFilter(
             IConfiguration configuration
            , UserManager<ApplicationUser> userManager,
             IAuthenticateService authenticateService)
        {
            Configuration = configuration;
            _userManager = userManager;
            _authenticateService = authenticateService;
        }

        public async Task<int> Check(PageHandlerExecutingContext context)
        {
            CheckToken checktoken = new CheckToken();
            checktoken.Token = context.HttpContext.Session.GetString("tokenz");
            var authenticateResponse = await _authenticateService.CheckToken(checktoken).ConfigureAwait(false);

            var objectResult = (ObjectResult)authenticateResponse.IActionResult;
            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            //throw new System.NotImplementedException();

            var user = await _userManager.GetUserAsync(context.HttpContext.User).ConfigureAwait(false);
            if (user == null)
            {
                //redirect to Home/signin
                context.Result = new RedirectResult("~/Index");
                return;
            }

            if (user.TwoFactorEnabled == true)
            {
                ////Verify Email
                ///
                if (user.EmailConfirmed == false)
                {
                    context.Result = new RedirectResult("~/Index");
                    return;
                }
            }
            //context.HttpContext.Session.SetString("Referer", context.HttpContext.Request.Path.ToString());//Return URL
            //context.HttpContext.Session.SetString("Referer", context.HttpContext.Request.Headers["Referer"].ToString());
        }

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
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
            await next.Invoke().ConfigureAwait(false);
        }

        public async Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            //_logger.LogDebug("Global OnPageHandlerSelectionAsync called.");
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
