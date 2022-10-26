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
    public class EditModel : PageModel
    {
        private readonly IUserAccessListService _userAccessListService;
        private readonly ILogger _logger;
        private readonly IAuthorityService _authorityService;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IRegisterService _registerService;

        public EditModel(IUserAccessListService userAccessListService, ILogger<EditModel> logger,
            IAuthorityService authorityService, IApplicationUsersService applicationUsersService,
            IRegisterService registerService)
        {
            _userAccessListService = userAccessListService;
            _logger = logger;
            _authorityService = authorityService;
            _applicationUsersService = applicationUsersService;
            _registerService = registerService;
        }

        [BindProperty]
        public UserAccessList UserAccessList { get; set; }

        [BindProperty]
        public string _EmailAddress { get; set; }

        private IList<string> _MyRoles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.Administrator.Role_View)]
        public async Task<IActionResult> OnGetAsync(long? id)
        {
            try
            {
                long ID = 0;
                bool result = long.TryParse(id.ToString(), out ID);
                if (id == null || result == false)
                {
                    return NotFound();
                }

                //populate drop downs
                await PopulateDropDown(ID).ConfigureAwait(false);

                if (UserAccessList == null)
                {
                    return NotFound();
                }
                _EmailAddress = UserAccessList.EmailAddress;

                //Set Return URL and store in session
                HttpContext.Session.SetString("Referer", Request.GetEncodedUrl());

                return Page();
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"UserAccess.Edit: {Ex.Message} {Environment.NewLine}");
                ModelState.AddModelError(string.Empty, Ex.Message);
                return Page();
            }
        }


        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.Administrator.Role_Change)]
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


                //Get Database Existing item for User Access List
                //var userAccessListResponse = await _userAccessListService.FindByIdAsync(UserAccessList.Id).ConfigureAwait(false);
                //var databaseUserAccessList = userAccessListResponse.UserAccessList;
                //if (databaseUserAccessList != null)
                //{}
                // if database email is not equal to new email
                if (_EmailAddress != UserAccessList.EmailAddress)
                {
                    //If new email already exists then display error since it means
                    //Email matches another email for a different record
                    var userAccessListResponse2 = await _userAccessListService.FindByEmailAsync(UserAccessList.EmailAddress).ConfigureAwait(false);
                    if (userAccessListResponse2.Success)//Email already exixts
                    {
                        await PopulateDropDown(UserAccessList.Id).ConfigureAwait(false);
                        string msg = $"The Email address:{UserAccessList.EmailAddress}" +
                            $" already exists and owned by a different User. Please enter a different E-Mail address";
                        _logger.LogError(msg, $"UserAccess.Add: {msg} {Environment.NewLine}");
                        ModelState.AddModelError(string.Empty, msg);
                        return Page();
                    }
                }

                var userAccessListResponse = await _userAccessListService.Update(UserAccessList.Id, UserAccessList).ConfigureAwait(false);
                UserAccessList = userAccessListResponse.UserAccessList;

                //Call service to update user
                ApplicationUser userOfInterest = await GetUser(UserAccessList.EmailAddress).ConfigureAwait(false);
                if (userOfInterest != null)
                {
                    userOfInterest.AuthorityId = UserAccessList.AuthorityId;
                    var appUserResp = await _registerService.UpdateAsync(userOfInterest).ConfigureAwait(false);
                }

                return RedirectToPage("./Index");
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"UserAccess.Edit: {Ex.Message} {Environment.NewLine}");
                ModelState.AddModelError(string.Empty, Ex.Message);
                return Page();
            }
        }

        #region Utilities
        private async Task PopulateDropDown(long ID)
        {
            UserAccessList = null;
            var userAccessListResponse = await _userAccessListService.FindByIdAsync(ID).ConfigureAwait(false);
            UserAccessList = userAccessListResponse.UserAccessList;

            var authority = await _authorityService.ListAsync("CG").ConfigureAwait(false);
            ViewData["AuthorityId"] = new SelectList(authority, "ID", "Name");

            ViewData["InstitutionGroupValue"] = (long)UserAccessList.InstitutionGroup;
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<ApplicationUser> GetUser(string Email)
        {
            try
            {
                ApplicationUser user = null;
                var appUserResponse = await _applicationUsersService.FindByEmailAsync(Email).ConfigureAwait(false);
                if (appUserResponse.Success)
                {
                    //user is found
                    var objectResult = (ObjectResult)appUserResponse.IActionResult;
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result2 = (OkObjectResult)objectResult;
                        user = (ApplicationUser)result2.Value;

                    }
                }
                return user;
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"Sign_Up_Individual Error {Environment.NewLine}");
                return null;
            }
        }
        #endregion
    }
}
