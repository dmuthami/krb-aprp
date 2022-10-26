using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace APRP.Web.MyPages.UserAccess
{
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class IndexModel : PageModel
    {
        private readonly IUserAccessListService _userAccessListService;
        private readonly ILogger _logger;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IRegisterService _registerService;

        public IndexModel(IUserAccessListService userAccessListService, ILogger<IndexModel> logger,
             IApplicationUsersService applicationUsersService, IRegisterService registerService)
        {
            _userAccessListService = userAccessListService;
            _applicationUsersService = applicationUsersService;
            _logger = logger;
            _registerService = registerService;
        }

        private IList<string> _MyRoles { get; set; }

        public IList<ApplicationUser> AllUsers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
        public IList<UserAccessList> UserAccessList { get;set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.Administrator.Role_View)]
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {

                var userAccessListResponse = await _userAccessListService.ListAsync().ConfigureAwait(false);
                UserAccessList = (IList<UserAccessList>)userAccessListResponse.UserAccessList;


                var alluserResp = await _registerService.ListAsync2().ConfigureAwait(false);
                var objectResult = (ObjectResult)alluserResp.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (OkObjectResult)objectResult;
                    AllUsers = (IList<ApplicationUser>)result2.Value;
                }
                //Set Return URL and store in session
                HttpContext.Session.SetString("Referer", Request.GetEncodedUrl());

                return Page();
            }
            catch (System.Exception Ex)
            {
                _logger.LogError(Ex, $"UserAccess.Index Page Error: {Ex.Message} " +
                    $"{Environment.NewLine}");
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
        #endregion
    }
}
