using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace APRP.Web.MyPages.RevenueCollectionCodeUnitRx
{

    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class CreateModel : PageModel
    {
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly ILogger _logger;
        private readonly IRevenueCollectionCodeUnitService _revenueCollectionCodeUnitService;
        private readonly IFinancialYearService _financialYearService;
        private readonly IRevenueCollectionCodeUnitTypeService _revenueCollectionCodeUnitTypeService;
        private readonly IFundingSourceService _fundingSourceService;


        public CreateModel(ILogger<CreateModel> logger,
            IApplicationUsersService applicationUsersService, IAuthorityService authorityService,
            IRevenueCollectionCodeUnitService revenueCollectionCodeUnitService,
            IFinancialYearService financialYearService,
            IRevenueCollectionCodeUnitTypeService revenueCollectionCodeUnitTypeService,
            IFundingSourceService fundingSourceService)
        {
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _logger = logger;
            _revenueCollectionCodeUnitService = revenueCollectionCodeUnitService;
            _financialYearService = financialYearService;
            _revenueCollectionCodeUnitTypeService = revenueCollectionCodeUnitTypeService;
            _fundingSourceService = fundingSourceService;
        }

        public Authority Authority { get; set; }

        [BindProperty]
        public RevenueCollectionCodeUnit RevenueCollectionCodeUnit { get; set; }

        [BindProperty]
        public FundingSource FundingSource { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RevenueCollectionCodeUnit.View)]
        public async Task<IActionResult> OnGet()
        {
            try
            {
                //Get logged in user
                ApplicationUser _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);
                Authority = _ApplicationUser.Authority;

                //Populate dropdowns
                await PopulateDropDown().ConfigureAwait(false);

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

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RevenueCollectionCodeUnit.Add)]
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                RevenueCollectionCodeUnit.RevenueStream = RevenueCollectionCodeUnit.RevenueStream;
                var revenueCollectionCodeUnitResp = await _revenueCollectionCodeUnitService.AddAsync(RevenueCollectionCodeUnit).ConfigureAwait(false);

                return RedirectToPage("./Index");
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionCodeUnitRx.Add Page Error: {Ex.Message} " +
                    $"{Environment.NewLine}");
                string msg = $"Revenue Collection Code Unit Error:{Ex.Message.ToString()}";
                ModelState.AddModelError(string.Empty, msg);
                //Populate dropdowns
                await PopulateDropDown().ConfigureAwait(false);
                return Page();
            }
        }

        #region Utilities
        private async Task PopulateDropDown()
        {
            //Set Financial Year
            var finacialResp = await _financialYearService.ListAsync().ConfigureAwait(false);
            IList<FinancialYear> financialYears = (IList<FinancialYear>)finacialResp;
            var newFinancialYearsList = financialYears
                .OrderByDescending(v=>v.Code)
                .Select(
                p => new
                {
                    ID = p.ID,
                    Code = $"{p.Code}_{p.Revision}"
                }
                    ).ToList();
            ViewData["FinancialYearId"] = new SelectList(newFinancialYearsList, "ID", "Code");

            //RevenueCollectionCodeUnitType
            var revenueCollectionCodeUnitTypeResp = await _revenueCollectionCodeUnitTypeService.ListAsync().ConfigureAwait(false);
            IList<RevenueCollectionCodeUnitType> revenueCollectionCodeUnitTypes = (IList<RevenueCollectionCodeUnitType>)revenueCollectionCodeUnitTypeResp.RevenueCollectionCodeUnitType;
            ViewData["RevenueCollectionCodeUnitTypeId"] = new SelectList(revenueCollectionCodeUnitTypes, "ID", "Type");

            var authorityResp = await _authorityService.ListAsync().ConfigureAwait(false);
            IList<Authority> authorities = (IList<Authority>)authorityResp;
            ViewData["AuthorityId"] = new SelectList(authorities, "ID", "Name");

            //Other funding source
            var resp = await _fundingSourceService.ListAsync().ConfigureAwait(false);
            IList<FundingSource> fundingSources = (IList<FundingSource>)resp;
            var newFundingSourcesList = fundingSources
                .OrderBy(v => v.Code)
                .Select(
                p => new
                {
                    ID = p.ID,
                    Code = $"{p.Code}_{p.ShortName}"
                }
                    ).ToList();
            ViewData["FundingSourceId"] = new SelectList(newFundingSourcesList, "ID", "Code");
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
