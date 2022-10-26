using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Http.Extensions;

namespace APRP.Web.MyPages.UserAccess
{
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class DeleteModel : PageModel
    {
        private readonly IUserAccessListService _userAccessListService;
        private readonly ILogger _logger;
        private readonly IApplicationUsersService _applicationUsersService;

        public DeleteModel(IUserAccessListService userAccessListService, ILogger<DeleteModel> logger,
            IApplicationUsersService applicationUsersService)
        {
            _userAccessListService = userAccessListService;
            _applicationUsersService = applicationUsersService;
            _logger = logger;
        }

        [BindProperty]
        public UserAccessList UserAccessList { get; set; }
        private IList<string> _MyRoles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> OnGetAsync(long? id)
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

                long ID = 0;
                bool result = long.TryParse(id.ToString(), out ID);
                if (id == null || result==false)
                {
                    return NotFound();
                }
              
                var userAccessListResponse = await _userAccessListService.FindByIdAsync(ID).ConfigureAwait(false);
                UserAccessList = userAccessListResponse.UserAccessList;

                if (UserAccessList == null)
                {
                    return NotFound();
                }
                //Set Return URL and store in session
                HttpContext.Session.SetString("Referer", Request.GetEncodedUrl());

                return Page();
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"UserAccess.Delete: {Ex.Message} {Environment.NewLine}");
                ModelState.AddModelError(string.Empty, Ex.Message);
                return Page();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> OnPostAsync(long? id)
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

                long ID = 0;
                bool result = long.TryParse(id.ToString(), out ID);

                if (id == null)
                {
                    return NotFound();
                }

                var userAccessListResponse = await _userAccessListService.FindByIdAsync(ID).ConfigureAwait(false);
                UserAccessList = userAccessListResponse.UserAccessList;

                if (UserAccessList != null)
                {
                    await _userAccessListService.RemoveAsync(ID).ConfigureAwait(false);
                }
                return RedirectToPage("./Index");
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"UserAccess.Delete: {Ex.Message} {Environment.NewLine}");
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
