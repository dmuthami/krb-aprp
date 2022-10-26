using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Http.Extensions;
using APRP.Web.ViewModels.ResponseTypes;

namespace APRP.Web.MyPages.RevenueCollectionCodeUnitRx
{
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class IndexModel : PageModel
    {
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly ILogger _logger;
        private readonly IRevenueCollectionCodeUnitService _revenueCollectionCodeUnitService;


        public IndexModel( ILogger<IndexModel> logger,
            IApplicationUsersService applicationUsersService, IAuthorityService authorityService,
            IRevenueCollectionCodeUnitService revenueCollectionCodeUnitService)
        {
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _logger = logger;
            _revenueCollectionCodeUnitService = revenueCollectionCodeUnitService;
        }

        public Authority Authority { get; set; }

        public IList<RevenueCollectionCodeUnit> RevenueCollectionCodeUnit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                //Get logged in user
                ApplicationUser _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);
                Authority = _ApplicationUser.Authority;

                var userResp = await _applicationUsersService.IsInRoleAsync(_ApplicationUser, "Administrators").ConfigureAwait(false);
                var objectResult = (ObjectResult)userResp.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (OkObjectResult)objectResult;
                    AuthResponse authResponse = (AuthResponse)result2.Value;
                    if (authResponse.Success == true)
                    {
                        //Return all Roads
                        var revenueCollectionCodeUnitResp = await _revenueCollectionCodeUnitService.ListAsync(null).ConfigureAwait(false);
                        RevenueCollectionCodeUnit = (IList<RevenueCollectionCodeUnit>)revenueCollectionCodeUnitResp.RevenueCollectionCodeUnit;
                    }
                    else
                    {
                        //Return for authority that user is placed
                        var revenueCollectionCodeUnitResp = await _revenueCollectionCodeUnitService.ListAsync(Authority.ID).ConfigureAwait(false);
                        RevenueCollectionCodeUnit = (IList<RevenueCollectionCodeUnit>)revenueCollectionCodeUnitResp.RevenueCollectionCodeUnit;
                    }
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
