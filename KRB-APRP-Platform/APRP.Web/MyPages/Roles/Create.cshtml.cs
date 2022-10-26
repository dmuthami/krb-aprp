using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace APRP.Web.MyPages.Roles
{
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class CreateModel : PageModel
    {
        private readonly IApplicationRolesService _applicationRolesService;
        private readonly ILogger _logger;
        private readonly IApplicationUsersService _applicationUsersService;
        public CreateModel(IApplicationRolesService applicationRolesService, ILogger<CreateModel> logger,
            IApplicationUsersService applicationUsersService)
        {
            _applicationRolesService = applicationRolesService;
            _applicationUsersService = applicationUsersService;
            _logger = logger;
        }

        [BindProperty]
        public ApplicationRole ApplicationRole { get; set; }

        private IList<string> _MyRoles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                //Get logged in user
                ApplicationUser _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);

                //Check if is approved
                if ((await IsApproved(_ApplicationUser).ConfigureAwait(false)) == false)
                {
                    string referer = HttpContext.Session.GetString("Referer");
                    if (!String.IsNullOrEmpty(referer))
                    {
                        return Redirect(referer);
                    }
                    return RedirectToAction("Dashboard", "Home");
                }

                //Set Return URL and store in session
                HttpContext.Session.SetString("Referer", Request.GetEncodedUrl());

                return Page();
            }
            catch (System.Exception Ex)
            {
                _logger.LogError(Ex, $"Roles.Index Page Error: {Ex.Message} " +
                    $"{Environment.NewLine}");
                return Page();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                //Get logged in user
                ApplicationUser _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);

                //Check if is approved
                if ((await IsApproved(_ApplicationUser).ConfigureAwait(false)) == false)
                {
                    string referer = HttpContext.Session.GetString("Referer");
                    if (!String.IsNullOrEmpty(referer))
                    {
                        return Redirect(referer);
                    }
                    return RedirectToAction("Dashboard", "Home");
                }
                if (!ModelState.IsValid)
                {
                    return Page();
                }
                var applicationRolesResponse = await _applicationRolesService.AddAsync(ApplicationRole).ConfigureAwait(false);
            
                return RedirectToPage("./Index");
            }
            catch (System.Exception Ex)
            {
                _logger.LogError(Ex,$"Roles.Create : {Ex.Message} {Environment.NewLine}");
                ModelState.AddModelError(string.Empty, Ex.Message);
                return Page();
            }
        }

        #region User Access

        private async Task<ApplicationUser> GetLoggedInUser()
        {
            var userResp = await _applicationUsersService.FindByNameAsync(User.Identity.Name).ConfigureAwait(false);
            ApplicationUser user = null;
            if (userResp.Success)
            {
                //user is found


                var objectResult = (ObjectResult)userResp.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (OkObjectResult)objectResult;
                    user = (ApplicationUser)result2.Value;
                }
            }
            return user;
        }

        private async Task<bool> IsApproved(ApplicationUser applicationUser)
        {
            bool _isApproved = false;

            var appUserResp = await _applicationUsersService.GetRolesAsync(applicationUser).ConfigureAwait(false);

            if (appUserResp.Success)
            {
                var objectResult = (ObjectResult)appUserResp.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (OkObjectResult)objectResult;
                    _MyRoles = (IList<string>)result2.Value;

                    if (_MyRoles.Contains("Administrators"))
                    {
                        _isApproved = true;
                    }
                }
            }

            //Check if in Administrator role
            return _isApproved;
        }

        #endregion
    }
}
