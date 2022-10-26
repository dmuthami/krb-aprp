using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace APRP.Web.MyPages.RoadConditions
{
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class IndexModel : PageModel
    {
        private readonly IRoadConditionService _roadConditionService;
        private readonly ILogger _logger;
        private readonly IAuthorityService _authorityService;
        private readonly IApplicationUsersService _applicationUsersService;

        #pragma warning disable CA2227 // Collection properties should be read only
        public IList<RoadCondition> RoadCondition { get; set; }
        #pragma warning restore CA2227 // Collection properties should be read only

        public IndexModel(IRoadConditionService roadConditionService, ILogger<IndexModel> logger,
            IAuthorityService authorityService, IApplicationUsersService applicationUsersService)
        {
            _roadConditionService = roadConditionService;
            _logger = logger;
            _authorityService = authorityService;
            _applicationUsersService = applicationUsersService;
        }

        public Authority Authority { get; set; }

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

                var roadConditionListResponse = await _roadConditionService.ListAsync().ConfigureAwait(false);
                RoadCondition = (IList<RoadCondition>)roadConditionListResponse.RoadCondtion;
                return Page();
            }
            catch (System.Exception Ex)
            {
                _logger.LogError(Ex,$"Roles.Index Page Error: {Ex.Message} " +
                    $"{Environment.NewLine}" );
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

                    if (_MyRoles.Contains("Administrators") || _MyRoles.Contains("ARICS.ConductARICS"))
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
