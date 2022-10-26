using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using APRP.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Globalization;

namespace APRP.Web.Controllers
{
    [Authorize]
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutAttribute))]
    public class DisbursementController : Controller
    {
        private readonly ILogger _logger;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly IDisbursementService _disbursementService;
        private readonly IFinancialYearService _financialYearService;
        private readonly IDisbursementTrancheService _disbursementTrancheService;
        private readonly IAllocationCodeUnitService _allocationCodeUnitService;
        private readonly IBudgetCeilingService _budgetCeilingService;
        private readonly IMemoryCache _cache;
        private readonly IBudgetCeilingComputationService _budgetCeilingComputationService;
        private readonly IDisbursementCodeListService _disbursementCodeListService;

        #region Private Variables
        private Dictionary<long, double> dictTranches = new Dictionary<long, double>();
        private double TotalAnnualCeiling = 0d;
        private double TotalDisbursement = 0d;
        private double TotalPercent = 0d;
        #endregion

        public DisbursementController(ILogger<DisbursementController> logger,
            IApplicationUsersService applicationUsersService, IAuthorityService authorityService,
            IDisbursementService disbursementService, IFinancialYearService financialYearService,
            IDisbursementTrancheService disbursementTrancheService, IAllocationCodeUnitService allocationCodeUnitService,
            IBudgetCeilingService budgetCeilingService, IMemoryCache cache, IBudgetCeilingComputationService budgetCeilingComputationService,
            IDisbursementCodeListService disbursementCodeListService)
        {
            _logger = logger;
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _disbursementService = disbursementService;
            _financialYearService = financialYearService;
            _disbursementTrancheService = disbursementTrancheService;
            _allocationCodeUnitService = allocationCodeUnitService;
            _budgetCeilingService = budgetCeilingService;
            _cache = cache;
            _budgetCeilingComputationService = budgetCeilingComputationService;
            _disbursementCodeListService = disbursementCodeListService;

        }

        #region Views Section
        // GET: DisbursementController
        [Authorize(Claims.Permission.Disbursement.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Index(long? FinancialYearId)
        {
            try
            {
                DisbursementViewModel disbursementViewModel = new DisbursementViewModel();

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
                    disbursementViewModel.FinancialYear = respFinancialYear.FinancialYear;

                    var resp = await _disbursementService.ListAsync(respFinancialYear.FinancialYear.ID).ConfigureAwait(false);
                    disbursementViewModel.Disbursements = resp.Disbursement;

                    //Get the summaries
                    IList<DisbursementSummaryViewModel> disbursementSummaryViewModel = null;
                    var respDisbursementSummaries = await _disbursementService.DisbursementSummaryAsync(respFinancialYear.FinancialYear.ID).ConfigureAwait(false);
                    if (respDisbursementSummaries.Success)
                    {
                        var objectResult = (ObjectResult)respDisbursementSummaries.IActionResult;
                        if (objectResult != null)
                        {
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var result = (OkObjectResult)objectResult;
                                var anon = result.Value;
                                disbursementViewModel.DisbursementSummaryViewModels = await ReadAnonymousType(anon).ConfigureAwait(false);
                            }
                        }
                    }
                }
                else
                {
                    var resp = await _disbursementService.ListAsync().ConfigureAwait(false);
                    disbursementViewModel.Disbursements = resp.Disbursement;

                }

                //financial year drop down
                await IndexDropDowns(respFinancialYear.FinancialYear.ID).ConfigureAwait(false);

                disbursementViewModel.Referer = Request.GetEncodedUrl();

                if (!disbursementViewModel.DisbursementSummaryViewModels.Any())
                {
                    disbursementViewModel.IsDisbursementSummaryViewModels = true;
                }
                else
                {
                    disbursementViewModel.IsDisbursementSummaryViewModels = false;
                }
                return View(disbursementViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementController Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "DisbursementController Index Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.Disbursement.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DisbursementActivities(long? FinancialYearId)
        {
            try
            {
                DisbursementViewModel disbursementViewModel = new DisbursementViewModel();

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
                    disbursementViewModel.FinancialYear = respFinancialYear.FinancialYear;

                    var resp = await _disbursementService.ListAsync(respFinancialYear.FinancialYear.ID).ConfigureAwait(false);
                    disbursementViewModel.Disbursements = resp.Disbursement;

                    //Get the summaries
                    IList<DisbursementSummaryViewModel> disbursementSummaryViewModel = null;
                    var respDisbursementSummaries = await _disbursementService.DisbursementSummaryAsync(respFinancialYear.FinancialYear.ID).ConfigureAwait(false);
                    if (respDisbursementSummaries.Success)
                    {
                        var objectResult = (ObjectResult)respDisbursementSummaries.IActionResult;
                        if (objectResult != null)
                        {
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var result = (OkObjectResult)objectResult;
                                var anon = result.Value;
                                disbursementViewModel.DisbursementSummaryViewModels = await ReadAnonymousType(anon).ConfigureAwait(false);
                            }
                        }
                    }
                }
                else
                {
                    var resp = await _disbursementService.ListAsync().ConfigureAwait(false);
                    disbursementViewModel.Disbursements = resp.Disbursement;

                }

                //financial year drop down
                await IndexDropDowns(respFinancialYear.FinancialYear.ID).ConfigureAwait(false);

                disbursementViewModel.Referer = Request.GetEncodedUrl();

                if (!disbursementViewModel.DisbursementSummaryViewModels.Any())
                {
                    disbursementViewModel.IsDisbursementSummaryViewModels = true;
                }
                else
                {
                    disbursementViewModel.IsDisbursementSummaryViewModels = false;
                }
                return PartialView("DisbursementPartialView", disbursementViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementController Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "DisbursementController Index Page has reloaded");
                return View();
            }
        }
        private async Task<IList<DisbursementSummaryViewModel>> ReadAnonymousType(dynamic anonymousobject)
        {
            IEnumerable<DisbursementSummaryViewModel> disbursementSummaryViewModels1 = new List<DisbursementSummaryViewModel>();
            IList<DisbursementSummaryViewModel> disbursementSummaryViewModels = (IList<DisbursementSummaryViewModel>)disbursementSummaryViewModels1;

            //Iterate through a list of anonymous objects in C#
            foreach (var obj in anonymousobject)
            {
                DisbursementSummaryViewModel disbursementSummaryViewModel = new DisbursementSummaryViewModel();
                var FinancialYearId = obj.GetType().GetProperty("FinancialYearId").GetValue(obj);
                var AuthorityId = obj.GetType().GetProperty("AuthorityId").GetValue(obj);
                var Count = obj.GetType().GetProperty("Count").GetValue(obj);
                var TotalDisbursement = obj.GetType().GetProperty("TotalDisbursement").GetValue(obj);

                int x;
                bool result = int.TryParse(Count.ToString(), out x);
                disbursementSummaryViewModel.Count = x;

                long y;
                result = long.TryParse(AuthorityId.ToString(), out y);
                disbursementSummaryViewModel.AuthorityId = y;

                result = long.TryParse(FinancialYearId.ToString(), out y);
                disbursementSummaryViewModel.FinancialYearId = y;

                double z;
                result = double.TryParse(TotalDisbursement.ToString(), out z);
                disbursementSummaryViewModel.TotalDisbursement = z;

                //Get Authority
                var authorityResp = await _authorityService.FindByIdAsync(disbursementSummaryViewModel.AuthorityId).ConfigureAwait(false);
                if (authorityResp.Success)
                {
                    disbursementSummaryViewModel.Authority = authorityResp.Authority;
                }

                //Get finanical year
                var finacialResp = await _financialYearService.FindByIdAsync(disbursementSummaryViewModel.FinancialYearId).ConfigureAwait(false);
                if (finacialResp.Success)
                {
                    disbursementSummaryViewModel.FinancialYear = finacialResp.FinancialYear;
                    //Get percent
                    if (disbursementSummaryViewModel.Authority != null)
                    {

                        var respAllocationcodeUnit = await _allocationCodeUnitService.FindByAuthorityAsync(disbursementSummaryViewModel.Authority.ID).ConfigureAwait(false);
                        if (respAllocationcodeUnit.Success)
                        {
                            disbursementSummaryViewModel.Percent = respAllocationcodeUnit.AllocationCodeUnit.Percent;
                        }
                        //Get Annual ceiling for financial year
                        var respBudgetCeiling = await _budgetCeilingService.FindByAuthorityIDAndFinancialYearID
                            (disbursementSummaryViewModel.Authority.ID, disbursementSummaryViewModel.FinancialYear.ID)
                            .ConfigureAwait(false);
                        if (respBudgetCeiling.Success)
                        {
                            disbursementSummaryViewModel.AnnualCeiling = respBudgetCeiling.BudgetCeiling.Amount;
                            //Compute percent ceiling
                            disbursementSummaryViewModel.PercentOfCeiling = disbursementSummaryViewModel.TotalDisbursement / disbursementSummaryViewModel.AnnualCeiling;
                        }
                    }
                }
                disbursementSummaryViewModels.Add(disbursementSummaryViewModel);
            }
            return disbursementSummaryViewModels;
        }

        private async Task<IList<DisbursementSummaryViewModel>> ReadAnonymousTypeForReport(dynamic anonymousobject, DisbursementViewModel disbursementViewModel)
        {
            IEnumerable<DisbursementSummaryViewModel> disbursementSummaryViewModels1 = new List<DisbursementSummaryViewModel>();
            IList<DisbursementSummaryViewModel> disbursementSummaryViewModels = (IList<DisbursementSummaryViewModel>)disbursementSummaryViewModels1;

            //Get a list of all budgetceiling computation
            IEnumerable<BudgetCeilingComputation> budgetCeilingComputations = null;
            var respBudgetCeilingComputation = await _budgetCeilingComputationService.ListAsync(disbursementViewModel.FinancialYear.ID).ConfigureAwait(false);
            if (respBudgetCeilingComputation.Success)
            {
                var objectResult = (ObjectResult)respBudgetCeilingComputation.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputations = (IEnumerable<BudgetCeilingComputation>)result.Value;
                    }
                }
            }

            //Iterate through a list of anonymous objects in C#
            foreach (var obj in anonymousobject)
            {
                DisbursementSummaryViewModel disbursementSummaryViewModel = new DisbursementSummaryViewModel();
                var BudgetCeilingComputationId = obj.GetType().GetProperty("BudgetCeilingComputationId").GetValue(obj);
                //var AuthorityId = obj.GetType().GetProperty("AuthorityId").GetValue(obj);
                var Count = obj.GetType().GetProperty("Count").GetValue(obj);
                var TotalDisbursement = obj.GetType().GetProperty("TotalDisbursement").GetValue(obj);

                int x;
                bool result = int.TryParse(Count.ToString(), out x);
                disbursementSummaryViewModel.Count = x;

                long budgetCeilingComputationId;
                result = long.TryParse(BudgetCeilingComputationId.ToString(), out budgetCeilingComputationId);
                disbursementSummaryViewModel.AuthorityId = budgetCeilingComputations.Where(x => x.ID == BudgetCeilingComputationId).FirstOrDefault()
                    .AuthorityId;

                long y;
                result = long.TryParse(BudgetCeilingComputationId.ToString(), out y);
                disbursementSummaryViewModel.BudgetCeilingComputationId = y;

                double z;
                result = double.TryParse(TotalDisbursement.ToString(), out z);
                disbursementSummaryViewModel.TotalDisbursement = z;

                //Get Authority
                var authorityResp = await _authorityService.FindByIdAsync(disbursementSummaryViewModel.AuthorityId).ConfigureAwait(false);
                if (authorityResp.Success)
                {
                    disbursementSummaryViewModel.Authority = authorityResp.Authority;
                }

                //get budgetceilingcomputation item
                BudgetCeilingComputation budgetCeilingComputation = null;
                var budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByIdAsync(disbursementSummaryViewModel.BudgetCeilingComputationId).ConfigureAwait(false);
                if (budgetCeilingComputationResp.Success)
                {
                    var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result2 = (OkObjectResult)objectResult;
                            budgetCeilingComputation = (BudgetCeilingComputation)result2.Value;
                        }
                    }
                }
                disbursementSummaryViewModel.BudgetCeilingComputation = budgetCeilingComputation;

                //Get financial year
                var finacialResp = await _financialYearService.FindByIdAsync(disbursementSummaryViewModel.BudgetCeilingComputation.FinancialYearId).ConfigureAwait(false);
                if (finacialResp.Success)
                {
                    disbursementSummaryViewModel.FinancialYear = finacialResp.FinancialYear;

                    //Get percent
                    if (disbursementSummaryViewModel.Authority != null)
                    {

                        var respAllocationcodeUnit = await _allocationCodeUnitService.FindByAuthorityAsync(disbursementSummaryViewModel.Authority.ID).ConfigureAwait(false);
                        if (respAllocationcodeUnit.Success)
                        {
                            disbursementSummaryViewModel.Percent = respAllocationcodeUnit.AllocationCodeUnit.Percent;
                        }

                        //Get Annual ceiling for financial year
                        disbursementSummaryViewModel.AnnualCeiling = disbursementSummaryViewModel.BudgetCeilingComputation.Amount;
                        //Compute percent ceiling
                        disbursementSummaryViewModel.PercentOfCeiling = disbursementSummaryViewModel.TotalDisbursement / disbursementSummaryViewModel.AnnualCeiling;
                    }
                }
                disbursementSummaryViewModels.Add(disbursementSummaryViewModel);
            }

            IList<DisbursementSummaryViewModel> disbursementSummaryViewModelsOrdered = (disbursementSummaryViewModels.OrderBy(o => o.AuthorityId)).ToList();
            return disbursementSummaryViewModelsOrdered;
        }

        [Authorize(Claims.Permission.Disbursement.Add), Authorize(Claims.Permission.Disbursement.Change)]
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
                    Disbursement disbursement = new Disbursement();
                    await PopulateDropDowns(0, 0, 0, 0).ConfigureAwait(false);
                    return View(disbursement);
                }
                else
                {
                    var resp = await _disbursementService.FindByIdAsync(ID).ConfigureAwait(false);
                    var disbursement = resp.Disbursement;
                    if (disbursement == null)
                    {
                        return NotFound();
                    }
                    await PopulateDropDowns(disbursement.AuthorityId, disbursement.FinancialYearId, disbursement.DisbursementTrancheId
                        , disbursement.BudgetCeilingComputationId).ConfigureAwait(false);
                    return View(disbursement);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementController.AddEdit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "DisbursementController.AddEdit Page has reloaded");
                return View();
            }
        }

        // POST: Disbursement/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.Disbursement.Add), Authorize(Claims.Permission.Disbursement.Change)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddEdit(long id, [Bind("ID,FinancialYearId,DisbursementTrancheId," +
            "Amount,AuthorityId,BudgetCeilingComputationId")] Disbursement disbursement)
        {
            try
            {
                bool success = false;
                string msg = null;
                if (id != disbursement.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    ApplicationUser loggedInuser = await GetLoggedInUser().ConfigureAwait(false);
                    //if id=0 then create new
                    if (id == 0)
                    {
                        var resp = await _disbursementService.FindByDisbursementEntryAsync(disbursement).ConfigureAwait(false);
                        if (resp.Disbursement != null)
                        {
                            string msgErr = $"A similar disbursement entry exists" +
                            $" No duplicate entries may exists for the same disbursement tranche for the same authority" +
                            $" in the same financial year";
                            ModelState.AddModelError(string.Empty, msgErr);
                            //Detach the first entry before attaching your updated entry
                            var respDetach = await _disbursementService.DetachFirstEntryAsync(resp.Disbursement).ConfigureAwait(false);
                            //return RedirectToAction("AddEdit",new { id=disbursement.ID});
                            return Json(new
                            {
                                Success = false,
                                Message = msgErr,
                                Href = Url.Action("AddEdit", "Disbursement", new { id = string.Empty })
                            });
                        }

                        //add disbursement
                        var disbursementResp = await _disbursementService.AddAsync(disbursement).ConfigureAwait(false);
                        if (disbursementResp.Success)
                        {
                            success = true;
                            msg = "Disbursement Successfully Added";
                        }
                    }
                    //if id>0 then its an edit
                    if (id > 0)
                    {
                        var disbursementResp = await _disbursementService.Update(id, disbursement).ConfigureAwait(false);
                        if (disbursementResp.Success)
                        {
                            success = true;
                            msg = "Disbursement Successfully Updated";
                        }
                    }

                    return Json(new
                    {
                        Success = success,
                        Message = msg,
                        Href = Url.Action("Index", "Disbursement")
                    }); ;
                }
                return Json(new
                {
                    Success = false,
                    Message = "Model Error",
                    Href = Url.Action("Index", "Disbursement")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementController AddEdit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "DisbursementController AddEdit Page has reloaded");
                await PopulateDropDowns(disbursement.AuthorityId, disbursement.FinancialYearId, disbursement.DisbursementTrancheId
                    , disbursement.BudgetCeilingComputationId).ConfigureAwait(false);
                return View(disbursement);
            }
        }

        // GET: DisbursementController/Details/5
        [Authorize(Claims.Permission.Disbursement.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Details(long AuthorityId, long FinancialYearId)
        {
            try
            {
                DisbursementViewModel disbursementViewModel = new DisbursementViewModel();
                var resp = await _disbursementService.ListAsync(AuthorityId, FinancialYearId).ConfigureAwait(false);
                var disbursement = resp.Disbursement;
                if (disbursement == null)
                {
                    return NotFound();
                }
                disbursementViewModel.Disbursements = disbursement;
                return View(disbursementViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementController.Details Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "DisbursementController.Details Page has reloaded");
                return View();
            }
        }

        // GET: DisbursementController/Details/5
        [Authorize(Claims.Permission.Disbursement.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DetailsIndividual(long? id)
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
                    Disbursement disbursement = new Disbursement();
                    return View(disbursement);
                }
                else
                {
                    var resp = await _disbursementService.FindByIdAsync(ID).ConfigureAwait(false);
                    var disbursement = resp.Disbursement;
                    if (disbursement == null)
                    {
                        return NotFound();
                    }
                    return View(disbursement);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementController.Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "DisbursementController.Delete Page has reloaded");
                return View();
            }
        }

        // GET: DisbursementController/Details/5
        [Authorize(Claims.Permission.Disbursement.View)]
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
                    Disbursement disbursement = new Disbursement();
                    return View(disbursement);
                }
                else
                {
                    var resp = await _disbursementService.FindByIdAsync(ID).ConfigureAwait(false);
                    var disbursement = resp.Disbursement;
                    if (disbursement == null)
                    {
                        return NotFound();
                    }
                    return View(disbursement);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementController.Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "DisbursementController.Delete Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.Disbursement.Delete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Delete(long id, [Bind("ID")] Disbursement disbursement)
        {
            try
            {
                bool success = false;
                string msg = "Delete Failed";
                if (id != disbursement.ID)
                {
                    return NotFound();
                }
                var resp = await _disbursementService.RemoveAsync(id).ConfigureAwait(false);
                if (resp.Success)
                {
                    success = true;
                    msg = "Disbursement Deleted";
                }
                return Json(new
                {
                    Success = success,
                    Message = msg,
                    Href = Url.Action("Index", "Disbursement")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementController.Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "DisbursementController.Delete Page has reloaded");
                return View(disbursement);
            }
        }

        #endregion

        #region Utilies
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

        private async Task PopulateDropDowns(long AuthorityId, long FinancialYearId, long DisbursementTrancheId, long BudgetCeilingComputationId)
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

            ////Disbursement Tranche
            IList<DisbursementTranche> disbursementTranches = null;
            var respDisbursementTranche = await _disbursementTrancheService.ListAsync().ConfigureAwait(false);
            if (respDisbursementTranche.Success)
            {
                var objectResult = (ObjectResult)respDisbursementTranche.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        disbursementTranches = (IList<DisbursementTranche>)result.Value;
                    }
                }
            }
            if (DisbursementTrancheId == 0)
            {
                ViewData["DisbursementTrancheId"] = new SelectList(disbursementTranches, "ID", "Name");
            }
            else
            {
                ViewData["DisbursementTrancheId"] = new SelectList(disbursementTranches, "ID", "Name", DisbursementTrancheId);
            }


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
            IList<BudgetCeilingComputation> budgetCeilingComputations = null;
            var budgetCeilingComputationResp = await _budgetCeilingComputationService.ListAsync(_FinancialYearId).ConfigureAwait(false);
            if (budgetCeilingComputationResp.Success)
            {
                var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        budgetCeilingComputations = (IList<BudgetCeilingComputation>)result.Value;
                    }
                }
            }
            var newbudgetComputationList = budgetCeilingComputations
                .OrderByDescending(v => v.Authority.Code)
                .Select(
                p => new
                {
                    ID = p.ID,
                    Code = $"{p.Name}-({p.Authority.Name})"
                }
                    ).ToList();

            if (BudgetCeilingComputationId == 0)
            {
                ViewData["BudgetCeilingComputationId"] = new SelectList(newbudgetComputationList, "ID", "Code");
            }
            else
            {
                ViewData["BudgetCeilingComputationId"] = new SelectList(newbudgetComputationList, "ID", "Code", BudgetCeilingComputationId);
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

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> GetDisbursementByFinancialYearIdYear(long FinancialYearId)
        {
            try
            {
                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Href = Url.Action("Disbursement", "Index", new { FinancialYearId = FinancialYearId })
                });

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementController.GetDisbursementByFinancialYearIdYear Error {Environment.NewLine}");
                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Href = Url.Action("Disbursement", "Index", new { FinancialYearId = FinancialYearId })
                });
            }
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> GetBudgetCeilingVoteForSelectedFinancialYear(long FinancialYearID)
        {
            try
            {
                //BudgetCeiling
                IList<BudgetCeilingComputation> budgetCeilingComputations = null;
                var budgetCeilingComputationResp = await _budgetCeilingComputationService.ListAsync(FinancialYearID).ConfigureAwait(false);
                if (budgetCeilingComputationResp.Success)
                {
                    var objectResult = (ObjectResult)budgetCeilingComputationResp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            budgetCeilingComputations = (IList<BudgetCeilingComputation>)result.Value;
                        }
                    }
                }
                var newbudgetComputationList = budgetCeilingComputations
                    .OrderByDescending(v => v.Authority.Code)
                    .Select(
                    p => new
                    {
                        ID = p.ID,
                        Code = $"{p.Name}-({p.Authority.Name})"
                    }
                        ).ToList();


                return Json(newbudgetComputationList);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"ARICSController.GetRoadSectionsForSpecificRoad Error{Ex.Message}");
                return Json(null);
            }
        }
        #endregion

        #region Reorts
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> ExportDisbursementReport(long FinancialYearId)
        {
            try
            {
                //Get the data
                DisbursementViewModel disbursementViewModel = new DisbursementViewModel();

                //Get current financial year
                var respFinancialYear = await _financialYearService.FindByIdAsync(FinancialYearId).ConfigureAwait(false);
                if (respFinancialYear.Success)
                {
                    //Set financial Year
                    disbursementViewModel.FinancialYear = respFinancialYear.FinancialYear;

                    var resp = await _disbursementService.ListAsync(respFinancialYear.FinancialYear.ID).ConfigureAwait(false);
                    disbursementViewModel.Disbursements = resp.Disbursement;

                    //Get the summaries ByBudgetCeilingComputation
                    IList<DisbursementSummaryViewModel> disbursementSummaryViewModel = null;
                    var respDisbursementSummaries = await _disbursementService.DisbursementSummaryByBudgetCeilingComputationAsync(respFinancialYear.FinancialYear.ID).ConfigureAwait(false);
                    if (respDisbursementSummaries.Success)
                    {
                        var objectResult = (ObjectResult)respDisbursementSummaries.IActionResult;
                        if (objectResult != null)
                        {
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var result = (OkObjectResult)objectResult;
                                var anon = result.Value;
                                disbursementViewModel.DisbursementSummaryByBudgetCeilingViewModels = await ReadAnonymousTypeForReport(anon, disbursementViewModel).ConfigureAwait(false);
                            }
                        }
                    }


                    //Get the summaries DisbursementSummaryAsync
                    respDisbursementSummaries = await _disbursementService.DisbursementSummaryAsync(respFinancialYear.FinancialYear.ID).ConfigureAwait(false);
                    if (respDisbursementSummaries.Success)
                    {
                        var objectResult = (ObjectResult)respDisbursementSummaries.IActionResult;
                        if (objectResult != null)
                        {
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var result = (OkObjectResult)objectResult;
                                var anon = result.Value;
                                disbursementViewModel.DisbursementSummaryViewModels = await ReadAnonymousType(anon).ConfigureAwait(false);
                            }
                        }
                    }

                }

                if (disbursementViewModel.DisbursementSummaryViewModels == null)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Failed",
                        Href = Url.Action("Index", "Disbursement")
                    });
                }

                //Get Percentages for the counties
                var respallocationCodeUnit = await _allocationCodeUnitService.ListAsync("CG").ConfigureAwait(false);
                if (respallocationCodeUnit.Success)
                {
                    disbursementViewModel.AllocationCodeUnits = respallocationCodeUnit.AllocationCodeUnit;
                }

                //Write to Excel
                MemoryStream stream = new MemoryStream();
                JsonResult myjson = await WriteToDisbursement(disbursementViewModel, stream).ConfigureAwait(false);
                return Json(myjson);

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementController.ExportDisbursementReport Error {Environment.NewLine}");
                return Json(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<JsonResult> WriteToDisbursement(DisbursementViewModel disbursementViewModel, MemoryStream stream)
        {
            //create dictionary for tranches
            dictTranches.Clear();
            TotalAnnualCeiling = 0d;
            TotalDisbursement = 0d;

            //Get disbursements
            IEnumerable<Disbursement> disbursements = null;
            var respDisbursements = await _disbursementService.ListAsync(disbursementViewModel.FinancialYear.ID).ConfigureAwait(false);
            if (respDisbursements.Success)
            {
                disbursements = respDisbursements.Disbursement;
            }

            //Create a new ExcelPackage  
            string handle = Guid.NewGuid().ToString();
            using (ExcelPackage excelPackage = new ExcelPackage(stream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = "KRB";
                excelPackage.Workbook.Properties.Title = "Sno.12 - Disbursement template";
                excelPackage.Workbook.Properties.Subject = "Sno.12 - Disbursement template Report";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("ALL KRBF");

                //Add some text to cell A1
                worksheet.Cells["A1"].Value = $"DISBURSEMENT SCHEDULE FOR FY {disbursementViewModel.FinancialYear.Code}" +
                    $" (AS AT {Ordinal(DateTime.Now.Day)} {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month).ToUpper()}" +
                    $" {DateTime.Now.Year})";
                worksheet.Cells["A1:H1"].Merge = true;
                worksheet.Cells["A1:H1"].Style.Font.Size = 11;
                worksheet.Cells["A1:H1"].Style.Font.Name = "Arial";
                worksheet.Cells["A1:H1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:H1"].Style.Font.Bold = true;
                //worksheet.Cells["A1:H1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //worksheet.Cells["A1:H1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#9CC2E5"));

                //Create the headers/columns

                //=RNo
                worksheet.Cells["A2"].Value = "No";
                worksheet.Cells["A2"].Style.Font.Size = 11;
                worksheet.Cells["A2"].Style.Font.Name = "Arial";
                worksheet.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2"].Style.Font.Bold = true;
                worksheet.Cells["A2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A2"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#9CC2E5"));

                //B1=Road Agency/Recipient
                worksheet.Cells["B2"].Value = "Road Agency/Recipient";
                worksheet.Cells["B2"].Style.Font.Size = 11;
                worksheet.Cells["B2"].Style.Font.Name = "Arial";
                worksheet.Cells["B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B2"].Style.Font.Bold = true;
                worksheet.Cells["B2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["B2"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#9CC2E5"));

                worksheet.Column(2).Width = 24.86;

                //C1=%
                worksheet.Cells["C2"].Value = "%";
                worksheet.Cells["C2"].Style.Font.Size = 11;
                worksheet.Cells["C2"].Style.Font.Name = "Arial";
                worksheet.Cells["C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["C2"].Style.Font.Bold = true;
                worksheet.Cells["C2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["C2"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#9CC2E5"));

                //D1=Annual Ceiling FY 2020/2021 (Kshs)
                worksheet.Cells["D2"].Value = $"Annual Ceiling FY {disbursementViewModel.FinancialYear.Code} (Kshs)";
                worksheet.Cells["D2"].Style.Font.Size = 11;
                worksheet.Cells["D2"].Style.Font.Name = "Arial";
                worksheet.Cells["D2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["D2"].Style.Font.Bold = true;
                worksheet.Cells["D2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["D2"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#9CC2E5"));

                //Tranches
                int columnIndex = 5;
                IList<DisbursementTranche> disbursementTrancheList = null;
                var rspdisbursementTranche = await _disbursementTrancheService.ListAsync().ConfigureAwait(false);
                if (rspdisbursementTranche.Success)
                {
                    var objectResult = (ObjectResult)rspdisbursementTranche.IActionResult;
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result2 = (OkObjectResult)objectResult;
                        disbursementTrancheList = (IList<DisbursementTranche>)result2.Value;
                    }
                }
                if (disbursementTrancheList != null)
                {
                    foreach (var disbursementTrancheList2 in disbursementTrancheList)
                    {
                        worksheet.Cells[2, columnIndex].Value = disbursementTrancheList2.Name;
                        worksheet.Cells[2, columnIndex].Style.Font.Size = 11;
                        worksheet.Cells[2, columnIndex].Style.Font.Name = "Arial";
                        worksheet.Cells[2, columnIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[2, columnIndex].Style.Font.Bold = true;
                        worksheet.Cells[2, columnIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[2, columnIndex].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#9CC2E5"));

                        dictTranches.Add(disbursementTrancheList2.ID, 0d);
                        columnIndex++;
                    }
                }

                //Total disbursement
                worksheet.Cells[2, columnIndex].Value = "Total Disbursement to Date";
                worksheet.Cells[2, columnIndex].Style.Font.Size = 11;
                worksheet.Cells[2, columnIndex].Style.Font.Name = "Arial";
                worksheet.Cells[2, columnIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, columnIndex].Style.Font.Bold = true;
                worksheet.Cells[2, columnIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[2, columnIndex].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#9CC2E5"));

                //%of ceiling
                worksheet.Cells[2, columnIndex + 1].Value = "% of Ceiling";
                worksheet.Cells[2, columnIndex + 1].Style.Font.Size = 11;
                worksheet.Cells[2, columnIndex + 1].Style.Font.Name = "Arial";
                worksheet.Cells[2, columnIndex + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, columnIndex + 1].Style.Font.Bold = true;
                worksheet.Cells[2, columnIndex + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[2, columnIndex + 1].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#9CC2E5"));

                //Compute Authority Sub Totals
                int i = 3;
                int trancheColumnID = 4;
                int newTrancheColumnID = 0;
                bool result = false;
                i = await ComputeAuthoritySubTotals(worksheet, disbursementViewModel, disbursementTrancheList, disbursements, columnIndex, i).ConfigureAwait(false);

                //Road Agency
                worksheet.Cells[i, 2].Value = "Total";

                //Total
                worksheet.Cells[i, 4].Value = string.Format("{0:0,0.00}", TotalAnnualCeiling);



                //loop through the disbursements
                if (disbursementTrancheList != null)
                {
                    foreach (var disbursementTrancheList2 in disbursementTrancheList)
                    {

                        result = int.TryParse((trancheColumnID + disbursementTrancheList2.ID).ToString(), out newTrancheColumnID);
                        try
                        {
                            worksheet.Cells[i, newTrancheColumnID].Value = string.Format("{0:0,0.00}", dictTranches[disbursementTrancheList2.ID]);
                        }
                        catch (Exception Ex)
                        {
                            _logger.LogError(Ex, $"DisbursementController.WriteToDisbursements Error {Environment.NewLine}");
                        }

                    }
                }

                //Total Disbursement
                worksheet.Cells[i, columnIndex].Value = string.Format("{0:0,0.00}", TotalDisbursement);
                worksheet.Cells[i, columnIndex].Style.Font.Size = 11;
                worksheet.Cells[i, columnIndex].Style.Font.Name = "Arial";
                worksheet.Cells[i, columnIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[i, columnIndex].Style.Font.Bold = true;

                //Total % of ceiling                
                //worksheet.Cells[i, columnIndex + 1].Value = string.Format("{0:0.0%}", TotalAnnualCeiling / TotalDisbursement);
                worksheet.Cells[i, columnIndex + 1].Value = (TotalDisbursement / TotalAnnualCeiling).ToString("P", CultureInfo.InvariantCulture);

                // ColumnNames.Add(worksheet.Cells[1, i].Value.ToString()); // 1 = First Row, i = Column Number
                string extremecol = ExcelCellBase.GetAddress(1, columnIndex + 1);
                string column = extremecol.Substring(0, 1);

                worksheet.Cells[$"A2:{column}{i}"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[$"A2:{column}{i}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[$"A2:{column}{i}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[$"A2:{column}{i}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells[$"B{i}:{column}{i}"].Style.Font.Size = 11;
                worksheet.Cells[$"B{i}:{column}{i}"].Style.Font.Name = "Arial";
                worksheet.Cells[$"B{i}:{column}{i}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"B{i}:{column}{i}"].Style.Font.Bold = true;
                worksheet.Cells[$"B{i}:{column}{i}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[$"B{i}:{column}{i}"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#9CC2E5"));

                worksheet.Cells[$"B{i + 1}"].Value = $"N.B. Each single disbursement for all the 47 CGs is an equivalent of the total quarterly allocation amount. ";
                worksheet.Cells[$"B{i + 1}:{column}{i + 1}"].Merge = true;
                worksheet.Cells[$"B{i + 1}:{column}{i + 1}"].Style.Font.Size = 11;
                worksheet.Cells[$"B{i + 1}:{column}{i + 1}"].Style.Font.Name = "Arial";
                worksheet.Cells[$"B{i + 1}:{column}{i + 1}"].Style.Font.Italic = true;
                worksheet.Cells[$"B{i + 1}:{column}{i + 1}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"B{i + 1}:{column}{i + 1}"].Style.Font.Bold = true;

                //Create the WorkSheet for CG Ceiling
                ExcelWorksheet worksheetCGCeiling = excelPackage.Workbook.Worksheets.Add("CGs Ceilings");

                //Populate the WorkSheet for CG Ceiling
                await CGWorkSheet(worksheetCGCeiling, disbursementViewModel).ConfigureAwait(false);

                //Auto fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                _cache.Set(handle, excelPackage.GetAsByteArray(),
                    new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));

                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Href = Url.Action("Download2", "Disbursement", new { fileGuid = handle, FileName = "Sno.12-Disbursement Report.xlsx" })
                });

            }

            //stream.Close();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<int> ComputeAuthoritySubTotals(ExcelWorksheet worksheet, DisbursementViewModel disbursementViewModel, IList<DisbursementTranche> disbursementTrancheList,
            IEnumerable<Disbursement> disbursements, int columnIndex, int i)
        {
            int trancheColumnID = 4;
            int newTrancheColumnID = 0;
            bool result = false;
            double disburmentxAmount = 0d;
            Dictionary<long, double> dictAnnualCeilingAuthorityTotals = new Dictionary<long, double>();

            //Get disbursement code list
            //Get the summaries
            IList<DisbursementCodeList> disbursementCodeLists = null;
            var respDisbursementCodeList = await _disbursementCodeListService.ListAsync().ConfigureAwait(false);
            if (respDisbursementCodeList.Success)
            {
                var objectResult = (ObjectResult)respDisbursementCodeList.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result3 = (OkObjectResult)objectResult;
                        disbursementCodeLists = (IList<DisbursementCodeList>)result3.Value;

                    }
                }
            }

            DisbursementSummaryViewModel disbursementSummaryViewModelFinal = null;
            //Compute Authority Sub Totals
            if (disbursementViewModel.DisbursementSummaryByBudgetCeilingViewModels != null)
            {
                long AuthorityIdPrev = 0; long AuthorityIdCurrent = 0;

                foreach (var rptItem in disbursementViewModel.DisbursementSummaryByBudgetCeilingViewModels)
                {
                    AuthorityIdCurrent = rptItem.AuthorityId;
                    if (AuthorityIdPrev != 0 && AuthorityIdPrev != AuthorityIdCurrent)
                    {
                        //Road Agency/Recepient
                        var authoritysummary = disbursementViewModel.DisbursementSummaryViewModels.Where(x => x.AuthorityId == AuthorityIdPrev)
                            .FirstOrDefault();
                        if (authoritysummary != null)
                        {
                            worksheet.Cells[i, 1].Value = i - 2;//Serial No

                            worksheet.Cells[i, 2].Value = $"Total {authoritysummary.Authority.Code}";

                            double value2;
                            bool dictRty = dictAnnualCeilingAuthorityTotals.TryGetValue(AuthorityIdPrev, out value2);
                            worksheet.Cells[i, 4].Value = $"{string.Format("{0:0,0.00}", value2)}";

                            //worksheet.Cells[i, 4].Value = $"{string.Format("{0:0,0.00}", authoritysummary.AnnualCeiling)}";

                            worksheet.Cells[i, columnIndex].Value = $"{string.Format("{0:0,0.00}", authoritysummary.TotalDisbursement)}";

                            //loop through the disbursements
                            if (disbursementTrancheList != null)
                            {
                                foreach (var disbursementTrancheList2 in disbursementTrancheList)
                                {

                                    authoritysummary = disbursementViewModel.DisbursementSummaryViewModels.Where(x => x.AuthorityId == AuthorityIdPrev)
                                        .FirstOrDefault();

                                    result = int.TryParse((trancheColumnID + disbursementTrancheList2.ID).ToString(), out newTrancheColumnID);//place it in the right column

                                    var disbursementsforAuthorityAndforFinancialYear = disbursements.Where(x => x.AuthorityId == AuthorityIdPrev);
                                    if (disbursementsforAuthorityAndforFinancialYear != null)
                                    {
                                        try
                                        {
                                            //Get disbursement sum for the tranche id
                                            var disbursementEntriesforTrancheID = disbursementsforAuthorityAndforFinancialYear.Where(x => x.DisbursementTrancheId == disbursementTrancheList2.ID);
                                            double amt = 0d; amt = disbursementEntriesforTrancheID.Sum(x => x.Amount);
                                            worksheet.Cells[i, newTrancheColumnID].Value = string.Format("{0:0,0.00}", amt);
                                        }
                                        catch (Exception Ex)
                                        {
                                            _logger.LogError(Ex, $"DisbursementController.WriteToDisbursements Error {Environment.NewLine}");
                                        }
                                    }
                                }
                            }

                            //%
                            double r = disbursementViewModel.DisbursementSummaryByBudgetCeilingViewModels.Where(x => x.AuthorityId == AuthorityIdPrev)
                                .Sum(s => s.BudgetCeilingComputation.Amount);
                            worksheet.Cells[i, newTrancheColumnID + 2].Value = string.Format("{0:0.00%}", authoritysummary.TotalDisbursement / r);
                            string extremecol1 = ExcelCellBase.GetAddress(i, newTrancheColumnID + 2);
                            worksheet.Cells[$"B{i}:{extremecol1.Substring(0, 1)}{i}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[$"B{i}:{extremecol1.Substring(0, 1)}{i}"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#BFBFBF"));

                            //dictTranches.Clear();
                            i++;
                            i++;
                        }
                        else
                        {
                            i++;
                        }

                    }

                    //Road Agency/Recepient
                    worksheet.Cells[i, 1].Value = i - 2;
                    DisbursementCodeList disbursementCodeList = disbursementCodeLists.Where(x => x.Code == rptItem.BudgetCeilingComputation.Code).FirstOrDefault();
                    if (disbursementCodeList == null)
                    {
                        worksheet.Cells[i, 2].Value = $"{rptItem.BudgetCeilingComputation.Name}";
                    }
                    else
                    {
                        worksheet.Cells[i, 2].Value = $"{disbursementCodeList.DisplayName}";
                    }



                    //%                       
                    if (disbursementCodeList == null)
                    {
                        worksheet.Cells[i, 3].Value = "";
                    }
                    else
                    {
                        double per = 0d;
                        bool res = double.TryParse(disbursementCodeList.Percent.ToString(), out per);
                        if (per == 0d)
                        {
                            worksheet.Cells[i, 3].Value = "";
                        }
                        else
                        {
                            worksheet.Cells[i, 3].Value = (per).ToString("P", CultureInfo.InvariantCulture);
                        }

                    }

                    //D1=Annual Ceiling FY 2020/2021 (Kshs)
                    worksheet.Cells[i, 4].Value = string.Format("{0:0,0.00}", rptItem.AnnualCeiling);
                    TotalAnnualCeiling = TotalAnnualCeiling + rptItem.AnnualCeiling;
                    //Sum the annual ceiling
                    // Keep the result of TryGetValue.
                    double value;
                    if (dictAnnualCeilingAuthorityTotals.TryGetValue(AuthorityIdCurrent, out value))
                    {
                        dictAnnualCeilingAuthorityTotals[AuthorityIdCurrent] = value + rptItem.AnnualCeiling;
                    }
                    else
                    {
                        dictAnnualCeilingAuthorityTotals[AuthorityIdCurrent] = value + rptItem.AnnualCeiling;
                    }

                    //loop through the disbursements
                    if (disbursementTrancheList != null)
                    {
                        foreach (var disbursementTrancheList2 in disbursementTrancheList)
                        {
                            var disburmentx = disbursementViewModel.Disbursements
                                  .Where(x => x.DisbursementTrancheId == disbursementTrancheList2.ID
                                  && x.BudgetCeilingComputationId == rptItem.BudgetCeilingComputation.ID);

                            result = int.TryParse((trancheColumnID + disbursementTrancheList2.ID).ToString(), out newTrancheColumnID);
                            if (disburmentx.Any())
                            {
                                try
                                {
                                    disburmentxAmount = disburmentx.Sum(x => x.Amount);
                                    worksheet.Cells[i, newTrancheColumnID].Value = string.Format("{0:0,0.00}", disburmentxAmount);
                                    dictTranches[disbursementTrancheList2.ID] = dictTranches[disbursementTrancheList2.ID] + disburmentxAmount;
                                }
                                catch (Exception Ex)
                                {
                                    _logger.LogError(Ex, $"DisbursementController.WriteToDisbursements Error {Environment.NewLine}");
                                }
                            }
                        }
                    }

                    //Total disbursement
                    worksheet.Cells[i, columnIndex].Value = string.Format("{0:0,0.00}", rptItem.TotalDisbursement);
                    TotalDisbursement = TotalDisbursement + rptItem.TotalDisbursement;

                    //% of ceiling
                    //worksheet.Cells[i, columnIndex + 1].Value = string.Format("{0:0,0.00000%}", rptItem.PercentOfCeiling);
                    worksheet.Cells[i, columnIndex + 1].Value = (rptItem.PercentOfCeiling).ToString("P", CultureInfo.InvariantCulture);

                    AuthorityIdPrev = rptItem.AuthorityId;
                    disbursementSummaryViewModelFinal = rptItem;
                    i++;
                }
            }

            //-------------The final authority sub totals are not shown and thus the repeated code is added heare-----------------
            //Road Agency/ Recepient
            worksheet.Cells[i, 1].Value = i - 2;//Serial No
            worksheet.Cells[i, 2].Value = $"Total {disbursementSummaryViewModelFinal.Authority.Code}";

            //%                       
            //worksheet.Cells[i, 3].Value = disbursementSummaryViewModelFinal.Percent.ToString("P", CultureInfo.InvariantCulture);

            //D1=Annual Ceiling FY 2020/2021 (Kshs)
            worksheet.Cells[i, 4].Value = string.Format("{0:0,0.00}", disbursementSummaryViewModelFinal.AnnualCeiling);

            //loop through the disbursements
            if (disbursementTrancheList != null)
            {
                foreach (var disbursementTrancheList2 in disbursementTrancheList)
                {
                    var disburmentx = disbursementViewModel.Disbursements
                          .Where(x => x.DisbursementTrancheId == disbursementTrancheList2.ID
                          && x.BudgetCeilingComputationId == disbursementSummaryViewModelFinal.BudgetCeilingComputation.ID);
                    //.FirstOrDefault();

                    result = int.TryParse((trancheColumnID + disbursementTrancheList2.ID).ToString(), out newTrancheColumnID);
                    if (disburmentx.Any())
                    {
                        try
                        {
                            double sum = disburmentx.Sum(x => x.Amount);
                            worksheet.Cells[i, newTrancheColumnID].Value = string.Format("{0:0,0.00}", sum);
                            //dictTranches[disbursementTrancheList2.ID] = dictTranches[disbursementTrancheList2.ID] + disburmentx.Amount;
                        }
                        catch (Exception Ex)
                        {
                            _logger.LogError(Ex, $"DisbursementController.WriteToDisbursements Error {Environment.NewLine}");
                        }
                    }
                }
            }

            //Total disbursement
            worksheet.Cells[i, columnIndex].Value = string.Format("{0:0,0.00}", disbursementSummaryViewModelFinal.TotalDisbursement);
            TotalDisbursement = TotalDisbursement + disbursementSummaryViewModelFinal.TotalDisbursement;

            //% of ceiling
            //worksheet.Cells[i, columnIndex + 1].Value = string.Format("{0:0,0.00%}", disbursementSummaryViewModelFinal.PercentOfCeiling);
            worksheet.Cells[i, columnIndex + 1].Value = disbursementSummaryViewModelFinal.PercentOfCeiling.ToString("P", CultureInfo.InvariantCulture);

            string extremecol2 = ExcelCellBase.GetAddress(i, newTrancheColumnID + 2);
            worksheet.Cells[$"B{i}:{extremecol2.Substring(0, 1)}{i}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[$"B{i}:{extremecol2.Substring(0, 1)}{i}"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#BFBFBF"));
            i++;
            //-------------End------The final authority sub totals are not shown and thus the repeated code is added here-----------------

            return i;
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> ExportDisbursementReport2(long FinancialYearId)
        {
            try
            {
                //Get the data
                DisbursementViewModel disbursementViewModel = new DisbursementViewModel();

                //Get current financial year
                var respFinancialYear = await _financialYearService.FindByIdAsync(FinancialYearId).ConfigureAwait(false);
                if (respFinancialYear.Success)
                {
                    //Set financial Year
                    disbursementViewModel.FinancialYear = respFinancialYear.FinancialYear;

                    var resp = await _disbursementService.ListAsync(respFinancialYear.FinancialYear.ID).ConfigureAwait(false);
                    disbursementViewModel.Disbursements = resp.Disbursement;

                    //Get the summaries ByBudgetCeilingComputation
                    IList<DisbursementSummaryViewModel> disbursementSummaryViewModel = null;
                    var respDisbursementSummaries = await _disbursementService.DisbursementSummaryByBudgetCeilingComputationAsync(respFinancialYear.FinancialYear.ID).ConfigureAwait(false);
                    if (respDisbursementSummaries.Success)
                    {
                        var objectResult = (ObjectResult)respDisbursementSummaries.IActionResult;
                        if (objectResult != null)
                        {
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var result = (OkObjectResult)objectResult;
                                var anon = result.Value;
                                disbursementViewModel.DisbursementSummaryByBudgetCeilingViewModels = await ReadAnonymousTypeForReport(anon, disbursementViewModel).ConfigureAwait(false);
                            }
                        }
                    }


                    //Get the summaries DisbursementSummaryAsync
                    respDisbursementSummaries = await _disbursementService.DisbursementSummaryAsync(respFinancialYear.FinancialYear.ID).ConfigureAwait(false);
                    if (respDisbursementSummaries.Success)
                    {
                        var objectResult = (ObjectResult)respDisbursementSummaries.IActionResult;
                        if (objectResult != null)
                        {
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var result = (OkObjectResult)objectResult;
                                var anon = result.Value;
                                disbursementViewModel.DisbursementSummaryViewModels = await ReadAnonymousType(anon).ConfigureAwait(false);
                            }
                        }
                    }

                }

                if (disbursementViewModel.DisbursementSummaryViewModels == null)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Failed",
                        Href = Url.Action("Index", "Disbursement")
                    });
                }

                //Get Percentages for the counties
                var respallocationCodeUnit = await _allocationCodeUnitService.ListAsync("CG").ConfigureAwait(false);
                if (respallocationCodeUnit.Success)
                {
                    disbursementViewModel.AllocationCodeUnits = respallocationCodeUnit.AllocationCodeUnit;
                }

                //Write to Excel
                MemoryStream stream = new MemoryStream();
                JsonResult myjson = await WriteToDisbursement2(disbursementViewModel, stream).ConfigureAwait(false);
                return Json(myjson);

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementController.ExportDisbursementReport Error {Environment.NewLine}");
                return Json(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<JsonResult> WriteToDisbursement2(DisbursementViewModel disbursementViewModel, MemoryStream stream)
        {
            //create dictionary for tranches
            dictTranches.Clear();
            TotalAnnualCeiling = 0d;
            TotalDisbursement = 0d;

            //Get disbursements
            IEnumerable<Disbursement> disbursements = null;
            var respDisbursements = await _disbursementService.ListAsync(disbursementViewModel.FinancialYear.ID).ConfigureAwait(false);
            if (respDisbursements.Success)
            {
                disbursements = respDisbursements.Disbursement;
            }

            //get disbursement code lists
            IList<DisbursementCodeList> disbursementCodeLists = null;
            var respDisbursementCodeList = await _disbursementCodeListService.ListAsync().ConfigureAwait(false);
            if (respDisbursementCodeList.Success)
            {
                var objectResult = (ObjectResult)respDisbursementCodeList.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        disbursementCodeLists = ((IList<DisbursementCodeList>)result.Value).OrderBy(x => x.Order).ToList();
                    }
                }
            }


            //Create a new ExcelPackage  
            string handle = Guid.NewGuid().ToString();
            using (ExcelPackage excelPackage = new ExcelPackage(stream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = "KRB";
                excelPackage.Workbook.Properties.Title = "Sno.12 - Disbursement template";
                excelPackage.Workbook.Properties.Subject = "Sno.12 - Disbursement template Report";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("ALL KRBF");

                //Add some text to cell A1
                worksheet.Cells["A1"].Value = $"DISBURSEMENT SCHEDULE FOR FY {disbursementViewModel.FinancialYear.Code}" +
                    $" (AS AT {Ordinal(DateTime.Now.Day)} {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month).ToUpper()}" +
                    $" {DateTime.Now.Year})";
                worksheet.Cells["A1:H1"].Merge = true;
                worksheet.Cells["A1:H1"].Style.Font.Size = 11;
                worksheet.Cells["A1:H1"].Style.Font.Name = "Arial";
                worksheet.Cells["A1:H1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:H1"].Style.Font.Bold = true;
                //worksheet.Cells["A1:H1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //worksheet.Cells["A1:H1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#9CC2E5"));

                //Create the headers/columns

                //=RNo
                worksheet.Cells["A2"].Value = "No";
                worksheet.Cells["A2"].Style.Font.Size = 11;
                worksheet.Cells["A2"].Style.Font.Name = "Arial";
                worksheet.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2"].Style.Font.Bold = true;
                worksheet.Cells["A2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A2"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#9CC2E5"));

                //B1=Road Agency/Recipient
                worksheet.Cells["B2"].Value = "Road Agency/Recipient";
                worksheet.Cells["B2"].Style.Font.Size = 11;
                worksheet.Cells["B2"].Style.Font.Name = "Arial";
                worksheet.Cells["B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B2"].Style.Font.Bold = true;
                worksheet.Cells["B2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["B2"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#9CC2E5"));

                worksheet.Column(2).Width = 24.86;

                //C1=%
                worksheet.Cells["C2"].Value = "%";
                worksheet.Cells["C2"].Style.Font.Size = 11;
                worksheet.Cells["C2"].Style.Font.Name = "Arial";
                worksheet.Cells["C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["C2"].Style.Font.Bold = true;
                worksheet.Cells["C2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["C2"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#9CC2E5"));

                //D1=Annual Ceiling FY 2020/2021 (Kshs)
                worksheet.Cells["D2"].Value = $"Annual Ceiling FY {disbursementViewModel.FinancialYear.Code} (Kshs)";
                worksheet.Cells["D2"].Style.Font.Size = 11;
                worksheet.Cells["D2"].Style.Font.Name = "Arial";
                worksheet.Cells["D2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["D2"].Style.Font.Bold = true;
                worksheet.Cells["D2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["D2"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#9CC2E5"));

                //Tranches
                int columnIndex = 5;
                IList<DisbursementTranche> disbursementTrancheList = null;
                var rspdisbursementTranche = await _disbursementTrancheService.ListAsync().ConfigureAwait(false);
                if (rspdisbursementTranche.Success)
                {
                    var objectResult = (ObjectResult)rspdisbursementTranche.IActionResult;
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result2 = (OkObjectResult)objectResult;
                        disbursementTrancheList = (IList<DisbursementTranche>)result2.Value;
                    }
                }
                if (disbursementTrancheList != null)
                {
                    foreach (var disbursementTrancheList2 in disbursementTrancheList)
                    {
                        worksheet.Cells[2, columnIndex].Value = disbursementTrancheList2.Name;
                        worksheet.Cells[2, columnIndex].Style.Font.Size = 11;
                        worksheet.Cells[2, columnIndex].Style.Font.Name = "Arial";
                        worksheet.Cells[2, columnIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[2, columnIndex].Style.Font.Bold = true;
                        worksheet.Cells[2, columnIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[2, columnIndex].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#9CC2E5"));

                        dictTranches.Add(disbursementTrancheList2.ID, 0d);
                        columnIndex++;
                    }
                }

                //Total disbursement
                worksheet.Cells[2, columnIndex].Value = "Total Disbursement to Date";
                worksheet.Cells[2, columnIndex].Style.Font.Size = 11;
                worksheet.Cells[2, columnIndex].Style.Font.Name = "Arial";
                worksheet.Cells[2, columnIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, columnIndex].Style.Font.Bold = true;
                worksheet.Cells[2, columnIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[2, columnIndex].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#9CC2E5"));

                //%of ceiling
                worksheet.Cells[2, columnIndex + 1].Value = "% of Ceiling";
                worksheet.Cells[2, columnIndex + 1].Style.Font.Size = 11;
                worksheet.Cells[2, columnIndex + 1].Style.Font.Name = "Arial";
                worksheet.Cells[2, columnIndex + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, columnIndex + 1].Style.Font.Bold = true;
                worksheet.Cells[2, columnIndex + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[2, columnIndex + 1].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#9CC2E5"));

                //Compute Authority Sub Totals
                int i = 3;
                int trancheColumnID = 4;
                int newTrancheColumnID = 0;
                bool result = false;
                i = await WriteDisbursementCodeListValues(worksheet, disbursementViewModel, disbursementTrancheList, disbursementCodeLists, columnIndex, i).ConfigureAwait(false);

                //Road Agency
                worksheet.Cells[i, 2].Value = "Total";

                //Total
                worksheet.Cells[i, 4].Value = string.Format("{0:0,0.00}", TotalAnnualCeiling);


                //Total Disbursement
                worksheet.Cells[i, columnIndex].Value = string.Format("{0:0,0.00}", TotalDisbursement);
                worksheet.Cells[i, columnIndex].Style.Font.Size = 11;
                worksheet.Cells[i, columnIndex].Style.Font.Name = "Arial";
                worksheet.Cells[i, columnIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[i, columnIndex].Style.Font.Bold = true;

                //Total % of ceiling                
                //worksheet.Cells[i, columnIndex + 1].Value = string.Format("{0:0.0%}", TotalAnnualCeiling / TotalDisbursement);
                worksheet.Cells[i, columnIndex + 1].Value = (TotalDisbursement / TotalAnnualCeiling).ToString("P", CultureInfo.InvariantCulture);

                // ColumnNames.Add(worksheet.Cells[1, i].Value.ToString()); // 1 = First Row, i = Column Number
                string extremecol = ExcelCellBase.GetAddress(1, columnIndex + 1);
                string column = extremecol.Substring(0, 1);

                worksheet.Cells[$"A2:{column}{i}"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[$"A2:{column}{i}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[$"A2:{column}{i}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[$"A2:{column}{i}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells[$"B{i}:{column}{i}"].Style.Font.Size = 11;
                worksheet.Cells[$"B{i}:{column}{i}"].Style.Font.Name = "Arial";
                worksheet.Cells[$"B{i}:{column}{i}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"B{i}:{column}{i}"].Style.Font.Bold = true;
                worksheet.Cells[$"B{i}:{column}{i}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[$"B{i}:{column}{i}"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#9CC2E5"));

                worksheet.Cells[$"B{i + 1}"].Value = $"N.B. Each single disbursement for all the 47 CGs is an equivalent of the total quarterly allocation amount. ";
                worksheet.Cells[$"B{i + 1}:{column}{i + 1}"].Merge = true;
                worksheet.Cells[$"B{i + 1}:{column}{i + 1}"].Style.Font.Size = 11;
                worksheet.Cells[$"B{i + 1}:{column}{i + 1}"].Style.Font.Name = "Arial";
                worksheet.Cells[$"B{i + 1}:{column}{i + 1}"].Style.Font.Italic = true;
                worksheet.Cells[$"B{i + 1}:{column}{i + 1}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"B{i + 1}:{column}{i + 1}"].Style.Font.Bold = true;

                //Create the WorkSheet for CG Ceiling
                ExcelWorksheet worksheetCGCeiling = excelPackage.Workbook.Worksheets.Add("CGs Ceilings");

                //Populate the WorkSheet for CG Ceiling
                await CGWorkSheet2(worksheetCGCeiling, disbursementViewModel).ConfigureAwait(false);

                //Auto fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                _cache.Set(handle, excelPackage.GetAsByteArray(),
                    new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));

                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Href = Url.Action("Download2", "Disbursement", new { fileGuid = handle, FileName = "Sno.12-Disbursement Report.xlsx" })
                });

            }

            //stream.Close();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<int> WriteDisbursementCodeListValues(ExcelWorksheet worksheet, DisbursementViewModel disbursementViewModel, IList<DisbursementTranche> disbursementTrancheList,
            IList<DisbursementCodeList> disbursementCodeLists, int columnIndex, int i)
        {
            int trancheColumnID = 4;
            int newTrancheColumnID = 0;
            bool result = false;
            double disburmentxAmount = 0d;
            Dictionary<long, double> dictAnnualCeilingAuthorityTotals = new Dictionary<long, double>();

            //get budget ceiling computations for financial year of interest
            IList<BudgetCeilingComputation> budgetCeilingComputations = null;
            var respBudgetCeilingComputation = await _budgetCeilingComputationService.ListAsync(disbursementViewModel.FinancialYear.ID).ConfigureAwait(false);
            if (respBudgetCeilingComputation.Success)
            {
                var objectResult = (ObjectResult)respBudgetCeilingComputation.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result3 = (OkObjectResult)objectResult;
                        budgetCeilingComputations = (IList<BudgetCeilingComputation>)result3.Value;

                    }
                }
            }

            //get summaries by ByFinancialYearIDAndAuthorityIDAndDisbursementTrancheId
            IList<DisbursementSummaryViewModel> disbursementSummaryViewModels = null;
            var respSummaries = await _disbursementService.SummarizeByFinancialYearIDAndAuthorityIDAndDisbursementTrancheIdAsync(
                disbursementViewModel.FinancialYear.ID).ConfigureAwait(false);
            if (respSummaries.Success)
            {
                var objectResult = (ObjectResult)respSummaries.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result3 = (OkObjectResult)objectResult;
                        disbursementSummaryViewModels = (IList<DisbursementSummaryViewModel>)result3.Value;

                    }
                }
            }

            //loop trhough each disbursement code list
            foreach (var disbursementCodeList in disbursementCodeLists)
            {
                //Road Agency/Recepient
                worksheet.Cells[i, 1].Value = disbursementCodeList.SNo;
                worksheet.Cells[i, 2].Value = $"{disbursementCodeList.DisplayName}";

                //%                       
                if (disbursementCodeList == null)
                {
                    worksheet.Cells[i, 3].Value = "";
                }
                else
                {
                    double per = 0d;
                    bool res = double.TryParse(disbursementCodeList.Percent.ToString(), out per);
                    if (per == 0d)
                    {
                        worksheet.Cells[i, 3].Value = "";
                    }
                    else
                    {
                        worksheet.Cells[i, 3].Value = (per).ToString("P", CultureInfo.InvariantCulture);
                        TotalPercent += per;
                    }
                }

                /*
                 * D1=Annual Ceiling FY 2020/2021 (Kshs)
                 */
                //get the code
                var code = disbursementCodeList.Code;
                //Get budgeteiling computation
                var budgetCeilingComputation = budgetCeilingComputations.Where(x => x.Code == code).FirstOrDefault();
                if (budgetCeilingComputation != null)
                {
                    var annualCeiling = budgetCeilingComputation.Amount;

                    worksheet.Cells[i, 4].Value = string.Format("{0:0,0.00}", annualCeiling);
                    TotalAnnualCeiling = TotalAnnualCeiling + annualCeiling;

                    //loop through the disbursements
                    if (disbursementTrancheList != null)
                    {
                        foreach (var disbursementTrancheList2 in disbursementTrancheList)
                        {
                            var disburmentx = disbursementViewModel.Disbursements
                                  .Where(x => x.DisbursementTrancheId == disbursementTrancheList2.ID
                                  && x.BudgetCeilingComputationId == budgetCeilingComputation.ID);

                            result = int.TryParse((trancheColumnID + disbursementTrancheList2.ID).ToString(), out newTrancheColumnID);
                            if (disburmentx.Any())
                            {
                                try
                                {
                                    disburmentxAmount = disburmentx.Sum(x => x.Amount);
                                    worksheet.Cells[i, newTrancheColumnID].Value = string.Format("{0:0,0.00}", disburmentxAmount);
                                    dictTranches[disbursementTrancheList2.ID] = dictTranches[disbursementTrancheList2.ID] + disburmentxAmount;
                                }
                                catch (Exception Ex)
                                {
                                    _logger.LogError(Ex, $"DisbursementController.WriteToDisbursements Error {Environment.NewLine}");
                                }
                            }
                        }
                    }

                    //Total disbursement
                    var authorityID = budgetCeilingComputation.AuthorityId;
                    var disbursementSummaryViewModel = disbursementViewModel.DisbursementSummaryViewModels.Where(x => x.AuthorityId == authorityID)
                        .FirstOrDefault();
                    if (disbursementSummaryViewModel != null)
                    {
                        var totalDisbursement = disbursementSummaryViewModel.TotalDisbursement;
                        worksheet.Cells[i, columnIndex].Value = string.Format("{0:0,0.00}", totalDisbursement);
                        TotalDisbursement = TotalDisbursement + totalDisbursement;
                        //% of ceiling
                        //worksheet.Cells[i, columnIndex + 1].Value = string.Format("{0:0,0.00000%}", rptItem.PercentOfCeiling);
                        worksheet.Cells[i, columnIndex + 1].Value = disbursementSummaryViewModel.PercentOfCeiling.ToString("P", CultureInfo.InvariantCulture);
                    }
                }

                /*
                 * Check if display name.substring 5 ==total
                 */
                if (disbursementCodeList.DisplayName != null && disbursementCodeList.DisplayName.Length >= 5)
                {
                    string subString = disbursementCodeList.DisplayName.Substring(0, 5);
                    if (subString.ToLower() == "total")
                    {
                        //Get sub totals
                        var authorityId = disbursementCodeList.AuthorityId;
                        long _authorityId = 0;
                        bool result10 = long.TryParse(authorityId.ToString(), out _authorityId);

                        var disbursement = disbursementViewModel.DisbursementSummaryViewModels
                            .Where(x => x.AuthorityId == _authorityId);

                        //Annual ceiling
                        var annualCeilingSum = disbursement.Sum(s => s.AnnualCeiling);
                        worksheet.Cells[i, 4].Value = string.Format("{0:0,0.00}", annualCeilingSum);

                        //loop through the disbursements
                        if (disbursementTrancheList != null)
                        {
                            foreach (var disbursementTranche in disbursementTrancheList)
                            {
                                result = int.TryParse((trancheColumnID + disbursementTranche.ID).ToString(), out newTrancheColumnID);
                                try
                                {
                                    var trancheAmt = disbursementSummaryViewModels
                                        .Where(x => x.AuthorityId == _authorityId && x.DisbursementTrancheId == disbursementTranche.ID)
                                        .Sum(s => s.DisbursementTrancheAmount);
                                    worksheet.Cells[i, newTrancheColumnID].Value = string.Format("{0:0,0.00}", trancheAmt);
                                }
                                catch (Exception Ex)
                                {
                                    _logger.LogError(Ex, $"DisbursementController.WriteToDisbursements Error {Environment.NewLine}");
                                }
                            }
                        }

                        //total disbursement
                        var totaldisbursementSum = disbursement.Sum(s => s.TotalDisbursement);
                        worksheet.Cells[i, columnIndex].Value = string.Format("{0:0,0.00}", totaldisbursementSum);

                        //% of ceiling
                        worksheet.Cells[i, columnIndex + 1].Value = (totaldisbursementSum / annualCeilingSum).ToString("P", CultureInfo.InvariantCulture);

                        //format
                        string extremecol1 = ExcelCellBase.GetAddress(i, newTrancheColumnID + 2);
                        worksheet.Cells[$"B{i}:{extremecol1.Substring(0, 1)}{i}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[$"B{i}:{extremecol1.Substring(0, 1)}{i}"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#BFBFBF"));
                        i++;
                    }
                }

                /*
                 * Check if display name.substring 5 ==Sub Total 1 or
                 * Check if display name.substring 5 ==Sub Total 2
                 */
                if (disbursementCodeList.Name != null)
                {
                    if (disbursementCodeList.Name.ToLower() == "Sub Total 1".ToLower() || disbursementCodeList.Name.ToLower() == "Sub Total 2".ToLower())
                    {
                        if (disbursementCodeList.Name.ToLower() == "Sub Total 1".ToLower())
                        {
                            //Total %Ceiling
                            worksheet.Cells[i, 3].Value = (TotalPercent).ToString("P", CultureInfo.InvariantCulture);

                        }
                        //Total Annual ceiling
                        worksheet.Cells[i, 4].Value = string.Format("{0:0,0.00}", TotalAnnualCeiling);

                        //loop through the disbursements
                        if (disbursementTrancheList != null)
                        {
                            foreach (var disbursementTranche in disbursementTrancheList)
                            {
                                result = int.TryParse((trancheColumnID + disbursementTranche.ID).ToString(), out newTrancheColumnID);
                                try
                                {
                                    worksheet.Cells[i, newTrancheColumnID].Value = string.Format("{0:0,0.00}", dictTranches[disbursementTranche.ID]);
                                }
                                catch (Exception Ex)
                                {
                                    _logger.LogError(Ex, $"DisbursementController.WriteToDisbursements Error {Environment.NewLine}");
                                }
                            }
                        }

                        //total disbursement
                        worksheet.Cells[i, columnIndex].Value = string.Format("{0:0,0.00}", TotalDisbursement);

                        //% of ceiling
                        worksheet.Cells[i, columnIndex + 1].Value = (TotalDisbursement / TotalAnnualCeiling).ToString("P", CultureInfo.InvariantCulture);

                        //format
                        string extremecol1 = ExcelCellBase.GetAddress(i, newTrancheColumnID + 2);
                        worksheet.Cells[$"B{i}:{extremecol1.Substring(0, 1)}{i}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[$"B{i}:{extremecol1.Substring(0, 1)}{i}"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#9CC2E5"));
                    }
                }
                i++;
            }

            return i;
        }

        private string Ordinal(int number)
        {
            var ones = number % 10;
            var tens = Math.Floor(number / 10f) % 10;
            if (tens == 1)
            {
                return number + "th".ToUpper();
            }

            switch (ones)
            {
                case 1: return number + "st".ToUpper();
                case 2: return number + "nd".ToUpper();
                case 3: return number + "rd".ToUpper();
                default: return number + "th".ToUpper();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task CGWorkSheet(ExcelWorksheet worksheet, DisbursementViewModel disbursementViewModel)
        {
            try
            {
                //Get budget Ceiling amounts
                var respBudhetceiling = await _budgetCeilingService.ListAsync("CG", disbursementViewModel.FinancialYear.ID).ConfigureAwait(false);
                if (respBudhetceiling != null)
                {
                    disbursementViewModel.BudgetCeilings = respBudhetceiling;
                }

                //Add some text to cell A5
                worksheet.Cells["A5"].Value = "No.";
                worksheet.Cells["A5"].Merge = true;
                worksheet.Cells["A5"].Style.Font.Size = 11;
                worksheet.Cells["A5"].Style.Font.Name = "Arial";
                worksheet.Cells["A5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A5"].Style.Font.Bold = true;

                //Add some text to cell B5
                worksheet.Cells["B5"].Value = "COUNTY NAME";
                worksheet.Cells["B5"].Merge = true;
                worksheet.Cells["B5"].Style.Font.Size = 11;
                worksheet.Cells["B5"].Style.Font.Name = "Arial";
                worksheet.Cells["B5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B5"].Style.Font.Bold = true;

                //Add some text to cell C5
                worksheet.Cells["C5"].Value = "Ratio";
                worksheet.Cells["C5"].Merge = true;
                worksheet.Cells["C5"].Style.Font.Size = 11;
                worksheet.Cells["C5"].Style.Font.Name = "Arial";
                worksheet.Cells["C5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["C5"].Style.Font.Bold = true;

                //Add some text to cell D5
                worksheet.Cells["D5"].Value = "ALLOCATION";
                worksheet.Cells["D5"].Merge = true;
                worksheet.Cells["D5"].Style.Font.Size = 11;
                worksheet.Cells["D5"].Style.Font.Name = "Arial";
                worksheet.Cells["D5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["D5"].Style.Font.Bold = true;

                //Add some text to cell E5
                worksheet.Cells["E5"].Value = "QUARTERLY DISBUREMENT";
                worksheet.Cells["E5"].Merge = true;
                worksheet.Cells["E5"].Style.Font.Size = 11;
                worksheet.Cells["E5"].Style.Font.Name = "Arial";
                worksheet.Cells["E5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["E5"].Style.Font.Bold = true;
                worksheet.Column(5).Width = 30.86;

                //Add some text to cell A3
                worksheet.Cells["A3"].Value = $"COUNTY GOVERNMENTS CEILINGS SCHEDULE - FY {disbursementViewModel.FinancialYear.Code}";
                worksheet.Cells["A3:E3"].Merge = true;
                worksheet.Cells["A3:E3"].Style.Font.Size = 12;
                worksheet.Cells["A3:E3"].Style.Font.Name = "Arial";
                worksheet.Cells["A3:E3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A3:E3"].Style.Font.Bold = true;

                //Add some text to cell A2
                worksheet.Cells["A1"].Value = "ROAD MAINTENANCE LEVY FUND";
                worksheet.Cells["A1:E1"].Merge = true;
                worksheet.Cells["A1:E1"].Style.Font.Size = 12;
                worksheet.Cells["A1:E1"].Style.Font.Name = "Arial";
                worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:E1"].Style.Font.Bold = true;



                int i = 6;
                double ratio = 0d;
                double allocation = 0d;
                double cumulativeTotalDisbursement = 0d;

                if (disbursementViewModel.AllocationCodeUnits != null)
                {

                    foreach (var rptItem in disbursementViewModel.AllocationCodeUnits)
                    {
                        //No
                        worksheet.Cells[i, 1].Value = i - 5;

                        //County Name
                        worksheet.Cells[i, 2].Value = rptItem.Authority.Name;

                        //County Name
                        worksheet.Cells[i, 3].Value = string.Format("{0:0.0%}", rptItem.Percent);
                        ratio = ratio + rptItem.Percent;

                        //Allocation
                        var budgetCeiling = disbursementViewModel.BudgetCeilings
                            .Where(w => w.Authority.ID == rptItem.AuthorityId)
                            .FirstOrDefault();
                        if (budgetCeiling != null)
                        {
                            worksheet.Cells[i, 4].Value = string.Format("{0:0,0.00}", budgetCeiling.Amount);
                            allocation = allocation + budgetCeiling.Amount;
                        }

                        //Quartely Disbursement
                        var totalDisbursement = disbursementViewModel.DisbursementSummaryViewModels
                        .Where(w => w.Authority.ID == rptItem.AuthorityId)
                        .FirstOrDefault();

                        if (totalDisbursement != null)
                        {
                            worksheet.Cells[i, 5].Value = string.Format("{0:0,0.00}", totalDisbursement.TotalDisbursement);
                            cumulativeTotalDisbursement = cumulativeTotalDisbursement + totalDisbursement.TotalDisbursement;
                        }
                        i++;
                    }
                }
                worksheet.Cells[i, 2].Value = "Total";
                worksheet.Cells[i, 3].Value = string.Format("{0:0.0%}", ratio);
                worksheet.Cells[i, 4].Value = string.Format("{0:0,0.00}", allocation);
                worksheet.Cells[i, 5].Value = string.Format("{0:0,0.00}", cumulativeTotalDisbursement);
                worksheet.Cells[$"A{i}:E{i}"].Style.Font.Bold = true;

                //formatting                  
                worksheet.Cells[$"A5:E{i}"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[$"A5:E{i}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[$"A5:E{i}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[$"A5:E{i}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                //Colour
                worksheet.Cells[$"D5:E{i - 1}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[$"D5:E{i - 1}"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#BDD7EE"));
                worksheet.Cells[$"D{i}:E{i}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[$"D{i}:E{i}"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#FFFF00"));

                //Auto fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"DisbursementController.CGWorkSheet Error {Environment.NewLine}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task CGWorkSheet2(ExcelWorksheet worksheet, DisbursementViewModel disbursementViewModel)
        {
            try
            {
                //Get budget Ceiling amounts
                var respBudhetceiling = await _budgetCeilingService.ListAsync("CG", disbursementViewModel.FinancialYear.ID).ConfigureAwait(false);
                if (respBudhetceiling != null)
                {
                    disbursementViewModel.BudgetCeilings = respBudhetceiling;
                }

                //Add some text to cell A5
                worksheet.Cells["A5"].Value = "No.";
                worksheet.Cells["A5"].Merge = true;
                worksheet.Cells["A5"].Style.Font.Size = 11;
                worksheet.Cells["A5"].Style.Font.Name = "Arial";
                worksheet.Cells["A5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A5"].Style.Font.Bold = true;

                //Add some text to cell B5
                worksheet.Cells["B5"].Value = "COUNTY NAME";
                worksheet.Cells["B5"].Merge = true;
                worksheet.Cells["B5"].Style.Font.Size = 11;
                worksheet.Cells["B5"].Style.Font.Name = "Arial";
                worksheet.Cells["B5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B5"].Style.Font.Bold = true;

                //Add some text to cell C5
                worksheet.Cells["C5"].Value = "Ratio";
                worksheet.Cells["C5"].Merge = true;
                worksheet.Cells["C5"].Style.Font.Size = 11;
                worksheet.Cells["C5"].Style.Font.Name = "Arial";
                worksheet.Cells["C5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["C5"].Style.Font.Bold = true;

                //Add some text to cell D5
                worksheet.Cells["D5"].Value = "ALLOCATION";
                worksheet.Cells["D5"].Merge = true;
                worksheet.Cells["D5"].Style.Font.Size = 11;
                worksheet.Cells["D5"].Style.Font.Name = "Arial";
                worksheet.Cells["D5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["D5"].Style.Font.Bold = true;

                //Add some text to cell E5
                worksheet.Cells["E5"].Value = "QUARTERLY DISBUREMENT";
                worksheet.Cells["E5"].Merge = true;
                worksheet.Cells["E5"].Style.Font.Size = 11;
                worksheet.Cells["E5"].Style.Font.Name = "Arial";
                worksheet.Cells["E5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["E5"].Style.Font.Bold = true;
                worksheet.Column(5).Width = 30.86;

                //Add some text to cell A3
                worksheet.Cells["A3"].Value = $"COUNTY GOVERNMENTS CEILINGS SCHEDULE - FY {disbursementViewModel.FinancialYear.Code}";
                worksheet.Cells["A3:E3"].Merge = true;
                worksheet.Cells["A3:E3"].Style.Font.Size = 12;
                worksheet.Cells["A3:E3"].Style.Font.Name = "Arial";
                worksheet.Cells["A3:E3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A3:E3"].Style.Font.Bold = true;

                //Add some text to cell A2
                worksheet.Cells["A1"].Value = "ROAD MAINTENANCE LEVY FUND";
                worksheet.Cells["A1:E1"].Merge = true;
                worksheet.Cells["A1:E1"].Style.Font.Size = 12;
                worksheet.Cells["A1:E1"].Style.Font.Name = "Arial";
                worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:E1"].Style.Font.Bold = true;



                int i = 6;
                double ratio = 0d;
                double allocation = 0d;
                double cumulativeTotalDisbursement = 0d;

                if (disbursementViewModel.AllocationCodeUnits != null)
                {

                    foreach (var rptItem in disbursementViewModel.AllocationCodeUnits)
                    {
                        //No
                        worksheet.Cells[i, 1].Value = i - 5;

                        //County Name
                        worksheet.Cells[i, 2].Value = rptItem.Authority.Name;

                        //County Name
                        worksheet.Cells[i, 3].Value = string.Format("{0:0.0%}", rptItem.Percent);
                        ratio = ratio + rptItem.Percent;

                        //Allocation
                        var budgetCeiling = disbursementViewModel.BudgetCeilings
                            .Where(w => w.Authority.ID == rptItem.AuthorityId)
                            .FirstOrDefault();
                        if (budgetCeiling != null)
                        {
                            worksheet.Cells[i, 4].Value = string.Format("{0:0,0.00}", budgetCeiling.Amount);
                            allocation = allocation + budgetCeiling.Amount;
                        }

                        //Quartely Disbursement
                        var totalDisbursement = disbursementViewModel.DisbursementSummaryViewModels
                        .Where(w => w.Authority.ID == rptItem.AuthorityId)
                        .FirstOrDefault();

                        if (totalDisbursement != null)
                        {
                            //worksheet.Cells[i, 5].Value = string.Format("{0:0,0.00}", totalDisbursement.TotalDisbursement);
                            //cumulativeTotalDisbursement = cumulativeTotalDisbursement + totalDisbursement.TotalDisbursement;                            
                        }
                        worksheet.Cells[i, 5].Value = string.Format("{0:0,0.00}", budgetCeiling.Amount / 4.0);
                        i++;
                    }
                }
                worksheet.Cells[i, 2].Value = "Total";
                worksheet.Cells[i, 3].Value = string.Format("{0:0.0%}", ratio);
                worksheet.Cells[i, 4].Value = string.Format("{0:0,0.00}", allocation);
                worksheet.Cells[i, 5].Value = string.Format("{0:0,0.00}", cumulativeTotalDisbursement);
                worksheet.Cells[$"A{i}:E{i}"].Style.Font.Bold = true;

                //formatting                  
                worksheet.Cells[$"A5:E{i}"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[$"A5:E{i}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[$"A5:E{i}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[$"A5:E{i}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                //Colour
                worksheet.Cells[$"D5:E{i - 1}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[$"D5:E{i - 1}"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#BDD7EE"));
                worksheet.Cells[$"D{i}:E{i}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[$"D{i}:E{i}"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#FFFF00"));

                //Auto fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"DisbursementController.CGWorkSheet Error {Environment.NewLine}");
            }
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<IList<Disbursement>> GetDisbursements(long FinancialYearId)
        {
            IList<Disbursement> disbursements = null;
            try
            {
                var respDisbursementlist = await _disbursementService.ListAsync(FinancialYearId).ConfigureAwait(false);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementController.GetDisbursements Error {Environment.NewLine}");
            }
            return disbursements;
        }
        #endregion


        #region Disbursement Code List

        [Authorize(Claims.Permission.Disbursement.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DisbursementCodeListIndex(long? FinancialYearId)
        {
            try
            {
                DisbursementViewModel disbursementViewModel = new DisbursementViewModel();

                return View(disbursementViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementController Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "DisbursementController DisbursementCodeListIndex Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.Disbursement.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DisbursementCodeListActivities()
        {
            try
            {
                DisbursementViewModel disbursementViewModel = new DisbursementViewModel();

                var respDisbursementCodeList = await _disbursementCodeListService.ListAsync().ConfigureAwait(false);
                if (respDisbursementCodeList.Success)
                {
                    var objectResult = (ObjectResult)respDisbursementCodeList.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            disbursementViewModel.DisbursementCodeLists = (IEnumerable<DisbursementCodeList>)result.Value;
                        }
                    }
                }

                return PartialView("DisbursementCodeListPartialView", disbursementViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementController Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "DisbursementController DisbursementCodeListIndex Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.Disbursement.Add), Authorize(Claims.Permission.Disbursement.Change)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DisbursementCodeListAddEdit(int? id)
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
                    DisbursementCodeList disbursementCodeList = new DisbursementCodeList();
                    await PopulateDropDownsForDisbursementCodeList(0).ConfigureAwait(false);
                    return View(disbursementCodeList);
                }
                else
                {
                    DisbursementCodeList disbursementCodeList = null;
                    var resp = await _disbursementCodeListService.FindByIdAsync(ID).ConfigureAwait(false);
                    if (resp.Success)
                    {
                        var objectResult = (ObjectResult)resp.IActionResult;
                        if (objectResult != null)
                        {
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var result2 = (OkObjectResult)objectResult;
                                disbursementCodeList = (DisbursementCodeList)result2.Value;
                            }
                        }
                    }
                    if (disbursementCodeList == null)
                    {
                        return NotFound();
                    }
                    long _authorityId = 0;
                    bool result4 = long.TryParse(disbursementCodeList.AuthorityId.ToString(), out _authorityId);
                    await PopulateDropDownsForDisbursementCodeList(_authorityId).ConfigureAwait(false);
                    return View(disbursementCodeList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementController.AddEdit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "DisbursementController.AddEdit Page has reloaded");
                return View();
            }
        }

        // POST: Disbursement/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.Disbursement.Add), Authorize(Claims.Permission.Disbursement.Change)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DisbursementCodeListAddEdit(long id, [Bind("ID,Code,Name," +
            "Percent,AuthorityId,Order,SNo")] DisbursementCodeList disbursementCodeList)
        {
            try
            {
                bool success = false;
                string msg = null;
                if (id != disbursementCodeList.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    ApplicationUser loggedInuser = await GetLoggedInUser().ConfigureAwait(false);
                    //if id=0 then create new
                    if (id == 0)
                    {
                        var resp = await _disbursementCodeListService.FindByDisbursementCodeListEntryAsync(disbursementCodeList).ConfigureAwait(false);
                        if (!resp.Success)
                        {
                            DisbursementCodeList disbursementCodeListExisting = null;
                            var objectResult = (ObjectResult)resp.IActionResult;
                            if (objectResult != null)
                            {
                                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    var result = (OkObjectResult)objectResult;
                                    disbursementCodeListExisting = (DisbursementCodeList)result.Value;
                                }
                            }

                            string msgErr = $"A similar disbursement code list entry exists" +
                            $" No duplicate entries may exists for the same disbursement code list" +
                            $" Enusre that Code{(disbursementCodeListExisting != null ? disbursementCodeListExisting.Code : null)} or " +
                            $"Name : {(disbursementCodeListExisting != null ? disbursementCodeListExisting.Name : null)} doesn't exist already";
                            ModelState.AddModelError(string.Empty, msgErr);
                            //Detach the first entry before attaching your updated entry
                            var respDetach = await _disbursementCodeListService.DetachFirstEntryAsync(disbursementCodeListExisting).ConfigureAwait(false);
                            //return RedirectToAction("AddEdit",new { id=disbursement.ID});
                            return Json(new
                            {
                                Success = false,
                                Message = msgErr,
                                Href = Url.Action("AddEdit", "Disbursement", new { id = string.Empty })
                            });
                        }

                        //add disbursementcodelist
                        var disbursementResp = await _disbursementCodeListService.AddAsync(disbursementCodeList).ConfigureAwait(false);
                        if (disbursementResp.Success)
                        {
                            success = true;
                            msg = "Disbursement Code list Successfully Added";
                        }
                    }
                    //if id>0 then its an edit
                    if (id > 0)
                    {
                        var disbursementResp = await _disbursementCodeListService.Update(id, disbursementCodeList).ConfigureAwait(false);
                        if (disbursementResp.Success)
                        {
                            success = true;
                            msg = "Disbursement Code List Successfully Updated";
                        }
                    }

                    return Json(new
                    {
                        Success = success,
                        Message = msg,
                        Href = Url.Action("DisbursementCodeListIndex", "Disbursement")
                    }); ;
                }
                return Json(new
                {
                    Success = false,
                    Message = "Model Error",
                    Href = Url.Action("DisbursementCodeListIndex", "Disbursement")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementController AddEdit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "DisbursementController AddEdit Page has reloaded");
                long _authorityId = 0;
                bool result4 = long.TryParse(disbursementCodeList.AuthorityId.ToString(), out _authorityId);
                await PopulateDropDownsForDisbursementCodeList(_authorityId).ConfigureAwait(false);
                return View(disbursementCodeList);
            }
        }

        private async Task PopulateDropDownsForDisbursementCodeList(long AuthorityId)
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
        }

        // GET: DisbursementController/Details/5
        [Authorize(Claims.Permission.Disbursement.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DisbursementCodeListDetails(long Id)
        {
            try
            {
                DisbursementViewModel disbursementViewModel = new DisbursementViewModel();
                var resp = await _disbursementCodeListService.FindByIdAsync(Id).ConfigureAwait(false);
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
                        disbursementViewModel.DisbursementCodeList = (DisbursementCodeList)result.Value;
                    }
                }
                return View(disbursementViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementController.DisbursementCodeListDetails Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "DisbursementController.DisbursementCodeListDetails Page has reloaded");
                return View();
            }
        }

        // GET: DisbursementController/Details/5
        [Authorize(Claims.Permission.Disbursement.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DisbursementCodeListDelete(long? id)
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
                    DisbursementCodeList disbursementCodeList = new DisbursementCodeList();
                    return View(disbursementCodeList);
                }
                else
                {
                    DisbursementViewModel disbursementViewModel = new DisbursementViewModel();
                    var resp = await _disbursementCodeListService.FindByIdAsync(ID).ConfigureAwait(false);
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
                            disbursementViewModel.DisbursementCodeList = (DisbursementCodeList)result2.Value;
                        }
                    }
                    return View(disbursementViewModel.DisbursementCodeList);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementController.Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "DisbursementController.Delete Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.Disbursement.Delete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DisbursementCodeListDelete(long id, [Bind("ID")] DisbursementCodeList disbursementCodeList)
        {
            try
            {
                bool success = false;
                string msg = "Delete Failed";
                if (id != disbursementCodeList.ID)
                {
                    return NotFound();
                }
                var resp = await _disbursementCodeListService.RemoveAsync(id).ConfigureAwait(false);
                if (resp.Success)
                {
                    success = true;
                    msg = "Disbursement Deleted";
                }
                return Json(new
                {
                    Success = success,
                    Message = msg,
                    Href = Url.Action("DisbursementCodeListIndex", "Disbursement")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementController.Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "DisbursementController.Delete Page has reloaded");
                return View(disbursementCodeList);
            }
        }


        #endregion
    }
}
