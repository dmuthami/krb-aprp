using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace APRP.Web.MyPages.RoadConditions
{
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class DetailsModel : PageModel
    {
        private readonly IRoadConditionService _roadConditionService;
        private readonly IAuthorityService _authorityService;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly ILogger _logger;

        public DetailsModel(IRoadConditionService roadConditionService, ILogger<DetailsModel> logger,
            IAuthorityService authorityService, IApplicationUsersService applicationUsersService)
        {
            _roadConditionService = roadConditionService;
            _logger = logger;
            _authorityService = authorityService;
            _applicationUsersService = applicationUsersService;
        }
        [BindProperty]
        public RoadCondition RoadCondition { get; set; }

        public Authority Authority { get; set; }

        private IList<string> _MyRoles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> OnGet(long id)
        {
            try
            {

                var roadConditionResponse = await _roadConditionService.FindByIdAsync(id).ConfigureAwait(false);
                RoadCondition = roadConditionResponse.RoadCondtion;

                if (RoadCondition == null)
                {
                    return NotFound();
                }
                return Page();
            }
            catch (System.Exception Ex)
            {
                _logger.LogError(Ex, $"RoadCondition.Detail: {Ex.Message} {Environment.NewLine}");
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
