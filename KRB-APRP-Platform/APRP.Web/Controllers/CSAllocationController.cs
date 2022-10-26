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
    public class CSAllocationController : Controller
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
        private readonly ICSAllocationService _cSAllocationService;


        public CSAllocationController(ILogger<CSAllocationController> logger,
            IApplicationUsersService applicationUsersService, IAuthorityService authorityService,
            IDisbursementService disbursementService, IFinancialYearService financialYearService,
            IDisbursementTrancheService disbursementTrancheService, IAllocationCodeUnitService allocationCodeUnitService,
            IBudgetCeilingService budgetCeilingService, IMemoryCache cache, IBudgetCeilingComputationService budgetCeilingComputationService,
            ICSAllocationService cSAllocationService)
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
            _cSAllocationService = cSAllocationService;
        }

        #region Views Section
        // GET: CSAllocationController
        [Authorize(Claims.Permission.CSAllocation.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        
        public async Task<IActionResult> Index(long? FinancialYearId)
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

                return View(cSAllocationViewModel);
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

        // POST: GIS/Edit/5
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
                                Href = Url.Action("AddEdit", "CSAllocation", new { id=string.Empty})
                            });                     
                        }

                        //add disbursement
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
                        Href = Url.Action("Index", "CSAllocation")
                    }); ;
                }
                return Json(new
                {
                    Success = false,
                    Message = "Model Error",
                    Href = Url.Action("Index", "CSAllocation")
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

        // GET: CSAllocationController/Details/5
        [Authorize(Claims.Permission.CSAllocation.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Details(long ID)
        {
            try
            {
                CSAllocationViewModel cSAllocationViewModel = new CSAllocationViewModel();
                var resp = await _cSAllocationService.FindByIdAsync(ID).ConfigureAwait(false);
                var cSallocation = resp.CSAllocation;
                if (cSallocation == null)
                {
                    return NotFound();
                }
                cSAllocationViewModel.CSAllocation = cSallocation;
                return View(cSAllocationViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"CSAllocationController.Details Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "CSAllocationController.Details Page has reloaded");
                return View();
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

            //Authority "RA" ==Road Agencies
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
            var budgetCeilingComputationResp = await _budgetCeilingComputationService.FindByCodeAndFinancialYearIdAsync("20",_FinancialYearId).ConfigureAwait(false);
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
                _logger.LogError(Ex, $"CSAllocationController.GetDisbursementByFinancialYearIdYear Error {Environment.NewLine}");
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

        #region Reports
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
                                //disbursementViewModel.DisbursementSummaryByBudgetCeilingViewModels = await ReadAnonymousTypeForReport(anon, disbursementViewModel).ConfigureAwait(false);
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
                                //disbursementViewModel.DisbursementSummaryViewModels = await ReadAnonymousType(anon).ConfigureAwait(false);
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
                _logger.LogError(Ex, $"CSAllocationController.ExportDisbursementReport Error {Environment.NewLine}");
                return Json(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<JsonResult> WriteToDisbursement(DisbursementViewModel disbursementViewModel, MemoryStream stream)
        {
            //create dictionary for tranches
            Dictionary<long, double> dictTranches = new Dictionary<long, double>();
            double TotalAnnualCeiling = 0d;
            double TotalDisbursement = 0d;

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
                i = ComputeAuthoritySubTotals(worksheet, disbursementViewModel, disbursementTrancheList, disbursements, columnIndex, i, ref TotalAnnualCeiling, ref TotalDisbursement);

                //Road/Agency
                worksheet.Cells[i, 2].Value = "Total";

                //Total
                worksheet.Cells[i, 4].Value = string.Format("{0:0,0.00}", TotalAnnualCeiling);
                worksheet.Cells[i, 4].Style.Font.Size = 11;
                worksheet.Cells[i, 4].Style.Font.Name = "Arial";
                worksheet.Cells[i, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[i, 4].Style.Font.Bold = true;


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
                            _logger.LogError(Ex, $"CSAllocationController.WriteToDisbursements Error {Environment.NewLine}");
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
                worksheet.Cells[i, columnIndex + 1].Value = string.Format("{0:0,0.00}", TotalDisbursement);
                worksheet.Cells[i, columnIndex + 1].Style.Font.Size = 11;
                worksheet.Cells[i, columnIndex + 1].Style.Font.Name = "Arial";
                worksheet.Cells[i, columnIndex + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[i, columnIndex + 1].Style.Font.Bold = true;




                // ColumnNames.Add(worksheet.Cells[1, i].Value.ToString()); // 1 = First Row, i = Column Number
                string extremecol = ExcelCellBase.GetAddress(1, columnIndex + 1);
                string column = extremecol.Substring(0, 1);

                worksheet.Cells[$"A2:{column}{i}"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[$"A2:{column}{i}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[$"A2:{column}{i}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[$"A2:{column}{i}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

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
        private int ComputeAuthoritySubTotals(ExcelWorksheet worksheet, DisbursementViewModel disbursementViewModel, IList<DisbursementTranche> disbursementTrancheList,
            IEnumerable<Disbursement> disbursements, int columnIndex, int i, ref double TotalAnnualCeiling, ref double TotalDisbursement)
        {
            int trancheColumnID = 4;
            int newTrancheColumnID = 0;
            bool result = false;
            Dictionary<long, double> dictAnnualCeilingAuthorityTotals = new Dictionary<long, double>();
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

                            worksheet.Cells[i, 2].Value = $"Sub-Total {authoritysummary.Authority.Name}";

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
                                            //dictTranches[disbursementTrancheList2.ID] = dictTranches[disbursementTrancheList2.ID] + amt;
                                        }
                                        catch (Exception Ex)
                                        {
                                            _logger.LogError(Ex, $"CSAllocationController.WriteToDisbursements Error {Environment.NewLine}");
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
                    worksheet.Cells[i, 2].Value = $"{rptItem.Authority.Name}-{rptItem.BudgetCeilingComputation.Name}";

                    //%                       
                    worksheet.Cells[i, 3].Value = string.Format("{0:0.0%}", rptItem.Percent);

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
                                    worksheet.Cells[i, newTrancheColumnID].Value = string.Format("{0:0,0.00}", disburmentx.Sum(x => x.Amount));
                                    //dictTranches[disbursementTrancheList2.ID] = dictTranches[disbursementTrancheList2.ID] + disburmentx.Amount;
                                }
                                catch (Exception Ex)
                                {
                                    _logger.LogError(Ex, $"CSAllocationController.WriteToDisbursements Error {Environment.NewLine}");
                                }
                            }
                        }
                    }

                    //Total disbursement
                    worksheet.Cells[i, columnIndex].Value = string.Format("{0:0,0.00}", rptItem.TotalDisbursement);
                    TotalDisbursement = TotalDisbursement + rptItem.TotalDisbursement;

                    //% of ceiling
                    worksheet.Cells[i, columnIndex + 1].Value = string.Format("{0:0,0.00000%}", rptItem.PercentOfCeiling);

                    AuthorityIdPrev = rptItem.AuthorityId;
                    disbursementSummaryViewModelFinal = rptItem;
                    i++;
                }
            }

            //-------------The final authority sub totals are not shown and thus the repeated code is added heare-----------------
            //Road Agency/ Recepient
            worksheet.Cells[i, 1].Value = i - 2;//Serial No
            worksheet.Cells[i, 2].Value = $"Sub Total {disbursementSummaryViewModelFinal.Authority.Name}";

            //%                       
            worksheet.Cells[i, 3].Value = string.Format("{0:0.0%}", disbursementSummaryViewModelFinal.Percent);

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
                            _logger.LogError(Ex, $"CSAllocationController.WriteToDisbursements Error {Environment.NewLine}");
                        }
                    }
                }
            }

            //Total disbursement
            worksheet.Cells[i, columnIndex].Value = string.Format("{0:0,0.00}", disbursementSummaryViewModelFinal.TotalDisbursement);
            TotalDisbursement = TotalDisbursement + disbursementSummaryViewModelFinal.TotalDisbursement;

            //% of ceiling
            worksheet.Cells[i, columnIndex + 1].Value = string.Format("{0:0,0.00000%}", disbursementSummaryViewModelFinal.PercentOfCeiling);
            string extremecol2 = ExcelCellBase.GetAddress(i, newTrancheColumnID + 2);
            worksheet.Cells[$"B{i}:{extremecol2.Substring(0, 1)}{i}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[$"B{i}:{extremecol2.Substring(0, 1)}{i}"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#BFBFBF"));
            i++;
            //-------------End------The final authority sub totals are not shown and thus the repeated code is added here-----------------

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

                _logger.LogError(Ex, $"CSAllocationController.CGWorkSheet Error {Environment.NewLine}");
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
                _logger.LogError(Ex, $"CSAllocationController.GetDisbursements Error {Environment.NewLine}");
            }
            return disbursements;
        }
        #endregion
    }
}
