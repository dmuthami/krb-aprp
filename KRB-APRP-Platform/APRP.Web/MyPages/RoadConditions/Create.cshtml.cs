using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace APRP.Web.MyPages.RoadConditions
{
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class CreateModel : PageModel
    {
        private readonly IRoadConditionService _roadConditionService;
        private readonly IRoadService _roadService;
        private readonly ILogger _logger;
        private readonly IAuthorityService _authorityService;
        private readonly IApplicationUsersService _applicationUsersService;
        public CreateModel(IRoadConditionService roadConditionService, 
            ILogger<CreateModel> logger,
             IRoadService roadService, 
             IAuthorityService authorityService, IApplicationUsersService applicationUsersService)
        {
            _roadConditionService = roadConditionService;
            _logger = logger;
            _roadService = roadService;
            _authorityService = authorityService;
            _applicationUsersService = applicationUsersService;
        }

        [BindProperty]
        public RoadCondition RoadCondition { get; set; }

        public Authority Authority { get; set; }

        private IList<string> _MyRoles { get; set; }

        public void OnGet()
        {

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }
                var roadConditionResponse = await _roadConditionService.AddAsync(RoadCondition).ConfigureAwait(false);

                //Get List of surfaceTypes
                var roadListResponse = await _roadService.ListAsync().ConfigureAwait(false);
                IList<Road> RoadList = (IList<Road>)roadListResponse.Roads;
                ViewData["Road_ID"] = new SelectList(RoadList, "ID", "RoadNumber");

                return RedirectToPage("./Index");
            }
            catch (System.Exception Ex)
            {
                _logger.LogError(Ex,$"RoadCondition.Create : {Ex.Message} {Environment.NewLine}");
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
