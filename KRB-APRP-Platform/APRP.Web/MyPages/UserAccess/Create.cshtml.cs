using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace APRP.Web.MyPages.UserAccess
{
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class CreateModel : PageModel
    {
        private readonly IUserAccessListService _userAccessListService;
        private readonly ILogger _logger;
        private readonly IAuthorityService _authorityService;
        private readonly IApplicationUsersService _applicationUsersService;

        public CreateModel(IUserAccessListService userAccessListService, ILogger<CreateModel> logger,
            IAuthorityService authorityService, IApplicationUsersService applicationUsersService)
        {
            _userAccessListService = userAccessListService;
            _logger = logger;
            _authorityService = authorityService;
            _applicationUsersService = applicationUsersService;
        }
        private IList<string> _MyRoles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.Administrator.Role_View)]
        public async Task<IActionResult> OnGet()
        {
            try
            {
                UserAccessList = new UserAccessList();
                await PopulateDropDown().ConfigureAwait(false);

                //Set Return URL and store in session
                HttpContext.Session.SetString("Referer", Request.GetEncodedUrl());

                return Page();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"UserAccess.Create: {Ex.Message} {Environment.NewLine}");
                ModelState.AddModelError(string.Empty, Ex.Message);
                return Page();
            }
        }

        [BindProperty]
        public UserAccessList UserAccessList { get; set; }


        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.Administrator.Role_Add)]
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                if (UserAccessList.InstitutionGroup != InstitutionGroup.CGs)
                {
                    if (UserAccessList.InstitutionGroup == InstitutionGroup.RSIP_CS)
                    {
                        UserAccessList.AuthorityId = (long)InstitutionGroup.RSIP_CS;
                    }
                    else
                    {
                        UserAccessList.AuthorityId = (long)UserAccessList.InstitutionGroup;
                    }
                }

                //Check if email address already exists
                var userAccessListResponse = await _userAccessListService.FindByEmailAsync(UserAccessList.EmailAddress).ConfigureAwait(false);
                if (userAccessListResponse.Success)//Email already exixts
                {
                    await PopulateDropDown().ConfigureAwait(false);
                    string msg = $"The Email address:{UserAccessList.EmailAddress}" +
                        $" already exists. Please select a different E-Mail address";
                    _logger.LogError(msg, $"UserAccess.Add: {msg} {Environment.NewLine}");
                    ModelState.AddModelError(string.Empty, msg);
                    return Page();
                }

                userAccessListResponse = await _userAccessListService.AddAsync(UserAccessList).ConfigureAwait(false);
                UserAccessList = userAccessListResponse.UserAccessList;

                return RedirectToPage("./Index");
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"UserAccess.Add: {Ex.Message} {Environment.NewLine}");
                ModelState.AddModelError(string.Empty, Ex.Message);
                return Page();
            }
        }

        #region Utilities
        private async Task PopulateDropDown()
        {
            var authority = await _authorityService.ListAsync().ConfigureAwait(false);
            ViewData["AuthorityId"] = new SelectList(authority, "ID", "Name");
        }
        #endregion

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
