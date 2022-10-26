using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using APRP.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace APRP.Web.MyPages.AllocationCodeUnitRx
{

    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class CreateModel : PageModel
    {
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly ILogger _logger;
        private readonly IAllocationCodeUnitService _allocationCodeUnitService;
        private readonly IFinancialYearService _financialYearService;
        private readonly IRevenueCollectionService _revenueCollectionService;
        private readonly IDisbursementService _disbursementService;
        private readonly IAllocationService _allocationService;
        private readonly IBudgetCeilingHeaderService _budgetCeilingHeaderService;

        public CreateModel(ILogger<CreateModel> logger,
            IApplicationUsersService applicationUsersService, IAuthorityService authorityService,
            IAllocationCodeUnitService allocationCodeUnitService,
            IFinancialYearService financialYearService,
                        IRevenueCollectionService revenueCollectionService,
            IDisbursementService disbursementService, IAllocationService allocationService,
            IBudgetCeilingHeaderService budgetCeilingHeaderService)
        {
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _logger = logger;
            _allocationCodeUnitService = allocationCodeUnitService;
            _financialYearService = financialYearService;
            _revenueCollectionService = revenueCollectionService;
            _disbursementService = disbursementService;
            _allocationService = allocationService;
            _budgetCeilingHeaderService = budgetCeilingHeaderService;
        }

        public Authority Authority { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.AllocationCodeUnit.View)]
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
        public AllocationCodeUnit AllocationCodeUnit { get; set; }


        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.AllocationCodeUnit.Add)]
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                ////check  if revenue collection code unit has been applied
                //var revenueCollectionResp = await _allocationCodeUnitService.FindByAuthorityAndFinancialIdAsync(AllocationCodeUnit.AuthorityId, AllocationCodeUnit.FinancialYearId).ConfigureAwait(false);
                //if (revenueCollectionResp.Success)
                //{
                //    string msg = "Revenue Collection Code Unit item has an existing item with a similar name";
                //    _logger.LogError(msg, $"Roles.Index Page Error: {msg} " +
                //    $"{Environment.NewLine}");
                //    ModelState.AddModelError(string.Empty, msg);
                //    return Page();
                //}
                //else
                //{
                   
                //}
                var allocationCodeUnitResp = await _allocationCodeUnitService.AddAsync(AllocationCodeUnit).ConfigureAwait(false);
                //update authority allocation values
                await SetAuthorityAllocationAmounts().ConfigureAwait(false);

                return RedirectToPage("./Index");
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AllocationCodeUnitRx.Add Page Error: {Ex.Message} " +
                    $"{Environment.NewLine}");
                return Page();
            }
        }

        #region Utilities
        private async Task SetAuthorityAllocationAmounts()
        {
            try
            {
                RevenueCollectionViewModel revenueCollectionViewModel = new RevenueCollectionViewModel();

                //Set Financial Year
                var finacialResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
                revenueCollectionViewModel.FinacialYear = finacialResp.FinancialYear;

                //Get Revenue collection for the year
                var revenuecollectionResp = await _revenueCollectionService.ListAsync(finacialResp.FinancialYear.ID, "KRB").ConfigureAwait(false);
                revenueCollectionViewModel.RevenueCollection = (IList<RevenueCollection>)revenuecollectionResp.RevenueCollection;
                revenueCollectionViewModel.RevenueCollectionSum = await _revenueCollectionService.RevenueCollectionSum(revenueCollectionViewModel.RevenueCollection).ConfigureAwait(false);

                //Disbursements
                var disbursementResp = await _disbursementService.ListAsync(finacialResp.FinancialYear.ID).ConfigureAwait(false);
                revenueCollectionViewModel.Disbursement = (IList<Disbursement>)disbursementResp.Disbursement;
                revenueCollectionViewModel.DisbursementSum = await _disbursementService.DisbursementItemSum(revenueCollectionViewModel.Disbursement).ConfigureAwait(false);

                //Get allocationscodeunits for the year of interest
                var allocationCodeUnitResp = await _allocationCodeUnitService.ListAsync("").ConfigureAwait(false);

                double allocationAmount = 0d;
                foreach (AllocationCodeUnit allocationCodeUnit in allocationCodeUnitResp.AllocationCodeUnit)
                {
                    //check  if revenue collection code unit has been applied
                    var allocationCodeUnitResp2 = await _allocationService.FindByAllocationCodeUnitIdAsync(allocationCodeUnit.ID).ConfigureAwait(false);
                    if (!allocationCodeUnitResp2.Success)//is false
                    {
                        //Get allocation percent                    
                        allocationAmount = (revenueCollectionViewModel.RevenueCollectionSum - revenueCollectionViewModel.DisbursementSum) * allocationCodeUnit.Percent; //Compute allocation for Authority
                        Allocation allocation = new Allocation();
                        allocation.AllocationCodeUnitId = allocationCodeUnit.ID;
                        allocation.Amount = allocationAmount;
                        var allocationResp = await _allocationService.AddAsync(allocation).ConfigureAwait(false);

                    }
                    else
                    {
                        //Update
                        Allocation allocation = allocationCodeUnitResp2.Allocation;

                        //Check if budget is approved
                        var resp = await _budgetCeilingHeaderService.FindCurrentAsync().ConfigureAwait(false);
                        if (resp.Success)
                        {
                            BudgetCeilingHeader budgetCeilingHeader = resp.BudgetCeilingHeader;
                            if (budgetCeilingHeader.ApprovalStatus != 2)
                            {
                                allocationAmount = (revenueCollectionViewModel.RevenueCollectionSum - revenueCollectionViewModel.DisbursementSum) * allocationCodeUnit.Percent; //Compute allocation for Authority
                                allocation.Amount = allocationAmount;
                                var allocationResp = await _allocationService.Update(allocation.ID, allocation).ConfigureAwait(false);
                            }
                        }
                    }
                }

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionService.Update Error: {Environment.NewLine}");
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
