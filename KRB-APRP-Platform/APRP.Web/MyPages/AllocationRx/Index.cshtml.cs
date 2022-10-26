using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Http.Extensions;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Authorization;

namespace APRP.Web.MyPages.AllocationRx
{
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class IndexModel : PageModel
    {
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly ILogger _logger;
        private readonly IBudgetCeilingService _budgetCeilingService;
        private readonly IFinancialYearService _financialYearService;

        public IndexModel( ILogger<IndexModel> logger,
            IApplicationUsersService applicationUsersService, IAuthorityService authorityService,
           IBudgetCeilingService budgetCeilingService, IFinancialYearService financialYearService)
        {
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _logger = logger;
            _budgetCeilingService = budgetCeilingService;
            _financialYearService = financialYearService;
        }

        public IList<BudgetCeiling> RABudgetCeiling { get;set; }

        public IList<BudgetCeiling> CGBudgetCeiling { get; set; }
        public Authority Authority { get; set; }

        public FinancialYear FinancialYear { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.Allocation.View)]
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                //Get logged in user
                ApplicationUser _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);
                Authority = _ApplicationUser.Authority;

                //Get Current Financial Year
                var finacialResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
                if (finacialResp.Success)
                {
                    FinancialYear = finacialResp.FinancialYear;
                    //List for current financial year and RA's only
                    var budgetResp = await _budgetCeilingService.ListAsync("RA", finacialResp.FinancialYear.ID).ConfigureAwait(false);
                    RABudgetCeiling = (IList<BudgetCeiling>)budgetResp;

                    //List for current financial year and CG's only
                    budgetResp = await _budgetCeilingService.ListAsync("CG", finacialResp.FinancialYear.ID).ConfigureAwait(false);
                    CGBudgetCeiling = (IList<BudgetCeiling>)budgetResp;
                }

                //Set Return URL and Store in Session
                HttpContext.Session.SetString("Referer", Request.GetEncodedUrl());
                return Page();
            }
            catch (Exception Ex)
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
