using System.Globalization;
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using APRP.Web.ViewModels;
using APRP.Web.ViewModels.ResponseTypes;
using APRP.Web.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Task = System.Threading.Tasks.Task;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APRP.Web.Controllers
{
    [Authorize]
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutAttribute))]
    public class WorkplanController : Controller
    {
        private readonly IRoadWorkBudgetLineService _roadWorkBudgetLinesService;
        private readonly IRoadWorkBudgetHeaderService _roadWorkBudgetHeaderService;
        private readonly IFundingSourceService _fundingSourceService;
        private readonly IFundTypeService _fundTypeService;
        private readonly IRoadService _roadService;
        private readonly IRoadSectionService _roadSectionService;
        private readonly IRoadWorkOperationalActivitiesBudgetService _roadWorkOperationalActivitiesBudgetService;
        private readonly IRoadWorkSectionPlanService _roadWorkSectionPlanService;
        private readonly IExecutionMethodService _executionMethodService;
        private readonly IWorkCategoryService _workCategoryService;
        private readonly IItemActivityUnitCostService _itemActivityUnitCostService;
        private readonly IItemActivityPBCService _itemActivityPBCService;
        private readonly IPlanActivityService _planActivityService;
        private readonly IPlanActivityPBCService _planActivityPBCService;
        private readonly ITechnologyService _technologyService;
        private readonly IAuthorityService _authorityService;
        private readonly IFinancialYearService _financialYearService;
        private readonly IBudgetCeilingService _budgetCeilingService;
        private readonly IBudgetCeilingHeaderService _budgetCeilingHeaderService;
        private readonly IARICSService _aRICSService;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IWorkplanApprovalBatchService _workplanApprovalBatchService;
        private readonly IWorkplanApprovalHistoryService _workplanApprovalHistoryService;
        private readonly IWorkplanUploadService _workplanUploadService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;
        private readonly IRoadPrioritizationService _roadPrioritizationService;
        private readonly IRoadConditionService _roadConditionService;
        private readonly IRevenueCollectionCodeUnitService _revenueCollectionCodeUnitService;
        private readonly IRegionService _regionService;
        private readonly IContractService _contractService;
        private readonly IUploadService _uploadService;
        private readonly IARICSYearService _aRICSYearService;
        private readonly IAdminOperationalActivityService _adminOperationalActivityService;
        private readonly IBudgetCeilingComputationService _budgetCeilingComputationService;


        public WorkplanController(
            IRoadWorkBudgetLineService roadWorkBudgetLinesService,
            IRoadWorkBudgetHeaderService roadWorkBudgetHeaderService,
            IFundingSourceService fundingSourceService,
            IFundTypeService fundTypeService,
            IRoadService roadService,
            IRoadSectionService roadSectionService,
            IRoadWorkOperationalActivitiesBudgetService roadWorkOperationalActivitiesBudgetService,
            IRoadWorkSectionPlanService roadWorkSectionPlanService,
            IExecutionMethodService executionMethodService,
            IWorkCategoryService workCategoryService,
            IItemActivityUnitCostService itemActivityUnitCostService,
            IItemActivityPBCService itemActivityPBCService,
            IPlanActivityService planActivityService,
            IPlanActivityPBCService planActivityPBCService,
            ITechnologyService technologyService,
            IAuthorityService authorityService,
            IFinancialYearService financialYearService,
            IBudgetCeilingService budgetCeilingService,
            IBudgetCeilingHeaderService budgetCeilingHeaderService,
            IARICSService aRICSService,
            IApplicationUsersService applicationUsersService,
            IWorkplanApprovalBatchService workplanApprovalBatchService,
            IWorkplanApprovalHistoryService workplanApprovalHistoryService,
            IWorkplanUploadService workplanUploadService,
            IWebHostEnvironment hostingEnvironment,
            IMemoryCache cache,
            ILogger<WorkplanController> logger,
            IRoadPrioritizationService roadPrioritizationService,
            IRoadConditionService roadConditionService,
            IRevenueCollectionCodeUnitService revenueCollectionCodeUnitService,
            IRegionService regionService,
            IContractService contractService,
            IUploadService uploadService,
            IARICSYearService aRICSYearService,
            IAdminOperationalActivityService adminOperationalActivityService,
            IBudgetCeilingComputationService budgetCeilingComputationService
        )
        {
            _roadWorkBudgetLinesService = roadWorkBudgetLinesService;
            _roadWorkBudgetHeaderService = roadWorkBudgetHeaderService;
            _fundingSourceService = fundingSourceService;
            _fundTypeService = fundTypeService;
            _roadService = roadService;
            _roadSectionService = roadSectionService;
            _roadWorkOperationalActivitiesBudgetService = roadWorkOperationalActivitiesBudgetService;
            _roadWorkSectionPlanService = roadWorkSectionPlanService;
            _executionMethodService = executionMethodService;
            _workCategoryService = workCategoryService;
            _itemActivityUnitCostService = itemActivityUnitCostService;
            _itemActivityPBCService = itemActivityPBCService;
            _planActivityService = planActivityService;
            _planActivityPBCService = planActivityPBCService;
            _technologyService = technologyService;
            _authorityService = authorityService;
            _financialYearService = financialYearService;
            _budgetCeilingService = budgetCeilingService;
            _budgetCeilingHeaderService = budgetCeilingHeaderService;
            _aRICSService = aRICSService;
            _applicationUsersService = applicationUsersService;
            _workplanApprovalBatchService = workplanApprovalBatchService;
            _workplanApprovalHistoryService = workplanApprovalHistoryService;
            _workplanUploadService = workplanUploadService;
            _cache = cache;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            _roadPrioritizationService = roadPrioritizationService;
            _roadConditionService = roadConditionService;
            _revenueCollectionCodeUnitService = revenueCollectionCodeUnitService;
            _uploadService = uploadService;
            _regionService = regionService;
            _contractService = contractService;
            _aRICSYearService = aRICSYearService;
            _adminOperationalActivityService = adminOperationalActivityService;
            _budgetCeilingComputationService = budgetCeilingComputationService;
        }


        // Section handling budgetline line configurations

        public async Task<IActionResult> GetRoadWorksBudgeting()
        {
            RoadBudgetLineViewModel roadBudgetLineViewModel = new RoadBudgetLineViewModel();

            var user = await GetLoggedInUser().ConfigureAwait(false);
            var authority = new Authority();
            if (user != null)
            {
                var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                ViewBag.Authority = userAuthority;
                authority = userAuthority.Authority;
            }

            ViewBag.Authority = user.Authority;

            //get current financial year
            var fYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);



            //check if budget has been defined for the authority in the current year
            if (fYearResp.Success)
            {
                var budgerHeader = await _roadWorkBudgetHeaderService.FindByAuthorityIdForCurrentYear(fYearResp.FinancialYear.ID, authority.ID).ConfigureAwait(false);

                if (budgerHeader.Success)
                {
                    roadBudgetLineViewModel.RoadWorkBudgetHeader = budgerHeader.RoadWorkBudgetHeader;
                    roadBudgetLineViewModel.RoadWorkBudgetLines = budgerHeader.RoadWorkBudgetHeader.RoadWorkBudgetLines;
                }
                else
                {
                    roadBudgetLineViewModel.RoadWorkBudgetHeader = new RoadWorkBudgetHeader
                    {
                        Code = fYearResp.FinancialYear.Code,
                        Summary = "Budget Plan for " + authority.Name + " for Fiscal Year : " + fYearResp.FinancialYear.Code
                    };
                    roadBudgetLineViewModel.RoadWorkBudgetLines = Enumerable.Empty<RoadWorkBudgetLine>();
                }

                //get the agency budget ceiling set
                var ceilingResp = await _budgetCeilingService.FindApprovedByAuthorityForCurrentYear(fYearResp.FinancialYear.ID, authority.ID).ConfigureAwait(false);
                if (ceilingResp.Success)
                {
                    roadBudgetLineViewModel.BudgetCeiling = ceilingResp.BudgetCeiling;
                }
            }
            return View("GetRoadWorksBudeting", roadBudgetLineViewModel);
        }

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

        /// <summary>
        /// returns partial view for budgetline editing or creation
        /// </summary>
        /// <param name="BudgetLineId"></param>
        /// <returns></returns>
        public async Task<IActionResult> ShowBudgetLine(long BudgetLineId)
        {
            RoadWorkBudgetLine model = new RoadWorkBudgetLine();

            var user = await GetLoggedInUser().ConfigureAwait(false);
            var authority = new Authority();
            if (user != null)
            {
                var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                authority = userAuthority.Authority;
            }


            //retrieving funding source for use in create view
            // var fundingSources = await _fundingSourceService.ListAsync().ConfigureAwait(false);
            var fundingSources = await _fundingSourceService.ListAsync().ConfigureAwait(false);
            var fundTypes = await _fundTypeService.ListAsync().ConfigureAwait(false);

            ViewBag.FundingSourceList = new SelectList(fundingSources, "ID", "Name");
            ViewBag.FundTypeList = new SelectList(fundTypes, "ID", "Name");

            if (BudgetLineId > 0)
            {
                //get budget line details
                var budgetLineResponse = await _roadWorkBudgetLinesService.FindByIdAsync(BudgetLineId).ConfigureAwait(false);
                if (budgetLineResponse.Success)
                {
                    model = budgetLineResponse.RoadWorkBudgetLine;
                }
            }

            return PartialView("BudgetLineIdPartialView", model);
        }

        /// <summary>
        /// Handles submitted form data for budgetline creation or modification
        /// </summary>
        /// <param name="roadWorkBudgetLine"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateBudgetLine(RoadWorkBudgetLine roadWorkBudgetLine)
        {
            if (roadWorkBudgetLine != null)
            {
                var user = await GetLoggedInUser().ConfigureAwait(false);
                var authority = new Authority();
                if (user != null)
                {
                    var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                    ViewBag.Authority = userAuthority;
                    authority = userAuthority.Authority;

                }

                //get current financial year
                var fYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);

                //check if budget has been defined for the authority in the current year
                if (fYearResp.Success)
                {
                    var budgerHeader = await _roadWorkBudgetHeaderService.FindByAuthorityIdForCurrentYear(fYearResp.FinancialYear.ID, authority.ID).ConfigureAwait(false);

                    if (budgerHeader.Success)
                    {
                        //existing budget
                        roadWorkBudgetLine.RoadWorkBudgetHeader = budgerHeader.RoadWorkBudgetHeader;
                    }
                    else
                    {
                        //is a new budget
                        RoadWorkBudgetHeader newBudgetHeader = new RoadWorkBudgetHeader
                        {
                            Code = fYearResp.FinancialYear.Code,
                            Summary = "Budget Plan for " + authority.Name + " for Fiscal Year : " + fYearResp.FinancialYear.Code,
                            CreatedBy = user.UserName,
                            CreationDate = DateTime.UtcNow,
                            FinancialYear = fYearResp.FinancialYear,
                            Authority = authority,
                            ApprovalStatus = 0
                        };
                        var saveResp = await _roadWorkBudgetHeaderService.AddAsync(newBudgetHeader).ConfigureAwait(false);
                        if (saveResp.Success)
                            roadWorkBudgetLine.RoadWorkBudgetHeader = saveResp.RoadWorkBudgetHeader;
                    }
                }

                if (roadWorkBudgetLine.ID > 0)
                {
                    //editing the record
                    roadWorkBudgetLine.UpdatedBy = user.UserName;
                    var response = await _roadWorkBudgetLinesService.UpdateAsync(roadWorkBudgetLine).ConfigureAwait(false);
                    if (response.Success)
                    {
                        return Json(Url.Action("GetRoadWorksBudgeting", "Workplan"));
                    }
                    else
                    {
                        return PartialView("BudgetLineIdPartialView", roadWorkBudgetLine);
                    }
                }

                else
                {
                    //new record
                    roadWorkBudgetLine.CreatedBy = user.UserName;
                    var response = await _roadWorkBudgetLinesService.AddAsync(roadWorkBudgetLine).ConfigureAwait(false);
                    if (response.Success)
                    {
                        return Json(Url.Action("GetRoadWorksBudgeting", "Workplan"));
                    }
                    else
                    {
                        return PartialView("BudgetLineIdPartialView", roadWorkBudgetLine);
                    }
                }
            }
            else
            {
                return PartialView("BudgetLineIdPartialView", roadWorkBudgetLine);
            }

        }
        /// <summary>
        /// Deletes the associated budget line item configure
        /// </summary>
        /// <param name="budgetLineId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteBudgetLine(long budgetLineId)
        {
            if (budgetLineId > 0)
            {
                var existingBudgetLine = await _roadWorkBudgetLinesService.RemoveAsync(budgetLineId).ConfigureAwait(false);
                if (existingBudgetLine.Success)
                {
                    return Json(new
                    {
                        Success = true,
                        Message = "Success",
                        Href = Url.Action("GetRoadWorksBudgeting", "Workplan")
                    });
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = existingBudgetLine.Message
                    });
                }
            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "Invalid BudgetLine"
                });
            }
        }

        // Section handling the road operation activities planning

        /// <summary>
        /// Returns the page for handling operational activities
        /// </summary>
        /// <returns></returns>        
        public async Task<IActionResult> GetOperationalActivities()
        {
            //get the agency budget ceiling set

            var viewModel = new RoadBudgetOperatopmActivityViewModel();
            var user = await GetLoggedInUser().ConfigureAwait(false);
            var authority = new Authority();
            if (user != null)
            {
                var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                ViewBag.Authority = userAuthority;
                authority = userAuthority.Authority;
            }
            ViewBag.Authority = user.Authority;


            //get current financial year
            var fYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);



            //check if budget has been defined for the authority in the current year
            if (fYearResp.Success)
            {
                var budgerHeader = await _roadWorkBudgetHeaderService.FindByAuthorityIdForCurrentYear(fYearResp.FinancialYear.ID, authority.ID).ConfigureAwait(false);

                if (budgerHeader.Success)
                {
                    viewModel.RoadWorkBudgetHeader = budgerHeader.RoadWorkBudgetHeader;
                    viewModel.RoadWorkOperationalActivitiesBudgets = budgerHeader.RoadWorkBudgetHeader.RoadWorkOperationalActivitiesBudgets;
                }
                else
                {
                    viewModel.RoadWorkBudgetHeader = new RoadWorkBudgetHeader
                    {
                        Code = fYearResp.FinancialYear.Code,
                        Summary = "Budget Plan for " + authority.Name + " for Fiscal Year : " + fYearResp.FinancialYear.Code
                    };
                    viewModel.RoadWorkOperationalActivitiesBudgets = Enumerable.Empty<RoadWorkOperationalActivitiesBudget>();
                }

                //get the agency budget ceiling set
                var ceilingResp = await _budgetCeilingService.FindApprovedByAuthorityForCurrentYear(fYearResp.FinancialYear.ID, authority.ID).ConfigureAwait(false);
                if (ceilingResp.Success)
                {
                    viewModel.BudgetCeiling = ceilingResp.BudgetCeiling;
                }
            }
            return View("GetRoadWorksOperationalActivities", viewModel);
        }

        /// <summary>
        /// returns partial view for budget line operational activities  overhead editing or creation
        /// </summary>
        /// <param name="BudgetLineId"></param>
        /// <returns></returns>
        public async Task<IActionResult> ShowBudgetOperationalActivityLine(long BudgetOperationalActivityLineId)
        {
            RoadWorkOperationalActivitiesBudget model = new RoadWorkOperationalActivitiesBudget();
            var user = await GetLoggedInUser().ConfigureAwait(false);
            var authority = new Authority();
            if (user != null)
            {
                var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                authority = userAuthority.Authority;
            }

            //retrieving funding source for use in create view
            //var fundingSources = await _fundingSourceService.ListAsync().ConfigureAwait(false);
            var fundingSources = await _fundingSourceService.ListAsync().ConfigureAwait(false);
            var fundTypes = await _fundTypeService.ListAsync().ConfigureAwait(false);

            ViewBag.FundingSourceList = new SelectList(fundingSources, "ID", "Name");
            ViewBag.FundTypeList = new SelectList(fundTypes, "ID", "Name");

            if (BudgetOperationalActivityLineId > 0)
            {
                //get budget line details
                var budgetLineResponse = await _roadWorkOperationalActivitiesBudgetService.FindByIdAsync(BudgetOperationalActivityLineId).ConfigureAwait(false);
                if (budgetLineResponse.Success)
                {
                    model = budgetLineResponse.RoadWorkOperationalActivitiesBudget;
                }
            }

            return PartialView("BudgetLineOperationalActivityPartialView", model);
        }

        /// <summary>
        /// Handles submitted form data for budgetline creation or modification
        /// </summary>
        /// <param name="roadWorkOperationalActivitiesBudget"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ShowBudgetOperationalActivityLine(RoadWorkOperationalActivitiesBudget roadWorkOperationalActivitiesBudget)
        {
            if (roadWorkOperationalActivitiesBudget != null)
            {
                var user = await GetLoggedInUser().ConfigureAwait(false);
                var authority = new Authority();
                if (user != null)
                {
                    var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                    ViewBag.Authority = userAuthority;
                    authority = userAuthority.Authority;

                }

                //get current financial year
                var fYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);

                //check if budget has been defined for the authority in the current year
                if (fYearResp.Success)
                {
                    var budgerHeader = await _roadWorkBudgetHeaderService.FindByAuthorityIdForCurrentYear(fYearResp.FinancialYear.ID, authority.ID).ConfigureAwait(false);

                    if (budgerHeader.Success)
                    {
                        //existing budget
                        roadWorkOperationalActivitiesBudget.RoadWorkBudgetHeader = budgerHeader.RoadWorkBudgetHeader;
                    }
                    else
                    {
                        //is a new budget
                        RoadWorkBudgetHeader newBudgetHeader = new RoadWorkBudgetHeader
                        {
                            Code = fYearResp.FinancialYear.Code,
                            Summary = "Budget Plan for " + authority.Name + " for Fiscal Year : " + fYearResp.FinancialYear.Code,
                            CreatedBy = user.UserName,
                            CreationDate = DateTime.UtcNow,
                            FinancialYear = fYearResp.FinancialYear,
                            Authority = authority,
                            ApprovalStatus = 0
                        };
                        var saveResp = await _roadWorkBudgetHeaderService.AddAsync(newBudgetHeader).ConfigureAwait(false);
                        if (saveResp.Success)
                            roadWorkOperationalActivitiesBudget.RoadWorkBudgetHeader = saveResp.RoadWorkBudgetHeader;
                    }
                }


                if (roadWorkOperationalActivitiesBudget.ID > 0)
                {
                    //editing the record
                    roadWorkOperationalActivitiesBudget.UpdatedBy = user.UserName;
                    var response = await _roadWorkOperationalActivitiesBudgetService.UpdateAsync(roadWorkOperationalActivitiesBudget).ConfigureAwait(false);
                    if (response.Success)
                    {

                        return Json(Url.Action("GetOperationalActivities", "Workplan"));
                    }
                    else
                    {
                        return PartialView("BudgetLineOperationalActivityPartialView", roadWorkOperationalActivitiesBudget);
                    }
                }
                else
                {
                    //new record
                    roadWorkOperationalActivitiesBudget.CreatedBy = user.UserName;
                    var response = await _roadWorkOperationalActivitiesBudgetService.AddAsync(roadWorkOperationalActivitiesBudget).ConfigureAwait(false);
                    if (response.Success)
                    {
                        return Json(Url.Action("GetOperationalActivities", "Workplan"));
                    }
                    else
                    {
                        return PartialView("BudgetLineOperationalActivityPartialView", roadWorkOperationalActivitiesBudget);
                    }
                }
            }
            else
            {
                return PartialView("BudgetLineOperationalActivityPartialView", roadWorkOperationalActivitiesBudget);
            }
        }

        /// <summary>
        /// Deletes the associated budget line item configure
        /// </summary>
        /// <param name="budgetOperationalActivityLineId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteShowBudgetOperationalActivityLine(long budgetOperationalActivityLineId)
        {
            if (budgetOperationalActivityLineId > 0)
            {
                var existingBudgetLine = await _roadWorkOperationalActivitiesBudgetService.RemoveAsync(budgetOperationalActivityLineId).ConfigureAwait(false);
                if (existingBudgetLine.Success)
                {
                    return Json(new
                    {
                        Success = true,
                        Message = "Success",
                        Href = Url.Action("GetOperationalActivities", "Workplan")
                    });
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = existingBudgetLine.Message
                    });
                }
            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "Invalid BudgetLine"
                });
            }
        }

        // Section handling the road summary planning

        /// <summary>
        /// Returns the page for handling operational activities
        /// </summary>
        /// <returns></returns>        
        public async Task<IActionResult> GetRoadWorksSummary()
        {
            if (TempData["failedToSubmitForApproval"] != null)
                ViewBag.failedToSubmitForApproval = TempData["failedToSubmitForApproval"].ToString();

            var viewModel = new RoadWorkPlanSummaryViewModel();
            // get the agency budget ceiling set

            var user = await GetLoggedInUser().ConfigureAwait(false);
            var authority = new Authority();
            if (user != null)
            {
                var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                ViewBag.Authority = userAuthority;
                authority = userAuthority.Authority;
            }
            ViewBag.Authority = user.Authority;
            //get current financial year
            var fYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);

            //check if budget has been defined for the authority in the current year
            if (fYearResp.Success)
            {
                var budgerHeader = await _roadWorkBudgetHeaderService.FindByAuthorityIdForCurrentYear(fYearResp.FinancialYear.ID, authority.ID).ConfigureAwait(false);

                if (budgerHeader.Success)
                {
                    viewModel.RoadWorkBudgetHeader = budgerHeader.RoadWorkBudgetHeader;
                    var allBudgetLines = budgerHeader.RoadWorkBudgetHeader.RoadWorkBudgetLines;
                    var allActivities = budgerHeader.RoadWorkBudgetHeader.RoadWorkOperationalActivitiesBudgets;

                    List<RoadWorkPlanViewModel> summaryPlans = new List<RoadWorkPlanViewModel>();

                    RoadWorkPlanViewModel model;
                    //loop through budget Line
                    foreach (var budgetLine in allBudgetLines)
                    {
                        model = new RoadWorkPlanViewModel();
                        model.FundingSource = budgetLine.FundingSource.Name;
                        model.FundType = budgetLine.FundType.Name;
                        model.TotalRoad = 0.0;
                        model.RoadWorks = (budgetLine.RoutineMaintanance + budgetLine.PeriodicMentanance + budgetLine.SpotImprovement + budgetLine.StructureConstruction + budgetLine.RehabilitationWork + budgetLine.NewStructure);

                        summaryPlans.Add(model);
                    }

                    foreach (var budgetActivity in allActivities)
                    {
                        model = new RoadWorkPlanViewModel();
                        model.FundingSource = budgetActivity.FundingSource.Name;
                        model.FundType = budgetActivity.FundType.Name;
                        model.TotalRoad = 0.0;
                        model.RoadWorks = 0.0;
                        model.OverHeads = budgetActivity.OverHeadBudget;

                        summaryPlans.Add(model);
                    }


                    viewModel.RoadWorkPlanViewModels = summaryPlans;
                }
                else
                {
                    viewModel.RoadWorkBudgetHeader = new RoadWorkBudgetHeader
                    {
                        Code = fYearResp.FinancialYear.Code,
                        Summary = "Budget Plan for " + authority.Name + " for Fiscal Year : " + fYearResp.FinancialYear.Code
                    };
                    viewModel.RoadWorkPlanViewModels = new List<RoadWorkPlanViewModel>();
                }

                //get the agency budget ceiling set
                var ceilingResp = await _budgetCeilingService.FindApprovedByAuthorityForCurrentYear(fYearResp.FinancialYear.ID, authority.ID).ConfigureAwait(false);
                if (ceilingResp.Success)
                {
                    viewModel.BudgetCeiling = ceilingResp.BudgetCeiling;
                }
            }


            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitBudgetPlan()
        {
            var user = await GetLoggedInUser().ConfigureAwait(false);
            var authority = new Authority();
            if (user != null)
            {
                var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                ViewBag.Authority = userAuthority;
                authority = userAuthority.Authority;
            }

            //get current financial year
            var fYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);

            //check if budget has been defined for the authority in the current year
            if (fYearResp.Success)
            {
                var budgerHeader = await _roadWorkBudgetHeaderService.FindByAuthorityIdForCurrentYear(fYearResp.FinancialYear.ID, authority.ID).ConfigureAwait(false);
                if (budgerHeader.Success)
                {
                    //update the budget as submitted
                    var existingRecord = budgerHeader.RoadWorkBudgetHeader;
                    existingRecord.SubmittedBy = user.UserName;
                    existingRecord.SubmissionDate = DateTime.UtcNow;
                    existingRecord.ApprovalStatus = 1;
                    var updateResp = await _roadWorkBudgetHeaderService.Update(existingRecord).ConfigureAwait(false);
                    if (!updateResp.Success)
                    {
                        TempData["failedToSubmitForApproval"] = "Could not submit for approval, Please contact system administrator";
                        return RedirectToAction("GetRoadWorksSummary");
                    }

                }
                else
                {
                    //invalid update being executed
                    TempData["failedToSubmitForApproval"] = "Could not submit for approval, Please contact system administrator";
                    return RedirectToAction("GetRoadWorksSummary");
                }

            }

            return Json(new
            {
                Success = true,
                Message = "Success",
                Href = Url.Action("GetRoadWorksSummary", "Workplan")
            });


        }//
        [HttpPost]
        public async Task<IActionResult> ApproveBudgetPlan()
        {
            var user = await GetLoggedInUser().ConfigureAwait(false);
            var authority = new Authority();
            if (user != null)
            {
                var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                ViewBag.Authority = userAuthority;
                authority = userAuthority.Authority;
            }

            //get current financial year
            var fYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);

            //check if budget has been defined for the authority in the current year
            if (fYearResp.Success)
            {
                var budgerHeader = await _roadWorkBudgetHeaderService.FindByAuthorityIdForCurrentYear(fYearResp.FinancialYear.ID, authority.ID).ConfigureAwait(false);
                if (budgerHeader.Success)
                {
                    //update the budget as submitted
                    var existingRecord = budgerHeader.RoadWorkBudgetHeader;
                    existingRecord.SubmittedBy = user.UserName;
                    existingRecord.ApprovalDate = DateTime.UtcNow;
                    existingRecord.ApprovalStatus = 2;
                    var updateResp = await _roadWorkBudgetHeaderService.Update(existingRecord).ConfigureAwait(false);
                    if (!updateResp.Success)
                    {
                        TempData["failedToSubmitForApproval"] = "Could not submit for approval, Please contact system administrator";
                        return RedirectToAction("GetRoadWorksSummary");
                    }

                }
                else
                {
                    //invalid update being executed
                    TempData["failedToSubmitForApproval"] = "Could not submit for approval, Please contact system administrator";
                    return RedirectToAction("GetRoadWorksSummary");
                }

            }

            return Json(new
            {
                Success = true,
                Message = "Success",
                Href = Url.Action("GetRoadWorksSummary", "Workplan")
            });


        }

        [HttpPost]
        public async Task<IActionResult> UploadWorkplanRefresh(long workplanUploadId)
        {
            var workplanUploadRecordResp = await _workplanUploadService.RemoveAsync(workplanUploadId).ConfigureAwait(false);
            if (workplanUploadRecordResp.Success)
            {
                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Href = Url.Action("UploadWorkplans", "Workplan")
                });
            }
            else
            {
                return Json(new { Success = false, Message = "We could not retrieve the workplan upload records, please contact the administrator" });
            }

        }

        public async Task<IActionResult> AdmininstrationActivities(long agecyId, long adminActivityId)
        {

            var viewModel = new WorkplanUploadVewModel();
            var user = await GetLoggedInUser().ConfigureAwait(false);

            var fyearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
            viewModel.FinancialYear = fyearResp.FinancialYear;

            viewModel.AdminOperationalActivities = (List<AdminOperationalActivity>)await _adminOperationalActivityService.ListByAuthorityAsync(user.Authority.ID, fyearResp.FinancialYear.ID).ConfigureAwait(false);
            viewModel.AdminOperationalActivity = new AdminOperationalActivity();
            if (adminActivityId > 0)
            {
                var existingRecord = await _adminOperationalActivityService.FindByIdAsync(adminActivityId).ConfigureAwait(false);
                if (existingRecord.Success)
                {
                    viewModel.AdminOperationalActivity = existingRecord.AdminOperationalActivity;
                }
            }
            ViewBag.BudgetAllocated = 0;
            ViewBag.AdminCosts = 0;
            ViewBag.RemainingCosts = 0;

            ViewBag.Authority = user.Authority;

            return View("GetRoadWorkPlanUpload", viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> AddAdmininstrationActivities(WorkplanUploadVewModel model)
        {
            var user = await GetLoggedInUser().ConfigureAwait(false);
            var fyearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);

            //check if edit or add
            if (model.AdminOperationalActivity.ID > 0)
            {
                //means existing record, so is an edit


                var updateResp = await _adminOperationalActivityService.Update(model.AdminOperationalActivity).ConfigureAwait(false);
                if (!updateResp.Success)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "An error occured in adding the record, please contact the administrator."
                    });
                }
            }
            else
            {
                //meands a new record, so and add
                AdminOperationalActivity newRecord = new AdminOperationalActivity
                {
                    Authority = user.Authority,
                    FinancialYear = fyearResp.FinancialYear,
                    ActivityGroup = model.AdminOperationalActivity.ActivityGroup,
                    ActivityWorks = model.AdminOperationalActivity.ActivityWorks,
                    Amount = model.AdminOperationalActivity.Amount,
                    ST = model.AdminOperationalActivity.ST,
                    KM = model.AdminOperationalActivity.KM,
                    RoadID = model.AdminOperationalActivity.RoadID,
                    RoadSection = model.AdminOperationalActivity.RoadSection,
                    CreatedBy = user.UserName,
                    CreationDate = DateTime.UtcNow
                };

                var addResp = await _adminOperationalActivityService.AddAsync(newRecord).ConfigureAwait(false);
                if (!addResp.Success)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "An error occured in adding the record, please contact the administrator."
                    });
                }
            }
            return Json(new
            {
                Success = true,
                Message = "Success",
                Href = Url.Action("AdmininstrationActivities", "Workplan", new { agencyId = user.Authority.ID })
            });

        }
        [HttpPost]
        public async Task<IActionResult> RemoveAdmininstrationActivities(long activityId)
        {
            var user = await GetLoggedInUser().ConfigureAwait(false);
            if (activityId > 0)
            {
                var removeResp = await _adminOperationalActivityService.Remove(activityId).ConfigureAwait(false);
                if (!removeResp.Success)
                {
                    return Json(new { Success = false, Message = "The record could not be removed. Please contact the adminstrator." });
                }
            }
            else
            {
                return Json(new { Success = false, Message = "Record to be removed could not be found." });
            }
            return Json(new { Success = true, Message = "Record successfully removed.", Href = Url.Action("AdmininstrationActivities", "Workplan", new { agencyId = user.Authority.ID }) });
        }


        public async Task<IActionResult> UploadWorkplans(long ID)
        {

            var viewModel = new WorkplanUploadVewModel();

            var user = await GetLoggedInUser().ConfigureAwait(false);
            if (user != null)
            {
                var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                viewModel.Authority = userAuthority.Authority;
            }
            //get the financial year
            var fyearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
            viewModel.FinancialYear = fyearResp.FinancialYear;
            //get all uploads in the current financial year
            viewModel.WorkplanUploads = (List<WorkplanUpload>)await _workplanUploadService.FindByFinancialYearAsync(fyearResp.FinancialYear.ID, viewModel.Authority.ID).ConfigureAwait(false);

            List<MissingSectionViewModel> missingRoadSections = new List<MissingSectionViewModel>();
            List<MissingWorkCategoriesViewModel> missingCategories = new List<MissingWorkCategoriesViewModel>();
            List<MissingItemViewModel> missingActivities = new List<MissingItemViewModel>(); // this holds all item codes not found in the database
            List<string> missingTechnologies = new List<string>();
            if (ID > 0)
            {
                //var uploadWorkplanResp = await _workplanUploadService.FindByFinancialYearAsync(fyearResp.FinancialYear.ID, viewModel.Authority.ID).ConfigureAwait(false);
                var uploadWorkplanResp = await _workplanUploadService.FindByIdAsync(ID).ConfigureAwait(false);
                if (uploadWorkplanResp.Success)
                {
                    viewModel.WorkplanUpload = uploadWorkplanResp.WorkplanUpload;
                    // Check validation details here
                    //pick all the uploads for the current year and for each, create a workplan


                    var dbActivities = (List<ItemActivityUnitCost>)await _itemActivityUnitCostService.ListAsync().ConfigureAwait(false);
                    var dbCategories = (List<WorkCategory>)await _workCategoryService.ListAsync().ConfigureAwait(false);
                    var technologyList = await _technologyService.ListAsync().ConfigureAwait(false);

                    RoadWorkSectionPlan newSection = new RoadWorkSectionPlan();
                    newSection.PlanActivities = new List<PlanActivity>();
                    PlanActivity newPlanAcvitity;
                    MissingItemViewModel missingItemViewModel = new MissingItemViewModel();
                    MissingSectionViewModel missingSectionViewModel = new MissingSectionViewModel();

                    List<RoadWorkSectionPlan> newSectionPlans = new List<RoadWorkSectionPlan>();



                    foreach (var section in uploadWorkplanResp.WorkplanUpload.WorkplanUploadSections)
                    {
                        //get the section from DB.
                        var dbSectionResp = await _roadSectionService.FindBySectionIdAsync(section.SectionId).ConfigureAwait(false);
                        if (dbSectionResp.Success)
                        {
                            //check if user is allowed to plan for the road
                            if (dbSectionResp.RoadSection.Road.AuthorityId == user.AuthorityId)
                            {
                                newSection.RoadSection = dbSectionResp.RoadSection;
                                newSection.Authority = user.Authority;
                                newSection.FinancialYear = fyearResp.FinancialYear;

                                //work category check
                                var categoryFound = dbCategories.Where(c => c.Code.ToLower().Equals(section.WorkCategory.ToLower())).FirstOrDefault();
                                if (categoryFound != null) //category is in the database
                                {
                                    newSection.WorkCategory = categoryFound;
                                }
                                else
                                {
                                    //is a missing category
                                    missingCategories.Add(new MissingWorkCategoriesViewModel { WorkCategoryCode = section.WorkCategory });
                                }
                            }
                            else
                            {
                                //remove duplicates
                                missingSectionViewModel = new MissingSectionViewModel { RoadId = section.RoadId, RoadSectionId = section.SectionId, RoadSectionName = section.SectionName };
                                if (!missingRoadSections.Any(s => s.RoadSectionId == missingSectionViewModel.RoadSectionId))
                                    missingRoadSections.Add(missingSectionViewModel);
                            }

                        }
                        else
                        {
                            //remove duplicates
                            missingSectionViewModel = new MissingSectionViewModel { RoadId = section.RoadId, RoadSectionId = section.SectionId, RoadSectionName = section.SectionName };
                            if (!missingRoadSections.Any(s => s.RoadSectionId == missingSectionViewModel.RoadSectionId))
                                missingRoadSections.Add(missingSectionViewModel);
                        }

                        //get related activities
                        foreach (var activity in section.WorkplanUploadSectionActivities)
                        {

                            //check if activity exists in db activities
                            var activityFound = dbActivities.Where(m => (m.ItemCode + '-' + m.SubItemCode + '-' + m.SubSubItemCode).ToLower().Equals(activity.ActivityCode.ToLower())).FirstOrDefault();
                            if (activityFound != null)
                            {
                                newPlanAcvitity = new PlanActivity();
                                newPlanAcvitity.ItemActivityUnitCost = activityFound;

                                newSection.PlanActivities.Add(newPlanAcvitity);
                            }
                            else
                            {
                                missingItemViewModel = new MissingItemViewModel
                                {
                                    Code = activity.ActivityCode,
                                    ItemName = activity.AcivityDescription
                                };
                                //remove duplicates
                                if (!missingActivities.Any(m => m.Code == missingItemViewModel.Code))
                                {
                                    missingActivities.Add(missingItemViewModel);
                                }
                            }

                            var tech = technologyList.FirstOrDefault(t => t.Code.ToLower().Equals(activity.Technology.ToLower()));
                            if (tech == null)
                            {
                                //missing technology
                                if (!missingTechnologies.Any(m => m.ToLower().Equals(activity.Technology.ToLower())))
                                {
                                    missingTechnologies.Add(activity.Technology);
                                }
                            }
                        }


                        //get the activities

                        newSectionPlans.Add(newSection);
                    }


                }
                else
                {
                    viewModel.WorkplanUpload = new WorkplanUpload { WorkplanUploadSections = new List<WorkplanUploadSection>() };
                }
            }
            else
            {
                viewModel.WorkplanUpload = new WorkplanUpload { WorkplanUploadSections = new List<WorkplanUploadSection>() };
            }


            viewModel.MissingActivities = missingActivities;
            viewModel.MissingRoadSections = missingRoadSections;
            viewModel.MissingCategories = missingCategories;
            viewModel.MissingTechnologies = missingTechnologies;

            //get summaries
            var totalBudgetAllocated = 0D;
            var workplansForAgencies = await _roadWorkSectionPlanService.ListByAgencyAsync(viewModel.Authority.ID, viewModel.FinancialYear.ID).ConfigureAwait(false);
            var totalWorkplanCost = workplansForAgencies.Sum(a => a.TotalEstimateCost);

            var ceilingResp = await _budgetCeilingService.FindApprovedByAuthorityForCurrentYear(viewModel.FinancialYear.ID, viewModel.Authority.ID).ConfigureAwait(false);
            if (ceilingResp.Success)
            {
                totalBudgetAllocated += ceilingResp.BudgetCeiling.Amount;
            }

            //get the agency budget allocation for other funding source

            var budgetTotalOthers = await _revenueCollectionCodeUnitService.ListAsync(viewModel.FinancialYear.ID, "Others").ConfigureAwait(false);
            if (budgetTotalOthers != null)
            {
                totalBudgetAllocated += budgetTotalOthers.RevenueCollectionCodeUnit.Where(a => a.AuthorityId == viewModel.Authority.ID).Sum(a => a.RevenueCollection.Amount);
            }
            var yearList = await _financialYearService.ListAsync().ConfigureAwait(false);
            var fundingSources = await _fundingSourceService.ListAsync().ConfigureAwait(false);
            var fundingSourcesSubCodesForRMLF = fundingSources.Where(f => string.Equals(f.Code, "03")).Select(s => s.FundingSourceSubCodes);
            var fundingSourceSelectList = new List<FundingSourceSubCode>();
            foreach (var fundingSourceCode in fundingSourcesSubCodesForRMLF)
            {
                foreach (var sourceCode in fundingSourceCode)
                {
                    if (!fundingSourceSelectList.Contains(sourceCode))
                        fundingSourceSelectList.Add(sourceCode);
                }
            }

            ViewBag.FinancialYearList = new SelectList(yearList.Where(f => (int)f.IsCurrent == 1), "ID", "Code");
            ViewBag.FundingSourceList = new SelectList(fundingSourceSelectList.Where(s => s.Name.ToUpper().Contains(user.Authority.Code.ToUpper())), "ID", "Name");

            ViewBag.BudgetAllocated = totalBudgetAllocated;
            ViewBag.totalWorkplanCost = totalWorkplanCost;
            ViewBag.Authority = user.Authority;

            if (TempData["fileNotSelected"] != null)
                ViewBag.fileNotSelected = TempData["fileNotSelected"].ToString();

            if (TempData["invalidFile"] != null)
                ViewBag.invalidFile = TempData["invalidFile"].ToString();

            if (TempData["financialYearNotSetInSystem"] != null)
                ViewBag.financialYearNotSetInSystem = TempData["financialYearNotSetInSystem"].ToString();

            if (TempData["failedToSubmitForApproval"] != null)
                ViewBag.failedToSubmitForApproval = TempData["failedToSubmitForApproval"].ToString();

            if (TempData["failedToApproveBudget"] != null)
                ViewBag.failedToSubmitForApproval = TempData["failedToApproveBudget"].ToString();

            return View("GetRoadWorkPlanUploads", viewModel);
        }



        [HttpPost]
        public async Task<IActionResult> UploadWorkplanFile([FromForm] WorkplanUploadVewModel model)
        {
            //chec if file has been selected
            if (model == null || model.SupportDocument == null || model.SupportDocument.Length == 0)
            {
                TempData["fileNotSelected"] = "File has not been selected, please select a valid file";
                return RedirectToAction("UploadWorkplans");
            }

            //check if financial year is set
            var newId = 0d;
            var financialYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
            if (financialYearResp.Success && financialYearResp.FinancialYear != null)
            {
                var path = Path.Combine(
                _hostingEnvironment.WebRootPath, "uploads", "budgets", model.SupportDocument.FileName);

                //var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\budgets", model.FileToUpload.FileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.SupportDocument.CopyToAsync(stream).ConfigureAwait(false);
                }
                var workplanValidations = ValidateFile(path, financialYearResp.FinancialYear);

                if (workplanValidations.FileIsValid)
                {
                    var user = await GetLoggedInUser().ConfigureAwait(false);

                    var authority = new Authority();
                    if (user != null)
                    {
                        var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                        authority = userAuthority.Authority;

                    }

                    // add the records in the database
                    WorkplanUpload uploadWorkplan = new WorkplanUpload();
                    uploadWorkplan.Authority = authority;
                    uploadWorkplan.FinancialYear = financialYearResp.FinancialYear;
                    uploadWorkplan.CreatedBy = user.UserName;
                    uploadWorkplan.CreationDate = DateTime.UtcNow;
                    uploadWorkplan.WorkplanUploadSections = workplanValidations.WorkplanUploadSections;
                    uploadWorkplan.UploadBudget = workplanValidations.programmeUploadBudget;


                    var addUploadRecordResp = await _workplanUploadService.AddAsync(uploadWorkplan).ConfigureAwait(false);
                    newId = addUploadRecordResp.WorkplanUpload.ID;
                }
                else
                {
                    TempData["invalidFile"] = "The file uploaded is invalid, validate the values correctly {code, amount, financial year}";
                    return RedirectToAction("UploadWorkplans");
                }
            }
            else
            {
                TempData["financialYearNotSetInSystem"] = "No current financial year has been set in the system, Please contact system administrator";
                return RedirectToAction("UploadWorkplans");
            }
            //return RedirectToAction("Upload");
            return await Task.Run<ActionResult>(() =>
            {
                return RedirectToAction("UploadWorkplans", "Workplan", new { ID = newId });

            }).ConfigureAwait(false);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitBudget()
        {

            //retrieve the currentyear budgetHeader
            var resp = await _budgetCeilingHeaderService.FindCurrentAsync().ConfigureAwait(false);
            if (resp.Success)
            {
                var user = await GetLoggedInUser().ConfigureAwait(false);
                //mark the budget as submitted
                var submittedBudget = resp.BudgetCeilingHeader;
                submittedBudget.SubmissionDate = DateTime.UtcNow;
                submittedBudget.SubmittedBy = user.UserName;
                submittedBudget.ApprovalStatus = 1; //has been submitted

                var updateResp = await _budgetCeilingHeaderService.Update(submittedBudget).ConfigureAwait(false);
                if (!updateResp.Success)
                {
                    TempData["failedToSubmitForApproval"] = "Could not submit for approval, Please contact system administrator";
                    return RedirectToAction("UploadWorkplans");
                }
            }
            //return RedirectToAction("Upload");
            return Json(new
            {
                Success = true,
                Message = "Success",
                Href = Url.Action("UploadWorkplans", "Workplan")
            });
        }

        [HttpPost]
        public async Task<IActionResult> ApproveBudget()
        {

            //retrieve the currentyear budgetHeader
            var resp = await _budgetCeilingHeaderService.FindCurrentAsync().ConfigureAwait(false);
            if (resp.Success)
            {
                //mark the budget as submitted
                var submittedBudget = resp.BudgetCeilingHeader;
                submittedBudget.ApprovalStatus = 2;//has been submitted
                submittedBudget.ApprovedBy = "kimosodavid";
                submittedBudget.ApprovalDate = DateTime.UtcNow;

                var updateResp = await _budgetCeilingHeaderService.Update(submittedBudget).ConfigureAwait(false);
                if (!updateResp.Success)
                {
                    TempData["failedToApproveBudget"] = "Could not approve budget ceilings, Please contact system administrator";
                    return RedirectToAction("UploadWorkplans");
                }

            }
            return Json(new
            {
                Success = true,
                Message = "Success",
                Href = Url.Action("UploadWorkplans", "Workplan")
            });
        }

        public async Task<IActionResult> GetBudgetCeilingAddEdit(long budgetId)
        {
            BudgetCeiling viewModel = new BudgetCeiling();
            if (budgetId > 0)
            {
                //edit
                var budgetModel = await _budgetCeilingService.FindByIdAsync(budgetId).ConfigureAwait(false);
                if (budgetModel.Success)
                {
                    viewModel = budgetModel.BudgetCeiling;
                }
            }


            var authorities = await _authorityService.ListAsync().ConfigureAwait(false);

            ViewBag.AuthorityList = new SelectList(authorities, "ID", "NAME");

            return PartialView("BudgetAddEditPartialView", viewModel);
        }

        private async Task ReadXLS(string FilePath, FinancialYear financialYear, string createdBy)
        {
            FileInfo existingFile = new FileInfo(FilePath);
            using (ExcelPackage package = new ExcelPackage(existingFile))
            {
                //get the first worksheet in the workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                var fiscalYear = worksheet.Cells[2, 2].Value.ToString().Trim();
                BudgetCeilingHeader header = new BudgetCeilingHeader();
                header.CreatedBy = createdBy;
                header.CreationDate = DateTime.UtcNow;
                header.FinancialYear = financialYear;
                header.isCurrent = 0;

                var headerResp = await _budgetCeilingHeaderService.AddAsync(header).ConfigureAwait(false);
                if (headerResp.Success)
                {

                    BudgetCeiling budgetCeiling;

                    int colCount = worksheet.Dimension.End.Column;  //get Column Count
                    int rowCount = worksheet.Dimension.End.Row;     //get row count


                    //get the lines
                    for (int row = 4; row <= rowCount; row++)
                    {
                        budgetCeiling = new BudgetCeiling();

                        budgetCeiling.AdditionalInfo = worksheet.Cells[row, 4].Value.ToString().Trim();
                        budgetCeiling.Amount = Convert.ToDouble(worksheet.Cells[row, 3].Value.ToString().Trim(), CultureInfo.InvariantCulture);
                        var authority = await _authorityService.FindByCodeAsync(worksheet.Cells[row, 1].Value.ToString().Trim()).ConfigureAwait(false);
                        budgetCeiling.Authority = authority.Authority;
                        budgetCeiling.RoadWorkBudgetHeader = header;
                        //insert record into the database
                        await _budgetCeilingService.AddAsync(budgetCeiling).ConfigureAwait(false);


                    }
                }
            }
        }

        private UploadWorkplanValidation ValidateFile(string FilePath, FinancialYear financialYear)
        {
            try
            {
                var returnValid = new UploadWorkplanValidation();
                var workplanUploadSections = new List<WorkplanUploadSection>();
                WorkplanUploadSection roadSection = new WorkplanUploadSection();
                WorkplanUploadSectionActivity uploadSectionActivity = new WorkplanUploadSectionActivity();

                FileInfo existingFile = new FileInfo(FilePath);
                using (ExcelPackage package = new ExcelPackage(existingFile))
                {
                    //get the first worksheet in the workbook
                    int iSheetsCount = package.Workbook.Worksheets.Count;

                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Upload_Roadworks_Activities"]; //sheet with roadworks activities
                    ExcelWorksheet worksheetAdmin = package.Workbook.Worksheets["Upload_Admin_Activities"]; //sheet with roadworks activities

                    //BEGIN VALIDATION FOR ROADWORKS ACTIVITIES

                    int colCount = (worksheet.Dimension.End != null ? worksheet.Dimension.End.Column : 0);  //get Column Count
                    int rowCount = (worksheet.Dimension.End != null ? worksheet.Dimension.End.Row : 0);     //get row count

                    if (colCount < 3)
                    {
                        returnValid.FileIsValid = false;
                        returnValid.WorkplanUploadSections = new List<WorkplanUploadSection>();

                        return returnValid;
                    }


                    if (rowCount < 4)
                    {
                        returnValid.FileIsValid = false;
                        returnValid.WorkplanUploadSections = new List<WorkplanUploadSection>();

                        return returnValid;
                    }

                    //fetch the header details
                    var programmeYear = worksheet.Cells[1, 3].Value != null ? worksheet.Cells[1, 3].Value.ToString().Trim() : "";
                    var programmeCGAgencyName = worksheet.Cells[2, 3].Value != null ? worksheet.Cells[2, 3].Value.ToString().Trim() : "";
                    var programmeCGAgencyCode = worksheet.Cells[3, 3].Value != null ? worksheet.Cells[3, 3].Value.ToString().Trim() : "";
                    var programmeUploadBudget = worksheet.Cells[4, 3].Value != null ? worksheet.Cells[4, 3].Value.ToString().Trim() : "";

                    returnValid.programmeYear = programmeYear;
                    returnValid.programmeCGAgencyCode = programmeCGAgencyCode;
                    returnValid.programmeCGAgencyName = programmeCGAgencyName;
                    double budgetUploaded;
                    double.TryParse(programmeUploadBudget, out budgetUploaded);
                    returnValid.programmeUploadBudget = budgetUploaded;




                    var sectionName = "";


                    for (int row = 7; row <= rowCount; row++) //pick the sections
                    {

                        sectionName = worksheet.Cells[row, 7].Value != null ? worksheet.Cells[row, 7].Value.ToString().Trim() : "";

                        if (!string.IsNullOrEmpty(sectionName))
                        {
                            //means is first row
                            roadSection = new WorkplanUploadSection();
                            workplanUploadSections.Add(roadSection);
                            roadSection.WorkplanUploadSectionActivities = new List<WorkplanUploadSectionActivity>();

                            roadSection.RegionCounty = worksheet.Cells[row, 1].Value != null ? worksheet.Cells[row, 1].Value.ToString().Trim() : "";
                            roadSection.Constituency = worksheet.Cells[row, 2].Value != null ? worksheet.Cells[row, 2].Value.ToString().Trim() : "";
                            roadSection.CityMunicipality = worksheet.Cells[row, 3].Value != null ? worksheet.Cells[row, 3].Value.ToString().Trim() : "";
                            roadSection.NationalPark = worksheet.Cells[row, 4].Value != null ? worksheet.Cells[row, 4].Value.ToString().Trim() : "";
                            roadSection.RoadId = worksheet.Cells[row, 6].Value != null ? worksheet.Cells[row, 6].Value.ToString().Trim() : "";
                            roadSection.SectionId = worksheet.Cells[row, 7].Value != null ? worksheet.Cells[row, 7].Value.ToString().Trim() : "";

                            roadSection.SectionName = sectionName;
                            roadSection.SurfaceType = worksheet.Cells[row, 9].Value != null ? worksheet.Cells[row, 9].Value.ToString().Trim() : "";
                            roadSection.WorkCategory = worksheet.Cells[row, 10].Value != null ? worksheet.Cells[row, 10].Value.ToString().Trim() : "";
                            double planLength;
                            double.TryParse(worksheet.Cells[row, 11].Value != null ? worksheet.Cells[row, 11].Value.ToString().Trim() : "0", out planLength);

                            roadSection.PlannedSectionLength = planLength;

                            uploadSectionActivity = new WorkplanUploadSectionActivity();
                            uploadSectionActivity.ActivityCode = worksheet.Cells[row, 12].Value != null ? worksheet.Cells[row, 12].Value.ToString().Trim() : "";
                            uploadSectionActivity.AcivityDescription = worksheet.Cells[row, 13].Value != null ? worksheet.Cells[row, 13].Value.ToString().Trim() : "";
                            uploadSectionActivity.Technology = worksheet.Cells[row, 14].Value != null ? worksheet.Cells[row, 14].Value.ToString().Trim() : "";
                            uploadSectionActivity.UnitMeasure = worksheet.Cells[row, 15].Value != null ? worksheet.Cells[row, 15].Value.ToString().Trim() : "";
                            decimal plannedQuantity;
                            decimal.TryParse(worksheet.Cells[row, 16].Value != null ? worksheet.Cells[row, 16].Value.ToString().Trim() : "", out plannedQuantity);
                            uploadSectionActivity.Quantity = plannedQuantity;
                            double planRate;
                            double.TryParse(worksheet.Cells[row, 17].Value != null ? worksheet.Cells[row, 17].Value.ToString().Trim() : "0", out planRate);
                            uploadSectionActivity.PlannedRate = planRate;
                        }
                        else
                        {
                            //is activity row
                            uploadSectionActivity = new WorkplanUploadSectionActivity();
                            uploadSectionActivity.ActivityCode = worksheet.Cells[row, 12].Value != null ? worksheet.Cells[row, 12].Value.ToString().Trim() : "";
                            uploadSectionActivity.AcivityDescription = worksheet.Cells[row, 13].Value != null ? worksheet.Cells[row, 13].Value.ToString().Trim() : "";
                            uploadSectionActivity.Technology = worksheet.Cells[row, 14].Value != null ? worksheet.Cells[row, 14].Value.ToString().Trim() : "";
                            uploadSectionActivity.UnitMeasure = worksheet.Cells[row, 15].Value != null ? worksheet.Cells[row, 15].Value.ToString().Trim() : "";
                            decimal plannedQuantity;
                            decimal.TryParse(worksheet.Cells[row, 16].Value != null ? worksheet.Cells[row, 16].Value.ToString().Trim() : "", out plannedQuantity);
                            uploadSectionActivity.Quantity = plannedQuantity;
                            double planRate;
                            double.TryParse(worksheet.Cells[row, 17].Value != null ? worksheet.Cells[row, 17].Value.ToString().Trim() : "0", out planRate);
                            uploadSectionActivity.PlannedRate = planRate;

                        }
                        if (!String.IsNullOrEmpty(uploadSectionActivity.AcivityDescription))
                            roadSection.WorkplanUploadSectionActivities.Add(uploadSectionActivity);
                        //check if county or agency is set

                        //find grand total and exit
                        var grandTotal = worksheet.Cells[row, 16].Value != null ? worksheet.Cells[row, 16].Value.ToString().Trim() : "";
                        if (String.Equals(grandTotal, "GRAND TOTAL")) // END OF FILE
                        {
                            break;
                        }
                    }
                }

                returnValid.WorkplanUploadSections = workplanUploadSections;
                //END VALIDATION FOR ROADWORKS ACTIVITIES

                //END VALIDATION FOR ADMIN ACTIVITIES


                //END VALIDATION FOR ADMIN ACTIVITIES

                returnValid.FileIsValid = true;

                return returnValid;
            }
            catch (Exception ex)
            {
                _logger.LogError($"WorkplanController.ValidateFile API Error {ex.Message}");
                return new UploadWorkplanValidation { FileIsValid = false, WorkplanUploadSections = new List<WorkplanUploadSection>() };
            }

        }

        //Generate the ARWP file
        public async Task<IActionResult> GetARWPForAgencyWithCode(string authorityCode, long? financialYearId)
        {
            FinancialYear financialYear = new FinancialYear();
            Authority authority = new Authority();

            if (financialYearId == null || financialYearId < 0)
            {
                return Json(new
                {
                    Success = false,
                    Message = "We could not find the financial year supplied"
                });
            }

            var authorityResp = await _authorityService.FindByCodeAsync(authorityCode).ConfigureAwait(false);
            if (authorityResp.Success)
            {
                return RedirectToAction("GetARWPForAgency", "Workplan", new { authorityId = authorityResp.Authority.ID, financialYearId = financialYearId, regionId = 0, countyId = 0 });
            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "We could not find the supplied authority / agenecy!"
                });
            }



        }

        public async Task<IActionResult> GetARWPForAgency(long authorityId, long? financialYearId, long regionId, long countyId)
        {
            try
            {

                //retrieve the records to be printed
                //get the current financial year
                var printData = new AWRPViewModel();


                //get the Authority to print for
                //var auth_id = ;
                var authorityResp = await _authorityService.FindByIdAsync(authorityId != 0 ? authorityId : countyId).ConfigureAwait(false);
                if (!authorityResp.Success)
                {
                    return Json(new { Success = false, Message = "The authority supplied is incorrect, please choose the right authority." });
                }
                var authority = authorityResp.Authority;
                printData.Authority = authority;

                //get the budget details for the authority
                var budgetHeaderResp = await _budgetCeilingHeaderService.FindCurrentAsync().ConfigureAwait(false);
                if (budgetHeaderResp.Success)
                    printData.BudgetCeilingHeader = budgetHeaderResp.BudgetCeilingHeader;

                //get budgets for the authority
                var totalBudget = 0.0;
                FinancialYear financialYear = new FinancialYear();

                if (financialYearId != null && financialYearId > 0)
                {

                    var fYearResp = await _financialYearService.FindByIdAsync((long)financialYearId).ConfigureAwait(false);
                    if (fYearResp.Success)
                        financialYear = fYearResp.FinancialYear;
                }
                else
                {
                    var fYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
                    if (fYearResp.Success)
                    {
                        financialYear = fYearResp.FinancialYear;
                    }
                    else
                    {
                        return Json(new { Success = false, Message = "The financial year supplied for record retrieval could not be found!" });
                    }

                }

                var ceilingResp = await _budgetCeilingService.FindApprovedByAuthorityForCurrentYear(financialYear.ID, authority.ID).ConfigureAwait(false);
                if (ceilingResp.Success)
                {
                    totalBudget += ceilingResp.BudgetCeiling.Amount;
                }

                //get the agency budget allocation for other funding source

                var budgetTotalOthers = await _revenueCollectionCodeUnitService.ListAsync(financialYear.ID, "Others").ConfigureAwait(false);
                if (budgetTotalOthers != null)
                {
                    totalBudget += budgetTotalOthers.RevenueCollectionCodeUnit.Where(a => a.AuthorityId == authority.ID).Sum(a => a.RevenueCollection.Amount);
                }

                //get the current workplans for the authority
                var authorityWorkplans = await _roadWorkSectionPlanService.ListByAgencyAsync(authority.ID, financialYear.ID).ConfigureAwait(false);
                List<RoadWorkSectionPlan> roadWorkSectionPlans = new List<RoadWorkSectionPlan>();


                if (regionId != 0)
                {
                    var region = await _regionService.GetRegionAsync(regionId).ConfigureAwait(false);
                    //get all constituencies in the region
                    List<long> constituencyIds = new List<long>();
                    List<long> countyIds = new List<long>();
                    //get the counties in the region
                    foreach (RegionToCounty regionToCounty in region.Region.RegionToCountys)
                    {
                        //loop through the constituencies to get the road ID
                        foreach (Constituency constituency in regionToCounty.County.Constituencys)
                        {
                            //get the county constitutencies
                            foreach (RoadWorkSectionPlan roadWorkSectionPlan in authorityWorkplans.ToList())
                            {

                                //chech the road section constituency if is same as region constituencies.
                                var roadSection = roadWorkSectionPlan.RoadSection;
                                if (roadSection.ConstituencyId == constituency.ID)
                                {
                                    //remove the workplan
                                    roadWorkSectionPlans.Add(roadWorkSectionPlan);
                                }
                            }
                        }


                    }
                    //return the filtered list
                    printData.RoadWorkSectionPlans = roadWorkSectionPlans;
                }
                else
                {
                    printData.RoadWorkSectionPlans = authorityWorkplans.ToList();
                }

                //check if there is any workplan to print
                if (!printData.RoadWorkSectionPlans.Any())
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "We did not find any workplans to load for download."
                    });
                }

                //generate the file.
                string handle = Guid.NewGuid().ToString();
                var memoryStream = new MemoryStream();
                using (ExcelPackage excel = new ExcelPackage(memoryStream))
                {
                    var sheetcreate = excel.Workbook.Worksheets.Add("Sheet1");
                    //print the column headers
                    //First Row
                    sheetcreate.Cells[1, 1].Value = "ANNUAL ROAD WORKS PROGRAMME ";
                    sheetcreate.Cells[1, 1].Style.Font.Bold = true;
                    sheetcreate.Cells[1, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreate.Cells[1, 2].Value = financialYear.Code;
                    sheetcreate.Cells[1, 2].Style.Font.Bold = true;
                    sheetcreate.Cells[1, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;



                    //Second Row
                    if (authority.Type == 1)
                    {
                        sheetcreate.Cells[2, 1].Value = "NAME OF AGENCY ";
                        sheetcreate.Cells[2, 1].Style.Font.Bold = true;
                        sheetcreate.Cells[2, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[2, 2].Value = printData.Authority.Name;
                        sheetcreate.Cells[2, 2].Style.Font.Bold = true;
                        sheetcreate.Cells[2, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }
                    else
                    {
                        sheetcreate.Cells[2, 1].Value = "NAME OF COUNTY GOVERNMENT ";
                        sheetcreate.Cells[2, 1].Style.Font.Bold = true;
                        sheetcreate.Cells[2, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[2, 2].Value = printData.Authority.Name;
                        sheetcreate.Cells[2, 2].Style.Font.Bold = true;
                        sheetcreate.Cells[2, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }

                    //Third Row
                    if (authority.Type == 1)
                    {
                        sheetcreate.Cells[3, 1].Value = "AGENCY CODE ";
                        sheetcreate.Cells[3, 1].Style.Font.Bold = true;
                        sheetcreate.Cells[3, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[3, 2].Value = printData.Authority.Code;
                        sheetcreate.Cells[3, 2].Style.Font.Bold = true;
                        sheetcreate.Cells[3, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }
                    else
                    {
                        sheetcreate.Cells[3, 1].Value = "COUNTY CODE ";
                        sheetcreate.Cells[3, 1].Style.Font.Bold = true;
                        sheetcreate.Cells[3, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[3, 2].Value = printData.Authority.Code;
                        sheetcreate.Cells[3, 2].Style.Font.Bold = true;
                        sheetcreate.Cells[3, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }

                    //Fourth Row

                    sheetcreate.Cells[4, 1].Value = "BUDGET (KSHS) ";
                    sheetcreate.Cells[4, 1].Style.Font.Bold = true;
                    sheetcreate.Cells[4, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[4, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreate.Cells[4, 2].Value = totalBudget.ToString("N");
                    sheetcreate.Cells[4, 2].Style.Font.Bold = true;
                    sheetcreate.Cells[4, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                    //create now the record items
                    //print headers
                    sheetcreate.Cells[6, 1].Value = "ITEM NO.";
                    sheetcreate.Cells[6, 1].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreate.Cells[6, 2].Value = "ROAD NUMBER";
                    sheetcreate.Cells[6, 2].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreate.Cells[6, 3].Value = "SECTION/ROAD NAME";
                    sheetcreate.Cells[6, 3].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreate.Cells[6, 4].Value = "SURFACE TYPE";
                    sheetcreate.Cells[6, 4].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreate.Cells[6, 5].Value = "LENGTH";
                    sheetcreate.Cells[6, 5].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreate.Cells[6, 6].Value = "ACTIVITY CODE";
                    sheetcreate.Cells[6, 6].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreate.Cells[6, 7].Value = "ACTIVITY DESCRIPTION";
                    sheetcreate.Cells[6, 7].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    /* sheetcreate.Cells[6, 8].Value = "WORKS CATEGORY";
                     sheetcreate.Cells[6, 8].Style.Font.Bold = true;
                     sheetcreate.Cells[6, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                     sheetcreate.Cells[6, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
 */
                    sheetcreate.Cells[6, 8].Value = "METHOD";
                    sheetcreate.Cells[6, 8].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreate.Cells[6, 9].Value = "UNIT MEASURE";
                    sheetcreate.Cells[6, 9].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreate.Cells[6, 10].Value = "PLANNED QUANTITY";
                    sheetcreate.Cells[6, 10].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreate.Cells[6, 11].Value = "PLANNED RATE";
                    sheetcreate.Cells[6, 11].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreate.Cells[6, 12].Value = "AMOUNT (KSH)";
                    sheetcreate.Cells[6, 12].Style.Font.Bold = true;
                    sheetcreate.Cells[6, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[6, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    int row = 7;
                    int itemNo = 1;
                    double total = 0;
                    foreach (var workplan in printData.RoadWorkSectionPlans.OrderBy(n => n.Road.RoadNumber))
                    {

                        double subTotal = 0;
                        // now set the individual activities
                        sheetcreate.Cells[row, 1].Value = itemNo;
                        sheetcreate.Cells[row, 1].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 2].Value = workplan.Road.RoadNumber;
                        sheetcreate.Cells[row, 2].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 3].Value = workplan.RoadSection.SectionName;
                        sheetcreate.Cells[row, 3].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 4].Value = workplan.RoadSection.SurfaceType.Description;
                        sheetcreate.Cells[row, 4].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 5].Value = workplan.RoadSection.Length;
                        sheetcreate.Cells[row, 5].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        if (workplan.PlanActivities.Any())
                        {
                            foreach (var plan in workplan.PlanActivities)
                            {
                                // sheetcreate.Cells[row, 1].Value = "";
                                sheetcreate.Cells[row, 1].Style.Font.Bold = true;
                                sheetcreate.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreate.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                // sheetcreate.Cells[row, 2].Value = "";
                                sheetcreate.Cells[row, 2].Style.Font.Bold = true;
                                sheetcreate.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreate.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                // sheetcreate.Cells[row, 3].Value = "";
                                sheetcreate.Cells[row, 3].Style.Font.Bold = true;
                                sheetcreate.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreate.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                // sheetcreate.Cells[row, 4].Value ="";
                                sheetcreate.Cells[row, 4].Style.Font.Bold = true;
                                sheetcreate.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreate.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreate.Cells[row, 5].Style.Font.Bold = true;
                                sheetcreate.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreate.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreate.Cells[row, 6].Value = plan.ItemActivityUnitCost.ItemCode + "-" + plan.ItemActivityUnitCost.SubItemCode + "-" + plan.ItemActivityUnitCost.SubSubItemCode;
                                sheetcreate.Cells[row, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreate.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreate.Cells[row, 7].Value = plan.ItemActivityUnitCost.Description;
                                sheetcreate.Cells[row, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreate.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreate.Cells[row, 8].Value = plan.Technology != null ? plan.Technology.Code : "";
                                sheetcreate.Cells[row, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreate.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreate.Cells[row, 9].Value = plan.ItemActivityUnitCost.UnitMeasure;
                                sheetcreate.Cells[row, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreate.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreate.Cells[row, 10].Value = plan.Quantity;
                                sheetcreate.Cells[row, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreate.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreate.Cells[row, 11].Value = plan.Rate;
                                sheetcreate.Cells[row, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreate.Cells[row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreate.Cells[row, 12].Value = plan.Amount.ToString("N");
                                sheetcreate.Cells[row, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreate.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                row++;

                                total = (total + plan.Amount);
                                subTotal += plan.Amount;

                            }
                        }


                        //PBC Activities
                        if (workplan.PlanActivityPBCs.Any())
                        {
                            foreach (var plan in workplan.PlanActivityPBCs)
                            {
                                // sheetcreate.Cells[row, 1].Value = "";
                                sheetcreate.Cells[row, 1].Style.Font.Bold = true;
                                sheetcreate.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreate.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                // sheetcreate.Cells[row, 2].Value = "";
                                sheetcreate.Cells[row, 2].Style.Font.Bold = true;
                                sheetcreate.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreate.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                // sheetcreate.Cells[row, 3].Value = "";
                                sheetcreate.Cells[row, 3].Style.Font.Bold = true;
                                sheetcreate.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreate.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                // sheetcreate.Cells[row, 4].Value ="";
                                sheetcreate.Cells[row, 4].Style.Font.Bold = true;
                                sheetcreate.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreate.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreate.Cells[row, 5].Style.Font.Bold = true;
                                sheetcreate.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreate.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreate.Cells[row, 6].Value = plan.ItemActivityPBC.Code;
                                sheetcreate.Cells[row, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreate.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreate.Cells[row, 7].Value = plan.ItemActivityPBC.Description;
                                sheetcreate.Cells[row, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreate.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreate.Cells[row, 8].Value = plan.ItemActivityPBC.Technology != null ? plan.ItemActivityPBC.Technology.Code : "";
                                sheetcreate.Cells[row, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreate.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreate.Cells[row, 9].Value = "KM Per Month";
                                sheetcreate.Cells[row, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreate.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreate.Cells[row, 10].Value = plan.PlannedKM;
                                sheetcreate.Cells[row, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreate.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreate.Cells[row, 11].Value = plan.CostPerKMPerMonth;
                                sheetcreate.Cells[row, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreate.Cells[row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreate.Cells[row, 12].Value = plan.TotalAmount.ToString("N");
                                sheetcreate.Cells[row, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreate.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                row++;

                                total = (total + plan.TotalAmount);
                                subTotal += plan.TotalAmount;

                            }
                        }



                        //set subtotal
                        row = row + 1;
                        sheetcreate.Cells[row, 1].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 2].Value = "";
                        sheetcreate.Cells[row, 2].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 3].Value = "";
                        sheetcreate.Cells[row, 3].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 4].Value ="";
                        sheetcreate.Cells[row, 4].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                        //sheetcreate.Cells[row, 5].Value = plan.ItemActivityUnitCost.SubSubItemCode;
                        sheetcreate.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 6].Value = plan.ItemActivityUnitCost.Description;
                        sheetcreate.Cells[row, 6].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 7].Value = plan.Technology;
                        sheetcreate.Cells[row, 7].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 8].Value = plan.ItemActivityUnitCost.UnitMeasure;
                        sheetcreate.Cells[row, 8].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 9].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 10].Value = "Sub Total (KSH)";
                        sheetcreate.Cells[row, 10].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 11].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 12].Value = subTotal.ToString("N");
                        sheetcreate.Cells[row, 12].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //set the VAT field
                        row = row + 1;
                        sheetcreate.Cells[row, 1].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 2].Value = "";
                        sheetcreate.Cells[row, 2].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 3].Value = "";
                        sheetcreate.Cells[row, 3].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 4].Value ="";
                        sheetcreate.Cells[row, 4].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                        //sheetcreate.Cells[row, 5].Value = plan.ItemActivityUnitCost.SubSubItemCode;
                        sheetcreate.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 6].Value = plan.ItemActivityUnitCost.Description;
                        sheetcreate.Cells[row, 6].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 7].Value = plan.Technology;
                        sheetcreate.Cells[row, 7].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 8].Value = plan.ItemActivityUnitCost.UnitMeasure;
                        sheetcreate.Cells[row, 8].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 9].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 10].Value = "16 % VAT";
                        sheetcreate.Cells[row, 10].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 11].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 12].Value = (0.16 * subTotal).ToString("N");
                        sheetcreate.Cells[row, 12].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //set the Contigencies field
                        row = row + 1;
                        sheetcreate.Cells[row, 1].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 2].Value = "";
                        sheetcreate.Cells[row, 2].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 3].Value = "";
                        sheetcreate.Cells[row, 3].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 4].Value ="";
                        sheetcreate.Cells[row, 4].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                        //sheetcreate.Cells[row, 5].Value = plan.ItemActivityUnitCost.SubSubItemCode;
                        sheetcreate.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 6].Value = plan.ItemActivityUnitCost.Description;
                        sheetcreate.Cells[row, 6].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 7].Value = plan.Technology;
                        sheetcreate.Cells[row, 7].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 8].Value = plan.ItemActivityUnitCost.UnitMeasure;
                        sheetcreate.Cells[row, 8].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 9].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 10].Value = "Contigencies@0%";
                        sheetcreate.Cells[row, 10].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 11].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 12].Value = "-";
                        sheetcreate.Cells[row, 12].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //set total with contigencies and VAT
                        row = row + 1;
                        sheetcreate.Cells[row, 1].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 2].Value = "";
                        sheetcreate.Cells[row, 2].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 3].Value = "";
                        sheetcreate.Cells[row, 3].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 4].Value ="";
                        sheetcreate.Cells[row, 4].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                        //sheetcreate.Cells[row, 5].Value = plan.ItemActivityUnitCost.SubSubItemCode;
                        sheetcreate.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 6].Value = plan.ItemActivityUnitCost.Description;
                        sheetcreate.Cells[row, 6].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 7].Value = plan.Technology;
                        sheetcreate.Cells[row, 7].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 8].Value = plan.ItemActivityUnitCost.UnitMeasure;
                        sheetcreate.Cells[row, 8].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 9].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 10].Value = "Total";
                        sheetcreate.Cells[row, 10].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 11].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 12].Value = (1.16 * subTotal).ToString("N");
                        sheetcreate.Cells[row, 12].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //set empty same
                        row = row + 2;
                        sheetcreate.Cells[row, 1].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 2].Value = "";
                        sheetcreate.Cells[row, 2].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 3].Value = "";
                        sheetcreate.Cells[row, 3].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 4].Value ="";
                        sheetcreate.Cells[row, 4].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                        //sheetcreate.Cells[row, 5].Value = plan.ItemActivityUnitCost.SubSubItemCode;
                        sheetcreate.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 6].Value = plan.ItemActivityUnitCost.Description;
                        sheetcreate.Cells[row, 6].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 7].Value = plan.Technology;
                        sheetcreate.Cells[row, 7].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 8].Value = plan.ItemActivityUnitCost.UnitMeasure;
                        sheetcreate.Cells[row, 8].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 9].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 10].Value = "";
                        sheetcreate.Cells[row, 10].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 11].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreate.Cells[row, 12].Value = "";
                        sheetcreate.Cells[row, 12].Style.Font.Bold = true;
                        sheetcreate.Cells[row, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreate.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        itemNo++;

                    }
                    sheetcreate.Cells[row + 1, 10].Value = "Grand Total (KSH)";
                    sheetcreate.Cells[row + 1, 10].Style.Font.Bold = true;
                    sheetcreate.Cells[row + 1, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[row + 1, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    //sheetcreate.Cells[row, 10].Value = plan.Rate;
                    sheetcreate.Cells[row + 1, 11].Style.Font.Bold = true;
                    sheetcreate.Cells[row + 1, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[row + 1, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreate.Cells[row + 1, 12].Value = (1.16 * total).ToString("N");
                    sheetcreate.Cells[row + 1, 12].Style.Font.Bold = true;
                    sheetcreate.Cells[row + 1, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[row + 1, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    //signed by line

                    sheetcreate.Cells[row + 3, 1].Value = "Signed By";
                    sheetcreate.Cells[row + 3, 1].Style.Font.Bold = true;
                    // sheetcreate.Cells[row + 3, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[row + 3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    /*string mergeStringSign = "B" + row + 3 + ":D" + row + 3;

                    sheetcreate.Cells[mergeStringSign].Value = " ";
                    sheetcreate.Cells[mergeStringSign].Merge = true;
                    sheetcreate.Cells[mergeStringSign].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    sheetcreate.Cells[mergeStringSign].Style.Font.Bold = true;*/
                    //sheetcreate.Cells[mergeStringSign].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    sheetcreate.Cells[row + 3, 4].Value = "Designation";
                    sheetcreate.Cells[row + 3, 4].Style.Font.Bold = true;
                    // sheetcreate.Cells[row + 3, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[row + 3, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                    sheetcreate.Cells[row + 3, 7].Value = "Date";
                    sheetcreate.Cells[row + 3, 7].Style.Font.Bold = true;
                    // sheetcreate.Cells[row + 3, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[row + 3, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreate.Cells[row + 3, 9].Value = "Sign";
                    sheetcreate.Cells[row + 3, 9].Style.Font.Bold = true;
                    // sheetcreate.Cells[row + 3, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[row + 3, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    /*  string mergeStringSignDesignation = "F" + row + 3 + ":G" + row + 3;

                      sheetcreate.Cells[mergeStringSignDesignation].Value = " ";
                      sheetcreate.Cells[mergeStringSignDesignation].Merge = true;
                      sheetcreate.Cells[mergeStringSignDesignation].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                      sheetcreate.Cells[mergeStringSignDesignation].Style.Font.Bold = true;*/
                    // sheetcreate.Cells[mergeStringSignDesignation].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);


                    sheetcreate.Cells.AutoFitColumns();
                    excel.Workbook.Properties.Title = "Attempts";
                    _cache.Set(handle, excel.GetAsByteArray(),
                                    new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));

                    return Json(new
                    {
                        Success = true,
                        Message = "Success",
                        Href = Url.Action("Download", "Workplan", new { fileGuid = handle, FileName = "GetARWPForAgency.xlsx" })
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ARICSController.GetRoads API Error {ex.Message}");
                return Json(new
                {
                    Success = false,
                    Message = "Error occured"
                });
            }

        }

        public async Task<IActionResult> GetAPRPForAgency(long authorityId, long financialYearId, long regionId, long countyId)
        {
            var authorityResp = await _authorityService.FindByIdAsync(authorityId).ConfigureAwait(false);
            if (!authorityResp.Success)
            {
                return Json(new { Success = false, Message = "We could not find the authority requested. Please contact the system administrator." });
            }
            switch (authorityResp.Authority.Code.ToUpper())
            {
                case "KENHA":
                    return RedirectToAction("GetAPRPForKenha", "Workplan", new { authorityId = authorityId, financialYearId = financialYearId, regionId = regionId, countyId = countyId });
                case "KURA":
                    return RedirectToAction("GetAPRPForKura", "Workplan", new { authorityId = authorityId, financialYearId = financialYearId, regionId = regionId, countyId = countyId });
                case "KERRA":
                    return RedirectToAction("GetAPRPForKerra", "Workplan", new { authorityId = authorityId, financialYearId = financialYearId, regionId = regionId, countyId = countyId });
                case "KWS":
                    return RedirectToAction("GetAPRPForKws", "Workplan", new { authorityId = authorityId, financialYearId = financialYearId, regionId = regionId, countyId = countyId });
                    break;
                default:
                    return Json(new { Success = false, Message = "APRP For the authority is still in implementation stage. Please contact the system administrator." });

            }
            //working on Kenha for now
            if (!authorityResp.Authority.Code.ToUpper().Equals("KENHA"))
            {
                return Json(new { Success = false, Message = "APRP For the authority is still in implementation stage. Please contact the system administrator." });
            }

            return RedirectToAction("GetAPRPForKenha", "Workplan", new { authorityId = authorityId, financialYearId = financialYearId, regionId = regionId, countyId = countyId });

        }

        public async Task<IActionResult> GetAPRPForKerra(long authorityId, long financialYearId, long regionId, long countyId)
        {
            try
            {
                //retrieve the records to be printed
                var printData = new AWRPViewModel();
                if (financialYearId > 0)
                {
                    //get the supplied financial year
                    var financialYearResp = await _financialYearService.FindByIdAsync((long)financialYearId).ConfigureAwait(false);
                    if (financialYearResp.Success)
                    {
                        printData.FinancialYear = financialYearResp.FinancialYear;
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "The financial year supplied could not be retrieved."
                        });
                    }
                }
                else
                {
                    //get the current financial year
                    var financialYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
                    if (financialYearResp.Success)
                        printData.FinancialYear = financialYearResp.FinancialYear;
                }




                //get the Authority to print for
                var authorityResp = await _authorityService.FindByIdAsync(authorityId != 0 ? authorityId : countyId).ConfigureAwait(false);
                if (!authorityResp.Success)
                {
                    return Json(new { Success = false, Message = "The authority supplied is incorrect, please choose the right authority." });
                }
                var authority = authorityResp.Authority;
                printData.Authority = authority;

                var ceilingResp = await _budgetCeilingService.FindApprovedByAuthorityForCurrentYear(printData.FinancialYear.ID, printData.Authority.ID).ConfigureAwait(false);
                printData.BudgetCeiling = ceilingResp.BudgetCeiling;

                //get the budget details for the authority
                var budgetHeaderResp = await _budgetCeilingHeaderService.FindCurrentAsync().ConfigureAwait(false);
                if (budgetHeaderResp.Success)
                    printData.BudgetCeilingHeader = budgetHeaderResp.BudgetCeilingHeader;

                //get the administrative activities for the authority
                var authorityAdminActivities = await _adminOperationalActivityService.ListByAuthorityAsync(printData.Authority.ID, printData.FinancialYear.ID).ConfigureAwait(false);
                printData.AdminOperationalActivities = authorityAdminActivities.ToList();
                //get the current workplans for the authority
                var authorityWorkplans = await _roadWorkSectionPlanService.ListByAgencyAsync(authority.ID, printData.FinancialYear.ID).ConfigureAwait(false);
                if (!authorityWorkplans.Any())
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "There are no records for the selected county in the selected financial year that could be retrieved."
                    });
                }
                printData.RoadWorkSectionPlans = authorityWorkplans.ToList();

                //generate the file.
                string handle = Guid.NewGuid().ToString();
                var memoryStream = new MemoryStream();
                using (ExcelPackage excel = new ExcelPackage(memoryStream))
                {
                    string currentFileName = System.IO.Path.GetFileName("Kerra_APRP");


                    var workTypeSummarySheet = excel.Workbook.Worksheets.Add("Grand Summary");
                    var worksByRegionsSummarySheet = excel.Workbook.Worksheets.Add("Regional Works Cat. Sum");
                    var aprpSummarySheet = excel.Workbook.Worksheets.Add("APRP format");

                    //WORKS CATEGORY SUMMARY
                    int rowCatSum = 3;
                    int countCat = 1;
                    var authorityRegions = await _regionService.ListRegionsByAuthority(printData.Authority.ID).ConfigureAwait(false);

                    var worksCategorySummary = printData.RoadWorkSectionPlans.GroupBy(p => p.WorkCategory).Select(
                            cat => new
                            {
                                Key = cat.Key,
                                Value = cat.First().WorkCategory.Name,
                                Code = cat.First().WorkCategory.Code,
                                KMs = cat.Sum(c => c.RoadSection.Length),
                                Budget = cat.Sum(p => p.TotalEstimateCost)
                            }
                        );

                    var groupedActivties = printData.AdminOperationalActivities.GroupBy(g => g.ActivityGroup).Select(
                      g => new
                      {
                          Key = g.Key,
                          Value = g.Sum(s => s.Amount),
                          Name = g.First().ActivityGroup
                      }
                    );

                    //print admin activities
                    workTypeSummarySheet.Cells["A1:D1"].Value = "15% RMLF KERRA ANNUAL ROADS WORK PROGRAMME (ARWP) FOR FINANCIAL YEAR " + printData.FinancialYear.Code + ". GRAND SUMMARY";
                    workTypeSummarySheet.Cells["A1:D1"].Merge = true;
                    workTypeSummarySheet.Cells["A1:D1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workTypeSummarySheet.Cells["A1:D1"].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells["A1:D1"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Row(1).Height = 30;
                    workTypeSummarySheet.Cells["A1:D1"].Style.WrapText = true;


                    workTypeSummarySheet.Cells[2, 1].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[2, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workTypeSummarySheet.Cells[2, 1].Value = "NO.";

                    workTypeSummarySheet.Cells[2, 2].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[2, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workTypeSummarySheet.Cells[2, 2].Value = "WORKS CATEGORY";

                    workTypeSummarySheet.Cells[2, 3].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[2, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workTypeSummarySheet.Cells[2, 3].Value = "KMS ";

                    workTypeSummarySheet.Cells[2, 4].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[2, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workTypeSummarySheet.Cells[2, 4].Value = "BUDGET FY " + printData.FinancialYear.Code;


                    foreach (var activity in printData.AdminOperationalActivities)
                    {
                        workTypeSummarySheet.Cells[rowCatSum, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        workTypeSummarySheet.Cells[rowCatSum, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workTypeSummarySheet.Cells[rowCatSum, 1].Value = countCat++;

                        workTypeSummarySheet.Cells[rowCatSum, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        workTypeSummarySheet.Cells[rowCatSum, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workTypeSummarySheet.Cells[rowCatSum, 2].Value = (activity.ActivityWorks != null ? activity.ActivityWorks.ToUpper() : "");

                        workTypeSummarySheet.Cells[rowCatSum, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        workTypeSummarySheet.Cells[rowCatSum, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workTypeSummarySheet.Cells[rowCatSum, 3].Value = "-";//get summary of workplans in that region

                        workTypeSummarySheet.Cells[rowCatSum, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        workTypeSummarySheet.Cells[rowCatSum, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workTypeSummarySheet.Cells[rowCatSum, 4].Value = activity.Amount;//get summary of workplans in that region
                        workTypeSummarySheet.Cells[rowCatSum, 4].Style.Numberformat.Format = "#,##0.00";
                        rowCatSum++;
                    }

                    //print works category summary

                    foreach (var category in worksCategorySummary)
                    {
                        workTypeSummarySheet.Cells[rowCatSum, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        workTypeSummarySheet.Cells[rowCatSum, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workTypeSummarySheet.Cells[rowCatSum, 1].Value = countCat++;

                        workTypeSummarySheet.Cells[rowCatSum, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        workTypeSummarySheet.Cells[rowCatSum, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workTypeSummarySheet.Cells[rowCatSum, 2].Value = category.Value.ToUpper();

                        workTypeSummarySheet.Cells[rowCatSum, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        workTypeSummarySheet.Cells[rowCatSum, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workTypeSummarySheet.Cells[rowCatSum, 3].Value = category.KMs;//get summary of workplans in that region
                        workTypeSummarySheet.Cells[rowCatSum, 3].Style.Numberformat.Format = "#,##0.00";

                        workTypeSummarySheet.Cells[rowCatSum, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        workTypeSummarySheet.Cells[rowCatSum, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workTypeSummarySheet.Cells[rowCatSum, 4].Value = category.Budget;//get summary of workplans in that region
                        workTypeSummarySheet.Cells[rowCatSum, 4].Style.Numberformat.Format = "#,##0.00";
                        rowCatSum++;
                    }

                    //grand total
                    workTypeSummarySheet.Cells[rowCatSum, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[rowCatSum, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    workTypeSummarySheet.Cells[rowCatSum, 2].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[rowCatSum, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[rowCatSum, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workTypeSummarySheet.Cells[rowCatSum, 2].Value = "GRAND TOTAL";

                    workTypeSummarySheet.Cells[rowCatSum, 3].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[rowCatSum, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[rowCatSum, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workTypeSummarySheet.Cells[rowCatSum, 3].Value = printData.RoadWorkSectionPlans.Sum(s => s.PlannedLength);//get summary of workplans in that region
                    workTypeSummarySheet.Cells[rowCatSum, 3].Style.Numberformat.Format = "#,##0.00";

                    workTypeSummarySheet.Cells[rowCatSum, 4].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[rowCatSum, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[rowCatSum, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workTypeSummarySheet.Cells[rowCatSum, 4].Value = (printData.AdminOperationalActivities.Sum(a => a.Amount) + printData.RoadWorkSectionPlans.Sum(p => p.TotalEstimateCost));//get summary of workplans in that region
                    workTypeSummarySheet.Cells[rowCatSum, 4].Style.Numberformat.Format = "#,##0.00";

                    workTypeSummarySheet.Cells.AutoFitColumns();


                    //END WORKS CATEGORY SUMMARY

                    //BEGIN WORKS BY REGIONS


                    int categoryCount = worksCategorySummary.Count();
                    var columnCount = (categoryCount * 2) + printData.AdminOperationalActivities.Count; // times *2 because each category spans two columns
                    int cellIdRegion = printData.AdminOperationalActivities.Count;
                    cellIdRegion += 7;
                    var loopCellId = cellIdRegion;
                    var countEntryRows = 6; // 6 because, the first data is here.
                    //include number of admin activities in output
                    countEntryRows += groupedActivties.Count();

                    //include number of constituencies in the region
                    foreach (var region in authorityRegions)
                    {

                        var regionConstituencies = region.RegionToCountys.Select(c => c.County.Constituencys);
                        List<Constituency> constituencies = new List<Constituency>();
                        foreach (var constituency in regionConstituencies)
                        {
                            countEntryRows += constituency.Count();
                        }
                        //include the total row for each region

                        countEntryRows++;
                    }


                    ///let us predetermine the total number of rows expected here



                    //print admin activities
                    worksByRegionsSummarySheet.Cells[1, 1, 1, (columnCount + 6)].Value = "15% RMLF KERRA ANNUAL ROADS WORK PROGRAMME (ARWP) FOR FINANCIAL YEAR " + printData.FinancialYear.Code + ". GRAND SUMMARY";

                    //worksheet.Cells["A1:B5"].Merge = true; =   workTypeSummarySheet.Cells[1, 1, 5, 2]

                    worksByRegionsSummarySheet.Cells[1, 1, 1, (columnCount + 6)].Merge = true;
                    worksByRegionsSummarySheet.Cells[1, 1, 1, (columnCount + 6)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksByRegionsSummarySheet.Cells[1, 1, 1, (columnCount + 6)].Style.Font.Bold = true;
                    worksByRegionsSummarySheet.Cells[1, 1, 1, (columnCount + 6)].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksByRegionsSummarySheet.Row(1).Height = 30;
                    worksByRegionsSummarySheet.Cells[1, 1, 1, (columnCount + 6)].Style.WrapText = true;

                    //set the No column and region
                    worksByRegionsSummarySheet.Cells[4, 1].Style.Font.Bold = true;
                    worksByRegionsSummarySheet.Cells[4, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksByRegionsSummarySheet.Cells[4, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksByRegionsSummarySheet.Cells[4, 1].Value = "NO.";

                    worksByRegionsSummarySheet.Cells[4, 2].Style.Font.Bold = true;
                    worksByRegionsSummarySheet.Cells[4, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksByRegionsSummarySheet.Cells[4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksByRegionsSummarySheet.Cells[4, 2].Value = "COUNTY";

                    worksByRegionsSummarySheet.Cells[4, 3].Style.Font.Bold = true;
                    worksByRegionsSummarySheet.Cells[4, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksByRegionsSummarySheet.Cells[4, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksByRegionsSummarySheet.Cells[4, 3].Value = "NO.";

                    worksByRegionsSummarySheet.Cells[4, 4].Style.Font.Bold = true;
                    worksByRegionsSummarySheet.Cells[4, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksByRegionsSummarySheet.Cells[4, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksByRegionsSummarySheet.Cells[4, 4].Value = "CONSTITUENCY";

                    worksByRegionsSummarySheet.Cells[4, 5].Style.Font.Bold = true;
                    worksByRegionsSummarySheet.Cells[4, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksByRegionsSummarySheet.Cells[4, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksByRegionsSummarySheet.Cells[4, 5].Value = "22% ALLOCATION";

                    worksByRegionsSummarySheet.Cells[4, 6].Style.Font.Bold = true;
                    worksByRegionsSummarySheet.Cells[4, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksByRegionsSummarySheet.Cells[4, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksByRegionsSummarySheet.Cells[4, 6].Value = "10% ALLOCATION";


                    //output the report headers
                    int cellId = 7;
                    //print the admin activities groups.
                    int rowCountSum = 6;
                    int countyCount = 1;
                    int constituencyCount = 1;
                    foreach (var activityGroup in groupedActivties)
                    {

                        worksByRegionsSummarySheet.Cells[rowCountSum, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksByRegionsSummarySheet.Cells[rowCountSum, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksByRegionsSummarySheet.Cells[rowCountSum, 1].Value = countyCount++;

                        worksByRegionsSummarySheet.Cells[rowCountSum, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksByRegionsSummarySheet.Cells[rowCountSum, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksByRegionsSummarySheet.Cells[rowCountSum, 2].Value = (activityGroup.Name != null ? activityGroup.Name.ToUpper() : "");

                        worksByRegionsSummarySheet.Cells[rowCountSum, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksByRegionsSummarySheet.Cells[rowCountSum, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksByRegionsSummarySheet.Cells[rowCountSum, 3].Value = constituencyCount++; ;


                        foreach (var activity in printData.AdminOperationalActivities.Where(a => a.ActivityGroup == activityGroup.Name))
                        {
                            worksByRegionsSummarySheet.Cells[4, cellId].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            worksByRegionsSummarySheet.Cells[4, cellId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksByRegionsSummarySheet.Cells[4, cellId].Value = (activity.ActivityWorks != null ? activity.ActivityWorks.ToUpper() : "");


                            worksByRegionsSummarySheet.Cells[5, cellId].Style.Font.Bold = true;
                            worksByRegionsSummarySheet.Cells[5, cellId].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            worksByRegionsSummarySheet.Cells[5, cellId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksByRegionsSummarySheet.Cells[5, cellId].Value = " BUDGET FY " + printData.FinancialYear.Code;

                            //output the values in the group row  for each

                            worksByRegionsSummarySheet.Cells[rowCountSum, cellId].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            worksByRegionsSummarySheet.Cells[rowCountSum, cellId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksByRegionsSummarySheet.Cells[rowCountSum, cellId].Value = activity.Amount;
                            worksByRegionsSummarySheet.Cells[rowCountSum, cellId].Style.Numberformat.Format = "#,##0.00";

                            cellId++;
                            //reset cost for the next cell
                        }

                        rowCountSum++;

                    }


                    //work category summary
                    var recountCount = rowCountSum;

                    foreach (var cat in worksCategorySummary)
                    {
                        worksByRegionsSummarySheet.Cells[4, cellId, 4, (cellId + 1)].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksByRegionsSummarySheet.Cells[4, cellId, 4, (cellId + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksByRegionsSummarySheet.Cells[4, cellId, 4, (cellId + 1)].Value = cat.Value.ToUpper();
                        //worksByRegionsSummarySheet.Cells[4, cellId, 4, (cellId + 1)].Style.Font.Bold = true;
                        worksByRegionsSummarySheet.Cells[4, cellId, 4, (cellId + 1)].Merge = true;


                        worksByRegionsSummarySheet.Cells[5, cellId].Style.Font.Bold = true;
                        worksByRegionsSummarySheet.Cells[5, cellId].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksByRegionsSummarySheet.Cells[5, cellId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksByRegionsSummarySheet.Cells[5, cellId].Value = " KMS ";

                        worksByRegionsSummarySheet.Cells[5, (cellId + 1)].Style.Font.Bold = true;
                        worksByRegionsSummarySheet.Cells[5, (cellId + 1)].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksByRegionsSummarySheet.Cells[5, (cellId + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksByRegionsSummarySheet.Cells[5, (cellId + 1)].Value = " BUDGET FY " + printData.FinancialYear.Code;


                        cellId += 2;

                    }



                    var countEntryColumns = ((2 * worksCategorySummary.Count()) + printData.AdminOperationalActivities.Count());
                    //outputing region values
                    foreach (var region in authorityRegions)
                    {

                        var regionConstituencies = region.RegionToCountys.Select(c => c.County.Constituencys);
                        List<Constituency> constituencies = new List<Constituency>();
                        foreach (var constituency in regionConstituencies)
                        {
                            foreach (var consti in constituency)
                            {
                                constituencies.Add(consti);
                            }
                        }

                        var regionWorkplans = printData.RoadWorkSectionPlans.Where(s => constituencies.Any(c => c.ID == (long)s.RoadSection.ConstituencyId));


                        worksByRegionsSummarySheet.Cells[rowCountSum, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksByRegionsSummarySheet.Cells[rowCountSum, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksByRegionsSummarySheet.Cells[rowCountSum, 1].Value = countyCount++;

                        worksByRegionsSummarySheet.Cells[rowCountSum, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksByRegionsSummarySheet.Cells[rowCountSum, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksByRegionsSummarySheet.Cells[rowCountSum, 2].Value = region.Name;

                        worksByRegionsSummarySheet.Cells[rowCountSum, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksByRegionsSummarySheet.Cells[rowCountSum, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksByRegionsSummarySheet.Cells[rowCountSum, 3].Value = constituencyCount++;

                        //output constituencies in the region
                        foreach (var constituency in constituencies)
                        {

                            worksByRegionsSummarySheet.Cells[rowCountSum, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            worksByRegionsSummarySheet.Cells[rowCountSum, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksByRegionsSummarySheet.Cells[rowCountSum, 4].Value = constituency.Name;

                            // output the workplans in the constituency summaries
                            foreach (var cat in worksCategorySummary)
                            {
                                //output summaries only on the specific cells

                                worksByRegionsSummarySheet.Cells[rowCountSum, cellIdRegion].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                worksByRegionsSummarySheet.Cells[rowCountSum, cellIdRegion].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksByRegionsSummarySheet.Cells[rowCountSum, cellIdRegion].Value = (double)regionWorkplans.Where(c => c.WorkCategory.Code == cat.Code && c.RoadSection.Constituency.ID == constituency.ID).Sum(c => c.RoadSection.Length);
                                worksByRegionsSummarySheet.Cells[rowCountSum, cellIdRegion].Style.Numberformat.Format = "#,##0.00";

                                worksByRegionsSummarySheet.Cells[rowCountSum, (cellIdRegion + 1)].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                worksByRegionsSummarySheet.Cells[rowCountSum, (cellIdRegion + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksByRegionsSummarySheet.Cells[rowCountSum, (cellIdRegion + 1)].Value = (double)regionWorkplans.Where(c => c.WorkCategory.Code == cat.Code && c.RoadSection.Constituency.ID == constituency.ID).Sum(c => c.TotalEstimateCost);
                                worksByRegionsSummarySheet.Cells[rowCountSum, (cellIdRegion + 1)].Style.Numberformat.Format = "#,##0.00";

                                cellIdRegion += 2;

                            }
                            rowCountSum++;
                            cellIdRegion = loopCellId;
                        }

                        //reset recount to old stage for the next region
                        //reset the cellId
                        rowCountSum++;
                    }

                    //output the total summaries
                    worksByRegionsSummarySheet.Cells[rowCountSum, 1].Style.Font.Bold = true;
                    worksByRegionsSummarySheet.Cells[rowCountSum, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksByRegionsSummarySheet.Cells[rowCountSum, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksByRegionsSummarySheet.Cells[rowCountSum, 1].Value = (rowCountSum - 3);

                    worksByRegionsSummarySheet.Cells[rowCountSum, 2].Style.Font.Bold = true;
                    worksByRegionsSummarySheet.Cells[rowCountSum, 1, rowCountSum, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksByRegionsSummarySheet.Cells[rowCountSum, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksByRegionsSummarySheet.Cells[rowCountSum, 2].Value = "GRAND TOTAL";

                    //loop through the categories and pull summation
                    int cellTotal = 7;

                    foreach (var activity in printData.AdminOperationalActivities)
                    {


                        //output the values in the group row  for each

                        worksByRegionsSummarySheet.Cells[rowCountSum, cellTotal].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksByRegionsSummarySheet.Cells[rowCountSum, cellTotal].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksByRegionsSummarySheet.Cells[rowCountSum, cellTotal].Value = activity.Amount;
                        worksByRegionsSummarySheet.Cells[rowCountSum, cellTotal].Style.Numberformat.Format = "#,##0.00";

                        cellTotal++;
                        //reset cost for the next cell
                    }

                    foreach (var cat in worksCategorySummary)
                    {
                        //output summaries only on the specific cells

                        worksByRegionsSummarySheet.Cells[rowCountSum, cellTotal].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksByRegionsSummarySheet.Cells[rowCountSum, cellTotal].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksByRegionsSummarySheet.Cells[rowCountSum, cellTotal].Value = printData.RoadWorkSectionPlans.Where(c => c.WorkCategory.Code == cat.Code).Sum(c => c.PlannedLength);
                        worksByRegionsSummarySheet.Cells[rowCountSum, cellTotal].Style.Numberformat.Format = "#,##0.00";

                        worksByRegionsSummarySheet.Cells[rowCountSum, (cellTotal + 1)].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksByRegionsSummarySheet.Cells[rowCountSum, (cellTotal + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksByRegionsSummarySheet.Cells[rowCountSum, (cellTotal + 1)].Value = (double)printData.RoadWorkSectionPlans.Where(c => c.WorkCategory.Code == cat.Code).Sum(c => c.TotalEstimateCost);
                        worksByRegionsSummarySheet.Cells[rowCountSum, (cellTotal + 1)].Style.Numberformat.Format = "#,##0.00";

                        cellTotal += 2;

                    }




                    worksByRegionsSummarySheet.Cells.AutoFitColumns();


                    //END WORKS BY REGION

                    //BEGIN APRP SHEET


                    //print admin activities
                    aprpSummarySheet.Cells[1, 1, 1, 9].Value = "15% PORTION OF RMLF KeRRA WORK  PLAN FOR FINANCIAL YEAR " + printData.FinancialYear.Code + ". CONSTITUENCY ROADS";

                    //worksheet.Cells["A1:B5"].Merge = true; =   workTypeSummarySheet.Cells[1, 1, 5, 2]

                    aprpSummarySheet.Cells[1, 1, 1, 9].Merge = true;
                    aprpSummarySheet.Cells[1, 1, 1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[1, 1, 1, 9].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[1, 1, 1, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Row(1).Height = 30;
                    aprpSummarySheet.Cells[1, 1, 1, 9].Style.WrapText = true;

                    aprpSummarySheet.Cells[2, 1, 2, 9].Value = "ADDITIONAL FUNDS (KES) PER CONSTITUENCY ";

                    //worksheet.Cells["A1:B5"].Merge = true; =   workTypeSummarySheet.Cells[1, 1, 5, 2]

                    aprpSummarySheet.Cells[2, 1, 2, 9].Merge = true;
                    aprpSummarySheet.Cells[2, 1, 2, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[2, 1, 2, 9].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[2, 1, 2, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Row(2).Height = 30;
                    aprpSummarySheet.Cells[2, 1, 2, 9].Style.WrapText = true;

                    //set the No column and region
                    aprpSummarySheet.Cells[3, 1].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[3, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[3, 1].Value = "NO";

                    aprpSummarySheet.Cells[3, 2].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[3, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[3, 2].Value = "";

                    aprpSummarySheet.Cells[3, 3].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[3, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[3, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[3, 3].Value = "REGION / CONSTITUENCY";

                    aprpSummarySheet.Cells[3, 4].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[3, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[3, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[3, 4].Value = "ROAD NO.";

                    aprpSummarySheet.Cells[3, 5].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[3, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[3, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[3, 5].Value = "ACTIVITY /WORKS CATEGORY";

                    aprpSummarySheet.Cells[3, 6].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[3, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[3, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[3, 6].Value = "ST";

                    aprpSummarySheet.Cells[3, 7].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[3, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[3, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[3, 7].Value = "ROAD NAME";

                    aprpSummarySheet.Cells[3, 8].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[3, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[3, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[3, 8].Value = "KMS";

                    aprpSummarySheet.Cells[3, 9].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[3, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[3, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[3, 9].Value = " BUDGET FY " + printData.FinancialYear.Code;

                    aprpSummarySheet.Cells[4, 2].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[4, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[4, 2].Value = "NO";



                    //print the admin activities
                    var printGroupName = "";
                    var aprpCount = 0;
                    int aprpRowCount = 4;
                    var numberCount = 1;

                    aprpCount = 1;

                    foreach (var region in authorityRegions)
                    {
                        var regionConstituencies = region.RegionToCountys.Select(c => c.County.Constituencys);
                        List<long> constituencyIds = new List<long>();
                        List<Constituency> constituencies = new List<Constituency>();
                        List<RoadWorkSectionPlan> regionWorkplans = new List<RoadWorkSectionPlan>();
                        foreach (var constituency in regionConstituencies)
                        {
                            foreach (var consti in constituency)
                            {
                                constituencyIds.Add(consti.ID);
                                constituencies.Add(consti);
                            }
                        }

                        if (constituencyIds != null)
                        {
                            if (constituencyIds.Count > 0)
                            {
                                regionWorkplans = printData.RoadWorkSectionPlans.Where(s => constituencyIds.Any(c => c == (s.RoadSection.ConstituencyId != null ? (long)s.RoadSection.ConstituencyId : 0))).ToList();
                            }

                        }


                        aprpSummarySheet.Cells[aprpRowCount, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        aprpSummarySheet.Cells[aprpRowCount, 1].Value = "";


                        aprpSummarySheet.Cells[aprpRowCount, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        aprpSummarySheet.Cells[aprpRowCount, 2].Value = numberCount++;
                        aprpSummarySheet.Cells[aprpRowCount, 2].Style.Numberformat.Format = "0";

                        aprpSummarySheet.Cells[aprpRowCount, 3, aprpRowCount, 9].Style.Font.Bold = true;
                        aprpSummarySheet.Cells[aprpRowCount, 3, aprpRowCount, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 3, aprpRowCount, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        aprpSummarySheet.Cells[aprpRowCount, 3, aprpRowCount, 9].Merge = true;
                        aprpSummarySheet.Cells[aprpRowCount, 3, aprpRowCount, 9].Value = "REGION : " + region.Name.ToUpper();

                        aprpRowCount++;
                        //find the workcategories in the region
                        if (regionWorkplans != null)
                        {
                            if (regionWorkplans.Any())
                            {
                                foreach (var consty in constituencies)
                                {
                                    aprpSummarySheet.Cells[aprpRowCount, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    aprpSummarySheet.Cells[aprpRowCount, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    aprpSummarySheet.Cells[aprpRowCount, 2].Value = numberCount++;
                                    aprpSummarySheet.Cells[aprpRowCount, 2].Style.Numberformat.Format = "0";

                                    //aprpSummarySheet.Cells[aprpRowCount, 4].Style.Font.Bold = true;
                                    aprpSummarySheet.Cells[aprpRowCount, 3, aprpRowCount, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    aprpSummarySheet.Cells[aprpRowCount, 3, aprpRowCount, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    aprpSummarySheet.Cells[aprpRowCount, 3, aprpRowCount, 9].Value = consty.Name.ToUpper();
                                    aprpSummarySheet.Cells[aprpRowCount, 3, aprpRowCount, 9].Merge = true;

                                    aprpRowCount++; //go to the next row.

                                    foreach (var plan in regionWorkplans.Where(p => p.RoadSection.Constituency.ID == consty.ID).OrderBy(c => c.WorkCategory.Code))
                                    {
                                        //output summaries only on the specific cells
                                        //aprpSummarySheet.Cells[aprpRowCount, 4].Style.Font.Bold = true;

                                        aprpSummarySheet.Cells[aprpRowCount, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                        aprpSummarySheet.Cells[aprpRowCount, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        aprpSummarySheet.Cells[aprpRowCount, 4].Value = plan.Road.RoadNumber;

                                        aprpSummarySheet.Cells[aprpRowCount, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                        aprpSummarySheet.Cells[aprpRowCount, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        aprpSummarySheet.Cells[aprpRowCount, 5].Value = plan.WorkCategory.Name;

                                        aprpSummarySheet.Cells[aprpRowCount, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                        aprpSummarySheet.Cells[aprpRowCount, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        aprpSummarySheet.Cells[aprpRowCount, 6].Value = (plan.RoadSection.SurfaceType != null ? plan.RoadSection.SurfaceType.Code : "");

                                        aprpSummarySheet.Cells[aprpRowCount, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                        aprpSummarySheet.Cells[aprpRowCount, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        aprpSummarySheet.Cells[aprpRowCount, 7].Value = plan.RoadSection.SectionName;

                                        //aprpSummarySheet.Cells[aprpRowCount, 8].Style.Font.Bold = true;
                                        aprpSummarySheet.Cells[aprpRowCount, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                        aprpSummarySheet.Cells[aprpRowCount, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        aprpSummarySheet.Cells[aprpRowCount, 8].Value = plan.RoadSection.Length;
                                        aprpSummarySheet.Cells[aprpRowCount, 8].Style.Numberformat.Format = "0";

                                        //aprpSummarySheet.Cells[aprpRowCount, 9].Style.Font.Bold = true;
                                        aprpSummarySheet.Cells[aprpRowCount, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                        aprpSummarySheet.Cells[aprpRowCount, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        aprpSummarySheet.Cells[aprpRowCount, 9].Value = plan.TotalEstimateCost;
                                        aprpSummarySheet.Cells[aprpRowCount, 9].Style.Numberformat.Format = "#,##0.00";

                                        aprpRowCount++;
                                    }
                                    aprpSummarySheet.Cells[aprpRowCount, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    aprpSummarySheet.Cells[aprpRowCount, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                    aprpSummarySheet.Cells[aprpRowCount, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    aprpSummarySheet.Cells[aprpRowCount, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    aprpSummarySheet.Cells[aprpRowCount, 2].Value = numberCount++;
                                    aprpSummarySheet.Cells[aprpRowCount, 2].Style.Numberformat.Format = "0";

                                    aprpSummarySheet.Cells[aprpRowCount, 3, aprpRowCount, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    aprpSummarySheet.Cells[aprpRowCount, 3, aprpRowCount, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                    aprpSummarySheet.Cells[aprpRowCount, 7].Style.Font.Bold = true;
                                    aprpSummarySheet.Cells[aprpRowCount, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    aprpSummarySheet.Cells[aprpRowCount, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    aprpSummarySheet.Cells[aprpRowCount, 7].Value = "TOTAL FOR : " + consty.Name.ToUpper();

                                    aprpSummarySheet.Cells[aprpRowCount, 8].Style.Font.Bold = true;
                                    aprpSummarySheet.Cells[aprpRowCount, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    aprpSummarySheet.Cells[aprpRowCount, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    aprpSummarySheet.Cells[aprpRowCount, 8].Value = regionWorkplans.Where(p => p.RoadSection.Constituency.ID == consty.ID).Sum(c => c.PlannedLength);
                                    aprpSummarySheet.Cells[aprpRowCount, 8].Style.Numberformat.Format = "#,##0.00";

                                    aprpSummarySheet.Cells[aprpRowCount, 9].Style.Font.Bold = true;
                                    aprpSummarySheet.Cells[aprpRowCount, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    aprpSummarySheet.Cells[aprpRowCount, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    aprpSummarySheet.Cells[aprpRowCount, 9].Value = regionWorkplans.Where(p => p.RoadSection.Constituency.ID == consty.ID).Sum(c => c.TotalEstimateCost);
                                    aprpSummarySheet.Cells[aprpRowCount, 9].Style.Numberformat.Format = "#,##0.00";

                                    aprpRowCount++;
                                }


                            }
                        }


                    }

                    aprpSummarySheet.Cells[aprpRowCount, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[aprpRowCount, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    aprpSummarySheet.Cells[aprpRowCount, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[aprpRowCount, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[aprpRowCount, 2].Value = "";

                    aprpSummarySheet.Cells[aprpRowCount, 3, aprpRowCount, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[aprpRowCount, 3, aprpRowCount, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    aprpSummarySheet.Cells[aprpRowCount, 7].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[aprpRowCount, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[aprpRowCount, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[aprpRowCount, 7].Value = "GRAND TOTAL ";

                    aprpSummarySheet.Cells[aprpRowCount, 8].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[aprpRowCount, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[aprpRowCount, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[aprpRowCount, 8].Value = printData.RoadWorkSectionPlans.Sum(c => c.PlannedLength);
                    aprpSummarySheet.Cells[aprpRowCount, 8].Style.Numberformat.Format = "#,##0.00";

                    aprpSummarySheet.Cells[aprpRowCount, 9].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[aprpRowCount, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[aprpRowCount, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[aprpRowCount, 9].Value = printData.RoadWorkSectionPlans.Sum(c => c.TotalEstimateCost);
                    aprpSummarySheet.Cells[aprpRowCount, 9].Style.Numberformat.Format = "#,##0.00";


                    aprpSummarySheet.Cells.AutoFitColumns();

                    //END APRP SHEET

                    excel.Workbook.Properties.Title = "Attempts";
                    _cache.Set(handle, excel.GetAsByteArray(),
                                    new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));
                    /*  using (var stream = new MemoryStream())
                      {
                          excel.SaveAs(stream);
                          _cache.Set(handle, excel.GetAsByteArray(),
                                              new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));
                      }*/

                    return Json(new
                    {
                        Success = true,
                        Message = "Success",
                        Href = Url.Action("Download", "Workplan", new { fileGuid = handle, FileName = printData.Authority.Code + "_APRP_" + printData.FinancialYear.Code + ".xlsx" })
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"WorkplanController.GetAPRPForAgency API Error {ex.Message}");
                return Json(new
                {
                    Success = false,
                    Message = "Error occured"
                });
            }

        }


        public async Task<IActionResult> GetAPRPForKenha(long authorityId, long financialYearId, long regionId, long countyId)
        {
            try
            {
                //retrieve the records to be printed
                var printData = new AWRPViewModel();
                if (financialYearId > 0)
                {
                    //get the supplied financial year
                    var financialYearResp = await _financialYearService.FindByIdAsync((long)financialYearId).ConfigureAwait(false);
                    if (financialYearResp.Success)
                    {
                        printData.FinancialYear = financialYearResp.FinancialYear;
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "The financial year supplied could not be retrieved."
                        });
                    }
                }
                else
                {
                    //get the current financial year
                    var financialYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
                    if (financialYearResp.Success)
                        printData.FinancialYear = financialYearResp.FinancialYear;
                }




                //get the Authority to print for
                var authorityResp = await _authorityService.FindByIdAsync(authorityId != 0 ? authorityId : countyId).ConfigureAwait(false);
                if (!authorityResp.Success)
                {
                    return Json(new { Success = false, Message = "The authority supplied is incorrect, please choose the right authority." });
                }
                var authority = authorityResp.Authority;
                printData.Authority = authority;

                var ceilingResp = await _budgetCeilingService.FindApprovedByAuthorityForCurrentYear(printData.FinancialYear.ID, printData.Authority.ID).ConfigureAwait(false);
                printData.BudgetCeiling = ceilingResp.BudgetCeiling;

                //get the budget details for the authority
                var budgetHeaderResp = await _budgetCeilingHeaderService.FindCurrentAsync().ConfigureAwait(false);
                if (budgetHeaderResp.Success)
                    printData.BudgetCeilingHeader = budgetHeaderResp.BudgetCeilingHeader;

                //get the administrative activities for the authority
                var authorityAdminActivities = await _adminOperationalActivityService.ListByAuthorityAsync(printData.Authority.ID, printData.FinancialYear.ID).ConfigureAwait(false);
                printData.AdminOperationalActivities = authorityAdminActivities.ToList();
                //get the current workplans for the authority
                var authorityWorkplans = await _roadWorkSectionPlanService.ListByAgencyAsync(authority.ID, printData.FinancialYear.ID).ConfigureAwait(false);
                if (!authorityWorkplans.Any())
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "There are no records for the selected county in the selected financial year that could be retrieved."
                    });
                }
                printData.RoadWorkSectionPlans = authorityWorkplans.ToList();

                //generate the file.
                string handle = Guid.NewGuid().ToString();
                var memoryStream = new MemoryStream();
                using (ExcelPackage excel = new ExcelPackage(memoryStream))
                {
                    string currentFileName = System.IO.Path.GetFileName("Kenha_APRP");

                    var grandSummarySheet = excel.Workbook.Worksheets.Add("1. Grand Summary");
                    var regionalSumSummarySheet = excel.Workbook.Worksheets.Add("2.Regional Sum");
                    var workTypeSummarySheet = excel.Workbook.Worksheets.Add("3. Works Type Summary");
                    var worksByRegionsSummarySheet = excel.Workbook.Worksheets.Add("4. Works by Regions");
                    var aprpSummarySheet = excel.Workbook.Worksheets.Add("5. APRP format");

                    var balanceForRoadWorks = 0d;
                    var totalIncome = 0d;
                    var budgetCeiling = 0d;
                    var transitTolls = 0d;
                    var operationsCost = 0d;

                    //BEGIN GRAN SUMMARY SHEET
                    //print the column headers
                    //First Row
                    int count = 1;
                    grandSummarySheet.Cells["A1:C1"].Value = "KENYA NATIONAL HIGHWAYS AUTHORITY.  ANNUAL ROADS WORK PROGRAMME (ARWP) FOR FINANCIAL YEAR" + printData.FinancialYear.Code;
                    grandSummarySheet.Cells["A1:C1"].Merge = true;
                    grandSummarySheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells["A1:C1"].Style.Font.Bold = true;
                    grandSummarySheet.Cells["A1:C1"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Row(1).Height = 30;
                    grandSummarySheet.Cells["A1:C1"].Style.WrapText = true;

                    grandSummarySheet.Cells[2, 1].Style.Font.Bold = true;
                    grandSummarySheet.Cells[2, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[2, 1].Value = "NO.";

                    grandSummarySheet.Cells[2, 2].Style.Font.Bold = true;
                    grandSummarySheet.Cells[2, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[2, 2].Value = "INCOME / CEILINGS";

                    grandSummarySheet.Cells[2, 3].Style.Font.Bold = true;
                    grandSummarySheet.Cells[2, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[2, 3].Value = "BUDGET FY " + printData.FinancialYear.Code;

                    //RMLF
                    grandSummarySheet.Cells[3, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[3, 1].Value = count++;
                    grandSummarySheet.Cells[3, 1].Style.Numberformat.Format = "0";

                    grandSummarySheet.Cells[3, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[3, 2].Value = "40% RMLF";

                    grandSummarySheet.Cells[3, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[3, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[3, 3].Value = printData.BudgetCeiling.Amount;
                    grandSummarySheet.Cells[3, 3].Style.Numberformat.Format = "#,##0.00";

                    //TRANSIT TOLLS
                    grandSummarySheet.Cells[4, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[4, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[4, 1].Value = count++;
                    grandSummarySheet.Cells[4, 1].Style.Numberformat.Format = "0";

                    grandSummarySheet.Cells[4, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[4, 2].Value = "TRANSIT TOLLS";

                    grandSummarySheet.Cells[4, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[4, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                    var kenhaTolls = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("16", printData.FinancialYear.ID).ConfigureAwait(false);
                    if (kenhaTolls.Success)
                    {
                        var objectResult = (ObjectResult)kenhaTolls.IActionResult;
                        if (objectResult != null)
                        {
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var result = (OkObjectResult)objectResult;

                                grandSummarySheet.Cells[4, 3].Value = ((BudgetCeilingComputation)result.Value).Amount;
                                transitTolls = ((BudgetCeilingComputation)result.Value).Amount;
                            }
                        }
                    }

                    grandSummarySheet.Cells[4, 3].Style.Numberformat.Format = "#,##0.00";

                    //TOTAL INCOME
                    grandSummarySheet.Cells[5, 1].Style.Font.Bold = true;
                    grandSummarySheet.Cells[5, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[5, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[5, 1].Value = count++;
                    grandSummarySheet.Cells[5, 1].Style.Numberformat.Format = "0";

                    grandSummarySheet.Cells[5, 2].Style.Font.Bold = true;
                    grandSummarySheet.Cells[5, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[5, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[5, 2].Value = "TOTAL INCOME";

                    var kenhaTotalIncome = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("17", printData.FinancialYear.ID).ConfigureAwait(false);

                    if (kenhaTotalIncome.Success)
                    {
                        var objectResult = (ObjectResult)kenhaTotalIncome.IActionResult;
                        if (objectResult != null)
                        {
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var result = (OkObjectResult)objectResult;

                                grandSummarySheet.Cells[5, 3].Value = ((BudgetCeilingComputation)result.Value).Amount;
                                totalIncome = ((BudgetCeilingComputation)result.Value).Amount;
                            }
                        }
                    }
                    grandSummarySheet.Cells[5, 3].Style.Font.Bold = true;
                    grandSummarySheet.Cells[5, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[5, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[5, 3].Style.Numberformat.Format = "#,##0.00";

                    //OPERATIONS COSTS (4% OF KRB FUND)
                    grandSummarySheet.Cells[6, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[6, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[6, 1].Value = count++;
                    grandSummarySheet.Cells[6, 1].Style.Numberformat.Format = "0";

                    grandSummarySheet.Cells[6, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[6, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[6, 2].Value = "LESS OPERATIONS 4% OF KRB FUND";


                    var kenhaOperations = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("18", printData.FinancialYear.ID).ConfigureAwait(false);
                    if (kenhaOperations.Success)
                    {
                        var objectResult = (ObjectResult)kenhaOperations.IActionResult;
                        if (objectResult != null)
                        {
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var result = (OkObjectResult)objectResult;

                                grandSummarySheet.Cells[6, 3].Value = ((BudgetCeilingComputation)result.Value).Amount;
                                operationsCost = ((BudgetCeilingComputation)result.Value).Amount;
                            }
                        }
                    }


                    grandSummarySheet.Cells[6, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[6, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[6, 3].Style.Numberformat.Format = "#,##0.00";

                    //BALANCE FOR ROAD WORKS
                    grandSummarySheet.Cells[7, 1].Style.Font.Bold = true;
                    grandSummarySheet.Cells[7, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[7, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[7, 1].Value = count++;
                    grandSummarySheet.Cells[7, 1].Style.Numberformat.Format = "0";

                    grandSummarySheet.Cells[7, 2].Style.Font.Bold = true;
                    grandSummarySheet.Cells[7, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[7, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[7, 2].Value = "BALANCE FOR ROAD WORKS";


                    var kenhaRoadWorksBalance = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("19", printData.FinancialYear.ID).ConfigureAwait(false);
                    if (kenhaRoadWorksBalance.Success)
                    {
                        var objectResult = (ObjectResult)kenhaRoadWorksBalance.IActionResult;
                        if (objectResult != null)
                        {
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var result = (OkObjectResult)objectResult;

                                grandSummarySheet.Cells[7, 3].Value = ((BudgetCeilingComputation)result.Value).Amount;
                                balanceForRoadWorks = ((BudgetCeilingComputation)result.Value).Amount;
                            }
                        }
                    }
                    grandSummarySheet.Cells[7, 3].Style.Font.Bold = true;
                    grandSummarySheet.Cells[7, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[7, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[7, 3].Style.Numberformat.Format = "#,##0.00";

                    //DEVELOPMENT PORTION
                    grandSummarySheet.Cells[8, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[8, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[8, 1].Value = count++;

                    grandSummarySheet.Cells[8, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[8, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[8, 2].Value = "LESS DEVELOPMENT PORTION";

                    grandSummarySheet.Cells[8, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[8, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[8, 3].Value = 0;
                    grandSummarySheet.Cells[8, 3].Style.Numberformat.Format = "#,##0.00";

                    //ADD CARRY OVER BUDGET 
                    grandSummarySheet.Cells[9, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[9, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[9, 1].Value = count++;
                    grandSummarySheet.Cells[9, 1].Style.Numberformat.Format = "0";

                    grandSummarySheet.Cells[9, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[9, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[9, 2].Value = "ADD CARRY OVER BUDGET ";

                    grandSummarySheet.Cells[9, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[9, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[9, 3].Value = 0;
                    grandSummarySheet.Cells[9, 3].Style.Numberformat.Format = "#,##0.00";

                    //ADD CARRY OVER BUDGET 
                    grandSummarySheet.Cells[10, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[10, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[10, 1].Value = count++;
                    grandSummarySheet.Cells[10, 1].Style.Numberformat.Format = "0";

                    grandSummarySheet.Cells[10, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[10, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[10, 2].Value = "SPECIAL ALLOCATION  ";

                    grandSummarySheet.Cells[10, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[10, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[10, 3].Value = 0;
                    grandSummarySheet.Cells[10, 3].Style.Numberformat.Format = "#,##0.00";


                    //TOTAL BUDGET FOR ROADWORKS
                    grandSummarySheet.Cells[11, 1].Style.Font.Bold = true;
                    grandSummarySheet.Cells[11, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[11, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[11, 1].Value = "";

                    grandSummarySheet.Cells[11, 2].Style.Font.Bold = true;
                    grandSummarySheet.Cells[11, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[11, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[11, 2].Value = "TOTAL BUDGET FOR RA & CM WORKS";

                    grandSummarySheet.Cells[11, 3].Style.Font.Bold = true;
                    grandSummarySheet.Cells[11, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[11, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[11, 3].Value = balanceForRoadWorks; // less development and special costs
                    grandSummarySheet.Cells[11, 3].Style.Numberformat.Format = "#,##0.00";

                    //EXPENDITURES
                    grandSummarySheet.Cells[12, 1].Style.Font.Bold = true;
                    grandSummarySheet.Cells[12, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[12, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[12, 1].Value = "";

                    grandSummarySheet.Cells[12, 2].Style.Font.Bold = true;
                    grandSummarySheet.Cells[12, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[12, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[12, 2].Value = "EXPENDITURES";

                    grandSummarySheet.Cells[12, 3].Style.Font.Bold = true;
                    grandSummarySheet.Cells[12, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[12, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                    //HEADQUARTERS
                    grandSummarySheet.Cells[13, 1].Style.Font.Bold = true;
                    grandSummarySheet.Cells[13, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[13, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[13, 1].Value = "";

                    grandSummarySheet.Cells[13, 2].Style.Font.Bold = true;
                    grandSummarySheet.Cells[13, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[13, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[13, 2].Value = "HEADQUARTERS";

                    grandSummarySheet.Cells[13, 3].Style.Font.Bold = true;
                    grandSummarySheet.Cells[13, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[13, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    int row = 14;

                    //print the admin activities.
                    foreach (var activity in printData.AdminOperationalActivities.OrderBy(g => g.ActivityGroup))
                    {
                        grandSummarySheet.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        grandSummarySheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        grandSummarySheet.Cells[row, 1].Value = count++;

                        grandSummarySheet.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        grandSummarySheet.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        grandSummarySheet.Cells[row, 2].Value = (activity.ActivityWorks != null ? activity.ActivityWorks.ToUpper() : "");

                        grandSummarySheet.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        grandSummarySheet.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        grandSummarySheet.Cells[row, 3].Value = activity.Amount;
                        grandSummarySheet.Cells[row, 3].Style.Numberformat.Format = "#,##0.00";
                        row++;
                    }


                    //HEADQUARTERS TOTALS
                    grandSummarySheet.Cells[row, 1].Style.Font.Bold = true;
                    grandSummarySheet.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[row, 1].Value = "";

                    grandSummarySheet.Cells[row, 2].Style.Font.Bold = true;
                    grandSummarySheet.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[row, 2].Value = "TOTAL FOR HEADQUARTERS";

                    grandSummarySheet.Cells[row, 3].Style.Font.Bold = true;
                    grandSummarySheet.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[row, 3].Value = printData.AdminOperationalActivities.Sum(a => a.Amount);
                    grandSummarySheet.Cells[row, 3].Style.Numberformat.Format = "#,##0.00";

                    //Region
                    row++;
                    grandSummarySheet.Cells[row, 1].Style.Font.Bold = true;
                    grandSummarySheet.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[row, 1].Value = "";

                    grandSummarySheet.Cells[row, 2].Style.Font.Bold = true;
                    grandSummarySheet.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[row, 2].Value = "REGIONS/CORRIDORS";

                    grandSummarySheet.Cells[row, 3].Style.Font.Bold = true;
                    grandSummarySheet.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    row++;


                    //get authority regions
                    var authorityRegions = await _regionService.ListRegionsByAuthority(printData.Authority.ID).ConfigureAwait(false);
                    foreach (var region in authorityRegions)
                    {
                        var regionConstituencies = region.RegionToCountys.Select(c => c.County.Constituencys);
                        List<long> constituencyIds = new List<long>();
                        foreach (var constituency in regionConstituencies)
                        {
                            foreach (var consti in constituency)
                            {
                                constituencyIds.Add(consti.ID);
                            }
                        }

                        var regionWorkplans = printData.RoadWorkSectionPlans.Where(s => constituencyIds.Any(c => c == (long)s.RoadSection.ConstituencyId));


                        grandSummarySheet.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        grandSummarySheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        grandSummarySheet.Cells[row, 1].Value = count++;

                        grandSummarySheet.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        grandSummarySheet.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        grandSummarySheet.Cells[row, 2].Value = region.Name;

                        grandSummarySheet.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        grandSummarySheet.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        grandSummarySheet.Cells[row, 3].Value = regionWorkplans.Sum(s => s.TotalEstimateCost);//get summary of workplans in that region
                        grandSummarySheet.Cells[row, 3].Style.Numberformat.Format = "#,##0.00";
                        row++;
                    }

                    //HEADQUARTERS TOTALS
                    row++;
                    grandSummarySheet.Cells[row, 1].Style.Font.Bold = true;
                    grandSummarySheet.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[row, 1].Value = "";

                    grandSummarySheet.Cells[row, 2].Style.Font.Bold = true;
                    grandSummarySheet.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[row, 2].Value = "TOTAL PLANNED FOR REGIONS/CORRIDORS (" + printData.RoadWorkSectionPlans.Sum(s => s.PlannedLength) + " KMS)";
                    grandSummarySheet.Cells[row, 2].Style.Numberformat.Format = "#,##0.00";

                    grandSummarySheet.Cells[row, 3].Style.Font.Bold = true;
                    grandSummarySheet.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[row, 3].Value = printData.RoadWorkSectionPlans.Sum(c => c.TotalEstimateCost);
                    grandSummarySheet.Cells[row, 3].Style.Numberformat.Format = "#,##0.00";


                    //GRAND TOTALS
                    row++;
                    grandSummarySheet.Cells[row, 1].Style.Font.Bold = true;
                    grandSummarySheet.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[row, 1].Value = "";

                    grandSummarySheet.Cells[row, 2].Style.Font.Bold = true;
                    grandSummarySheet.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[row, 2].Value = "GRAND TOTAL";

                    grandSummarySheet.Cells[row, 3].Style.Font.Bold = true;
                    grandSummarySheet.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[row, 3].Value = (printData.RoadWorkSectionPlans.Sum(c => c.TotalEstimateCost) + printData.AdminOperationalActivities.Sum(c => c.Amount)); ;
                    grandSummarySheet.Cells[row, 3].Style.Numberformat.Format = "#,##0.00";

                    grandSummarySheet.Cells.AutoFitColumns();

                    //END GRAND SUMMARY SHEET

                    //BEGIN REGIONAL SUMM SHEET
                    regionalSumSummarySheet.Cells["A1:D1"].Value = "KENYA NATIONAL HIGHWAYS AUTHORITY ANNUAL ROADS WORK PROGRAMME (ARWP) FOR FINANCIAL YEAR " + printData.FinancialYear.Code + " REGIONAL SUMMARY";
                    regionalSumSummarySheet.Cells["A1:D1"].Merge = true;
                    regionalSumSummarySheet.Cells["A1:D1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    regionalSumSummarySheet.Cells["A1:D1"].Style.Font.Bold = true;
                    regionalSumSummarySheet.Cells["A1:D1"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    regionalSumSummarySheet.Row(1).Height = 30;
                    regionalSumSummarySheet.Cells["A1:D1"].Style.WrapText = true;


                    regionalSumSummarySheet.Cells[2, 1].Style.Font.Bold = true;
                    regionalSumSummarySheet.Cells[2, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    regionalSumSummarySheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    regionalSumSummarySheet.Cells[2, 1].Value = "NO.";

                    regionalSumSummarySheet.Cells[2, 2].Style.Font.Bold = true;
                    regionalSumSummarySheet.Cells[2, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    regionalSumSummarySheet.Cells[2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    regionalSumSummarySheet.Cells[2, 2].Value = "REGION";

                    regionalSumSummarySheet.Cells[2, 3].Style.Font.Bold = true;
                    regionalSumSummarySheet.Cells[2, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    regionalSumSummarySheet.Cells[2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    regionalSumSummarySheet.Cells[2, 3].Value = "KMS ";

                    regionalSumSummarySheet.Cells[2, 3].Style.Font.Bold = true;
                    regionalSumSummarySheet.Cells[2, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    regionalSumSummarySheet.Cells[2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    regionalSumSummarySheet.Cells[2, 3].Value = "BUDGET FY " + printData.FinancialYear.Code;



                    //print admin summaries
                    int rowSum = 3;
                    int countSum = 1;
                    //print the admin activities.
                    var groupedActivties = printData.AdminOperationalActivities.GroupBy(g => g.ActivityGroup).Select(
                            g => new
                            {
                                Key = g.Key,
                                Value = g.Sum(s => s.Amount),
                                Name = g.First().ActivityGroup
                            }
                        );
                    foreach (var activity in groupedActivties)
                    {

                        regionalSumSummarySheet.Cells[rowSum, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        regionalSumSummarySheet.Cells[rowSum, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        regionalSumSummarySheet.Cells[rowSum, 1].Value = countSum++;

                        regionalSumSummarySheet.Cells[rowSum, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        regionalSumSummarySheet.Cells[rowSum, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        regionalSumSummarySheet.Cells[rowSum, 2].Value = activity.Name;

                        regionalSumSummarySheet.Cells[rowSum, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        regionalSumSummarySheet.Cells[rowSum, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        regionalSumSummarySheet.Cells[rowSum, 3].Value = "-";


                        regionalSumSummarySheet.Cells[rowSum, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        regionalSumSummarySheet.Cells[rowSum, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        regionalSumSummarySheet.Cells[rowSum, 4].Value = activity.Value;
                        regionalSumSummarySheet.Cells[rowSum, 4].Style.Numberformat.Format = "#,##0.00";
                        rowSum++;
                    }

                    //get authority regions for summary
                    foreach (var region in authorityRegions)
                    {
                        var regionConstituencies = region.RegionToCountys.Select(c => c.County.Constituencys);
                        List<long> constituencyIds = new List<long>();
                        foreach (var constituency in regionConstituencies)
                        {
                            foreach (var consti in constituency)
                            {
                                constituencyIds.Add(consti.ID);
                            }
                        }

                        var regionWorkplans = printData.RoadWorkSectionPlans.Where(s => constituencyIds.Any(c => c == (long)s.RoadSection.ConstituencyId));


                        regionalSumSummarySheet.Cells[rowSum, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        regionalSumSummarySheet.Cells[rowSum, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        regionalSumSummarySheet.Cells[rowSum, 1].Value = countSum++;

                        regionalSumSummarySheet.Cells[rowSum, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        regionalSumSummarySheet.Cells[rowSum, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        regionalSumSummarySheet.Cells[rowSum, 2].Value = region.Name;

                        regionalSumSummarySheet.Cells[rowSum, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        regionalSumSummarySheet.Cells[rowSum, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        regionalSumSummarySheet.Cells[rowSum, 3].Value = regionWorkplans.Sum(s => s.PlannedLength);//get summary of workplans in that region
                        regionalSumSummarySheet.Cells[rowSum, 3].Style.Numberformat.Format = "#,##0.00";

                        regionalSumSummarySheet.Cells[rowSum, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        regionalSumSummarySheet.Cells[rowSum, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        regionalSumSummarySheet.Cells[rowSum, 4].Value = regionWorkplans.Sum(s => s.TotalEstimateCost);//get summary of workplans in that region
                        regionalSumSummarySheet.Cells[rowSum, 4].Style.Numberformat.Format = "#,##0.00";
                        rowSum++;
                    }


                    //grand total
                    regionalSumSummarySheet.Cells[rowSum, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    regionalSumSummarySheet.Cells[rowSum, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    regionalSumSummarySheet.Cells[rowSum, 1].Style.Font.Bold = true;
                    regionalSumSummarySheet.Cells[rowSum, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    regionalSumSummarySheet.Cells[rowSum, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    regionalSumSummarySheet.Cells[rowSum, 2].Value = "GRAND TOTAL";

                    regionalSumSummarySheet.Cells[rowSum, 3].Style.Font.Bold = true;
                    regionalSumSummarySheet.Cells[rowSum, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    regionalSumSummarySheet.Cells[rowSum, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    regionalSumSummarySheet.Cells[rowSum, 3].Value = printData.RoadWorkSectionPlans.Sum(s => s.PlannedLength);//get summary of workplans in that region
                    regionalSumSummarySheet.Cells[rowSum, 3].Style.Numberformat.Format = "#,##0.00";

                    regionalSumSummarySheet.Cells[rowSum, 4].Style.Font.Bold = true;
                    regionalSumSummarySheet.Cells[rowSum, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    regionalSumSummarySheet.Cells[rowSum, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    regionalSumSummarySheet.Cells[rowSum, 4].Value = (printData.AdminOperationalActivities.Sum(a => a.Amount) + printData.RoadWorkSectionPlans.Sum(p => p.TotalEstimateCost));//get summary of workplans in that region
                    regionalSumSummarySheet.Cells[rowSum, 4].Style.Numberformat.Format = "#,##0.00";


                    regionalSumSummarySheet.Cells.AutoFitColumns();

                    //END REGIONAL SUMM SHEET

                    //WORKS CATEGORY SUMMARY
                    int rowCatSum = 3;
                    int countCat = 1;
                    var worksCategorySummary = printData.RoadWorkSectionPlans.GroupBy(p => p.WorkCategory).Select(
                            cat => new
                            {
                                Key = cat.Key,
                                Value = cat.First().WorkCategory.Name,
                                Code = cat.First().WorkCategory.Code,
                                KMs = cat.Sum(c => c.RoadSection.Length),
                                Budget = cat.Sum(p => p.TotalEstimateCost)
                            }
                        );

                    //print admin activities
                    workTypeSummarySheet.Cells["A1:D1"].Value = "KENYA NATIONAL HIGHWAYS AUTHORITY ANNUAL ROADS WORK PROGRAMME (ARWP) FOR FINANCIAL YEAR " + printData.FinancialYear.Code + " SUMMARY OF WORKS CATEGORY";
                    workTypeSummarySheet.Cells["A1:D1"].Merge = true;
                    workTypeSummarySheet.Cells["A1:D1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workTypeSummarySheet.Cells["A1:D1"].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells["A1:D1"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Row(1).Height = 30;
                    workTypeSummarySheet.Cells["A1:D1"].Style.WrapText = true;


                    workTypeSummarySheet.Cells[2, 1].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[2, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workTypeSummarySheet.Cells[2, 1].Value = "NO.";

                    workTypeSummarySheet.Cells[2, 2].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[2, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workTypeSummarySheet.Cells[2, 2].Value = "WORKS CATEGORY";

                    workTypeSummarySheet.Cells[2, 3].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[2, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workTypeSummarySheet.Cells[2, 3].Value = "KMS ";

                    workTypeSummarySheet.Cells[2, 4].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[2, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workTypeSummarySheet.Cells[2, 4].Value = "BUDGET FY " + printData.FinancialYear.Code;


                    foreach (var activity in printData.AdminOperationalActivities)
                    {
                        workTypeSummarySheet.Cells[rowCatSum, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        workTypeSummarySheet.Cells[rowCatSum, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workTypeSummarySheet.Cells[rowCatSum, 1].Value = countCat++;

                        workTypeSummarySheet.Cells[rowCatSum, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        workTypeSummarySheet.Cells[rowCatSum, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workTypeSummarySheet.Cells[rowCatSum, 2].Value = (activity.ActivityWorks != null ? activity.ActivityWorks.ToUpper() : "");

                        workTypeSummarySheet.Cells[rowCatSum, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        workTypeSummarySheet.Cells[rowCatSum, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workTypeSummarySheet.Cells[rowCatSum, 3].Value = "-";//get summary of workplans in that region

                        workTypeSummarySheet.Cells[rowCatSum, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        workTypeSummarySheet.Cells[rowCatSum, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workTypeSummarySheet.Cells[rowCatSum, 4].Value = activity.Amount;//get summary of workplans in that region
                        workTypeSummarySheet.Cells[rowCatSum, 4].Style.Numberformat.Format = "#,##0.00";
                        rowCatSum++;
                    }

                    //print works category summary

                    foreach (var category in worksCategorySummary)
                    {
                        workTypeSummarySheet.Cells[rowCatSum, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        workTypeSummarySheet.Cells[rowCatSum, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workTypeSummarySheet.Cells[rowCatSum, 1].Value = countCat++;

                        workTypeSummarySheet.Cells[rowCatSum, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        workTypeSummarySheet.Cells[rowCatSum, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workTypeSummarySheet.Cells[rowCatSum, 2].Value = category.Value.ToUpper();

                        workTypeSummarySheet.Cells[rowCatSum, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        workTypeSummarySheet.Cells[rowCatSum, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workTypeSummarySheet.Cells[rowCatSum, 3].Value = category.KMs;//get summary of workplans in that region
                        workTypeSummarySheet.Cells[rowCatSum, 3].Style.Numberformat.Format = "#,##0.00";

                        workTypeSummarySheet.Cells[rowCatSum, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        workTypeSummarySheet.Cells[rowCatSum, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workTypeSummarySheet.Cells[rowCatSum, 4].Value = category.Budget;//get summary of workplans in that region
                        workTypeSummarySheet.Cells[rowCatSum, 4].Style.Numberformat.Format = "#,##0.00";
                        rowCatSum++;
                    }

                    //grand total
                    workTypeSummarySheet.Cells[rowCatSum, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[rowCatSum, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    workTypeSummarySheet.Cells[rowCatSum, 2].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[rowCatSum, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[rowCatSum, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workTypeSummarySheet.Cells[rowCatSum, 2].Value = "GRAND TOTAL";

                    workTypeSummarySheet.Cells[rowCatSum, 3].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[rowCatSum, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[rowCatSum, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workTypeSummarySheet.Cells[rowCatSum, 3].Value = printData.RoadWorkSectionPlans.Sum(s => s.PlannedLength);//get summary of workplans in that region
                    workTypeSummarySheet.Cells[rowCatSum, 3].Style.Numberformat.Format = "#,##0.00";

                    workTypeSummarySheet.Cells[rowCatSum, 4].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[rowCatSum, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[rowCatSum, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workTypeSummarySheet.Cells[rowCatSum, 4].Value = (printData.AdminOperationalActivities.Sum(a => a.Amount) + printData.RoadWorkSectionPlans.Sum(p => p.TotalEstimateCost));//get summary of workplans in that region
                    workTypeSummarySheet.Cells[rowCatSum, 4].Style.Numberformat.Format = "#,##0.00";

                    workTypeSummarySheet.Cells.AutoFitColumns();


                    //END WORKS CATEGORY SUMMARY

                    //BEGIN WORKS BY REGIONS
                    int categoryCount = worksCategorySummary.Count();
                    var columnCount = (categoryCount * 2) + printData.AdminOperationalActivities.Count; // times *2 because each category spans two columns

                    //print admin activities
                    worksByRegionsSummarySheet.Cells[1, 1, 1, (columnCount + 2)].Value = "KENYA NATIONAL HIGHWAYS AUTHORITY ANNUAL ROADS WORK PROGRAMME (ARWP) FOR FINANCIAL YEAR " + printData.FinancialYear.Code + " REGIONAL WORKS CATEGORY";

                    //worksheet.Cells["A1:B5"].Merge = true; =   workTypeSummarySheet.Cells[1, 1, 5, 2]

                    worksByRegionsSummarySheet.Cells[1, 1, 1, (columnCount + 2)].Merge = true;
                    worksByRegionsSummarySheet.Cells[1, 1, 1, (columnCount + 2)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksByRegionsSummarySheet.Cells[1, 1, 1, (columnCount + 2)].Style.Font.Bold = true;
                    worksByRegionsSummarySheet.Cells[1, 1, 1, (columnCount + 2)].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksByRegionsSummarySheet.Row(1).Height = 30;
                    worksByRegionsSummarySheet.Cells[1, 1, 1, (columnCount + 2)].Style.WrapText = true;

                    //set the No column and region
                    worksByRegionsSummarySheet.Cells[3, 1].Style.Font.Bold = true;
                    worksByRegionsSummarySheet.Cells[3, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksByRegionsSummarySheet.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksByRegionsSummarySheet.Cells[3, 1].Value = "NO.";

                    worksByRegionsSummarySheet.Cells[3, 2].Style.Font.Bold = true;
                    worksByRegionsSummarySheet.Cells[3, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksByRegionsSummarySheet.Cells[3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksByRegionsSummarySheet.Cells[3, 2].Value = "REGION";

                    //output the report headers
                    int cellId = 3;
                    //print the admin activities groups.
                    int rowCountSum = 4;
                    foreach (var activityGroup in groupedActivties)
                    {

                        worksByRegionsSummarySheet.Cells[rowCountSum, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksByRegionsSummarySheet.Cells[rowCountSum, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksByRegionsSummarySheet.Cells[rowCountSum, 1].Value = (rowCountSum - 3);

                        worksByRegionsSummarySheet.Cells[rowCountSum, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksByRegionsSummarySheet.Cells[rowCountSum, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksByRegionsSummarySheet.Cells[rowCountSum, 2].Value = (activityGroup.Name != null ? activityGroup.Name.ToUpper() : "");



                        foreach (var activity in printData.AdminOperationalActivities.Where(a => a.ActivityGroup == activityGroup.Name))
                        {
                            worksByRegionsSummarySheet.Cells[2, cellId].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            worksByRegionsSummarySheet.Cells[2, cellId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksByRegionsSummarySheet.Cells[2, cellId].Value = (activity.ActivityWorks != null ? activity.ActivityWorks.ToUpper() : "");
                            worksByRegionsSummarySheet.Cells[2, cellId].Style.Font.Bold = true;


                            worksByRegionsSummarySheet.Cells[3, cellId].Style.Font.Bold = true;
                            worksByRegionsSummarySheet.Cells[3, cellId].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            worksByRegionsSummarySheet.Cells[3, cellId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksByRegionsSummarySheet.Cells[3, cellId].Value = " BUDGET FY " + printData.FinancialYear.Code;

                            //output the values in the group row  for each

                            worksByRegionsSummarySheet.Cells[rowCountSum, cellId].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            worksByRegionsSummarySheet.Cells[rowCountSum, cellId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksByRegionsSummarySheet.Cells[rowCountSum, cellId].Value = activity.Amount;
                            worksByRegionsSummarySheet.Cells[rowCountSum, cellId].Style.Numberformat.Format = "#,##0.00";

                            cellId++;
                        }

                        rowCountSum++;

                    }


                    //work category summary
                    var recountCount = rowCountSum;

                    foreach (var cat in worksCategorySummary)
                    {
                        worksByRegionsSummarySheet.Cells[2, cellId].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksByRegionsSummarySheet.Cells[2, cellId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksByRegionsSummarySheet.Cells[2, cellId].Value = cat.Value.ToUpper();
                        worksByRegionsSummarySheet.Cells[2, cellId].Style.Font.Bold = true;
                        worksByRegionsSummarySheet.Cells[2, cellId, 2, (cellId + 1)].Merge = true;


                        worksByRegionsSummarySheet.Cells[3, cellId].Style.Font.Bold = true;
                        worksByRegionsSummarySheet.Cells[3, cellId].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksByRegionsSummarySheet.Cells[3, cellId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksByRegionsSummarySheet.Cells[3, cellId].Value = " KMS ";

                        worksByRegionsSummarySheet.Cells[3, (cellId + 1)].Style.Font.Bold = true;
                        worksByRegionsSummarySheet.Cells[3, (cellId + 1)].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksByRegionsSummarySheet.Cells[3, (cellId + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksByRegionsSummarySheet.Cells[3, (cellId + 1)].Value = " BUDGET FY " + printData.FinancialYear.Code;


                        cellId += 2;

                    }

                    int cellIdRegion = printData.AdminOperationalActivities.Count;
                    cellIdRegion += 3;
                    var loopCellId = cellIdRegion;
                    var countEntryRows = (authorityRegions.Count() + groupedActivties.Count());
                    var countEntryColumns = ((2 * worksCategorySummary.Count()) + printData.AdminOperationalActivities.Count());

                    foreach (var region in authorityRegions)
                    {

                        var regionConstituencies = region.RegionToCountys.Select(c => c.County.Constituencys);
                        List<long> constituencyIds = new List<long>();
                        foreach (var constituency in regionConstituencies)
                        {
                            foreach (var consti in constituency)
                            {
                                constituencyIds.Add(consti.ID);
                            }
                        }

                        var regionWorkplans = printData.RoadWorkSectionPlans.Where(s => constituencyIds.Any(c => c == (long)s.RoadSection.ConstituencyId));


                        worksByRegionsSummarySheet.Cells[rowCountSum, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksByRegionsSummarySheet.Cells[rowCountSum, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksByRegionsSummarySheet.Cells[rowCountSum, 1].Value = (rowCountSum - 3);

                        worksByRegionsSummarySheet.Cells[rowCountSum, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksByRegionsSummarySheet.Cells[rowCountSum, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksByRegionsSummarySheet.Cells[rowCountSum, 2].Value = region.Name;


                        foreach (var cat in worksCategorySummary)
                        {
                            //output summaries only on the specific cells
                            worksByRegionsSummarySheet.Cells[rowCountSum, cellIdRegion].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            worksByRegionsSummarySheet.Cells[rowCountSum, cellIdRegion].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksByRegionsSummarySheet.Cells[rowCountSum, cellIdRegion].Value = (double)regionWorkplans.Where(c => c.WorkCategory.Code == cat.Code).Sum(c => c.PlannedLength);
                            worksByRegionsSummarySheet.Cells[rowCountSum, cellIdRegion].Style.Numberformat.Format = "#,##0.00";

                            worksByRegionsSummarySheet.Cells[rowCountSum, (cellIdRegion + 1)].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            worksByRegionsSummarySheet.Cells[rowCountSum, (cellIdRegion + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksByRegionsSummarySheet.Cells[rowCountSum, (cellIdRegion + 1)].Value = (double)regionWorkplans.Where(c => c.WorkCategory.Code == cat.Code).Sum(c => c.TotalEstimateCost);
                            worksByRegionsSummarySheet.Cells[rowCountSum, (cellIdRegion + 1)].Style.Numberformat.Format = "#,##0.00";



                            cellIdRegion += 2;
                        }

                        //reset recount to old stage for the next region
                        //reset the cellId
                        cellIdRegion = loopCellId;

                        rowCountSum++;
                    }

                    //output the total summaries
                    worksByRegionsSummarySheet.Cells[rowCountSum, 1].Style.Font.Bold = true;
                    worksByRegionsSummarySheet.Cells[rowCountSum, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksByRegionsSummarySheet.Cells[rowCountSum, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksByRegionsSummarySheet.Cells[rowCountSum, 1].Value = (rowCountSum - 3);

                    worksByRegionsSummarySheet.Cells[rowCountSum, 2].Style.Font.Bold = true;
                    worksByRegionsSummarySheet.Cells[rowCountSum, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksByRegionsSummarySheet.Cells[rowCountSum, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksByRegionsSummarySheet.Cells[rowCountSum, 2].Value = "GRAND TOTAL";



                    //generate sum entry

                    for (int columnIndex = 3; columnIndex <= (countEntryColumns + 2); columnIndex++)
                    {
                        string totalMonthAddress = ExcelCellBase.GetAddress(
                            4, columnIndex,
                            (countEntryRows + 3), columnIndex
                        );
                        worksByRegionsSummarySheet.Cells[rowCountSum, columnIndex].Style.Font.Bold = true;
                        worksByRegionsSummarySheet.Cells[rowCountSum, columnIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksByRegionsSummarySheet.Cells[rowCountSum, columnIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksByRegionsSummarySheet.Cells[rowCountSum, columnIndex].Formula = string.Format("SUM({0})", totalMonthAddress);
                        worksByRegionsSummarySheet.Cells[rowCountSum, columnIndex].Style.Numberformat.Format = "#,##0.00";

                    }

                    //set borders entire page
                    for (int i = 4; i <= countEntryRows; i++)
                    {
                        //loop through the cells
                        for (int j = 3; j <= countEntryColumns; j++)
                        {
                            worksByRegionsSummarySheet.Cells[i, j].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        }
                    }


                    worksByRegionsSummarySheet.Cells.AutoFitColumns();


                    //END WORKS BY REGION

                    //BEGIN APRP SHEET


                    //print admin activities
                    aprpSummarySheet.Cells[1, 1, 1, (columnCount + 1)].Value = "KENYA NATIONAL HIGHWAYS AUTHORITY ANNUAL ROADS WORK PROGRAMME (ARWP) FOR FINANCIAL YEAR " + printData.FinancialYear.Code + ". DETAILED WORK PLAN";

                    //worksheet.Cells["A1:B5"].Merge = true; =   workTypeSummarySheet.Cells[1, 1, 5, 2]

                    aprpSummarySheet.Cells[1, 1, 1, (columnCount + 1)].Merge = true;
                    aprpSummarySheet.Cells[1, 1, 1, (columnCount + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[1, 1, 1, (columnCount + 1)].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[1, 1, 1, (columnCount + 1)].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Row(1).Height = 30;
                    aprpSummarySheet.Cells[1, 1, 1, (columnCount + 1)].Style.WrapText = true;

                    //set the No column and region
                    aprpSummarySheet.Cells[2, 1].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[2, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[2, 1].Value = "";

                    aprpSummarySheet.Cells[2, 2].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[2, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[2, 2].Value = "REGION";

                    aprpSummarySheet.Cells[2, 3].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[2, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[2, 3].Value = "NO.";

                    aprpSummarySheet.Cells[2, 4].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[2, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[2, 4].Value = "ACTIVITY /WORKS CATEGORY";

                    aprpSummarySheet.Cells[2, 5].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[2, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[2, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[2, 5].Value = "ROAD ID";

                    aprpSummarySheet.Cells[2, 6].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[2, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[2, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[2, 6].Value = "ROAD SECTION";

                    aprpSummarySheet.Cells[2, 7].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[2, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[2, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[2, 7].Value = "KMS";

                    aprpSummarySheet.Cells[2, 8].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[2, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[2, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[2, 8].Value = " BUDGET FY " + printData.FinancialYear.Code;



                    //print the admin activities
                    var printGroupName = "";
                    var aprpCount = 0;
                    int aprpRowCount = 3;
                    var numberCount = 1;

                    foreach (var group in groupedActivties)
                    {
                        aprpCount++;
                        aprpSummarySheet.Cells[aprpRowCount, 1].Style.Font.Bold = true;
                        aprpSummarySheet.Cells[aprpRowCount, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        aprpSummarySheet.Cells[aprpRowCount, 1].Value = aprpCount;

                        aprpSummarySheet.Cells[aprpRowCount, 2].Style.Font.Bold = true;
                        aprpSummarySheet.Cells[aprpRowCount, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        aprpSummarySheet.Cells[aprpRowCount, 2].Value = group.Name;

                        foreach (var activity in printData.AdminOperationalActivities.Where(g => g.ActivityGroup == group.Name))
                        {

                            // aprpSummarySheet.Cells[aprpRowCount, 3].Style.Font.Bold = true;
                            aprpSummarySheet.Cells[aprpRowCount, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            aprpSummarySheet.Cells[aprpRowCount, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            aprpSummarySheet.Cells[aprpRowCount, 3].Value = numberCount;

                            //aprpSummarySheet.Cells[aprpRowCount, 4].Style.Font.Bold = true;
                            aprpSummarySheet.Cells[aprpRowCount, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            aprpSummarySheet.Cells[aprpRowCount, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            aprpSummarySheet.Cells[aprpRowCount, 4].Value = (activity.ActivityWorks != null ? activity.ActivityWorks : "");

                            //aprpSummarySheet.Cells[aprpRowCount, 5].Style.Font.Bold = true;
                            aprpSummarySheet.Cells[aprpRowCount, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            aprpSummarySheet.Cells[aprpRowCount, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            aprpSummarySheet.Cells[aprpRowCount, 5].Value = activity.RoadID;

                            //aprpSummarySheet.Cells[aprpRowCount, 6].Style.Font.Bold = true;
                            aprpSummarySheet.Cells[aprpRowCount, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            aprpSummarySheet.Cells[aprpRowCount, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            aprpSummarySheet.Cells[aprpRowCount, 6].Value = activity.RoadSection;

                            // aprpSummarySheet.Cells[aprpRowCount, 7].Style.Font.Bold = true;
                            aprpSummarySheet.Cells[aprpRowCount, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            aprpSummarySheet.Cells[aprpRowCount, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            aprpSummarySheet.Cells[aprpRowCount, 7].Value = activity.KM;

                            //aprpSummarySheet.Cells[aprpRowCount, 8].Style.Font.Bold = true;
                            aprpSummarySheet.Cells[aprpRowCount, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            aprpSummarySheet.Cells[aprpRowCount, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            aprpSummarySheet.Cells[aprpRowCount, 8].Value = activity.Amount;
                            aprpSummarySheet.Cells[aprpRowCount, 8].Style.Numberformat.Format = "#,##0.00";

                            aprpRowCount++;
                            numberCount++;

                            //print totals if end of activity list

                        }
                        aprpSummarySheet.Cells[aprpRowCount, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        aprpSummarySheet.Cells[aprpRowCount, 2].Style.Font.Bold = true;
                        aprpSummarySheet.Cells[aprpRowCount, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        aprpSummarySheet.Cells[aprpRowCount, 2].Value = group.Name.ToUpper() + " TOTAL";

                        aprpSummarySheet.Cells[aprpRowCount, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        aprpSummarySheet.Cells[aprpRowCount, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        aprpSummarySheet.Cells[aprpRowCount, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        aprpSummarySheet.Cells[aprpRowCount, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        aprpSummarySheet.Cells[aprpRowCount, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        aprpSummarySheet.Cells[aprpRowCount, 7].Value = "";

                        aprpSummarySheet.Cells[aprpRowCount, 8].Style.Font.Bold = true;
                        aprpSummarySheet.Cells[aprpRowCount, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        aprpSummarySheet.Cells[aprpRowCount, 8].Value = group.Value;
                        aprpSummarySheet.Cells[aprpRowCount, 8].Style.Numberformat.Format = "#,##0.00";

                        aprpRowCount += 2;
                    }


                    aprpCount = 1;

                    foreach (var region in authorityRegions)
                    {
                        var regionConstituencies = region.RegionToCountys.Select(c => c.County.Constituencys);
                        List<long> constituencyIds = new List<long>();
                        List<RoadWorkSectionPlan> regionWorkplans = new List<RoadWorkSectionPlan>();
                        foreach (var constituency in regionConstituencies)
                        {
                            foreach (var consti in constituency)
                            {
                                constituencyIds.Add(consti.ID);
                            }
                        }

                        if (constituencyIds != null)
                        {
                            if (constituencyIds.Count > 0)
                            {
                                regionWorkplans = printData.RoadWorkSectionPlans.Where(s => constituencyIds.Any(c => c == (s.RoadSection.ConstituencyId != null ? (long)s.RoadSection.ConstituencyId : 0))).ToList();
                            }

                        }


                        aprpSummarySheet.Cells[aprpRowCount, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        aprpSummarySheet.Cells[aprpRowCount, 1].Value = aprpCount++;

                        aprpSummarySheet.Cells[aprpRowCount, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        aprpSummarySheet.Cells[aprpRowCount, 2].Value = region.Name.ToUpper();


                        //aprpRowCount++;
                        //find the workcategories in the region
                        if (regionWorkplans != null)
                        {
                            if (regionWorkplans.Any())
                            {
                                foreach (var plan in regionWorkplans.OrderBy(c => c.WorkCategory.Code))
                                {
                                    aprpSummarySheet.Cells[aprpRowCount, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    aprpSummarySheet.Cells[aprpRowCount, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    aprpSummarySheet.Cells[aprpRowCount, 3].Value = numberCount;
                                    aprpSummarySheet.Cells[aprpRowCount, 3].Style.Numberformat.Format = "0";

                                    //output summaries only on the specific cells
                                    //aprpSummarySheet.Cells[aprpRowCount, 4].Style.Font.Bold = true;
                                    aprpSummarySheet.Cells[aprpRowCount, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    aprpSummarySheet.Cells[aprpRowCount, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    aprpSummarySheet.Cells[aprpRowCount, 4].Value = plan.WorkCategory.Name;

                                    aprpSummarySheet.Cells[aprpRowCount, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    aprpSummarySheet.Cells[aprpRowCount, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    aprpSummarySheet.Cells[aprpRowCount, 5].Value = plan.Road.RoadNumber;


                                    aprpSummarySheet.Cells[aprpRowCount, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    aprpSummarySheet.Cells[aprpRowCount, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    aprpSummarySheet.Cells[aprpRowCount, 6].Value = plan.RoadSection.SectionName;

                                    aprpSummarySheet.Cells[aprpRowCount, 7].Style.Font.Bold = true;
                                    aprpSummarySheet.Cells[aprpRowCount, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    aprpSummarySheet.Cells[aprpRowCount, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    aprpSummarySheet.Cells[aprpRowCount, 7].Value = plan.PlannedLength;
                                    aprpSummarySheet.Cells[aprpRowCount, 7].Style.Numberformat.Format = "#,##0.00";

                                    aprpSummarySheet.Cells[aprpRowCount, 8].Style.Font.Bold = true;
                                    aprpSummarySheet.Cells[aprpRowCount, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    aprpSummarySheet.Cells[aprpRowCount, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    aprpSummarySheet.Cells[aprpRowCount, 8].Value = plan.TotalEstimateCost;
                                    aprpSummarySheet.Cells[aprpRowCount, 8].Style.Numberformat.Format = "#,##0.00";

                                    aprpRowCount++;
                                    numberCount++;
                                }
                            }
                        }


                        aprpSummarySheet.Cells[aprpRowCount, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        aprpSummarySheet.Cells[aprpRowCount, 2].Style.Font.Bold = true;
                        aprpSummarySheet.Cells[aprpRowCount, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        aprpSummarySheet.Cells[aprpRowCount, 2].Value = region.Name.ToUpper() + " TOTAL";

                        aprpSummarySheet.Cells[aprpRowCount, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        aprpSummarySheet.Cells[aprpRowCount, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        aprpSummarySheet.Cells[aprpRowCount, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        aprpSummarySheet.Cells[aprpRowCount, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        aprpSummarySheet.Cells[aprpRowCount, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        aprpSummarySheet.Cells[aprpRowCount, 7].Value = regionWorkplans.Sum(c => c.PlannedLength);
                        aprpSummarySheet.Cells[aprpRowCount, 7].Style.Numberformat.Format = "#,##0.00";

                        aprpSummarySheet.Cells[aprpRowCount, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        aprpSummarySheet.Cells[aprpRowCount, 8].Value = regionWorkplans.Sum(c => c.TotalEstimateCost);
                        aprpSummarySheet.Cells[aprpRowCount, 8].Style.Numberformat.Format = "#,##0.00";

                        aprpRowCount += 2;

                        //reset the values per region
                        regionConstituencies = null;
                        constituencyIds = null;
                        regionWorkplans = null;
                    }

                    aprpSummarySheet.Cells[aprpRowCount, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[aprpRowCount, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    aprpSummarySheet.Cells[aprpRowCount, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[aprpRowCount, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[aprpRowCount, 2].Value = "GRAND TOTAL ";

                    aprpSummarySheet.Cells[aprpRowCount, 3, aprpRowCount, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[aprpRowCount, 3, aprpRowCount, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    aprpSummarySheet.Cells[aprpRowCount, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[aprpRowCount, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[aprpRowCount, 7].Value = printData.RoadWorkSectionPlans.Sum(c => c.PlannedLength);
                    aprpSummarySheet.Cells[aprpRowCount, 7].Style.Numberformat.Format = "#,##0.00";

                    aprpSummarySheet.Cells[aprpRowCount, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[aprpRowCount, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[aprpRowCount, 8].Value = printData.RoadWorkSectionPlans.Sum(c => c.TotalEstimateCost);
                    aprpSummarySheet.Cells[aprpRowCount, 8].Style.Numberformat.Format = "#,##0.00";

                    aprpSummarySheet.Cells.AutoFitColumns();

                    //END APRP SHEET

                    excel.Workbook.Properties.Title = "Attempts";
                    _cache.Set(handle, excel.GetAsByteArray(),
                                    new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));
                    /*  using (var stream = new MemoryStream())
                      {
                          excel.SaveAs(stream);
                          _cache.Set(handle, excel.GetAsByteArray(),
                                              new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));
                      }*/

                    return Json(new
                    {
                        Success = true,
                        Message = "Success",
                        Href = Url.Action("Download", "Workplan", new { fileGuid = handle, FileName = printData.Authority.Code + "_APRP_" + printData.FinancialYear.Code + ".xlsx" })
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"WorkplanController.GetAPRPForAgency API Error {ex.Message}");
                return Json(new
                {
                    Success = false,
                    Message = "Error occured"
                });
            }

        }

        public async Task<IActionResult> DownloadAPRPImplementationReport(long authorityId, long financialYearId)
        {
            {
                try
                {
                    //retrieve the records to be printed
                    var printData = new AWRPViewModel();
                    if (financialYearId > 0)
                    {
                        //get the supplied financial year
                        var financialYearResp = await _financialYearService.FindByIdAsync((long)financialYearId).ConfigureAwait(false);
                        if (financialYearResp.Success)
                        {
                            printData.FinancialYear = financialYearResp.FinancialYear;
                        }
                        else
                        {
                            return Json(new
                            {
                                Success = false,
                                Message = "The financial year supplied could not be retrieved."
                            });
                        }
                    }
                    else
                    {
                        //get the current financial year
                        var financialYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
                        if (financialYearResp.Success)
                            printData.FinancialYear = financialYearResp.FinancialYear;
                    }

                    //get last two financial years
                    var financialLastYearOneResp = await _financialYearService.FindPreviousYearFromCurrentYear(printData.FinancialYear).ConfigureAwait(false);
                    var financialLastYearTwoResp = await _financialYearService.FindPreviousYearFromCurrentYear(financialLastYearOneResp.FinancialYear).ConfigureAwait(false);


                    //get the Authority to print for
                    var authorityResp = await _authorityService.FindByIdAsync(authorityId).ConfigureAwait(false);
                    if (!authorityResp.Success)
                    {
                        return Json(new { Success = false, Message = "The authority supplied is incorrect, please choose the right authority." });
                    }
                    var authority = authorityResp.Authority;
                    printData.Authority = authority;

                    var ceilingResp = await _budgetCeilingService.FindApprovedByAuthorityForCurrentYear(printData.FinancialYear.ID, printData.Authority.ID).ConfigureAwait(false);
                    printData.BudgetCeiling = ceilingResp.BudgetCeiling;

                    //get the budget details for the authority
                    var budgetHeaderResp = await _budgetCeilingHeaderService.FindCurrentAsync().ConfigureAwait(false);
                    if (budgetHeaderResp.Success)
                        printData.BudgetCeilingHeader = budgetHeaderResp.BudgetCeilingHeader;

                    //get contract listing for the authority
                    var authorityContracts = await _contractService.ListContractsByAgencyByFinancialYear(printData.Authority.ID, printData.FinancialYear.ID).ConfigureAwait(false);
                    var authorityContractsLastYearOne = await _contractService.ListContractsByAgencyByFinancialYear(printData.Authority.ID, financialLastYearOneResp.FinancialYear.ID).ConfigureAwait(false);
                    var authorityContractsLastYearTwo = await _contractService.ListContractsByAgencyByFinancialYear(printData.Authority.ID, financialLastYearTwoResp.FinancialYear.ID).ConfigureAwait(false);

                    printData.Contracts = authorityContracts.ToList();
                    if (!printData.Contracts.Any())
                    {
                        return Json(new { Success = false, Message = "There are no contract records for the authority for download." });
                    }
                    //generate the file.
                    string handle = Guid.NewGuid().ToString();
                    var memoryStream = new MemoryStream();
                    using (ExcelPackage excel = new ExcelPackage(memoryStream))
                    {
                        string currentFileName = System.IO.Path.GetFileName("Kenha_APRP");




                        var lastYearSTwoSheet = excel.Workbook.Worksheets.Add(financialLastYearTwoResp.FinancialYear.Code);
                        var lastYearOneSheet = excel.Workbook.Worksheets.Add(financialLastYearOneResp.FinancialYear.Code);
                        var currentYearSheet = excel.Workbook.Worksheets.Add(printData.FinancialYear.Code);

                        //BEGIN CURRENT YEAR SHEET
                        //print the column headers
                        //First Row
                        currentYearSheet.Cells["B1:E1"].Value = "KENYA ROADS BOARD FUNDED PROJECTS";
                        currentYearSheet.Cells["B1:E1"].Merge = true;
                        currentYearSheet.Cells["B1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells["B1:E1"].Style.Font.Bold = true;
                        currentYearSheet.Cells["B1:E1"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Row(1).Height = 30;
                        currentYearSheet.Cells["B1:E1"].Style.WrapText = true;

                        currentYearSheet.Cells["B2:E2"].Value = "ROAD AGENCY NAME : " + printData.Authority.Name.ToUpper();
                        currentYearSheet.Cells["B2:E2"].Merge = true;
                        currentYearSheet.Cells["B2:E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells["B2:E2"].Style.Font.Bold = true;
                        currentYearSheet.Cells["B2:E2"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Row(1).Height = 30;
                        currentYearSheet.Cells["B2:E2"].Style.WrapText = true;

                        currentYearSheet.Cells["B3:E3"].Value = "APRP IMPLEMENTATION REPORT. DATE OF REPORT " + DateTime.UtcNow.ToShortDateString();
                        currentYearSheet.Cells["B3:E3"].Merge = true;
                        currentYearSheet.Cells["B3:E3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells["B3:E3"].Style.Font.Bold = true;
                        currentYearSheet.Cells["B3:E3"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Row(1).Height = 30;
                        currentYearSheet.Cells["B3:E3"].Style.WrapText = true;

                        currentYearSheet.Cells[4, 2].Style.Font.Bold = true;
                        currentYearSheet.Cells[4, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Cells[4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells[4, 2].Value = "S/NO.";


                        currentYearSheet.Cells[4, 3].Style.Font.Bold = true;
                        currentYearSheet.Cells[4, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Cells[4, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells[4, 3].Value = "FINANCIAL YEAR";


                        currentYearSheet.Cells[4, 4].Style.Font.Bold = true;
                        currentYearSheet.Cells[4, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Cells[4, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells[4, 4].Value = "FUNDING SOURCE";


                        currentYearSheet.Cells[4, 5].Style.Font.Bold = true;
                        currentYearSheet.Cells[4, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Cells[4, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells[4, 5].Value = "FUNDING TYPE";

                        currentYearSheet.Cells[4, 6].Style.Font.Bold = true;
                        currentYearSheet.Cells[4, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Cells[4, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells[4, 6].Value = "ROAD ID";

                        currentYearSheet.Cells[4, 7].Style.Font.Bold = true;
                        currentYearSheet.Cells[4, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Cells[4, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells[4, 7].Value = "ROAD NAME";

                        currentYearSheet.Cells[4, 8].Style.Font.Bold = true;
                        currentYearSheet.Cells[4, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Cells[4, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells[4, 8].Value = "SECTION ID";

                        currentYearSheet.Cells[4, 9].Style.Font.Bold = true;
                        currentYearSheet.Cells[4, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Cells[4, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells[4, 9].Value = "SURFACE TYPE";

                        currentYearSheet.Cells[4, 10].Style.Font.Bold = true;
                        currentYearSheet.Cells[4, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Cells[4, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells[4, 10].Value = "ROAD CONDITION";

                        currentYearSheet.Cells[4, 11].Style.Font.Bold = true;
                        currentYearSheet.Cells[4, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Cells[4, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells[4, 11].Value = "WORKS CATEGORY";

                        currentYearSheet.Cells[4, 12].Style.Font.Bold = true;
                        currentYearSheet.Cells[4, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Cells[4, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells[4, 12].Value = "APRP BUDGET AMOUNT (KES)";

                        currentYearSheet.Cells[4, 13].Style.Font.Bold = true;
                        currentYearSheet.Cells[4, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Cells[4, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells[4, 13].Value = "REVISED APRP BUDGET AMOUNT (KES)";

                        currentYearSheet.Cells[4, 14].Style.Font.Bold = true;
                        currentYearSheet.Cells[4, 14].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Cells[4, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells[4, 14].Value = "PLANNED LENGTH (KM)";


                        currentYearSheet.Cells[4, 15].Style.Font.Bold = true;
                        currentYearSheet.Cells[4, 15].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Cells[4, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells[4, 15].Value = "COUNTY LOCATION";

                        currentYearSheet.Cells[4, 16].Style.Font.Bold = true;
                        currentYearSheet.Cells[4, 16].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Cells[4, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells[4, 16].Value = "CONSTITUENCY NAME";

                        currentYearSheet.Cells[4, 17].Style.Font.Bold = true;
                        currentYearSheet.Cells[4, 17].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Cells[4, 17].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells[4, 17].Value = "CONTRACT NO.";

                        currentYearSheet.Cells[4, 18].Style.Font.Bold = true;
                        currentYearSheet.Cells[4, 18].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Cells[4, 18].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells[4, 18].Value = "CONTRACTOR'S NAME";

                        currentYearSheet.Cells[4, 19].Style.Font.Bold = true;
                        currentYearSheet.Cells[4, 19].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Cells[4, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells[4, 19].Value = "CONTRACT PERIOD";

                        currentYearSheet.Cells[4, 20].Style.Font.Bold = true;
                        currentYearSheet.Cells[4, 20].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Cells[4, 20].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells[4, 20].Value = "COMMENCEMENT DATE";

                        currentYearSheet.Cells[4, 21].Style.Font.Bold = true;
                        currentYearSheet.Cells[4, 21].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Cells[4, 21].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells[4, 21].Value = "COMPLETION DATE";

                        currentYearSheet.Cells[4, 22].Style.Font.Bold = true;
                        currentYearSheet.Cells[4, 22].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Cells[4, 22].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells[4, 22].Value = "DATE OF ACTUAL COMPLETION";

                        currentYearSheet.Cells[4, 23].Style.Font.Bold = true;
                        currentYearSheet.Cells[4, 23].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Cells[4, 23].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells[4, 23].Value = "CONTRACT SUM";

                        currentYearSheet.Cells[4, 24].Style.Font.Bold = true;
                        currentYearSheet.Cells[4, 24].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Cells[4, 24].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells[4, 24].Value = "ACHIEVED KM";

                        currentYearSheet.Cells[4, 25].Style.Font.Bold = true;
                        currentYearSheet.Cells[4, 25].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Cells[4, 25].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells[4, 25].Value = "PAYMENT DURING THE YEAR (KES)";

                        currentYearSheet.Cells[4, 26].Style.Font.Bold = true;
                        currentYearSheet.Cells[4, 26].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Cells[4, 26].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells[4, 26].Value = "COMMUNLATIVE PAYMENTS TO DATE";

                        currentYearSheet.Cells[4, 27].Style.Font.Bold = true;
                        currentYearSheet.Cells[4, 27].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Cells[4, 27].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells[4, 27].Value = "IMPLEMENTATION STATUS (% COMPLETION)";

                        currentYearSheet.Cells[4, 28].Style.Font.Bold = true;
                        currentYearSheet.Cells[4, 28].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        currentYearSheet.Cells[4, 28].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        currentYearSheet.Cells[4, 28].Value = "REMARKS";

                        currentYearSheet.Cells.AutoFitColumns();

                        //output the workplan details
                        var rowCount = 5;
                        var cellCount = 2;

                        foreach (var contract in printData.Contracts)
                        {
                            //get the workplans in the contract
                            foreach (var workplanPackagePlan in contract.WorkPlanPackage.WorkpackageRoadWorkSectionPlans)
                            {
                                //currentYearSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                currentYearSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                currentYearSheet.Cells[rowCount, cellCount].Value = (rowCount - 4);
                                currentYearSheet.Cells[rowCount, cellCount].Style.Numberformat.Format = "0";

                                cellCount++;
                                //currentYearSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                currentYearSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                currentYearSheet.Cells[rowCount, cellCount].Value = contract.WorkPlanPackage.FinancialYear.Code;

                                cellCount++;
                                //currentYearSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                currentYearSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                currentYearSheet.Cells[rowCount, cellCount].Value = (workplanPackagePlan.RoadWorkSectionPlan.FundingSource != null ? workplanPackagePlan.RoadWorkSectionPlan.FundingSource.Code : "");

                                cellCount++;
                                //currentYearSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                currentYearSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                currentYearSheet.Cells[rowCount, cellCount].Value = (workplanPackagePlan.RoadWorkSectionPlan.FundType != null ? workplanPackagePlan.RoadWorkSectionPlan.FundType.Code : "");

                                cellCount++;
                                //currentYearSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                currentYearSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                currentYearSheet.Cells[rowCount, cellCount].Value = workplanPackagePlan.RoadWorkSectionPlan.Road.RoadNumber;

                                cellCount++;
                                //currentYearSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                currentYearSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                currentYearSheet.Cells[rowCount, cellCount].Value = workplanPackagePlan.RoadWorkSectionPlan.Road.RoadName;

                                cellCount++;
                                //currentYearSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                currentYearSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                currentYearSheet.Cells[rowCount, cellCount].Value = workplanPackagePlan.RoadWorkSectionPlan.RoadSection.SectionName;

                                cellCount++;
                                //currentYearSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                currentYearSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                currentYearSheet.Cells[rowCount, cellCount].Value = (workplanPackagePlan.RoadWorkSectionPlan.RoadSection.SurfaceType != null ? workplanPackagePlan.RoadWorkSectionPlan.RoadSection.SurfaceType.Code : "");

                                cellCount++;
                                //currentYearSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                currentYearSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                currentYearSheet.Cells[rowCount, cellCount].Value = "road condition";

                                cellCount++;
                                //currentYearSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                currentYearSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                currentYearSheet.Cells[rowCount, cellCount].Value = (workplanPackagePlan.RoadWorkSectionPlan.WorkCategory != null ? workplanPackagePlan.RoadWorkSectionPlan.WorkCategory.Code : "");

                                cellCount++;
                                //currentYearSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                currentYearSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                currentYearSheet.Cells[rowCount, cellCount].Value = workplanPackagePlan.RoadWorkSectionPlan.TotalEstimateCost;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Numberformat.Format = "#,##0.00";

                                cellCount++;
                                //currentYearSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                currentYearSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                currentYearSheet.Cells[rowCount, cellCount].Value = "0";
                                currentYearSheet.Cells[rowCount, cellCount].Style.Numberformat.Format = "#,##0.00";

                                cellCount++;
                                //currentYearSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                currentYearSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                currentYearSheet.Cells[rowCount, cellCount].Value = workplanPackagePlan.RoadWorkSectionPlan.RoadSection.Length;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Numberformat.Format = "#,##0.00";

                                cellCount++;
                                //currentYearSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                currentYearSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                currentYearSheet.Cells[rowCount, cellCount].Value = (workplanPackagePlan.RoadWorkSectionPlan.RoadSection.Constituency != null ? workplanPackagePlan.RoadWorkSectionPlan.RoadSection.Constituency.Name : "");

                                cellCount++;
                                //currentYearSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                currentYearSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                currentYearSheet.Cells[rowCount, cellCount].Value = (workplanPackagePlan.RoadWorkSectionPlan.RoadSection.Constituency.County != null ? workplanPackagePlan.RoadWorkSectionPlan.RoadSection.Constituency.County.Name : "");

                                cellCount++;
                                //currentYearSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                currentYearSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                currentYearSheet.Cells[rowCount, cellCount].Value = contract.WorkPlanPackage.Name;

                                cellCount++;
                                //currentYearSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                currentYearSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                currentYearSheet.Cells[rowCount, cellCount].Value = (contract.Contractor != null ? contract.Contractor.Name : "");

                                cellCount++;
                                //currentYearSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                currentYearSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                var period = 0;
                                if (contract.ContractStartDate != null || contract.ContractEndDate != null)
                                {
                                    period = (((DateTime)contract.ContractEndDate).Month - ((DateTime)contract.ContractStartDate).Month) + 12 * (((DateTime)contract.ContractEndDate).Year - ((DateTime)contract.ContractStartDate).Year);
                                }
                                currentYearSheet.Cells[rowCount, cellCount].Value = period + " Months";

                                cellCount++;
                                //currentYearSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                currentYearSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                currentYearSheet.Cells[rowCount, cellCount].Value = ((DateTime)contract.CommencementDate).ToShortDateString();

                                cellCount++;
                                //currentYearSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                currentYearSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                currentYearSheet.Cells[rowCount, cellCount].Value = ((DateTime)contract.CompletionDate).ToShortDateString();

                                cellCount++;
                                //currentYearSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                currentYearSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                currentYearSheet.Cells[rowCount, cellCount].Value = ((DateTime)contract.ActualCompletionDate).ToShortDateString();

                                cellCount++;
                                //currentYearSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                currentYearSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                currentYearSheet.Cells[rowCount, cellCount].Value = contract.ContractSumPackage;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Numberformat.Format = "#,##0.00";

                                cellCount++;
                                //currentYearSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                currentYearSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                currentYearSheet.Cells[rowCount, cellCount].Value = workplanPackagePlan.RoadWorkSectionPlan.AchievedLength;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Numberformat.Format = "#,##0.00";

                                cellCount++;
                                //currentYearSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                currentYearSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                currentYearSheet.Cells[rowCount, cellCount].Value = contract.PaymentCertificates.Sum(s => s.CertificateTotalWorkDoneThis);

                                cellCount++;
                                //currentYearSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                currentYearSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                currentYearSheet.Cells[rowCount, cellCount].Value = contract.PaymentCertificates.Sum(s => s.CertificateTotalWorkDoneThis);

                                cellCount++;
                                //currentYearSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                currentYearSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                //calculating status
                                var status = 0d;
                                var plannedLength = workplanPackagePlan.RoadWorkSectionPlan.PlannedLength;
                                var achievedLength = workplanPackagePlan.RoadWorkSectionPlan.AchievedLength;
                                if (plannedLength > 0 && achievedLength > 0)
                                {
                                    status = (achievedLength / plannedLength);
                                }
                                currentYearSheet.Cells[rowCount, cellCount].Value = status;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Numberformat.Format = "#,##0.00";
                                cellCount++;
                                //currentYearSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                currentYearSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                currentYearSheet.Cells[rowCount, cellCount].Value = "Remarks";

                                cellCount = 2;
                                rowCount++;
                            }


                        }

                        //END CURRENT YEAR SHEET

                        //BEGIN YEAR ONE SHEET
                        //print the column headers
                        //First Row
                        lastYearOneSheet.Cells["B1:E1"].Value = "KENYA ROADS BOARD FUNDED PROJECTS";
                        lastYearOneSheet.Cells["B1:E1"].Merge = true;
                        lastYearOneSheet.Cells["B1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells["B1:E1"].Style.Font.Bold = true;
                        lastYearOneSheet.Cells["B1:E1"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Row(1).Height = 30;
                        lastYearOneSheet.Cells["B1:E1"].Style.WrapText = true;

                        lastYearOneSheet.Cells["B2:E2"].Value = "ROAD AGENCY NAME : " + printData.Authority.Name.ToUpper();
                        lastYearOneSheet.Cells["B2:E2"].Merge = true;
                        lastYearOneSheet.Cells["B2:E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells["B2:E2"].Style.Font.Bold = true;
                        lastYearOneSheet.Cells["B2:E2"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Row(1).Height = 30;
                        lastYearOneSheet.Cells["B2:E2"].Style.WrapText = true;

                        lastYearOneSheet.Cells["B3:E3"].Value = "APRP IMPLEMENTATION REPORT. DATE OF REPORT " + DateTime.UtcNow.ToShortDateString();
                        lastYearOneSheet.Cells["B3:E3"].Merge = true;
                        lastYearOneSheet.Cells["B3:E3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells["B3:E3"].Style.Font.Bold = true;
                        lastYearOneSheet.Cells["B3:E3"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Row(1).Height = 30;
                        lastYearOneSheet.Cells["B3:E3"].Style.WrapText = true;

                        lastYearOneSheet.Cells[4, 2].Style.Font.Bold = true;
                        lastYearOneSheet.Cells[4, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Cells[4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells[4, 2].Value = "S/NO.";


                        lastYearOneSheet.Cells[4, 3].Style.Font.Bold = true;
                        lastYearOneSheet.Cells[4, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Cells[4, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells[4, 3].Value = "FINANCIAL YEAR";


                        lastYearOneSheet.Cells[4, 4].Style.Font.Bold = true;
                        lastYearOneSheet.Cells[4, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Cells[4, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells[4, 4].Value = "FUNDING SOURCE";


                        lastYearOneSheet.Cells[4, 5].Style.Font.Bold = true;
                        lastYearOneSheet.Cells[4, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Cells[4, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells[4, 5].Value = "FUNDING TYPE";

                        lastYearOneSheet.Cells[4, 6].Style.Font.Bold = true;
                        lastYearOneSheet.Cells[4, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Cells[4, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells[4, 6].Value = "ROAD ID";

                        lastYearOneSheet.Cells[4, 7].Style.Font.Bold = true;
                        lastYearOneSheet.Cells[4, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Cells[4, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells[4, 7].Value = "ROAD NAME";

                        lastYearOneSheet.Cells[4, 8].Style.Font.Bold = true;
                        lastYearOneSheet.Cells[4, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Cells[4, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells[4, 8].Value = "SECTION ID";

                        lastYearOneSheet.Cells[4, 9].Style.Font.Bold = true;
                        lastYearOneSheet.Cells[4, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Cells[4, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells[4, 9].Value = "SURFACE TYPE";

                        lastYearOneSheet.Cells[4, 10].Style.Font.Bold = true;
                        lastYearOneSheet.Cells[4, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Cells[4, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells[4, 10].Value = "ROAD CONDITION";

                        lastYearOneSheet.Cells[4, 11].Style.Font.Bold = true;
                        lastYearOneSheet.Cells[4, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Cells[4, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells[4, 11].Value = "WORKS CATEGORY";

                        lastYearOneSheet.Cells[4, 12].Style.Font.Bold = true;
                        lastYearOneSheet.Cells[4, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Cells[4, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells[4, 12].Value = "APRP BUDGET AMOUNT (KES)";

                        lastYearOneSheet.Cells[4, 13].Style.Font.Bold = true;
                        lastYearOneSheet.Cells[4, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Cells[4, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells[4, 13].Value = "REVISED APRP BUDGET AMOUNT (KES)";

                        lastYearOneSheet.Cells[4, 14].Style.Font.Bold = true;
                        lastYearOneSheet.Cells[4, 14].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Cells[4, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells[4, 14].Value = "PLANNED LENGTH (KM)";


                        lastYearOneSheet.Cells[4, 15].Style.Font.Bold = true;
                        lastYearOneSheet.Cells[4, 15].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Cells[4, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells[4, 15].Value = "COUNTY LOCATION";

                        lastYearOneSheet.Cells[4, 16].Style.Font.Bold = true;
                        lastYearOneSheet.Cells[4, 16].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Cells[4, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells[4, 16].Value = "CONSTITUENCY NAME";

                        lastYearOneSheet.Cells[4, 17].Style.Font.Bold = true;
                        lastYearOneSheet.Cells[4, 17].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Cells[4, 17].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells[4, 17].Value = "CONTRACT NO.";

                        lastYearOneSheet.Cells[4, 18].Style.Font.Bold = true;
                        lastYearOneSheet.Cells[4, 18].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Cells[4, 18].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells[4, 18].Value = "CONTRACTOR'S NAME";

                        lastYearOneSheet.Cells[4, 19].Style.Font.Bold = true;
                        lastYearOneSheet.Cells[4, 19].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Cells[4, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells[4, 19].Value = "CONTRACT PERIOD";

                        lastYearOneSheet.Cells[4, 20].Style.Font.Bold = true;
                        lastYearOneSheet.Cells[4, 20].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Cells[4, 20].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells[4, 20].Value = "COMMENCEMENT DATE";

                        lastYearOneSheet.Cells[4, 21].Style.Font.Bold = true;
                        lastYearOneSheet.Cells[4, 21].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Cells[4, 21].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells[4, 21].Value = "COMPLETION DATE";

                        lastYearOneSheet.Cells[4, 22].Style.Font.Bold = true;
                        lastYearOneSheet.Cells[4, 22].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Cells[4, 22].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells[4, 22].Value = "DATE OF ACTUAL COMPLETION";

                        lastYearOneSheet.Cells[4, 23].Style.Font.Bold = true;
                        lastYearOneSheet.Cells[4, 23].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Cells[4, 23].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells[4, 23].Value = "CONTRACT SUM";

                        lastYearOneSheet.Cells[4, 24].Style.Font.Bold = true;
                        lastYearOneSheet.Cells[4, 24].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Cells[4, 24].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells[4, 24].Value = "ACHIEVED KM";

                        lastYearOneSheet.Cells[4, 25].Style.Font.Bold = true;
                        lastYearOneSheet.Cells[4, 25].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Cells[4, 25].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells[4, 25].Value = "PAYMENT DURING THE YEAR (KES)";

                        lastYearOneSheet.Cells[4, 26].Style.Font.Bold = true;
                        lastYearOneSheet.Cells[4, 26].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Cells[4, 26].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells[4, 26].Value = "COMMUNLATIVE PAYMENTS TO DATE";

                        lastYearOneSheet.Cells[4, 27].Style.Font.Bold = true;
                        lastYearOneSheet.Cells[4, 27].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Cells[4, 27].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells[4, 27].Value = "IMPLEMENTATION STATUS (% COMPLETION)";

                        lastYearOneSheet.Cells[4, 28].Style.Font.Bold = true;
                        lastYearOneSheet.Cells[4, 28].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearOneSheet.Cells[4, 28].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearOneSheet.Cells[4, 28].Value = "REMARKS";

                        lastYearOneSheet.Cells.AutoFitColumns();

                        //output the workplan details
                        rowCount = 5;
                        cellCount = 2;

                        foreach (var contract in authorityContractsLastYearOne)
                        {
                            //get the workplans in the contract
                            foreach (var workplanPackagePlan in contract.WorkPlanPackage.WorkpackageRoadWorkSectionPlans)
                            {
                                //lastYearOneSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearOneSheet.Cells[rowCount, cellCount].Value = (rowCount - 4);
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Numberformat.Format = "0";

                                cellCount++;
                                //lastYearOneSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearOneSheet.Cells[rowCount, cellCount].Value = contract.WorkPlanPackage.FinancialYear.Code;

                                cellCount++;
                                //lastYearOneSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearOneSheet.Cells[rowCount, cellCount].Value = (workplanPackagePlan.RoadWorkSectionPlan.FundingSource != null ? workplanPackagePlan.RoadWorkSectionPlan.FundingSource.Code : "");

                                cellCount++;
                                //lastYearOneSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearOneSheet.Cells[rowCount, cellCount].Value = (workplanPackagePlan.RoadWorkSectionPlan.FundType != null ? workplanPackagePlan.RoadWorkSectionPlan.FundType.Code : "");

                                cellCount++;
                                //lastYearOneSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearOneSheet.Cells[rowCount, cellCount].Value = workplanPackagePlan.RoadWorkSectionPlan.Road.RoadNumber;

                                cellCount++;
                                //lastYearOneSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearOneSheet.Cells[rowCount, cellCount].Value = workplanPackagePlan.RoadWorkSectionPlan.Road.RoadName;

                                cellCount++;
                                //lastYearOneSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearOneSheet.Cells[rowCount, cellCount].Value = workplanPackagePlan.RoadWorkSectionPlan.RoadSection.SectionName;

                                cellCount++;
                                //lastYearOneSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearOneSheet.Cells[rowCount, cellCount].Value = (workplanPackagePlan.RoadWorkSectionPlan.RoadSection.SurfaceType != null ? workplanPackagePlan.RoadWorkSectionPlan.RoadSection.SurfaceType.Code : "");

                                cellCount++;
                                //lastYearOneSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearOneSheet.Cells[rowCount, cellCount].Value = "road condition";

                                cellCount++;
                                //lastYearOneSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearOneSheet.Cells[rowCount, cellCount].Value = (workplanPackagePlan.RoadWorkSectionPlan.WorkCategory != null ? workplanPackagePlan.RoadWorkSectionPlan.WorkCategory.Code : "");

                                cellCount++;
                                //lastYearOneSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearOneSheet.Cells[rowCount, cellCount].Value = workplanPackagePlan.RoadWorkSectionPlan.TotalEstimateCost;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Numberformat.Format = "#,##0.00";

                                cellCount++;
                                //lastYearOneSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearOneSheet.Cells[rowCount, cellCount].Value = "0";
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Numberformat.Format = "#,##0.00";

                                cellCount++;
                                //lastYearOneSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearOneSheet.Cells[rowCount, cellCount].Value = workplanPackagePlan.RoadWorkSectionPlan.RoadSection.Length;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Numberformat.Format = "#,##0.00";

                                cellCount++;
                                //lastYearOneSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearOneSheet.Cells[rowCount, cellCount].Value = (workplanPackagePlan.RoadWorkSectionPlan.RoadSection.Constituency != null ? workplanPackagePlan.RoadWorkSectionPlan.RoadSection.Constituency.Name : "");

                                cellCount++;
                                //lastYearOneSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearOneSheet.Cells[rowCount, cellCount].Value = (workplanPackagePlan.RoadWorkSectionPlan.RoadSection.Constituency.County != null ? workplanPackagePlan.RoadWorkSectionPlan.RoadSection.Constituency.County.Name : "");

                                cellCount++;
                                //lastYearOneSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearOneSheet.Cells[rowCount, cellCount].Value = contract.WorkPlanPackage.Name;

                                cellCount++;
                                //lastYearOneSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearOneSheet.Cells[rowCount, cellCount].Value = (contract.Contractor != null ? contract.Contractor.Name : "");

                                cellCount++;
                                //lastYearOneSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                var period = 0;
                                if (contract.ContractStartDate != null || contract.ContractEndDate != null)
                                {
                                    period = (((DateTime)contract.ContractEndDate).Month - ((DateTime)contract.ContractStartDate).Month) + 12 * (((DateTime)contract.ContractEndDate).Year - ((DateTime)contract.ContractStartDate).Year);
                                }
                                lastYearOneSheet.Cells[rowCount, cellCount].Value = period + " Months";

                                cellCount++;
                                //lastYearOneSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearOneSheet.Cells[rowCount, cellCount].Value = ((DateTime)contract.CommencementDate).ToShortDateString();

                                cellCount++;
                                //lastYearOneSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearOneSheet.Cells[rowCount, cellCount].Value = ((DateTime)contract.CompletionDate).ToShortDateString();

                                cellCount++;
                                //lastYearOneSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearOneSheet.Cells[rowCount, cellCount].Value = ((DateTime)contract.ActualCompletionDate).ToShortDateString();

                                cellCount++;
                                //lastYearOneSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearOneSheet.Cells[rowCount, cellCount].Value = contract.ContractSum;

                                cellCount++;
                                //lastYearOneSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                currentYearSheet.Cells[rowCount, cellCount].Value = workplanPackagePlan.RoadWorkSectionPlan.AchievedLength;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Numberformat.Format = "#,##0.00";

                                cellCount++;
                                //lastYearOneSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearOneSheet.Cells[rowCount, cellCount].Value = contract.PaymentCertificates.Sum(s => s.CertificateTotalWorkDoneThis);

                                cellCount++;
                                //lastYearOneSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearOneSheet.Cells[rowCount, cellCount].Value = contract.PaymentCertificates.Sum(s => s.CertificateTotalWorkDoneThis);

                                cellCount++;
                                //lastYearOneSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearOneSheet.Cells[rowCount, cellCount].Value = "Status";


                                cellCount++;
                                //lastYearOneSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearOneSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearOneSheet.Cells[rowCount, cellCount].Value = "Remarks";

                                cellCount = 2;
                                rowCount++;
                            }


                        }

                        //END  YEAR ONE SHEET
                        lastYearOneSheet.Cells.AutoFitColumns();

                        //BEGIN YEAR TWO SHEET
                        //print the column headers
                        //First Row
                        lastYearSTwoSheet.Cells["B1:E1"].Value = "KENYA ROADS BOARD FUNDED PROJECTS";
                        lastYearSTwoSheet.Cells["B1:E1"].Merge = true;
                        lastYearSTwoSheet.Cells["B1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells["B1:E1"].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells["B1:E1"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Row(1).Height = 30;
                        lastYearSTwoSheet.Cells["B1:E1"].Style.WrapText = true;

                        lastYearSTwoSheet.Cells["B2:E2"].Value = "ROAD AGENCY NAME : " + printData.Authority.Name.ToUpper();
                        lastYearSTwoSheet.Cells["B2:E2"].Merge = true;
                        lastYearSTwoSheet.Cells["B2:E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells["B2:E2"].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells["B2:E2"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Row(1).Height = 30;
                        lastYearSTwoSheet.Cells["B2:E2"].Style.WrapText = true;

                        lastYearSTwoSheet.Cells["B3:E3"].Value = "APRP IMPLEMENTATION REPORT. DATE OF REPORT " + DateTime.UtcNow.ToShortDateString();
                        lastYearSTwoSheet.Cells["B3:E3"].Merge = true;
                        lastYearSTwoSheet.Cells["B3:E3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells["B3:E3"].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells["B3:E3"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Row(1).Height = 30;
                        lastYearSTwoSheet.Cells["B3:E3"].Style.WrapText = true;

                        lastYearSTwoSheet.Cells[4, 2].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells[4, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Cells[4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells[4, 2].Value = "S/NO.";


                        lastYearSTwoSheet.Cells[4, 3].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells[4, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Cells[4, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells[4, 3].Value = "FINANCIAL YEAR";


                        lastYearSTwoSheet.Cells[4, 4].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells[4, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Cells[4, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells[4, 4].Value = "FUNDING SOURCE";


                        lastYearSTwoSheet.Cells[4, 5].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells[4, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Cells[4, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells[4, 5].Value = "FUNDING TYPE";

                        lastYearSTwoSheet.Cells[4, 6].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells[4, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Cells[4, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells[4, 6].Value = "ROAD ID";

                        lastYearSTwoSheet.Cells[4, 7].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells[4, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Cells[4, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells[4, 7].Value = "ROAD NAME";

                        lastYearSTwoSheet.Cells[4, 8].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells[4, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Cells[4, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells[4, 8].Value = "SECTION ID";

                        lastYearSTwoSheet.Cells[4, 9].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells[4, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Cells[4, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells[4, 9].Value = "SURFACE TYPE";

                        lastYearSTwoSheet.Cells[4, 10].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells[4, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Cells[4, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells[4, 10].Value = "ROAD CONDITION";

                        lastYearSTwoSheet.Cells[4, 11].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells[4, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Cells[4, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells[4, 11].Value = "WORKS CATEGORY";

                        lastYearSTwoSheet.Cells[4, 12].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells[4, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Cells[4, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells[4, 12].Value = "APRP BUDGET AMOUNT (KES)";

                        lastYearSTwoSheet.Cells[4, 13].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells[4, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Cells[4, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells[4, 13].Value = "REVISED APRP BUDGET AMOUNT (KES)";

                        lastYearSTwoSheet.Cells[4, 14].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells[4, 14].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Cells[4, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells[4, 14].Value = "PLANNED LENGTH (KM)";


                        lastYearSTwoSheet.Cells[4, 15].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells[4, 15].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Cells[4, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells[4, 15].Value = "COUNTY LOCATION";

                        lastYearSTwoSheet.Cells[4, 16].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells[4, 16].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Cells[4, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells[4, 16].Value = "CONSTITUENCY NAME";

                        lastYearSTwoSheet.Cells[4, 17].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells[4, 17].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Cells[4, 17].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells[4, 17].Value = "CONTRACT NO.";

                        lastYearSTwoSheet.Cells[4, 18].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells[4, 18].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Cells[4, 18].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells[4, 18].Value = "CONTRACTOR'S NAME";

                        lastYearSTwoSheet.Cells[4, 19].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells[4, 19].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Cells[4, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells[4, 19].Value = "CONTRACT PERIOD";

                        lastYearSTwoSheet.Cells[4, 20].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells[4, 20].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Cells[4, 20].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells[4, 20].Value = "COMMENCEMENT DATE";

                        lastYearSTwoSheet.Cells[4, 21].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells[4, 21].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Cells[4, 21].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells[4, 21].Value = "COMPLETION DATE";

                        lastYearSTwoSheet.Cells[4, 22].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells[4, 22].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Cells[4, 22].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells[4, 22].Value = "DATE OF ACTUAL COMPLETION";

                        lastYearSTwoSheet.Cells[4, 23].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells[4, 23].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Cells[4, 23].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells[4, 23].Value = "CONTRACT SUM";

                        lastYearSTwoSheet.Cells[4, 24].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells[4, 24].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Cells[4, 24].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells[4, 24].Value = "ACHIEVED KM";

                        lastYearSTwoSheet.Cells[4, 25].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells[4, 25].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Cells[4, 25].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells[4, 25].Value = "PAYMENT DURING THE YEAR (KES)";

                        lastYearSTwoSheet.Cells[4, 26].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells[4, 26].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Cells[4, 26].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells[4, 26].Value = "COMMUNLATIVE PAYMENTS TO DATE";

                        lastYearSTwoSheet.Cells[4, 27].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells[4, 27].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Cells[4, 27].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells[4, 27].Value = "IMPLEMENTATION STATUS (% COMPLETION)";

                        lastYearSTwoSheet.Cells[4, 28].Style.Font.Bold = true;
                        lastYearSTwoSheet.Cells[4, 28].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        lastYearSTwoSheet.Cells[4, 28].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        lastYearSTwoSheet.Cells[4, 28].Value = "REMARKS";

                        lastYearSTwoSheet.Cells.AutoFitColumns();

                        //output the workplan details
                        rowCount = 5;
                        cellCount = 2;

                        foreach (var contract in authorityContractsLastYearTwo)
                        {
                            //get the workplans in the contract
                            foreach (var workplanPackagePlan in contract.WorkPlanPackage.WorkpackageRoadWorkSectionPlans)
                            {
                                //lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Value = (rowCount - 4);
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Numberformat.Format = "0";

                                cellCount++;
                                //lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Value = contract.WorkPlanPackage.FinancialYear.Code;

                                cellCount++;
                                //lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Value = (workplanPackagePlan.RoadWorkSectionPlan.FundingSource != null ? workplanPackagePlan.RoadWorkSectionPlan.FundingSource.Code : "");

                                cellCount++;
                                //lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Value = (workplanPackagePlan.RoadWorkSectionPlan.FundType != null ? workplanPackagePlan.RoadWorkSectionPlan.FundType.Code : "");

                                cellCount++;
                                //lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Value = workplanPackagePlan.RoadWorkSectionPlan.Road.RoadNumber;

                                cellCount++;
                                //lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Value = workplanPackagePlan.RoadWorkSectionPlan.Road.RoadName;

                                cellCount++;
                                //lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Value = workplanPackagePlan.RoadWorkSectionPlan.RoadSection.SectionName;

                                cellCount++;
                                //lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Value = (workplanPackagePlan.RoadWorkSectionPlan.RoadSection.SurfaceType != null ? workplanPackagePlan.RoadWorkSectionPlan.RoadSection.SurfaceType.Code : "");

                                cellCount++;
                                //lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Value = "road condition";

                                cellCount++;
                                //lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Value = (workplanPackagePlan.RoadWorkSectionPlan.WorkCategory != null ? workplanPackagePlan.RoadWorkSectionPlan.WorkCategory.Code : "");

                                cellCount++;
                                //lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Value = workplanPackagePlan.RoadWorkSectionPlan.TotalEstimateCost;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Numberformat.Format = "#,##0.00";

                                cellCount++;
                                //lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Value = "0";
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Numberformat.Format = "#,##0.00";

                                cellCount++;
                                //lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Value = workplanPackagePlan.RoadWorkSectionPlan.RoadSection.Length;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Numberformat.Format = "#,##0.00";

                                cellCount++;
                                //lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Value = (workplanPackagePlan.RoadWorkSectionPlan.RoadSection.Constituency != null ? workplanPackagePlan.RoadWorkSectionPlan.RoadSection.Constituency.Name : "");

                                cellCount++;
                                //lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Value = (workplanPackagePlan.RoadWorkSectionPlan.RoadSection.Constituency.County != null ? workplanPackagePlan.RoadWorkSectionPlan.RoadSection.Constituency.County.Name : "");

                                cellCount++;
                                //lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Value = contract.WorkPlanPackage.Name;

                                cellCount++;
                                //lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Value = (contract.Contractor != null ? contract.Contractor.Name : "");

                                cellCount++;
                                //lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                var period = 0;
                                if (contract.ContractStartDate != null || contract.ContractEndDate != null)
                                {
                                    period = (((DateTime)contract.ContractEndDate).Month - ((DateTime)contract.ContractStartDate).Month) + 12 * (((DateTime)contract.ContractEndDate).Year - ((DateTime)contract.ContractStartDate).Year);
                                }
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Value = period + " Months";

                                cellCount++;
                                //lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Value = ((DateTime)contract.CommencementDate).ToShortDateString();

                                cellCount++;
                                //lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Value = ((DateTime)contract.CompletionDate).ToShortDateString();

                                cellCount++;
                                //lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearOneSheet.Cells[rowCount, cellCount].Value = ((DateTime)contract.ActualCompletionDate).ToShortDateString();

                                cellCount++;
                                //lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Value = contract.ContractSum;

                                cellCount++;
                                //lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                currentYearSheet.Cells[rowCount, cellCount].Value = workplanPackagePlan.RoadWorkSectionPlan.AchievedLength;
                                currentYearSheet.Cells[rowCount, cellCount].Style.Numberformat.Format = "#,##0.00";

                                cellCount++;
                                //lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Value = contract.PaymentCertificates.Sum(s => s.CertificateTotalWorkDoneThis);

                                cellCount++;
                                //lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Value = contract.PaymentCertificates.Sum(s => s.CertificateTotalWorkDoneThis);

                                cellCount++;
                                //lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Value = "Status";


                                cellCount++;
                                //lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Font.Bold = true;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                lastYearSTwoSheet.Cells[rowCount, cellCount].Value = "Remarks";

                                cellCount = 2;
                                rowCount++;
                            }


                        }

                        //END  YEAR TWO SHEET
                        lastYearSTwoSheet.Cells.AutoFitColumns();

                        excel.Workbook.Properties.Title = "Attempts";
                        _cache.Set(handle, excel.GetAsByteArray(),
                                        new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));
                        /*  using (var stream = new MemoryStream())
                          {
                              excel.SaveAs(stream);
                              _cache.Set(handle, excel.GetAsByteArray(),
                                                  new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));
                          }*/

                        return Json(new
                        {
                            Success = true,
                            Message = "Success",
                            Href = Url.Action("Download", "Workplan", new { fileGuid = handle, FileName = printData.Authority.Code + "_APRP_" + printData.FinancialYear.Code + ".xlsx" })
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"WorkplanController.GetAPRPForAgency API Error {ex.Message}");
                    return Json(new
                    {
                        Success = false,
                        Message = "Error occured"
                    });
                }

            }
        }



        public async Task<IActionResult> GetAPRPForKura(long authorityId, long financialYearId, long regionId, long countyId)
        {
            try
            {
                //retrieve the records to be printed
                var printData = new AWRPViewModel();
                if (financialYearId > 0)
                {
                    //get the supplied financial year
                    var financialYearResp = await _financialYearService.FindByIdAsync((long)financialYearId).ConfigureAwait(false);
                    if (financialYearResp.Success)
                    {
                        printData.FinancialYear = financialYearResp.FinancialYear;
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "The financial year supplied could not be retrieved."
                        });
                    }
                }
                else
                {
                    //get the current financial year
                    var financialYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
                    if (financialYearResp.Success)
                        printData.FinancialYear = financialYearResp.FinancialYear;
                }




                //get the Authority to print for
                var authorityResp = await _authorityService.FindByIdAsync(authorityId != 0 ? authorityId : countyId).ConfigureAwait(false);
                if (!authorityResp.Success)
                {
                    return Json(new { Success = false, Message = "The authority supplied is incorrect, please choose the right authority." });
                }
                var authority = authorityResp.Authority;
                printData.Authority = authority;

                var ceilingResp = await _budgetCeilingService.FindApprovedByAuthorityForCurrentYear(printData.FinancialYear.ID, printData.Authority.ID).ConfigureAwait(false);
                printData.BudgetCeiling = ceilingResp.BudgetCeiling;

                //get the budget details for the authority
                var budgetHeaderResp = await _budgetCeilingHeaderService.FindCurrentAsync().ConfigureAwait(false);
                if (budgetHeaderResp.Success)
                    printData.BudgetCeilingHeader = budgetHeaderResp.BudgetCeilingHeader;

                //get the administrative activities for the authority
                var authorityAdminActivities = await _adminOperationalActivityService.ListByAuthorityAsync(printData.Authority.ID, printData.FinancialYear.ID).ConfigureAwait(false);
                printData.AdminOperationalActivities = authorityAdminActivities.ToList();
                //get the current workplans for the authority
                var authorityWorkplans = await _roadWorkSectionPlanService.ListByAgencyAsync(authority.ID, printData.FinancialYear.ID).ConfigureAwait(false);
                if (!authorityWorkplans.Any())
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "There are no records for the selected county in the selected financial year that could be retrieved."
                    });
                }
                printData.RoadWorkSectionPlans = authorityWorkplans.ToList();

                //generate the file.
                string handle = Guid.NewGuid().ToString();
                var memoryStream = new MemoryStream();
                using (ExcelPackage excel = new ExcelPackage(memoryStream))
                {
                    string currentFileName = System.IO.Path.GetFileName("Kenha_APRP");

                    var grandSummarySheet = excel.Workbook.Worksheets.Add("1. Grand Summary");
                    var regionalSumSummarySheet = excel.Workbook.Worksheets.Add("2.Regional Sum");
                    var workTypeSummarySheet = excel.Workbook.Worksheets.Add("3. Works Type Summary");
                    var worksByRegionsSummarySheet = excel.Workbook.Worksheets.Add("4. Works by Regions");
                    var aprpSummarySheet = excel.Workbook.Worksheets.Add("5. APRP format");

                    var balanceForRoadWorks = 0d;
                    var totalIncome = 0d;
                    var budgetCeiling = 0d;
                    var transitTolls = 0d;
                    var operationsCost = 0d;

                    //BEGIN GRAN SUMMARY SHEET
                    //print the column headers
                    //First Row
                    int count = 1;
                    grandSummarySheet.Cells["A1:D1"].Value = "KENYA URBAN ROADS AUTHORITY ANNUAL ROADS WORKS PROGRAMME (ARWP) FOR FINANCIAL YEAR " + printData.FinancialYear.Code;
                    grandSummarySheet.Cells["A1:D1"].Merge = true;
                    grandSummarySheet.Cells["A1:D1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells["A1:D1"].Style.Font.Bold = true;
                    grandSummarySheet.Cells["A1:D1"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Row(1).Height = 30;
                    grandSummarySheet.Cells["A1:D1"].Style.WrapText = true;

                    grandSummarySheet.Cells[2, 1].Style.Font.Bold = true;
                    grandSummarySheet.Cells[2, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[2, 1].Value = "NO.";

                    grandSummarySheet.Cells[2, 2].Style.Font.Bold = true;
                    grandSummarySheet.Cells[2, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[2, 2].Value = "INCOME / CEILINGS";

                    grandSummarySheet.Cells[2, 3].Style.Font.Bold = true;
                    grandSummarySheet.Cells[2, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[2, 3].Value = "BUDGET FY " + printData.FinancialYear.Code;

                    grandSummarySheet.Cells[2, 4].Style.Font.Bold = true;
                    grandSummarySheet.Cells[2, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[2, 4].Value = " ";

                    //RMLF
                    grandSummarySheet.Cells[3, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[3, 1].Value = count++;
                    grandSummarySheet.Cells[3, 1].Style.Numberformat.Format = "0";

                    grandSummarySheet.Cells[3, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[3, 2].Value = "10.2% RMLF";

                    grandSummarySheet.Cells[3, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[3, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[3, 3].Value = printData.BudgetCeiling.Amount;
                    grandSummarySheet.Cells[3, 3].Style.Numberformat.Format = "#,##0.00";

                    grandSummarySheet.Cells[3, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[3, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    grandSummarySheet.Cells[3, 4].Value = " - ";



                    //TOTAL INCOME
                    grandSummarySheet.Cells[4, 1].Style.Font.Bold = true;
                    grandSummarySheet.Cells[4, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[4, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[4, 1].Value = count++;
                    grandSummarySheet.Cells[4, 1].Style.Numberformat.Format = "0";

                    grandSummarySheet.Cells[4, 2].Style.Font.Bold = true;
                    grandSummarySheet.Cells[4, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[4, 2].Value = "TOTAL INCOME";

                    grandSummarySheet.Cells[4, 3].Value = printData.BudgetCeiling.Amount;
                    grandSummarySheet.Cells[4, 3].Style.Font.Bold = true;
                    grandSummarySheet.Cells[4, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[4, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[4, 3].Style.Numberformat.Format = "#,##0.00";

                    grandSummarySheet.Cells[4, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[4, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[4, 4].Style.Numberformat.Format = "-";


                    //OPERATIONS COSTS (4% OF KRB FUND)
                    grandSummarySheet.Cells[5, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[5, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[5, 1].Value = count++;
                    grandSummarySheet.Cells[5, 1].Style.Numberformat.Format = "0";

                    grandSummarySheet.Cells[5, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[5, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[5, 2].Value = "LESS OPERATIONS";



                    var kurraOperations = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("23", printData.FinancialYear.ID).ConfigureAwait(false);
                    if (kurraOperations.Success)
                    {
                        var objectResult = (ObjectResult)kurraOperations.IActionResult;
                        if (objectResult != null)
                        {
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var result = (OkObjectResult)objectResult;

                                grandSummarySheet.Cells[5, 3].Value = ((BudgetCeilingComputation)result.Value).Amount;
                                operationsCost = ((BudgetCeilingComputation)result.Value).Amount;
                            }
                        }
                    }
                    grandSummarySheet.Cells[5, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[5, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[5, 3].Style.Numberformat.Format = "#,##0.00";

                    grandSummarySheet.Cells[5, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[5, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[5, 4].Style.Numberformat.Format = "-";

                    grandSummarySheet.Cells[6, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[6, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[6, 4].Style.Numberformat.Format = "-";

                    //BALANCE FOR ROAD WORKS
                    grandSummarySheet.Cells[6, 1].Style.Font.Bold = true;
                    grandSummarySheet.Cells[6, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[6, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    grandSummarySheet.Cells[6, 2].Style.Font.Bold = true;
                    grandSummarySheet.Cells[6, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[6, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[6, 2].Value = "BALANCE FOR ROAD WORKS";


                    var kurraRoadWorksBalance = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("24", printData.FinancialYear.ID).ConfigureAwait(false);
                    if (kurraRoadWorksBalance.Success)
                    {
                        var objectResult = (ObjectResult)kurraRoadWorksBalance.IActionResult;
                        if (objectResult != null)
                        {
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var result = (OkObjectResult)objectResult;

                                grandSummarySheet.Cells[6, 3].Value = ((BudgetCeilingComputation)result.Value).Amount;
                                balanceForRoadWorks = ((BudgetCeilingComputation)result.Value).Amount;
                            }
                        }
                    }
                    grandSummarySheet.Cells[6, 3].Style.Font.Bold = true;
                    grandSummarySheet.Cells[6, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[6, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[6, 3].Style.Numberformat.Format = "#,##0.00";

                    //EXPENDITURES
                    grandSummarySheet.Cells[7, 1].Style.Font.Bold = true;
                    grandSummarySheet.Cells[7, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[7, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[7, 1].Value = "";

                    grandSummarySheet.Cells[7, 2, 7, 4].Style.Font.Bold = true;
                    grandSummarySheet.Cells[7, 2, 7, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[7, 2, 7, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[7, 2, 7, 4].Value = "EXPENDITURES";
                    grandSummarySheet.Cells[7, 2, 7, 4].Merge = true;

                    //Region
                    grandSummarySheet.Cells[8, 1].Style.Font.Bold = true;
                    grandSummarySheet.Cells[8, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[8, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[8, 1].Value = "";

                    grandSummarySheet.Cells[8, 2].Style.Font.Bold = true;
                    grandSummarySheet.Cells[8, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[8, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[8, 2].Value = "REGIONS/CORRIDORS";

                    grandSummarySheet.Cells[8, 3].Style.Font.Bold = true;
                    grandSummarySheet.Cells[8, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[8, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[8, 3].Value = "BUDGET FY " + printData.FinancialYear.Code;

                    grandSummarySheet.Cells[8, 4].Style.Font.Bold = true;
                    grandSummarySheet.Cells[8, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[8, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[8, 4].Value = "KMs";


                    int row = 9;
                    count = 1; // reset count.

                    //get authority regions
                    var authorityRegions = await _regionService.ListRegionsByAuthority(printData.Authority.ID).ConfigureAwait(false);
                    foreach (var region in authorityRegions)
                    {
                        var regionConstituencies = region.RegionToCountys.Select(c => c.County.Constituencys);
                        List<long> constituencyIds = new List<long>();
                        foreach (var constituency in regionConstituencies)
                        {
                            foreach (var consti in constituency)
                            {
                                constituencyIds.Add(consti.ID);
                            }
                        }

                        var regionWorkplans = printData.RoadWorkSectionPlans.Where(s => constituencyIds.Any(c => c == (long)s.RoadSection.ConstituencyId));


                        grandSummarySheet.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        grandSummarySheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        grandSummarySheet.Cells[row, 1].Value = count++;

                        grandSummarySheet.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        grandSummarySheet.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        grandSummarySheet.Cells[row, 2].Value = region.Name;

                        grandSummarySheet.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        grandSummarySheet.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        grandSummarySheet.Cells[row, 3].Value = regionWorkplans.Sum(s => s.TotalEstimateCost);//get summary of workplans in that region
                        grandSummarySheet.Cells[row, 3].Style.Numberformat.Format = "#,##0.00";

                        grandSummarySheet.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        grandSummarySheet.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        grandSummarySheet.Cells[row, 4].Value = regionWorkplans.Sum(s => s.PlannedLength);//get summary of workplans in that region
                        grandSummarySheet.Cells[row, 4].Style.Numberformat.Format = "#,##0.00";

                        row++;
                    }

                    //REGION TOTALS
                    row++;
                    grandSummarySheet.Cells[row, 1].Style.Font.Bold = true;
                    grandSummarySheet.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[row, 1].Value = "";

                    grandSummarySheet.Cells[row, 2].Style.Font.Bold = true;
                    grandSummarySheet.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[row, 2].Value = "TOTAL PLANNED FOR REGIONS (" + printData.RoadWorkSectionPlans.Sum(s => s.PlannedLength) + " KMS)";
                    grandSummarySheet.Cells[row, 2].Style.Numberformat.Format = "#,##0.00";

                    grandSummarySheet.Cells[row, 3].Style.Font.Bold = true;
                    grandSummarySheet.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[row, 3].Value = printData.RoadWorkSectionPlans.Sum(c => c.TotalEstimateCost);
                    grandSummarySheet.Cells[row, 3].Style.Numberformat.Format = "#,##0.00";

                    grandSummarySheet.Cells[row, 4].Style.Font.Bold = true;
                    grandSummarySheet.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[row, 4].Value = printData.RoadWorkSectionPlans.Sum(s => s.PlannedLength);
                    grandSummarySheet.Cells[row, 4].Style.Numberformat.Format = "#,##0.00";


                    //GRAND TOTALS
                    row++;
                    grandSummarySheet.Cells[row, 1].Style.Font.Bold = true;
                    grandSummarySheet.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[row, 1].Value = "";

                    grandSummarySheet.Cells[row, 2].Style.Font.Bold = true;
                    grandSummarySheet.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[row, 2].Value = "GRAND TOTAL";

                    grandSummarySheet.Cells[row, 3].Style.Font.Bold = true;
                    grandSummarySheet.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[row, 3].Value = (printData.RoadWorkSectionPlans.Sum(c => c.TotalEstimateCost) + printData.AdminOperationalActivities.Sum(c => c.Amount)); ;
                    grandSummarySheet.Cells[row, 3].Style.Numberformat.Format = "#,##0.00";

                    grandSummarySheet.Cells[row, 4].Style.Font.Bold = true;
                    grandSummarySheet.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    grandSummarySheet.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    grandSummarySheet.Cells[row, 4].Value = printData.RoadWorkSectionPlans.Sum(s => s.PlannedLength);
                    grandSummarySheet.Cells[row, 4].Style.Numberformat.Format = "#,##0.00";

                    grandSummarySheet.Cells.AutoFitColumns();

                    //END GRAND SUMMARY SHEET

                    //BEGIN REGIONAL SUMM SHEET
                    regionalSumSummarySheet.Cells["A1:D1"].Value = "KENYA URBAN ROADS AUTHORITY ANNUAL ROADS WORKS PROGRAMME (ARWP) FOR FINANCIAL YEAR " + printData.FinancialYear.Code + " REGIONAL SUMMARY";
                    regionalSumSummarySheet.Cells["A1:D1"].Merge = true;
                    regionalSumSummarySheet.Cells["A1:D1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    regionalSumSummarySheet.Cells["A1:D1"].Style.Font.Bold = true;
                    regionalSumSummarySheet.Cells["A1:D1"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    regionalSumSummarySheet.Row(1).Height = 30;
                    regionalSumSummarySheet.Cells["A1:D1"].Style.WrapText = true;


                    regionalSumSummarySheet.Cells[2, 1].Style.Font.Bold = true;
                    regionalSumSummarySheet.Cells[2, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    regionalSumSummarySheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    regionalSumSummarySheet.Cells[2, 1].Value = "NO.";

                    regionalSumSummarySheet.Cells[2, 2].Style.Font.Bold = true;
                    regionalSumSummarySheet.Cells[2, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    regionalSumSummarySheet.Cells[2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    regionalSumSummarySheet.Cells[2, 2].Value = "REGION";

                    regionalSumSummarySheet.Cells[2, 3].Style.Font.Bold = true;
                    regionalSumSummarySheet.Cells[2, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    regionalSumSummarySheet.Cells[2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    regionalSumSummarySheet.Cells[2, 3].Value = "KMS ";

                    regionalSumSummarySheet.Cells[2, 4].Style.Font.Bold = true;
                    regionalSumSummarySheet.Cells[2, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    regionalSumSummarySheet.Cells[2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    regionalSumSummarySheet.Cells[2, 4].Value = "BUDGET FY " + printData.FinancialYear.Code;



                    //print admin summaries
                    int rowSum = 3;
                    int countSum = 1;
                    //print the admin activities.


                    //get authority regions for summary
                    foreach (var region in authorityRegions)
                    {
                        var regionConstituencies = region.RegionToCountys.Select(c => c.County.Constituencys);
                        List<long> constituencyIds = new List<long>();
                        foreach (var constituency in regionConstituencies)
                        {
                            foreach (var consti in constituency)
                            {
                                constituencyIds.Add(consti.ID);
                            }
                        }

                        var regionWorkplans = printData.RoadWorkSectionPlans.Where(s => constituencyIds.Any(c => c == (long)s.RoadSection.ConstituencyId));


                        regionalSumSummarySheet.Cells[rowSum, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        regionalSumSummarySheet.Cells[rowSum, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        regionalSumSummarySheet.Cells[rowSum, 1].Value = countSum++;

                        regionalSumSummarySheet.Cells[rowSum, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        regionalSumSummarySheet.Cells[rowSum, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        regionalSumSummarySheet.Cells[rowSum, 2].Value = region.Name;

                        regionalSumSummarySheet.Cells[rowSum, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        regionalSumSummarySheet.Cells[rowSum, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        regionalSumSummarySheet.Cells[rowSum, 3].Value = regionWorkplans.Sum(s => s.PlannedLength);//get summary of workplans in that region
                        regionalSumSummarySheet.Cells[rowSum, 3].Style.Numberformat.Format = "#,##0.00";

                        regionalSumSummarySheet.Cells[rowSum, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        regionalSumSummarySheet.Cells[rowSum, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        regionalSumSummarySheet.Cells[rowSum, 4].Value = regionWorkplans.Sum(s => s.TotalEstimateCost);//get summary of workplans in that region
                        regionalSumSummarySheet.Cells[rowSum, 4].Style.Numberformat.Format = "#,##0.00";
                        rowSum++;
                    }


                    //grand total
                    regionalSumSummarySheet.Cells[rowSum, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    regionalSumSummarySheet.Cells[rowSum, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    regionalSumSummarySheet.Cells[rowSum, 1].Style.Font.Bold = true;
                    regionalSumSummarySheet.Cells[rowSum, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    regionalSumSummarySheet.Cells[rowSum, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    regionalSumSummarySheet.Cells[rowSum, 2].Value = "GRAND TOTAL";

                    regionalSumSummarySheet.Cells[rowSum, 3].Style.Font.Bold = true;
                    regionalSumSummarySheet.Cells[rowSum, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    regionalSumSummarySheet.Cells[rowSum, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    regionalSumSummarySheet.Cells[rowSum, 3].Value = printData.RoadWorkSectionPlans.Sum(s => s.PlannedLength);//get summary of workplans in that region
                    regionalSumSummarySheet.Cells[rowSum, 3].Style.Numberformat.Format = "#,##0.00";

                    regionalSumSummarySheet.Cells[rowSum, 4].Style.Font.Bold = true;
                    regionalSumSummarySheet.Cells[rowSum, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    regionalSumSummarySheet.Cells[rowSum, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    regionalSumSummarySheet.Cells[rowSum, 4].Value = (printData.AdminOperationalActivities.Sum(a => a.Amount) + printData.RoadWorkSectionPlans.Sum(p => p.TotalEstimateCost));//get summary of workplans in that region
                    regionalSumSummarySheet.Cells[rowSum, 4].Style.Numberformat.Format = "#,##0.00";


                    regionalSumSummarySheet.Cells.AutoFitColumns();

                    //END REGIONAL SUMM SHEET

                    //WORKS CATEGORY SUMMARY
                    int rowCatSum = 3;
                    int countCat = 1;
                    var worksCategorySummary = printData.RoadWorkSectionPlans.GroupBy(p => p.WorkCategory).Select(
                            cat => new
                            {
                                Key = cat.Key,
                                Value = cat.First().WorkCategory.Name,
                                Code = cat.First().WorkCategory.Code,
                                KMs = cat.Sum(c => c.RoadSection.Length),
                                Budget = cat.Sum(p => p.TotalEstimateCost)
                            }
                        );

                    //print admin activities
                    workTypeSummarySheet.Cells["A1:D1"].Value = "KENYA URBAN ROADS AUTHORITY ANNUAL ROADS WORKS PROGRAMME (ARWP) FOR FINANCIAL YEAR  " + printData.FinancialYear.Code + " SUMMARY OF WORKS CATEGORY";
                    workTypeSummarySheet.Cells["A1:D1"].Merge = true;
                    workTypeSummarySheet.Cells["A1:D1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workTypeSummarySheet.Cells["A1:D1"].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells["A1:D1"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Row(1).Height = 30;
                    workTypeSummarySheet.Cells["A1:D1"].Style.WrapText = true;


                    workTypeSummarySheet.Cells[2, 1].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[2, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workTypeSummarySheet.Cells[2, 1].Value = "NO.";

                    workTypeSummarySheet.Cells[2, 2].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[2, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workTypeSummarySheet.Cells[2, 2].Value = "WORKS CATEGORY";

                    workTypeSummarySheet.Cells[2, 3].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[2, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workTypeSummarySheet.Cells[2, 3].Value = "KMS ";

                    workTypeSummarySheet.Cells[2, 4].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[2, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workTypeSummarySheet.Cells[2, 4].Value = "BUDGET FY " + printData.FinancialYear.Code;


                    //print works category summary

                    foreach (var category in worksCategorySummary)
                    {
                        workTypeSummarySheet.Cells[rowCatSum, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        workTypeSummarySheet.Cells[rowCatSum, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workTypeSummarySheet.Cells[rowCatSum, 1].Value = countCat++;

                        workTypeSummarySheet.Cells[rowCatSum, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        workTypeSummarySheet.Cells[rowCatSum, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workTypeSummarySheet.Cells[rowCatSum, 2].Value = category.Value.ToUpper();

                        workTypeSummarySheet.Cells[rowCatSum, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        workTypeSummarySheet.Cells[rowCatSum, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workTypeSummarySheet.Cells[rowCatSum, 3].Value = category.KMs;//get summary of workplans in that region
                        workTypeSummarySheet.Cells[rowCatSum, 3].Style.Numberformat.Format = "#,##0.00";

                        workTypeSummarySheet.Cells[rowCatSum, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        workTypeSummarySheet.Cells[rowCatSum, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workTypeSummarySheet.Cells[rowCatSum, 4].Value = category.Budget;//get summary of workplans in that region
                        workTypeSummarySheet.Cells[rowCatSum, 4].Style.Numberformat.Format = "#,##0.00";
                        rowCatSum++;
                    }

                    //grand total
                    workTypeSummarySheet.Cells[rowCatSum, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[rowCatSum, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    workTypeSummarySheet.Cells[rowCatSum, 2].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[rowCatSum, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[rowCatSum, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workTypeSummarySheet.Cells[rowCatSum, 2].Value = "GRAND TOTAL";

                    workTypeSummarySheet.Cells[rowCatSum, 3].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[rowCatSum, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[rowCatSum, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workTypeSummarySheet.Cells[rowCatSum, 3].Value = printData.RoadWorkSectionPlans.Sum(s => s.PlannedLength);//get summary of workplans in that region
                    workTypeSummarySheet.Cells[rowCatSum, 3].Style.Numberformat.Format = "#,##0.00";

                    workTypeSummarySheet.Cells[rowCatSum, 4].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[rowCatSum, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[rowCatSum, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workTypeSummarySheet.Cells[rowCatSum, 4].Value = printData.RoadWorkSectionPlans.Sum(p => p.TotalEstimateCost);//get summary of workplans in that region
                    workTypeSummarySheet.Cells[rowCatSum, 4].Style.Numberformat.Format = "#,##0.00";

                    workTypeSummarySheet.Cells.AutoFitColumns();


                    //END WORKS CATEGORY SUMMARY

                    //BEGIN WORKS BY REGIONS
                    int categoryCount = worksCategorySummary.Count();
                    var columnCount = (categoryCount * 2) + printData.AdminOperationalActivities.Count; // times *2 because each category spans two columns

                    //print admin activities
                    worksByRegionsSummarySheet.Cells[1, 1, 1, (columnCount + 2)].Value = "KENYA URBAN ROADS AUTHORITY ANNUAL ROADS WORKS PROGRAMME (ARWP) FOR FINANCIAL YEAR " + printData.FinancialYear.Code + " REGIONAL WORKS CATEGORY";

                    //worksheet.Cells["A1:B5"].Merge = true; =   workTypeSummarySheet.Cells[1, 1, 5, 2]

                    worksByRegionsSummarySheet.Cells[1, 1, 1, (columnCount + 2)].Merge = true;
                    worksByRegionsSummarySheet.Cells[1, 1, 1, (columnCount + 2)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksByRegionsSummarySheet.Cells[1, 1, 1, (columnCount + 2)].Style.Font.Bold = true;
                    worksByRegionsSummarySheet.Cells[1, 1, 1, (columnCount + 2)].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksByRegionsSummarySheet.Row(1).Height = 30;
                    worksByRegionsSummarySheet.Cells[1, 1, 1, (columnCount + 2)].Style.WrapText = true;

                    //set the No column and region
                    worksByRegionsSummarySheet.Cells[3, 1].Style.Font.Bold = true;
                    worksByRegionsSummarySheet.Cells[3, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksByRegionsSummarySheet.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksByRegionsSummarySheet.Cells[3, 1].Value = "NO.";

                    worksByRegionsSummarySheet.Cells[3, 2].Style.Font.Bold = true;
                    worksByRegionsSummarySheet.Cells[3, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksByRegionsSummarySheet.Cells[3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksByRegionsSummarySheet.Cells[3, 2].Value = "REGION";

                    //output the report headers
                    int cellId = 3;
                    //print the admin activities groups.
                    int rowCountSum = 4;
                    //work category summary

                    foreach (var cat in worksCategorySummary)
                    {
                        worksByRegionsSummarySheet.Cells[2, cellId].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksByRegionsSummarySheet.Cells[2, cellId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksByRegionsSummarySheet.Cells[2, cellId].Value = cat.Value.ToUpper();
                        worksByRegionsSummarySheet.Cells[2, cellId].Style.Font.Bold = true;
                        worksByRegionsSummarySheet.Cells[2, cellId, 2, (cellId + 1)].Merge = true;


                        worksByRegionsSummarySheet.Cells[3, cellId].Style.Font.Bold = true;
                        worksByRegionsSummarySheet.Cells[3, cellId].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksByRegionsSummarySheet.Cells[3, cellId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksByRegionsSummarySheet.Cells[3, cellId].Value = " KMS ";

                        worksByRegionsSummarySheet.Cells[3, (cellId + 1)].Style.Font.Bold = true;
                        worksByRegionsSummarySheet.Cells[3, (cellId + 1)].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksByRegionsSummarySheet.Cells[3, (cellId + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksByRegionsSummarySheet.Cells[3, (cellId + 1)].Value = " BUDGET FY " + printData.FinancialYear.Code;


                        cellId += 2;

                    }

                    int cellIdRegion = 3;
                    var loopCellId = cellIdRegion;
                    var countEntryRows = authorityRegions.Count();
                    var countEntryColumns = 2 * worksCategorySummary.Count();

                    foreach (var region in authorityRegions)
                    {

                        var regionConstituencies = region.RegionToCountys.Select(c => c.County.Constituencys);
                        List<long> constituencyIds = new List<long>();
                        foreach (var constituency in regionConstituencies)
                        {
                            foreach (var consti in constituency)
                            {
                                constituencyIds.Add(consti.ID);
                            }
                        }

                        var regionWorkplans = printData.RoadWorkSectionPlans.Where(s => constituencyIds.Any(c => c == (long)s.RoadSection.ConstituencyId));


                        worksByRegionsSummarySheet.Cells[rowCountSum, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksByRegionsSummarySheet.Cells[rowCountSum, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksByRegionsSummarySheet.Cells[rowCountSum, 1].Value = (rowCountSum - 3);

                        worksByRegionsSummarySheet.Cells[rowCountSum, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksByRegionsSummarySheet.Cells[rowCountSum, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksByRegionsSummarySheet.Cells[rowCountSum, 2].Value = region.Name;


                        foreach (var cat in worksCategorySummary)
                        {
                            //output summaries only on the specific cells
                            worksByRegionsSummarySheet.Cells[rowCountSum, cellIdRegion].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            worksByRegionsSummarySheet.Cells[rowCountSum, cellIdRegion].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksByRegionsSummarySheet.Cells[rowCountSum, cellIdRegion].Value = (double)regionWorkplans.Where(c => c.WorkCategory.Code == cat.Code).Sum(c => c.PlannedLength);
                            worksByRegionsSummarySheet.Cells[rowCountSum, cellIdRegion].Style.Numberformat.Format = "#,##0.00";

                            worksByRegionsSummarySheet.Cells[rowCountSum, (cellIdRegion + 1)].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            worksByRegionsSummarySheet.Cells[rowCountSum, (cellIdRegion + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksByRegionsSummarySheet.Cells[rowCountSum, (cellIdRegion + 1)].Value = (double)regionWorkplans.Where(c => c.WorkCategory.Code == cat.Code).Sum(c => c.TotalEstimateCost);
                            worksByRegionsSummarySheet.Cells[rowCountSum, (cellIdRegion + 1)].Style.Numberformat.Format = "#,##0.00";

                            cellIdRegion += 2;
                        }

                        //reset recount to old stage for the next region
                        //reset the cellId
                        cellIdRegion = loopCellId;

                        rowCountSum++;
                    }

                    //output the total summaries
                    worksByRegionsSummarySheet.Cells[rowCountSum, 1].Style.Font.Bold = true;
                    worksByRegionsSummarySheet.Cells[rowCountSum, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksByRegionsSummarySheet.Cells[rowCountSum, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksByRegionsSummarySheet.Cells[rowCountSum, 1].Value = (rowCountSum - 3);

                    worksByRegionsSummarySheet.Cells[rowCountSum, 2].Style.Font.Bold = true;
                    worksByRegionsSummarySheet.Cells[rowCountSum, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    worksByRegionsSummarySheet.Cells[rowCountSum, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksByRegionsSummarySheet.Cells[rowCountSum, 2].Value = "GRAND TOTAL";



                    //generate sum entry

                    for (int columnIndex = 3; columnIndex <= (countEntryColumns + 2); columnIndex++)
                    {
                        string totalMonthAddress = ExcelCellBase.GetAddress(
                            4, columnIndex,
                            (countEntryRows + 3), columnIndex
                        );
                        worksByRegionsSummarySheet.Cells[rowCountSum, columnIndex].Style.Font.Bold = true;
                        worksByRegionsSummarySheet.Cells[rowCountSum, columnIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        worksByRegionsSummarySheet.Cells[rowCountSum, columnIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksByRegionsSummarySheet.Cells[rowCountSum, columnIndex].Formula = string.Format("SUM({0})", totalMonthAddress);
                        worksByRegionsSummarySheet.Cells[rowCountSum, columnIndex].Style.Numberformat.Format = "#,##0.00";

                    }

                    //set borders entire page
                    for (int i = 4; i <= countEntryRows; i++)
                    {
                        //loop through the cells
                        for (int j = 3; j <= countEntryColumns; j++)
                        {
                            worksByRegionsSummarySheet.Cells[i, j].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        }
                    }


                    worksByRegionsSummarySheet.Cells.AutoFitColumns();


                    //END WORKS BY REGION

                    //BEGIN APRP SHEET


                    //print admin activities
                    aprpSummarySheet.Cells[1, 1, 1, (columnCount + 2)].Value = "KENYA URBAN ROADS AUTHORITY ANNUAL ROADS WORKS PROGRAMME (ARWP) FOR FINANCIAL YEAR " + printData.FinancialYear.Code + ". DETAILED WORK PLAN";

                    //worksheet.Cells["A1:B5"].Merge = true; =   workTypeSummarySheet.Cells[1, 1, 5, 2]

                    aprpSummarySheet.Cells[1, 1, 1, (columnCount + 2)].Merge = true;
                    aprpSummarySheet.Cells[1, 1, 1, (columnCount + 2)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[1, 1, 1, (columnCount + 2)].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[1, 1, 1, (columnCount + 2)].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Row(1).Height = 30;
                    aprpSummarySheet.Cells[1, 1, 1, (columnCount + 2)].Style.WrapText = true;

                    //set the No column and region
                    aprpSummarySheet.Cells[2, 1].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[2, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[2, 1].Value = "NO";

                    aprpSummarySheet.Cells[2, 2].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[2, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[2, 2].Value = "REGION";

                    aprpSummarySheet.Cells[2, 3].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[2, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[2, 3].Value = "CITY / MUNICIPALITY";

                    aprpSummarySheet.Cells[2, 4].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[2, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[2, 4].Value = "ACTIVITY /WORKS CATEGORY";

                    aprpSummarySheet.Cells[2, 5].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[2, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[2, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[2, 5].Value = "ROAD CLASS";

                    aprpSummarySheet.Cells[2, 6].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[2, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[2, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[2, 6].Value = "ROAD SECTION";

                    aprpSummarySheet.Cells[2, 7].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[2, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[2, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[2, 7].Value = "KMS";

                    aprpSummarySheet.Cells[2, 8].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[2, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[2, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[2, 8].Value = " BUDGET FY " + printData.FinancialYear.Code;



                    //print the admin activities
                    var printGroupName = "";
                    var aprpCount = 0;
                    int aprpRowCount = 3;
                    var numberCount = 1;


                    aprpCount = 1;

                    foreach (var region in authorityRegions)
                    {
                        var regionConstituencies = region.RegionToCountys.Select(c => c.County.Constituencys);
                        List<long> constituencyIds = new List<long>();
                        List<Constituency> constituencies = new List<Constituency>();
                        List<County> regionCounties = new List<County>();
                        List<RoadWorkSectionPlan> regionWorkplans = new List<RoadWorkSectionPlan>();
                        foreach (var constituency in regionConstituencies)
                        {
                            foreach (var consti in constituency)
                            {
                                constituencyIds.Add(consti.ID);
                                constituencies.Add(consti);
                                if (!regionCounties.Contains(consti.County))
                                {
                                    regionCounties.Add(consti.County);
                                }
                            }
                        }

                        if (constituencyIds != null)
                        {
                            if (constituencyIds.Count > 0)
                            {
                                regionWorkplans = printData.RoadWorkSectionPlans.Where(s => constituencyIds.Any(c => c == (s.RoadSection.ConstituencyId != null ? (long)s.RoadSection.ConstituencyId : 0))).ToList();
                            }

                        }



                        aprpSummarySheet.Cells[aprpRowCount, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        aprpSummarySheet.Cells[aprpRowCount, 1].Value = aprpCount++;

                        aprpSummarySheet.Cells[aprpRowCount, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        aprpSummarySheet.Cells[aprpRowCount, 2].Value = region.Name.ToUpper();

                        //aprpRowCount++;
                        //find the workcategories in the region
                        if (regionWorkplans != null)
                        {
                            if (regionWorkplans.Any())
                            {
                                //get the counties in the region
                                foreach (var county in regionCounties)
                                {
                                    if (county.Municipalitys != null)
                                    {
                                        foreach (var municipality in county.Municipalitys)
                                        {
                                            aprpSummarySheet.Cells[aprpRowCount, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                            aprpSummarySheet.Cells[aprpRowCount, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            aprpSummarySheet.Cells[aprpRowCount, 3].Value = municipality.Name.ToUpper();

                                            if (regionWorkplans.Where(c => c.RoadSection.Municipality.ID == municipality.ID).Any())
                                            {
                                                foreach (var plan in regionWorkplans.Where(c => c.RoadSection.Municipality.ID == municipality.ID).OrderBy(c => c.WorkCategory.Code))
                                                {
                                                    //output summaries only on the specific cells
                                                    //aprpSummarySheet.Cells[aprpRowCount, 4].Style.Font.Bold = true;
                                                    aprpSummarySheet.Cells[aprpRowCount, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                                    aprpSummarySheet.Cells[aprpRowCount, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                    aprpSummarySheet.Cells[aprpRowCount, 4].Value = plan.WorkCategory.Name;

                                                    aprpSummarySheet.Cells[aprpRowCount, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                                    aprpSummarySheet.Cells[aprpRowCount, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                    aprpSummarySheet.Cells[aprpRowCount, 5].Value = plan.Road.RoadNumber;


                                                    aprpSummarySheet.Cells[aprpRowCount, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                                    aprpSummarySheet.Cells[aprpRowCount, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                    aprpSummarySheet.Cells[aprpRowCount, 6].Value = plan.RoadSection.SectionName;

                                                    aprpSummarySheet.Cells[aprpRowCount, 7].Style.Font.Bold = true;
                                                    aprpSummarySheet.Cells[aprpRowCount, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                                    aprpSummarySheet.Cells[aprpRowCount, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                    aprpSummarySheet.Cells[aprpRowCount, 7].Value = plan.PlannedLength;
                                                    aprpSummarySheet.Cells[aprpRowCount, 7].Style.Numberformat.Format = "#,##0.00";

                                                    aprpSummarySheet.Cells[aprpRowCount, 8].Style.Font.Bold = true;
                                                    aprpSummarySheet.Cells[aprpRowCount, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                                    aprpSummarySheet.Cells[aprpRowCount, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                                    aprpSummarySheet.Cells[aprpRowCount, 8].Value = plan.TotalEstimateCost;
                                                    aprpSummarySheet.Cells[aprpRowCount, 8].Style.Numberformat.Format = "#,##0.00";

                                                    aprpRowCount++;
                                                }
                                            }
                                            else
                                            {
                                                aprpSummarySheet.Cells[aprpRowCount, 4, aprpRowCount, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                                aprpSummarySheet.Cells[aprpRowCount, 4, aprpRowCount, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                            }


                                            aprpRowCount++;
                                        }
                                    }


                                }
                            }
                        }


                        aprpSummarySheet.Cells[aprpRowCount, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        aprpSummarySheet.Cells[aprpRowCount, 2].Style.Font.Bold = true;
                        aprpSummarySheet.Cells[aprpRowCount, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        aprpSummarySheet.Cells[aprpRowCount, 2].Value = region.Name.ToUpper() + " TOTAL";

                        aprpSummarySheet.Cells[aprpRowCount, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        aprpSummarySheet.Cells[aprpRowCount, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        aprpSummarySheet.Cells[aprpRowCount, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        aprpSummarySheet.Cells[aprpRowCount, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        aprpSummarySheet.Cells[aprpRowCount, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        aprpSummarySheet.Cells[aprpRowCount, 7].Value = regionWorkplans.Sum(c => c.PlannedLength);
                        aprpSummarySheet.Cells[aprpRowCount, 7].Style.Numberformat.Format = "#,##0.00";

                        aprpSummarySheet.Cells[aprpRowCount, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        aprpSummarySheet.Cells[aprpRowCount, 8].Value = regionWorkplans.Sum(c => c.TotalEstimateCost);
                        aprpSummarySheet.Cells[aprpRowCount, 8].Style.Numberformat.Format = "#,##0.00";

                        aprpRowCount += 2;

                        //reset the values per region
                        regionConstituencies = null;
                        constituencyIds = null;
                        regionWorkplans = null;
                    }

                    aprpSummarySheet.Cells.AutoFitColumns();

                    //END APRP SHEET

                    excel.Workbook.Properties.Title = "Attempts";
                    _cache.Set(handle, excel.GetAsByteArray(),
                                    new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));
                    /*  using (var stream = new MemoryStream())
                      {
                          excel.SaveAs(stream);
                          _cache.Set(handle, excel.GetAsByteArray(),
                                              new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));
                      }*/

                    return Json(new
                    {
                        Success = true,
                        Message = "Success",
                        Href = Url.Action("Download", "Workplan", new { fileGuid = handle, FileName = printData.Authority.Code + "_APRP_" + printData.FinancialYear.Code + ".xlsx" })
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"WorkplanController.GetAPRPForAgency API Error {ex.Message}");
                return Json(new
                {
                    Success = false,
                    Message = "Error occured"
                });
            }

        }

        public async Task<IActionResult> GetAPRPForKws(long authorityId, long financialYearId, long regionId, long countyId)
        {
            try
            {
                //retrieve the records to be printed
                var printData = new AWRPViewModel();
                if (financialYearId > 0)
                {
                    //get the supplied financial year
                    var financialYearResp = await _financialYearService.FindByIdAsync((long)financialYearId).ConfigureAwait(false);
                    if (financialYearResp.Success)
                    {
                        printData.FinancialYear = financialYearResp.FinancialYear;
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "The financial year supplied could not be retrieved."
                        });
                    }
                }
                else
                {
                    //get the current financial year
                    var financialYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
                    if (financialYearResp.Success)
                        printData.FinancialYear = financialYearResp.FinancialYear;
                }




                //get the Authority to print for
                var authorityResp = await _authorityService.FindByIdAsync(authorityId != 0 ? authorityId : countyId).ConfigureAwait(false);
                if (!authorityResp.Success)
                {
                    return Json(new { Success = false, Message = "The authority supplied is incorrect, please choose the right authority." });
                }
                var authority = authorityResp.Authority;
                printData.Authority = authority;

                var ceilingResp = await _budgetCeilingService.FindApprovedByAuthorityForCurrentYear(printData.FinancialYear.ID, printData.Authority.ID).ConfigureAwait(false);
                printData.BudgetCeiling = ceilingResp.BudgetCeiling;

                //get the budget details for the authority
                var budgetHeaderResp = await _budgetCeilingHeaderService.FindCurrentAsync().ConfigureAwait(false);
                if (budgetHeaderResp.Success)
                    printData.BudgetCeilingHeader = budgetHeaderResp.BudgetCeilingHeader;

                //get the administrative activities for the authority
                var authorityAdminActivities = await _adminOperationalActivityService.ListByAuthorityAsync(printData.Authority.ID, printData.FinancialYear.ID).ConfigureAwait(false);
                printData.AdminOperationalActivities = authorityAdminActivities.ToList();
                //get the current workplans for the authority
                var authorityWorkplans = await _roadWorkSectionPlanService.ListByAgencyAsync(authority.ID, printData.FinancialYear.ID).ConfigureAwait(false);
                if (!authorityWorkplans.Any())
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "There are no records for the selected county in the selected financial year that could be retrieved."
                    });
                }
                printData.RoadWorkSectionPlans = authorityWorkplans.ToList();

                //generate the file.
                string handle = Guid.NewGuid().ToString();
                var memoryStream = new MemoryStream();
                using (ExcelPackage excel = new ExcelPackage(memoryStream))
                {
                    string currentFileName = System.IO.Path.GetFileName("Kerra_APRP");


                    var workTypeSummarySheet = excel.Workbook.Worksheets.Add("GRAND SUMMARY");
                    var aprpSummarySheet = excel.Workbook.Worksheets.Add("1% ARRP FORMAT");

                    //WORKS CATEGORY SUMMARY
                    int rowCatSum = 3;
                    int countCat = 1;
                    var authorityRegions = await _regionService.ListRegionsByAuthority(printData.Authority.ID).ConfigureAwait(false);

                    var worksCategorySummary = printData.RoadWorkSectionPlans.GroupBy(p => p.WorkCategory).Select(
                            cat => new
                            {
                                Key = cat.Key,
                                Value = cat.First().WorkCategory.Name,
                                Code = cat.First().WorkCategory.Code,
                                KMs = cat.Sum(c => c.RoadSection.Length),
                                Budget = cat.Sum(p => p.TotalEstimateCost)
                            }
                        );

                    var groupedActivties = printData.AdminOperationalActivities.GroupBy(g => g.ActivityGroup).Select(
                      g => new
                      {
                          Key = g.Key,
                          Value = g.Sum(s => s.Amount),
                          Name = g.First().ActivityGroup
                      }
                    );

                    //print admin activities
                    workTypeSummarySheet.Cells["A1:E1"].Value = "KENYA WILD LIFE SERVICES ANNUAL ROADS WORKS PROGRAMME (ARWP) FOR FINANCIAL " + printData.FinancialYear.Code + ". GRAND SUMMARY";
                    workTypeSummarySheet.Cells["A1:E1"].Merge = true;
                    workTypeSummarySheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workTypeSummarySheet.Cells["A1:E1"].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells["A1:E1"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Row(1).Height = 30;
                    workTypeSummarySheet.Cells["A1:E1"].Style.WrapText = true;


                    workTypeSummarySheet.Cells[2, 1].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[2, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workTypeSummarySheet.Cells[2, 1].Value = "NO.";

                    workTypeSummarySheet.Cells[2, 2].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[2, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workTypeSummarySheet.Cells[2, 2].Value = "REGION";

                    workTypeSummarySheet.Cells[2, 3].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[2, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workTypeSummarySheet.Cells[2, 3].Value = "WORKS CATEGORY";

                    workTypeSummarySheet.Cells[2, 4].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[2, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workTypeSummarySheet.Cells[2, 4].Value = "KMS ";

                    workTypeSummarySheet.Cells[2, 5].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[2, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[2, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workTypeSummarySheet.Cells[2, 5].Value = "BUDGET FY " + printData.FinancialYear.Code;

                    int rowCountSum = 3;
                    int countyCount = 1;

                    //print works category summary
                    foreach (var region in authorityRegions)
                    {
                        List<KWSPark> parks = new List<KWSPark>();
                        parks = region.KWSParks.ToList();
                        var regionWorkplans = printData.RoadWorkSectionPlans.Where(s => parks.Any(c => c.ID == (long)s.RoadSection.KWSParkId));


                        workTypeSummarySheet.Cells[rowCountSum, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        workTypeSummarySheet.Cells[rowCountSum, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workTypeSummarySheet.Cells[rowCountSum, 1].Value = countyCount++;

                        workTypeSummarySheet.Cells[rowCountSum, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        workTypeSummarySheet.Cells[rowCountSum, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workTypeSummarySheet.Cells[rowCountSum, 2].Value = region.Name.ToUpper();

                        // output the workplans in the constituency summaries
                        foreach (var cat in worksCategorySummary)
                        {
                            //output summaries only on the specific cells
                            workTypeSummarySheet.Cells[rowCountSum, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            workTypeSummarySheet.Cells[rowCountSum, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            workTypeSummarySheet.Cells[rowCountSum, 3].Value = cat.Value.ToUpper();

                            workTypeSummarySheet.Cells[rowCountSum, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            workTypeSummarySheet.Cells[rowCountSum, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            workTypeSummarySheet.Cells[rowCountSum, 4].Value = (double)regionWorkplans.Where(c => c.WorkCategory.Code == cat.Code).Sum(c => c.PlannedLength);
                            workTypeSummarySheet.Cells[rowCountSum, 4].Style.Numberformat.Format = "#,##0.00";

                            workTypeSummarySheet.Cells[rowCountSum, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            workTypeSummarySheet.Cells[rowCountSum, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            workTypeSummarySheet.Cells[rowCountSum, 5].Value = (double)regionWorkplans.Where(c => c.WorkCategory.Code == cat.Code).Sum(c => c.TotalEstimateCost);
                            workTypeSummarySheet.Cells[rowCountSum, 5].Style.Numberformat.Format = "#,##0.00";

                            rowCountSum++;

                        }
                    }



                    //grand total
                    workTypeSummarySheet.Cells[rowCountSum, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[rowCountSum, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    workTypeSummarySheet.Cells[rowCountSum, 2].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[rowCountSum, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[rowCountSum, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workTypeSummarySheet.Cells[rowCountSum, 2].Value = "GRAND TOTAL";

                    workTypeSummarySheet.Cells[rowCountSum, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[rowCountSum, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    workTypeSummarySheet.Cells[rowCountSum, 4].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[rowCountSum, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[rowCountSum, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workTypeSummarySheet.Cells[rowCountSum, 4].Value = printData.RoadWorkSectionPlans.Sum(s => s.PlannedLength);//get summary of workplans in that region
                    workTypeSummarySheet.Cells[rowCountSum, 4].Style.Numberformat.Format = "#,##0.00";

                    workTypeSummarySheet.Cells[rowCountSum, 5].Style.Font.Bold = true;
                    workTypeSummarySheet.Cells[rowCountSum, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    workTypeSummarySheet.Cells[rowCountSum, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workTypeSummarySheet.Cells[rowCountSum, 5].Value = printData.RoadWorkSectionPlans.Sum(p => p.TotalEstimateCost);//get summary of workplans in that region
                    workTypeSummarySheet.Cells[rowCountSum, 5].Style.Numberformat.Format = "#,##0.00";

                    workTypeSummarySheet.Cells.AutoFitColumns();

                    //END WORKS CATEGORY SUMMARY

                    //BEGIN APRP SHEET


                    //print admin activities
                    aprpSummarySheet.Cells[1, 1, 1, 8].Value = "1% KWS WORK PLAN FINANCIAL YEAR " + printData.FinancialYear.Code;

                    //worksheet.Cells["A1:B5"].Merge = true; =   workTypeSummarySheet.Cells[1, 1, 5, 2]

                    aprpSummarySheet.Cells[1, 1, 1, 8].Merge = true;
                    aprpSummarySheet.Cells[1, 1, 1, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[1, 1, 1, 8].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[1, 1, 1, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Row(1).Height = 30;
                    aprpSummarySheet.Cells[1, 1, 1, 8].Style.WrapText = true;


                    //set the No column and region
                    aprpSummarySheet.Cells[2, 1].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[2, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[2, 1].Value = "NO";

                    aprpSummarySheet.Cells[2, 2].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[2, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[2, 2].Value = "NATIONAL PARK";

                    aprpSummarySheet.Cells[2, 3].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[2, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[2, 3].Value = "ACTIVITY / WORK CATEGORY";

                    aprpSummarySheet.Cells[2, 4].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[2, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[2, 4].Value = "ROAD NO.";

                    aprpSummarySheet.Cells[2, 5].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[2, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[2, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[2, 5].Value = "ROAD NAME";

                    aprpSummarySheet.Cells[2, 6].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[2, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[2, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[2, 6].Value = "ST";

                    aprpSummarySheet.Cells[2, 7].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[2, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[2, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[2, 7].Value = "KMS";

                    aprpSummarySheet.Cells[2, 8].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[2, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[2, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[2, 8].Value = " AMOUNT ";




                    //print the admin activities
                    var printGroupName = "";
                    var aprpCount = 0;
                    int aprpRowCount = 3;
                    var numberCount = 1;

                    aprpCount = 1;

                    foreach (var region in authorityRegions)
                    {
                        List<KWSPark> parks = new List<KWSPark>();
                        parks = region.KWSParks.ToList();
                        var regionWorkplans = printData.RoadWorkSectionPlans.Where(s => parks.Any(c => c.ID == (long)s.RoadSection.KWSParkId));

                        aprpSummarySheet.Cells[aprpRowCount, 1, aprpRowCount, 8].Style.Font.Bold = true;
                        aprpSummarySheet.Cells[aprpRowCount, 1, aprpRowCount, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        aprpSummarySheet.Cells[aprpRowCount, 1, aprpRowCount, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        aprpSummarySheet.Cells[aprpRowCount, 1, aprpRowCount, 8].Merge = true;
                        aprpSummarySheet.Cells[aprpRowCount, 1, aprpRowCount, 8].Value = region.Name.ToUpper() + " REGION";

                        aprpRowCount++;
                        //find the workcategories in the region
                        if (regionWorkplans != null)
                        {
                            if (regionWorkplans.Any())
                            {
                                foreach (var park in parks)
                                {
                                    //aprpSummarySheet.Cells[aprpRowCount, 4].Style.Font.Bold = true;
                                    aprpSummarySheet.Cells[aprpRowCount, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    aprpSummarySheet.Cells[aprpRowCount, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    aprpSummarySheet.Cells[aprpRowCount, 2].Value = park.NationalPark.ToUpper();
                                    aprpSummarySheet.Cells[aprpRowCount, 2].Merge = true;

                                    foreach (var plan in regionWorkplans.Where(p => p.RoadSection.KWSPark.ID == park.ID).OrderBy(c => c.WorkCategory.Code))
                                    {
                                        aprpSummarySheet.Cells[aprpRowCount, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                        aprpSummarySheet.Cells[aprpRowCount, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        aprpSummarySheet.Cells[aprpRowCount, 1].Value = numberCount++;
                                        aprpSummarySheet.Cells[aprpRowCount, 1].Style.Numberformat.Format = "0";
                                        //output summaries only on the specific cells
                                        //aprpSummarySheet.Cells[aprpRowCount, 4].Style.Font.Bold = true;

                                        aprpSummarySheet.Cells[aprpRowCount, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                        aprpSummarySheet.Cells[aprpRowCount, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        aprpSummarySheet.Cells[aprpRowCount, 3].Value = plan.WorkCategory.Name;

                                        aprpSummarySheet.Cells[aprpRowCount, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                        aprpSummarySheet.Cells[aprpRowCount, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        aprpSummarySheet.Cells[aprpRowCount, 4].Value = plan.Road.RoadNumber;

                                        aprpSummarySheet.Cells[aprpRowCount, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                        aprpSummarySheet.Cells[aprpRowCount, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        aprpSummarySheet.Cells[aprpRowCount, 5].Value = plan.RoadSection.SectionName;

                                        aprpSummarySheet.Cells[aprpRowCount, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                        aprpSummarySheet.Cells[aprpRowCount, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        aprpSummarySheet.Cells[aprpRowCount, 6].Value = (plan.RoadSection.SurfaceType != null ? plan.RoadSection.SurfaceType.Code : "");

                                        aprpSummarySheet.Cells[aprpRowCount, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                        aprpSummarySheet.Cells[aprpRowCount, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        aprpSummarySheet.Cells[aprpRowCount, 7].Value = plan.PlannedLength;
                                        aprpSummarySheet.Cells[aprpRowCount, 7].Style.Numberformat.Format = "#,##0.00";

                                        aprpSummarySheet.Cells[aprpRowCount, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                        aprpSummarySheet.Cells[aprpRowCount, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                        aprpSummarySheet.Cells[aprpRowCount, 8].Value = plan.TotalEstimateCost;
                                        aprpSummarySheet.Cells[aprpRowCount, 8].Style.Numberformat.Format = "#,##0.00";

                                        aprpRowCount++;
                                    }

                                    aprpSummarySheet.Cells[aprpRowCount, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    aprpSummarySheet.Cells[aprpRowCount, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    aprpSummarySheet.Cells[aprpRowCount, 1].Value = numberCount++;
                                    aprpSummarySheet.Cells[aprpRowCount, 1].Style.Numberformat.Format = "0";

                                    aprpSummarySheet.Cells[aprpRowCount, 2, aprpRowCount, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    aprpSummarySheet.Cells[aprpRowCount, 2, aprpRowCount, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                    aprpSummarySheet.Cells[aprpRowCount, 7].Style.Font.Bold = true;
                                    aprpSummarySheet.Cells[aprpRowCount, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    aprpSummarySheet.Cells[aprpRowCount, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    aprpSummarySheet.Cells[aprpRowCount, 7].Value = regionWorkplans.Where(p => p.RoadSection.KWSPark.ID == park.ID).Sum(c => c.PlannedLength);
                                    aprpSummarySheet.Cells[aprpRowCount, 7].Style.Numberformat.Format = "#,##0.00";

                                    aprpSummarySheet.Cells[aprpRowCount, 8].Style.Font.Bold = true;
                                    aprpSummarySheet.Cells[aprpRowCount, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                    aprpSummarySheet.Cells[aprpRowCount, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    aprpSummarySheet.Cells[aprpRowCount, 8].Value = regionWorkplans.Where(p => p.RoadSection.KWSPark.ID == park.ID).Sum(c => c.TotalEstimateCost);
                                    aprpSummarySheet.Cells[aprpRowCount, 8].Style.Numberformat.Format = "#,##0.00";

                                    aprpRowCount++;
                                }


                            }
                        }


                    }

                    aprpSummarySheet.Cells[aprpRowCount, 1, aprpRowCount, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[aprpRowCount, 1, aprpRowCount, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    aprpSummarySheet.Cells[aprpRowCount, 5, aprpRowCount, 6].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[aprpRowCount, 5, aprpRowCount, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[aprpRowCount, 5, aprpRowCount, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[aprpRowCount, 5, aprpRowCount, 6].Value = "GRAND TOTAL ";

                    aprpSummarySheet.Cells[aprpRowCount, 7].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[aprpRowCount, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[aprpRowCount, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[aprpRowCount, 7].Value = printData.RoadWorkSectionPlans.Sum(c => c.PlannedLength);
                    aprpSummarySheet.Cells[aprpRowCount, 7].Style.Numberformat.Format = "#,##0.00";

                    aprpSummarySheet.Cells[aprpRowCount, 8].Style.Font.Bold = true;
                    aprpSummarySheet.Cells[aprpRowCount, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    aprpSummarySheet.Cells[aprpRowCount, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    aprpSummarySheet.Cells[aprpRowCount, 8].Value = printData.RoadWorkSectionPlans.Sum(c => c.TotalEstimateCost);
                    aprpSummarySheet.Cells[aprpRowCount, 8].Style.Numberformat.Format = "#,##0.00";


                    aprpSummarySheet.Cells.AutoFitColumns();

                    //END APRP SHEET

                    excel.Workbook.Properties.Title = "Attempts";
                    _cache.Set(handle, excel.GetAsByteArray(),
                                    new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));
                    /*  using (var stream = new MemoryStream())
                      {
                          excel.SaveAs(stream);
                          _cache.Set(handle, excel.GetAsByteArray(),
                                              new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));
                      }*/

                    return Json(new
                    {
                        Success = true,
                        Message = "Success",
                        Href = Url.Action("Download", "Workplan", new { fileGuid = handle, FileName = printData.Authority.Code + "_APRP_" + printData.FinancialYear.Code + ".xlsx" })
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"WorkplanController.GetAPRPForAgency API Error {ex.Message}");
                return Json(new
                {
                    Success = false,
                    Message = "Error occured"
                });
            }

        }

        public async Task<IActionResult> GetCombinedARWPForAgency(long authorityId, long? financialYearId, long regionId, long countyId)
        {
            try
            {

                //retrieve the records to be printed
                //get the current financial year
                var printData = new AWRPViewModel();


                //get the Authority to print for
                //var auth_id = ;
                var authorityResp = await _authorityService.FindByIdAsync(authorityId != 0 ? authorityId : countyId).ConfigureAwait(false);
                if (!authorityResp.Success)
                {
                    return Json(new { Success = false, Message = "The authority supplied is incorrect, please choose the right authority." });
                }
                var authority = authorityResp.Authority;
                printData.Authority = authority;

                FinancialYear financialYear = new FinancialYear();

                if (financialYearId != null && financialYearId > 0)
                {

                    var fYearResp = await _financialYearService.FindByIdAsync((long)financialYearId).ConfigureAwait(false);
                    if (fYearResp.Success)
                        financialYear = fYearResp.FinancialYear;
                }
                else
                {
                    var fYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
                    if (fYearResp.Success)
                    {
                        financialYear = fYearResp.FinancialYear;
                    }
                    else
                    {
                        return Json(new { Success = false, Message = "The financial year supplied for record retrieval could not be found!" });
                    }

                }

                //get the budget details for the authority
                //get budgets for the authority
                var totalBudget = 0.0;
                var budgetHeaderResp = await _budgetCeilingHeaderService.FindCurrentAsync().ConfigureAwait(false);
                if (budgetHeaderResp.Success)
                    printData.BudgetCeilingHeader = budgetHeaderResp.BudgetCeilingHeader;

                var ceilingResp = await _budgetCeilingService.FindApprovedByAuthorityForCurrentYear(financialYear.ID, authority.ID).ConfigureAwait(false);
                if (ceilingResp.Success)
                {
                    totalBudget += ceilingResp.BudgetCeiling.Amount;
                }

                //get the agency budget allocation for other funding source

                var budgetTotalOthers = await _revenueCollectionCodeUnitService.ListAsync(financialYear.ID, "Others").ConfigureAwait(false);
                if (budgetTotalOthers != null)
                {
                    totalBudget += budgetTotalOthers.RevenueCollectionCodeUnit.Where(a => a.AuthorityId == authority.ID).Sum(a => a.RevenueCollection.Amount);
                }

                //get the current workplans for the authority
                var authorityWorkplans = await _roadWorkSectionPlanService.ListByAgencyAsync(authority.ID, financialYear.ID).ConfigureAwait(false);
                List<RoadWorkSectionPlan> roadWorkSectionPlans = new List<RoadWorkSectionPlan>();


                if (regionId != 0)
                {
                    var region = await _regionService.GetRegionAsync(regionId).ConfigureAwait(false);
                    //get all constituencies in the region
                    List<long> constituencyIds = new List<long>();
                    List<long> countyIds = new List<long>();
                    //get the counties in the region
                    foreach (RegionToCounty regionToCounty in region.Region.RegionToCountys)
                    {
                        //loop through the constituencies to get the road ID
                        foreach (Constituency constituency in regionToCounty.County.Constituencys)
                        {
                            //get the county constitutencies
                            foreach (RoadWorkSectionPlan roadWorkSectionPlan in authorityWorkplans.ToList())
                            {

                                //chech the road section constituency if is same as region constituencies.
                                var roadSection = roadWorkSectionPlan.RoadSection;
                                if (roadSection.ConstituencyId == constituency.ID)
                                {
                                    //remove the workplan
                                    roadWorkSectionPlans.Add(roadWorkSectionPlan);
                                }
                            }
                        }


                    }
                    //return the filtered list
                    printData.RoadWorkSectionPlans = roadWorkSectionPlans;
                }
                else
                {
                    printData.RoadWorkSectionPlans = authorityWorkplans.ToList();
                }

                //check if there is any workplan to print
                if (!printData.RoadWorkSectionPlans.Any())
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "We did not find any workplans to load for download. Please check with the administrator."
                    });
                }

                //generate the file.
                string handle = Guid.NewGuid().ToString();
                var memoryStream = new MemoryStream();
                using (ExcelPackage excel = new ExcelPackage(memoryStream))
                {
                    string currentFileName = "ARWP_Summary_" + printData.Authority.Code + "_" + financialYear.Code + ".xlsx";
                    // string currentFileName = System.IO.Path.GetFileName(fileName);
                    var sheetcreateAPRP = excel.Workbook.Worksheets.Add("ARWP Summary");
                    var sheetcreateARWP = excel.Workbook.Worksheets.Add("ARWP Details");



                    //Begin ARWP File Creation
                    //print the column headers
                    //First Row
                    sheetcreateARWP.Cells[1, 1].Value = "ANNUAL ROAD WORKS PROGRAMME ";
                    sheetcreateARWP.Cells[1, 1].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[1, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[1, 2].Value = financialYear.Code;
                    sheetcreateARWP.Cells[1, 2].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[1, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;



                    //Second Row
                    if (authority.Type == 1)
                    {
                        sheetcreateARWP.Cells[2, 1].Value = "NAME OF AGENCY ";
                        sheetcreateARWP.Cells[2, 1].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[2, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[2, 2].Value = printData.Authority.Name;
                        sheetcreateARWP.Cells[2, 2].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[2, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }
                    else
                    {
                        sheetcreateARWP.Cells[2, 1].Value = "NAME OF COUNTY GOVERNMENT ";
                        sheetcreateARWP.Cells[2, 1].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[2, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[2, 2].Value = printData.Authority.Name;
                        sheetcreateARWP.Cells[2, 2].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[2, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }

                    //Third Row
                    if (authority.Type == 1)
                    {
                        sheetcreateARWP.Cells[3, 1].Value = "AGENCY CODE ";
                        sheetcreateARWP.Cells[3, 1].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[3, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[3, 2].Value = printData.Authority.Code;
                        sheetcreateARWP.Cells[3, 2].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[3, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }
                    else
                    {
                        sheetcreateARWP.Cells[3, 1].Value = "COUNTY CODE ";
                        sheetcreateARWP.Cells[3, 1].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[3, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[3, 2].Value = printData.Authority.Code;
                        sheetcreateARWP.Cells[3, 2].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[3, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }

                    //Fourth Row

                    sheetcreateARWP.Cells[4, 1].Value = "BUDGET (KSHS) ";
                    sheetcreateARWP.Cells[4, 1].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[4, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[4, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[4, 2].Value = totalBudget.ToString("N");
                    sheetcreateARWP.Cells[4, 2].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[4, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                    //create now the record items
                    //print headers
                    sheetcreateARWP.Cells[6, 1].Value = "ITEM NO.";
                    sheetcreateARWP.Cells[6, 1].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[6, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[6, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[6, 2].Value = "ROAD NUMBER";
                    sheetcreateARWP.Cells[6, 2].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[6, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[6, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[6, 3].Value = "SECTION/ROAD NAME";
                    sheetcreateARWP.Cells[6, 3].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[6, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[6, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[6, 4].Value = "WORK CATEGORY";
                    sheetcreateARWP.Cells[6, 4].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[6, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[6, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[6, 5].Value = "SURFACE TYPE";
                    sheetcreateARWP.Cells[6, 5].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[6, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[6, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[6, 6].Value = "LENGTH";
                    sheetcreateARWP.Cells[6, 6].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[6, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[6, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[6, 7].Value = "ACTIVITY CODE";
                    sheetcreateARWP.Cells[6, 7].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[6, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[6, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[6, 8].Value = "ACTIVITY DESCRIPTION";
                    sheetcreateARWP.Cells[6, 8].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[6, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[6, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    /* sheetcreate.Cells[6, 8].Value = "WORKS CATEGORY";
                     sheetcreate.Cells[6, 8].Style.Font.Bold = true;
                     sheetcreate.Cells[6, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                     sheetcreate.Cells[6, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
 */
                    sheetcreateARWP.Cells[6, 9].Value = "METHOD";
                    sheetcreateARWP.Cells[6, 9].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[6, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[6, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[6, 10].Value = "UNIT MEASURE";
                    sheetcreateARWP.Cells[6, 10].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[6, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[6, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[6, 11].Value = "PLANNED QUANTITY";
                    sheetcreateARWP.Cells[6, 11].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[6, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[6, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[6, 12].Value = "PLANNED RATE";
                    sheetcreateARWP.Cells[6, 12].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[6, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[6, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[6, 13].Value = "AMOUNT (KSH)";
                    sheetcreateARWP.Cells[6, 13].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[6, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[6, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    int row = 7;
                    int itemNo = 1;
                    double total = 0;
                    foreach (var workplan in printData.RoadWorkSectionPlans.OrderBy(n => n.Road.RoadNumber))
                    {

                        double subTotal = 0;
                        // now set the individual activities
                        sheetcreateARWP.Cells[row, 1].Value = itemNo;
                        sheetcreateARWP.Cells[row, 1].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 2].Value = workplan.Road.RoadNumber;
                        sheetcreateARWP.Cells[row, 2].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 3].Value = workplan.RoadSection.SectionName;
                        sheetcreateARWP.Cells[row, 3].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 4].Value = (workplan.WorkCategory != null ? workplan.WorkCategory.Name : "");
                        sheetcreateARWP.Cells[row, 4].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 5].Value = (workplan.RoadSection.SurfaceType != null ? workplan.RoadSection.SurfaceType.Description : "");
                        sheetcreateARWP.Cells[row, 5].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 6].Value = workplan.RoadSection.Length;
                        sheetcreateARWP.Cells[row, 6].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        if (workplan.PlanActivities.Any())
                        {
                            foreach (var plan in workplan.PlanActivities)
                            {
                                // sheetcreate.Cells[row, 1].Value = "";
                                sheetcreateARWP.Cells[row, 1].Style.Font.Bold = true;
                                sheetcreateARWP.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                // sheetcreate.Cells[row, 2].Value = "";
                                sheetcreateARWP.Cells[row, 2].Style.Font.Bold = true;
                                sheetcreateARWP.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                // sheetcreate.Cells[row, 3].Value = "";
                                sheetcreateARWP.Cells[row, 3].Style.Font.Bold = true;
                                sheetcreateARWP.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                // sheetcreate.Cells[row, 4].Value ="";
                                sheetcreateARWP.Cells[row, 4].Style.Font.Bold = true;
                                sheetcreateARWP.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 5].Style.Font.Bold = true;
                                sheetcreateARWP.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 6].Style.Font.Bold = true;
                                sheetcreateARWP.Cells[row, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 7].Value = plan.ItemActivityUnitCost.ItemCode + "-" + plan.ItemActivityUnitCost.SubItemCode + "-" + plan.ItemActivityUnitCost.SubSubItemCode;
                                sheetcreateARWP.Cells[row, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 8].Value = plan.ItemActivityUnitCost.Description;
                                sheetcreateARWP.Cells[row, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 9].Value = plan.Technology != null ? plan.Technology.Code : "";
                                sheetcreateARWP.Cells[row, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 10].Value = plan.ItemActivityUnitCost.UnitMeasure;
                                sheetcreateARWP.Cells[row, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 11].Value = plan.Quantity;
                                sheetcreateARWP.Cells[row, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 12].Value = plan.Rate;
                                sheetcreateARWP.Cells[row, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 13].Value = plan.Amount.ToString("N");
                                sheetcreateARWP.Cells[row, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                row++;

                                total = (total + plan.Amount);
                                subTotal += plan.Amount;

                            }
                        }


                        //PBC Activities
                        if (workplan.PlanActivityPBCs.Any())
                        {
                            foreach (var plan in workplan.PlanActivityPBCs)
                            {
                                // sheetcreate.Cells[row, 1].Value = "";
                                sheetcreateARWP.Cells[row, 1].Style.Font.Bold = true;
                                sheetcreateARWP.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                // sheetcreate.Cells[row, 2].Value = "";
                                sheetcreateARWP.Cells[row, 2].Style.Font.Bold = true;
                                sheetcreateARWP.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                // sheetcreate.Cells[row, 3].Value = "";
                                sheetcreateARWP.Cells[row, 3].Style.Font.Bold = true;
                                sheetcreateARWP.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                // sheetcreate.Cells[row, 4].Value ="";
                                sheetcreateARWP.Cells[row, 4].Style.Font.Bold = true;
                                sheetcreateARWP.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 5].Style.Font.Bold = true;
                                sheetcreateARWP.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 6].Style.Font.Bold = true;
                                sheetcreateARWP.Cells[row, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 7].Value = plan.ItemActivityPBC.Code;
                                sheetcreateARWP.Cells[row, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 8].Value = plan.ItemActivityPBC.Description;
                                sheetcreateARWP.Cells[row, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 9].Value = plan.ItemActivityPBC.Technology != null ? plan.ItemActivityPBC.Technology.Code : "";
                                sheetcreateARWP.Cells[row, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 10].Value = "KM Per Month";
                                sheetcreateARWP.Cells[row, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 11].Value = plan.PlannedKM;
                                sheetcreateARWP.Cells[row, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 12].Value = plan.CostPerKMPerMonth;
                                sheetcreateARWP.Cells[row, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 13].Value = plan.TotalAmount.ToString("N");
                                sheetcreateARWP.Cells[row, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                row++;

                                total = (total + plan.TotalAmount);
                                subTotal += plan.TotalAmount;

                            }
                        }



                        //set subtotal
                        row = row + 1;
                        sheetcreateARWP.Cells[row, 1].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 2].Value = "";
                        sheetcreateARWP.Cells[row, 2].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 3].Value = "";
                        sheetcreateARWP.Cells[row, 3].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 4].Value ="";
                        sheetcreateARWP.Cells[row, 4].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                        //sheetcreate.Cells[row, 5].Value = plan.ItemActivityUnitCost.SubSubItemCode;
                        sheetcreateARWP.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 6].Value = plan.ItemActivityUnitCost.Description;
                        sheetcreateARWP.Cells[row, 7].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 7].Value = plan.Technology;
                        sheetcreateARWP.Cells[row, 8].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 8].Value = plan.ItemActivityUnitCost.UnitMeasure;
                        sheetcreateARWP.Cells[row, 9].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 10].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 11].Value = "Sub Total (KSH)";
                        sheetcreateARWP.Cells[row, 11].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 12].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 13].Value = subTotal.ToString("N");
                        sheetcreateARWP.Cells[row, 13].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //set the VAT field
                        row = row + 1;
                        sheetcreateARWP.Cells[row, 1].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 2].Value = "";
                        sheetcreateARWP.Cells[row, 2].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 3].Value = "";
                        sheetcreateARWP.Cells[row, 3].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 4].Value ="";
                        sheetcreateARWP.Cells[row, 4].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                        //sheetcreate.Cells[row, 5].Value = plan.ItemActivityUnitCost.SubSubItemCode;
                        sheetcreateARWP.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 5].Value = plan.ItemActivityUnitCost.SubSubItemCode;
                        sheetcreateARWP.Cells[row, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 6].Value = plan.ItemActivityUnitCost.Description;
                        sheetcreateARWP.Cells[row, 7].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 7].Value = plan.Technology;
                        sheetcreateARWP.Cells[row, 8].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 8].Value = plan.ItemActivityUnitCost.UnitMeasure;
                        sheetcreateARWP.Cells[row, 9].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 10].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 11].Value = "16 % VAT";
                        sheetcreateARWP.Cells[row, 11].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 12].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 13].Value = (0.16 * subTotal).ToString("N");
                        sheetcreateARWP.Cells[row, 13].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //set the Contigencies field
                        row = row + 1;
                        sheetcreateARWP.Cells[row, 1].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 2].Value = "";
                        sheetcreateARWP.Cells[row, 2].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 3].Value = "";
                        sheetcreateARWP.Cells[row, 3].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 4].Value ="";
                        sheetcreateARWP.Cells[row, 4].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                        //sheetcreate.Cells[row, 5].Value = plan.ItemActivityUnitCost.SubSubItemCode;
                        sheetcreateARWP.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 6].Value = plan.ItemActivityUnitCost.Description;
                        sheetcreateARWP.Cells[row, 7].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 7].Value = plan.Technology;
                        sheetcreateARWP.Cells[row, 8].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 8].Value = plan.ItemActivityUnitCost.UnitMeasure;
                        sheetcreateARWP.Cells[row, 9].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 10].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 11].Value = "Contigencies@0%";
                        sheetcreateARWP.Cells[row, 11].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 12].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 13].Value = "-";
                        sheetcreateARWP.Cells[row, 13].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //set total with contigencies and VAT
                        row = row + 1;
                        sheetcreateARWP.Cells[row, 1].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 2].Value = "";
                        sheetcreateARWP.Cells[row, 2].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 3].Value = "";
                        sheetcreateARWP.Cells[row, 3].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 4].Value ="";
                        sheetcreateARWP.Cells[row, 4].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                        //sheetcreate.Cells[row, 5].Value = plan.ItemActivityUnitCost.SubSubItemCode;
                        sheetcreateARWP.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 6].Value = plan.ItemActivityUnitCost.Description;
                        sheetcreateARWP.Cells[row, 7].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 7].Value = plan.Technology;
                        sheetcreateARWP.Cells[row, 8].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 8].Value = plan.ItemActivityUnitCost.UnitMeasure;
                        sheetcreateARWP.Cells[row, 9].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 10].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 11].Value = "Total";
                        sheetcreateARWP.Cells[row, 11].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 12].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 13].Value = (1.16 * subTotal).ToString("N");
                        sheetcreateARWP.Cells[row, 13].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //set empty same
                        row = row + 2;
                        sheetcreateARWP.Cells[row, 1].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 2].Value = "";
                        sheetcreateARWP.Cells[row, 2].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 3].Value = "";
                        sheetcreateARWP.Cells[row, 3].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 4].Value ="";
                        sheetcreateARWP.Cells[row, 4].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                        //sheetcreate.Cells[row, 5].Value = plan.ItemActivityUnitCost.SubSubItemCode;
                        sheetcreateARWP.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 6].Value = plan.ItemActivityUnitCost.Description;
                        sheetcreateARWP.Cells[row, 7].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 7].Value = plan.Technology;
                        sheetcreateARWP.Cells[row, 8].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 8].Value = plan.ItemActivityUnitCost.UnitMeasure;
                        sheetcreateARWP.Cells[row, 9].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 10].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 11].Value = "";
                        sheetcreateARWP.Cells[row, 11].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 12].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 13].Value = "";
                        sheetcreateARWP.Cells[row, 13].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        itemNo++;

                    }
                    sheetcreateARWP.Cells[row + 1, 11].Value = "Grand Total (KSH)";
                    sheetcreateARWP.Cells[row + 1, 11].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[row + 1, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[row + 1, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[row + 1, 12].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[row + 1, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[row + 1, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[row + 1, 13].Value = (1.16 * total).ToString("N");
                    sheetcreateARWP.Cells[row + 1, 13].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[row + 1, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[row + 1, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[row + 3, 1].Value = "Signed By";
                    sheetcreateARWP.Cells[row + 3, 1].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[row + 3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                    sheetcreateARWP.Cells[row + 3, 4].Value = "Designation";
                    sheetcreateARWP.Cells[row + 3, 4].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[row + 3, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                    sheetcreateARWP.Cells[row + 3, 7].Value = "Date";
                    sheetcreateARWP.Cells[row + 3, 7].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[row + 3, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[row + 3, 9].Value = "Sign";
                    sheetcreateARWP.Cells[row + 3, 9].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[row + 3, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells.AutoFitColumns();

                    //End ARWP Summary Creation

                    //Begin APRP Worksheet Creation
                    //First Row
                    sheetcreateAPRP.Cells[2, 2].Value = "Routine Maintainance Work Program Report";
                    sheetcreateAPRP.Cells["B2:H2"].Merge = true;
                    sheetcreateAPRP.Cells["B2:H2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    sheetcreateAPRP.Cells["B2:H2"].Style.Font.Bold = true;
                    sheetcreateAPRP.Cells["B2:H2"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    if (printData.Authority.Type == 1)
                    {
                        sheetcreateAPRP.Cells[3, 2].Value = "Authority";
                    }
                    else
                    {
                        sheetcreateAPRP.Cells[3, 2].Value = "County";
                    }

                    sheetcreateAPRP.Cells[3, 2].Style.Font.Bold = true;
                    sheetcreateAPRP.Cells[3, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateAPRP.Cells[3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateAPRP.Cells[3, 3].Value = printData.Authority.Name;
                    sheetcreateAPRP.Cells[3, 3].Style.Font.Bold = true;
                    sheetcreateAPRP.Cells[3, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateAPRP.Cells[3, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateAPRP.Cells[4, 2].Value = "Funding Source";
                    sheetcreateAPRP.Cells[4, 2].Style.Font.Bold = true;
                    sheetcreateAPRP.Cells[4, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateAPRP.Cells[4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                    sheetcreateAPRP.Cells[4, 7].Value = "Financial Year";
                    sheetcreateAPRP.Cells[4, 7].Style.Font.Bold = true;
                    sheetcreateAPRP.Cells[4, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateAPRP.Cells[4, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateAPRP.Cells[4, 8].Value = financialYear.Code;
                    sheetcreateAPRP.Cells[4, 8].Style.Font.Bold = true;
                    sheetcreateAPRP.Cells[4, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateAPRP.Cells[4, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    //create now the record items
                    //print headers
                    sheetcreateAPRP.Cells[6, 2].Value = "REGION/CONSTITUENCY";
                    sheetcreateAPRP.Cells[6, 2].Style.Font.Bold = true;
                    sheetcreateAPRP.Cells[6, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateAPRP.Cells[6, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateAPRP.Cells[6, 3].Value = "ROAD NO.";
                    sheetcreateAPRP.Cells[6, 3].Style.Font.Bold = true;
                    sheetcreateAPRP.Cells[6, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateAPRP.Cells[6, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateAPRP.Cells[6, 4].Value = "SECTION NAME";
                    sheetcreateAPRP.Cells[6, 4].Style.Font.Bold = true;
                    sheetcreateAPRP.Cells[6, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateAPRP.Cells[6, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateAPRP.Cells[6, 5].Value = "WORK CATEGORY";
                    sheetcreateAPRP.Cells[6, 5].Style.Font.Bold = true;
                    sheetcreateAPRP.Cells[6, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateAPRP.Cells[6, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateAPRP.Cells[6, 6].Value = "SURFACE TYPE";
                    sheetcreateAPRP.Cells[6, 6].Style.Font.Bold = true;
                    sheetcreateAPRP.Cells[6, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateAPRP.Cells[6, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateAPRP.Cells[6, 7].Value = "Length (Km)";
                    sheetcreateAPRP.Cells[6, 7].Style.Font.Bold = true;
                    sheetcreateAPRP.Cells[6, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateAPRP.Cells[6, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateAPRP.Cells[6, 8].Value = "Workplan Cost (Ksh) including VAT";
                    sheetcreateAPRP.Cells[6, 8].Style.Font.Bold = true;
                    sheetcreateAPRP.Cells[6, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateAPRP.Cells[6, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;



                    int rowAPRP = 7;
                    int itemNoAPRP = 1;
                    foreach (var workplan in printData.RoadWorkSectionPlans)
                    {

                        sheetcreateAPRP.Cells[rowAPRP, 2].Value = workplan.RoadSection.Constituency.Name;
                        //sheetcreate.Cells[rowAPRP, 2].Style.Font.Bold = true;
                        sheetcreateAPRP.Cells[rowAPRP, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateAPRP.Cells[rowAPRP, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateAPRP.Cells[rowAPRP, 3].Value = workplan.Road.RoadNumber;
                        //sheetcreate.Cells[rowAPRP, 3].Style.Font.Bold = true;
                        sheetcreateAPRP.Cells[rowAPRP, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateAPRP.Cells[rowAPRP, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateAPRP.Cells[rowAPRP, 4].Value = workplan.RoadSection.SectionName;
                        //sheetcreate.Cells[rowAPRP, 4].Style.Font.Bold = true;
                        sheetcreateAPRP.Cells[rowAPRP, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateAPRP.Cells[rowAPRP, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateAPRP.Cells[rowAPRP, 5].Value = (workplan.WorkCategory != null ? workplan.WorkCategory.Name : "");
                        // sheetcreate.Cells[rowAPRP, 5].Style.Font.Bold = true;
                        sheetcreateAPRP.Cells[rowAPRP, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateAPRP.Cells[rowAPRP, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateAPRP.Cells[rowAPRP, 6].Value = workplan.RoadSection.SurfaceType.Description;
                        // sheetcreate.Cells[rowAPRP, 5].Style.Font.Bold = true;
                        sheetcreateAPRP.Cells[rowAPRP, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateAPRP.Cells[rowAPRP, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateAPRP.Cells[rowAPRP, 7].Value = workplan.RoadSection.Length;
                        //sheetcreate.Cells[rowAPRP, 6].Style.Font.Bold = true;
                        sheetcreateAPRP.Cells[rowAPRP, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateAPRP.Cells[rowAPRP, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateAPRP.Cells[rowAPRP, 8].Value = (workplan.PlanActivities.Sum(a => a.Amount) * 1.16).ToString("N");
                        //sheetcreate.Cells[rowAPRP, 7].Style.Font.Bold = true;
                        sheetcreateAPRP.Cells[rowAPRP, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateAPRP.Cells[rowAPRP, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                        itemNoAPRP++;

                        rowAPRP++;



                    }
                    sheetcreateAPRP.Cells.AutoFitColumns();

                    //END APRP Worksheet Creation

                    excel.Workbook.Properties.Title = "Attempts";
                    _cache.Set(handle, excel.GetAsByteArray(),
                                    new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));

                    return Json(new
                    {
                        Success = true,
                        Message = "Success",
                        Href = Url.Action("Download", "Workplan", new { fileGuid = handle, FileName = currentFileName })
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ARICSController.GetRoads API Error {ex.Message}");
                return Json(new
                {
                    Success = false,
                    Message = "Error occured"
                });
            }

        }

        public async Task<IActionResult> GetCombinedARWPForAgencyV2(long authorityId, long? financialYearId, long regionId, long countyId)
        {
            try
            {

                //retrieve the records to be printed
                //get the current financial year
                var printData = new AWRPViewModel();


                //get the Authority to print for
                //var auth_id = ;
                var authorityResp = await _authorityService.FindByIdAsync(authorityId != 0 ? authorityId : countyId).ConfigureAwait(false);
                if (!authorityResp.Success)
                {
                    return Json(new { Success = false, Message = "The authority supplied is incorrect, please choose the right authority." });
                }
                var authority = authorityResp.Authority;
                printData.Authority = authority;

                FinancialYear financialYear = new FinancialYear();

                if (financialYearId != null && financialYearId > 0)
                {

                    var fYearResp = await _financialYearService.FindByIdAsync((long)financialYearId).ConfigureAwait(false);
                    if (fYearResp.Success)
                        financialYear = fYearResp.FinancialYear;
                }
                else
                {
                    var fYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
                    if (fYearResp.Success)
                    {
                        financialYear = fYearResp.FinancialYear;
                    }
                    else
                    {
                        return Json(new { Success = false, Message = "The financial year supplied for record retrieval could not be found!" });
                    }

                }

                //get the budget details for the authority
                //get budgets for the authority
                var totalBudget = 0.0;
                var budgetHeaderResp = await _budgetCeilingHeaderService.FindCurrentAsync().ConfigureAwait(false);
                if (budgetHeaderResp.Success)
                    printData.BudgetCeilingHeader = budgetHeaderResp.BudgetCeilingHeader;

                var ceilingResp = await _budgetCeilingService.FindApprovedByAuthorityForCurrentYear(financialYear.ID, authority.ID).ConfigureAwait(false);
                if (ceilingResp.Success)
                {
                    totalBudget += ceilingResp.BudgetCeiling.Amount;
                }

                //get the agency budget allocation for other funding source

                var budgetTotalOthers = await _revenueCollectionCodeUnitService.ListAsync(financialYear.ID, "Others").ConfigureAwait(false);
                if (budgetTotalOthers != null)
                {
                    totalBudget += budgetTotalOthers.RevenueCollectionCodeUnit.Where(a => a.AuthorityId == authority.ID).Sum(a => a.RevenueCollection.Amount);
                }

                //get the current workplans for the authority
                var authorityWorkplans = await _roadWorkSectionPlanService.ListByAgencyAsync(authority.ID, financialYear.ID).ConfigureAwait(false);
                List<RoadWorkSectionPlan> roadWorkSectionPlans = new List<RoadWorkSectionPlan>();


                if (regionId != 0)
                {
                    var region = await _regionService.GetRegionAsync(regionId).ConfigureAwait(false);
                    //get all constituencies in the region
                    List<long> constituencyIds = new List<long>();
                    List<long> countyIds = new List<long>();
                    //get the counties in the region
                    foreach (RegionToCounty regionToCounty in region.Region.RegionToCountys)
                    {
                        //loop through the constituencies to get the road ID
                        foreach (Constituency constituency in regionToCounty.County.Constituencys)
                        {
                            //get the county constitutencies
                            foreach (RoadWorkSectionPlan roadWorkSectionPlan in authorityWorkplans.ToList())
                            {

                                //chech the road section constituency if is same as region constituencies.
                                var roadSection = roadWorkSectionPlan.RoadSection;
                                if (roadSection.ConstituencyId == constituency.ID)
                                {
                                    //remove the workplan
                                    roadWorkSectionPlans.Add(roadWorkSectionPlan);
                                }
                            }
                        }


                    }
                    //return the filtered list
                    printData.RoadWorkSectionPlans = roadWorkSectionPlans;
                }
                else
                {
                    printData.RoadWorkSectionPlans = authorityWorkplans.ToList();
                }

                //check if there is any workplan to print
                if (!printData.RoadWorkSectionPlans.Any())
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "We did not find any workplans to load for download. Please check with the administrator."
                    });
                }

                //generate the file.
                string handle = Guid.NewGuid().ToString();
                var memoryStream = new MemoryStream();
                using (ExcelPackage excel = new ExcelPackage(memoryStream))
                {
                    string currentFileName = "ARWP_Summary_" + printData.Authority.Code + "_" + financialYear.Code + ".xlsx";
                    // string currentFileName = System.IO.Path.GetFileName(fileName);
                    var sheetcreateAPRP = excel.Workbook.Worksheets.Add("ARWP Summary");
                    var sheetcreateARWP = excel.Workbook.Worksheets.Add("ARWP Details");



                    //Begin ARWP File Creation
                    //print the column headers
                    //First Row
                    sheetcreateARWP.Cells[1, 1].Value = "ANNUAL ROAD WORKS PROGRAMME ";
                    sheetcreateARWP.Cells[1, 1].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[1, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[1, 2].Value = financialYear.Code;
                    sheetcreateARWP.Cells[1, 2].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[1, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;



                    //Second Row
                    if (authority.Type == 1)
                    {
                        sheetcreateARWP.Cells[2, 1].Value = "NAME OF AGENCY ";
                        sheetcreateARWP.Cells[2, 1].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[2, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[2, 2].Value = printData.Authority.Name;
                        sheetcreateARWP.Cells[2, 2].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[2, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }
                    else
                    {
                        sheetcreateARWP.Cells[2, 1].Value = "NAME OF COUNTY GOVERNMENT ";
                        sheetcreateARWP.Cells[2, 1].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[2, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[2, 2].Value = printData.Authority.Name;
                        sheetcreateARWP.Cells[2, 2].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[2, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }

                    //Third Row
                    if (authority.Type == 1)
                    {
                        sheetcreateARWP.Cells[3, 1].Value = "AGENCY CODE ";
                        sheetcreateARWP.Cells[3, 1].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[3, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[3, 2].Value = printData.Authority.Code;
                        sheetcreateARWP.Cells[3, 2].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[3, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }
                    else
                    {
                        sheetcreateARWP.Cells[3, 1].Value = "COUNTY CODE ";
                        sheetcreateARWP.Cells[3, 1].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[3, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[3, 2].Value = printData.Authority.Code;
                        sheetcreateARWP.Cells[3, 2].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[3, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }

                    //Fourth Row

                    sheetcreateARWP.Cells[4, 1].Value = "BUDGET (KSHS) ";
                    sheetcreateARWP.Cells[4, 1].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[4, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[4, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[4, 2].Value = totalBudget.ToString("N");
                    sheetcreateARWP.Cells[4, 2].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[4, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                    //create now the record items
                    //print headers
                    sheetcreateARWP.Cells[6, 1].Value = "ITEM NO.";
                    sheetcreateARWP.Cells[6, 1].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[6, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[6, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[6, 2].Value = "ROAD NUMBER";
                    sheetcreateARWP.Cells[6, 2].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[6, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[6, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[6, 3].Value = "SECTION/ROAD NAME";
                    sheetcreateARWP.Cells[6, 3].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[6, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[6, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[6, 4].Value = "SURFACE TYPE";
                    sheetcreateARWP.Cells[6, 4].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[6, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[6, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[6, 5].Value = "LENGTH";
                    sheetcreateARWP.Cells[6, 5].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[6, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[6, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[6, 6].Value = "ACTIVITY CODE";
                    sheetcreateARWP.Cells[6, 6].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[6, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[6, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[6, 7].Value = "ACTIVITY DESCRIPTION";
                    sheetcreateARWP.Cells[6, 7].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[6, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[6, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    /* sheetcreate.Cells[6, 8].Value = "WORKS CATEGORY";
                     sheetcreate.Cells[6, 8].Style.Font.Bold = true;
                     sheetcreate.Cells[6, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                     sheetcreate.Cells[6, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
 */
                    sheetcreateARWP.Cells[6, 8].Value = "METHOD";
                    sheetcreateARWP.Cells[6, 8].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[6, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[6, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[6, 9].Value = "UNIT MEASURE";
                    sheetcreateARWP.Cells[6, 9].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[6, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[6, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[6, 10].Value = "PLANNED QUANTITY";
                    sheetcreateARWP.Cells[6, 10].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[6, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[6, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[6, 11].Value = "PLANNED RATE";
                    sheetcreateARWP.Cells[6, 11].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[6, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[6, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[6, 12].Value = "AMOUNT (KSH)";
                    sheetcreateARWP.Cells[6, 12].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[6, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[6, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    int row = 7;
                    int itemNo = 1;
                    double total = 0;
                    foreach (var workplan in printData.RoadWorkSectionPlans.OrderBy(n => n.Road.RoadNumber))
                    {

                        double subTotal = 0;
                        // now set the individual activities
                        sheetcreateARWP.Cells[row, 1].Value = itemNo;
                        sheetcreateARWP.Cells[row, 1].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 2].Value = workplan.Road.RoadNumber;
                        sheetcreateARWP.Cells[row, 2].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 3].Value = workplan.RoadSection.SectionName;
                        sheetcreateARWP.Cells[row, 3].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 4].Value = workplan.RoadSection.SurfaceType.Description;
                        sheetcreateARWP.Cells[row, 4].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 5].Value = workplan.RoadSection.Length;
                        sheetcreateARWP.Cells[row, 5].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        if (workplan.PlanActivities.Any())
                        {
                            foreach (var plan in workplan.PlanActivities)
                            {
                                // sheetcreate.Cells[row, 1].Value = "";
                                sheetcreateARWP.Cells[row, 1].Style.Font.Bold = true;
                                sheetcreateARWP.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                // sheetcreate.Cells[row, 2].Value = "";
                                sheetcreateARWP.Cells[row, 2].Style.Font.Bold = true;
                                sheetcreateARWP.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                // sheetcreate.Cells[row, 3].Value = "";
                                sheetcreateARWP.Cells[row, 3].Style.Font.Bold = true;
                                sheetcreateARWP.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                // sheetcreate.Cells[row, 4].Value ="";
                                sheetcreateARWP.Cells[row, 4].Style.Font.Bold = true;
                                sheetcreateARWP.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 5].Style.Font.Bold = true;
                                sheetcreateARWP.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 6].Value = plan.ItemActivityUnitCost.ItemCode + "-" + plan.ItemActivityUnitCost.SubItemCode + "-" + plan.ItemActivityUnitCost.SubSubItemCode;
                                sheetcreateARWP.Cells[row, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 7].Value = plan.ItemActivityUnitCost.Description;
                                sheetcreateARWP.Cells[row, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 8].Value = plan.Technology != null ? plan.Technology.Code : "";
                                sheetcreateARWP.Cells[row, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 9].Value = plan.ItemActivityUnitCost.UnitMeasure;
                                sheetcreateARWP.Cells[row, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 10].Value = plan.Quantity;
                                sheetcreateARWP.Cells[row, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 11].Value = plan.Rate;
                                sheetcreateARWP.Cells[row, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 12].Value = plan.Amount.ToString("N");
                                sheetcreateARWP.Cells[row, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                row++;

                                total = (total + plan.Amount);
                                subTotal += plan.Amount;

                            }
                        }


                        //PBC Activities
                        if (workplan.PlanActivityPBCs.Any())
                        {
                            foreach (var plan in workplan.PlanActivityPBCs)
                            {
                                // sheetcreate.Cells[row, 1].Value = "";
                                sheetcreateARWP.Cells[row, 1].Style.Font.Bold = true;
                                sheetcreateARWP.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                // sheetcreate.Cells[row, 2].Value = "";
                                sheetcreateARWP.Cells[row, 2].Style.Font.Bold = true;
                                sheetcreateARWP.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                // sheetcreate.Cells[row, 3].Value = "";
                                sheetcreateARWP.Cells[row, 3].Style.Font.Bold = true;
                                sheetcreateARWP.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                // sheetcreate.Cells[row, 4].Value ="";
                                sheetcreateARWP.Cells[row, 4].Style.Font.Bold = true;
                                sheetcreateARWP.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 5].Style.Font.Bold = true;
                                sheetcreateARWP.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 6].Value = plan.ItemActivityPBC.Code;
                                sheetcreateARWP.Cells[row, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 7].Value = plan.ItemActivityPBC.Description;
                                sheetcreateARWP.Cells[row, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 8].Value = plan.ItemActivityPBC.Technology != null ? plan.ItemActivityPBC.Technology.Code : "";
                                sheetcreateARWP.Cells[row, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 9].Value = "KM Per Month";
                                sheetcreateARWP.Cells[row, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 10].Value = plan.PlannedKM;
                                sheetcreateARWP.Cells[row, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 11].Value = plan.CostPerKMPerMonth;
                                sheetcreateARWP.Cells[row, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                sheetcreateARWP.Cells[row, 12].Value = plan.TotalAmount.ToString("N");
                                sheetcreateARWP.Cells[row, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                sheetcreateARWP.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                row++;

                                total = (total + plan.TotalAmount);
                                subTotal += plan.TotalAmount;

                            }
                        }



                        //set subtotal
                        row = row + 1;
                        sheetcreateARWP.Cells[row, 1].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 2].Value = "";
                        sheetcreateARWP.Cells[row, 2].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 3].Value = "";
                        sheetcreateARWP.Cells[row, 3].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 4].Value ="";
                        sheetcreateARWP.Cells[row, 4].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                        //sheetcreate.Cells[row, 5].Value = plan.ItemActivityUnitCost.SubSubItemCode;
                        sheetcreateARWP.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 6].Value = plan.ItemActivityUnitCost.Description;
                        sheetcreateARWP.Cells[row, 6].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 7].Value = plan.Technology;
                        sheetcreateARWP.Cells[row, 7].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 8].Value = plan.ItemActivityUnitCost.UnitMeasure;
                        sheetcreateARWP.Cells[row, 8].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 9].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 10].Value = "Sub Total (KSH)";
                        sheetcreateARWP.Cells[row, 10].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 11].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 12].Value = subTotal.ToString("N");
                        sheetcreateARWP.Cells[row, 12].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //set the VAT field
                        row = row + 1;
                        sheetcreateARWP.Cells[row, 1].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 2].Value = "";
                        sheetcreateARWP.Cells[row, 2].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 3].Value = "";
                        sheetcreateARWP.Cells[row, 3].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 4].Value ="";
                        sheetcreateARWP.Cells[row, 4].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                        //sheetcreate.Cells[row, 5].Value = plan.ItemActivityUnitCost.SubSubItemCode;
                        sheetcreateARWP.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 6].Value = plan.ItemActivityUnitCost.Description;
                        sheetcreateARWP.Cells[row, 6].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 7].Value = plan.Technology;
                        sheetcreateARWP.Cells[row, 7].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 8].Value = plan.ItemActivityUnitCost.UnitMeasure;
                        sheetcreateARWP.Cells[row, 8].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 9].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 10].Value = "16 % VAT";
                        sheetcreateARWP.Cells[row, 10].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 11].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 12].Value = (0.16 * subTotal).ToString("N");
                        sheetcreateARWP.Cells[row, 12].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //set the Contigencies field
                        row = row + 1;
                        sheetcreateARWP.Cells[row, 1].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 2].Value = "";
                        sheetcreateARWP.Cells[row, 2].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 3].Value = "";
                        sheetcreateARWP.Cells[row, 3].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 4].Value ="";
                        sheetcreateARWP.Cells[row, 4].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                        //sheetcreate.Cells[row, 5].Value = plan.ItemActivityUnitCost.SubSubItemCode;
                        sheetcreateARWP.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 6].Value = plan.ItemActivityUnitCost.Description;
                        sheetcreateARWP.Cells[row, 6].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 7].Value = plan.Technology;
                        sheetcreateARWP.Cells[row, 7].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 8].Value = plan.ItemActivityUnitCost.UnitMeasure;
                        sheetcreateARWP.Cells[row, 8].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 9].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 10].Value = "Contigencies@0%";
                        sheetcreateARWP.Cells[row, 10].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 11].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 12].Value = "-";
                        sheetcreateARWP.Cells[row, 12].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //set total with contigencies and VAT
                        row = row + 1;
                        sheetcreateARWP.Cells[row, 1].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 2].Value = "";
                        sheetcreateARWP.Cells[row, 2].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 3].Value = "";
                        sheetcreateARWP.Cells[row, 3].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 4].Value ="";
                        sheetcreateARWP.Cells[row, 4].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                        //sheetcreate.Cells[row, 5].Value = plan.ItemActivityUnitCost.SubSubItemCode;
                        sheetcreateARWP.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 6].Value = plan.ItemActivityUnitCost.Description;
                        sheetcreateARWP.Cells[row, 6].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 7].Value = plan.Technology;
                        sheetcreateARWP.Cells[row, 7].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 8].Value = plan.ItemActivityUnitCost.UnitMeasure;
                        sheetcreateARWP.Cells[row, 8].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 9].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 10].Value = "Total";
                        sheetcreateARWP.Cells[row, 10].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 11].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 12].Value = (1.16 * subTotal).ToString("N");
                        sheetcreateARWP.Cells[row, 12].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //set empty same
                        row = row + 2;
                        sheetcreateARWP.Cells[row, 1].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 2].Value = "";
                        sheetcreateARWP.Cells[row, 2].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 3].Value = "";
                        sheetcreateARWP.Cells[row, 3].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 4].Value ="";
                        sheetcreateARWP.Cells[row, 4].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                        //sheetcreate.Cells[row, 5].Value = plan.ItemActivityUnitCost.SubSubItemCode;
                        sheetcreateARWP.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // sheetcreate.Cells[row, 6].Value = plan.ItemActivityUnitCost.Description;
                        sheetcreateARWP.Cells[row, 6].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 7].Value = plan.Technology;
                        sheetcreateARWP.Cells[row, 7].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        //sheetcreate.Cells[row, 8].Value = plan.ItemActivityUnitCost.UnitMeasure;
                        sheetcreateARWP.Cells[row, 8].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 9].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 10].Value = "";
                        sheetcreateARWP.Cells[row, 10].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 11].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateARWP.Cells[row, 12].Value = "";
                        sheetcreateARWP.Cells[row, 12].Style.Font.Bold = true;
                        sheetcreateARWP.Cells[row, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateARWP.Cells[row, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        itemNo++;

                    }
                    sheetcreateARWP.Cells[row + 1, 10].Value = "Grand Total (KSH)";
                    sheetcreateARWP.Cells[row + 1, 10].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[row + 1, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[row + 1, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[row + 1, 11].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[row + 1, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[row + 1, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[row + 1, 12].Value = (1.16 * total).ToString("N");
                    sheetcreateARWP.Cells[row + 1, 12].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[row + 1, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateARWP.Cells[row + 1, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[row + 3, 1].Value = "Signed By";
                    sheetcreateARWP.Cells[row + 3, 1].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[row + 3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                    sheetcreateARWP.Cells[row + 3, 4].Value = "Designation";
                    sheetcreateARWP.Cells[row + 3, 4].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[row + 3, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                    sheetcreateARWP.Cells[row + 3, 7].Value = "Date";
                    sheetcreateARWP.Cells[row + 3, 7].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[row + 3, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells[row + 3, 9].Value = "Sign";
                    sheetcreateARWP.Cells[row + 3, 9].Style.Font.Bold = true;
                    sheetcreateARWP.Cells[row + 3, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateARWP.Cells.AutoFitColumns();

                    //End ARWP Summary Creation

                    //Begin APRP Worksheet Creation
                    //First Row
                    sheetcreateAPRP.Cells[2, 2].Value = "Routine Maintainance Work Program Report";
                    sheetcreateAPRP.Cells["B2:G2"].Merge = true;
                    sheetcreateAPRP.Cells["B2:G2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    sheetcreateAPRP.Cells["B2:G2"].Style.Font.Bold = true;
                    sheetcreateAPRP.Cells["B2:G2"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    if (printData.Authority.Type == 1)
                    {
                        sheetcreateAPRP.Cells[3, 2].Value = "Authority";
                    }
                    else
                    {
                        sheetcreateAPRP.Cells[3, 2].Value = "County";
                    }

                    sheetcreateAPRP.Cells[3, 2].Style.Font.Bold = true;
                    sheetcreateAPRP.Cells[3, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateAPRP.Cells[3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateAPRP.Cells[3, 3].Value = printData.Authority.Name;
                    sheetcreateAPRP.Cells[3, 3].Style.Font.Bold = true;
                    sheetcreateAPRP.Cells[3, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateAPRP.Cells[3, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateAPRP.Cells[4, 2].Value = "Funding Source";
                    sheetcreateAPRP.Cells[4, 2].Style.Font.Bold = true;
                    sheetcreateAPRP.Cells[4, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateAPRP.Cells[4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                    sheetcreateAPRP.Cells[4, 6].Value = "Financial Year";
                    sheetcreateAPRP.Cells[4, 6].Style.Font.Bold = true;
                    sheetcreateAPRP.Cells[4, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateAPRP.Cells[4, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateAPRP.Cells[4, 7].Value = financialYear.Code;
                    sheetcreateAPRP.Cells[4, 7].Style.Font.Bold = true;
                    sheetcreateAPRP.Cells[4, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateAPRP.Cells[4, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    //create now the record items
                    //print headers
                    sheetcreateAPRP.Cells[6, 2].Value = "REGION/CONSTITUENCY";
                    sheetcreateAPRP.Cells[6, 2].Style.Font.Bold = true;
                    sheetcreateAPRP.Cells[6, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateAPRP.Cells[6, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateAPRP.Cells[6, 3].Value = "ROAD NO.";
                    sheetcreateAPRP.Cells[6, 3].Style.Font.Bold = true;
                    sheetcreateAPRP.Cells[6, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateAPRP.Cells[6, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateAPRP.Cells[6, 4].Value = "SECTION NAME";
                    sheetcreateAPRP.Cells[6, 4].Style.Font.Bold = true;
                    sheetcreateAPRP.Cells[6, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateAPRP.Cells[6, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateAPRP.Cells[6, 5].Value = "SURFACE TYPE";
                    sheetcreateAPRP.Cells[6, 5].Style.Font.Bold = true;
                    sheetcreateAPRP.Cells[6, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateAPRP.Cells[6, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateAPRP.Cells[6, 6].Value = "Length (Km)";
                    sheetcreateAPRP.Cells[6, 6].Style.Font.Bold = true;
                    sheetcreateAPRP.Cells[6, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateAPRP.Cells[6, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    sheetcreateAPRP.Cells[6, 7].Value = "Workplan Cost (Ksh) including VAT";
                    sheetcreateAPRP.Cells[6, 7].Style.Font.Bold = true;
                    sheetcreateAPRP.Cells[6, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreateAPRP.Cells[6, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;



                    int rowAPRP = 7;
                    int itemNoAPRP = 1;
                    foreach (var workplan in printData.RoadWorkSectionPlans)
                    {

                        sheetcreateAPRP.Cells[rowAPRP, 2].Value = workplan.RoadSection.Constituency.Name;
                        //sheetcreate.Cells[rowAPRP, 2].Style.Font.Bold = true;
                        sheetcreateAPRP.Cells[rowAPRP, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateAPRP.Cells[rowAPRP, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateAPRP.Cells[rowAPRP, 3].Value = workplan.Road.RoadNumber;
                        //sheetcreate.Cells[rowAPRP, 3].Style.Font.Bold = true;
                        sheetcreateAPRP.Cells[rowAPRP, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateAPRP.Cells[rowAPRP, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateAPRP.Cells[rowAPRP, 4].Value = workplan.RoadSection.SectionName;
                        //sheetcreate.Cells[rowAPRP, 4].Style.Font.Bold = true;
                        sheetcreateAPRP.Cells[rowAPRP, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateAPRP.Cells[rowAPRP, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateAPRP.Cells[rowAPRP, 5].Value = workplan.RoadSection.SurfaceType.Description;
                        // sheetcreate.Cells[rowAPRP, 5].Style.Font.Bold = true;
                        sheetcreateAPRP.Cells[rowAPRP, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateAPRP.Cells[rowAPRP, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateAPRP.Cells[rowAPRP, 6].Value = workplan.RoadSection.Length;
                        //sheetcreate.Cells[rowAPRP, 6].Style.Font.Bold = true;
                        sheetcreateAPRP.Cells[rowAPRP, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateAPRP.Cells[rowAPRP, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        sheetcreateAPRP.Cells[rowAPRP, 7].Value = (workplan.PlanActivities.Sum(a => a.Amount) * 1.16).ToString("N");
                        //sheetcreate.Cells[rowAPRP, 7].Style.Font.Bold = true;
                        sheetcreateAPRP.Cells[rowAPRP, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        sheetcreateAPRP.Cells[rowAPRP, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                        itemNoAPRP++;

                        rowAPRP++;



                    }
                    sheetcreateAPRP.Cells.AutoFitColumns();

                    //END APRP Worksheet Creation

                    excel.Workbook.Properties.Title = "Attempts";
                    _cache.Set(handle, excel.GetAsByteArray(),
                                    new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));

                    return Json(new
                    {
                        Success = true,
                        Message = "Success",
                        Href = Url.Action("Download", "Workplan", new { fileGuid = handle, FileName = currentFileName })
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ARICSController.GetRoads API Error {ex.Message}");
                return Json(new
                {
                    Success = false,
                    Message = "Error occured"
                });
            }

        }


        public async Task<IActionResult> GetR2000QuarterlyReportForAgency(long authorityId, long financialYearId, long regionId, long countyId)
        {
            R2000ProjectViewModel modelPrint = new R2000ProjectViewModel();

            List<R2000ProjectViewModel> printList = new List<R2000ProjectViewModel>();
            //get the Authority to print for
            var authorityResp = await _authorityService.FindByIdAsync(authorityId).ConfigureAwait(false);
            if (!authorityResp.Success)
                return Json(new
                {
                    Success = false,
                    Message = "The requested Authority could not be found. Please contact the administrator!"
                });

            var authority = authorityResp.Authority;

            //get financial yeat

            var financialYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
            if (!financialYearResp.Success)
                return Json(new
                {
                    Success = false,
                    Message = "The requested Financial year could not be found. Please contact the administrator!"
                });

            var financialYear = financialYearResp.FinancialYear;




            //Get list of contracts for the agency
            var agencyContracts = await _contractService.ListContractsByAgencyByFinancialYear(authorityId, financialYearId).ConfigureAwait(false);
            foreach (Contract contract in agencyContracts)
            {
                //for each contract, get the list of workplans
                foreach (WorkpackageRoadWorkSectionPlan workpackageRoadWorkSectionPlan in contract.WorkPlanPackage.WorkpackageRoadWorkSectionPlans)
                {
                    modelPrint.Intervention = workpackageRoadWorkSectionPlan.RoadWorkSectionPlan.WorkCategory.Name;
                    modelPrint.AchievedKM = 0;
                    modelPrint.TargetKMAchievedToDate = 0;
                    modelPrint.ExpenditureOnAchieved = 0;
                    modelPrint.ExpenditureOnAchievedToDate = 0;
                    modelPrint.ManCount = 0;
                    modelPrint.WomanCount = 0;
                    modelPrint.YouthCount = 0;
                    modelPrint.PersonWithDisabilityCount = 0;
                    modelPrint.TotalContractPersonDays = 0;

                    printList.Add(modelPrint);

                }
            }

            //generate the r2000 projection report file.
            string handle = Guid.NewGuid().ToString();
            var memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                string currentFileName = System.IO.Path.GetFileName("R2000 Quarterly Report.xlsx");
                var sheetcreate = excel.Workbook.Worksheets.Add("Quarter Report");
                //print the column headers
                //First Row
                sheetcreate.Cells[2, 2].Value = "R2000 Quarterly Report";
                sheetcreate.Cells["B2:G2"].Merge = true;
                sheetcreate.Cells["B2:G2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                sheetcreate.Cells["B2:G2"].Style.Font.Bold = true;
                sheetcreate.Cells["B2:G2"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                if (authority.Type == 1)
                {
                    sheetcreate.Cells[3, 2].Value = "Authority";
                }
                else
                {
                    sheetcreate.Cells[3, 2].Value = "County";
                }

                sheetcreate.Cells[3, 2].Style.Font.Bold = true;
                sheetcreate.Cells[3, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                sheetcreate.Cells[3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;



                sheetcreate.Cells[4, 6].Value = "Financial Year";
                sheetcreate.Cells[4, 6].Style.Font.Bold = true;
                sheetcreate.Cells[4, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                sheetcreate.Cells[4, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                sheetcreate.Cells[4, 7].Value = financialYear.Code;
                sheetcreate.Cells[4, 7].Style.Font.Bold = true;
                sheetcreate.Cells[4, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                sheetcreate.Cells[4, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                //print headers
                sheetcreate.Cells[6, 2].Value = "INTERVENTION";
                sheetcreate.Cells[6, 2].Style.Font.Bold = true;
                sheetcreate.Cells[6, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                sheetcreate.Cells[6, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                sheetcreate.Cells[6, 3].Value = "QUARTERLY EMPLOYMENT TARGETS";
                sheetcreate.Cells["B7:G12"].Merge = true;
                sheetcreate.Cells["B7:G12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                sheetcreate.Cells["B7:G12"].Style.Font.Bold = true;
                sheetcreate.Cells["B7:G12"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);


                sheetcreate.Cells[6, 3].Value = "QUARTERLY PHYSICAL ACHIEVEMENT";
                sheetcreate.Cells["B3:G6"].Merge = true;
                sheetcreate.Cells["B3:G6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                sheetcreate.Cells["B3:G6"].Style.Font.Bold = true;
                sheetcreate.Cells["B3:G6"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                sheetcreate.Cells[7, 3].Value = "ACHIEVED (KM).";
                sheetcreate.Cells[7, 3].Style.Font.Bold = true;
                sheetcreate.Cells[7, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                sheetcreate.Cells[7, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                sheetcreate.Cells[7, 4].Value = "% OF TARGET ACHIEVED TO DATE";
                sheetcreate.Cells[7, 4].Style.Font.Bold = true;
                sheetcreate.Cells[7, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                sheetcreate.Cells[7, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                sheetcreate.Cells[7, 5].Value = "EXPENDITURE (MIL. KSHS)";
                sheetcreate.Cells[7, 5].Style.Font.Bold = true;
                sheetcreate.Cells[7, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                sheetcreate.Cells[7, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                sheetcreate.Cells[7, 6].Value = "% OF TARGET ACHIEVED TO DATE (KSH)";
                sheetcreate.Cells[7, 6].Style.Font.Bold = true;
                sheetcreate.Cells[7, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                sheetcreate.Cells[7, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                sheetcreate.Cells[7, 7].Value = "MEN EMPLOYMENT ('000' PD)";
                sheetcreate.Cells[7, 7].Style.Font.Bold = true;
                sheetcreate.Cells[7, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                sheetcreate.Cells[7, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                sheetcreate.Cells[7, 8].Value = "WOMEN EMPLOYMENT ('000' PD)";
                sheetcreate.Cells[7, 8].Style.Font.Bold = true;
                sheetcreate.Cells[7, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                sheetcreate.Cells[7, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                sheetcreate.Cells[7, 9].Value = "YOUTH EMPLOYMENT ('000' PD)";
                sheetcreate.Cells[7, 9].Style.Font.Bold = true;
                sheetcreate.Cells[7, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                sheetcreate.Cells[7, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                sheetcreate.Cells[7, 10].Value = "PERSONS WITH DISABILITIES ('000' PD)";
                sheetcreate.Cells[7, 10].Style.Font.Bold = true;
                sheetcreate.Cells[7, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                sheetcreate.Cells[7, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                sheetcreate.Cells[7, 11].Value = "TOTAL EMPLOYMENT ('000' PD)";
                sheetcreate.Cells[7, 11].Style.Font.Bold = true;
                sheetcreate.Cells[7, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                sheetcreate.Cells[7, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                sheetcreate.Cells[7, 11].Value = "% OF TARGET ACHIEVED TO DATE";
                sheetcreate.Cells[7, 11].Style.Font.Bold = true;
                sheetcreate.Cells[7, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                sheetcreate.Cells[7, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                //create now the record items
                //print headers

                if (!printList.Any())
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "There are no records available to generate the report. Kindly load data!"
                    });

                }



                int row = 8;
                int itemNo = 1;
                foreach (var printModel in printList)
                {

                    sheetcreate.Cells[row, 2].Value = printModel.Intervention;
                    //sheetcreate.Cells[row, 2].Style.Font.Bold = true;
                    sheetcreate.Cells[row, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    sheetcreate.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    /*
                                        sheetcreate.Cells[row, 3].Value = workplan.Road.RoadNumber;
                                        //sheetcreate.Cells[row, 3].Style.Font.Bold = true;
                                        sheetcreate.Cells[row, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                        sheetcreate.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                        sheetcreate.Cells[row, 4].Value = workplan.RoadSection.SectionName;
                                        //sheetcreate.Cells[row, 4].Style.Font.Bold = true;
                                        sheetcreate.Cells[row, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                        sheetcreate.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                        sheetcreate.Cells[row, 5].Value = workplan.RoadSection.SurfaceType.Description;
                                        // sheetcreate.Cells[row, 5].Style.Font.Bold = true;
                                        sheetcreate.Cells[row, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                        sheetcreate.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                        sheetcreate.Cells[row, 6].Value = workplan.RoadSection.Length;
                                        //sheetcreate.Cells[row, 6].Style.Font.Bold = true;
                                        sheetcreate.Cells[row, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                        sheetcreate.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                        sheetcreate.Cells[row, 7].Value = (workplan.PlanActivities.Sum(a => a.Amount) * 1.16).ToString("N");
                                        //sheetcreate.Cells[row, 7].Style.Font.Bold = true;
                                        sheetcreate.Cells[row, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                        sheetcreate.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                                        */
                    itemNo++;

                    row++;



                }
                sheetcreate.Cells.AutoFitColumns();
                excel.Workbook.Properties.Title = "Attempts";
                _cache.Set(handle, excel.GetAsByteArray(),
                                new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));


                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Href = Url.Action("Download", "Workplan", new { fileGuid = handle, FileName = currentFileName })
                });
            }
        }
        public ActionResult Download(string fileGuid, string fileName)
        {

            if (_cache.Get<byte[]>(fileGuid) != null)
            {
                byte[] data = _cache.Get<byte[]>(fileGuid);
                _cache.Remove(fileGuid); //cleanup here as we don't need it in cache anymore
                return File(data, "application/vnd.ms-excel", fileName);
            }
            else
            {
                // Something has gone wrong...
                return View("Error"); // or whatever/wherever you want to return the user
            }
        }
        [Authorize(Claims.Permission.WorkplanPermissions.View)]
        [HttpGet]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> GetRoadsForWorkPlan(long? financialYearId, long? fundingSourceId, long? workCategoryId, long? executionMethodId)
        {
            GetRoadWorkPlanViewModel viewModel = new GetRoadWorkPlanViewModel();

            //Get Logged in user
            var user = await GetLoggedInUser().ConfigureAwait(false);

            var authority = new Authority();
            if (user != null)
            {
                var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                authority = userAuthority.Authority;
                viewModel.Authority = authority;
                viewModel.ApplicationUser = user;

            }

            if (viewModel.Authority.Code == "KRB")
            {
                Response.StatusCode = 404;
                return View("WorkplanErrorViewModel", viewModel);
            }

            //return funding sources configured in revenue collection code unit
            var fundingSources = await _fundingSourceService.ListAsync().ConfigureAwait(false);
            var fundTypes = await _fundTypeService.ListAsync().ConfigureAwait(false);
            var workCategories = await _workCategoryService.ListAsync().ConfigureAwait(false);
            var executionMethods = await _executionMethodService.ListAsync().ConfigureAwait(false);

            //Get current financial year
            var fYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
            viewModel.FinancialYear = fYearResp.FinancialYear;
            var yearList = await _financialYearService.ListAsync().ConfigureAwait(false);


            ViewBag.FundingSourceList = new SelectList(fundingSources, "ID", "Name");
            ViewBag.FundTypeList = new SelectList(fundTypes, "ID", "Name");
            ViewBag.WorkCategoryList = new SelectList(workCategories, "ID", "Name");
            ViewBag.ExecutionMethodList = new SelectList(executionMethods, "ID", "Name");
            ViewBag.FinancialYearList = new SelectList(yearList.Where(f => (int)f.IsCurrent == 1), "ID", "Code");
            ViewBag.Authority = user.Authority;

            //get summaries
            var workplansForAgencies = await _roadWorkSectionPlanService.ListByAgencyAsync(authority.ID, fYearResp.FinancialYear.ID).ConfigureAwait(false);
            var totalBudgetAllocated = 0D;
            var totalWorkplanCost = workplansForAgencies.Sum(a => a.TotalEstimateCost);

            var ceilingResp = await _budgetCeilingService.FindApprovedByAuthorityForCurrentYear(fYearResp.FinancialYear.ID, authority.ID).ConfigureAwait(false);
            if (ceilingResp.Success)
            {
                totalBudgetAllocated += ceilingResp.BudgetCeiling.Amount;
                viewModel.BudgetCeiling = ceilingResp.BudgetCeiling;
            }

            //get the agency budget allocation for other funding source

            var budgetTotalOthers = await _revenueCollectionCodeUnitService.ListAsync(fYearResp.FinancialYear.ID, "Others").ConfigureAwait(false);
            if (budgetTotalOthers != null)
            {
                totalBudgetAllocated += budgetTotalOthers.RevenueCollectionCodeUnit.Where(a => a.AuthorityId == authority.ID).Sum(a => a.RevenueCollection.Amount);
            }
            ViewBag.BudgetAllocated = totalBudgetAllocated;
            ViewBag.totalWorkplanCost = totalWorkplanCost;


            //Get the specific roads for the authority only as per the user assigned authority
            var roads = await _roadService.ListAsync(viewModel.Authority).ConfigureAwait(false);


            if (budgetTotalOthers != null)
            {
                try
                {
                    viewModel.budgetPlanTotalOthers = budgetTotalOthers.RevenueCollectionCodeUnit.Where(a => a.AuthorityId == authority.ID).Sum(a => a.RevenueCollection.Amount);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"WorkplanController.GetRoadsForWorkPlan Error: {Environment.NewLine}");//Means budget amount in revenue collection is not set and thus error is thrown
                    try
                    {
                        viewModel.budgetPlanTotalOthers = 0d;//Means budget amount in revenue collection is set to zero
                    }
                    catch (Exception Ex1)
                    {

                        _logger.LogError(Ex1, $"WorkplanController.GetRoadsForWorkPlan Error: {Environment.NewLine}");
                    }
                }
            }

            // getting arics data
            //Specify Year
            int _Year = 0;

            //bool result = int.TryParse(fYearResp.FinancialYear.Code.Split("/")[0].ToString(), out _Year);
            //if (result == false)
            //{
            //    _Year = DateTime.UtcNow.Year;
            //}

            _Year = fYearResp.FinancialYear.ARICSYear?.Year ?? DateTime.UtcNow.Year;
            //Populate drop down
            await PopulateDropDown(_Year).ConfigureAwait(false);

            //Get arics list for the financial year
            var aricsList = await _aRICSService.GetRoadsAndConditions(_Year, authority).ConfigureAwait(false);

            var aricedRoads = await GetRoads((IList<ARICSData>)aricsList.ARICSData).ConfigureAwait(false);//Get a list of ariced roads
                                                                                                          //aricedRoads.OrderBy(s=>s.);
            if (aricsList.Success)
            {
                if (aricsList.ARICSData != null)
                {
                    viewModel.AricsData = aricsList.ARICSData
                        .OrderBy(o => o.PriorityRate).ThenBy(t => t.IRI).ThenBy(a => a.RateOfDeterioration).ToList();
                }
                else
                {
                    viewModel.AricsData = (Enumerable.Empty<ARICSData>()).ToList();
                }
            }

            viewModel.Roads = roads.Roads.Where(f => viewModel.AricsData.All(r => r.Road.ID != f.ID));

            return View("GetRoadsWorkPlan", viewModel);
        }

        [Authorize(Claims.Permission.WorkplanPermissions.View)]
        [HttpGet]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> GetRoadWorkPlan(long id)
        {
            GetRoadWorkPlanViewModel viewModel = new GetRoadWorkPlanViewModel();
            Road roadModel = new Road();

            //Get Logged in user
            var user = await GetLoggedInUser().ConfigureAwait(false);

            var authority = new Authority();
            if (user != null)
            {
                var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                ViewBag.Authority = userAuthority;
                authority = userAuthority.Authority;
                viewModel.Authority = authority;
                viewModel.ApplicationUser = user;

            }

            //Get current financial year
            var fYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);

            //Check if budget has been defined for the authority in the current year
            if (fYearResp.Success)
            {

                if (id > 0)
                {
                    var response = await _roadService.FindByIdAsync(id).ConfigureAwait(false);
                    if (response.Success)
                    {
                        roadModel = response.Road;
                        authority = roadModel.Authority;
                    }



                    // IEnumerable<RoadWorkSectionPlan> roadWorkSectionPlans = await _roadWorkSectionPlanService.ListByAgencyAsync(authority.ID, fYearResp.FinancialYear.ID).ConfigureAwait(false);
                    IEnumerable<RoadWorkSectionPlan> roadWorkSectionPlans = await _roadWorkSectionPlanService.ListAsync(roadModel.ID, fYearResp.FinancialYear.ID).ConfigureAwait(false);

                    if (user.Authority.Code == "KRB")
                    {
                        viewModel.RoadWorkSectionPlans = roadWorkSectionPlans;
                    }
                    else
                    {
                        viewModel.RoadWorkSectionPlans = roadWorkSectionPlans.Where(a => a.Authority.ID == user.Authority.ID);
                    }

                    //get arics for the road
                    var roadArics = await _aRICSService.GetIRIForRoad(roadModel, null).ConfigureAwait(false);
                    if (roadArics.Success)
                    {
                        if (roadArics.ARICSData != null)
                            viewModel.RoadArics = roadArics.ARICSData.RateOfDeterioration;
                    }
                }

                viewModel.FinancialYear = fYearResp.FinancialYear;
                var budgerHeader = await _roadWorkBudgetHeaderService.FindByAuthorityIdForCurrentYear(fYearResp.FinancialYear.ID, authority.ID).ConfigureAwait(false);

                if (budgerHeader.Success)
                {
                    //existing budget
                    viewModel.RoadWorkBudgetHeader = budgerHeader.RoadWorkBudgetHeader;
                }
                else
                {
                    //is a new budget
                    RoadWorkBudgetHeader newBudgetHeader = new RoadWorkBudgetHeader
                    {
                        Code = fYearResp.FinancialYear.Code,
                        Summary = "Budget Plan for " + authority.Name + " for Fiscal Year : " + fYearResp.FinancialYear.Code,
                        CreatedBy = user.UserName,
                        CreationDate = DateTime.UtcNow,
                        FinancialYear = fYearResp.FinancialYear,
                        Authority = authority,
                        ApprovalStatus = 0
                    };
                    var saveResp = await _roadWorkBudgetHeaderService.AddAsync(newBudgetHeader).ConfigureAwait(false);
                    if (saveResp.Success)
                        viewModel.RoadWorkBudgetHeader = saveResp.RoadWorkBudgetHeader;
                }
                //get the agency budget ceiling set
                var ceilingResp = await _budgetCeilingService.FindApprovedByAuthorityForCurrentYear(fYearResp.FinancialYear.ID, authority.ID).ConfigureAwait(false);
                if (ceilingResp.Success)
                {
                    viewModel.BudgetCeiling = ceilingResp.BudgetCeiling;
                }


                viewModel.Road = roadModel;
            }
            ViewBag.Authority = user.Authority;

            return View("GetRoadWorkPlan", viewModel);
        }


        [HttpGet]
        [Authorize(Claims.Permission.WorkplanPermissions.View)]
        public async Task<IActionResult> GetRoadWorkPlanActivities(long roadWorkSectionId, string? viewName)
        {
            //retrieve the road work section
            var roadWorkSectionPlan = await _roadWorkSectionPlanService.FindByIdAsync(roadWorkSectionId).ConfigureAwait(false);
            GetRoadWorkPlanViewModel viewModel = new GetRoadWorkPlanViewModel();
            viewModel.RoadWorkSectionPlan = roadWorkSectionPlan.RoadWorkSectionPlan;

            //Get Logged in user
            var user = await GetLoggedInUser().ConfigureAwait(false);

            var authority = new Authority();
            authority = user.Authority;
            ViewBag.Authority = user.Authority;
            viewModel.Authority = user.Authority; ;
            viewModel.ApplicationUser = user;
            //Get current financial year
            var fYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);

            //Check if budget has been defined for the authority in the current year
            if (fYearResp.Success)
            {
                viewModel.FinancialYear = fYearResp.FinancialYear;
                var budgerHeader = await _roadWorkBudgetHeaderService.FindByAuthorityIdForCurrentYear(fYearResp.FinancialYear.ID, authority.ID).ConfigureAwait(false);

                var roadWorkSectionPlans = await _roadWorkSectionPlanService.ListByAgencyAsync(authority.ID, fYearResp.FinancialYear.ID).ConfigureAwait(false);
                viewModel.RoadWorkSectionPlans = roadWorkSectionPlans;

                if (budgerHeader.Success)
                {
                    //existing budget
                    viewModel.RoadWorkBudgetHeader = budgerHeader.RoadWorkBudgetHeader;
                }
                else
                {
                    //is a new budget
                    RoadWorkBudgetHeader newBudgetHeader = new RoadWorkBudgetHeader
                    {
                        Code = fYearResp.FinancialYear.Code,
                        Summary = "Budget Plan for " + authority.Name + " for Fiscal Year : " + fYearResp.FinancialYear.Code,
                        CreatedBy = user.UserName,
                        CreationDate = DateTime.UtcNow,
                        FinancialYear = fYearResp.FinancialYear,
                        Authority = authority,
                        ApprovalStatus = 0
                    };
                    var saveResp = await _roadWorkBudgetHeaderService.AddAsync(newBudgetHeader).ConfigureAwait(false);
                    if (saveResp.Success)
                        viewModel.RoadWorkBudgetHeader = saveResp.RoadWorkBudgetHeader;
                }
            }
            //get summaries
            var workplansForAgencies = await _roadWorkSectionPlanService.ListByAgencyAsync(authority.ID, fYearResp.FinancialYear.ID).ConfigureAwait(false);
            var totalBudgetAllocated = 0D;
            var totalWorkplanCost = workplansForAgencies.Sum(a => a.TotalEstimateCost);

            var ceilingResp = await _budgetCeilingService.FindApprovedByAuthorityForCurrentYear(fYearResp.FinancialYear.ID, authority.ID).ConfigureAwait(false);
            if (ceilingResp.Success)
            {
                totalBudgetAllocated += ceilingResp.BudgetCeiling.Amount;
                viewModel.BudgetCeiling = ceilingResp.BudgetCeiling;
            }

            //get the agency budget allocation for other funding source

            var budgetTotalOthers = await _revenueCollectionCodeUnitService.ListAsync(fYearResp.FinancialYear.ID, "Others").ConfigureAwait(false);
            if (budgetTotalOthers != null)
            {
                totalBudgetAllocated += budgetTotalOthers.RevenueCollectionCodeUnit.Where(a => a.AuthorityId == authority.ID).Sum(a => a.RevenueCollection.Amount);
            }
            ViewBag.BudgetAllocated = totalBudgetAllocated;
            ViewBag.totalWorkplanCost = totalWorkplanCost;
            if (!string.IsNullOrEmpty(viewName) && viewName == "GetRoadWorkPlanApproval")
            {
                return View("GetRoadWorkPlanApproval", viewModel);
            }

            return View("GetRoadWorkPlanActivities", viewModel);
        }
        [Authorize(Claims.Permission.WorkplanPermissions.View)]
        [HttpGet]
        public async Task<IActionResult> GetAuthorityApprovals(long authorityId)
        {
            var authResp = await _authorityService.FindByIdAsync(authorityId).ConfigureAwait(false);
            if (authResp.Success)
            {
                return RedirectToAction("Index", "Workplan", new { viewName = "agency-approval-summary-batch", authorityName = authResp.Authority.Code });
            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "The authority record could not be retrieved."
                });
            }

        }

        [Authorize(Claims.Permission.WorkplanPermissions.View)]
        [HttpGet]
        public async Task<IActionResult> Index(string? viewName, string? authorityName)
        {
            GetRoadWorkPlanViewModel viewModel = new GetRoadWorkPlanViewModel();

            //retrieve the current logged in user information.

            var user = await GetLoggedInUser().ConfigureAwait(false);
            Authority authority = new Authority();
            if (!string.IsNullOrEmpty(authorityName))
            {
                var dataAuthority = await _authorityService.FindByCodeAsync(authorityName).ConfigureAwait(false);
                viewModel.Authority = dataAuthority.Authority;
                if (dataAuthority.Success)
                    authority = dataAuthority.Authority;
                else
                    authority = user.Authority;
            }
            else
            {
                if (user != null)
                {
                    authority = user.Authority;

                }
            }

            //user authority
            ViewBag.Authority = user.Authority;

            //get current financial year
            var fYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);

            //get the workplan approval batch
            var workplanApprovalBatchResp = await _workplanApprovalBatchService.FindByFinancialYearAsync(fYearResp.FinancialYear.ID, authority.ID).ConfigureAwait(false);
            if (workplanApprovalBatchResp.Success)
            {
                viewModel.WorkplanApprovalBatch = workplanApprovalBatchResp.WorkplanApprovalBatch;
            }
            else
            {
                viewModel.WorkplanApprovalBatch = new WorkplanApprovalBatch();
                viewModel.WorkplanApprovalBatch.WorkplanApprovalHistories = Enumerable.Empty<WorkplanApprovalHistory>();
            }

            //Get the specific roads for the authority only as per the user assigned authority
            var workplansForAgencies = Enumerable.Empty<RoadWorkSectionPlan>();

            if (authority.Code.Equals("KRB", StringComparison.OrdinalIgnoreCase))
            {
                workplansForAgencies = await _roadWorkSectionPlanService.ListAgenciesAllAsync(fYearResp.FinancialYear.ID).ConfigureAwait(false);
            }
            else
            {
                workplansForAgencies = await _roadWorkSectionPlanService.ListByAgencyAsync(authority.ID, fYearResp.FinancialYear.ID).ConfigureAwait(false);
            }


            ARICSData roadData;

            //fetch the road list with ARICs information for the roads.
            List<ARICSData> workplanRoads = new List<ARICSData>();
            List<ARICSData> workplanR200Roads = new List<ARICSData>();
            List<Road> checkedRoads = new List<Road>();
            var approvedWorkplans = 0;
            var totalWorkplans = workplansForAgencies.Count(); ;

            foreach (var wp in workplansForAgencies)
            {
                //getting the approved workplans
                if (wp.ApprovalStatus == true)
                    approvedWorkplans += 1;

                roadData = new ARICSData();
                //getting the arics data for the workplan road

                //check if the road has been queried for IRI already, not to query again for performance.
                if (!checkedRoads.Contains(wp.Road))
                {


                    var aricsIRI = await _aRICSService.GetIRIForRoad(wp.Road, null).ConfigureAwait(false);
                    if (aricsIRI.Success)
                        roadData.RateOfDeterioration = aricsIRI.ARICSData.RateOfDeterioration;

                    checkedRoads.Add(wp.Road);

                    roadData.Road = wp.Road;
                }



                //calculating workplan totals, to determine whether the road is in R2000 strategy
                var planActivitiesOnTheRoadTotalLB = 0;
                var planActivitiesOnTheRoadTotal = 0;
                if (wp.PlanActivities != null)
                {
                    if (wp.PlanActivities.Any())
                    {
                        planActivitiesOnTheRoadTotal += wp.PlanActivities.Count();
                        planActivitiesOnTheRoadTotalLB += wp.PlanActivities.Where(t => t.Technology != null).Count(r => r.Technology.Code == "LB");
                    }
                }

                //check if the workplan has any activities, this avoids divide by zero exception
                if (planActivitiesOnTheRoadTotal > 0)
                {
                    if (((double)planActivitiesOnTheRoadTotalLB / (double)planActivitiesOnTheRoadTotal) > 0.5)
                    {
                        if (roadData.Road != null)
                        {

                            if (!workplanR200Roads.Contains(roadData))
                                workplanR200Roads.Add(roadData);
                        }
                    }
                    else
                    {
                        if (roadData.Road != null)
                        {

                            if (!workplanRoads.Contains(roadData))
                                workplanRoads.Add(roadData);
                        }
                    }
                }
                else
                {
                    if (roadData.Road != null)
                    {
                        if (!workplanRoads.Contains(roadData))
                            workplanRoads.Add(roadData);
                    }
                }
            }

            viewModel.WorkplanRoads = workplanRoads;
            viewModel.WorkplanR2000Roads = workplanR200Roads;
            viewModel.ApprovedWorkplans = approvedWorkplans;
            viewModel.TotalWorkplans = totalWorkplans;
            viewModel.Authority = authority;


            //set view back data sources
            var fundingSources = await _revenueCollectionCodeUnitService.ListAsync(authority.ID).ConfigureAwait(false);
            var yearList = await _financialYearService.ListAsync().ConfigureAwait(false);
            var authorityList = await _authorityService.ListAsync().ConfigureAwait(false);

            var county = new Authority();
            county.ID = 0;
            county.Code = "Counties";
            county.Name = "Counties";
            county.Type = 10;

            var authList = authorityList.ToList();
            authList.Add(county);

            // authorityList.ToList().Add(allAuthority);



            ViewBag.FundingSourceList = new SelectList(fundingSources.RevenueCollectionCodeUnit, "ID", "RevenueStream");
            if (authority.Type == 0 || authority.Type == 3) //if KRB or Consultant, all authorities are fetched.
            {
                ViewBag.AuthorityList = new SelectList(authList.Where(f => f.Code != "KRB" && f.Code != "Csltnt" && f.Type != 2).OrderBy(o => o.Type), "ID", "Name");
            }
            else
            {
                ViewBag.AuthorityList = new SelectList(authList.Where(f => f.Code == authority.Code).OrderBy(o => o.Type), "ID", "Name");
            }

            ViewBag.FinancialYearList = new SelectList(yearList, "ID", "Code", yearList.OrderByDescending(o => o.IsCurrent));
            ViewBag.CountyList = new SelectList(authList.Where(f => f.Type == 2).OrderBy(o => o.Type), "ID", "Name");

            //get summaries
            var totalBudgetAllocated = 0D;
            var totalWorkplanCost = workplansForAgencies.Sum(a => a.TotalEstimateCost);

            var ceilingResp = await _budgetCeilingService.FindApprovedByAuthorityForCurrentYear(fYearResp.FinancialYear.ID, authority.ID).ConfigureAwait(false);
            if (ceilingResp.Success)
            {
                totalBudgetAllocated += ceilingResp.BudgetCeiling.Amount;
            }

            //get the agency budget allocation for other funding source

            var budgetTotalOthers = await _revenueCollectionCodeUnitService.ListAsync(fYearResp.FinancialYear.ID, "Others").ConfigureAwait(false);
            if (budgetTotalOthers != null)
            {
                totalBudgetAllocated += budgetTotalOthers.RevenueCollectionCodeUnit.Where(a => a.AuthorityId == authority.ID).Sum(a => a.RevenueCollection.Amount);
            }
            ViewBag.BudgetAllocated = totalBudgetAllocated;
            ViewBag.totalWorkplanCost = totalWorkplanCost;
            ViewBag.FinancialYear = fYearResp.FinancialYear.Code;


            //return the appropriate view for the user
            if (authority.Code.Equals("KRB", StringComparison.OrdinalIgnoreCase))
            {
                //check if approvals or summary
                if (viewName != null)
                {
                    if (viewName == "agency-approval-summary-batch")
                    {
                        viewModel.approvalAuthority = authorityName;
                        return View("agency-approval-summary-batch", viewModel);
                    }
                }

                return View("authority-summary", viewModel);
            }
            else
            {
                if (viewName != null)
                {
                    if (viewName == "agency-approval-summary-batch")
                    {
                        viewModel.approvalAuthority = authorityName;
                        return View("agency-approval-summary-batch", viewModel);
                    }
                }
                return View("agency-summary", viewModel);
            }
        }

        [Authorize(Claims.Permission.WorkplanPermissions.View)]
        [HttpGet]
        public async Task<IActionResult> GetRoadWorkPlanLine(long id)
        {
            RoadWorkPlanViewItemModel viewModel = new RoadWorkPlanViewItemModel();
            Road roadModel = new Road();

            if (id > 0)
            {
                var response = await _roadService.FindByIdAsync(id).ConfigureAwait(false);
                if (response.Success)
                {
                    roadModel = response.Road;
                }
            }
            viewModel.Road = roadModel;
            viewModel.RoadSections = roadModel.RoadSections;
            viewModel.FundingSources = await _fundingSourceService.ListAsync().ConfigureAwait(false);
            viewModel.FundTypes = await _fundTypeService.ListAsync().ConfigureAwait(false);
            viewModel.ExecutionMethods = await _executionMethodService.ListAsync().ConfigureAwait(false);
            viewModel.WorkCategories = await _workCategoryService.ListAsync().ConfigureAwait(false);
            var user = await GetLoggedInUser().ConfigureAwait(false);
            ViewBag.Authority = user.Authority;
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUploadWorkplan(long uploadId)
        {
            var user = await GetLoggedInUser().ConfigureAwait(false);

            var fYear = new FinancialYear();
            var fYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
            if (fYearResp.Success)
                fYear = fYearResp.FinancialYear;

            //pick all the uploads for the current year and for each, create a workplan


            var dbActivities = (List<ItemActivityUnitCost>)await _itemActivityUnitCostService.ListAsync().ConfigureAwait(false);
            //var dbActivitiesPbc = (List<ItemActivityPBC>)await _itemActivityPBCService.ListAsync().ConfigureAwait(false);

            var dbCategories = (List<WorkCategory>)await _workCategoryService.ListAsync().ConfigureAwait(false);
            var agencyUpload = await _workplanUploadService.FindByIdAsync(uploadId).ConfigureAwait(false);
            var technologyList = await _technologyService.ListAsync().ConfigureAwait(false);

            WorkplanApprovalBatch workplanApprovalBatch = new WorkplanApprovalBatch();
            var workplanApprovalBatchResp = await _workplanApprovalBatchService.FindByFinancialYearAsync(fYear.ID, user.Authority.ID).ConfigureAwait(false);
            if (workplanApprovalBatchResp.Success)
            {
                workplanApprovalBatch = workplanApprovalBatchResp.WorkplanApprovalBatch;
            }
            else
            {
                workplanApprovalBatch.ApprovalStatus = 0;
                workplanApprovalBatch.FinancialYear = fYear;
                workplanApprovalBatch.Authority = user.Authority;
            }


            List<string> missingRoadSections = new List<string>();
            List<MissingItemViewModel> missingActivities = new List<MissingItemViewModel>(); // this holds all item codes not found in the database
            List<MissingWorkCategoriesViewModel> missingCategories = new List<MissingWorkCategoriesViewModel>();

            RoadWorkSectionPlan newSection;
            PlanActivity newPlanAcvitity;
            MissingItemViewModel missingItemViewModel = new MissingItemViewModel();

            List<RoadWorkSectionPlan> newSectionPlans = new List<RoadWorkSectionPlan>();


            if (agencyUpload.Success)
            {
                foreach (var section in agencyUpload.WorkplanUpload.WorkplanUploadSections)
                {
                    // reset the road section
                    newSection = new RoadWorkSectionPlan();
                    newSection.PlanActivities = new List<PlanActivity>();
                    newSection.WorkplanApprovalBatch = workplanApprovalBatch;
                    //get the section from DB.
                    var dbActivityResp = await _roadSectionService.FindBySectionIdAsync(section.SectionId).ConfigureAwait(false);
                    if (dbActivityResp.Success)
                    {
                        newSection.RoadSection = dbActivityResp.RoadSection;
                        newSection.RoadSectionId = dbActivityResp.RoadSection.ID;
                        newSection.Road = dbActivityResp.RoadSection.Road;
                        newSection.RoadId = dbActivityResp.RoadSection.Road.ID;
                        newSection.Authority = user.Authority;
                        newSection.FinancialYear = fYear;

                        //work category check
                        var categoryFound = dbCategories.Where(c => c.Code.ToLower().Equals(section.WorkCategory.ToLower())).FirstOrDefault();
                        if (categoryFound != null) //category is in the database
                        {
                            newSection.WorkCategory = categoryFound;
                        }
                    }
                    //get related activities
                    foreach (var activity in section.WorkplanUploadSectionActivities)
                    {

                        //check if activity exists in db activities
                        var activityFound = dbActivities.Where(m => (m.ItemCode + '-' + m.SubItemCode + '-' + m.SubSubItemCode).ToLower().Equals(activity.ActivityCode.ToLower())).FirstOrDefault();
                        if (activityFound != null)
                        {
                            newPlanAcvitity = new PlanActivity();
                            newPlanAcvitity.ItemActivityUnitCost = activityFound;
                            newPlanAcvitity.Rate = activity.PlannedRate;
                            newPlanAcvitity.Quantity = activity.Quantity;
                            newPlanAcvitity.Amount = (double)((decimal)activity.PlannedRate * activity.Quantity);

                            var tech = technologyList.FirstOrDefault(t => t.Code.ToLower().Equals(activity.Technology.ToLower()));
                            newPlanAcvitity.Technology = tech;

                            newSection.PlanActivities.Add(newPlanAcvitity);
                        }
                    }

                    // newSectionPlans.Add(newSection);
                    newSection.TotalEstimateCost = newSection.PlanActivities.Sum(a => (double)a.Quantity * a.Rate);
                    newSection.PlannedLength = section.PlannedSectionLength;
                    var addResp = await _roadWorkSectionPlanService.AddAsync(newSection).ConfigureAwait(false);
                }

                //set the workplans have been created.
                WorkplanUpload uploadUpdate = new WorkplanUpload();
                uploadUpdate = agencyUpload.WorkplanUpload;
                uploadUpdate.WorkplansCreatedDate = DateTime.UtcNow;
                uploadUpdate.WorkplansCreatedBy = user.UserName;
                uploadUpdate.WorkplansCreated = 1; //set as one to show workplans have been create already.

                await _workplanUploadService.UpdateAsync(uploadUpdate).ConfigureAwait(false);
            }
            return Json(new
            {
                Success = true,
                Message = "Success",
                Href = Url.Action("UploadWorkplans", "Workplan")
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateSectionWorkplan(long sectionId, long financialYearId, long fundingSourceId, long workCategoryId, long executionMethodId)
        {
            try
            {
                //initiate work plan creation for the road Id
                var resp = await _roadSectionService.FindByIdAsync(sectionId).ConfigureAwait(false);
                if (resp.Success)
                {
                    var user = await GetLoggedInUser().ConfigureAwait(false);
                    var authority = new Authority();
                    WorkplanApprovalBatch workplanApprovalBatch = new WorkplanApprovalBatch();
                    if (user != null)
                    {
                        var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                        ViewBag.Authority = userAuthority;
                        authority = userAuthority.Authority;
                    }

                    var financialYear = await _financialYearService.FindByIdAsync(financialYearId).ConfigureAwait(false);
                    //check if there exists an approval bathc
                    var workplanApprovalBatchResp = await _workplanApprovalBatchService.FindByFinancialYearAsync(financialYear.FinancialYear.ID, authority.ID).ConfigureAwait(false);
                    if (workplanApprovalBatchResp.Success)
                    {
                        workplanApprovalBatch = workplanApprovalBatchResp.WorkplanApprovalBatch;
                    }
                    else
                    {
                        workplanApprovalBatch.ApprovalStatus = 0;
                        workplanApprovalBatch.FinancialYear = financialYear.FinancialYear;
                        workplanApprovalBatch.Authority = authority;
                    }


                    //Create work plan first, then associate approval record
                    RoadWorkSectionPlan workplanItem;


                    //generate work plan
                    workplanItem = new RoadWorkSectionPlan
                    {
                        Road = resp.RoadSection.Road,
                        RoadSectionId = resp.RoadSection.ID,
                        RevisionStatus = false,
                        TotalEstimateCost = 0.0,
                        PlannedLength = 0.0,
                        ApprovalStatus = false,
                        AuthorityId = authority.ID,
                        //planning details
                        FinancialYearId = financialYear.FinancialYear.ID,
                        FundingSourceId = fundingSourceId,
                        WorkCategoryId = workCategoryId,
                        ExecutionMethodId = executionMethodId,
                        WorkplanApprovalBatch = workplanApprovalBatch
                    };

                    var newWorkplanResp = await _roadWorkSectionPlanService.AddAsync(workplanItem).ConfigureAwait(false);

                    if (!newWorkplanResp.Success)
                    {
                        /* WorkplanApprovalBatch workplanApprovalBatch = new WorkplanApprovalBatch();
                         workplanApprovalBatch.ApprovalStatus = 0;
                         await _workplanApprovalService.AddAsync(workplanApprovalBatch).ConfigureAwait(false);*/
                        return Json(new
                        {
                            Success = false,
                            Message = "Failed to create the workplan"
                        });
                    }

                }
                return Json(new
                {
                    Success = true,
                    Message = "Workplan for : " + resp.RoadSection.SectionName + " has been created. Add Activities?",
                    Href = Url.Action("GetRoadWorkPlan", "Workplan", new { id = resp.RoadSection.Road.ID })
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"Workplancontroller Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                return Json(new
                {
                    Success = false,
                    Message = "Exception"
                });
            }
        }


        [HttpPost]
        public async Task<IActionResult> SaveUpdateSectionWorkplan(RoadWorkSectionPlan roadWorkSectionPlan)
        {
            if (roadWorkSectionPlan != null)
            {
                var user = await GetLoggedInUser().ConfigureAwait(false);
                var authority = new Authority();
                if (user != null)
                {
                    var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                    ViewBag.Authority = userAuthority;
                    authority = userAuthority.Authority;
                }

                //editing only the specific fields for work planning the record
                if (roadWorkSectionPlan.ID > 0)
                {
                    //existing record
                    var response = await _roadWorkSectionPlanService.FindByIdAsync(roadWorkSectionPlan.ID).ConfigureAwait(false);
                    if (response.Success)
                    {
                        var updateModel = response.RoadWorkSectionPlan;

                        updateModel.RevisionStatus = roadWorkSectionPlan.RevisionStatus;
                        updateModel.ApprovalStatus = roadWorkSectionPlan.ApprovalStatus;
                        updateModel.ExecutionMethodId = roadWorkSectionPlan.ExecutionMethodId;
                        updateModel.FundingSourceId = roadWorkSectionPlan.FundingSourceId;
                        updateModel.FundTypeId = roadWorkSectionPlan.FundTypeId;
                        updateModel.WorkCategoryId = roadWorkSectionPlan.WorkCategoryId;
                        updateModel.PlannedLength = roadWorkSectionPlan.PlannedLength;
                        updateModel.AchievedLength = roadWorkSectionPlan.AchievedLength;
                        //updateModel.TotalEstimateCost = roadWorkSectionPlan.TotalEstimateCost;

                        var updateResponse = await _roadWorkSectionPlanService.UpdateAsync(updateModel).ConfigureAwait(false);
                        if (updateResponse.Success)
                        {
                            return Json(new
                            {
                                Success = true,
                                Message = "Success",
                                Href = Url.Action("GetRoadWorkPlan", "Workplan", new { id = updateResponse.RoadWorkSectionPlan.Road.ID })
                            });
                        }
                    }
                    else
                    {
                        return PartialView("SectionWorkplanEditPartialView", roadWorkSectionPlan);
                    }
                }
                else
                {
                    //new record
                    //get the road section
                    var roadSection = await _roadSectionService.FindByIdAsync((long)roadWorkSectionPlan.RoadSection.ID).ConfigureAwait(false);
                    if (roadSection.Success)
                    {
                        //create approval line for the work plan

                        var financialYear = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
                        WorkplanApprovalBatch workplanApprovalBatch = new WorkplanApprovalBatch();
                        workplanApprovalBatch.ApprovalStatus = 0;
                        workplanApprovalBatch.FinancialYear = financialYear.FinancialYear;

                        var newRecord = new RoadWorkSectionPlan
                        {
                            RevisionStatus = roadWorkSectionPlan.RevisionStatus,
                            ApprovalStatus = roadWorkSectionPlan.ApprovalStatus,
                            ExecutionMethodId = roadWorkSectionPlan.ExecutionMethodId,
                            FundingSourceId = roadWorkSectionPlan.FundingSourceId,
                            FundTypeId = roadWorkSectionPlan.FundTypeId,
                            WorkCategoryId = roadWorkSectionPlan.WorkCategoryId,
                            PlannedLength = roadWorkSectionPlan.PlannedLength,
                            RoadSectionId = roadSection.RoadSection.ID,
                            Road = roadSection.RoadSection.Road,
                            FinancialYearId = financialYear.FinancialYear.ID,
                            WorkplanApprovalBatch = workplanApprovalBatch
                        };

                        var addResponse = await _roadWorkSectionPlanService.AddAsync(newRecord).ConfigureAwait(false);
                        if (addResponse.Success)
                        {
                            return Json(new
                            {
                                Success = true,
                                Message = "Success"
                            });
                        }
                    }
                }
            }
            return PartialView("SectionWorkplanEditPartialView", roadWorkSectionPlan);
        }

        public async Task<IActionResult> GetSectionWorkplanEditView(long SectionPlanId, string flagNew)
        {
            RoadWorkSectionPlan sectionPlanModel = new RoadWorkSectionPlan();

            //Get Logged in user
            var user = await GetLoggedInUser().ConfigureAwait(false);

            var authority = new Authority();
            if (user != null)
            {
                var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                authority = userAuthority.Authority;
            }

            // var fundingSources = await _fundingSourceService.ListAsync().ConfigureAwait(false);
            var fundingSources = await _fundingSourceService.ListAsync().ConfigureAwait(false);
            var fundTypes = await _fundTypeService.ListAsync().ConfigureAwait(false);
            var workCategories = await _workCategoryService.ListAsync().ConfigureAwait(false);
            var executionMethods = await _executionMethodService.ListAsync().ConfigureAwait(false);

            if (SectionPlanId > 0)
            {
                if (flagNew == "NEW")
                {
                    //new record
                    var section = await _roadSectionService.FindByIdAsync(SectionPlanId).ConfigureAwait(false);
                    if (section.Success)
                    {
                        sectionPlanModel.RoadSection = section.RoadSection;
                    }
                }
                else
                {
                    var sectionPlan = await _roadWorkSectionPlanService.FindByIdAsync(SectionPlanId).ConfigureAwait(false);
                    if (sectionPlan.Success)
                        sectionPlanModel = sectionPlan.RoadWorkSectionPlan;
                }

            }

            ViewBag.FundingSourceList = new SelectList(fundingSources, "ID", "Name");
            ViewBag.FundTypeList = new SelectList(fundTypes, "ID", "Name");
            ViewBag.WorkCategoryList = new SelectList(workCategories, "ID", "Name");
            ViewBag.ExecutionMethodList = new SelectList(executionMethods, "ID", "Name");
            ViewBag.FlagNew = flagNew;

            return PartialView("SectionWorkplanEditPartialView", sectionPlanModel);

        }



        public async Task<IActionResult> GetItemActivitySelect(string activityType)
        {
            var viewModel = await _itemActivityUnitCostService.ListAsync().ConfigureAwait(false);
            if (activityType == "admin")
            {
                viewModel = viewModel.Where(i => i.OverheadRoutineImprovement == "OV");
            }
            else
            {
                viewModel = viewModel.Where(i => i.OverheadRoutineImprovement != "OV");
            }

            return PartialView("ItemAcvitityListPartialViewSelect", viewModel);
        }

        public async Task<IActionResult> GetPBCActivitySelect()
        {

            var viewModel = await _itemActivityPBCService.ListAsync().ConfigureAwait(false);
            return PartialView("ItemPBCAcvitityListPartialViewSelect", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSelectedItemActivities(long[] seletectedIds, long sectionPlanId)
        {

            // selectedData: seletectedIds
            if (sectionPlanId > 0)
            {
                if (seletectedIds != null)
                {
                    PlanActivity planToSave;
                    //editing only the specific fields for work planning the record
                    if (seletectedIds.Length > 0)
                    {
                        foreach (long id in seletectedIds)
                        {
                            planToSave = new PlanActivity();

                            //Get Logged in user
                            var user = await GetLoggedInUser().ConfigureAwait(false);
                            var authority = new Authority();
                            if (user != null)
                            {
                                var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                                authority = userAuthority.Authority;
                            }

                            //get rate for the ID
                            var selectedId = await _itemActivityUnitCostService.FindByIdAsync(id).ConfigureAwait(false);
                            if (selectedId.Success)
                            {
                                var activityRate = selectedId.ItemActivityUnitCost.ItemActivityUnitCostRates.Where(a => a.Authority.ID == authority.ID).SingleOrDefault();
                                if (activityRate != null)
                                {
                                    planToSave.Rate = activityRate.AuthorityRate;
                                }
                                else
                                {
                                    activityRate = selectedId.ItemActivityUnitCost.ItemActivityUnitCostRates.Where(a => a.Authority.Code == "KRB").SingleOrDefault();
                                    if (activityRate != null)
                                    {
                                        planToSave.Rate = activityRate.AuthorityRate;
                                    }
                                    else
                                    {
                                        planToSave.Rate = 0.0;
                                    }
                                }
                            }
                            else
                            {
                                planToSave.Rate = 0.0;
                            }

                            //create the record in the database for the activity with the plan

                            planToSave.RoadWorkSectionPlanId = sectionPlanId;
                            planToSave.ItemActivityUnitCostId = id;
                            planToSave.Quantity = 0;
                            planToSave.Amount = 0.0;

                            await _planActivityService.AddAsync(planToSave).ConfigureAwait(false);
                        }

                        return Json(new
                        {
                            Success = true,
                            Message = "Success",
                            Href = Url.Action("GetRoadWorkPlanActivities", "Workplan", new { roadWorkSectionId = sectionPlanId })
                        });

                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Invalid Selection"
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Please select a plan"
                    });
                }
            }
            var viewModel = await _itemActivityUnitCostService.ListAsync().ConfigureAwait(false);
            return PartialView("ItemAcvitityListPartialViewSelect", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSelectedItemActivitiesPBC(long[] seletectedIds, long sectionPlanId)
        {

            // selectedData: seletectedIds
            if (sectionPlanId > 0)
            {
                if (seletectedIds != null)
                {
                    PlanActivityPBC planToSave;
                    //editing only the specific fields for work planning the record
                    if (seletectedIds.Length > 0)
                    {
                        foreach (long id in seletectedIds)
                        {
                            //create the record in the database for the activity with the plan
                            planToSave = new PlanActivityPBC();
                            planToSave.RoadWorkSectionPlanId = sectionPlanId;
                            planToSave.ItemActivityPBCId = id;

                            await _planActivityPBCService.AddAsync(planToSave).ConfigureAwait(false);
                        }

                        return Json(new
                        {
                            Success = true,
                            Message = "Success",
                            Href = Url.Action("GetRoadWorkPlanActivities", "Workplan", new { roadWorkSectionId = sectionPlanId })
                        });

                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Invalid Selection"
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Please select a plan"
                    });
                }
            }
            var viewModel = await _itemActivityUnitCostService.ListAsync().ConfigureAwait(false);
            return PartialView("ItemPBCAcvitityListPartialViewSelect", viewModel);
        }

        //GetItemActivitySelectForSection
        public async Task<IActionResult> GetItemActivitySelectForSection(long SectionPlanId, long RoadId)
        {
            if (SectionPlanId > 0)
            {
                //value set correctly, retrieve the Item Activities mapped to the road
                var activityPlanList = await _planActivityService.ListAsync().ConfigureAwait(false);
                var viewModel = new PlanActivityViewModel();
                viewModel.RoadId = RoadId;
                viewModel.PlanActivities = activityPlanList.Where(s => s.RoadWorkSectionPlanId == SectionPlanId);
                return PartialView("RoadSectionItemActivities", viewModel);
            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "Please select a plan"
                });
            }
        }

        public async Task<IActionResult> GetRoadSectionsSelect(long roadId)
        {
            if (roadId > 0)
            {
                var roadGet = await _roadService.FindByIdAsync(roadId).ConfigureAwait(false);
                if (roadGet.Success)
                {
                    var fYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
                    var roadSectionsForPlanning = Enumerable.Empty<RoadSection>();
                    roadSectionsForPlanning = roadGet.Road.RoadSections;

                    //find the sections not yet planned.
                    var unPlannedSections = _roadSectionService.ListUnPlannedSectionsByRoadIdAsync(roadId, fYearResp.FinancialYear.ID);

                    return PartialView("RoadSectionsSelect", roadSectionsForPlanning);
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "No road was found for the submitted ID. Please check with the administrator."
                    });
                }
            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "No road has been selected for planning."
                });
            }
        }



        public async Task<IActionResult> ShowRoadWorkPlanLineAddEdit(long SectionPlanActivityId)
        {
            PlanActivity sectionPlanActivityModel = new PlanActivity();
            var technologyList = await _technologyService.ListAsync().ConfigureAwait(false);

            ViewBag.TechnologyList = new SelectList(technologyList, "ID", "Code");


            if (SectionPlanActivityId > 0)
            {
                var sectionPlanActivityResponse = await _planActivityService.FindByIdAsync(SectionPlanActivityId).ConfigureAwait(false);
                if (sectionPlanActivityResponse.Success)
                    sectionPlanActivityModel = sectionPlanActivityResponse.PlanActivity;
            }

            return PartialView("SectionWorkplanActivityEditPartialView", sectionPlanActivityModel);
        }

        public async Task<IActionResult> ShowRoadWorkPlanPBCLineAddEdit(long SectionPlanActivityId)
        {
            PlanActivityPBC sectionPlanActivityModel = new PlanActivityPBC();
            var technologyList = await _technologyService.ListAsync().ConfigureAwait(false);

            ViewBag.TechnologyList = new SelectList(technologyList, "ID", "Code");


            if (SectionPlanActivityId > 0)
            {
                var sectionPlanActivityResponse = await _planActivityPBCService.FindByIdAsync(SectionPlanActivityId).ConfigureAwait(false);
                if (sectionPlanActivityResponse.Success)
                    sectionPlanActivityModel = sectionPlanActivityResponse.PlanActivityPBC;
            }

            return PartialView("SectionWorkplanPBCActivityEditPartialView", sectionPlanActivityModel);
        }


        [HttpPost]
        public async Task<IActionResult> RemoveWorkplanActivity(long activityId, string flag)
        {

            if (activityId > 0)
            {
                if (flag == "INSTR")
                {
                    var removeResponse = await _planActivityService.RemoveAsync(activityId).ConfigureAwait(false);
                    if (removeResponse.Success)
                    {
                        return Json(new
                        {
                            Success = true,
                            Message = "Success",
                            Href = Url.Action("GetRoadWorkPlanActivities", "Workplan", new { roadWorkSectionId = removeResponse.PlanActivity.RoadWorkSectionPlanId })
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Could not remove the record."
                        });
                    }

                }
                else
                {
                    var removeResponse = await _planActivityPBCService.RemoveAsync(activityId).ConfigureAwait(false);
                    if (removeResponse.Success)
                    {
                        return Json(new
                        {
                            Success = true,
                            Message = "Success",
                            Href = Url.Action("GetRoadWorkPlanActivities", "Workplan", new { roadWorkSectionId = removeResponse.PlanActivityPBC.RoadWorkSectionPlanId })
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Could not remove the record."
                        });
                    }

                }

            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "Invalid Activity Selected."
                });
            }

        }

        [HttpPost]
        public async Task<IActionResult> UpdatePlanActivity(PlanActivity planActivity)
        {
            if (planActivity != null)
            {
                if (planActivity.ID > 0)
                {
                    //editing the record
                    var updateResponse = await _planActivityService.UpdateAsync(planActivity).ConfigureAwait(false);
                    if (updateResponse.Success)
                    {
                        var roadWorkSectionId = updateResponse.PlanActivity.RoadWorkSectionPlan.ID;
                        return Json(new
                        {
                            Success = true,
                            Message = "Success",
                            Href = Url.Action("GetRoadWorkPlanActivities", "Workplan", new { roadWorkSectionId = roadWorkSectionId })
                        });
                    }
                    else
                    {
                        return PartialView("SectionWorkplanActivityEditPartialView", planActivity);
                    }
                }
                else
                {
                    //is a nenw record
                    var addResponse = await _planActivityService.AddAsync(planActivity).ConfigureAwait(false);
                    if (addResponse.Success)
                    {
                        var id = addResponse.PlanActivity.RoadWorkSectionPlan.Road.ID;
                        return Json(new
                        {
                            Success = true,
                            Message = "Success",
                            Href = Url.Action("GetRoadWorkPlan", "Workplan", new { id = id })
                        });
                    }
                    else
                    {
                        return PartialView("SectionWorkplanActivityEditPartialView", planActivity);
                    }
                }
            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "Submitted record is null"
                });
            }
        }

        /**
         * Updates the PBC Activity values in the database
         * 
         * **/
        [HttpPost]
        public async Task<IActionResult> UpdatePlanActivityPBC(PlanActivityPBC planActivityPBC)
        {
            if (planActivityPBC != null)
            {
                if (planActivityPBC.ID > 0)
                {
                    //editing the record
                    var updateResponse = await _planActivityPBCService.UpdateAsync(planActivityPBC).ConfigureAwait(false);
                    if (updateResponse.Success)
                    {
                        var roadWorkSectionId = updateResponse.PlanActivityPBC.RoadWorkSectionPlan.ID;
                        return Json(new
                        {
                            Success = true,
                            Message = "Success",
                            Href = Url.Action("GetRoadWorkPlanActivities", "Workplan", new { roadWorkSectionId = roadWorkSectionId })
                        });
                    }
                    else
                    {
                        return PartialView("SectionWorkplanActivityEditPartialView", planActivityPBC);
                    }
                }
                else
                {
                    //is a nenw record
                    var addResponse = await _planActivityPBCService.AddAsync(planActivityPBC).ConfigureAwait(false);
                    if (addResponse.Success)
                    {
                        var id = addResponse.PlanActivityPBC.RoadWorkSectionPlan.Road.ID;
                        return Json(new
                        {
                            Success = true,
                            Message = "Success",
                            Href = Url.Action("GetRoadWorkPlan", "Workplan", new { id = id })
                        });
                    }
                    else
                    {
                        return PartialView("SectionWorkplanActivityEditPartialView", planActivityPBC);
                    }
                }
            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "Submitted record is null"
                });
            }
        }

        public async Task<IActionResult> GetAgencyApprovalForm(long workplanBatchId, string? approvalForm)
        {
            //check if is a submission or approval request
            if (string.IsNullOrEmpty(approvalForm))
            {
                ViewBag.ApprovalForm = true;
            }
            else
            {
                ViewBag.ApprovalForm = false;
            }

            //Get Logged in user
            var user = await GetLoggedInUser().ConfigureAwait(false);

            var authority = new Authority();
            if (user != null)
            {
                var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                authority = userAuthority.Authority;
            }

            var viewModel = new SubmitRoadWorkSectionPlanApprovalModel();
            viewModel.Authority = authority;

            //get current financial year
            var fYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
            viewModel.FinancialYear = fYearResp.FinancialYear;
            //retrieve the workplan
            var workplanResp = await _workplanApprovalBatchService.FindByIdAsync(workplanBatchId).ConfigureAwait(false);

            if (workplanResp.Success)
                viewModel.WorkplanApprovalBatch = workplanResp.WorkplanApprovalBatch;

            return PartialView("ApprovalSubmitPartialView", viewModel);
        }

        public async Task<IActionResult> SubmitWorkplanInternal(long workplanBatchId)
        {
            //get current financial year
            var fYearResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);


            var viewModel = new SubmitRoadWorkSectionPlanApprovalModel();
            viewModel.FinancialYear = fYearResp.FinancialYear;
            viewModel.WorkplanApprovalBatch = new WorkplanApprovalBatch();
            //retrieve the road and authority
            var workplanResp = await _workplanApprovalBatchService.FindByIdAsync(workplanBatchId).ConfigureAwait(false);
            if (workplanResp.Success)
            {
                viewModel.WorkplanApprovalBatch = workplanResp.WorkplanApprovalBatch;
            }

            return PartialView("ApprovalSubmitPartialView", viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> SubmitWorkplanInternal([FromForm] SubmitRoadWorkSectionPlanApprovalModel model)
        {
            if (ModelState.IsValid && model != null)
            {
                var workplanAuthority = new Authority();
                //generate unique file name
                string uploadFileName = null;
                if (model.SupportDocument != null)
                {
                    string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "workplanSupportingDocuments");
                    uploadFileName = Guid.NewGuid().ToString() + "_" + model.SupportDocument.FileName;
                    string filePath = Path.Combine(uploadFolder, uploadFileName);

                    if (!Directory.Exists(uploadFolder))
                        Directory.CreateDirectory(uploadFolder);

                    //copy contents of the uploaded file to stream
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await model.SupportDocument.CopyToAsync(stream).ConfigureAwait(false);
                }
                var user = await GetLoggedInUser().ConfigureAwait(false);
                var financialYearId = model.FinancialYear.ID;

                //Update the workplan approval record
                //get the workplan batch  details
                var batchResp = await _workplanApprovalBatchService.FindByIdAsync(model.WorkplanApprovalBatch.ID).ConfigureAwait(false);
                if (model.WorkplanApprovalBatch.ID == 0)
                {
                    //this means it is a new submission
                    var workplanBatch = new WorkplanApprovalBatch();
                    workplanBatch.ApprovalStatus = 1;
                    workplanBatch.FinancialYearId = financialYearId;
                    workplanBatch.AuthorityId = user.AuthorityId;
                    workplanBatch.isAricsDone = model.WorkplanApprovalBatch.isAricsDone;
                    workplanBatch.isR2000AspectsDone = model.WorkplanApprovalBatch.isR2000AspectsDone;
                    workplanBatch.isRoadRelateCourseDone = model.WorkplanApprovalBatch.isRoadRelateCourseDone;
                    workplanBatch.isRoadSafetyEnvironmentDone = model.WorkplanApprovalBatch.isRoadSafetyEnvironmentDone;
                    workplanBatch.isUnitRatesEstimateDone = model.WorkplanApprovalBatch.isUnitRatesEstimateDone;

                    //create the record
                    var createBatchResp = await _workplanApprovalBatchService.AddAsync(workplanBatch).ConfigureAwait(false);
                    if (createBatchResp.Success)
                    {
                        //add the history record
                        WorkplanApprovalHistory approvalHistoryAdd = new WorkplanApprovalHistory
                        {
                            ApprovalBy = user.UserName,
                            ApprovalComment = model.Comment,
                            ApprovalDate = DateTime.UtcNow,
                            approvalStatus = 1,
                            documentLink = uploadFileName,
                            WorkplanApprovalBatchId = createBatchResp.WorkplanApprovalBatch.ID
                        };
                        //insert the history
                        await _workplanApprovalHistoryService.AddAsync(approvalHistoryAdd).ConfigureAwait(false);

                        //update the workplan records for the financial year
                        await _roadWorkSectionPlanService.UpdateBatchId(financialYearId, createBatchResp.WorkplanApprovalBatch.ID, user.Authority.ID, false).ConfigureAwait(false);
                        workplanAuthority = user.Authority;

                    }

                }
                else
                {
                    //this means is an updated on the record for the batch through approvals
                    var workplanApprovalResp = await _workplanApprovalBatchService.FindByIdAsync(model.WorkplanApprovalBatch.ID).ConfigureAwait(false);
                    if (workplanApprovalResp.Success)
                    {
                        var updateRecord = workplanApprovalResp.WorkplanApprovalBatch;

                        if (workplanApprovalResp.WorkplanApprovalBatch.ApprovalStatus == 0) //Submission / Re-submission
                        {
                            updateRecord.ApprovalStatus = 1;
                            updateRecord.isAricsDone = model.WorkplanApprovalBatch.isAricsDone;
                            updateRecord.isR2000AspectsDone = model.WorkplanApprovalBatch.isR2000AspectsDone;
                            updateRecord.isRoadRelateCourseDone = model.WorkplanApprovalBatch.isRoadRelateCourseDone;
                            updateRecord.isRoadSafetyEnvironmentDone = model.WorkplanApprovalBatch.isRoadSafetyEnvironmentDone;
                            updateRecord.isUnitRatesEstimateDone = model.WorkplanApprovalBatch.isUnitRatesEstimateDone;
                        }
                        else if (workplanApprovalResp.WorkplanApprovalBatch.ApprovalStatus == 1) //level 1 approval stage
                        {
                            if (model.WorkplanApproval == "Approve") //if approved, change status to approval by agency / county internal
                            {
                                updateRecord.ApprovalStatus = 2;
                            }
                            else
                            {
                                updateRecord.ApprovalStatus = 0; // revert back to submission stage for editing.

                            }

                        }
                        else if (workplanApprovalResp.WorkplanApprovalBatch.ApprovalStatus == 2) // level 2 approval stage \\ Agency Ends Here
                        {

                            if (model.WorkplanApproval == "Approve") //if approved, change status to approval by KRB first level
                            {
                                if (user.Authority.Type == 1) //if agency
                                {
                                    updateRecord.ApprovalStatus = 4;
                                }
                                else
                                {
                                    updateRecord.ApprovalStatus = 3;
                                }

                            }
                            else
                            {
                                updateRecord.ApprovalStatus = 0; // revert back to submission stage for editing.
                            }
                        }
                        else if (workplanApprovalResp.WorkplanApprovalBatch.ApprovalStatus == 3) // level 3 approval stage
                        {
                            if (model.WorkplanApproval == "Approve") //if approved, change status to approval by KRB first level
                            {
                                updateRecord.ApprovalStatus = 4;
                            }
                            else
                            {
                                updateRecord.ApprovalStatus = 0; // revert back to submission stage for editing.
                            }
                        }
                        else if (workplanApprovalResp.WorkplanApprovalBatch.ApprovalStatus == 4) // level 1 approval stage at KRB
                        {
                            if (model.WorkplanApproval == "Approve") //if approved, change status to approval by KRB first level
                            {
                                updateRecord.ApprovalStatus = 5;
                            }
                            else
                            {
                                updateRecord.ApprovalStatus = 0; // revert back to submission stage for editing.
                            }
                        }
                        else if (workplanApprovalResp.WorkplanApprovalBatch.ApprovalStatus == 5) // level 1 approval stage at KRB
                        {
                            if (model.WorkplanApproval == "Approve") //if approved, change status to approval by KRB first level
                            {
                                updateRecord.ApprovalStatus = 6;
                            }
                            else
                            {
                                updateRecord.ApprovalStatus = 0; // revert back to submission stage for editing.
                            }
                        }
                        else //at last stage of approval at KRB
                        {
                            if (model.WorkplanApproval == "Approve") //if approved, change status to fully by KRB first level
                            {
                                updateRecord.ApprovalStatus = 7;

                            }
                            else
                            {
                                updateRecord.ApprovalStatus = 0; // revert back to submission stage for editing.
                                                                 //set the revision status code.

                            }
                        }

                        var response = await _workplanApprovalBatchService.UpdateAsync(updateRecord).ConfigureAwait(false);
                        if (response.Success)
                        {
                            //add a record for approval history
                            WorkplanApprovalHistory approvalHistoryAdd = new WorkplanApprovalHistory
                            {
                                ApprovalBy = user.UserName,
                                ApprovalComment = model.Comment,
                                ApprovalDate = DateTime.UtcNow,
                                approvalStatus = model.WorkplanApproval == "Approve" ? 1 : 0,
                                documentLink = uploadFileName,
                                WorkplanApprovalBatchId = response.WorkplanApprovalBatch.ID
                            };


                            //inser the history
                            await _workplanApprovalHistoryService.AddAsync(approvalHistoryAdd).ConfigureAwait(false);

                            // UPDATE WORKPLAN FULLY APPROVED
                            if (updateRecord.ApprovalStatus == 7)
                            {
                                //update the workplan records for the financial year
                                await _roadWorkSectionPlanService.UpdateBatchId(financialYearId, response.WorkplanApprovalBatch.ID, updateRecord.Authority.ID, true).ConfigureAwait(false);
                            }
                        }
                        workplanAuthority = updateRecord.Authority;
                    }
                }


                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Href = Url.Action("index", "Workplan", new { viewName = "agency-approval-summary-batch", authorityName = workplanAuthority.Code })
                });
            }
            return Json(new
            {
                Success = false,
                Message = "Please validate the input data before submission."
            });
        }

        public ActionResult DownLoadApprovalDocument(string approvalHistoryDocument)
        {
            return RedirectToAction("DownloadSupportingDocs", "Workplan", new { filename = approvalHistoryDocument, folder = "workplanSupportingDocuments" });
            //(string filename, string folder)
        }
        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }

        #region Prioritization
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> GetPrioritizationYear(int Year)
        {
            try
            {
                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Href = Url.Action("Prioritization", "Workplan", new { Year = Year })
                });

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"WorkplanController.GetPrioritizationYear Error {Environment.NewLine}");
                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Href = Url.Action("Prioritization", "Workplan", new { Year = Year })
                });
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Prioritization(int? Year)
        {
            try
            {
                Authority authority = null;
                var viewModel = new ARICSDataViewModelList();
                var user = await GetLoggedInUser().ConfigureAwait(false);
                var userResp = await _applicationUsersService.IsInRoleAsync(user, "Administrators").ConfigureAwait(false);
                var objectResult = (ObjectResult)userResp.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (OkObjectResult)objectResult;
                    AuthResponse authResponse = (AuthResponse)result2.Value;
                    if (authResponse.Success == true)
                    {
                        //Return all Roads
                        authority = null;
                    }
                    else
                    {
                        //Return for authority that user is placed
                        var authorityResp = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                        authority = authorityResp.Authority;
                    }
                }
                ViewBag.Authority = user.Authority;

                //Specify Year
                int _Year = DateTime.UtcNow.Year;
                bool result = int.TryParse(Year.ToString(), out _Year);
                if (result == false)
                {
                    _Year = DateTime.UtcNow.Year;
                }

                //Set year in view model
                viewModel.Year = _Year;

                //Populate drop down
                await PopulateDropDown(_Year).ConfigureAwait(false);

                //Get arics list for the financial year
                var aricsList = await _aRICSService.GetRoadsAndConditions(_Year, authority).ConfigureAwait(false);

                var aricedRoads = await GetRoads((IList<ARICSData>)aricsList.ARICSData).ConfigureAwait(false);//Get a list of ariced roads
                                                                                                              //aricedRoads.OrderBy(s=>s.);
                if (aricsList.Success)
                {
                    if (aricsList.ARICSData != null)
                    {
                        viewModel.ARICSDatas = aricsList.ARICSData
                            .OrderBy(o => o.PriorityRate).ThenBy(t => t.IRI).ThenBy(a => a.RateOfDeterioration).ToList();
                        //viewModel.ARICSDatas = aricsList.ARICSData;
                    }
                    else
                    {
                        viewModel.ARICSDatas = (Enumerable.Empty<ARICSData>()).ToList();
                    }
                }

                if (authority == null)
                {
                    return View("GetRoadWorkPlanPrioritizationKRB", viewModel);
                }
                else
                {
                    return View("GetRoadWorkPlanPrioritization", viewModel);
                }

            }
            catch (Exception Ex)
            {
                _logger.LogError($"WorkplanController.Prioritization Action {Ex.Message}");
                return Json(null);
            }
        }

        private async Task PopulateDropDown(int _Year)
        {
            IList<ARICSYear> aRICSYears = null;
            //ARICS Year
            var aricsYearResp = await _aRICSYearService.ListAsync().ConfigureAwait(false);
            if (aricsYearResp.Success)
            {
                var objectResult = (ObjectResult)aricsYearResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        aRICSYears = (IList<ARICSYear>)result.Value;
                    }
                }
            }
            ViewData["ARICSYearId"] = new SelectList(aRICSYears, "Year", "Year", _Year);
        }


        private async Task<IList<Road>> GetRoads(IList<ARICSData> _ARICSData)
        {
            IList<Road> _Road = new List<Road>();
            if (_ARICSData != null)
            {
                if (_ARICSData.Any())
                {
                    foreach (var aRICSData in _ARICSData)
                    {
                        _Road.Add(aRICSData.Road);
                    }
                }

            }
            return _Road;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [HttpPost]
        public async Task<JsonResult> OnGetRoadsWithoutARICS(int ARICSYear)
        {
            try
            {
                Authority authority = null;
                IQueryable<RoadViewWithARICSModel> roadsData = null;
                var user = await GetLoggedInUser().ConfigureAwait(false);
                var userResp = await _applicationUsersService.IsInRoleAsync(user, "Administrators").ConfigureAwait(false);
                var objectResult = (ObjectResult)userResp.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (OkObjectResult)objectResult;
                    AuthResponse authResponse = (AuthResponse)result2.Value;
                    if (authResponse.Success == true)
                    {
                        //Return all Roads
                        var roadViewResponse = await _roadService.ListViewWithAricsAsync(ARICSYear).ConfigureAwait(false);
                        roadsData = roadViewResponse.Roads;
                    }
                    else
                    {
                        //Return for authority that user is placed
                        var authorityResp = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                        authority = authorityResp.Authority;
                        var roadViewResponse = await _roadService.ListViewWithAricsAsync(authority, ARICSYear).ConfigureAwait(false);
                        roadsData = roadViewResponse.Roads;
                    }
                }
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();

                // Skiping number of Rows count
                var start = Request.Form["start"].FirstOrDefault();

                // Paging Length 10,20
                var length = Request.Form["length"].FirstOrDefault();

                //string sortOrder = ""; string r;
                //r = String.IsNullOrEmpty(sortOrder) ? "Village_desc" : "";
                //r = sortOrder == "NationalID" ? "NationalID_desc" : "NationalID";

                // Sort Column Name
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                // Sort Column Direction ( asc ,desc)
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                // Search Value from (Search box)
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10,20,50,100)
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    //Sorting
                    if (sortColumnDirection == "desc")
                    {
                        if (sortColumn == "road_number")
                        {
                            roadsData = roadsData.OrderByDescending(s => s.road_number);
                        }
                        else if (sortColumn == "road_name")
                        {
                            roadsData = roadsData.OrderByDescending(s => s.road_name);
                        }
                        else if (sortColumn == "authority_name")
                        {
                            roadsData = roadsData.OrderByDescending(s => s.authority_name);
                        }

                    }
                    else
                    {
                        if (sortColumn == "road_number")
                        {
                            roadsData = roadsData.OrderBy(s => s.road_number);
                        }
                        else if (sortColumn == "road_name")
                        {
                            roadsData = roadsData.OrderBy(s => s.road_name);
                        }
                        else if (sortColumn == "authority_name")
                        {
                            roadsData = roadsData.OrderBy(s => s.authority_name);
                        }
                    }
                }
                //All roads returned
                IEnumerable<RoadViewWithARICSModel> roadsEnum = roadsData.AsEnumerable<RoadViewWithARICSModel>();
                //
                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    roadsEnum = roadsEnum
                        .Where(
                         m => (m.road_name != null && m.road_name.ToLower().Contains(searchValue.ToLower()))
                        || (m.road_number != null && m.road_number.ToString().ToLower().Contains(searchValue.ToLower()))
                        || (m.authority_name != null && m.authority_name.ToString().ToLower().Contains(searchValue.ToLower()))
                    );
                }

                //Get ARICED Roads
                IEnumerable<ARICSData> aRICSDatas;
                if (authority == null)
                {
                    aRICSDatas = (await _aRICSService.GetRoadsAndConditions(ARICSYear, null).ConfigureAwait(false)).ARICSData;
                }
                else
                {
                    aRICSDatas = (await _aRICSService.GetRoadsAndConditions(ARICSYear, authority).ConfigureAwait(false)).ARICSData;
                }

                var aricedRoads = await GetRoads((IList<ARICSData>)aRICSDatas).ConfigureAwait(false);//Get a list of ariced roads

                //return roads without ARICS
                var result = roadsEnum.Where(p => !aricedRoads.Any(p2 => p2.ID == p.id));

                //total number of rows count 
                recordsTotal = result.Count();
                //Paging 
                var data = result.Skip(skip).Take(pageSize).ToList();

                //Returning Json Data
                return new JsonResult(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = data
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"Roles.Index Page Error: {Ex.Message} " +
                    $"{Environment.NewLine}");
                return new JsonResult(null);
            }
        }

        [Authorize(Claims.Permission.RoadPrioritization.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> PrioritizeRoad(long RoadId, int ARICSYear)
        {
            try
            {
                PrioritizeRoadViewModel prioritizeRoadViewModel = new PrioritizeRoadViewModel();

                //Get Road
                var roadResp = await _roadService.FindByIdAsync(RoadId).ConfigureAwait(false);
                prioritizeRoadViewModel.Road = roadResp.Road;

                //Get ARICS Year
                ARICSYear aRICSYear = null;
                var aricsYearResp = await _aRICSYearService.FindByYearAsync(ARICSYear).ConfigureAwait(false);
                if (aricsYearResp.Success)
                {
                    var objectResult = (ObjectResult)aricsYearResp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            aRICSYear = (ARICSYear)result.Value;
                            prioritizeRoadViewModel.ARICSYear = aRICSYear;

                            //Set ARICS year Id
                            prioritizeRoadViewModel.ARICSYearId = aRICSYear.ID;
                        }
                    }
                }


                //Get road condition for year
                prioritizeRoadViewModel.RoadCondition = prioritizeRoadViewModel.Road.RoadConditions
                                    .Where(y => y.Year == DateTime.UtcNow.Year)
                                    .FirstOrDefault();

                var user = await GetLoggedInUser().ConfigureAwait(false);
                ViewBag.Authority = user.Authority;

                //ViewData["RoadPrioritizationId"] = new SelectList(roadPriorityResp.RoadPrioritization, "ID", "Rate", roadPrioritizationID);

                return View(prioritizeRoadViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"WorkplanController.PrioritizeRoad Action {Ex.Message}");
                return View();
            }
        }

        [Authorize(Claims.Permission.RoadPrioritization.Change)]
        [HttpPost]
        public async Task<IActionResult> EditRoadPriority(long RoadPriorityRate, long RoadId, String Comment, long RoadConditionId, int ARICSYear)
        {

            var user = await GetLoggedInUser().ConfigureAwait(false);
            var authority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);

            //Get the road
            var roadResp = await _roadService.FindByIdAsync(RoadId).ConfigureAwait(false);
            var road = roadResp.Road;

            //Check if you are assigned to that authority
            if (authority.Authority.ID != road.Authority.ID)
            {
                string msg = $"Message:" +
                $" Road {road.RoadNumber} is for authority {road.Authority.Code + ":" + road.Authority.Name}" +
                $" while you are assigned to authority : {authority.Authority.Code + ":" + road.Authority.Name}";
                return Json(new
                {
                    Success = false,
                    data = msg,
                    Message = msg,
                    Href = Url.Action("Prioritization", "Workplan")
                });
            }

            //check priorirty rate for year and for the authority
            var roadcondResp = await _roadConditionService.FindByPriorityRateAsync(authority.Authority.ID, ARICSYear, RoadPriorityRate).ConfigureAwait(false);
            if (roadcondResp.Success)
            {
                if (roadcondResp.RoadCondtion.ID != RoadConditionId)
                {
                    return Json(new
                    {
                        Success = false,
                        data = $"Message:{roadcondResp.Message} " +
                        $" Priority Rate Already Exists",
                        Message = "Failed",
                        Href = Url.Action("Prioritization", "Workplan")
                    });
                }
            }

            //If no road condition then create one and add the road priority

            RoadCondition roadCondition1 = null;
            roadCondition1 = road.RoadConditions.Where(y => y.Year == ARICSYear).SingleOrDefault();
            if (roadCondition1 == null)
            {
                //Add road condition
                RoadCondition roadCondition = new RoadCondition();
                roadCondition.ComputationTime = DateTime.UtcNow;
                roadCondition.ARD = 0;
                roadCondition.Year = ARICSYear;
                roadCondition.RoadId = road.ID;
                roadCondition.PriorityRate = RoadPriorityRate;//Default 0 meaning unclassified
                roadCondition.Comment = Comment;
                var roadConditionResponse = await _roadConditionService.AddAsync(roadCondition).ConfigureAwait(false);

            }
            else
            {
                //update road condition
                RoadCondition roadCondition = road.RoadConditions.FirstOrDefault();
                roadCondition.PriorityRate = RoadPriorityRate;//Default 0 meaning unclassified
                roadCondition.Comment = Comment;
                var roadConditionResponse2 = await _roadConditionService.Update(roadCondition.ID, roadCondition).ConfigureAwait(false);
            }

            return Json(new
            {
                Success = true,
                Message = "Success",
                Href = Url.Action("Prioritization", "Workplan")
            });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> SupportingDocs(long RoadId)
        {
            try
            {
                //Create Model
                PrioritizeRoadViewModel prioritizeRoadViewModel = new PrioritizeRoadViewModel();

                //Get road
                var roadResp = await _roadService.FindByIdAsync(RoadId).ConfigureAwait(false);
                prioritizeRoadViewModel.Road = roadResp.Road;

                //Get road condition for year
                prioritizeRoadViewModel.RoadCondition = prioritizeRoadViewModel.Road.RoadConditions
                                    .Where(y => y.Year == DateTime.UtcNow.Year)
                                    .FirstOrDefault();

                //Set Referer
                prioritizeRoadViewModel.Referer = Request.Headers["Referer"].ToString();
                var user = await GetLoggedInUser().ConfigureAwait(false);
                ViewBag.Authority = user.Authority;
                return View(prioritizeRoadViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"WorkplanController.Supporting Docs Action {Ex.Message}");
                return View();
            }
        }

        [Authorize(Claims.Permission.RoadPrioritization.Change)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> UploadSupportingDocs()
        {
            try
            {
                //Create Model
                PrioritizeRoadViewModel prioritizeRoadViewModel = new PrioritizeRoadViewModel();

                //Get RoadId
                long RoadId;
                bool results = long.TryParse(Request.Form["RoadId"].ToString(), out RoadId);

                //Get road
                var roadResp = await _roadService.FindByIdAsync(RoadId).ConfigureAwait(false);
                prioritizeRoadViewModel.Road = roadResp.Road;

                //Upload the files
                //Check if files injected in request object
                if (Request.Form.Files.Count > 0)
                {
                    try
                    {
                        var files = Request.Form.Files;
                        for (int i = 0; i < files.Count; i++)
                        {
                            var file = files[i];
                            string fname;
                            string x = Request.Headers["User-Agent"].ToString();//.Contains("Edge")
                            if (Request.Headers["User-Agent"].ToString().Contains("IE") || Request.Headers["User-Agent"].ToString().Contains("INTERNETEXPLORER")
                                || Request.Headers["User-Agent"].ToString().Contains("Edge"))
                            {
                                string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                fname = testfiles[testfiles.Length - 1];
                            }
                            else
                            {
                                fname = file.FileName;
                            }
                            var path = Path.Combine(
                            _hostingEnvironment.WebRootPath, "uploads", "prioritization", fname);

                            using (var fileStream = new FileStream(path, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream).ConfigureAwait(false);
                            }

                            //Register with file uploads
                            Upload upload = new Upload();
                            upload.filename = fname;
                            upload.ForeignId = prioritizeRoadViewModel.Road.ID;
                            upload.type = "prioritization";

                            var uploadResp = await _uploadService.AddAsync(upload).ConfigureAwait(false);
                        }

                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"SubmitBudget() File Upload Fails Error: {Ex.Message} " +
                        $"{Environment.NewLine}");
                    }
                }

                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Href = Url.Action("SupportingDocs", "Workplan", new { RoadId = RoadId })
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError($"WorkplanController.SupportingDocs Action {Ex.Message}");
                return Json(new
                {
                    Success = false,
                    Message = "Error",
                    Href = Url.Action("Prioritization", "Workplan")
                });
            }
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> GetPrioritizationUploads(string Type, long RoadId)
        {
            try
            {
                var UploadListResponse = await _uploadService.ListAsync(Type, RoadId).ConfigureAwait(false);
                IList<Upload> ARICSUpload = (IList<Upload>)UploadListResponse.Upload;
                return Json(ARICSUpload);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"ARICSController.GetARICSUploads Error: {Ex.Message}");
                return Json(null);
            }
        }

        [Authorize(Claims.Permission.RoadPrioritization.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> PrioritizeRoadDelete(long RoadId, long RoadConditionId)
        {
            try
            {
                PrioritizeRoadViewModel prioritizeRoadViewModel = new PrioritizeRoadViewModel();

                //Get Road
                var roadResp = await _roadService.FindByIdAsync(RoadId).ConfigureAwait(false);
                prioritizeRoadViewModel.Road = roadResp.Road;


                //Get road condition for year
                //prioritizeRoadViewModel.RoadCondition = prioritizeRoadViewModel.Road.RoadConditions
                //                    .Where(y => y.Year == DateTime.UtcNow.Year)
                //                    .FirstOrDefault();

                var respRoadcontion = await _roadConditionService.FindByIdAsync(RoadConditionId).ConfigureAwait(false);
                if (respRoadcontion.Success)
                {
                    prioritizeRoadViewModel.RoadCondition = respRoadcontion.RoadCondtion;
                }
                var user = await GetLoggedInUser().ConfigureAwait(false);
                ViewBag.Authority = user.Authority;

                return View(prioritizeRoadViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"WorkplanController.PrioritizeRoad Action {Ex.Message}");
                return View();
            }
        }

        [Authorize(Claims.Permission.RoadPrioritization.Delete)]
        [HttpPost]
        public async Task<IActionResult> DeleteRoadPriority(long RoadConditionId, long RoadId)
        {

            var user = await GetLoggedInUser().ConfigureAwait(false);
            var authority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);

            //Get the road
            var roadResp = await _roadService.FindByIdAsync(RoadId).ConfigureAwait(false);
            var road = roadResp.Road;

            //Check if you are assigned to that authority
            if (authority.Authority.ID != road.Authority.ID)
            {
                string msg = $"Message:" +
                $" Road {road.RoadNumber} is for authority {road.Authority.Code + ":" + road.Authority.Name}" +
                $" while you are assigned to authority : {authority.Authority.Code + ":" + road.Authority.Name}";
                return Json(new
                {
                    Success = false,
                    data = msg,
                    Message = msg,
                    Href = Url.Action("Prioritization", "Workplan")
                });
            }

            //check priorirty rate for year and for the authority
            var roadcondResp = await _roadConditionService.FindByIdAsync(RoadConditionId).ConfigureAwait(false);
            if (roadcondResp.Success)
            {
                RoadCondition rdcondition = roadcondResp.RoadCondtion;

                if (rdcondition.ARD < 1 && rdcondition.IRI < 1)
                {
                    //Detach the first entry before attaching your updated entry
                    var respDetach = await _roadConditionService.DetachFirstEntryAsync(rdcondition).ConfigureAwait(false);

                    //delete
                    var roadConditionResponse = await _roadConditionService.RemoveAsync(rdcondition.ID).ConfigureAwait(false);
                }
                else
                {
                    //Detach

                    //update
                    rdcondition.PriorityRate = 999999;
                    var roadConditionResponse = await _roadConditionService.Update(rdcondition.ID, rdcondition).ConfigureAwait(false);
                }

            }

            //If no road condition then create one and add the road priority

            return Json(new
            {
                Success = true,
                Message = "Success",
                Href = Url.Action("Prioritization", "Workplan")
            });
        }

        #endregion

        #region Download

        public async Task<IActionResult> DownloadSupportingDocs(string filename, string folder)
        {
            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(
                           _hostingEnvironment.WebRootPath, "uploads",
                           folder, filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory).ConfigureAwait(false);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> DeletePrioritizationAttachment(long Id, string filename, long RoadId)
        {
            try
            {
                Upload upload = null;
                //delete the file
                Boolean FileDelete = DeleteFile(filename, "prioritization");

                var uploadResponse = await _uploadService.RemoveAsync(Id).ConfigureAwait(false);
                upload = uploadResponse.Upload;
                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    data = upload,
                    Href = Url.Action("SupportingDocs", "Workplan", new { RoadId = RoadId })
                });

            }
            catch (Exception Ex)
            {
                _logger.LogError($"DeletePrioritizationAttachment Error: {Ex.Message}");
                return Json(null);
            }
        }

        private Boolean DeleteFile(string filename, string subFolder)
        {
            try
            {
                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", subFolder, filename);
                FileInfo File = new FileInfo(path);

                // Check if file exists with its full path    
                if (File.Exists)
                {
                    // If file found, delete it    
                    File.Delete();
                    return false;

                }
                else return false;
            }
            catch (IOException IOExp)
            {
                string msg = $"Error Message: {IOExp.Message.ToString()} \r\n" +
                    $"Inner Exception: {IOExp.InnerException.ToString()} \r\n" +
                    $"Stack Trace:  {IOExp.StackTrace.ToString()}";
                _logger.LogError($"ARICSController.DeleteFile: \r\n {msg}");
                return false;
            }

        }

        #endregion

        #region Export To Excel
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> ExportPrioritized(int? Year)
        {
            try
            {
                Authority authority = null;
                var viewModel = new ARICSDataViewModelList();
                var user = await GetLoggedInUser().ConfigureAwait(false);
                var userResp = await _applicationUsersService.IsInRoleAsync(user, "Administrators").ConfigureAwait(false);
                var objectResult = (ObjectResult)userResp.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (OkObjectResult)objectResult;
                    AuthResponse authResponse = (AuthResponse)result2.Value;
                    if (authResponse.Success == true)
                    {
                        //Return all Roads
                        authority = null;
                    }
                    else
                    {
                        //Return for authority that user is placed
                        var authorityResp = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                        authority = authorityResp.Authority;
                    }
                }
                var aricsList = await _aRICSService.GetRoadsAndConditions(Year, authority).ConfigureAwait(false);

                var aricedRoads = await GetRoads((IList<ARICSData>)aricsList.ARICSData).ConfigureAwait(false);//Get a list of ariced roads
                                                                                                              //aricedRoads.OrderBy(s=>s.);
                if (aricsList.Success)
                {
                    if (aricsList.ARICSData != null)
                    {
                        var aricsData = aricsList.ARICSData
                            .OrderBy(o => o.PriorityRate).ThenBy(t => t.IRI).ThenBy(a => a.RateOfDeterioration);

                        //Write to Excel
                        MemoryStream stream = new MemoryStream();
                        JsonResult myjson = await WriteToExcelPrioritizedRoads(aricsList.ARICSData, Year, stream).ConfigureAwait(false);

                        return Json(myjson);
                    }
                    else
                    {
                        return Json(null);
                    }
                }
                else
                {
                    return Json(null);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"WorkplanController.ExportPrioritized Error {Environment.NewLine}");
                return Json(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<JsonResult> WriteToExcelPrioritizedRoads(IEnumerable<ARICSData> ARICSDatas, int? Year, MemoryStream stream)
        {
            //Create a new ExcelPackage  
            string handle = Guid.NewGuid().ToString();
            using (ExcelPackage excelPackage = new ExcelPackage(stream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = "KRB";
                excelPackage.Workbook.Properties.Title = "Prioritized-Roads";
                excelPackage.Workbook.Properties.Subject = "Prioritized Roads Report";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add($"Prioritized_roads");

                //RA/CG Name
                worksheet.Cells["A1"].Value = $"RA/CG Name";
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.Font.Name = "Arial";
                worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1"].Style.Font.Bold = true;

                var user = await GetLoggedInUser().ConfigureAwait(false);
                var authorityResp = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                var authority = authorityResp.Authority;

                worksheet.Cells["B1"].Value = $"{authority.Name}";
                worksheet.Cells["B1"].Style.Font.Size = 16;
                worksheet.Cells["B1"].Style.Font.Name = "Arial";
                worksheet.Cells["B1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B1"].Style.Font.Bold = true;

                //RA/CG Code
                worksheet.Cells["A2"].Value = $"RA/CG Code";
                worksheet.Cells["A2"].Style.Font.Size = 16;
                worksheet.Cells["A2"].Style.Font.Name = "Arial";
                worksheet.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2"].Style.Font.Bold = true;


                worksheet.Cells["B2"].Value = $"{authority.Code}";
                worksheet.Cells["B2"].Style.Font.Size = 16;
                worksheet.Cells["B2"].Style.Font.Name = "Arial";
                worksheet.Cells["B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B2"].Style.Font.Bold = true;

                //Year
                worksheet.Cells["A3"].Value = $"Year";
                worksheet.Cells["A3"].Style.Font.Size = 16;
                worksheet.Cells["A3"].Style.Font.Name = "Arial";
                worksheet.Cells["A3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A3"].Style.Font.Bold = true;

                worksheet.Cells["B3"].Value = $"{Year}";
                worksheet.Cells["B3"].Style.Font.Size = 16;
                worksheet.Cells["B3"].Style.Font.Name = "Arial";
                worksheet.Cells["B3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B3"].Style.Font.Bold = true;

                //Row Number 6
                worksheet.Cells["A4"].Value = $"Roads Priority List";
                worksheet.Cells["A4:F4"].Merge = true;
                worksheet.Cells["A4:F4"].Style.Font.Size = 16;
                worksheet.Cells["A4:F4"].Style.Font.Name = "Arial";
                worksheet.Cells["A4:F4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A4:F4"].Style.Font.Bold = true;

                worksheet.Cells["A5"].Value = "Road ID";
                worksheet.Cells["A5"].Style.Font.Size = 12;
                worksheet.Cells["A5"].Style.Font.Name = "Arial";
                worksheet.Cells["A5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A5"].Style.Font.Bold = true;

                worksheet.Cells["B5"].Value = "Road Link";
                worksheet.Cells["B5"].Style.Font.Size = 12;
                worksheet.Cells["B5"].Style.Font.Name = "Arial";
                worksheet.Cells["B5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B5"].Style.Font.Bold = true;


                worksheet.Cells["C5"].Value = "ARD";
                worksheet.Cells["C5"].Style.Font.Size = 12;
                worksheet.Cells["C5"].Style.Font.Name = "Arial";
                worksheet.Cells["C5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["C5"].Style.Font.Bold = true;

                worksheet.Cells["D5"].Value = "IRI";
                worksheet.Cells["D5"].Style.Font.Size = 12;
                worksheet.Cells["D5"].Style.Font.Name = "Arial";
                worksheet.Cells["D5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["D5"].Style.Font.Bold = true;

                worksheet.Cells["E5"].Value = "Priority";
                worksheet.Cells["E5"].Style.Font.Size = 12;
                worksheet.Cells["E5"].Style.Font.Name = "Arial";
                worksheet.Cells["E5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["E5"].Style.Font.Bold = true;

                worksheet.Cells["F5"].Value = "Comment (Only For Roads without ARICS)";
                worksheet.Cells["F5"].Style.Font.Size = 12;
                worksheet.Cells["F5"].Style.Font.Name = "Arial";
                worksheet.Cells["F5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["F5"].Style.Font.Bold = true;
                //You could also use [line, column] notation:
                //Worksheet.Cells[1, 2].Value = "This is cell B1!";


                int i = 6;
                foreach (var aricsDATA in ARICSDatas)
                {
                    try
                    {
                        worksheet.Cells[i, 1].Value = aricsDATA.Road.RoadNumber; //Road ID
                    }
                    catch (Exception Ex)
                    {

                        _logger.LogError(Ex, $"WorkplanController.WriteToExcelPrioritizedRoads Error {Environment.NewLine}");
                    }
                    try
                    {
                        worksheet.Cells[i, 2].Value = aricsDATA.Road.RoadName; //Road link
                    }
                    catch (Exception Ex)
                    {

                        _logger.LogError(Ex, $"WorkplanController.WriteToExcelPrioritizedRoads Error {Environment.NewLine}");
                    }
                    try
                    {
                        worksheet.Cells[i, 3].Value = aricsDATA.RateOfDeterioration; //ARD
                    }
                    catch (Exception Ex)
                    {

                        _logger.LogError(Ex, $"WorkplanController.WriteToExcelPrioritizedRoads Error {Environment.NewLine}");
                    }
                    try
                    {
                        worksheet.Cells[i, 4].Value = aricsDATA.IRI; //IRI
                    }
                    catch (Exception Ex)
                    {

                        _logger.LogError(Ex, $"WorkplanController.WriteToExcelPrioritizedRoads Error {Environment.NewLine}");
                    }
                    try
                    {
                        worksheet.Cells[i, 5].Value = aricsDATA.PriorityRate; //Priority
                    }
                    catch (Exception Ex)
                    {

                        _logger.LogError(Ex, $"WorkplanController.WriteToExcelPrioritizedRoads Error {Environment.NewLine}");
                    }

                    try
                    {
                        worksheet.Cells[i, 6].Value = aricsDATA.Comment; //Comment
                    }
                    catch (Exception Ex)
                    {

                        _logger.LogError(Ex, $"WorkplanController.WriteToExcelPrioritizedRoads Error {Environment.NewLine}");
                    }
                    i++;

                }

                //Formating
                worksheet.Cells["A4:F" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A4:F" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A4:F" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A4:F" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                //Autofit columns
                //Make all text fit the cells
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                _cache.Set(handle, excelPackage.GetAsByteArray(),
                    new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));

                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Href = Url.Action("Download2", "Workplan", new { fileGuid = handle, FileName = "Prioritized_roads.xlsx" })
                });

            }

            //stream.Close();
        }

        public async Task<JsonResult> ExportPrioritizedKRB(int? Year, long? AuthorityId)
        {
            try
            {
                Authority authority = null;
                var viewModel = new ARICSDataViewModelList();

                long _AuthorityId = 0;
                bool result = long.TryParse(AuthorityId.ToString(), out _AuthorityId);

                //Return for authority the supplied authorityid
                var authorityResp = await _authorityService.FindByIdAsync(_AuthorityId).ConfigureAwait(false);
                authority = authorityResp.Authority;

                var aricsList = await _aRICSService.GetRoadsAndConditions(Year, authority).ConfigureAwait(false);

                var aricedRoads = await GetRoads((IList<ARICSData>)aricsList.ARICSData).ConfigureAwait(false);//Get a list of ariced roads
                                                                                                              //aricedRoads.OrderBy(s=>s.);
                if (aricsList.Success)
                {
                    if (aricsList.ARICSData != null)
                    {
                        var aricsData = aricsList.ARICSData
                            .OrderBy(o => o.Road.Authority.Name)
                            .ThenBy(t => t.PriorityRate)
                            .ThenBy(t => t.IRI)
                            .ThenBy(a => a.RateOfDeterioration)
                            .ToList();

                        //Write to Excel
                        MemoryStream stream = new MemoryStream();
                        JsonResult myjson = await WriteToExcelPrioritizedRoadsKRB((IList<ARICSData>)aricsList.ARICSData, Year, stream, authority).ConfigureAwait(false);

                        return Json(myjson);
                    }
                    else
                    {
                        return Json(null);
                    }
                }
                else
                {
                    return Json(null);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"WorkplanController.ExportPrioritized Error {Environment.NewLine}");
                return Json(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<JsonResult> WriteToExcelPrioritizedRoadsKRB(IList<ARICSData> ARICSDatas, int? Year, MemoryStream stream, Authority authority)
        {
            //Create a new ExcelPackage  
            string handle = Guid.NewGuid().ToString();
            using (ExcelPackage excelPackage = new ExcelPackage(stream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = "KRB";
                excelPackage.Workbook.Properties.Title = "Prioritized-Roads";
                excelPackage.Workbook.Properties.Subject = "Prioritized Roads Report";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add($"Prioritized_roads");

                //RA/CG Name
                worksheet.Cells["A1"].Value = $"RA/CG Name";
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.Font.Name = "Arial";
                worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1"].Style.Font.Bold = true;

                worksheet.Cells["B1"].Value = $"{authority.Name}";
                worksheet.Cells["B1"].Style.Font.Size = 16;
                worksheet.Cells["B1"].Style.Font.Name = "Arial";
                worksheet.Cells["B1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B1"].Style.Font.Bold = true;

                //RA/CG Code
                worksheet.Cells["A2"].Value = $"RA/CG Code";
                worksheet.Cells["A2"].Style.Font.Size = 16;
                worksheet.Cells["A2"].Style.Font.Name = "Arial";
                worksheet.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2"].Style.Font.Bold = true;


                worksheet.Cells["B2"].Value = $"{authority.Code}";
                worksheet.Cells["B2"].Style.Font.Size = 16;
                worksheet.Cells["B2"].Style.Font.Name = "Arial";
                worksheet.Cells["B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B2"].Style.Font.Bold = true;

                //Year
                worksheet.Cells["A3"].Value = $"Year";
                worksheet.Cells["A3"].Style.Font.Size = 16;
                worksheet.Cells["A3"].Style.Font.Name = "Arial";
                worksheet.Cells["A3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A3"].Style.Font.Bold = true;

                worksheet.Cells["B3"].Value = $"{Year}";
                worksheet.Cells["B3"].Style.Font.Size = 16;
                worksheet.Cells["B3"].Style.Font.Name = "Arial";
                worksheet.Cells["B3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B3"].Style.Font.Bold = true;

                //Row Number 6
                worksheet.Cells["A4"].Value = $"Roads Priority List";
                worksheet.Cells["A4:F4"].Merge = true;
                worksheet.Cells["A4:F4"].Style.Font.Size = 16;
                worksheet.Cells["A4:F4"].Style.Font.Name = "Arial";
                worksheet.Cells["A4:F4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A4:F4"].Style.Font.Bold = true;

                worksheet.Cells["A5"].Value = "Road ID";
                worksheet.Cells["A5"].Style.Font.Size = 12;
                worksheet.Cells["A5"].Style.Font.Name = "Arial";
                worksheet.Cells["A5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A5"].Style.Font.Bold = true;

                worksheet.Cells["B5"].Value = "Road Link";
                worksheet.Cells["B5"].Style.Font.Size = 12;
                worksheet.Cells["B5"].Style.Font.Name = "Arial";
                worksheet.Cells["B5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B5"].Style.Font.Bold = true;


                worksheet.Cells["C5"].Value = "ARD";
                worksheet.Cells["C5"].Style.Font.Size = 12;
                worksheet.Cells["C5"].Style.Font.Name = "Arial";
                worksheet.Cells["C5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["C5"].Style.Font.Bold = true;

                worksheet.Cells["D5"].Value = "IRI";
                worksheet.Cells["D5"].Style.Font.Size = 12;
                worksheet.Cells["D5"].Style.Font.Name = "Arial";
                worksheet.Cells["D5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["D5"].Style.Font.Bold = true;

                worksheet.Cells["E5"].Value = "Priority";
                worksheet.Cells["E5"].Style.Font.Size = 12;
                worksheet.Cells["E5"].Style.Font.Name = "Arial";
                worksheet.Cells["E5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["E5"].Style.Font.Bold = true;

                worksheet.Cells["F5"].Value = "Comment (Only For Roads without ARICS)";
                worksheet.Cells["F5"].Style.Font.Size = 12;
                worksheet.Cells["F5"].Style.Font.Name = "Arial";
                worksheet.Cells["F5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["F5"].Style.Font.Bold = true;
                //You could also use [line, column] notation:
                //Worksheet.Cells[1, 2].Value = "This is cell B1!";

                int i = 6;
                foreach (var aricsDATA in ARICSDatas)
                {
                    try
                    {
                        worksheet.Cells[i, 1].Value = aricsDATA.Road.Authority.Name; //Road Institution
                    }
                    catch (Exception Ex)
                    {

                        _logger.LogError(Ex, $"WorkplanController.WriteToExcelPrioritizedRoads Error {Environment.NewLine}");
                    }
                    try
                    {
                        worksheet.Cells[i, 2].Value = aricsDATA.Road.RoadNumber; //Road ID
                    }
                    catch (Exception Ex)
                    {

                        _logger.LogError(Ex, $"WorkplanController.WriteToExcelPrioritizedRoads Error {Environment.NewLine}");
                    }
                    try
                    {
                        worksheet.Cells[i, 3].Value = aricsDATA.Road.RoadName; //Road link
                    }
                    catch (Exception Ex)
                    {

                        _logger.LogError(Ex, $"WorkplanController.WriteToExcelPrioritizedRoads Error {Environment.NewLine}");
                    }
                    try
                    {
                        worksheet.Cells[i, 4].Value = aricsDATA.RateOfDeterioration; //ARD
                    }
                    catch (Exception Ex)
                    {

                        _logger.LogError(Ex, $"WorkplanController.WriteToExcelPrioritizedRoads Error {Environment.NewLine}");
                    }
                    try
                    {
                        worksheet.Cells[i, 5].Value = aricsDATA.IRI; //IRI
                    }
                    catch (Exception Ex)
                    {

                        _logger.LogError(Ex, $"WorkplanController.WriteToExcelPrioritizedRoads Error {Environment.NewLine}");
                    }
                    try
                    {
                        worksheet.Cells[i, 6].Value = aricsDATA.PriorityRate; //Priority
                    }
                    catch (Exception Ex)
                    {

                        _logger.LogError(Ex, $"WorkplanController.WriteToExcelPrioritizedRoads Error {Environment.NewLine}");
                    }

                    try
                    {
                        worksheet.Cells[i, 7].Value = aricsDATA.Comment; //Comment
                    }
                    catch (Exception Ex)
                    {

                        _logger.LogError(Ex, $"WorkplanController.WriteToExcelPrioritizedRoads Error {Environment.NewLine}");
                    }
                    i++;

                }

                //Formating
                worksheet.Cells["A3:G" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A3:G" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A3:G" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A3:G" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                //Autofit columns
                //Make all text fit the cells
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                _cache.Set(handle, excelPackage.GetAsByteArray(),
                    new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));

                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Href = Url.Action("Download2", "Workplan", new { fileGuid = handle, FileName = "Prioritized_roads.xlsx" })
                });

            }

            //stream.Close();
        }

        public ActionResult Download2(string fileGuid, string fileName)
        {

            if (_cache.Get<byte[]>(fileGuid) != null)
            {
                byte[] data = _cache.Get<byte[]>(fileGuid);
                _cache.Remove(fileGuid); //cleanup here as we don't need it in cache anymore
                return File(data, "application/vnd.ms-excel", fileName);
            }
            else
            {
                // Something has gone wrong...
                return View("Error"); // or whatever/wherever you want to return the user
            }
        }
        #endregion
    }
}
