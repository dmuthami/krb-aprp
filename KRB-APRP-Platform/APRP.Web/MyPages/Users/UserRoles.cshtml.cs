using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace APRP.Web.MyPages.Users
{
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class UserRolesModel : PageModel
    {
        private readonly IApplicationRolesService _applicationRolesService;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;

        private readonly ILogger _logger;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
        public IList<ApplicationRole> ApplicationRole { get; set; }
            
        #pragma warning restore CA2227 // Collection properties should be read only
        public ApplicationUser ApplicationUser { get; set; }

        public Authority Authority { get; set; }
        public IList<string> MyRoles { get; set; }

        public UserRolesModel(IApplicationRolesService applicationRolesService,
            ILogger<UserRolesModel> logger, IApplicationUsersService applicationUsersService,
             IAuthorityService authorityService)
        {
            _applicationRolesService = applicationRolesService;
            _logger = logger;
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
        }
        private IList<string> _MyRoles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.Administrator.Role_View)]
        public async Task<IActionResult> OnGetAsync(string ID)
        {
            try
            {
                //Get logged in user
                ApplicationUser LoggedInUser = await GetLoggedInUser().ConfigureAwait(false);
                Authority = LoggedInUser.Authority;

                //Populate the roles of the user whose id has been parsed 
                ApplicationUser = null;
                var userResp = await _applicationUsersService.FindByIdAsync(ID).ConfigureAwait(false);               
                if (userResp.Success)
                {
                    //user is found
                    var objectResult2 = (ObjectResult)userResp.IActionResult;
                    if (objectResult2.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result2 = (OkObjectResult)objectResult2;
                        ApplicationUser = (ApplicationUser)result2.Value;
                    }
                }

                var applicationRolesListResponse = await _applicationRolesService.ListDefaultRolesAsync().ConfigureAwait(false);
                ApplicationRole = (IList<ApplicationRole>)applicationRolesListResponse.ApplicationRole;

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
                    if (user != null)
                    {
                        var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                        user.Authority = userAuthority.Authority;
                    }
                }
            }
            return user;
        }
        #endregion
    }
}
