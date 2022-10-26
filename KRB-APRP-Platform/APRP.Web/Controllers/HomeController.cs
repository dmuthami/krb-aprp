using Microsoft.AspNetCore.Mvc;
using APRP.Web.Models;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using APRP.Web.ViewModels;
using Microsoft.AspNetCore.Http.Extensions;

namespace APRP.Web.Controllers
{
    [Authorize]
    //[TypeFilter(typeof(Extensions.Filters.SessionTimeoutAttribute))]
    public class HomeController : Controller
    {
        private readonly IRoadService _roadService;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly ILogger _logger;
        private readonly IBudgetCeilingHeaderService _budgetCeilingHeaderService;
        private readonly IARICSService _aRICSService;
        private readonly IKenHARoadService _kenHARoadService;
        private readonly IKeRRARoadService _keRRARoadService;
        private readonly IKuRARoadService _kuRARoadService;
        private readonly ICountiesRoadService _countiesRoadService;
        private readonly IRoadClassificationService _roadClassificationService;
        private readonly IBudgetCeilingService _budgetCeilingService;
        private readonly IBudgetCeilingComputationService _budgetCeilingComputationService;
        private readonly IFinancialYearService _financialYearService;
        private readonly IDisbursementService _disbursementService;



        public HomeController(IRoadService roadService, IApplicationUsersService applicationUsersService,
            ILogger<HomeController> logger, IAuthorityService authorityService,
            IBudgetCeilingHeaderService budgetCeilingHeaderService,
            IARICSService aRICSService, IKenHARoadService kenHARoadService,
            IKeRRARoadService keRRARoadService, IKuRARoadService kuRARoadService,
            ICountiesRoadService countiesRoadService, IRoadClassificationService roadClassificationService,
            IBudgetCeilingService budgetCeilingService, IBudgetCeilingComputationService budgetCeilingComputationService,
            IFinancialYearService financialYearService, IDisbursementService disbursementService)
        {
            _roadService = roadService;
            _applicationUsersService = applicationUsersService;
            _logger = logger;
            _authorityService = authorityService;
            _budgetCeilingHeaderService = budgetCeilingHeaderService;
            _aRICSService = aRICSService;
            _kenHARoadService = kenHARoadService;
            _keRRARoadService = keRRARoadService;
            _kuRARoadService = kuRARoadService;
            _countiesRoadService = countiesRoadService;
            _roadClassificationService = roadClassificationService;
            _budgetCeilingService = budgetCeilingService;
            _budgetCeilingComputationService = budgetCeilingComputationService;
            _financialYearService = financialYearService;
            _disbursementService = disbursementService;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Dashboard()
        {
            DashboardViewModel dashboardViewModel = new DashboardViewModel();
            var user = await GetLoggedInUser().ConfigureAwait(false);
            try
            {
                //Budget
                //dashboardViewModel.BudgetCeilingAmount = 0.0d;
                ApplicationUser _ApplicationUser = null;
                Authority Authority = null;

                //budget 2
                _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);
                Authority = _ApplicationUser.Authority;

                //Try to get current financial year
                var currentFinancialResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
                if (currentFinancialResp.Success)
                {
                    if (Authority.Code.ToLower() == "krb")
                    {
                        //Get Sub-TOTAL- Funds Allocated
                        double SubtotalFundsAllocated = await GetBudgetVote("26", currentFinancialResp.FinancialYear.ID).ConfigureAwait(false);

                        //Get county allocation
                        dashboardViewModel.TotalCGCeilingBudget = await GetCountyAllocation(currentFinancialResp.FinancialYear.ID).ConfigureAwait(false);

                        //Get Road annuity fund
                        double roadAnnuityFund = await GetBudgetVote("9", currentFinancialResp.FinancialYear.ID).ConfigureAwait(false);

                        //Total RA Ceiling Budget
                        dashboardViewModel.TotalRACeilingBudget = SubtotalFundsAllocated - (dashboardViewModel.TotalCGCeilingBudget
                            + roadAnnuityFund);

                        //Get Total Disbursed Funds
                        dashboardViewModel.TotalDisbursedFunds = await GetTotalDisbursedFunds(currentFinancialResp.FinancialYear.ID).ConfigureAwait(false);

                        //Disbursement Balance
                        dashboardViewModel.TotalDisbursementBalance = dashboardViewModel.TotalCGCeilingBudget + dashboardViewModel.TotalRACeilingBudget -
                            dashboardViewModel.TotalDisbursedFunds;
                    }
                    else if (Authority.Code.ToLower() == "kenha")
                    {

                        //get kenha budget
                        dashboardViewModel.TotalAuthorityCeilingBudget = await GetBudgetVote("17", currentFinancialResp.FinancialYear.ID).ConfigureAwait(false);

                        //Get total disbursements for KENHA
                        dashboardViewModel.TotalAuthorityDisbursedFunds = 0d;
                        var disbursementResp = await _disbursementService.SummarizeByFinancialYearIDAndAuthorityIDAsync(currentFinancialResp.FinancialYear.ID).ConfigureAwait(false);
                        if (disbursementResp.Success)
                        {
                            var objectResult = (ObjectResult)disbursementResp.IActionResult;
                            if (objectResult != null)
                            {
                                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    var result = (OkObjectResult)objectResult;
                                    IList<DisbursementSummaryViewModel> disbursementSummaryViewModels = (IList<DisbursementSummaryViewModel>)result.Value;

                                    IEnumerable<DisbursementSummaryViewModel> disbursementSummaryViewModelsFilter = disbursementSummaryViewModels.
                                        Where(x => x.AuthorityId == Authority.ID);
                                    if (disbursementSummaryViewModelsFilter.Any())
                                    {
                                        dashboardViewModel.TotalAuthorityDisbursedFunds = disbursementSummaryViewModelsFilter.Sum(x => x.TotalDisbursement);
                                    }

                                }
                            }
                        }

                        //Total disbursements balance
                        dashboardViewModel.TotalAuthorityBudgetBalance = dashboardViewModel.TotalAuthorityCeilingBudget
                            - dashboardViewModel.TotalAuthorityDisbursedFunds;

                    }
                    else if (Authority.Code.ToLower() == "kerra")
                    {
                        //get kerra budget
                        dashboardViewModel.TotalAuthorityCeilingBudget = await GetBudgetVote("30", currentFinancialResp.FinancialYear.ID).ConfigureAwait(false);

                        //Get total disbursements for kerra
                        dashboardViewModel.TotalAuthorityDisbursedFunds = 0d;
                        var disbursementResp = await _disbursementService.SummarizeByFinancialYearIDAndAuthorityIDAsync(currentFinancialResp.FinancialYear.ID).ConfigureAwait(false);
                        if (disbursementResp.Success)
                        {
                            var objectResult = (ObjectResult)disbursementResp.IActionResult;
                            if (objectResult != null)
                            {
                                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    var result = (OkObjectResult)objectResult;
                                    IList<DisbursementSummaryViewModel> disbursementSummaryViewModels = (IList<DisbursementSummaryViewModel>)result.Value;
                                    IEnumerable<DisbursementSummaryViewModel> disbursementSummaryViewModelsFilter = disbursementSummaryViewModels.
                                        Where(x => x.AuthorityId == Authority.ID);
                                    if (disbursementSummaryViewModelsFilter.Any())
                                    {
                                        dashboardViewModel.TotalAuthorityDisbursedFunds = disbursementSummaryViewModelsFilter.Sum(x => x.TotalDisbursement);
                                    }
                                }
                            }
                        }

                        //Total disbursements balance
                        dashboardViewModel.TotalAuthorityBudgetBalance = dashboardViewModel.TotalAuthorityCeilingBudget
                            - dashboardViewModel.TotalAuthorityDisbursedFunds;


                    }
                    else if (Authority.Code.ToLower() == "kura")
                    {
                        //get kura budget
                        dashboardViewModel.TotalAuthorityCeilingBudget = await GetBudgetVote("22", currentFinancialResp.FinancialYear.ID).ConfigureAwait(false);

                        //Get total disbursements for kura
                        dashboardViewModel.TotalAuthorityDisbursedFunds = 0d;
                        var disbursementResp = await _disbursementService.SummarizeByFinancialYearIDAndAuthorityIDAsync(currentFinancialResp.FinancialYear.ID).ConfigureAwait(false);
                        if (disbursementResp.Success)
                        {
                            var objectResult = (ObjectResult)disbursementResp.IActionResult;
                            if (objectResult != null)
                            {
                                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    var result = (OkObjectResult)objectResult;
                                    IList<DisbursementSummaryViewModel> disbursementSummaryViewModels = (IList<DisbursementSummaryViewModel>)result.Value;
                                    IEnumerable<DisbursementSummaryViewModel> disbursementSummaryViewModelsFilter = disbursementSummaryViewModels.
                                        Where(x => x.AuthorityId == Authority.ID);
                                    if (disbursementSummaryViewModelsFilter.Any())
                                    {
                                        dashboardViewModel.TotalAuthorityDisbursedFunds = disbursementSummaryViewModelsFilter.Sum(x => x.TotalDisbursement);
                                    }
                                }
                            }
                        }

                        //Total disbursements balance
                        dashboardViewModel.TotalAuthorityBudgetBalance = dashboardViewModel.TotalAuthorityCeilingBudget
                            - dashboardViewModel.TotalAuthorityDisbursedFunds;

                    }
                    else if (Authority.Code.ToLower() == "kws")
                    {
                        //get kws budget
                        dashboardViewModel.TotalAuthorityCeilingBudget = await GetBudgetVote("21", currentFinancialResp.FinancialYear.ID).ConfigureAwait(false);

                        //Get total disbursements for kws
                        dashboardViewModel.TotalAuthorityDisbursedFunds = 0d;
                        var disbursementResp = await _disbursementService.SummarizeByFinancialYearIDAndAuthorityIDAsync(currentFinancialResp.FinancialYear.ID).ConfigureAwait(false);
                        if (disbursementResp.Success)
                        {
                            var objectResult = (ObjectResult)disbursementResp.IActionResult;
                            if (objectResult != null)
                            {
                                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    var result = (OkObjectResult)objectResult;
                                    IList<DisbursementSummaryViewModel> disbursementSummaryViewModels = (IList<DisbursementSummaryViewModel>)result.Value;
                                    IEnumerable<DisbursementSummaryViewModel> disbursementSummaryViewModelsFilter = disbursementSummaryViewModels.
                                        Where(x => x.AuthorityId == Authority.ID);
                                    if (disbursementSummaryViewModelsFilter.Any())
                                    {
                                        dashboardViewModel.TotalAuthorityDisbursedFunds = disbursementSummaryViewModelsFilter.Sum(x => x.TotalDisbursement);
                                    }
                                }
                            }
                        }

                        //Total disbursements balance
                        dashboardViewModel.TotalAuthorityBudgetBalance = dashboardViewModel.TotalAuthorityCeilingBudget
                            - dashboardViewModel.TotalAuthorityDisbursedFunds;

                    }
                    else if ((Authority.Type == 2))
                    {

                        //get specific county allocation in current financial year
                        dashboardViewModel.TotalAuthorityCeilingBudget = 0d;
                        dashboardViewModel.TotalAuthorityDisbursedFunds = 0d;
                        var resp = await _budgetCeilingHeaderService.FindCurrentAsync().ConfigureAwait(false);
                        if (resp.Success)
                        {
                            var respBudgetCeiling = await _budgetCeilingService.FindByBudgetHeaderIDAndAuthority(resp.BudgetCeilingHeader.ID,
                                Authority.ID).ConfigureAwait(false);
                            if (respBudgetCeiling.Success)
                            {
                                dashboardViewModel.TotalAuthorityCeilingBudget = respBudgetCeiling.BudgetCeiling.Amount;
                            }
                        }
                        //get specific county disbursement in financial year of ineterest
                        var disbursementResp = await _disbursementService.FindByAuthorityIdAndFinancialYearIdAsync(Authority.ID, currentFinancialResp.FinancialYear.ID).ConfigureAwait(false);
                        if (disbursementResp.Disbursement != null)
                        {
                            dashboardViewModel.TotalAuthorityDisbursedFunds = disbursementResp.Disbursement.Amount;
                        }
                        else
                        {
                            dashboardViewModel.TotalAuthorityDisbursedFunds = 0d;
                        }
                       

                        //get the balance
                        dashboardViewModel.TotalAuthorityBudgetBalance = dashboardViewModel.TotalAuthorityCeilingBudget
                        - dashboardViewModel.TotalAuthorityDisbursedFunds;
                    }
                    else
                    {
                        //To do: Not Sure
                    }

                    dashboardViewModel.FinancialYear = currentFinancialResp.FinancialYear;
                }
                else
                {
                    //ToDo: Not sure
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"HomeController.Dashboard Error {Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "Sign up individual page has reloaded");
            }
            //Set Return URL and store in session
            HttpContext.Session.SetString("Referer", Request.GetEncodedUrl());
            ViewBag.Authority = user.Authority;
            if (user.Authority.Type == 0) // KRB & Consultants
            {
                return View(dashboardViewModel);
            }
            else
            {
                return View("Dashboard2", dashboardViewModel);
            }
        }

        public async Task<IActionResult> Dashboard2()
        {
            DashboardViewModel dashboardViewModel = new DashboardViewModel();
            try
            {
                //Round Count
                var roadListResponse = await _roadService.ListAsync().ConfigureAwait(false);
                dashboardViewModel.AllRoadsCount = ((IList<Road>)roadListResponse.Roads).Count;

                //Budget
                dashboardViewModel.BudgetCeilingAmount = 0.0d;
                var resp = await _budgetCeilingHeaderService.FindCurrentAsync().ConfigureAwait(false);
                if (resp.Success)
                {
                    ApplicationUser _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);
                    Authority Authority = _ApplicationUser.Authority;
                    if (Authority.Code == "KRB")
                    {
                        dashboardViewModel.BudgetCeilingAmount = resp.BudgetCeilingHeader.TotalAmount;
                    }
                    else
                    {
                        var respBudgetCeiling = await _budgetCeilingService.FindByBudgetHeaderIDAndAuthority(resp.BudgetCeilingHeader.ID,
                            Authority.ID).ConfigureAwait(false);
                        if (respBudgetCeiling.Success)
                        {
                            dashboardViewModel.BudgetCeilingAmount = respBudgetCeiling.BudgetCeiling.Amount;
                        }
                    }
                    dashboardViewModel.BudgetCeilingHeader = resp.BudgetCeilingHeader;
                }



                //Get count of roads with ARICS
                var aricsResp = await _aRICSService.GetARICEDRoads(null).ConfigureAwait(false);
                dashboardViewModel.ARICEDRoadsCount = ((IList<Road>)aricsResp.Roads).Count;

                //Get roads without arics
                dashboardViewModel.AllRoadsWithoutARICS = dashboardViewModel.AllRoadsCount - dashboardViewModel.ARICEDRoadsCount;

                //Get statistics for Kenha
                dashboardViewModel.KenhaStatistics = (await _kenHARoadService.ListByRoadClassAsync().ConfigureAwait(false)).Dictionary;

                //Get statistics for kerra
                dashboardViewModel.KeRRaStatistics = (await _keRRARoadService.ListByRoadClassAsync().ConfigureAwait(false)).Dictionary;

                //Get statistics for kura
                dashboardViewModel.KuRAStatistics = (await _kuRARoadService.ListByRoadClassAsync().ConfigureAwait(false)).Dictionary;

                //Get statistics for kura
                dashboardViewModel.CountiesStatistics = (await _countiesRoadService.ListByRoadClassAsync().ConfigureAwait(false)).Dictionary;

                //Get Road Request Statistics
                var roadRequestClassResp = await _roadClassificationService.ListByStatusAsync().ConfigureAwait(false);
                dashboardViewModel.KRBRoadRequestsStatistics = roadRequestClassResp.Dictionary;

                long count = 0;
                bool result = long.TryParse(dashboardViewModel.KRBRoadRequestsStatistics["ApprovedAtKRB"].ToString(), out count);
                dashboardViewModel.KRBRoadRequestsCount += count;
                result = long.TryParse(dashboardViewModel.KRBRoadRequestsStatistics["RejectedAtKRB"].ToString(), out count);
                dashboardViewModel.KRBRoadRequestsCount += count;
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"HomeController.Dashboard Error {Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "Sign up individual page has reloaded");
            }
            //Set Return URL and store in session
            HttpContext.Session.SetString("Referer", Request.GetEncodedUrl());

            return View(dashboardViewModel);
        }

        public async Task<IActionResult> Dashboard3()
        {
            DashboardViewModel dashboardViewModel = new DashboardViewModel();
            try
            {
                //Round Count
                var roadListResponse = await _roadService.ListAsync().ConfigureAwait(false);
                dashboardViewModel.AllRoadsCount = ((IList<Road>)roadListResponse.Roads).Count;

                //Budget
                dashboardViewModel.BudgetCeilingAmount = 0.0d;
                var resp = await _budgetCeilingHeaderService.FindCurrentAsync().ConfigureAwait(false);
                if (resp.Success)
                {
                    ApplicationUser _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);
                    Authority Authority = _ApplicationUser.Authority;
                    if (Authority.Code == "KRB")
                    {
                        dashboardViewModel.BudgetCeilingAmount = resp.BudgetCeilingHeader.TotalAmount;
                    }
                    else
                    {
                        var respBudgetCeiling = await _budgetCeilingService.FindByBudgetHeaderIDAndAuthority(resp.BudgetCeilingHeader.ID,
                            Authority.ID).ConfigureAwait(false);
                        if (respBudgetCeiling.Success)
                        {
                            dashboardViewModel.BudgetCeilingAmount = respBudgetCeiling.BudgetCeiling.Amount;
                        }
                    }
                    dashboardViewModel.BudgetCeilingHeader = resp.BudgetCeilingHeader;
                }



                //Get count of roads with ARICS
                var aricsResp = await _aRICSService.GetARICEDRoads(null).ConfigureAwait(false);
                dashboardViewModel.ARICEDRoadsCount = ((IList<Road>)aricsResp.Roads).Count;

                //Get roads without arics
                dashboardViewModel.AllRoadsWithoutARICS = dashboardViewModel.AllRoadsCount - dashboardViewModel.ARICEDRoadsCount;

                //Get statistics for Kenha
                dashboardViewModel.KenhaStatistics = (await _kenHARoadService.ListByRoadClassAsync().ConfigureAwait(false)).Dictionary;

                //Get statistics for kerra
                dashboardViewModel.KeRRaStatistics = (await _keRRARoadService.ListByRoadClassAsync().ConfigureAwait(false)).Dictionary;

                //Get statistics for kura
                dashboardViewModel.KuRAStatistics = (await _kuRARoadService.ListByRoadClassAsync().ConfigureAwait(false)).Dictionary;

                //Get statistics for kura
                dashboardViewModel.CountiesStatistics = (await _countiesRoadService.ListByRoadClassAsync().ConfigureAwait(false)).Dictionary;

                //Get Road Request Statistics
                var roadRequestClassResp = await _roadClassificationService.ListByStatusAsync().ConfigureAwait(false);
                dashboardViewModel.KRBRoadRequestsStatistics = roadRequestClassResp.Dictionary;

                long count = 0;
                bool result = long.TryParse(dashboardViewModel.KRBRoadRequestsStatistics["ApprovedAtKRB"].ToString(), out count);
                dashboardViewModel.KRBRoadRequestsCount += count;
                result = long.TryParse(dashboardViewModel.KRBRoadRequestsStatistics["RejectedAtKRB"].ToString(), out count);
                dashboardViewModel.KRBRoadRequestsCount += count;
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"HomeController.Dashboard Error {Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "Sign up individual page has reloaded");
            }
            //Set Return URL and store in session
            HttpContext.Session.SetString("Referer", Request.GetEncodedUrl());

            return View(dashboardViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return base.View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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

        #region Utilities
        private async Task<double> GetCountyAllocation(long FinancialYearId)
        {
            BudgetCeilingComputation budgetCeilingComputation = null;
            var authorityResp = await _authorityService.FindByCodeAsync("CGs").ConfigureAwait(false);
            if (authorityResp.Success)
            {
                // other authority types
                var bugetCelingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("14", FinancialYearId).ConfigureAwait(false);
                if (bugetCelingComputationResp.Success)
                {
                    var objectResult = (ObjectResult)bugetCelingComputationResp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result2 = (OkObjectResult)objectResult;
                            budgetCeilingComputation = (BudgetCeilingComputation)result2.Value;
                        }
                    }
                }
            }

            if (budgetCeilingComputation == null)
            {
                return 0d;
            }
            else
            {
                return budgetCeilingComputation.Amount;
            }
        }
        private async Task<double> GetBudgetVote(string Code, long FinancialYearId)
        {
            BudgetCeilingComputation budgetCeilingComputation = null;
            var bugetCelingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync(Code, FinancialYearId).ConfigureAwait(false);
            if (bugetCelingComputationResp.Success)
            {
                var objectResult = (ObjectResult)bugetCelingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result2 = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result2.Value;
                    }
                }
            }

            if (budgetCeilingComputation == null)
            {
                return 0d;
            }
            else
            {
                return budgetCeilingComputation.Amount;
            }
        }

        private async Task<double> GetTotalDisbursedFunds(long FinancialYearId)
        {
            IEnumerable<Disbursement> disbursements = null;
            var disbursementServiceResp = await _disbursementService.ListAsync(FinancialYearId).ConfigureAwait(false);
            if (disbursementServiceResp.Success)
            {
                disbursements = disbursementServiceResp.Disbursement;
            }

            if (!disbursements.Any())
            {
                return 0d;
            }
            else
            {
                return disbursements.Sum(x => x.Amount);
            }
        }
        #endregion
    }
}
