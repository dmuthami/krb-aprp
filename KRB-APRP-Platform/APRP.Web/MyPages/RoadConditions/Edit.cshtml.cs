using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace APRP.Web.MyPages.RoadConditions
{
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class EditModel : PageModel
    {
        private readonly IRoadConditionService _roadConditionService;
        private readonly ILogger _logger;
        private readonly IAuthorityService _authorityService;
        private readonly IApplicationUsersService _applicationUsersService;

        public EditModel(IRoadConditionService roadConditionService, ILogger<EditModel> logger,
            IAuthorityService authorityService, IApplicationUsersService applicationUsersService)
        {
            _roadConditionService = roadConditionService;
            _logger = logger;
            _authorityService = authorityService;
            _applicationUsersService = applicationUsersService;
        }
        [BindProperty]
        public RoadCondition RoadCondition { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> OnGet(long id)
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

                var applicationRolesResponse = await _roadConditionService.FindByIdAsync(id).ConfigureAwait(false);
                RoadCondition = applicationRolesResponse.RoadCondtion;

                if (RoadCondition == null)
                {
                    return NotFound();
                }

                //Set Return URL and store in session
                HttpContext.Session.SetString("Referer", Request.GetEncodedUrl());

                return Page();
            }
            catch (System.Exception Ex)
            {
                _logger.LogError(Ex, $"RoadCondition.Edit: {Ex.Message} {Environment.NewLine}");
                ModelState.AddModelError(string.Empty, Ex.Message);
                return Page();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var applicationRolesResponse = await _roadConditionService.Update(RoadCondition.ID, RoadCondition).ConfigureAwait(false);
                RoadCondition = applicationRolesResponse.RoadCondtion;
                return RedirectToPage("./Index");
            }
            catch (System.Exception Ex)
            {
                _logger.LogError(Ex, $"RoadCondition.Edit Error: {Ex.Message} {Environment.NewLine}");
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

        private IList<string> _MyRoles { get; set; }
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
