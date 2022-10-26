using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace APRP.Web.MyPages.AllocationCodeUnitRx
{
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class IndexModel : PageModel
    {
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly ILogger _logger;
        private readonly IAllocationCodeUnitService _allocationCodeUnitService;
        private readonly IFinancialYearService _financialYearService;


        public IndexModel(ILogger<IndexModel> logger,
            IApplicationUsersService applicationUsersService, IAuthorityService authorityService,
            IAllocationCodeUnitService allocationCodeUnitService,
            IFinancialYearService financialYearService)
        {
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _logger = logger;
            _allocationCodeUnitService = allocationCodeUnitService;
            _financialYearService = financialYearService;
        }

        public Authority Authority { get; set; }

        public IList<AllocationCodeUnit> RAAllocationCodeUnit { get; set; }

        public IList<AllocationCodeUnit> CGAllocationCodeUnit { get; set; }

        public double RAPercent { get; set; }
        public double CGPercent { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.AllocationCodeUnit.View)]
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                //Get logged in user
                ApplicationUser _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);
                Authority = _ApplicationUser.Authority;

                //Road agencies and others
                var RAallocationCodeUnitResp = await _allocationCodeUnitService.ListAsync("RA"
                    ).ConfigureAwait(false);
                RAAllocationCodeUnit = (IList<AllocationCodeUnit>)RAallocationCodeUnitResp.AllocationCodeUnit;

                RAPercent = RAAllocationCodeUnit.Sum(x => x.Percent);
                ////County governments
                var CGllocationCodeUnitResp = await _allocationCodeUnitService.ListAsync("CG"
                    ).ConfigureAwait(false);
                CGAllocationCodeUnit = (IList<AllocationCodeUnit>)CGllocationCodeUnitResp.AllocationCodeUnit;
                CGPercent = CGAllocationCodeUnit.Sum(x => x.Percent);

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
