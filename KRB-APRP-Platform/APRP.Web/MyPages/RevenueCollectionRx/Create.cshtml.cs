using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Http.Extensions;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Authorization;

namespace APRP.Web.MyPages.RevenueCollectionRx
{
    [Authorize]
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class CreateModel : PageModel
    {
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly ILogger _logger;
        private readonly IRevenueCollectionService _revenueCollectionService;
        private readonly IFinancialYearService _financialYearService;
        private readonly IRevenueCollectionCodeUnitService _revenueCollectionCodeUnitService;
        public CreateModel(ILogger<CreateModel> logger,
            IApplicationUsersService applicationUsersService, IAuthorityService authorityService,
            IRevenueCollectionService revenueCollectionService,
            IFinancialYearService financialYearService,
            IRevenueCollectionCodeUnitService revenueCollectionCodeUnitService)
        {
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _logger = logger;
            _revenueCollectionService = revenueCollectionService;
            _financialYearService = financialYearService;
            _revenueCollectionCodeUnitService = revenueCollectionCodeUnitService;
        }

        public Authority Authority { get; set; }

        public string Referer { get; set; }

        public FinancialYear FinancialYear { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RevenueCollection.View)]
        public async Task<IActionResult> OnGet()
        {
            try
            {
                //Set variables
                await SetVariables();

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
                ModelState.AddModelError(string.Empty, Ex.Message.ToString());
                //Populate dropdowns
                await PopulateDropDown().ConfigureAwait(false);
                return Page();
            }
        }

        [BindProperty]
        public RevenueCollection RevenueCollection { get; set; }


        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RevenueCollection.Add)]
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                ApplicationUser loggedInuser = await GetLoggedInUser().ConfigureAwait(false);
                var appUserResp = await _applicationUsersService.GetRolesAsync(loggedInuser).ConfigureAwait(false);

                //check  if revenue collection code unit has been applied
                var revenueCollectionResp = await _revenueCollectionService.FindByRevenueCollectionCodeUnitIdAsync(RevenueCollection.RevenueCollectionCodeUnitId).ConfigureAwait(false);
                if (revenueCollectionResp.Success)
                {
                    string msg = "Revenue collection item has already been applied to the financial year";
                    _logger.LogError(msg, $"Roles.Index Page Error: {msg} " +
                    $"{Environment.NewLine}");
                    //Set variables
                    await SetVariables();

                    //Populate dropdowns
                    await PopulateDropDown().ConfigureAwait(false);
                    ModelState.AddModelError(string.Empty, msg);
                    return Page();
                }
                else
                {
                    revenueCollectionResp = await _revenueCollectionService.AddAsync(RevenueCollection).ConfigureAwait(false);
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
                _logger.LogError(Ex, $"RevenueCollectionRx.Add Page Error: {Ex.Message} " +
                    $"{Environment.NewLine}");
                return Page();
            }
        }

        #region Utilities
        private async Task PopulateDropDown()
        {
            //Set Financial Year
            var finacialResp = await _financialYearService.ListAsync().ConfigureAwait(false);
            IList<FinancialYear> financialYears = (IList<FinancialYear>)finacialResp;

            ViewData["FinancialYearId"] = new SelectList(financialYears, "ID", "Code");

            if (FinancialYear != null)
            {
                var revCollectioncodeUnitsResp = await _revenueCollectionCodeUnitService.ListAsync(FinancialYear.ID, null).ConfigureAwait(false);
                //if (revCollectioncodeUnitsResp.Success)
                //{
                //    IList<RevenueCollectionCodeUnit> RC= (IList<RevenueCollectionCodeUnit>)revCollectioncodeUnitsResp.RevenueCollectionCodeUnit;
                //    foreach (var item in RC) {
                //        RC.
                //    }
                
                //}
                ViewData["RevenueCollectionCodeUnitId"] = new SelectList((IList<RevenueCollectionCodeUnit>)revCollectioncodeUnitsResp.RevenueCollectionCodeUnit, "ID", "RevenueStream");
            }

        }
        
        private async Task SetVariables()
        {
            //Get logged in user
            ApplicationUser _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);
            Authority = _ApplicationUser.Authority;

            //Set Previous Return URL
            Referer = Request.Headers["Referer"].ToString();

            //Get Current Financial Year
            var finacialResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
            if (!finacialResp.Success)
            {
                string msg = "Unable to get Current Financial Year";
                _logger.LogError(msg, $"RevenueCollectionRX.Create.OnGet Page Error: {msg} " +
                $"{Environment.NewLine}");

                //Populate dropdowns
                await PopulateDropDown().ConfigureAwait(false);

                ModelState.AddModelError(string.Empty, msg);
            }

            //Set financial year
            FinancialYear = finacialResp.FinancialYear;

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
