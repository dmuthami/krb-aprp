using System.Drawing;
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Services.Communication;
using APRP.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace APRP.Web.Controllers
{
    [Authorize]
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutAttribute))]
    public class RevenueCollectionController : Controller
    {
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly IFinancialYearService _financialYearService;
        private readonly ILogger _logger;
        private readonly IBudgetCeilingHeaderService _budgetCeilingHeaderService;
        private readonly IBudgetCeilingService _budgetCeilingService;
        private readonly IRoadWorkBudgetLineService _roadWorkBudgetLinesService;
        private readonly IRoadWorkBudgetHeaderService _roadWorkBudgetHeaderService;
        private readonly IRevenueCollectionService _revenueCollectionService;
        private readonly IDisbursementService _disbursementService;
        private readonly IAllocationService _allocationService;
        private readonly IAllocationCodeUnitService _allocationCodeUnitService;
        private readonly IWorkCategoryFundingMatrixService _workCategoryFundingMatrixService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IUploadService _uploadService;
        private readonly IMemoryCache _cache;
        private readonly IBudgetCeilingComputationService _budgetCeilingComputationService;
        private readonly ICSAllocationService _cSAllocationService;
        private readonly IDisbursementCodeListService _disbursementCodeListService;

        public RevenueCollectionController(
            ILogger<RevenueCollectionController> logger,
            IApplicationUsersService applicationUsersService, IAuthorityService authorityService, IFinancialYearService financialYearService,
            IBudgetCeilingService budgetCeilingService,
            IBudgetCeilingHeaderService budgetCeilingHeaderService,
            IRoadWorkBudgetLineService roadWorkBudgetLinesService,
            IRoadWorkBudgetHeaderService roadWorkBudgetHeaderService,
            IRevenueCollectionService revenueCollectionService,
            IDisbursementService disbursementService,
            IAllocationService allocationService,
            IAllocationCodeUnitService allocationCodeUnitService,
            IWorkCategoryFundingMatrixService workCategoryFundingMatrixService,
            IWebHostEnvironment hostingEnvironment,
            IUploadService uploadService, IMemoryCache cache,
            IBudgetCeilingComputationService budgetCeilingComputationService,
            ICSAllocationService cSAllocationService, IDisbursementCodeListService disbursementCodeListService)
        {
            _roadWorkBudgetLinesService = roadWorkBudgetLinesService;
            _roadWorkBudgetHeaderService = roadWorkBudgetHeaderService;
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _logger = logger;
            _financialYearService = financialYearService;
            _budgetCeilingService = budgetCeilingService;
            _budgetCeilingHeaderService = budgetCeilingHeaderService;
            _revenueCollectionService = revenueCollectionService;
            _disbursementService = disbursementService;
            _allocationService = allocationService;
            _allocationCodeUnitService = allocationCodeUnitService;
            _workCategoryFundingMatrixService = workCategoryFundingMatrixService;
            _hostingEnvironment = hostingEnvironment;
            _uploadService = uploadService;
            _cache = cache;
            _budgetCeilingComputationService = budgetCeilingComputationService;
            _cSAllocationService = cSAllocationService;
            _disbursementCodeListService = disbursementCodeListService;
        }

        #region Revenue collection
        [HttpPost]
        [Authorize(Claims.Permission.RevenueCollection.Delete)]
        public async Task<IActionResult> RemoveRevenueItem(long id)
        {
            //Call service to removethe revenue item from the financial year
            var revenueCollResponse = await _revenueCollectionService.RemoveAsync(id).ConfigureAwait(false);

            return Json(new
            {
                Success = revenueCollResponse.Success,
                Message = "Success",
                Href = Url.Action("Index", "RevenueCollection")
            });
        }

        [HttpPost]
        [Authorize(Claims.Permission.Disbursement.Delete)]
        public async Task<IActionResult> RemoveDisbursementItem(long id)
        {
            //Call service to remove the revenue item from the financial year
            var disbursementResponse = await _disbursementService.RemoveAsync(id).ConfigureAwait(false);

            return Json(new
            {
                Success = disbursementResponse.Success,
                Message = "Success",
                Href = Url.Action("Index", "RevenueCollection")
            });
        }

        [Authorize(Claims.Permission.RevenueCollection.View)]
        // GET: RevenueCollection
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ActionResult> Index(int? id)
        {
            try
            {
                //Get logged in user
                ApplicationUser _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);

                //Set Authority
                RevenueCollectionViewModel revenueCollectionViewModel = new RevenueCollectionViewModel();
                revenueCollectionViewModel.Authority = _ApplicationUser.Authority;
                //Revenue= 0
                //CS allocation = 1
                int _ID = 0;
                bool result = int.TryParse(id.ToString(), out _ID);
                ViewData["SelectedSctionID"] = _ID;

                var respFinancialYear = await _financialYearService.FindCurrentYear().ConfigureAwait(false);

                if (respFinancialYear.Success)
                {
                    revenueCollectionViewModel.FinacialYear = respFinancialYear.FinancialYear;

                    ViewData["_FinancialYearId"] = revenueCollectionViewModel.FinacialYear.ID;


                    //financial year drop down
                    await IndexDropDowns(revenueCollectionViewModel.FinacialYear.ID).ConfigureAwait(false);
                }


                return View(revenueCollectionViewModel);
            }
            catch (System.Exception Ex)
            {
                _logger.LogError(Ex, $"Roles.Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                return View(null);
            }
        }

        [Authorize(Claims.Permission.RevenueCollection.View)]
        // GET: RevenueCollection
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ActionResult> IndexActivities(long FinancialYearId)
        {
            try
            {
                //Get logged in user
                ApplicationUser _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);

                //Set Authority
                RevenueCollectionViewModel revenueCollectionViewModel = new RevenueCollectionViewModel();
                revenueCollectionViewModel.Authority = _ApplicationUser.Authority;

                //Set Financial Year
                var finacialResp = await _financialYearService.FindByIdAsync(FinancialYearId).ConfigureAwait(false);
                if (finacialResp.Success)
                {
                    revenueCollectionViewModel.FinacialYear = finacialResp.FinancialYear;

                    //Get Revenue collection for the year
                    var revenuecollectionResp = await _revenueCollectionService.ListAsync(finacialResp.FinancialYear.ID, "KRB").ConfigureAwait(false);
                    if (revenuecollectionResp.Success)
                    {
                        revenueCollectionViewModel.RevenueCollection = (IList<RevenueCollection>)revenuecollectionResp.RevenueCollection;
                        revenueCollectionViewModel.RevenueCollectionSum = await _revenueCollectionService.RevenueCollectionSum(revenueCollectionViewModel.RevenueCollection).ConfigureAwait(false);
                    }

                    //Get Budget for current Financial Year
                    var updateResp = await _budgetCeilingHeaderService.FindCurrentAsync().ConfigureAwait(false);
                    if (!updateResp.Success)
                    {
                        //New budget for the financial year
                        BudgetCeilingHeader submittedBudget = new BudgetCeilingHeader();
                        submittedBudget.SubmissionDate = DateTime.UtcNow;
                        submittedBudget.SubmittedBy = _ApplicationUser.UserName;
                        submittedBudget.FinancialYearId = finacialResp.FinancialYear.ID;

                        double amt = 0d;
                        var dict = await ComputeBudget(finacialResp.FinancialYear.ID).ConfigureAwait(false);
                        var key = dict.Keys.FirstOrDefault();
                        dict.TryGetValue(key, out amt);
                        submittedBudget.TotalAmount = amt;
                        submittedBudget.ApprovalStatus = 0; //has been submitted

                        updateResp = await _budgetCeilingHeaderService.AddAsync(submittedBudget).ConfigureAwait(false);
                    }

                    BudgetCeilingHeader BudgetCeilingHeader = updateResp.BudgetCeilingHeader;
                    revenueCollectionViewModel.ApprovalStatus = BudgetCeilingHeader.ApprovalStatus;
                    revenueCollectionViewModel.CeilingsEstimateViewModel = await DraftBudget(
                        updateResp.BudgetCeilingHeader.FinancialYearId).ConfigureAwait(false);


                }

                //Set Referer=
                revenueCollectionViewModel.Referer = Request.Headers["Referer"].ToString();
                //Set Return URL and store in session
                HttpContext.Session.SetString("Referer", Request.GetEncodedUrl());

                //financial year drop down
                await IndexDropDowns(revenueCollectionViewModel.FinacialYear.ID).ConfigureAwait(false);

                return PartialView("RevenueCollectionIndexPartialView", revenueCollectionViewModel);
            }
            catch (System.Exception Ex)
            {
                _logger.LogError(Ex, $"Roles.Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                return PartialView("RevenueCollectionIndexPartialView", null);
            }
        }

        [Authorize(Claims.Permission.RevenueCollection.View)]
        // GET: RevenueCollection
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<ActionResult> BudgetActivitiesByFinancialYear(long FinancialYearId)
        {
            try
            {
                //Get logged in user
                ApplicationUser _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);

                //Set Authority
                RevenueCollectionViewModel revenueCollectionViewModel = new RevenueCollectionViewModel();
                revenueCollectionViewModel.Authority = _ApplicationUser.Authority;

                //Set Financial Year
                var finacialResp = await _financialYearService.FindByIdAsync(FinancialYearId).ConfigureAwait(false);
                if (finacialResp.Success)
                {
                    revenueCollectionViewModel.FinacialYear = finacialResp.FinancialYear;

                    //Get Revenue collection for the year
                    var revenuecollectionResp = await _revenueCollectionService.ListAsync(finacialResp.FinancialYear.ID, "KRB").ConfigureAwait(false);
                    if (revenuecollectionResp.Success)
                    {
                        revenueCollectionViewModel.RevenueCollection = (IList<RevenueCollection>)revenuecollectionResp.RevenueCollection;
                        revenueCollectionViewModel.RevenueCollectionSum = await _revenueCollectionService.RevenueCollectionSum(revenueCollectionViewModel.RevenueCollection).ConfigureAwait(false);
                    }

                    //Get Budget for current Financial Year
                    var updateResp = await _budgetCeilingHeaderService.FindByFinancialYearAsync(FinancialYearId).ConfigureAwait(false);
                    if (!updateResp.Success)
                    {
                        //New budget for the financial year
                        BudgetCeilingHeader submittedBudget = new BudgetCeilingHeader();
                        submittedBudget.SubmissionDate = DateTime.UtcNow;
                        submittedBudget.SubmittedBy = _ApplicationUser.UserName;
                        submittedBudget.FinancialYearId = finacialResp.FinancialYear.ID;

                        double amt = 0d;
                        var dict = await ComputeBudget(finacialResp.FinancialYear.ID).ConfigureAwait(false);
                        var key = dict.Keys.FirstOrDefault();
                        dict.TryGetValue(key, out amt);
                        submittedBudget.TotalAmount = amt;
                        submittedBudget.ApprovalStatus = 0; //has been submitted

                        updateResp = await _budgetCeilingHeaderService.AddAsync(submittedBudget).ConfigureAwait(false);
                    }

                    BudgetCeilingHeader BudgetCeilingHeader = updateResp.BudgetCeilingHeader;
                    revenueCollectionViewModel.ApprovalStatus = BudgetCeilingHeader.ApprovalStatus;
                    revenueCollectionViewModel.CeilingsEstimateViewModel = await DraftBudget(
                        updateResp.BudgetCeilingHeader.FinancialYearId).ConfigureAwait(false);


                }

                //Set Referer=
                revenueCollectionViewModel.Referer = Request.Headers["Referer"].ToString();
                //Set Return URL and store in session
                HttpContext.Session.SetString("Referer", Request.GetEncodedUrl());

                //financial year drop down
                await IndexDropDowns(revenueCollectionViewModel.FinacialYear.ID).ConfigureAwait(false);

                return PartialView("RevenueCollectionIndexPartialView", revenueCollectionViewModel);
            }
            catch (System.Exception Ex)
            {
                _logger.LogError(Ex, $"Roles.Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                return PartialView("RevenueCollectionIndexPartialView", null);
            }
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> GetBudgetUploads(string Type, long FinancialYearId)
        {
            try
            {
                var UploadListResponse = await _uploadService.ListAsync(Type, FinancialYearId).ConfigureAwait(false);
                IList<Upload> ARICSUpload = (IList<Upload>)UploadListResponse.Upload;
                return Json(ARICSUpload);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"ARICSController.GetARICSUploads Error: {Ex.Message}");
                return Json(null);
            }
        }

        private async Task IndexDropDowns(long FinancialYearId)
        {
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



        }
        private async Task<CeilingsEstimateViewModel> DraftBudget(long FinancialYearID)
        {
            CeilingsEstimateViewModel ceilingsEstimateViewModel = new CeilingsEstimateViewModel();
            //Set Financial Year
            var finacialResp = await _financialYearService.FindByIdAsync(FinancialYearID).ConfigureAwait(false);
            //FinancialYear FinacialYear = finacialResp.FinancialYear;


            //Get fuel levy
            RevenueCollection fuelLevy = null;
            RevenueStream revenueStream = RevenueStream.Fuel_Levy;
            var revCollectResp = await _revenueCollectionService.
                FindByRevenueStreamAndFinancialYearAsync(FinancialYearID, revenueStream).ConfigureAwait(false);
            if (revCollectResp.Success)
            {
                fuelLevy = revCollectResp.RevenueCollection;
                ceilingsEstimateViewModel.FuelLevy = fuelLevy.Amount;
            }

            //Get transit toll
            RevenueCollection transitTolls = null;
            revenueStream = RevenueStream.Transit_Tolls;
            revCollectResp = await _revenueCollectionService.
               FindByRevenueStreamAndFinancialYearAsync(FinancialYearID, revenueStream).ConfigureAwait(false);
            if (revCollectResp.Success)
            {
                transitTolls = revCollectResp.RevenueCollection;
                ceilingsEstimateViewModel.TransitTolls = transitTolls.Amount;
            }
            double TotalFundsAvailable = 0d;
            TotalFundsAvailable = ceilingsEstimateViewModel.TransitTolls + ceilingsEstimateViewModel.FuelLevy;
            ceilingsEstimateViewModel.TotalFundsAvailable = TotalFundsAvailable;
            //Get Allocation Ratio for KRB 
            double kRBAllocationratio = 0d;
            var allCodeUnitResp = await _allocationCodeUnitService.FindByAuthorityAsync(8).ConfigureAwait(false);
            if (allCodeUnitResp.Success)
            {
                kRBAllocationratio = allCodeUnitResp.AllocationCodeUnit.Percent;
            }

            //Compute KRB operations & KRB 2% Transit Trolls
            double KRBOperations = 0d;
            double KRBTransitTrolls = 0d;
            if (fuelLevy != null)
            {
                KRBOperations = fuelLevy.Amount * kRBAllocationratio;
                ceilingsEstimateViewModel.KRBOperations = KRBOperations;

            }
            if (transitTolls != null)
            {
                KRBTransitTrolls = transitTolls.Amount * kRBAllocationratio;
                ceilingsEstimateViewModel.KRBTransitTrolls = KRBTransitTrolls;

            }

            //KRB Total Operations
            double KRBTotalOperations = 0d;
            KRBTotalOperations = KRBTransitTrolls + KRBOperations;
            ceilingsEstimateViewModel.KRBTotalOperations = KRBTotalOperations;


            //NET Funds available after administration costs
            double NETFundsAvailableAfterAdministrationCosts = 0d;
            NETFundsAvailableAfterAdministrationCosts = TotalFundsAvailable - KRBTotalOperations;
            ceilingsEstimateViewModel.NETFundsAvailableAfterAdministrationCosts = NETFundsAvailableAfterAdministrationCosts;

            // (i) Road Agencies-Fuel Levy  &  (ii) Road Annuity Fund &  Sub-total Fuel Levy 
            //NetTransitTolls
            double Road_Agencies_Fuel_Levy = 0d; double Road_Annuity_Fund = 0d; double SubTotal_Fuel_Levy = 0d;
            double NetTransitTolls = 0d;

            // If the fuel is null, set value to 0.
            // ... Otherwise, set value to fuel.amount.
            double fuelamt = fuelLevy == null ? 0 : fuelLevy.Amount;
            Road_Agencies_Fuel_Levy = (fuelamt - KRBOperations) * 15 / 18;
            Road_Annuity_Fund = (fuelamt - KRBOperations) * 3 / 18;
            SubTotal_Fuel_Levy = Road_Agencies_Fuel_Levy + Road_Annuity_Fund;

            double transitTollsamt = transitTolls == null ? 0 : transitTolls.Amount;
            NetTransitTolls = transitTollsamt - KRBTransitTrolls;

            //// (i) Road Agencies-Fuel Levy  &  (ii) Road Annuity Fund &  Sub-total Fuel Levy 
            ////NetTransitTolls
            //double Road_Agencies_Fuel_Levy = 0d; double Road_Annuity_Fund = 0d; double SubTotal_Fuel_Levy = 0d;
            //double NetTransitTolls = 0d;
            //Road_Agencies_Fuel_Levy = (fuelLevy.Amount - KRBOperations) * 15 / 18;
            //Road_Annuity_Fund = (fuelLevy.Amount - KRBOperations) * 3 / 18;
            //SubTotal_Fuel_Levy = Road_Agencies_Fuel_Levy + Road_Annuity_Fund;
            //NetTransitTolls = transitTolls.Amount - KRBTransitTrolls;

            ceilingsEstimateViewModel.Road_Agencies_Fuel_Levy = Road_Agencies_Fuel_Levy;
            ceilingsEstimateViewModel.Road_Annuity_Fund = Road_Annuity_Fund;
            ceilingsEstimateViewModel.SubTotal_Fuel_Levy = SubTotal_Fuel_Levy;
            ceilingsEstimateViewModel.NetTransitTolls = NetTransitTolls;


            //  TOTAL Funds For Allocation 
            double TOTALFundsForAllocation = 0d;
            TOTALFundsForAllocation = SubTotal_Fuel_Levy + NetTransitTolls;
            ceilingsEstimateViewModel.TOTALFundsForAllocation = TOTALFundsForAllocation;

            //RMLF less RAF
            ceilingsEstimateViewModel.RMLFLessRAF = ceilingsEstimateViewModel.FuelLevy - ceilingsEstimateViewModel.Road_Annuity_Fund;

            // CountyAllocation 
            double countyAllocationratio = 0d;
            double CountyAllocation = 0d;
            allCodeUnitResp = await _allocationCodeUnitService.FindByAuthorityAsync(55).ConfigureAwait(false);
            if (allCodeUnitResp.Success)
            {
                countyAllocationratio = allCodeUnitResp.AllocationCodeUnit.Percent;
                CountyAllocation = (countyAllocationratio * Road_Agencies_Fuel_Levy) / 0.98;
                ceilingsEstimateViewModel.CountyAllocation = CountyAllocation;
            }


            // KENHAAllocation 
            double kenhaAllocationratio = 0d;
            double kenhaAllocation = 0d;
            double kenhaTotal = 0d;
            allCodeUnitResp = await _allocationCodeUnitService.FindByAuthorityAsync(1).ConfigureAwait(false);
            if (allCodeUnitResp.Success)
            {
                kenhaAllocationratio = allCodeUnitResp.AllocationCodeUnit.Percent;
                kenhaAllocation = (kenhaAllocationratio * Road_Agencies_Fuel_Levy) / 0.98;
                kenhaTotal = kenhaAllocation + NetTransitTolls;
                ceilingsEstimateViewModel.kenhaAllocation = kenhaAllocation;
                ceilingsEstimateViewModel.kenhaTotal = kenhaTotal;
            }


            // KRBBoard_CS Allocalocation 
            double KRBBoard_CSAllocationratio = 0d;
            double KRBBoard_CSAllocation = 0d;
            allCodeUnitResp = await _allocationCodeUnitService.FindByAuthorityAsync(54).ConfigureAwait(false);
            if (allCodeUnitResp.Success)
            {
                KRBBoard_CSAllocationratio = allCodeUnitResp.AllocationCodeUnit.Percent;
                KRBBoard_CSAllocation = (KRBBoard_CSAllocationratio * Road_Agencies_Fuel_Levy) / 0.98;
                ceilingsEstimateViewModel.KRBBoard_CSAllocation = KRBBoard_CSAllocation;
            }


            // KWS 
            double KWSAllocationratio = 0d;
            double KWSAllocation = 0d;
            allCodeUnitResp = await _allocationCodeUnitService.FindByAuthorityAsync(4).ConfigureAwait(false);
            if (allCodeUnitResp.Success)
            {
                KWSAllocationratio = allCodeUnitResp.AllocationCodeUnit.Percent;
                KWSAllocation = (KWSAllocationratio * Road_Agencies_Fuel_Levy) / 0.98;
                ceilingsEstimateViewModel.KWSAllocation = KWSAllocation;
            }


            // KERRA
            double KERRAAllocationratio = 0d;
            double KERRAAllocation = 0d;

            allCodeUnitResp = await _allocationCodeUnitService.FindByAuthorityAsync(2).ConfigureAwait(false);
            if (allCodeUnitResp.Success)
            {
                KERRAAllocationratio = allCodeUnitResp.AllocationCodeUnit.Percent;
                KERRAAllocation = (Road_Agencies_Fuel_Levy - CountyAllocation - kenhaAllocation
                    - KRBBoard_CSAllocation - KWSAllocation) * 32 / 47;
                ceilingsEstimateViewModel.KERRAAllocation = KERRAAllocation;
            }

            // KURA
            double KURAAllocationratio = 0d;
            double KURAAllocation = 0d;

            allCodeUnitResp = await _allocationCodeUnitService.FindByAuthorityAsync(3).ConfigureAwait(false);
            if (allCodeUnitResp.Success)
            {
                KURAAllocationratio = allCodeUnitResp.AllocationCodeUnit.Percent;
                KURAAllocation = (Road_Agencies_Fuel_Levy - CountyAllocation - kenhaAllocation
                    - KRBBoard_CSAllocation - KWSAllocation) * 15 / 47;
                ceilingsEstimateViewModel.KURAAllocation = KURAAllocation;
            }

            double SubtotalFundsAllocated = 0d;
            SubtotalFundsAllocated = Road_Annuity_Fund + CountyAllocation + kenhaTotal
                + KRBBoard_CSAllocation + KWSAllocation + KERRAAllocation + KURAAllocation;
            ceilingsEstimateViewModel.SubtotalFundsAllocated = SubtotalFundsAllocated;

            double GrossRMLF = 0d;
            GrossRMLF = KRBTotalOperations + SubtotalFundsAllocated;
            ceilingsEstimateViewModel.GrossRMLF = GrossRMLF;

            //Admin Operations
            ceilingsEstimateViewModel.KURAAdminOperations = 1892000000d;
            ceilingsEstimateViewModel.KERRAAdminOperations = ceilingsEstimateViewModel.RMLFLessRAF * 5.5d / 100d; ;
            ceilingsEstimateViewModel.kenhaAdminOperations = ceilingsEstimateViewModel.RMLFLessRAF * 4d / 100d;

            //KERRA computation
            ceilingsEstimateViewModel.KERRAConstituencyAllocation = ceilingsEstimateViewModel.KERRAAllocation * (22d / 32d);
            ceilingsEstimateViewModel.KERRACriticalLinksAllocation = ceilingsEstimateViewModel.KERRAAllocation * (10d / 32d);
            ceilingsEstimateViewModel.KERRATotalAdminBudget = ceilingsEstimateViewModel.RMLFLessRAF * 5.5d / 100d;
            ceilingsEstimateViewModel.KERRAPortinAdminTwentyTwoPercent = ceilingsEstimateViewModel.KERRATotalAdminBudget * (22d / 32d);
            ceilingsEstimateViewModel.KERRAPortinAdminTenPercent = ceilingsEstimateViewModel.KERRATotalAdminBudget * (10d / 32d);
            ceilingsEstimateViewModel.KERRAPortinRoadWorksTwentyTwoPercent = ceilingsEstimateViewModel.KERRAConstituencyAllocation
                - ceilingsEstimateViewModel.KERRAPortinAdminTwentyTwoPercent;
            ceilingsEstimateViewModel.KERRAPortinRoadworksTenPercent = ceilingsEstimateViewModel.KERRACriticalLinksAllocation
                - ceilingsEstimateViewModel.KERRAPortinAdminTenPercent;
            ceilingsEstimateViewModel.KERRATotalBudgetForWorks = ceilingsEstimateViewModel.KERRAPortinRoadWorksTwentyTwoPercent
                + ceilingsEstimateViewModel.KERRAPortinRoadworksTenPercent;

            ceilingsEstimateViewModel.KERRATwentyTwoPercentAllocPerConstituency = ceilingsEstimateViewModel.KERRAPortinRoadWorksTwentyTwoPercent
                / 290d;
            ceilingsEstimateViewModel.KERRATenPercentAllocPerConstituency = ceilingsEstimateViewModel.KERRAPortinRoadworksTenPercent
                / 290d;
            ceilingsEstimateViewModel.KERRATotalAllocPerConstituency = ceilingsEstimateViewModel.KERRATwentyTwoPercentAllocPerConstituency
            + ceilingsEstimateViewModel.KERRATenPercentAllocPerConstituency;

            //Kenha computation
            ceilingsEstimateViewModel.kenhaTransitTolls = ceilingsEstimateViewModel.NetTransitTolls;
            ceilingsEstimateViewModel.kenhaTotalAllocation = ceilingsEstimateViewModel.kenhaAllocation
                + ceilingsEstimateViewModel.NetTransitTolls;
            ceilingsEstimateViewModel.kenhaRoadWorks = ceilingsEstimateViewModel.kenhaTotalAllocation
                - ceilingsEstimateViewModel.kenhaAdminOperations;
            //KURA computation
            ceilingsEstimateViewModel.KURARoadWorks = ceilingsEstimateViewModel.KURAAllocation
                - ceilingsEstimateViewModel.KURAAdminOperations;

            return ceilingsEstimateViewModel;
        }


        private async Task<Dictionary<AllocationCodeUnit, double>> ComputeBudget(long FinancialYearID)
        {
            Dictionary<AllocationCodeUnit, double> RADict = new Dictionary<AllocationCodeUnit, double>();
            //Set Financial Year
            var finacialResp = await _financialYearService.FindByIdAsync(FinancialYearID).ConfigureAwait(false);
            //FinancialYear FinacialYear = finacialResp.FinancialYear;

            //Get Revenue collection for the year
            var revenuecollectionResp = await _revenueCollectionService.ListAsync(finacialResp.FinancialYear.ID, "KRB").ConfigureAwait(false);
            IList<RevenueCollection> RevenueCollections = (IList<RevenueCollection>)revenuecollectionResp.RevenueCollection;
            double TotalFundsAvailable = await _revenueCollectionService.RevenueCollectionSum(RevenueCollections).ConfigureAwait(false);

            //Get fuel levy
            RevenueCollection fuelLevy = null;
            RevenueStream revenueStream = RevenueStream.Fuel_Levy;
            var revCollectResp = await _revenueCollectionService.
                FindByRevenueStreamAndFinancialYearAsync(FinancialYearID, revenueStream).ConfigureAwait(false);
            if (revCollectResp.Success)
            {
                fuelLevy = revCollectResp.RevenueCollection;
            }

            //Get transit toll
            RevenueCollection transitTolls = null;
            revenueStream = RevenueStream.Transit_Tolls;
            revCollectResp = await _revenueCollectionService.
               FindByRevenueStreamAndFinancialYearAsync(FinancialYearID, revenueStream).ConfigureAwait(false);
            if (revCollectResp.Success)
            {
                transitTolls = revCollectResp.RevenueCollection;
            }

            //Get Allocation Ratio for KRB 
            double kRBAllocationratio = 0d;
            var allCodeUnitResp = await _allocationCodeUnitService.FindByAuthorityAsync(8).ConfigureAwait(false);
            if (allCodeUnitResp.Success)
            {
                kRBAllocationratio = allCodeUnitResp.AllocationCodeUnit.Percent;
            }

            //Compute KRB operations & Transit Trolls
            double KRBOperations = 0d;
            double KRBTransitTrolls = 0d;
            if (fuelLevy != null)
            {
                KRBOperations = fuelLevy.Amount * kRBAllocationratio;

            }
            if (transitTolls != null)
            {
                KRBTransitTrolls = transitTolls.Amount * kRBAllocationratio;

            }

            //KRB Total Operations
            double KRBTotalOperations = 0d;
            KRBTotalOperations = KRBTransitTrolls + KRBOperations;

            //NET Funds available after administration costs
            double NETFundsAvailableAfterAdministrationCosts = 0d;
            NETFundsAvailableAfterAdministrationCosts = TotalFundsAvailable - KRBTotalOperations;

            // (i) Road Agencies-Fuel Levy  &  (ii) Road Annuity Fund &  Sub-total Fuel Levy 
            //NetTransitTolls
            double Road_Agencies_Fuel_Levy = 0d; double Road_Annuity_Fund = 0d; double SubTotal_Fuel_Levy = 0d;
            double NetTransitTolls = 0d;

            // If the fuel is null, set value to 0.
            // ... Otherwise, set value to fuel.amount.
            double fuelamt = fuelLevy == null ? 0 : fuelLevy.Amount;
            Road_Agencies_Fuel_Levy = (fuelamt - KRBOperations) * 15 / 18;
            Road_Annuity_Fund = (fuelamt - KRBOperations) * 3 / 18;
            SubTotal_Fuel_Levy = Road_Agencies_Fuel_Levy + Road_Annuity_Fund;

            double transitTollsamt = transitTolls == null ? 0 : transitTolls.Amount;
            NetTransitTolls = transitTollsamt - KRBTransitTrolls;

            //  TOTAL Funds For Allocation 
            double TOTALFundsForAllocation = 0d;
            TOTALFundsForAllocation = SubTotal_Fuel_Levy + NetTransitTolls;
            AllocationCodeUnit allocationCodeUnit = new AllocationCodeUnit();
            allocationCodeUnit.ID = 0;
            RADict.Add(allocationCodeUnit, TOTALFundsForAllocation);

            // CountyAllocation 
            double countyAllocationratio = 0d;
            double CountyAllocation = 0d;
            allCodeUnitResp = await _allocationCodeUnitService.FindByAuthorityAsync(55).ConfigureAwait(false);
            if (allCodeUnitResp.Success)
            {
                countyAllocationratio = allCodeUnitResp.AllocationCodeUnit.Percent;
                CountyAllocation = (countyAllocationratio * Road_Agencies_Fuel_Levy) / 0.98;
                RADict.Add(allCodeUnitResp.AllocationCodeUnit, CountyAllocation);
            }


            // KENHAAllocation 
            double kenhaAllocationratio = 0d;
            double kenhaAllocation = 0d;
            allCodeUnitResp = await _allocationCodeUnitService.FindByAuthorityAsync(1).ConfigureAwait(false);
            if (allCodeUnitResp.Success)
            {
                kenhaAllocationratio = allCodeUnitResp.AllocationCodeUnit.Percent;
                kenhaAllocation = (kenhaAllocationratio * Road_Agencies_Fuel_Levy) / 0.98;
                RADict.Add(allCodeUnitResp.AllocationCodeUnit, kenhaAllocation);
            }


            // KRBBoard_CS Allocalocation 
            double KRBBoard_CSAllocationratio = 0d;
            double KRBBoard_CSAllocation = 0d;
            allCodeUnitResp = await _allocationCodeUnitService.FindByAuthorityAsync(54).ConfigureAwait(false);
            if (allCodeUnitResp.Success)
            {
                KRBBoard_CSAllocationratio = allCodeUnitResp.AllocationCodeUnit.Percent;
                KRBBoard_CSAllocation = (KRBBoard_CSAllocationratio * Road_Agencies_Fuel_Levy) / 0.98;
                RADict.Add(allCodeUnitResp.AllocationCodeUnit, KRBBoard_CSAllocation);
            }


            // KWS 
            double KWSAllocationratio = 0d;
            double KWSAllocation = 0d;
            allCodeUnitResp = await _allocationCodeUnitService.FindByAuthorityAsync(4).ConfigureAwait(false);
            if (allCodeUnitResp.Success)
            {
                KWSAllocationratio = allCodeUnitResp.AllocationCodeUnit.Percent;
                KWSAllocation = (KWSAllocationratio * Road_Agencies_Fuel_Levy) / 0.98;
                RADict.Add(allCodeUnitResp.AllocationCodeUnit, KWSAllocation);
            }


            // KERRA
            double KERRAAllocationratio = 0d;
            double KERRAAllocation = 0d;

            allCodeUnitResp = await _allocationCodeUnitService.FindByAuthorityAsync(2).ConfigureAwait(false);
            if (allCodeUnitResp.Success)
            {
                KERRAAllocationratio = allCodeUnitResp.AllocationCodeUnit.Percent;
                KERRAAllocation = (Road_Agencies_Fuel_Levy - CountyAllocation - kenhaAllocation
                    - KRBBoard_CSAllocation - KWSAllocation) * 32 / 47;
                RADict.Add(allCodeUnitResp.AllocationCodeUnit, KERRAAllocation);
            }

            // KURA
            double KURAAllocationratio = 0d;
            double KURAAllocation = 0d;

            allCodeUnitResp = await _allocationCodeUnitService.FindByAuthorityAsync(3).ConfigureAwait(false);
            if (allCodeUnitResp.Success)
            {
                KURAAllocationratio = allCodeUnitResp.AllocationCodeUnit.Percent;
                KURAAllocation = (Road_Agencies_Fuel_Levy - CountyAllocation - kenhaAllocation
                    - KRBBoard_CSAllocation - KWSAllocation) * 15 / 47;
                RADict.Add(allCodeUnitResp.AllocationCodeUnit, KURAAllocation);
            }
            return RADict;
        }


        [Authorize(Claims.Permission.RevenueCollection.SubmitBudget)]
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> SubmitBudget()
        {
            long FinancialID = 0;
            bool results = long.TryParse(Request.Form["FinancialID"].ToString(), out FinancialID);

            var user = await GetLoggedInUser().ConfigureAwait(false);
            //retrieve the currentyear budgetHeader
            var resp = await _budgetCeilingHeaderService.FindByFinancialYearAsync(FinancialID).ConfigureAwait(false); //Todo: Fix need to get budget header for the respective fincial year
            if (resp.Success)
            {
                //Existing budget for the financial year
                var submittedBudget = resp.BudgetCeilingHeader;
                submittedBudget.SubmissionDate = DateTime.UtcNow;
                submittedBudget.SubmittedBy = user.UserName;


                double amt = 0d;
                var dict = await ComputeBudget(FinancialID).ConfigureAwait(false);
                var key = dict.Keys.FirstOrDefault();
                dict.TryGetValue(key, out amt);
                submittedBudget.TotalAmount = amt;
                submittedBudget.ApprovalStatus = 1; //has been submitted

                var updateResp = await _budgetCeilingHeaderService.Update(submittedBudget.ID, submittedBudget).ConfigureAwait(false);
                if (!updateResp.Success)
                {
                    TempData["failedToSubmitForApproval"] = "Could not submit for approval, Please contact system administrator";
                    return RedirectToAction("Upload");
                }
                else
                {
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
                                _hostingEnvironment.WebRootPath, "uploads", "budgets", fname);

                                using (var fileStream = new FileStream(path, FileMode.Create))
                                {
                                    await file.CopyToAsync(fileStream).ConfigureAwait(false);
                                }

                                //Register with file uploads
                                Upload upload = new Upload();
                                upload.filename = fname;
                                upload.ForeignId = updateResp.BudgetCeilingHeader.FinancialYearId;
                                upload.type = "budget";

                                var uploadResp = await _uploadService.AddAsync(upload).ConfigureAwait(false);
                            }

                        }
                        catch (Exception Ex)
                        {
                            _logger.LogError(Ex, $"SubmitBudget() File Upload Fails Error: {Ex.Message} " +
                            $"{Environment.NewLine}");
                        }
                    }
                    await UpdateBudgetLines(updateResp, user, dict).ConfigureAwait(false);
                    await StoreBudgetCeilingComputation(FinancialID).ConfigureAwait(false);
                }
            }
            else
            {
                //New budget for the financial year
                BudgetCeilingHeader submittedBudget = new BudgetCeilingHeader();
                submittedBudget.SubmissionDate = DateTime.UtcNow;
                submittedBudget.SubmittedBy = user.UserName;
                submittedBudget.FinancialYearId = FinancialID;

                double amt = 0d;
                var dict = await ComputeBudget(FinancialID).ConfigureAwait(false);
                var key = dict.Keys.FirstOrDefault();
                dict.TryGetValue(key, out amt);
                submittedBudget.TotalAmount = amt;
                submittedBudget.ApprovalStatus = 1; //has been submitted

                var updateResp = await _budgetCeilingHeaderService.AddAsync(submittedBudget).ConfigureAwait(false);
                if (!updateResp.Success)
                {
                    TempData["failedToSubmitForApproval"] = "Could not submit for approval, Please contact system administrator";
                    return RedirectToAction("Index");
                }
                else
                {
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
                                _hostingEnvironment.WebRootPath, "uploads", "budgets", fname);

                                using (var fileStream = new FileStream(path, FileMode.Create))
                                {
                                    await file.CopyToAsync(fileStream).ConfigureAwait(false);
                                }

                                //Register with file uploads
                                Upload upload = new Upload();
                                upload.filename = fname;
                                upload.ForeignId = updateResp.BudgetCeilingHeader.FinancialYearId;
                                upload.type = "budget";

                                var uploadResp = await _uploadService.AddAsync(upload).ConfigureAwait(false);
                            }

                        }
                        catch (Exception Ex)
                        {
                            _logger.LogError(Ex, $"SubmitBudget() File Upload Fails Error: {Ex.Message} " +
                            $"{Environment.NewLine}");
                        }
                    }
                    await UpdateBudgetLines(updateResp, user, dict).ConfigureAwait(false);
                    await StoreBudgetCeilingComputation(FinancialID).ConfigureAwait(false);
                }
            }

            return Json(new
            {
                Success = true,
                Message = "Success",
                Href = Url.Action("Index", "RevenueCollection")
            });
        }

        [HttpPost]
        [Authorize(Claims.Permission.RevenueCollection.ApproveBudget)]
        public async Task<IActionResult> ApproveBudget(long FinancialID)
        {
            //retrieve the currentyear budgetHeader
            var resp = await _budgetCeilingHeaderService.FindByFinancialYearAsync(FinancialID).ConfigureAwait(false);
            if (resp.Success)
            {
                var user = await GetLoggedInUser().ConfigureAwait(false);
                //mark the budget as approved
                var submittedBudget = resp.BudgetCeilingHeader;
                submittedBudget.ApprovalStatus = 2;//Has been approved
                submittedBudget.ApprovedBy = user.UserName;
                submittedBudget.ApprovalDate = DateTime.UtcNow;

                var updateResp = await _budgetCeilingHeaderService.Update(submittedBudget.ID, submittedBudget).ConfigureAwait(false);
            }
            return Json(new
            {
                Success = true,
                Message = "Success",
                Href = Url.Action("Index", "RevenueCollection")
            });
        }

        [HttpPost]
        [Authorize(Claims.Permission.RevenueCollection.ApproveBudget)]
        public async Task<IActionResult> RejectBudget(long FinancialID)
        {
            //retrieve the currentyear budgetHeader
            var resp = await _budgetCeilingHeaderService.FindByFinancialYearAsync(FinancialID).ConfigureAwait(false);
            if (resp.Success)
            {
                var user = await GetLoggedInUser().ConfigureAwait(false);
                //mark the budget as approved
                var submittedBudget = resp.BudgetCeilingHeader;
                submittedBudget.ApprovalStatus = 0;//Has been rejected and reset to zero
                submittedBudget.ApprovedBy = user.UserName;
                submittedBudget.ApprovalDate = DateTime.UtcNow;

                var updateResp = await _budgetCeilingHeaderService.Update(submittedBudget.ID, submittedBudget).ConfigureAwait(false);

            }
            return Json(new
            {
                Success = true,
                Message = "Success",
                Href = Url.Action("Index", "RevenueCollection")
            });
        }

        [HttpPost]
        [Authorize(Claims.Permission.RevenueCollection.ApproveBudget)]
        public async Task<IActionResult> ResetBudget(long FinancialID)
        {
            //retrieve the budgetHeader  by current FinancialID
            var resp = await _budgetCeilingHeaderService.FindByFinancialYearAsync(FinancialID).ConfigureAwait(false);
            if (resp.Success)
            {
                var user = await GetLoggedInUser().ConfigureAwait(false);
                //mark the budget as approved
                var submittedBudget = resp.BudgetCeilingHeader;
                submittedBudget.ApprovalStatus = 0;//Has been approved but their is need to reset to zero
                submittedBudget.ApprovedBy = user.UserName;
                submittedBudget.ApprovalDate = DateTime.UtcNow;

                var updateResp = await _budgetCeilingHeaderService.Update(submittedBudget.ID, submittedBudget).ConfigureAwait(false);

            }
            return Json(new
            {
                Success = true,
                Message = "Success",
                Href = Url.Action("Index", "RevenueCollection")
            });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task UpdateBudgetLines(BudgetCeilingHeaderResponse updateResp, ApplicationUser user, Dictionary<AllocationCodeUnit, double> RADict)
        {
            try
            {
                //Remove all budget allocations
                var budgetCeilingResp2 = await _budgetCeilingService.RemoveAllAsync(updateResp.BudgetCeilingHeader.ID).ConfigureAwait(false);

                double CountyAllocation = 0d; //Get county budget


                //Update RA Budget Allocations          
                foreach (var pair in RADict)
                {
                    if (pair.Key.ID != 0)
                    {

                        //Add new
                        BudgetCeiling budgetCeiling = new BudgetCeiling();
                        if (pair.Key.AuthorityId == 55)//CG total allocation
                        {
                            CountyAllocation = pair.Value;
                        }
                        budgetCeiling.Amount = pair.Value;
                        budgetCeiling.AuthorityId = pair.Key.Authority.ID;
                        //Budgetheadrid
                        budgetCeiling.BudgetCeilingHeaderId = updateResp.BudgetCeilingHeader.ID;
                        budgetCeiling.UpdateDate = DateTime.UtcNow;
                        budgetCeiling.UpdatedBy = user.UserName;
                        budgetCeiling.AdditionalInfo = "";
                        var budgetCeilingUpdateResp = await _budgetCeilingService.AddAsync(budgetCeiling).ConfigureAwait(false);
                        if (budgetCeilingUpdateResp.Success)
                        {
                            _logger.LogInformation($"Budget ceiling line item added successfully: {budgetCeilingUpdateResp.BudgetCeiling.ID} " +
                            $"{Environment.NewLine}");
                        }
                        else
                        {
                            _logger.LogError($"Budget ceiling line item failed: {budgetCeilingUpdateResp.BudgetCeiling.ID} " +
                            $"{Environment.NewLine}");
                        }
                    }
                }

                //Get county budget
                var allocationcodeUnitResp = await _allocationCodeUnitService.ListAsync("CG").ConfigureAwait(false);
                if (allocationcodeUnitResp.Success)
                {

                    foreach (AllocationCodeUnit allocationCodeUnit in allocationcodeUnitResp.AllocationCodeUnit)
                    {
                        //Add new
                        BudgetCeiling budgetCeiling = new BudgetCeiling();
                        budgetCeiling.Amount = (CountyAllocation * allocationCodeUnit.Percent);
                        budgetCeiling.AuthorityId = allocationCodeUnit.Authority.ID;
                        //Budgetheadrid
                        budgetCeiling.BudgetCeilingHeaderId = updateResp.BudgetCeilingHeader.ID;
                        budgetCeiling.UpdateDate = DateTime.UtcNow;
                        budgetCeiling.UpdatedBy = user.UserName;
                        budgetCeiling.AdditionalInfo = "";
                        var budgetCeilingUpdateResp = await _budgetCeilingService.AddAsync(budgetCeiling).ConfigureAwait(false);
                        if (budgetCeilingUpdateResp.Success)
                        {
                            _logger.LogInformation($"Budget ceiling line item added successfully: {budgetCeilingUpdateResp.BudgetCeiling.ID} " +
                            $"{Environment.NewLine}");
                        }
                        else
                        {
                            _logger.LogError($"Budget ceiling line item failed: {budgetCeilingUpdateResp.BudgetCeiling.ID} " +
                            $"{Environment.NewLine}");
                        }

                    }
                }
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"UpdateBudgetLines Method Error: {Ex.Message} " +
                $"{Environment.NewLine}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task UpdateRoadworksBudgetLinesforRoadAgencies(BudgetCeiling budgetCeiling, long AuthorityID, BudgetCeilingHeaderResponse updateResp,
            ApplicationUser user, WorkCategoryFundingMatrix workCategoryFundingMatrix)
        {
            try
            {
                //Get Administration figure
                double Administration = 0d;
                Administration = budgetCeiling.Amount * workCategoryFundingMatrix.Administration;

                //Get  routine Maintenace figure
                double routineMaintenance = 0d;
                routineMaintenance = budgetCeiling.Amount * workCategoryFundingMatrix.RoutineMaintenance;

                //Get periodic maintenace figure
                double periodicMaintenance = 0d;
                periodicMaintenance = budgetCeiling.Amount * workCategoryFundingMatrix.PeriodicMentanance;

                //Get spot improvement figure
                double spotImprovement = 0d;
                spotImprovement = budgetCeiling.Amount * workCategoryFundingMatrix.SpotImprovement;

                //Get structures figure
                double structure = 0d;
                structure = budgetCeiling.Amount * workCategoryFundingMatrix.StructureConstruction;

                //Get PBC Management Figure
                double PBCManagement = 0d;
                PBCManagement = budgetCeiling.Amount * workCategoryFundingMatrix.PBCManagement;

                //Get Axle Load Activities
                double RAM = 0d;
                RAM = budgetCeiling.Amount * workCategoryFundingMatrix.RAM;

                //Network
                double Network = 0d;
                Network = budgetCeiling.Amount * workCategoryFundingMatrix.Network;

                //WeighBridges
                double WeighBridges = 0d;
                WeighBridges = budgetCeiling.Amount * workCategoryFundingMatrix.WeighBridges;

                //AlehuOperationsAndPurchaseOfVehicles
                double AlehuOperationsAndPurchaseOfVehicles = 0d;
                AlehuOperationsAndPurchaseOfVehicles = budgetCeiling.Amount * workCategoryFundingMatrix.AlehuOperationsAndPurchaseOfVehicles;

                //AxleLoadActivities
                double AxleLoadActivities = 0d;
                AxleLoadActivities = budgetCeiling.Amount * workCategoryFundingMatrix.AxleLoadActivities;

                //EmergencyWorks
                double EmergencyWorks = 0d;
                EmergencyWorks = budgetCeiling.Amount * workCategoryFundingMatrix.EmergencyWorks;

                //RoadSafety
                double RoadSafety = 0d;
                RoadSafety = budgetCeiling.Amount * workCategoryFundingMatrix.RoadSafety;

                //RoadConditionSurvey
                double RoadConditionSurvey = 0d;
                RoadConditionSurvey = budgetCeiling.Amount * workCategoryFundingMatrix.RoadConditionSurvey;

                //BasedContracts
                double BasedContracts = 0d;
                BasedContracts = budgetCeiling.Amount * workCategoryFundingMatrix.BasedContracts;

                //Matters
                double Matters = 0d;
                Matters = budgetCeiling.Amount * workCategoryFundingMatrix.Matters;

                //Support
                double Support = 0d;
                Support = budgetCeiling.Amount * workCategoryFundingMatrix.Support;

                //Get HQ Activities
                double HQActivities = 0d;
                HQActivities = budgetCeiling.Amount * workCategoryFundingMatrix.HQActivities;

                //Get RSIP/CriticalLinks
                double RSIPCriticalLinks = 0d;
                RSIPCriticalLinks = budgetCeiling.Amount * workCategoryFundingMatrix.RSIPCriticalLinks;

                //Rehabilation Work
                double rehabiliatation = 0d;
                rehabiliatation = budgetCeiling.Amount * workCategoryFundingMatrix.RehabilitationWork;

                //New Structure
                double newStructure = 0d;
                newStructure = budgetCeiling.Amount * workCategoryFundingMatrix.NewStructure;

                var resp = await _roadWorkBudgetHeaderService.FindByFinancialYearIdAndAuthorityID(updateResp.BudgetCeilingHeader.FinancialYearId, AuthorityID).ConfigureAwait(false);
                if (resp.Success)
                {
                    //update
                    resp.RoadWorkBudgetHeader.SubmittedBy = user.UserName;
                    resp.RoadWorkBudgetHeader.SubmissionDate = DateTime.UtcNow;
                    resp.RoadWorkBudgetHeader.ApprovalStatus = updateResp.BudgetCeilingHeader.ApprovalStatus;
                    resp.RoadWorkBudgetHeader.ApprovedBy = user.UserName;

                    var rdWorkBudgHeader = await _roadWorkBudgetHeaderService.Update(resp.RoadWorkBudgetHeader).ConfigureAwait(false);
                    if (rdWorkBudgHeader.Success)
                    {
                        //Add the activities to the roads works header items                       
                        try
                        {
                            //find by RoadWorkbudgetheaderid
                            var rdworkbudgLineResp = await _roadWorkBudgetLinesService.FindByRoadWorkBudgetHeaderIdAsync(rdWorkBudgHeader.RoadWorkBudgetHeader.ID).ConfigureAwait(false);
                            //if exists --update else add async
                            if (rdworkbudgLineResp.Success)
                            {
                                rdworkbudgLineResp.RoadWorkBudgetLine.FundingSourceId = 15;//Todo:Hardcoded 22% RMLF  Portion
                                rdworkbudgLineResp.RoadWorkBudgetLine.FundTypeId = 2;//Todo:Regular
                                rdworkbudgLineResp.RoadWorkBudgetLine.Administration = Administration;
                                rdworkbudgLineResp.RoadWorkBudgetLine.RoutineMaintanance = routineMaintenance;
                                rdworkbudgLineResp.RoadWorkBudgetLine.PeriodicMentanance = periodicMaintenance;
                                rdworkbudgLineResp.RoadWorkBudgetLine.SpotImprovement = spotImprovement;
                                rdworkbudgLineResp.RoadWorkBudgetLine.StructureConstruction = structure;
                                rdworkbudgLineResp.RoadWorkBudgetLine.PBCManagement = PBCManagement;
                                rdworkbudgLineResp.RoadWorkBudgetLine.RAM = RAM;
                                rdworkbudgLineResp.RoadWorkBudgetLine.Network = Network;
                                rdworkbudgLineResp.RoadWorkBudgetLine.WeighBridges = WeighBridges;
                                rdworkbudgLineResp.RoadWorkBudgetLine.AlehuOperationsAndPurchaseOfVehicles = AlehuOperationsAndPurchaseOfVehicles;
                                rdworkbudgLineResp.RoadWorkBudgetLine.AxleLoadActivities = AxleLoadActivities;
                                rdworkbudgLineResp.RoadWorkBudgetLine.EmergencyWorks = EmergencyWorks;
                                rdworkbudgLineResp.RoadWorkBudgetLine.RoadSafety = RoadSafety;
                                rdworkbudgLineResp.RoadWorkBudgetLine.RoadConditionSurvey = RoadConditionSurvey;
                                rdworkbudgLineResp.RoadWorkBudgetLine.BasedContracts = BasedContracts;
                                rdworkbudgLineResp.RoadWorkBudgetLine.Matters = Matters;
                                rdworkbudgLineResp.RoadWorkBudgetLine.Support = Support;
                                rdworkbudgLineResp.RoadWorkBudgetLine.HQActivities = HQActivities;
                                rdworkbudgLineResp.RoadWorkBudgetLine.RSIPCriticalLinks = RSIPCriticalLinks;
                                rdworkbudgLineResp.RoadWorkBudgetLine.RehabilitationWork = rehabiliatation;
                                rdworkbudgLineResp.RoadWorkBudgetLine.NewStructure = newStructure;
                                rdworkbudgLineResp.RoadWorkBudgetLine.UpdatedBy = user.UserName;
                                rdworkbudgLineResp.RoadWorkBudgetLine.UpdateDate = DateTime.UtcNow;
                                rdworkbudgLineResp = await _roadWorkBudgetLinesService.UpdateAsync(rdworkbudgLineResp.RoadWorkBudgetLine).ConfigureAwait(false);
                            }
                            else
                            {
                                RoadWorkBudgetLine roadWorkBudgetLine = new RoadWorkBudgetLine();
                                roadWorkBudgetLine.RoadWorkBudgetHeaderId = rdWorkBudgHeader.RoadWorkBudgetHeader.ID;
                                roadWorkBudgetLine.FundingSourceId = 15;//Todo:Hardcoded 22% RMLF  Portion
                                roadWorkBudgetLine.FundTypeId = 2;//Todo:Regular
                                rdworkbudgLineResp.RoadWorkBudgetLine = roadWorkBudgetLine;
                                rdworkbudgLineResp.RoadWorkBudgetLine.Administration = Administration;
                                rdworkbudgLineResp.RoadWorkBudgetLine.RoutineMaintanance = routineMaintenance;
                                rdworkbudgLineResp.RoadWorkBudgetLine.PeriodicMentanance = periodicMaintenance;
                                rdworkbudgLineResp.RoadWorkBudgetLine.SpotImprovement = spotImprovement;
                                rdworkbudgLineResp.RoadWorkBudgetLine.StructureConstruction = structure;
                                rdworkbudgLineResp.RoadWorkBudgetLine.PBCManagement = PBCManagement;
                                rdworkbudgLineResp.RoadWorkBudgetLine.RAM = RAM;
                                rdworkbudgLineResp.RoadWorkBudgetLine.Network = Network;
                                rdworkbudgLineResp.RoadWorkBudgetLine.WeighBridges = WeighBridges;
                                rdworkbudgLineResp.RoadWorkBudgetLine.AlehuOperationsAndPurchaseOfVehicles = AlehuOperationsAndPurchaseOfVehicles;
                                rdworkbudgLineResp.RoadWorkBudgetLine.AxleLoadActivities = AxleLoadActivities;
                                rdworkbudgLineResp.RoadWorkBudgetLine.EmergencyWorks = EmergencyWorks;
                                rdworkbudgLineResp.RoadWorkBudgetLine.RoadSafety = RoadSafety;
                                rdworkbudgLineResp.RoadWorkBudgetLine.RoadConditionSurvey = RoadConditionSurvey;
                                rdworkbudgLineResp.RoadWorkBudgetLine.BasedContracts = BasedContracts;
                                rdworkbudgLineResp.RoadWorkBudgetLine.Matters = Matters;
                                rdworkbudgLineResp.RoadWorkBudgetLine.Support = Support;
                                rdworkbudgLineResp.RoadWorkBudgetLine.HQActivities = HQActivities;
                                rdworkbudgLineResp.RoadWorkBudgetLine.RSIPCriticalLinks = RSIPCriticalLinks;
                                rdworkbudgLineResp.RoadWorkBudgetLine.RehabilitationWork = rehabiliatation;
                                rdworkbudgLineResp.RoadWorkBudgetLine.NewStructure = newStructure;
                                roadWorkBudgetLine.CreatedBy = user.UserName;
                                roadWorkBudgetLine.CreationDate = DateTime.UtcNow;
                                rdworkbudgLineResp = await _roadWorkBudgetLinesService.AddAsync(roadWorkBudgetLine).ConfigureAwait(false);
                            }
                        }
                        catch (Exception Ex)
                        {
                            _logger.LogError(Ex, $"Roles.Index Page Error: {Ex.Message} " +
                            $"{Environment.NewLine}");
                        }
                    }
                }
                else
                {
                    //Add
                    RoadWorkBudgetHeader roadWorkBudgetHeader = new RoadWorkBudgetHeader();
                    roadWorkBudgetHeader.FinancialYearId = updateResp.BudgetCeilingHeader.FinancialYearId;
                    var userAuthority = await _authorityService.FindByIdAsync(AuthorityID).ConfigureAwait(false);
                    if (userAuthority != null)
                    {
                        roadWorkBudgetHeader.Authority = userAuthority.Authority;
                    }
                    roadWorkBudgetHeader.Code = updateResp.BudgetCeilingHeader.FinancialYear.Code;
                    roadWorkBudgetHeader.Summary = "Budget Plan for Kenya Rural Roads Authority for Fiscal Year :" + roadWorkBudgetHeader.Code;
                    roadWorkBudgetHeader.SubmittedBy = user.UserName;
                    roadWorkBudgetHeader.SubmissionDate = DateTime.UtcNow;
                    roadWorkBudgetHeader.ApprovalStatus = updateResp.BudgetCeilingHeader.ApprovalStatus;
                    roadWorkBudgetHeader.ApprovedBy = user.UserName;

                    var rdWorkBudgHeader = await _roadWorkBudgetHeaderService.AddAsync(roadWorkBudgetHeader).ConfigureAwait(false);
                    if (rdWorkBudgHeader.Success)
                    {
                        //Add the activities to the roads works header items
                        try
                        {
                            //find by RoadWorkbudgetheaderid
                            var rdworkbudgLineResp = await _roadWorkBudgetLinesService.FindByRoadWorkBudgetHeaderIdAsync(rdWorkBudgHeader.RoadWorkBudgetHeader.ID).ConfigureAwait(false);
                            //if exists --update else add async
                            if (rdworkbudgLineResp.Success)
                            {
                                rdworkbudgLineResp.RoadWorkBudgetLine.FundingSourceId = 15;//Todo:Hardcoded 22% RMLF  Portion
                                rdworkbudgLineResp.RoadWorkBudgetLine.FundTypeId = 2;//Todo:Regular
                                rdworkbudgLineResp.RoadWorkBudgetLine.Administration = Administration;
                                rdworkbudgLineResp.RoadWorkBudgetLine.RoutineMaintanance = routineMaintenance;
                                rdworkbudgLineResp.RoadWorkBudgetLine.PeriodicMentanance = periodicMaintenance;
                                rdworkbudgLineResp.RoadWorkBudgetLine.SpotImprovement = spotImprovement;
                                rdworkbudgLineResp.RoadWorkBudgetLine.StructureConstruction = structure;
                                rdworkbudgLineResp.RoadWorkBudgetLine.PBCManagement = PBCManagement;
                                rdworkbudgLineResp.RoadWorkBudgetLine.RAM = RAM;
                                rdworkbudgLineResp.RoadWorkBudgetLine.Network = Network;
                                rdworkbudgLineResp.RoadWorkBudgetLine.WeighBridges = WeighBridges;
                                rdworkbudgLineResp.RoadWorkBudgetLine.AlehuOperationsAndPurchaseOfVehicles = AlehuOperationsAndPurchaseOfVehicles;
                                rdworkbudgLineResp.RoadWorkBudgetLine.AxleLoadActivities = AxleLoadActivities;
                                rdworkbudgLineResp.RoadWorkBudgetLine.EmergencyWorks = EmergencyWorks;
                                rdworkbudgLineResp.RoadWorkBudgetLine.RoadSafety = RoadSafety;
                                rdworkbudgLineResp.RoadWorkBudgetLine.RoadConditionSurvey = RoadConditionSurvey;
                                rdworkbudgLineResp.RoadWorkBudgetLine.BasedContracts = BasedContracts;
                                rdworkbudgLineResp.RoadWorkBudgetLine.Matters = Matters;
                                rdworkbudgLineResp.RoadWorkBudgetLine.Support = Support;
                                rdworkbudgLineResp.RoadWorkBudgetLine.HQActivities = HQActivities;
                                rdworkbudgLineResp.RoadWorkBudgetLine.RSIPCriticalLinks = RSIPCriticalLinks;
                                rdworkbudgLineResp.RoadWorkBudgetLine.RehabilitationWork = rehabiliatation;
                                rdworkbudgLineResp.RoadWorkBudgetLine.NewStructure = newStructure;
                                rdworkbudgLineResp.RoadWorkBudgetLine.UpdatedBy = user.UserName;
                                rdworkbudgLineResp.RoadWorkBudgetLine.UpdateDate = DateTime.UtcNow;
                                rdworkbudgLineResp = await _roadWorkBudgetLinesService.UpdateAsync(rdworkbudgLineResp.RoadWorkBudgetLine).ConfigureAwait(false);
                            }
                            else
                            {
                                RoadWorkBudgetLine roadWorkBudgetLine = new RoadWorkBudgetLine();
                                roadWorkBudgetLine.RoadWorkBudgetHeaderId = rdWorkBudgHeader.RoadWorkBudgetHeader.ID;
                                roadWorkBudgetLine.FundingSourceId = 15;//Todo:Hardcoded 22% RMLF  Portion
                                roadWorkBudgetLine.FundTypeId = 2;//Todo:Regular
                                rdworkbudgLineResp.RoadWorkBudgetLine.Administration = Administration;
                                rdworkbudgLineResp.RoadWorkBudgetLine.RoutineMaintanance = routineMaintenance;
                                rdworkbudgLineResp.RoadWorkBudgetLine.PeriodicMentanance = periodicMaintenance;
                                rdworkbudgLineResp.RoadWorkBudgetLine.SpotImprovement = spotImprovement;
                                rdworkbudgLineResp.RoadWorkBudgetLine.StructureConstruction = structure;
                                rdworkbudgLineResp.RoadWorkBudgetLine.PBCManagement = PBCManagement;
                                rdworkbudgLineResp.RoadWorkBudgetLine.RAM = RAM;
                                rdworkbudgLineResp.RoadWorkBudgetLine.Network = Network;
                                rdworkbudgLineResp.RoadWorkBudgetLine.WeighBridges = WeighBridges;
                                rdworkbudgLineResp.RoadWorkBudgetLine.AlehuOperationsAndPurchaseOfVehicles = AlehuOperationsAndPurchaseOfVehicles;
                                rdworkbudgLineResp.RoadWorkBudgetLine.AxleLoadActivities = AxleLoadActivities;
                                rdworkbudgLineResp.RoadWorkBudgetLine.EmergencyWorks = EmergencyWorks;
                                rdworkbudgLineResp.RoadWorkBudgetLine.RoadSafety = RoadSafety;
                                rdworkbudgLineResp.RoadWorkBudgetLine.RoadConditionSurvey = RoadConditionSurvey;
                                rdworkbudgLineResp.RoadWorkBudgetLine.BasedContracts = BasedContracts;
                                rdworkbudgLineResp.RoadWorkBudgetLine.Matters = Matters;
                                rdworkbudgLineResp.RoadWorkBudgetLine.Support = Support;
                                rdworkbudgLineResp.RoadWorkBudgetLine.HQActivities = HQActivities;
                                rdworkbudgLineResp.RoadWorkBudgetLine.RSIPCriticalLinks = RSIPCriticalLinks;
                                rdworkbudgLineResp.RoadWorkBudgetLine.RehabilitationWork = rehabiliatation;
                                rdworkbudgLineResp.RoadWorkBudgetLine.NewStructure = newStructure;
                                roadWorkBudgetLine.CreatedBy = user.UserName;
                                roadWorkBudgetLine.CreationDate = DateTime.UtcNow;
                                rdworkbudgLineResp = await _roadWorkBudgetLinesService.AddAsync(roadWorkBudgetLine).ConfigureAwait(false);
                            }
                        }
                        catch (Exception Ex)
                        {
                            _logger.LogError(Ex, $"Roles.Index Page Error: {Ex.Message} " +
                            $"{Environment.NewLine}");
                        }
                    }
                }
            }
            catch (System.Exception Ex)
            {
                _logger.LogError(Ex, $"Roles.Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
            }
        }

        private async Task StoreBudgetCeilingComputation(long FinancialYearID)
        {
            CeilingsEstimateViewModel ceilingsEstimateViewModel = await DraftBudget(FinancialYearID).ConfigureAwait(false);

            //TotalFundsAvailable ==code=1
            BudgetCeilingComputation budgetCeilingComputation = null;
            var budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("1", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.TotalFundsAvailable;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            var respAuthority = await _authorityService.FindByCodeAsync("KRB").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                //New
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "1";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.TotalFundsAvailable;
                                budgetCeilingComputation.Name = " TOTAL FUNDS AVAILABLE ";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }
            
            //Fuel Levy ==code=2
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("2", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.FuelLevy;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            var respAuthority = await _authorityService.FindByCodeAsync("KRB").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                //New
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "2";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.FuelLevy;
                                budgetCeilingComputation.Name = "Fuel Levy";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            //TransitTolls ==code=3
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("3", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.TransitTolls;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            var respAuthority = await _authorityService.FindByCodeAsync("KRB").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                //New
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "3";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.TransitTolls;
                                budgetCeilingComputation.Name = "Transit Tolls";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            //KRBOperations ==code=4
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("4", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KRBOperations;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            var respAuthority = await _authorityService.FindByCodeAsync("KRB").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                //New
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "4";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KRBOperations;
                                budgetCeilingComputation.Name = "KRB Operations";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }
            if (budgetCeilingComputation != null)
            {
                await UpdateDisbursementCodeList(budgetCeilingComputation.Code, budgetCeilingComputation.AuthorityId).ConfigureAwait(false);
            }


            // KRB 2% Transit Trolls ==code=5
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("5", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KRBTransitTrolls;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            var respAuthority = await _authorityService.FindByCodeAsync("KRB").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                //New
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "5";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KRBTransitTrolls;
                                budgetCeilingComputation.Name = "KRB: 2% Transit Tolls";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            // KRBTotalOperations ==code=6
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("6", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KRBTotalOperations;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            var respAuthority = await _authorityService.FindByCodeAsync("KRB").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                //New
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "6";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KRBTotalOperations;
                                budgetCeilingComputation.Name = "KRB OPERATIONS TOTAL";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            // NETFundsAvailableAfterAdministrationCosts ==code=7
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("7", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.NETFundsAvailableAfterAdministrationCosts;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            var respAuthority = await _authorityService.FindByCodeAsync("KRB").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                //New
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "7";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.NETFundsAvailableAfterAdministrationCosts;
                                budgetCeilingComputation.Name = "KRB OPERATIONS TOTAL";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            // Road_Agencies_Fuel_Levy ==code=8
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("8", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.Road_Agencies_Fuel_Levy;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            var respAuthority = await _authorityService.FindByCodeAsync("KRB").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                //New
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "8";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.Road_Agencies_Fuel_Levy;
                                budgetCeilingComputation.Name = "KRB OPERATIONS TOTAL";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            //Road_Annuity_Fund ==code=9
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("9", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.Road_Annuity_Fund;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            var respAuthority = await _authorityService.FindByCodeAsync("KRB").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                //New
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "9";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.Road_Annuity_Fund;
                                budgetCeilingComputation.Name = "Road Annuity Fund";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            //SubTotal_Fuel_Levy ==code=10
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("10", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.SubTotal_Fuel_Levy;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            var respAuthority = await _authorityService.FindByCodeAsync("KRB").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                //New
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "10";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.SubTotal_Fuel_Levy;
                                budgetCeilingComputation.Name = "Sub-total Fuel Levy" ;
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            //SubTotal_Fuel_Levy ==code=11
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("11", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.NetTransitTolls;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            var respAuthority = await _authorityService.FindByCodeAsync("KRB").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                //New
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "11";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.NetTransitTolls;
                                budgetCeilingComputation.Name = "Transit Tolls";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            //SubTotal_Fuel_Levy ==code=12
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("12", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.TOTALFundsForAllocation;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            var respAuthority = await _authorityService.FindByCodeAsync("KRB").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                //New
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "12";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.TOTALFundsForAllocation;
                                budgetCeilingComputation.Name = "TOTAL Funds For Allocation";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            //RMLFLessRAF ==code=13
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("13", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.RMLFLessRAF;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            var respAuthority = await _authorityService.FindByCodeAsync("KRB").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                //New
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "13";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.RMLFLessRAF;
                                budgetCeilingComputation.Name = "TOTAL Funds For Allocation";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            //CountyAllocation ==code=14
            budgetCeilingComputation = null;
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("14", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.CountyAllocation;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            //New
                            var respAuthority = await _authorityService.FindByCodeAsync("CGs").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "14";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.CountyAllocation;
                                budgetCeilingComputation.Name = "County Allocation";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }
            if (budgetCeilingComputation != null)
            {
                await UpdateDisbursementCodeList(budgetCeilingComputation.Code, budgetCeilingComputation.AuthorityId).ConfigureAwait(false);
            }

            //kenhaallocation ==code=15
            budgetCeilingComputation = null;
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("15", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.kenhaAllocation;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            //New
                            var respAuthority = await _authorityService.FindByCodeAsync("kenha").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "15";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.kenhaAllocation;
                                budgetCeilingComputation.Name = "40% RMLF";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }
            if (budgetCeilingComputation != null)
            {
                await UpdateDisbursementCodeList(budgetCeilingComputation.Code, budgetCeilingComputation.AuthorityId).ConfigureAwait(false);
            }

            //kenhaTransitTolls ==code=16
            budgetCeilingComputation = null;
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("16", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.kenhaTransitTolls;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            //New
                            var respAuthority = await _authorityService.FindByCodeAsync("kenha").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "16";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.kenhaTransitTolls;
                                budgetCeilingComputation.Name = "Transit Toll";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            //kenhaTotalAllocation ==code=17
            budgetCeilingComputation = null;
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("17", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.kenhaTotalAllocation;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            //New
                            var respAuthority = await _authorityService.FindByCodeAsync("kenha").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "17";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.kenhaTotalAllocation;
                                budgetCeilingComputation.Name = "Total allocation to KeNHA";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            //kenhaAdminOperations ==code=18
            budgetCeilingComputation = null;
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("18", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.kenhaAdminOperations;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            //New
                            var respAuthority = await _authorityService.FindByCodeAsync("kenha").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "18";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.kenhaAdminOperations;
                                budgetCeilingComputation.Name = "KeNHA 4%";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            //kenhaRoadWorks ==code=19
            budgetCeilingComputation = null;
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("19", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.kenhaRoadWorks;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            //New
                            var respAuthority = await _authorityService.FindByCodeAsync("kenha").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "19";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.kenhaRoadWorks;
                                budgetCeilingComputation.Name = "RoadWorks";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            //KRBBoard_CSAllocation ==code=20
            budgetCeilingComputation = null;
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("20", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KRBBoard_CSAllocation;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            //New
                            var respAuthority = await _authorityService.FindByCodeAsync("RSIP/CS").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "20";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KRBBoard_CSAllocation;
                                budgetCeilingComputation.Name = "KRB Board/CS Allocation";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }
            if (budgetCeilingComputation != null)
            {
                await UpdateDisbursementCodeList(budgetCeilingComputation.Code, budgetCeilingComputation.AuthorityId).ConfigureAwait(false);
            }

            //KWSAllocation ==code=21
            budgetCeilingComputation = null;
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("21", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KWSAllocation;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            //New
                            var respAuthority = await _authorityService.FindByCodeAsync("kws").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "21";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KWSAllocation;
                                budgetCeilingComputation.Name = "National Park Roads (KWS)";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }
            if (budgetCeilingComputation != null)
            {
                await UpdateDisbursementCodeList(budgetCeilingComputation.Code, budgetCeilingComputation.AuthorityId).ConfigureAwait(false);
            }

            //KURAAllocation ==code=22
            budgetCeilingComputation = null;
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("22", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KURAAllocation;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            //New
                            var respAuthority = await _authorityService.FindByCodeAsync("kura").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "22";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KURAAllocation;
                                budgetCeilingComputation.Name = "10.2% RMLF";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }
            if (budgetCeilingComputation != null)
            {
                await UpdateDisbursementCodeList(budgetCeilingComputation.Code, budgetCeilingComputation.AuthorityId).ConfigureAwait(false);
            }

            //KURAAdminOperations ==code=23
            budgetCeilingComputation = null;
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("23", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KURAAdminOperations;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            //New
                            var respAuthority = await _authorityService.FindByCodeAsync("kura").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "23";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KURAAdminOperations;
                                budgetCeilingComputation.Name = "KURA 5.5% (Caping Amount = 1,892,000,000)";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            //KURARoadWorks ==code=25
            budgetCeilingComputation = null;
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("24", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KURARoadWorks;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            //New
                            var respAuthority = await _authorityService.FindByCodeAsync("kura").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "24";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KURARoadWorks;
                                budgetCeilingComputation.Name = "Roadworks";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            //kenhaTotal ==code=24
            budgetCeilingComputation = null;
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("25", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.kenhaTotal;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            //New
                            var respAuthority = await _authorityService.FindByCodeAsync("kenha").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "25";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.kenhaTotal;
                                budgetCeilingComputation.Name = "Roadworks";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            //SubtotalFundsAllocated ==code=26
            budgetCeilingComputation = null;
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("26", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.SubtotalFundsAllocated;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            //New
                            var respAuthority = await _authorityService.FindByCodeAsync("KRB").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "26";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.SubtotalFundsAllocated;
                                budgetCeilingComputation.Name = "Sub-TOTAL- Funds Allocated";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            //GrossRMLF ==code=27
            budgetCeilingComputation = null;
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("27", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.GrossRMLF;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            //New
                            var respAuthority = await _authorityService.FindByCodeAsync("KRB").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "27";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.GrossRMLF;
                                budgetCeilingComputation.Name = "Gross KRBF";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }


            //KERRAAdminOperations ==code=28
            budgetCeilingComputation = null;
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("28", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KERRAAdminOperations;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            //New
                            var respAuthority = await _authorityService.FindByCodeAsync("kerra").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "28";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KERRAAdminOperations;
                                budgetCeilingComputation.Name = "KeRRA 5.5%";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            //KERRAAllocation ==code=30
            budgetCeilingComputation = null;
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("30", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KERRAAllocation;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            //New
                            var respAuthority = await _authorityService.FindByCodeAsync("kerra").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "30";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KERRAAllocation;
                                budgetCeilingComputation.Name = "Total allocation to KeRRA";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            //KERRAConstituencyAllocation ==code=31
            budgetCeilingComputation = null;
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("31", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KERRAConstituencyAllocation;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            //New
                            var respAuthority = await _authorityService.FindByCodeAsync("kerra").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "31";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KERRAConstituencyAllocation;
                                budgetCeilingComputation.Name = "Constituency allocation(22/32 of KeRRA alloc";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }
            if (budgetCeilingComputation != null)
            {
                await UpdateDisbursementCodeList(budgetCeilingComputation.Code, 0.15).ConfigureAwait(false);
            }

            //KERRACriticalLinksAllocation ==code=32
            budgetCeilingComputation = null;
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("32", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KERRACriticalLinksAllocation;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            //New

                            var respAuthority = await _authorityService.FindByCodeAsync("kerra").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "32";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KERRACriticalLinksAllocation;
                                budgetCeilingComputation.Name = "Critical Links allocation(10/32 of KeRRA alloc.";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }
            if (budgetCeilingComputation != null)
            {
                await UpdateDisbursementCodeList(budgetCeilingComputation.Code, 0.068).ConfigureAwait(false);
            }

            //KERRATotalAdminBudget ==code=33
            budgetCeilingComputation = null;
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("33", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KERRATotalAdminBudget;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            //New
                            var respAuthority = await _authorityService.FindByCodeAsync("kerra").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "33";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KERRATotalAdminBudget;
                                budgetCeilingComputation.Name = "Total admin budget(5.5% of RMLF)";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            //KERRAPortinAdminTwentyTwoPercent ==code=34
            budgetCeilingComputation = null;
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("34", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KERRAPortinAdminTwentyTwoPercent;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            //New
                            var respAuthority = await _authorityService.FindByCodeAsync("kerra").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "34";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KERRAPortinAdminTwentyTwoPercent;
                                budgetCeilingComputation.Name = "22% portion admin(22/32)";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            //KERRAPortinAdminTenPercent ==code=35
            budgetCeilingComputation = null;
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("35", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KERRAPortinAdminTenPercent;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            //New
                            var respAuthority = await _authorityService.FindByCodeAsync("kerra").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "35";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KERRAPortinAdminTenPercent;
                                budgetCeilingComputation.Name = "10% portion admin(10/32)";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            //KERRAPortinRoadWorksTwentyTwoPercent ==code=36
            budgetCeilingComputation = null;
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("36", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KERRAPortinRoadWorksTwentyTwoPercent;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            //New
                            var respAuthority = await _authorityService.FindByCodeAsync("kerra").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "36";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KERRAPortinRoadWorksTwentyTwoPercent;
                                budgetCeilingComputation.Name = "22% portion road works";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            //KERRAPortinRoadworksTenPercent ==code=37
            budgetCeilingComputation = null;
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("37", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KERRAPortinRoadworksTenPercent;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            //New
                            var respAuthority = await _authorityService.FindByCodeAsync("kerra").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "37";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KERRAPortinRoadworksTenPercent;
                                budgetCeilingComputation.Name = "10% portion road works";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            //KERRATotalBudgetForWorks ==code=38
            budgetCeilingComputation = null;
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("38", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KERRATotalBudgetForWorks;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            //New
                            var respAuthority = await _authorityService.FindByCodeAsync("kerra").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "38";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KERRATotalBudgetForWorks;
                                budgetCeilingComputation.Name = "Total budget for works";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            //KERRATwentyTwoPercentAllocPerConstituency ==code=39
            budgetCeilingComputation = null;
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("39", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KERRATwentyTwoPercentAllocPerConstituency;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            //New
                            var respAuthority = await _authorityService.FindByCodeAsync("kerra").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "39";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KERRATwentyTwoPercentAllocPerConstituency;
                                budgetCeilingComputation.Name = "22% alloc. /Constituency";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            //KERRATenPercentAllocPerConstituency ==code=40
            budgetCeilingComputation = null;
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("40", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KERRATenPercentAllocPerConstituency;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            //New
                            var respAuthority = await _authorityService.FindByCodeAsync("kerra").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "40";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KERRATenPercentAllocPerConstituency;
                                budgetCeilingComputation.Name = "10% alloc. /Constituency";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

            //KERRATotalAllocPerConstituency ==code=41
            budgetCeilingComputation = null;
            budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("41", FinancialYearID).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                        if (budgetCeilingComputation != null)
                        {
                            //Update
                            budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KERRATotalAllocPerConstituency;
                            var budgetCeilingComputationResp2 = await _budgetCeilingComputationService.Update(budgetCeilingComputation.ID, budgetCeilingComputation).ConfigureAwait(false);
                        }
                        else
                        {
                            //New
                            var respAuthority = await _authorityService.FindByCodeAsync("kerra").ConfigureAwait(false);
                            if (respAuthority.Success)
                            {
                                budgetCeilingComputation = new BudgetCeilingComputation();
                                budgetCeilingComputation.AuthorityId = respAuthority.Authority.ID;
                                budgetCeilingComputation.Code = "41";
                                budgetCeilingComputation.Amount = ceilingsEstimateViewModel.KERRATotalAllocPerConstituency;
                                budgetCeilingComputation.Name = "Total Alloc. /Constituency";
                                budgetCeilingComputation.FinancialYearId = FinancialYearID;
                                var budgetCeilingComputationResp2 = await _budgetCeilingComputationService
                                    .AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                            }

                        }
                    }
                }
            }

        }

        /// <summary>
        /// Update the percent figure
        /// </summary>
        /// <returns></returns>
        private async Task UpdateDisbursementCodeList(String Code, long AuthorityID)
        {

            try
            {
                //Get allocation code unit
                var allocationCodeUnitResp = await _allocationCodeUnitService.FindByAuthorityAsync(AuthorityID).ConfigureAwait(false);

                if (allocationCodeUnitResp.Success)
                {
                    AllocationCodeUnit allocationCodeUnit = allocationCodeUnitResp.AllocationCodeUnit;

                    //Find disbursementcodelist 
                    var disbursementCodeListResp = await _disbursementCodeListService.FindByCodeAsync(Code).ConfigureAwait(false);
                    if (disbursementCodeListResp.Success)
                    {
                        //Detach the first entry before attaching your updated entry
                        DisbursementCodeList disbursementCodeList = null;
                        if (disbursementCodeListResp.Success)
                        {
                            var objectResult = (ObjectResult)disbursementCodeListResp.IActionResult;
                            if (objectResult != null)
                            {
                                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    var result3 = (OkObjectResult)objectResult;
                                    disbursementCodeList = (DisbursementCodeList)result3.Value;

                                }
                            }
                        }
                        var respDetach = await _disbursementCodeListService.DetachFirstEntryAsync(disbursementCodeList).ConfigureAwait(false);
                        //return after successful detach

                        //Attach percent and update
                        disbursementCodeList.Percent = allocationCodeUnit.Percent;

                        respDetach = await _disbursementCodeListService.Update(disbursementCodeList.ID, disbursementCodeList).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionController.UpdateDisbursementCodeList Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
            }
        }

        private async Task UpdateDisbursementCodeList(String Code, double Percent)
        {
            try
            {
                //Find disbursementcodelist 
                var disbursementCodeListResp = await _disbursementCodeListService.FindByCodeAsync(Code).ConfigureAwait(false);
                if (disbursementCodeListResp.Success)
                {
                    //Detach the first entry before attaching your updated entry
                    DisbursementCodeList disbursementCodeList = null;
                    if (disbursementCodeListResp.Success)
                    {
                        var objectResult = (ObjectResult)disbursementCodeListResp.IActionResult;
                        if (objectResult != null)
                        {
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var result3 = (OkObjectResult)objectResult;
                                disbursementCodeList = (DisbursementCodeList)result3.Value;

                            }
                        }
                    }
                    var respDetach = await _disbursementCodeListService.DetachFirstEntryAsync(disbursementCodeList).ConfigureAwait(false);
                    //return after successful detach

                    //Attach percent and update
                    disbursementCodeList.Percent = Percent;

                    respDetach = await _disbursementCodeListService.Update(disbursementCodeList.ID, disbursementCodeList).ConfigureAwait(false);
                }


            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionController.UpdateDisbursementCodeList Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
            }
        }
        #endregion

        #region Utilities
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task SetAuthorityAllocationAmounts(RevenueCollectionViewModel revenueCollectionViewModel)
        {
            try
            {
                //Get allocationscodeunits for the year of interest
                var allocationCodeUnitResp = await _allocationCodeUnitService.ListAsync("RA").ConfigureAwait(false);

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
                _logger.LogError(Ex, $"ARICSService.GetIRI Error: {Environment.NewLine}");
            }
        }

        public async Task<IActionResult> Download(string filename, string folder)
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
        public async Task<JsonResult> DeleteBudgetAttachment(long Id, string filename, long FinancialID)
        {
            try
            {
                Upload upload = null;
                //delete if at submit stage
                var resp = await _budgetCeilingHeaderService.FindCurrentAsync().ConfigureAwait(false); //Todo: Fix need to get budget header for the respective fincial year
                if (resp.Success)
                {
                    if (resp.BudgetCeilingHeader.ApprovalStatus == 0)
                    {
                        //delete the file
                        Boolean FileDelete = DeleteFile(filename, "budgets");

                        var uploadResponse = await _uploadService.RemoveAsync(Id).ConfigureAwait(false);
                        upload = uploadResponse.Upload;
                    }
                }
                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    data = upload,
                    Href = Url.Action("Index", "RevenueCollection")
                });

            }
            catch (Exception Ex)
            {
                _logger.LogError($"DeleteBudgetAttachment Error: {Ex.Message}");
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

        #region Reports
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.BudgetCeiling.Report)]
        public async Task<JsonResult> ExportBudgetCeilingReport(long FinancialYearID)
        {
            try
            {

                var ceilingsEstimateViewModel = await DraftBudget(FinancialYearID).ConfigureAwait(false);

                MemoryStream stream = new MemoryStream();
                JsonResult myjson = await WriteToBudgetCeilingReport(ceilingsEstimateViewModel, FinancialYearID, stream).ConfigureAwait(false);

                return Json(myjson);


            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionController.ExportBudgetCeilingReport Error {Environment.NewLine}");
                return Json(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<JsonResult> WriteToBudgetCeilingReport(CeilingsEstimateViewModel ceilingsEstimateViewModel, long FinancialYearID, MemoryStream stream)
        {
            //Create a new ExcelPackage  
            string handle = Guid.NewGuid().ToString();
            using (ExcelPackage excelPackage = new ExcelPackage(stream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = "KRB";
                excelPackage.Workbook.Properties.Title = "Ceiling_Estimates_Format_Report";
                excelPackage.Workbook.Properties.Subject = "Ceiling Estimates Format Report";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Allocations");

                //Add some text to cell A1
                worksheet.Cells["B1"].Value = "RECEIPTS";
                worksheet.Cells["B1"].Style.Font.Bold = true;
                worksheet.Cells["D1"].Value = "Estimates For";
                worksheet.Cells["B1:D1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B1:D1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B1:D1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B1:D1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#BDD7EE");
                worksheet.Cells["B1:D1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["B1:D1"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                //get financial year
                var fyearResp = await _financialYearService.FindByIdAsync(FinancialYearID).ConfigureAwait(false);
                if (fyearResp.Success)
                {
                    worksheet.Cells["D2"].Value = $"FY {fyearResp.FinancialYear.Code}";
                }
                worksheet.Cells["D2"].Style.Font.Bold = true;
                worksheet.Cells["B2:D2"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B2:D2"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B2:D2"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B2:D2"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["D3"].Value = "Kshs";
                worksheet.Cells["D3"].Style.Font.Bold = true;
                worksheet.Cells["B3:D3"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B3:D3"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B3:D3"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B3:D3"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                //Get Fuel Levy
                worksheet.Cells["A4"].Value = "1";
                worksheet.Cells["B4"].Value = "Grants Received by Other General Government Units from Fund Accounts:";
                worksheet.Cells["B4"].Style.Font.Bold = true;
                worksheet.Cells["B5"].Value = "(i) Fuel Levy";
                worksheet.Cells["B5"].Style.Font.Bold = true;
                worksheet.Cells["D5"].Value = ceilingsEstimateViewModel.FuelLevy;
                worksheet.Cells["D5"].Style.Numberformat.Format = "#,##0";
                worksheet.Cells["D5"].Style.Font.Bold = true;

                //Get Transit Tolls
                worksheet.Cells["A7"].Value = "2";
                worksheet.Cells["B7"].Value = "Receipts of Taxes on Goods and Services:";
                worksheet.Cells["B7"].Style.Font.Bold = true;
                worksheet.Cells["B8"].Value = "(ii) Transit Tolls";
                worksheet.Cells["B8"].Style.Font.Bold = true;
                worksheet.Cells["D8"].Value = ceilingsEstimateViewModel.TransitTolls;
                worksheet.Cells["D8"].Style.Numberformat.Format = "#,##0";
                worksheet.Cells["D8"].Style.Font.Bold = true;

                //Total Funds Available
                worksheet.Cells["A10"].Value = "3";
                worksheet.Cells["B10"].Value = "TOTAL FUNDS AVAILABLE";
                worksheet.Cells["B10"].Style.Font.Bold = true;
                worksheet.Cells["D10"].Value = ceilingsEstimateViewModel.TotalFundsAvailable;
                worksheet.Cells["D10"].Style.Numberformat.Format = "#,##0";
                worksheet.Cells["D10"].Style.Font.Bold = true;
                worksheet.Cells["B10:D10"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B10:D10"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B10:D10"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B10:D10"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                colFromHex = System.Drawing.ColorTranslator.FromHtml("#BDD7EE");
                worksheet.Cells["B10:D10"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["B10:D10"].Style.Fill.BackgroundColor.SetColor(colFromHex);

                //DISBURSEMENTS                
                worksheet.Cells["B12"].Value = "DISBURSEMENTS";
                worksheet.Cells["B12"].Style.Font.Bold = true;
                worksheet.Cells["B12:D12"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B12:D12"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B12:D12"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B12:D12"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                colFromHex = System.Drawing.ColorTranslator.FromHtml("#BDD7EE");
                worksheet.Cells["B12:D12"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["B12:D12"].Style.Fill.BackgroundColor.SetColor(colFromHex);

                worksheet.Cells["A15"].Value = "4";
                worksheet.Cells["B15"].Value = "KRB Operations";
                worksheet.Cells["C15"].Value = "2%";
                worksheet.Cells["C15"].Style.Font.Bold = true;
                worksheet.Cells["D15"].Value = ceilingsEstimateViewModel.KRBOperations;
                worksheet.Cells["D15"].Style.Numberformat.Format = "#,##0";

                worksheet.Cells["A16"].Value = "5";
                worksheet.Cells["B16"].Value = "KRB: 2% Transit Tolls";
                worksheet.Cells["C16"].Value = "";
                worksheet.Cells["D16"].Value = ceilingsEstimateViewModel.KRBTransitTrolls;
                worksheet.Cells["D16"].Style.Numberformat.Format = "#,##0";

                worksheet.Cells["A17"].Value = "6";
                worksheet.Cells["A17"].Style.Font.Bold = true;
                worksheet.Cells["B17"].Value = "KRB OPERATIONS TOTAL";
                worksheet.Cells["C17"].Value = "";
                worksheet.Cells["D17"].Value = ceilingsEstimateViewModel.KRBTotalOperations;
                worksheet.Cells["D17"].Style.Numberformat.Format = "#,##0";
                worksheet.Cells["D17"].Style.Font.Bold = true;
                worksheet.Cells["B17:D17"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B17:D17"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B17:D17"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B17:D17"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["A19"].Value = "7";
                worksheet.Cells["A19"].Style.Font.Bold = true;
                worksheet.Cells["B19"].Value = "NET Funds available after administration costs";
                worksheet.Cells["C19"].Value = "";
                worksheet.Cells["D19"].Value = ceilingsEstimateViewModel.NETFundsAvailableAfterAdministrationCosts;
                worksheet.Cells["D19"].Style.Numberformat.Format = "#,##0";
                worksheet.Cells["D19"].Style.Font.Bold = true;

                worksheet.Cells["A21"].Value = "8";
                worksheet.Cells["B21"].Value = "(i) Road Agencies-Fuel Levy ";
                worksheet.Cells["C21"].Value = "";
                worksheet.Cells["D21"].Value = ceilingsEstimateViewModel.Road_Agencies_Fuel_Levy;
                worksheet.Cells["D21"].Style.Numberformat.Format = "#,##0";

                worksheet.Cells["A22"].Value = "";
                worksheet.Cells["B22"].Value = "(ii) Road Annuity Fund";
                worksheet.Cells["C22"].Value = "";
                worksheet.Cells["D22"].Value = ceilingsEstimateViewModel.Road_Annuity_Fund;
                worksheet.Cells["D22"].Style.Numberformat.Format = "#,##0";
                worksheet.Cells["B22:D22"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["A23"].Value = "";
                worksheet.Cells["B23"].Value = "Sub-total Fuel Levy";
                worksheet.Cells["C23"].Value = "";
                worksheet.Cells["D23"].Value = ceilingsEstimateViewModel.SubTotal_Fuel_Levy;
                worksheet.Cells["D23"].Style.Numberformat.Format = "#,##0";

                worksheet.Cells["A24"].Value = "";
                worksheet.Cells["B24"].Value = "(ii)Transit Tolls ";
                worksheet.Cells["C24"].Value = "";
                worksheet.Cells["D24"].Value = ceilingsEstimateViewModel.NetTransitTolls;
                worksheet.Cells["D24"].Style.Numberformat.Format = "#,##0";

                worksheet.Cells["A25"].Value = "9";
                worksheet.Cells["B25"].Value = "TOTAL Funds For Allocation";
                worksheet.Cells["B25"].Style.Font.Bold = true;
                worksheet.Cells["C25"].Value = "";
                worksheet.Cells["D25"].Value = ceilingsEstimateViewModel.TOTALFundsForAllocation;
                worksheet.Cells["D25"].Style.Numberformat.Format = "#,##0";
                worksheet.Cells["D25"].Style.Font.Bold = true;
                worksheet.Cells["B25:D25"].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                worksheet.Cells["B25:D25"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B25:D25"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B25:D25"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B25"].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                colFromHex = System.Drawing.ColorTranslator.FromHtml("#BDD7EE");
                worksheet.Cells["B25:D25"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["B25:D25"].Style.Fill.BackgroundColor.SetColor(colFromHex);

                worksheet.Cells["B27"].Value = "FUND ALLOCATION";
                worksheet.Cells["B27"].Style.Font.Bold = true;
                worksheet.Cells["B27:D27"].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                worksheet.Cells["A28"].Value = "10";
                worksheet.Cells["B28"].Value = "Road Annuity Fund";
                worksheet.Cells["B28"].Style.Font.Bold = true;
                worksheet.Cells["C28"].Value = "";
                worksheet.Cells["D28"].Value = ceilingsEstimateViewModel.Road_Annuity_Fund;
                worksheet.Cells["D28"].Style.Numberformat.Format = "#,##0";
                worksheet.Cells["D28"].Style.Font.Bold = true;

                worksheet.Cells["A29"].Value = "11";
                worksheet.Cells["B29"].Value = "County Allocation";
                worksheet.Cells["B29"].Style.Font.Bold = true;
                worksheet.Cells["C29"].Value = "15%";
                worksheet.Cells["C29"].Style.Font.Bold = true;
                worksheet.Cells["D29"].Value = ceilingsEstimateViewModel.CountyAllocation;
                worksheet.Cells["D29"].Style.Numberformat.Format = "#,##0";


                worksheet.Cells["A30"].Value = "12";
                worksheet.Cells["B30"].Value = "KeNHA";
                worksheet.Cells["B30"].Style.Font.Bold = true;
                worksheet.Cells["C30"].Value = "40.0%";
                worksheet.Cells["C30"].Style.Font.Bold = true;
                worksheet.Cells["D30"].Value = ceilingsEstimateViewModel.kenhaAllocation;
                worksheet.Cells["D30"].Style.Numberformat.Format = "#,##0";

                worksheet.Cells["A31"].Value = "13";
                worksheet.Cells["B31"].Value = "Transit Tolls ";
                worksheet.Cells["B31"].Style.Font.Bold = true;
                worksheet.Cells["C31"].Value = "";
                worksheet.Cells["D31"].Value = ceilingsEstimateViewModel.NetTransitTolls;
                worksheet.Cells["D31"].Style.Numberformat.Format = "#,##0";

                worksheet.Cells["A32"].Value = "14";
                worksheet.Cells["B32"].Value = "KeNHA TOTAL";
                worksheet.Cells["B32"].Style.Font.Bold = true;
                worksheet.Cells["C32"].Value = "";
                worksheet.Cells["D32"].Value = ceilingsEstimateViewModel.kenhaTotal;
                worksheet.Cells["D32"].Style.Numberformat.Format = "#,##0";
                worksheet.Cells["D32"].Style.Font.Bold = true;
                worksheet.Cells["B32:D32"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B32:D32"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B32:D32"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B32:D32"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                colFromHex = System.Drawing.ColorTranslator.FromHtml("#9BC2E6");
                worksheet.Cells["B32:D32"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["B32:D32"].Style.Fill.BackgroundColor.SetColor(colFromHex);


                worksheet.Cells["A33"].Value = "15";
                worksheet.Cells["B33"].Value = "KRB Board/CS Allocation";
                worksheet.Cells["C33"].Value = "10.0%";
                worksheet.Cells["D33"].Value = ceilingsEstimateViewModel.KRBBoard_CSAllocation;
                worksheet.Cells["D33"].Style.Numberformat.Format = "#,##0";
                worksheet.Cells["A33:D33"].Style.Font.Bold = true;


                worksheet.Cells["A34"].Value = "16";
                worksheet.Cells["B34"].Value = "National Park Roads (KWS)";
                worksheet.Cells["C34"].Value = "1.0%";
                worksheet.Cells["D34"].Value = ceilingsEstimateViewModel.KWSAllocation;
                worksheet.Cells["D34"].Style.Numberformat.Format = "#,##0";
                worksheet.Cells["A34:D34"].Style.Font.Bold = true;

                worksheet.Cells["A35"].Value = "17";
                worksheet.Cells["B35"].Value = "Rural Roads (KeRRA)";
                worksheet.Cells["C35"].Value = "21.8%";
                worksheet.Cells["D35"].Value = ceilingsEstimateViewModel.KERRAAllocation;
                worksheet.Cells["D35"].Style.Numberformat.Format = "#,##0";
                worksheet.Cells["A35:D35"].Style.Font.Bold = true;

                worksheet.Cells["A36"].Value = "18";
                worksheet.Cells["B36"].Value = "Urban Roads (KURA)";
                worksheet.Cells["C36"].Value = "10.2%";
                worksheet.Cells["D36"].Value = ceilingsEstimateViewModel.KURAAllocation;
                worksheet.Cells["D36"].Style.Numberformat.Format = "#,##0";
                worksheet.Cells["A36:D36"].Style.Font.Bold = true;

                worksheet.Cells["A37"].Value = "19";
                worksheet.Cells["B37"].Value = "Sub-TOTAL- Funds Allocated";
                worksheet.Cells["C37"].Value = "100%";
                worksheet.Cells["D37"].Value = ceilingsEstimateViewModel.SubtotalFundsAllocated;
                worksheet.Cells["D37"].Style.Numberformat.Format = "#,##0";
                worksheet.Cells["A37:D37"].Style.Font.Bold = true;
                worksheet.Cells["B37:D37"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B37:D37"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B37:D37"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B37:D37"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["D37"].Style.Border.Left.Style = ExcelBorderStyle.Thick;
                colFromHex = System.Drawing.ColorTranslator.FromHtml("#BDD7EE");
                worksheet.Cells["B37:D37"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["B37:D37"].Style.Fill.BackgroundColor.SetColor(colFromHex);

                worksheet.Cells["A39"].Value = "20";
                worksheet.Cells["B39"].Value = "Gross RMLF ";
                worksheet.Cells["C39"].Value = "";
                worksheet.Cells["D39"].Value = ceilingsEstimateViewModel.GrossRMLF;
                worksheet.Cells["D39"].Style.Numberformat.Format = "#,##0";
                worksheet.Cells["A39:D39"].Style.Font.Bold = true;
                worksheet.Cells["B39:D39"].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                worksheet.Cells["B39:D39"].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                worksheet.Cells["B39:D39"].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                worksheet.Cells["B39:D39"].Style.Border.Left.Style = ExcelBorderStyle.Thick;
                colFromHex = System.Drawing.ColorTranslator.FromHtml("#BDD7EE");
                worksheet.Cells["B39:D39"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["B39:D39"].Style.Fill.BackgroundColor.SetColor(colFromHex);


                //Formating
                worksheet.Cells["B1:B39"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B1:B39"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["C1:C39"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["D1:D39"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["C1:C39"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["D1:D39"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                /*
                 * Admin operations
                 */
                worksheet.Cells["B40"].Value = "Admin/Operations";
                //kura admin operations
                var resp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("23", FinancialYearID)
                    .ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            var budgetCeilingcomputation = (BudgetCeilingComputation)result.Value;
                            worksheet.Cells["B41"].Value = budgetCeilingcomputation.Name;
                            worksheet.Cells["D41"].Value = budgetCeilingcomputation.Amount;
                            worksheet.Cells["D41"].Style.Numberformat.Format = "#,##0";
                        }
                    }
                }
                //Kerra Admin
                resp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("28", FinancialYearID)
                    .ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            var budgetCeilingcomputation = (BudgetCeilingComputation)result.Value;
                            worksheet.Cells["B42"].Value = budgetCeilingcomputation.Name;
                            worksheet.Cells["D42"].Value = budgetCeilingcomputation.Amount;
                            worksheet.Cells["D42"].Style.Numberformat.Format = "#,##0";
                        }
                    }
                }

                //Kenha Admin
                resp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("18", FinancialYearID)
                    .ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            var budgetCeilingcomputation = (BudgetCeilingComputation)result.Value;
                            worksheet.Cells["B43"].Value = budgetCeilingcomputation.Name;
                            worksheet.Cells["D43"].Value = budgetCeilingcomputation.Amount;
                            worksheet.Cells["D43"].Style.Numberformat.Format = "#,##0";
                        }
                    }
                }

                worksheet.Cells["B40:D43"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B40:D43"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B40:D43"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B40:D43"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                colFromHex = System.Drawing.ColorTranslator.FromHtml("#FFFF00");
                worksheet.Cells["B41:D43"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["B41:D43"].Style.Fill.BackgroundColor.SetColor(colFromHex);

                /*
                 * Kerra Computation
                 */
                worksheet.Cells["B46"].Value = "KeRRA Computation";
                worksheet.Cells["B46"].Style.Font.Bold = true;
                //Total allocation to Kerra
                resp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("30", FinancialYearID)
                    .ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            var budgetCeilingcomputation = (BudgetCeilingComputation)result.Value;
                            worksheet.Cells["B47"].Value = budgetCeilingcomputation.Name;
                            worksheet.Cells["C47"].Value = "21.8%";
                            worksheet.Cells["C47"].Style.Font.Bold = true;
                            worksheet.Cells["D47"].Value = budgetCeilingcomputation.Amount;
                            worksheet.Cells["D47"].Style.Numberformat.Format = "#,##0";
                        }
                    }
                }

                // Kerra constituency allocation
                resp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("31", FinancialYearID)
                    .ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            var budgetCeilingcomputation = (BudgetCeilingComputation)result.Value;
                            worksheet.Cells["B48"].Value = budgetCeilingcomputation.Name;
                            worksheet.Cells["D48"].Value = budgetCeilingcomputation.Amount;
                            worksheet.Cells["D48"].Style.Numberformat.Format = "#,##0";
                        }
                    }
                }
                // Kerra critical links allocation
                resp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("32", FinancialYearID)
                    .ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            var budgetCeilingcomputation = (BudgetCeilingComputation)result.Value;
                            worksheet.Cells["B49"].Value = budgetCeilingcomputation.Name;
                            worksheet.Cells["D49"].Value = budgetCeilingcomputation.Amount;
                            worksheet.Cells["D49"].Style.Numberformat.Format = "#,##0";
                        }
                    }
                }
                // Kerra total admin budget
                resp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("33", FinancialYearID)
                    .ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            var budgetCeilingcomputation = (BudgetCeilingComputation)result.Value;
                            worksheet.Cells["B50"].Value = budgetCeilingcomputation.Name;
                            worksheet.Cells["D50"].Value = budgetCeilingcomputation.Amount;
                            worksheet.Cells["D50"].Style.Numberformat.Format = "#,##0";
                        }
                    }
                }
                // Kerra 22% porton admin
                resp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("34", FinancialYearID)
                    .ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            var budgetCeilingcomputation = (BudgetCeilingComputation)result.Value;
                            worksheet.Cells["B51"].Value = budgetCeilingcomputation.Name;
                            worksheet.Cells["D51"].Value = budgetCeilingcomputation.Amount;
                            worksheet.Cells["D51"].Style.Numberformat.Format = "#,##0";
                        }
                    }
                }
                // Kerra 10% porton admin
                resp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("35", FinancialYearID)
                    .ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            var budgetCeilingcomputation = (BudgetCeilingComputation)result.Value;
                            worksheet.Cells["B52"].Value = budgetCeilingcomputation.Name;
                            worksheet.Cells["D52"].Value = budgetCeilingcomputation.Amount;
                            worksheet.Cells["D52"].Style.Numberformat.Format = "#,##0";
                        }
                    }
                }

                // Kerra 22% portion road works
                resp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("36", FinancialYearID)
                    .ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            var budgetCeilingcomputation = (BudgetCeilingComputation)result.Value;
                            worksheet.Cells["B53"].Value = budgetCeilingcomputation.Name;
                            worksheet.Cells["D53"].Value = budgetCeilingcomputation.Amount;
                            worksheet.Cells["D53"].Style.Numberformat.Format = "#,##0";
                        }
                    }
                }
                // Kerra 10% portion road works
                resp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("37", FinancialYearID)
                    .ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            var budgetCeilingcomputation = (BudgetCeilingComputation)result.Value;
                            worksheet.Cells["B54"].Value = budgetCeilingcomputation.Name;
                            worksheet.Cells["D54"].Value = budgetCeilingcomputation.Amount;
                            worksheet.Cells["D54"].Style.Numberformat.Format = "#,##0";
                        }
                    }
                }
                // Kerra total budget for roadworks
                resp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("38", FinancialYearID)
                    .ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            var budgetCeilingcomputation = (BudgetCeilingComputation)result.Value;
                            worksheet.Cells["B55"].Value = budgetCeilingcomputation.Name;
                            worksheet.Cells["D55"].Value = budgetCeilingcomputation.Amount;
                            worksheet.Cells["D55"].Style.Numberformat.Format = "#,##0";
                        }
                    }
                }
                // Kerra 22% allocation/Constituency
                resp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("39", FinancialYearID)
                    .ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            var budgetCeilingcomputation = (BudgetCeilingComputation)result.Value;
                            worksheet.Cells["B56"].Value = budgetCeilingcomputation.Name;
                            worksheet.Cells["D56"].Value = budgetCeilingcomputation.Amount;
                            worksheet.Cells["D56"].Style.Numberformat.Format = "#,##0";
                            colFromHex = System.Drawing.ColorTranslator.FromHtml("#FFFF00");
                            worksheet.Cells["B56"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["B56"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                        }
                    }
                }
                // Kerra 10% allocation/Constituency
                resp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("40", FinancialYearID)
                    .ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            var budgetCeilingcomputation = (BudgetCeilingComputation)result.Value;
                            worksheet.Cells["B57"].Value = budgetCeilingcomputation.Name;
                            worksheet.Cells["D57"].Value = budgetCeilingcomputation.Amount;
                            worksheet.Cells["D57"].Style.Numberformat.Format = "#,##0";
                            colFromHex = System.Drawing.ColorTranslator.FromHtml("#FFFF00");
                            worksheet.Cells["B57"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["B57"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                        }
                    }
                }
                // Kerra Total allocation/Constituency
                resp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("41", FinancialYearID)
                    .ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            var budgetCeilingcomputation = (BudgetCeilingComputation)result.Value;
                            worksheet.Cells["B58"].Value = budgetCeilingcomputation?.Name;
                            worksheet.Cells["D58"].Value = budgetCeilingcomputation?.Amount;
                            worksheet.Cells["D58"].Style.Numberformat.Format = "#,##0";
                            colFromHex = System.Drawing.ColorTranslator.FromHtml("#FF0000");
                            worksheet.Cells["B58"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["B58"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                        }
                    }
                }

                worksheet.Cells["B46:D58"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B46:D58"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B46:D58"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B46:D58"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                /*
                 * Kenha Computation
                 */
                worksheet.Cells["B61"].Value = "KeNHA Computation";
                worksheet.Cells["B61"].Style.Font.Bold = true;

                // 40% RLMLF
                resp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("15", FinancialYearID)
                    .ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            var budgetCeilingcomputation = (BudgetCeilingComputation)result.Value;
                            worksheet.Cells["B62"].Value = budgetCeilingcomputation?.Name;
                            worksheet.Cells["C62"].Value = "40.0%";
                            worksheet.Cells["C62"].Style.Font.Bold = true;
                            worksheet.Cells["D62"].Value = budgetCeilingcomputation?.Amount;
                            worksheet.Cells["D62"].Style.Numberformat.Format = "#,##0";
                        }
                    }
                }
                
                // KENHA Transit Tolls
                resp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("16", FinancialYearID)
                    .ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            var budgetCeilingcomputation = (BudgetCeilingComputation)result.Value;
                            worksheet.Cells["B63"].Value = budgetCeilingcomputation?.Name;
                            worksheet.Cells["D63"].Value = budgetCeilingcomputation?.Amount;
                            worksheet.Cells["D63"].Style.Numberformat.Format = "#,##0";
                        }
                    }
                }
                
                // KENHA Total allocation to Kenha
                resp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("17", FinancialYearID)
                    .ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            var budgetCeilingcomputation = (BudgetCeilingComputation)result.Value;
                            worksheet.Cells["B64"].Value = budgetCeilingcomputation?.Name;
                            worksheet.Cells["D64"].Value = budgetCeilingcomputation?.Amount;
                            worksheet.Cells["D64"].Style.Numberformat.Format = "#,##0";
                            worksheet.Cells["B64:D64"].Style.Font.Bold = true;
                        }
                    }
                }

                // KENHA admin operations
                resp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("18", FinancialYearID)
                    .ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            var budgetCeilingcomputation = (BudgetCeilingComputation)result.Value;
                            worksheet.Cells["B65"].Value = budgetCeilingcomputation?.Name;
                            worksheet.Cells["D65"].Value = budgetCeilingcomputation?.Amount;
                            worksheet.Cells["D65"].Style.Numberformat.Format = "#,##0";
                            //worksheet.Cells["B64:D64"].Style.Font.Bold = true;
                        }
                    }
                }
                
                // KENHA road works
                resp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("19", FinancialYearID)
                    .ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            var budgetCeilingcomputation = (BudgetCeilingComputation)result.Value;
                            worksheet.Cells["B66"].Value = budgetCeilingcomputation?.Name;
                            worksheet.Cells["D66"].Value = budgetCeilingcomputation?.Amount;
                            worksheet.Cells["D66"].Style.Numberformat.Format = "#,##0";
                            //worksheet.Cells["B64:D64"].Style.Font.Bold = true;
                        }
                    }
                }
                worksheet.Cells["B61:D66"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B61:D66"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B61:D66"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B61:D66"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                /*
                 * KURA Computation
                 */
                worksheet.Cells["B69"].Value = " KURA Computation ";
                worksheet.Cells["B69"].Style.Font.Bold = true;
                // KURA 10.2% RMLF
                resp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("22", FinancialYearID)
                    .ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            var budgetCeilingcomputation = (BudgetCeilingComputation)result.Value;
                            worksheet.Cells["B70"].Value = budgetCeilingcomputation?.Name;
                            worksheet.Cells["C70"].Value = "10.2%";
                            worksheet.Cells["C70"].Style.Font.Bold = true;
                            worksheet.Cells["D70"].Value = budgetCeilingcomputation?.Amount;
                            worksheet.Cells["D70"].Style.Numberformat.Format = "#,##0";
                            
                        }
                    }
                }
                // KURA Admin and Operations
                resp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("23", FinancialYearID)
                    .ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            var budgetCeilingcomputation = (BudgetCeilingComputation)result.Value;
                            worksheet.Cells["B71"].Value = budgetCeilingcomputation?.Name;
                            worksheet.Cells["D71"].Value = budgetCeilingcomputation?.Amount;
                            worksheet.Cells["D71"].Style.Numberformat.Format = "#,##0";
                            //worksheet.Cells["B64:D64"].Style.Font.Bold = true;
                        }
                    }
                }
                // KURA Road Works
                resp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("24", FinancialYearID)
                    .ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            var budgetCeilingcomputation = (BudgetCeilingComputation)result.Value;
                            worksheet.Cells["B72"].Value = budgetCeilingcomputation?.Name;
                            worksheet.Cells["D72"].Value = budgetCeilingcomputation?.Amount;
                            worksheet.Cells["D72"].Style.Numberformat.Format = "#,##0";
                            //worksheet.Cells["B64:D64"].Style.Font.Bold = true;
                        }
                    }
                }
                worksheet.Cells["B69:D72"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B69:D72"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B69:D72"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B69:D72"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                //Autofit columns
                //Make all text fit the cells
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                worksheet.Column(1).Width = 3;

                _cache.Set(handle, excelPackage.GetAsByteArray(),
                    new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));

                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Href = Url.Action("Download2", "RevenueCollection", new { fileGuid = handle, FileName = "Ceilings_Estimate_Format_Report.xlsx" })
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

        #region CS Allocation

        [Authorize(Claims.Permission.CSAllocation.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> CSAllocationActivitiesIndex(long? FinancialYearId)
        {
            try
            {
                CSAllocationViewModel cSAllocationViewModel = new CSAllocationViewModel();

                //Get current financial year

                var respFinancialYear = await _financialYearService.FindCurrentYear().ConfigureAwait(false);

                if (FinancialYearId != null)
                {
                    long _FinancialYearId;
                    bool result = long.TryParse(FinancialYearId.ToString(), out _FinancialYearId);

                    respFinancialYear = await _financialYearService.FindByIdAsync(_FinancialYearId).ConfigureAwait(false);
                }

                if (respFinancialYear.Success)
                {
                    //Set financial Year
                    cSAllocationViewModel.FinancialYear = respFinancialYear.FinancialYear;

                    var resp = await _cSAllocationService.ListAsync(respFinancialYear.FinancialYear.ID).ConfigureAwait(false);
                    cSAllocationViewModel.CSAllocations = resp.CSAllocation;

                }
                else
                {
                    var resp = await _cSAllocationService.ListAsync().ConfigureAwait(false);
                    cSAllocationViewModel.CSAllocations = resp.CSAllocation;

                }

                cSAllocationViewModel.FinancialYear = respFinancialYear.FinancialYear;

                ViewData["_FinancialYearId"] = cSAllocationViewModel.FinancialYear.ID;

                //financial year drop down
                //await IndexDropDowns(respFinancialYear.FinancialYear.ID).ConfigureAwait(false);

                cSAllocationViewModel.Referer = Request.GetEncodedUrl();

                return PartialView("CSAllocationIndexPartialView", cSAllocationViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CSAllocationController Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "CSAllocationController Index Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.CSAllocation.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> CSAllocationActivities(long? FinancialYearId)
        {
            try
            {
                CSAllocationViewModel cSAllocationViewModel = new CSAllocationViewModel();

                //Get current financial year

                var respFinancialYear = await _financialYearService.FindCurrentYear().ConfigureAwait(false);

                if (FinancialYearId != null)
                {
                    long _FinancialYearId;
                    bool result = long.TryParse(FinancialYearId.ToString(), out _FinancialYearId);

                    respFinancialYear = await _financialYearService.FindByIdAsync(_FinancialYearId).ConfigureAwait(false);
                }

                if (respFinancialYear.Success)
                {
                    //Set financial Year
                    cSAllocationViewModel.FinancialYear = respFinancialYear.FinancialYear;

                    var resp = await _cSAllocationService.ListAsync(respFinancialYear.FinancialYear.ID).ConfigureAwait(false);
                    cSAllocationViewModel.CSAllocations = resp.CSAllocation;

                }
                else
                {
                    var resp = await _cSAllocationService.ListAsync().ConfigureAwait(false);
                    cSAllocationViewModel.CSAllocations = resp.CSAllocation;

                }

                //financial year drop down
                await IndexDropDowns(respFinancialYear.FinancialYear.ID).ConfigureAwait(false);

                cSAllocationViewModel.Referer = Request.GetEncodedUrl();

                return PartialView("CSAllocationPartialView", cSAllocationViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CSAllocationController Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "CSAllocationController Index Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.CSAllocation.Add), Authorize(Claims.Permission.CSAllocation.Change)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddEdit(int? id)
        {
            try
            {
                //DisbursementViewModel disbursementViewModel = new DisbursementViewModel();
                if (id == null)
                {
                    return NotFound();
                }
                int ID;
                bool result = int.TryParse(id.ToString(), out ID);
                if (ID == 0)
                {
                    CSAllocation cSAllocation = new CSAllocation();
                    await PopulateDropDowns(0, 0, 0).ConfigureAwait(false);
                    return View(cSAllocation);
                }
                else
                {
                    var resp = await _cSAllocationService.FindByIdAsync(ID).ConfigureAwait(false);
                    var cSAllocation = resp.CSAllocation;
                    if (cSAllocation == null)
                    {
                        return NotFound();
                    }
                    await PopulateDropDowns(cSAllocation.AuthorityId, cSAllocation.BudgetCeilingComputation.FinancialYearId,
                         cSAllocation.BudgetCeilingComputationId).ConfigureAwait(false);
                    return View(cSAllocation);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CSAllocationController.AddEdit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "CSAllocationController.AddEdit Page has reloaded");
                return View();
            }
        }


        // POST: Revenue Collection/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.CSAllocation.Add), Authorize(Claims.Permission.CSAllocation.Change)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddEdit(long id, [Bind("ID,FinancialYearId," +
            "Amount,AuthorityId,BudgetCeilingComputationId")] CSAllocation cSAllocation)
        {
            try
            {
                bool success = false;
                string msg = null;
                if (id != cSAllocation.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    ApplicationUser loggedInuser = await GetLoggedInUser().ConfigureAwait(false);
                    //if id=0 then create new
                    if (id == 0)
                    {
                        var resp = await _cSAllocationService.FindByCSAllocationEntryAsync(cSAllocation).ConfigureAwait(false);
                        if (resp.CSAllocation != null)
                        {
                            string msgErr = $"A similar CS allocation entry exists" +
                            $" No duplicate entries may exists for the same CS Allocation tranche for the same authority" +
                            $" in the same financial year";
                            ModelState.AddModelError(string.Empty, msgErr);
                            //Detach the first entry before attaching your updated entry
                            var respDetach = await _cSAllocationService.DetachFirstEntryAsync(resp.CSAllocation).ConfigureAwait(false);
                            return Json(new
                            {
                                Success = false,
                                Message = msgErr,
                                Href = Url.Action("AddEdit", "RevenueCollection", new { id = string.Empty })
                            });
                        }

                        /*add cs allocation*/
                        //get budgetceilingcomputation id for selected financial year
                        var cSallocationtResp = await _cSAllocationService.AddAsync(cSAllocation).ConfigureAwait(false);
                        if (cSallocationtResp.Success)
                        {
                            success = true;
                            msg = "CSAllocation Successfully Added";
                        }
                    }
                    //if id>0 then its an edit
                    if (id > 0)
                    {
                        var cSallocationtResp = await _cSAllocationService.Update(id, cSAllocation).ConfigureAwait(false);
                        if (cSallocationtResp.Success)
                        {
                            success = true;
                            msg = "CSAllocation Successfully Updated";
                        }
                    }

                    return Json(new
                    {
                        Success = success,
                        Message = msg,
                        Href = Url.Action("Index", "RevenueCollection")
                    }); ;
                }
                return Json(new
                {
                    Success = false,
                    Message = "Model Error",
                    Href = Url.Action("Index", "RevenueCollection")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CSAllocationController AddEdit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "CSAllocationController AddEdit Page has reloaded");
                await PopulateDropDowns(cSAllocation.AuthorityId, cSAllocation.BudgetCeilingComputation.FinancialYearId,
                    cSAllocation.BudgetCeilingComputationId).ConfigureAwait(false);
                return View(cSAllocation);
            }
        }

        // GET: CSAllocationController/Delete/5
        [Authorize(Claims.Permission.CSAllocation.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Delete(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                int ID;
                bool result = int.TryParse(id.ToString(), out ID);
                if (ID == 0)
                {
                    CSAllocation cSAllocation = new CSAllocation();
                    return View(cSAllocation);
                }
                else
                {
                    var resp = await _cSAllocationService.FindByIdAsync(ID).ConfigureAwait(false);
                    var cSAllocation = resp.CSAllocation;
                    if (cSAllocation == null)
                    {
                        return NotFound();
                    }
                    return View(cSAllocation);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CSAllocationController.Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "CSAllocationController.Delete Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.CSAllocation.Delete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Delete(long id, [Bind("ID")] CSAllocation cSAllocation)
        {
            try
            {
                bool success = false;
                string msg = "Delete Failed";
                if (id != cSAllocation.ID)
                {
                    return NotFound();
                }
                var resp = await _cSAllocationService.RemoveAsync(id).ConfigureAwait(false);
                if (resp.Success)
                {
                    success = true;
                    msg = "CSAllocation Deleted";
                }
                return Json(new
                {
                    Success = success,
                    Message = msg,
                    Href = Url.Action("Index", "CSAllocation")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CSAllocationController.Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "CSAllocationController.Delete Page has reloaded");
                return View(cSAllocation);
            }
        }

        private async Task PopulateDropDowns(long AuthorityId, long FinancialYearId, long BudgetCeilingComputationId)
        {
            long _FinancialYearId = 0; ;
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
                //Try to get current financial year
                var currentFinancialResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
                if (currentFinancialResp.Success)
                {
                    _FinancialYearId = currentFinancialResp.FinancialYear.ID;
                    ViewData["FinancialYearId"] = new SelectList(newFinancialYearsList, "ID", "Code", _FinancialYearId);
                }
                else
                {
                    ViewData["FinancialYearId"] = new SelectList(newFinancialYearsList, "ID", "Code", _FinancialYearId);
                }

            }
            else
            {
                _FinancialYearId = FinancialYearId;
                ViewData["FinancialYearId"] = new SelectList(newFinancialYearsList, "ID", "Code", _FinancialYearId);
            }

            ViewData["_FinancialYearId"] = _FinancialYearId;

            //Authority "RA" 
            var authorityResp = await _authorityService.ListAsync("RA").ConfigureAwait(false);
            IList<Authority> authoritylist = (IList<Authority>)authorityResp;
            if (AuthorityId == 0)
            {
                ViewData["AuthorityId"] = new SelectList(authoritylist, "ID", "Name");
            }
            else
            {
                ViewData["AuthorityId"] = new SelectList(authoritylist, "ID", "Name", AuthorityId);
            }

            //BudgetCeiling
            BudgetCeilingComputation budgetCeilingComputation = null;
            var budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("20", _FinancialYearId).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                    }
                }
            }

            if (budgetCeilingComputation == null)
            {
                ViewData["_BudgetCeilingComputationId"] = 0;
            }
            else
            {
                ViewData["_BudgetCeilingComputationId"] = budgetCeilingComputation.ID;
            }

            //Get financial years for the budget ceiling code 
            IList<BudgetCeilingComputation> budgetCeilingComputationList = null;
            budgetCeilingComputationResp = await _budgetCeilingComputationService.ListAsync("20").ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputationList = (IList<BudgetCeilingComputation>)result.Value;
                    }
                }
            }
            var newFakeFinancialYearsList = budgetCeilingComputationList
                .OrderByDescending(v => v.FinancialYear.Code)
                .Select(
                p => new
                {
                    ID = p.ID,
                    Code = $"{p.FinancialYear.Code}-{p.FinancialYear.Revision}"
                }
                    ).ToList();
            if (BudgetCeilingComputationId == 0)
            {
                ViewData["FinancialYearId2"] = new SelectList(newFakeFinancialYearsList, "ID", "Code");
            }
            else
            {
                ViewData["FinancialYearId2"] = new SelectList(newFakeFinancialYearsList, "ID", "Code", BudgetCeilingComputationId);
            }


        }

        #endregion

        #region Budget Vote
        [Authorize(Claims.Permission.BudgetCeilingComputation.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> BudgetVoteListIndex(long? FinancialYearId)
        {
            try
            {
                BudgetVoteViewModel budgetVoteViewModel = new BudgetVoteViewModel();

                //Get current financial year

                var respFinancialYear = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
                if (respFinancialYear.Success)
                {
                    if (FinancialYearId != null)
                    {
                        long _FinancialYearId;
                        bool result = long.TryParse(FinancialYearId.ToString(), out _FinancialYearId);

                        respFinancialYear = await _financialYearService.FindByIdAsync(_FinancialYearId).ConfigureAwait(false);
                        budgetVoteViewModel.FinancialYear = respFinancialYear.FinancialYear;

                        //financial year drop down
                        await IndexDropDowns(respFinancialYear.FinancialYear.ID).ConfigureAwait(false);
                    }
                    else
                    {
                        budgetVoteViewModel.FinancialYear = respFinancialYear.FinancialYear;

                        //financial year drop down
                        await IndexDropDowns(0).ConfigureAwait(false);
                    }
                }
                return View(budgetVoteViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionController Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "RevenueCollectionController BudgetVoteListIndex Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.BudgetCeilingComputation.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> BudgetVoteListActivities(long FinancialYearId)
        {
            try
            {
                BudgetVoteViewModel budgetVoteViewModel = new BudgetVoteViewModel();

                var respBudgetVoteList = await _budgetCeilingComputationService.ListAsync(FinancialYearId).ConfigureAwait(false);
                if (respBudgetVoteList.Success)
                {
                    var objectResult = (ObjectResult)respBudgetVoteList.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            budgetVoteViewModel.BudgetCeilingComputations = (IEnumerable<BudgetCeilingComputation>)result.Value;
                        }
                    }
                }

                return PartialView("BudgetVoteListPartialView", budgetVoteViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionController Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "RevenueCollectionController BudgetVoteListIndex Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.BudgetCeilingComputation.Add), Authorize(Claims.Permission.BudgetCeilingComputation.Change)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> BudgetVoteListAddEdit(int? id, long FinancialYearId)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                int ID;
                bool result = int.TryParse(id.ToString(), out ID);
                if (ID == 0)
                {
                    BudgetCeilingComputation budgetCeilingComputation = new BudgetCeilingComputation();
                    budgetCeilingComputation.FinancialYearId = FinancialYearId;
                    await PopulateDropDownsForBudgetVoteList(0, 0).ConfigureAwait(false);
                    return View(budgetCeilingComputation);
                }
                else
                {
                    BudgetCeilingComputation budgetCeilingComputation = null;
                    var resp = await _budgetCeilingComputationService.FindByIdAsync(ID).ConfigureAwait(false);
                    if (resp.Success)
                    {
                        var objectResult = (ObjectResult)resp.IActionResult;
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
                        return NotFound();
                    }
                    long _authorityId = 0;
                    bool result4 = long.TryParse(budgetCeilingComputation.AuthorityId.ToString(), out _authorityId);
                    await PopulateDropDownsForBudgetVoteList(_authorityId, budgetCeilingComputation.FinancialYearId).ConfigureAwait(false);
                    return View(budgetCeilingComputation);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionController.AddEdit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "RevenueCollectionController.AddEdit Page has reloaded");
                return View();
            }
        }

        // POST: BudgetCeilingComputation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.BudgetCeilingComputation.Add), Authorize(Claims.Permission.BudgetCeilingComputation.Change)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> BudgetVoteListAddEdit(long id, [Bind("ID,Code,Name," +
            "Amount,AuthorityId,FinancialYearId")] BudgetCeilingComputation budgetCeilingComputation)
        {
            try
            {
                bool success = false;
                string msg = null;
                if (id != budgetCeilingComputation.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    ApplicationUser loggedInuser = await GetLoggedInUser().ConfigureAwait(false);
                    //if id=0 then create new
                    if (id == 0)
                    {
                        var resp = await _budgetCeilingComputationService.FindByBudgetVoteEntryAsync(budgetCeilingComputation).ConfigureAwait(false);
                        if (!resp.Success)
                        {
                            BudgetCeilingComputation budgetCeilingComputationExisting = null;
                            var objectResult = (ObjectResult)resp.IActionResult;
                            if (objectResult != null)
                            {
                                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    var result = (OkObjectResult)objectResult;
                                    budgetCeilingComputationExisting = (BudgetCeilingComputation)result.Value;
                                }
                            }

                            string msgErr = $"A similar budget vote entry exists" +
                            $" No duplicate entries may exists for the same budget vote entry" +
                            $" Enusre that Code{(budgetCeilingComputationExisting != null ? budgetCeilingComputationExisting.Code : null)} or " +
                            $"Name : {(budgetCeilingComputationExisting != null ? budgetCeilingComputationExisting.Name : null)} doesn't exist already";
                            ModelState.AddModelError(string.Empty, msgErr);
                            //Detach the first entry before attaching your updated entry
                            var respDetach = await _budgetCeilingComputationService.DetachFirstEntryAsync(budgetCeilingComputationExisting).ConfigureAwait(false);
                            //return RedirectToAction("AddEdit",new { id=disbursement.ID});
                            return Json(new
                            {
                                Success = false,
                                Message = msgErr,
                                Href = Url.Action("BudgetVoteListAddEdit", "RevenueCollection", new { id = string.Empty })
                            });
                        }

                        //add budgetVote
                        var disbursementResp = await _budgetCeilingComputationService.AddAsync(budgetCeilingComputation).ConfigureAwait(false);
                        if (disbursementResp.Success)
                        {
                            success = true;
                            msg = "Budget Vote Added Successfully";
                        }
                    }
                    //if id>0 then its an edit
                    if (id > 0)
                    {
                        var budgetVoteResp = await _budgetCeilingComputationService.Update(id, budgetCeilingComputation).ConfigureAwait(false);
                        if (budgetVoteResp.Success)
                        {
                            //Call function to update county quarters 
                            if (budgetCeilingComputation.Code=="14")
                            {
                                //get financial year for this budget vote

                                //Get budget ceiling heder for this fincial year

                                //Get CG for budgetceiling header id


                            }
                            success = true;
                            msg = "Budget Vote Successfully Updated";
                        }
                    }

                    return Json(new
                    {
                        Success = success,
                        Message = msg,
                        Href = Url.Action("BudgetVoteListIndex", "RevenueCollection")
                    }); ;
                }
                return Json(new
                {
                    Success = false,
                    Message = "Model Error",
                    Href = Url.Action("BudgetVoteListIndex", "RevenueCollection")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionController AddEdit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "RevenueCollectionController AddEdit Page has reloaded");
                long _authorityId = 0;
                bool result4 = long.TryParse(budgetCeilingComputation.AuthorityId.ToString(), out _authorityId);
                await PopulateDropDownsForBudgetVoteList(_authorityId, budgetCeilingComputation.FinancialYearId).ConfigureAwait(false);
                return View(budgetCeilingComputation);
            }
        }

        private async Task PopulateDropDownsForBudgetVoteList(long AuthorityId, long FinancialYearId)
        {
            //Authority "RA" 
            var authorityResp = await _authorityService.ListAsync().ConfigureAwait(false);
            IList<Authority> authoritylist = (IList<Authority>)authorityResp;
            if (AuthorityId == 0)
            {
                ViewData["AuthorityId"] = new SelectList(authoritylist, "ID", "Name");
            }
            else
            {
                ViewData["AuthorityId"] = new SelectList(authoritylist, "ID", "Name", AuthorityId);
            }

            long _FinancialYearId = 0; ;
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
                //Try to get current financial year
                var currentFinancialResp = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
                if (currentFinancialResp.Success)
                {
                    _FinancialYearId = currentFinancialResp.FinancialYear.ID;
                    ViewData["FinancialYearId"] = new SelectList(newFinancialYearsList, "ID", "Code", _FinancialYearId);
                }
                else
                {
                    ViewData["FinancialYearId"] = new SelectList(newFinancialYearsList, "ID", "Code", _FinancialYearId);
                }

            }
            else
            {
                _FinancialYearId = FinancialYearId;
                ViewData["FinancialYearId"] = new SelectList(newFinancialYearsList, "ID", "Code", _FinancialYearId);
            }

            ViewData["_FinancialYearId"] = _FinancialYearId;
        }

        // GET: RevenueCollectionController/Details/5
        [Authorize(Claims.Permission.BudgetCeilingComputation.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> BudgetVoteListDetails(long Id)
        {
            try
            {
                BudgetVoteViewModel budgetVoteViewModel = new BudgetVoteViewModel();
                var resp = await _budgetCeilingComputationService.FindByIdAsync(Id).ConfigureAwait(false);
                if (!resp.Success)
                {
                    return NotFound();
                }
                var objectResult = (ObjectResult)resp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetVoteViewModel.BudgetCeilingComputation = (BudgetCeilingComputation)result.Value;
                    }
                }

                budgetVoteViewModel.Referer = Request.Headers["Referer"].ToString();

                return View(budgetVoteViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionController.BudgetVoteListDetails Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "RevenueCollectionController.BudgetVoteListDetails Page has reloaded");
                return View();
            }
        }

        // GET: DisbursementController/Details/5
        [Authorize(Claims.Permission.BudgetCeilingComputation.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> BudgetVoteListDelete(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                int ID;
                bool result = int.TryParse(id.ToString(), out ID);
                if (ID == 0)
                {
                    BudgetVoteViewModel budgetVoteViewModel = new BudgetVoteViewModel();
                    return View(budgetVoteViewModel);
                }
                else
                {
                    BudgetVoteViewModel budgetVoteViewModel = new BudgetVoteViewModel();
                    var resp = await _budgetCeilingComputationService.FindByIdAsync(ID).ConfigureAwait(false);
                    if (!resp.Success)
                    {
                        return NotFound();
                    }
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result2 = (OkObjectResult)objectResult;
                            budgetVoteViewModel.BudgetCeilingComputation = (BudgetCeilingComputation)result2.Value;
                        }
                    }
                    return View(budgetVoteViewModel.BudgetCeilingComputation);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionController.Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "RevenueCollectionController.Delete Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.BudgetCeilingComputation.Delete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> BudgetVoteListDelete(long id, [Bind("ID")] BudgetCeilingComputation budgetCeilingComputation)
        {
            try
            {
                bool success = false;
                string msg = "Delete Failed";
                if (id != budgetCeilingComputation.ID)
                {
                    return NotFound();
                }
                var resp = await _budgetCeilingComputationService.RemoveAsync(id).ConfigureAwait(false);
                if (resp.Success)
                {
                    success = true;
                    msg = "Budget Ceiling Vote Deleted";
                }
                return Json(new
                {
                    Success = success,
                    Message = msg,
                    Href = Url.Action("BudgetVoteListIndex", "RevenueCollection")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionController.Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "RevenueCollectionController.Delete Page has reloaded");
                return View(budgetCeilingComputation);
            }
        }

        #endregion
    }
}