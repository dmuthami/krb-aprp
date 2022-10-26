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
using System.Globalization;

namespace APRP.Web.Controllers
{
    [Authorize]
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutAttribute))]
    public class ReleasesController : Controller
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
        private readonly IReleaseService _releaseService;
        private readonly IDisbursementReleaseService _disbursementReleaseService;

        #region Private Variables
        private Dictionary<long, double> dictTranches = new Dictionary<long, double>();
        private double TotalAnnualCeiling = 0d;
        private double TotalDisbursement = 0d;
        private double TotalPercent = 0d;
        #endregion

        public ReleasesController(ILogger<ReleasesController> logger,
            IApplicationUsersService applicationUsersService, IAuthorityService authorityService,
            IDisbursementService disbursementService, IFinancialYearService financialYearService,
            IDisbursementTrancheService disbursementTrancheService, IAllocationCodeUnitService allocationCodeUnitService,
            IBudgetCeilingService budgetCeilingService, IMemoryCache cache, IBudgetCeilingComputationService budgetCeilingComputationService,
            IDisbursementCodeListService disbursementCodeListService, IReleaseService releaseService,
            IDisbursementReleaseService disbursementReleaseService)
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
            _releaseService = releaseService;
            _disbursementReleaseService = disbursementReleaseService;

        }

        #region Views Section
        // GET: ReleaseController
        [Authorize(Claims.Permission.Release.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Index(long? FinancialYearId)
        {
            try
            {
                ReleaseViewModel releaseViewModel = new ReleaseViewModel();

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
                    releaseViewModel.FinancialYear = respFinancialYear.FinancialYear;
                    releaseViewModel.FinancialYearId = respFinancialYear.FinancialYear.ID;

                    //var resp = await _releaseService.ListAsync(respFinancialYear.FinancialYear.ID).ConfigureAwait(false);
                    //releaseViewModel.Releases = resp.Disbursement;

                }


                //financial year drop down
                await IndexDropDowns(respFinancialYear.FinancialYear.ID).ConfigureAwait(false);

                releaseViewModel.Referer = Request.GetEncodedUrl();

                return View(releaseViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ReleaseController Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "ReleaseController Index Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.Release.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ReleasesActivities(long? FinancialYearId)
        {
            try
            {
                ReleaseViewModel releaseViewModel = new ReleaseViewModel();
                long _FinancialYearId = 0;
                bool result = long.TryParse(FinancialYearId.ToString(), out _FinancialYearId);
                //if FinancialYearId ==0 then return all releases
                if (_FinancialYearId == 0)
                {
                    var respFinancialYear = await _financialYearService.FindCurrentYear().ConfigureAwait(false);
                    if (respFinancialYear.Success)
                    {
                        //Set financial Year
                        releaseViewModel.FinancialYear = respFinancialYear.FinancialYear;

                        var resp = await _releaseService.ListAsync().ConfigureAwait(false);
                        releaseViewModel.Releases = resp.Release;
                    }
                }
                else
                {
                    var respFinancialYear = await _financialYearService.FindByIdAsync(_FinancialYearId).ConfigureAwait(false);
                    if (respFinancialYear.Success)
                    {
                        //Set financial Year
                        releaseViewModel.FinancialYear = respFinancialYear.FinancialYear;

                        var resp = await _releaseService.ListAsync(releaseViewModel.FinancialYear.ID).ConfigureAwait(false);
                        releaseViewModel.Releases = resp.Release;
                    }
                }

                releaseViewModel.FinancialYearId = _FinancialYearId;

                //Get disbursements for the current financial year
                var respDisbursement = await _disbursementService.ListAsync(releaseViewModel.FinancialYearId)
                    .ConfigureAwait(false);

                if (respDisbursement.Success)
                {
                    releaseViewModel.Disbursements = respDisbursement.Disbursement;
                }

                //financial year drop down
                await IndexDropDowns(_FinancialYearId).ConfigureAwait(false);

                releaseViewModel.Referer = Request.GetEncodedUrl();

                return PartialView("ReleasePartialView", releaseViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ReleaseController Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "ReleaseController Index Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.Release.Add), Authorize(Claims.Permission.Release.Change)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ReleaseAddEdit(int? id, long FinancialYearId)
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
                    ReleaseViewModel releaseViewModel = new ReleaseViewModel();
                    releaseViewModel.Release = new Release();
                    releaseViewModel.FinancialYearId = FinancialYearId;
                    //await PopulateDropDowns(0, 0, 0, 0).ConfigureAwait(false);

                    //Get all releases
                    var disbursementResp = await _disbursementService
                      .ListAsync()
                      .ConfigureAwait(false);

                    releaseViewModel.Disbursements = disbursementResp.Disbursement;
                    return View(releaseViewModel);
                }
                else
                {
                    ReleaseViewModel releaseViewModel = new ReleaseViewModel();
                    var resp = await _releaseService.FindByIdAsync(ID).ConfigureAwait(false);
                    releaseViewModel.Release = resp.Release;
                    if (releaseViewModel.Release == null)
                    {
                        return NotFound();
                    }
                    releaseViewModel.FinancialYearId = FinancialYearId;

                    //Get disbursements for my release=ID
                    IList<Disbursement> disbursements = null;
                    var respDisbursement = await _releaseService.ListDisbursementByReleaseAsync(releaseViewModel.Release).ConfigureAwait(false);
                    if (respDisbursement.Success)
                    {
                        var objectResult = (ObjectResult)respDisbursement.IActionResult;
                        if (objectResult != null)
                        {
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var result2 = (OkObjectResult)objectResult;
                                disbursements = (IList<Disbursement>)result2.Value;
                            }
                        }
                    }
                    releaseViewModel.ReleaseDisbursements = disbursements;

                    ////Get all releases for financial year
                    //if (FinancialYearId == 0)
                    //{
                    //    var disbursementResp = await _disbursementService
                    //      .ListAsync()
                    //      .ConfigureAwait(false);
                    //    releaseViewModel.Disbursements = disbursementResp.Disbursement;
                    //}
                    //else
                    //{
                    //    var disbursementResp = await _disbursementService
                    //      .ListAsync(FinancialYearId)
                    //      .ConfigureAwait(false);
                    //    releaseViewModel.Disbursements = disbursementResp.Disbursement;
                    //}

                    //Get all releases
                    var disbursementResp = await _disbursementService
                      .ListAsync()
                      .ConfigureAwait(false);
                    releaseViewModel.Disbursements = disbursementResp.Disbursement;

                    return View(releaseViewModel);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ReleaseController.ReleaseAddEdit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "ReleaseController.ReleaseAddEdit Page has reloaded");
                return View();
            }
        }

        // POST: Disbursement/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.Release.Add), Authorize(Claims.Permission.Release.Change)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ReleaseAddEdit(long FinancialYearId, [Bind("ID,ReleaseDate,ChequeNo," +
            "DetailsOrPayee,ReleaseAmount")] Release release)
        {
            try
            {
                bool success = false;
                string msg = null;

                if (ModelState.IsValid)
                {
                    ApplicationUser loggedInuser = await GetLoggedInUser().ConfigureAwait(false);
                    //if id=0 then create new
                    if (release.ID == 0)
                    {
                        var resp = await _releaseService.FindByReleaseEntryAsync(release).ConfigureAwait(false);
                        if (resp.Release != null)
                        {
                            string msgErr = $"A similar release entry exists" +
                            $" No duplicate entries may exists for the same release for the same authority" +
                            $" in the same financial year";
                            ModelState.AddModelError(string.Empty, msgErr);
                            //Detach the first entry before attaching your updated entry
                            var respDetach = await _releaseService.DetachFirstEntryAsync(resp.Release).ConfigureAwait(false);
                            //return RedirectToAction("AddEdit",new { id=disbursement.ID});
                            return Json(new
                            {
                                Success = false,
                                Message = msgErr,
                                Href = Url.Action("ReleaseAddEdit", "Releases", new { id = string.Empty, FinancialYearId= FinancialYearId })
                            });
                        }

                        //add disbursement
                        var disbursementResp = await _releaseService.AddAsync(release).ConfigureAwait(false);
                        if (disbursementResp.Success)
                        {
                            success = true;
                            msg = "Release Successfully Added";
                        }
                    }
                    //if id>0 then its an edit
                    if (release.ID > 0)
                    {
                        var releaseResp = await _releaseService.Update(release.ID, release).ConfigureAwait(false);
                        if (releaseResp.Success)
                        {
                            success = true;
                            msg = "Release Successfully Updated";
                        }
                    }

                    return Json(new
                    {
                        Success = success,
                        Message = msg,
                        Href = Url.Action("ReleaseAddEdit", "Releases", new { id = release.ID, FinancialYearId= FinancialYearId })
                    }); ;
                }
                return Json(new
                {
                    Success = false,
                    Message = "Model Error",
                    Href = Url.Action("Index", "Releases")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ReleaseController ReleaseAddEdit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "ReleaseController ReleaseAddEdit Page has reloaded");
                return View(release);
            }
        }

        // GET: ReleaseController/Details/5
        [Authorize(Claims.Permission.Release.View)]
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
                _logger.LogError(Ex, $"ReleaseController.Details Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "ReleaseController.Details Page has reloaded");
                return View();
            }
        }

        // GET: ReleaseController/Details/5
        [Authorize(Claims.Permission.Release.View)]
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
                _logger.LogError(Ex, $"ReleaseController.Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "ReleaseController.Delete Page has reloaded");
                return View();
            }
        }

        // GET: ReleaseController/Details/5
        [Authorize(Claims.Permission.Release.View)]
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
                    Release release = new Release();
                    return View(release);
                }
                else
                {
                    var resp = await _releaseService.FindByIdAsync(ID).ConfigureAwait(false);
                    var release = resp.Release;
                    if (release == null)
                    {
                        return NotFound();
                    }
                    return View(release);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ReleaseController.Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "ReleaseController.Delete Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.Release.Delete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Delete(long id, [Bind("ID")] Release release)
        {
            try
            {
                bool success = false;
                string msg = "Delete Failed";
                if (id != release.ID)
                {
                    return NotFound();
                }
                var resp = await _releaseService.RemoveAsync(id).ConfigureAwait(false);
                if (resp.Success)
                {
                    success = true;
                    msg = "Release Deleted";
                }
                return Json(new
                {
                    Success = success,
                    Message = msg,
                    Href = Url.Action("Index", "Releases")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ReleaseController.Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "ReleaseController.Delete Page has reloaded");
                return View(release);
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
            //long _FinancialYearId = 0;
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
            //Add for all with id 0
            newFinancialYearsList.Insert(0, new { ID = Convert.ToInt64(0), Code = "All" });

            ViewData["FinancialYearId"] = new SelectList(newFinancialYearsList, "ID", "Code", FinancialYearId);

            ViewData["_FinancialYearId"] = FinancialYearId;

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
                _logger.LogError(Ex, $"ReleaseController.GetDisbursementByFinancialYearIdYear Error {Environment.NewLine}");
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
        public async Task<JsonResult> ExportReleaseReport2(long FinancialYearId)
        {
            try
            {
                //Get the data
                ReleaseViewModel releaseViewModel = new ReleaseViewModel();

                //Get current financial year
                var respFinancialYear = await _financialYearService.FindByIdAsync(FinancialYearId).ConfigureAwait(false);
                if (respFinancialYear.Success)
                {
                    //Set financial Year
                    releaseViewModel.FinancialYear = respFinancialYear.FinancialYear;

                    var resp = await _releaseService.ListAsync(respFinancialYear.FinancialYear.ID).ConfigureAwait(false);
                    releaseViewModel.Releases = resp.Release;
                }

                if (releaseViewModel.Releases == null)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Failed",
                        Href = Url.Action("Index", "Disbursement")
                    });
                }

                //Write to Excel
                MemoryStream stream = new MemoryStream();
                JsonResult myjson = await WriteToRelease2(releaseViewModel, stream).ConfigureAwait(false);
                return Json(myjson);

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ReleaseController.ExportReleaseReport Error {Environment.NewLine}");
                return Json(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<JsonResult> WriteToRelease2(ReleaseViewModel releaseViewModel, MemoryStream stream)
        {

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
                        disbursementCodeLists = ((IList<DisbursementCodeList>)result.Value)
                            .Where(w => w.ReleaseOrder != null)
                            .OrderBy(x => x.ReleaseOrder).ToList();
                    }
                }
            }


            //Create a new ExcelPackage  
            string handle = Guid.NewGuid().ToString();
            using (ExcelPackage excelPackage = new ExcelPackage(stream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = "KRB";
                excelPackage.Workbook.Properties.Title = $"Bank Releases FY {releaseViewModel.FinancialYear.Code}";
                excelPackage.Workbook.Properties.Subject = $"Bank Releases FY {releaseViewModel.FinancialYear.Code} Report";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Detailed List");

                //Heading 1
                worksheet.Cells["A1"].Value = $"KENYA ROADS BOARD";
                worksheet.Cells["A1"].Style.Font.Size = 9;
                worksheet.Cells["A1"].Style.Font.Name = "Arial";
                worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells["A1"].Style.Font.Bold = true;

                //Heading 2
                worksheet.Cells["A2"].Value = $"RELEASES OF FUNDS IN FINANCIAL YEAR {releaseViewModel.FinancialYear.Code}";
                worksheet.Cells["A2"].Style.Font.Size = 9;
                worksheet.Cells["A2"].Style.Font.Name = "Arial";
                worksheet.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells["A2"].Style.Font.Bold = true;

                //worksheet.Cells["A2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //worksheet.Cells["A2"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#9CC2E5"));

                var arr = releaseViewModel.FinancialYear.Code.Split("/");

                //get start date
                var startDate = new DateTime(Convert.ToInt32(arr[0]), 7, 1, 0, 0, 0);

                //get end date
                var endDate = new DateTime(Convert.ToInt32(arr[1]), 6, 30, 23, 59, 59);

                //Heading 3
               
                    
                worksheet.Cells["A3"].Value = $"Period Covered: " +
                    $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(startDate.Month).ToUpper()} " +
                    $"{startDate.Year} " +
                    $"to  {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(endDate.Month).ToUpper()}" +
                    $" {endDate.Year}";
                worksheet.Cells["A3"].Style.Font.Size = 9;
                worksheet.Cells["A3"].Style.Font.Name = "Arial";
                worksheet.Cells["A3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells["A3"].Style.Font.Bold = true;
                //worksheet.Cells["B2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                //worksheet.Cells["B2"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#9CC2E5"));

                int i = 5;
                if (disbursementCodeLists != null)
                {
                    int k1 = 1;
                    foreach (var disbursementCodeList in disbursementCodeLists)
                    {
                        //Get the releases for each disbursementCodeList 
                        IEnumerable<Release> releases = null;
                        var resp = await _releaseService.ListAsync2(releaseViewModel.FinancialYear,
                            disbursementCodeList.Code).ConfigureAwait(false);
                        releases = resp.Release.OrderBy(c=>c.ChequeNo);

                        double totalReleases = 0d;
                        int unitNo = 1;
                        //loop through the releases and do something
                        int k = 0;
                        int j = 0;
                        foreach (var release in releases)
                        {
                            //Header row
                            if (k == 0)
                            {
                                //get authority
                                var budgetCeiResp = await _budgetCeilingComputationService
                                    .FindByCodeAndFinancialYearIdAsync(disbursementCodeList.Code, releaseViewModel.FinancialYear.ID).ConfigureAwait(false);
                                if (budgetCeiResp.Success)
                                {
                                    BudgetCeilingComputation budgetCeilingComputation = null;
                                    var objectResult = (ObjectResult)budgetCeiResp.IActionResult;
                                    if (objectResult != null)
                                    {
                                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            var result3 = (OkObjectResult)objectResult;
                                            budgetCeilingComputation = (BudgetCeilingComputation)result3.Value;

                                        }
                                    }
                                    if (budgetCeilingComputation != null)
                                    {
                                        worksheet.Cells[i, 1].Value = $"NOTE: {k + k1}" +
                                            $" {budgetCeilingComputation.Authority.Code}-{disbursementCodeList.DisplayName}";
                                    }
                                    else
                                    {
                                        worksheet.Cells[i, 1].Value = $"NOTE: {k + k1}" +
                                            $"  {disbursementCodeList.DisplayName}";
                                    }

                                }
                                else
                                {
                                    worksheet.Cells[i, 1].Value = $"NOTE: {k + k1} {disbursementCodeList.DisplayName}";
                                }

                                worksheet.Cells[i, 1].Style.Font.Size = 9;
                                worksheet.Cells[i, 1].Style.Font.Name = "Arial";
                                worksheet.Cells[i, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells[i, 1].Style.Font.Bold = true;
                                k++;
                                i++; j = i;
                                worksheet.Cells[i, 1].Value = $"UNIT";
                                worksheet.Cells[i, 2].Value = $"DATE";
                                worksheet.Cells[i, 3].Value = $"CHQ. NO.";
                                worksheet.Cells[i, 4].Value = $"DETAILS/PAYEE";
                                worksheet.Cells[i, 5].Value = $"AMOUNT (KSHS)";
                                worksheet.Cells[i, 6].Value = $"FY";

                                worksheet.Cells[$"A{i}:F{i}"].Style.Font.Size = 9;
                                worksheet.Cells[$"A{i}:F{i}"].Style.Font.Name = "Arial";
                                worksheet.Cells[$"A{i}:F{i}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[$"A{i}:F{i}"].Style.Font.Bold = true;
                                i++;
                            }

                            //other rows
                            worksheet.Cells[i, 1].Value = unitNo++;
                            worksheet.Cells[i, 2].Value = release.ReleaseDate;
                            worksheet.Cells[i, 2].Style.Numberformat.Format = "yyyy-mm-dd";
                            worksheet.Cells[i, 3].Value = release.ChequeNo;
                            worksheet.Cells[i, 4].Value = release.DetailsOrPayee;
                            worksheet.Cells[i, 5].Value = string.Format("{0:0,0.00}", release.ReleaseAmount);
                            totalReleases += release.ReleaseAmount;

                            //Financial Year
                            var resp1 = await _releaseService.FindDisbursementByReleaseAsync(release).ConfigureAwait(false);
                            if (resp1.Success)
                            {
                                //get Fincial year by id
                                var respFinancialYear = await _financialYearService.FindByIdAsync(resp1.Disbursement.FinancialYearId).ConfigureAwait(false);
                                if (respFinancialYear.Success)
                                {
                                    //Set financial Year
                                    worksheet.Cells[i, 6].Value = $"{respFinancialYear.FinancialYear.Code}";
                                }
                            }


                            //formating
                            worksheet.Cells[$"A{j}:F{i}"].Style.Font.Size = 9;
                            worksheet.Cells[$"A{j}:F{i}"].Style.Font.Name = "Arial";
                            worksheet.Cells[$"A{j}:F{i}"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[$"A{j}:F{i}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[$"A{j}:F{i}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[$"A{j}:F{i}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            i++;
                        }

                        //Show totals for the release
                        if (totalReleases > 0)
                        {
                            worksheet.Cells[i, 4].Value = "TOTAL";
                            worksheet.Cells[i, 5].Value = string.Format("{0:0,0.00}", totalReleases);
                            worksheet.Cells[$"D{i}:F{i}"].Style.Font.Name = "Arial";
                            worksheet.Cells[$"D{i}:F{i}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[$"D{i}:F{i}"].Style.Font.Bold = true;
                            worksheet.Cells[$"A{i}:E{i}"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[$"A{i}:E{i}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[$"A{i}:E{i}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[$"A{i}:E{i}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        }

                        i++; k1++;
                    }
                }
                //Auto fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                _cache.Set(handle, excelPackage.GetAsByteArray(),
                    new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));

                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Href = Url.Action("Download2", "Releases", new { fileGuid = handle, FileName = $"Bank Releases FY {releaseViewModel.FinancialYear.Code} Report.xlsx" })
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

        #region Add/Remove Disbursements to Releases

        [Authorize(Claims.Permission.Release.Add)]
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddDisbursementFromRelease(long ReleaseId, long DisbursementId, long FinancialYearId)
        {
            try
            {
                bool success = false;
                string msg = null;

                if (ModelState.IsValid)
                {
                    DisbursementRelease disbursementRelease = new DisbursementRelease();
                    disbursementRelease.ReleaseId = ReleaseId;
                    disbursementRelease.DisbursementId = DisbursementId;


                    var resp = await _disbursementReleaseService.AddAsync(disbursementRelease).ConfigureAwait(false);
                    if (resp.Success)
                    {
                        var objectResult = (ObjectResult)resp.IActionResult;
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            var disbursementReleaseCreated = (DisbursementRelease)result.Value;

                            success = true;
                            msg = $"Disbursement release ReleaseId:{disbursementReleaseCreated.ReleaseId}" +
                                $" DisbursementId:{disbursementReleaseCreated.DisbursementId} added successfully";
                        }
                        else
                        {
                            var result = (BadRequestObjectResult)objectResult;
                            var disbursementFailedMessage = (String)result.Value;

                            success = false;
                            msg = $"Disbursement release failed with message:{disbursementFailedMessage}";
                        }
                    }
                    else
                    {
                        success = false;
                        msg = $"Disbursement release failed with message:{resp.Message}";
                    }

                    return Json(new
                    {
                        Success = success,
                        Message = msg,
                        Href = Url.Action("ReleaseAddEdit", "Releases", new { id = ReleaseId, FinancialYearId = FinancialYearId })
                    }); ;
                }
                return Json(new
                {
                    Success = false,
                    Message = "Model Error",
                    Href = Url.Action("ReleaseAddEdit", "Releases", new { id = ReleaseId, FinancialYearId = FinancialYearId })
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ReleasesController Add/Edit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "ReleasesController Add/Edit Page has reloaded");
                return View("ReleaseAddEdit", new { id = ReleaseId, FinancialYearId = FinancialYearId });
            }
        }

        [Authorize(Claims.Permission.Release.Delete)]
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> RemoveDisbursementFromRelease(long ReleaseId, long DisbursementId, long FinancialYearId)
        {
            try
            {
                bool success = false;
                string msg = null;

                if (ModelState.IsValid)
                {
                    /*
                     * --------Perform Remove function---------------
                     */

                    var resp = await _disbursementReleaseService.RemoveAsync(ReleaseId, DisbursementId).ConfigureAwait(false);
                    if (resp.Success)
                    {
                        var objectResult = (ObjectResult)resp.IActionResult;
                        if (objectResult != null)
                        {
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var result = (OkObjectResult)objectResult;
                                DisbursementRelease disbursementRelease = (DisbursementRelease)result.Value;

                                success = true;
                                msg = $"Disbursement release ReleaseId:{ReleaseId}" +
                                    $" DisbursementId:{DisbursementId} removed successfully";
                            }
                            else
                            {
                                success = false;
                                msg = $"Disbursement release ReleaseId:{ReleaseId}" +
                                    $" DisbursementId:{DisbursementId} failed to be removed";
                            }
                        }
                        else
                        {
                            success = false;
                            msg = $"Disbursement release ReleaseId:{ReleaseId}" +
                                $" DisbursementId:{DisbursementId} failed to be removed";
                        }
                    }
                    else
                    {
                        success = false;
                        msg = $"Disbursement release failed with message:{resp.Message}";
                    }

                    /*
                     * ----------------End Perform Remove function----
                     */

                    return Json(new
                    {
                        Success = success,
                        Message = msg,
                        Href = Url.Action("ReleaseAddEdit", "Releases", new { id = ReleaseId, FinancialYearId = FinancialYearId })
                    });
                }
                return Json(new
                {
                    Success = false,
                    Message = "Model Error",
                    Href = Url.Action("ReleaseAddEdit", "Releases", new { id = ReleaseId, FinancialYearId = FinancialYearId })
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ReleasesController Add/Edit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "ReleasesController Add/Edit Page has reloaded");
                return View("ReleaseAddEdit", new { id = ReleaseId, FinancialYearId = FinancialYearId });
            }
        }

        #endregion
    }
}
