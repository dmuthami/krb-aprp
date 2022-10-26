using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Http.Extensions;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Authorization;

namespace APRP.Web.MyPages.QuarterCodeUnitRx
{
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class IndexModel : PageModel
    {
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly ILogger _logger;
        private readonly IQuarterCodeUnitService _quarterCodeUnitService;

        public IndexModel( ILogger<IndexModel> logger,
            IApplicationUsersService applicationUsersService, IAuthorityService authorityService,
            IQuarterCodeUnitService quarterCodeUnitService)
        {
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _logger = logger;
            _quarterCodeUnitService = quarterCodeUnitService;
        }

        public IList<QuarterCodeUnit> QuarterCodeUnit { get;set; }
        public Authority Authority { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.QuarterCodeUnit.View)]
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                //Get logged in user
                ApplicationUser _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);
                Authority = _ApplicationUser.Authority;

                var quarterCodeUnitResp = await _quarterCodeUnitService.ListAsync().ConfigureAwait(false);                
                //QuarterCodeUnit = (IList<QuarterCodeUnit>)
                //    (quarterCodeUnitResp.QuarterCodeUnit.OrderByDescending(x => x.FinancialYear.IsCurrent).ToList());

                QuarterCodeUnit = (IList<QuarterCodeUnit>)quarterCodeUnitResp.QuarterCodeUnit;
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
