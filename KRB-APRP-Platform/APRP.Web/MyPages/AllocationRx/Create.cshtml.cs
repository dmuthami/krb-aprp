using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Http.Extensions;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Authorization;

namespace APRP.Web.MyPages.AllocationRx
{
    [Authorize]
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class CreateModel : PageModel
    {
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly ILogger _logger;
        private readonly IAllocationService _allocationService;
        private readonly IFinancialYearService _financialYearService;
        private readonly IAllocationCodeUnitService _allocationCodeUnitService;
        private readonly IRevenueCollectionService _revenueCollectionService;
        private readonly IDisbursementService _disbursementService;
        public CreateModel(ILogger<CreateModel> logger,
            IApplicationUsersService applicationUsersService, IAuthorityService authorityService,
            IAllocationService allocationService,
            IFinancialYearService financialYearService,
            IAllocationCodeUnitService allocationCodeUnitService,
            IRevenueCollectionService revenueCollectionService,
            IDisbursementService disbursementService)
        {
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _logger = logger;
            _allocationService = allocationService;
            _financialYearService = financialYearService;
            _allocationCodeUnitService = allocationCodeUnitService;
            _revenueCollectionService = revenueCollectionService;
            _disbursementService = disbursementService;
        }

        public Authority Authority { get; set; }

        public string Referer { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.Allocation.View)]
        public async Task<IActionResult> OnGet()
        {
            try
            {
                //Get logged in user
                ApplicationUser _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);
                Authority = _ApplicationUser.Authority;

                //Set Previous Return URL
                Referer = Request.Headers["Referer"].ToString();

                var revCollectioncodeUnitsResp = await _allocationCodeUnitService.ListAsync("").ConfigureAwait(false);
                ViewData["AllocationCodeUnitId"] = new SelectList((IList<AllocationCodeUnit>)revCollectioncodeUnitsResp.AllocationCodeUnit, "ID", "Item");

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
        public Allocation Allocation { get; set; }


        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.Allocation.Add)]
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                //check  if revenue collection code unit has been applied
                var revenueCollectionResp = await _allocationService.FindByAllocationCodeUnitIdAsync(Allocation.AllocationCodeUnitId).ConfigureAwait(false);
                if (revenueCollectionResp.Success)
                {
                    string msg = "Revenue collection item has already been applied to the finacial year";
                    _logger.LogError(msg, $"Roles.Index Page Error: {msg} " +
                    $"{Environment.NewLine}");
                    ModelState.AddModelError(string.Empty, msg);
                    return Page();
                }
                else
                {

                    //Get Authority Allocation
                    Allocation.Amount = await GetAuthorityAllocationAmount(Allocation.AllocationCodeUnitId).ConfigureAwait(false);                   
                    
                    revenueCollectionResp = await _allocationService.AddAsync(Allocation).ConfigureAwait(false);
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
                _logger.LogError(Ex, $"AllocationRx.Add Page Error: {Ex.Message} " +
                    $"{Environment.NewLine}");
                return Page();
            }
        }

        #region Utilities

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<double> GetAuthorityAllocationAmount(long AllocationCodeUnitId)
        {
            try
            {
                double allocationAmount = 0d;

                //get current financial year
                var finacialResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
                FinancialYear financialYear = finacialResp.FinancialYear;

                //Get revenue allocations for the year and Revenue collectionsum

                var revenuecollectionResp = await _revenueCollectionService.ListAsync(financialYear.ID, "KRB").ConfigureAwait(false);
                IList<RevenueCollection> revenueCollections = (IList<RevenueCollection>)revenuecollectionResp.RevenueCollection;
                double RevenueCollectionSum = await _revenueCollectionService.RevenueCollectionSum(revenueCollections).ConfigureAwait(false);


                //Get disbursements for the year and disbursementsum
                var disbursementResp = await _disbursementService.ListAsync(financialYear.ID).ConfigureAwait(false);
                IList<Disbursement> disbursements = (IList<Disbursement>)disbursementResp.Disbursement;
                double DisbursementSum = await _disbursementService.DisbursementItemSum(disbursements).ConfigureAwait(false);

                //Get allocation percent
                var allocationCodeUnitResp = await _allocationCodeUnitService.FindByIdAsync(AllocationCodeUnitId).ConfigureAwait(false);
                allocationAmount = (RevenueCollectionSum - DisbursementSum) * allocationCodeUnitResp.AllocationCodeUnit.Percent; //Compute allocation for Authority
               

                return allocationAmount;
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AllocationRx.Add Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                return 0d; ;
            }
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
