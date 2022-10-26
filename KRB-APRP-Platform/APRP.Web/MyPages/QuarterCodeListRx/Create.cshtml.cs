using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace APRP.Web.MyPages.QuarterCodeListRx
{

    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class CreateModel : PageModel
    {
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly ILogger _logger;
        private readonly IQuarterCodeListService _quarterCodeListService;
        private readonly IFinancialYearService _financialYearService;

        public CreateModel(ILogger<CreateModel> logger,
            IApplicationUsersService applicationUsersService, IAuthorityService authorityService,
            IQuarterCodeListService allocationCodeUnitService,
            IFinancialYearService financialYearService)
        {
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _logger = logger;
            _quarterCodeListService = allocationCodeUnitService;
            _financialYearService = financialYearService;
        }

        public Authority Authority { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.QuarterCodeList.View)]
        public async Task<IActionResult> OnGet()
        {
            try
            {
                //Get logged in user
                ApplicationUser _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);
                Authority = _ApplicationUser.Authority;

                //Set Financial Year
                var finacialResp = await _financialYearService.ListAsync().ConfigureAwait(false);
                IList<FinancialYear> financialYears = (IList<FinancialYear>)finacialResp;
                ViewData["FinancialYearId"] = new SelectList(financialYears, "ID", "Code");

                //Set Authority Year
                var authorityResp = await _authorityService.ListAsync().ConfigureAwait(false);
                IList<Authority> authoritys = (IList<Authority>)authorityResp;
                ViewData["AuthorityId"] = new SelectList(authoritys, "ID", "Name");

                //Set Return URL and store in session
                HttpContext.Session.SetString("Referer", Request.GetEncodedUrl());
                return Page();
            }
            catch (System.Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectioncodeUnit.Index Page Error: {Ex.Message} " +
                    $"{Environment.NewLine}");
                return Page();
            }
        }

        [BindProperty]
        public QuarterCodeList QuarterCodeList { get; set; }


        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.QuarterCodeList.Add)]
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                //check  if revenue collection code unit has been applied
                var revenueCollectionResp = await _quarterCodeListService.FindByNameAsync(QuarterCodeList.Name).ConfigureAwait(false);
                if (revenueCollectionResp.Success)
                {
                    string msg = "Revenue Collection Code Unit item has an existing item with a similar name";
                    _logger.LogError(msg, $"Roles.Index Page Error: {msg} " +
                    $"{Environment.NewLine}");
                    ModelState.AddModelError(string.Empty, msg);
                    return Page();
                }
                else
                {
                    var quarterCodeListResp = await _quarterCodeListService.AddAsync(QuarterCodeList).ConfigureAwait(false);
                }
                return RedirectToPage("./Index");
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"QuarterCodeListRx.Add Page Error: {Ex.Message} " +
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
