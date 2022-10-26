using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Http.Extensions;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Authorization;

namespace APRP.Web.MyPages.QuarterCodeUnitRx
{
    [Authorize]
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class CreateModel : PageModel
    {
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly ILogger _logger;
        private readonly IQuarterCodeUnitService _quarterCodeUnitService;
        private readonly IFinancialYearService _financialYearService;
        private readonly IQuarterCodeListService _quarterCodeListService;
        private readonly IRevenueCollectionService _revenueCollectionService;
        private readonly IDisbursementService _disbursementService;
        public CreateModel(ILogger<CreateModel> logger,
            IApplicationUsersService applicationUsersService, IAuthorityService authorityService,
            IQuarterCodeUnitService quarterCodeUnitService,
            IFinancialYearService financialYearService,
            IQuarterCodeListService quarterCodeListService,
            IRevenueCollectionService revenueCollectionService,
            IDisbursementService disbursementService)
        {
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _logger = logger;
            _quarterCodeUnitService = quarterCodeUnitService;
            _financialYearService = financialYearService;
            _quarterCodeListService = quarterCodeListService;
            _revenueCollectionService = revenueCollectionService;
            _disbursementService = disbursementService;
        }

        public Authority Authority { get; set; }

        public string Referer { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.QuarterCodeUnit.View)]
        public async Task<IActionResult> OnGet()
        {
            try
            {
                //Get logged in user
                ApplicationUser _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);
                Authority = _ApplicationUser.Authority;

                //Set Previous Return URL
                Referer = Request.Headers["Referer"].ToString();

                //Populate dropdowns
                await PopulateDropDown().ConfigureAwait(false);

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

        [BindProperty]
        public QuarterCodeUnit QuarterCodeUnit { get; set; }


        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.QuarterCodeUnit.Add)]
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }
                //check  if revenue collection code unit has been applied
                var revenueCollectionResp = await _quarterCodeUnitService.FindByQuarterCodeListIdAndFinancialIdAsync(QuarterCodeUnit.QuarterCodeListId, QuarterCodeUnit.FinancialYearId).ConfigureAwait(false);
                if (revenueCollectionResp.Success)
                {
                    string msg = "Revenue collection item has already been applied to the financial year";
                    _logger.LogError(msg, $"Roles.Index Page Error: {msg} " +
                    $"{Environment.NewLine}");
                    ModelState.AddModelError(string.Empty, msg);
                    return Page();
                }
                else
                {             
                    
                    revenueCollectionResp = await _quarterCodeUnitService.AddAsync(QuarterCodeUnit).ConfigureAwait(false);
                }
                if (Referer != null)
                {
                    return Redirect(Referer);
                }
                else
                {
                    return RedirectToPage("./Index");
                }

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"QuarterCodeUnitRx.Add Page Error: {Ex.Message} " +
                    $"{Environment.NewLine}");
                string msg = $"Quarter Code Unit Error:{Ex.Message.ToString()}";
                ModelState.AddModelError(string.Empty, msg);
                //Populate dropdowns
                await PopulateDropDown().ConfigureAwait(false);
                return Page();
            }
        }

        #region Utilities
        private async Task PopulateDropDown()
        {
            long FinancialYearId = 0;
            //get current financial year
            var respCurrentfinancialYear = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
            if (respCurrentfinancialYear.Success)
            {
                FinancialYearId = respCurrentfinancialYear.FinancialYear.ID;
            }
            //Set Financial Year
            var finacialResp = await _financialYearService.ListAsync().ConfigureAwait(false);
            IList<FinancialYear> financialYears = (IList<FinancialYear>)finacialResp;
            var newFinancialYearsList = financialYears
                .OrderByDescending(v => v.Code)
                .Select(
                p => new
                {
                    ID = p.ID,
                    Code = $"{p.Code}-{p.Revision}"
                }
                    ).ToList();
            if (FinancialYearId == 0)
            {
                ViewData["FinancialYearId"] = new SelectList(newFinancialYearsList, "ID", "Code");
            }
            else
            {
                ViewData["FinancialYearId"] = new SelectList(newFinancialYearsList, "ID", "Code", FinancialYearId);
            }

            var quarterCodeListResp = await _quarterCodeListService.ListAsync().ConfigureAwait(false);
            ViewData["QuarterCodeListId"] = new SelectList((IList<QuarterCodeList>)quarterCodeListResp.QuarterCodeList, "ID", "Name");

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
