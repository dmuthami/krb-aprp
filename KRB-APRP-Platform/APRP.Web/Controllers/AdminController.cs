using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using APRP.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace APRP.Web.Controllers
{
    [Authorize]
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutAttribute))]
    public class AdminController : Controller
    {
        private readonly IFinancialYearService _financialYearService;
        private readonly IItemActivityGroupService _itemActivityGroupService;
        private readonly IItemActivityUnitCostService _itemActivityUnitCostService;
        private readonly IFundingSourceService _fundingSourceService;
        private readonly IFundTypeService _fundTypeService;
        private readonly IAuthorityService _authorityService;
        private readonly IRegionService _regionService;
        private readonly IConstituencyService _constituencyService;
        private readonly IContractorService _contractorService;
        private readonly IDirectorService _directorService;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly ILogger _logger;
        private readonly IFundingSourceSubCodeService _fundingSourceSubCodeService;
        private readonly IARICSYearService _aRICSYearService;

        public AdminController(IFinancialYearService financialYearService,
            IItemActivityGroupService itemActivityGroupService,
            IItemActivityUnitCostService itemActivityUnitCostService,
            IFundingSourceService fundingSourceService,
            IFundTypeService fundTypeService,
            IAuthorityService authorityService,
            IRegionService regionService,
            IConstituencyService constituencyService,
            IContractorService contractorService,
            IApplicationUsersService applicationUsersService,
            IDirectorService directorService,
            ILogger<AdminController> logger, IFundingSourceSubCodeService fundingSourceSubCodeService,
            IARICSYearService aRICSYearService)
        {
            _financialYearService = financialYearService;
            _itemActivityGroupService = itemActivityGroupService;
            _itemActivityUnitCostService = itemActivityUnitCostService;
            _fundingSourceService = fundingSourceService;
            _fundTypeService = fundTypeService;
            _authorityService = authorityService;
            _regionService = regionService;
            _constituencyService = constituencyService;
            _contractorService = contractorService;
            _applicationUsersService = applicationUsersService;
            _directorService = directorService;
            _logger = logger;
            _fundingSourceSubCodeService = fundingSourceSubCodeService;
            _aRICSYearService = aRICSYearService;
        }

        #region Financial Year
        [Authorize(Claims.Permission.FinancialYear.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var viewModel = await _financialYearService.ListAsync().ConfigureAwait(false);
                return View("financial_planning", viewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController Index Page has reloaded");
                return View();
            }
        }


        [Authorize(Claims.Permission.FinancialYear.View)]
        // GET: FinancialYears/Details/5
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FinancialYearDetails(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                var resp = await _financialYearService.FindByIdAsync(ID).ConfigureAwait(false);
                var financialYear = resp.FinancialYear;
                if (financialYear == null)
                {
                    return NotFound();
                }

                return View(financialYear);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController.FinancialYearDetails Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController.FinancialYearDetails Page has reloaded");
                return View();
            }
        }

        // GET: FinancialYears/Create
        [Authorize(Claims.Permission.FinancialYear.View)]
        public async Task<IActionResult> FinancialYearCreate()
        {
            await PopulateDropDowns(0).ConfigureAwait(false);

            return View();
        }

        // POST: FinancialYears/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.FinancialYear.Add)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FinancialYearCreate([Bind("ID,Code,Summary,IsCurrent,Revision,ARICSYearId")] FinancialYear financialYear)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var resp = await _financialYearService.AddAsync(financialYear).ConfigureAwait(false);
                    if (!resp.Success)
                    {
                        ModelState.AddModelError(string.Empty, $"{resp.Message}");
                        int ARICSYearId = 0;
                        bool result2 = int.TryParse(financialYear.ARICSYearId.ToString(), out ARICSYearId);
                        await PopulateDropDowns(ARICSYearId).ConfigureAwait(false);
                        return View(financialYear);
                    }
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            ModelState.AddModelError(string.Empty, $"{error.ErrorMessage}");
                        }
                    }
                    int ARICSYearId = 0;
                    bool result2 = int.TryParse(financialYear.ARICSYearId.ToString(), out ARICSYearId);
                    await PopulateDropDowns(ARICSYearId).ConfigureAwait(false);
                    return View(financialYear);
                }

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController.FinancialYearCreate Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController.FinancialYearCreate Create Page has reloaded");
                int ARICSYearId = 0;
                bool result2 = int.TryParse(financialYear.ARICSYearId.ToString(), out ARICSYearId);
                await PopulateDropDowns(ARICSYearId).ConfigureAwait(false);
                return View(financialYear);
            }
        }


        // GET: FinancialYears/Edit/5
        [Authorize(Claims.Permission.FinancialYear.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FinancialYearEdit(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                var resp = await _financialYearService.FindByIdAsync(ID).ConfigureAwait(false);
                var financialYear = resp.FinancialYear;
                if (financialYear == null)
                {
                    return NotFound();
                }
                int ARICSYearId = 0;
                if (financialYear.ARICSYearId != null)
                {

                    bool result2 = int.TryParse(id.ToString(), out ARICSYearId);
                    await PopulateDropDowns(ARICSYearId).ConfigureAwait(false);
                }
                else
                {
                    await PopulateDropDowns(ARICSYearId).ConfigureAwait(false);
                }


                return View(financialYear);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"AdminController.FinancialYearEdit Page Error: {Ex.Message} " +
               $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController.FinancialYearEdit Page has reloaded");
                return View();
            }
        }

        // POST: FinancialYears/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.FinancialYear.Change), Authorize(Claims.Permission.FinancialYear.View)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FinancialYearEdit(long ID, [Bind("ID,Code,Summary,IsCurrent,Revision,ARICSYearId")] FinancialYear financialYear)
        {
            try
            {
                if (ID != financialYear.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var resp = await _financialYearService.Update(ID, financialYear).ConfigureAwait(false);
                    return RedirectToAction(nameof(Index));
                }
                return View(financialYear);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminControllers.FinancialYearEdit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminControllers.FinancialYearEdit Page has reloaded");
                return View(financialYear);
            }
        }


        // GET: FinancialYears/Delete/5
        [Authorize(Claims.Permission.FinancialYear.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FinancialYearDelete(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                var resp = await _financialYearService.FindByIdAsync(ID).ConfigureAwait(false);
                var financialYear = resp.FinancialYear;
                if (financialYear == null)
                {
                    return NotFound();
                }

                return View(financialYear);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"AdminController.FinancialYearDelete Page Error: {Ex.Message} " +
               $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController.FinancialYearDelete Page has reloaded");
                return View("Index");
            }
        }

        // POST: FinancialYears/Delete/5
        [Authorize(Claims.Permission.FinancialYear.Delete)]
        [HttpPost, ActionName("FinancialYearDelete")]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FinancialYearDeleteConfirmed(long id)
        {
            try
            {
                var resp = await _financialYearService.FindByIdAsync(id).ConfigureAwait(false);
                var financialYear = resp.FinancialYear;
                if (financialYear == null)
                {
                    return NotFound();
                }
                resp = await _financialYearService.RemoveAsync(id).ConfigureAwait(false);
                if (!resp.Success)
                {
                    ModelState.AddModelError(string.Empty, resp.Message);
                    return View(financialYear);
                }
                return RedirectToAction("Index");
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController.FinancialYearDeleteConfirmed Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController.FinancialYearDeleteConfirmed Page has reloaded");
                return View("Index");
            }
        }
        #endregion

        #region Activity Group
        [Authorize(Claims.Permission.ActivityGroup.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> UnitCost()
        {
            try
            {
                ItemActivityGroupViewModel itemActivityGroupViewModel = new ItemActivityGroupViewModel();
                //Set Return URL and store in session
                string url;
                if (HttpContext.Session.GetString("RefererToARICSSummary") == null)
                {
                    url = Request.Headers["Referer"].ToString(); /*Request.GetEncodedUrl();*/
                    HttpContext.Session.SetString("RefererToARICSSummary", url);
                }

                itemActivityGroupViewModel.Referer = HttpContext.Session.GetString("RefererToARICSSummary");


                return View("unit_cost", itemActivityGroupViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController Index Page has reloaded");
                return View();
            }
        }

        // GET: ItemActivityGroups/Details/5
        [Authorize(Claims.Permission.ActivityGroup.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> UnitCostDetails(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                var resp = await _itemActivityGroupService.FindByIdAsync(ID).ConfigureAwait(false);
                var itemActivityGroup = resp.ItemActivityGroup;
                if (itemActivityGroup == null)
                {
                    return NotFound();
                }

                return View(itemActivityGroup);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"AdminController Details Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController Details Page has reloaded");
                return View();
            }
        }

        // GET: ItemActivityGroups/Create
        [Authorize(Claims.Permission.ActivityGroup.View)]
        public IActionResult UnitCostCreate()
        {
            return View();
        }

        // POST: ItemActivityGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.ActivityGroup.Add)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> UnitCostCreate([Bind("ID,BillNumber,Description")] ItemActivityGroup itemActivityGroup)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var resp = await _itemActivityGroupService.AddAsync(itemActivityGroup).ConfigureAwait(false);
                    return RedirectToAction("UnitCost");
                }
                return View(itemActivityGroup);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"AdminController Create Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController Create Page has reloaded");
                return View(itemActivityGroup);
            }
        }

        // GET: ItemActivityGroups/Edit/5
        [Authorize(Claims.Permission.ActivityGroup.View)]
        public async Task<IActionResult> UnitCostEdit(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                var resp = await _itemActivityGroupService.FindByIdAsync(ID).ConfigureAwait(false);
                var itemActivityGroup = resp.ItemActivityGroup;
                if (itemActivityGroup == null)
                {
                    return NotFound();
                }
                return View(itemActivityGroup);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController Edit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController Edit Page has reloaded");
                return View("UnitCost"); throw;
            }
        }

        // POST: ItemActivityGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.ActivityGroup.Change)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> UnitCostEdit(long id, [Bind("ID,BillNumber,Description")] ItemActivityGroup itemActivityGroup)
        {
            try
            {
                if (id != itemActivityGroup.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var resp = await _itemActivityGroupService.Update(id, itemActivityGroup).ConfigureAwait(false);
                    return RedirectToAction("UnitCost");
                }
                return View(itemActivityGroup);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController Page has reloaded");
                return View(itemActivityGroup);
            }
        }

        // GET: ItemActivityGroups/Delete/5
        [Authorize(Claims.Permission.ActivityGroup.View)]
        public async Task<IActionResult> UnitCostDelete(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                var resp = await _itemActivityGroupService.FindByIdAsync(ID).ConfigureAwait(false);
                var itemActivityGroup = resp.ItemActivityGroup;
                if (itemActivityGroup == null)
                {
                    return NotFound();
                }
                return View(itemActivityGroup);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController Delete Page has reloaded");
                return View("UnitCost");
            }
        }

        // POST: ItemActivityGroups/Delete/5
        [Authorize(Claims.Permission.ActivityGroup.Delete)]
        [HttpPost, ActionName("UnitCostDelete")]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> UnitCostDeleteConfirmed(long id)
        {
            try
            {
                var resp = await _itemActivityGroupService.FindByIdAsync(id).ConfigureAwait(false);
                if (!resp.Success)
                {
                    return NotFound();
                }
                resp = await _itemActivityGroupService.RemoveAsync(id).ConfigureAwait(false);
                return RedirectToAction("UnitCost");
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController Delete Page has reloaded");
                return View();
            }
        }

        #endregion

        #region ItemActivityUnitCost
        [Authorize(Claims.Permission.ActivityItem.View)]
        public async Task<IActionResult> ActivityList(long groupId)
        {
            var viewModel = await _itemActivityUnitCostService.ListAsync().ConfigureAwait(false);
            ItemActivityGroupViewModel itemActivityGroupViewModel = new ItemActivityGroupViewModel();
            itemActivityGroupViewModel.ItemActivityUnitCostList = viewModel.Where(g => g.ItemActivityGroupId == groupId);
            itemActivityGroupViewModel.ItemActivityGroupId = groupId;
            return PartialView("ItemActivityListPartialView", itemActivityGroupViewModel);
        }

        public async Task<IActionResult> ActivityGroupList()
        {
            ItemActivityGroupViewModel itemActivityGroupViewModel = new ItemActivityGroupViewModel();
            itemActivityGroupViewModel.ItemActivityGroupList = await _itemActivityGroupService.ListAsync().ConfigureAwait(false); ;

            return PartialView("ItemActivityGroupListPartialView", itemActivityGroupViewModel);
        }


        // GET: ItemActivityUnitCosts/Details/5
        [Authorize(Claims.Permission.ActivityItem.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ActivityListDetails(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                var resp = await _itemActivityUnitCostService.FindByIdAsync(ID).ConfigureAwait(false);
                var itemActivityUnitCost = resp.ItemActivityUnitCost;
                if (itemActivityUnitCost == null)
                {
                    return NotFound();
                }

                return View(itemActivityUnitCost);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"AdminController Details Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController Details Page has reloaded");
                return View("Unitcost");
            }
        }


        // GET: ItemActivityUnitCosts/Create
        [Authorize(Claims.Permission.ActivityItem.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ActivityListCreate(long ItemActivityGroupId)
        {
            try
            {
                var resp = await _itemActivityGroupService.ListAsync().ConfigureAwait(false);
                ViewData["ItemActivityGroupId"] = new SelectList(resp, "ID", "Description", ItemActivityGroupId);
                return View();
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController Create Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController Create Page has reloaded");
                return View("Unitcost");
            }
        }

        // POST: ItemActivityUnitCosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.ActivityItem.Add)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ActivityListCreate([Bind("ID,ItemCode,SubItemCode,SubSubItemCode,Description,OverheadRoutineImprovement,UnitCode,UnitMeasure,UnitDescription,PlannedCost,ItemActivityGroupId")] ItemActivityUnitCost itemActivityUnitCost)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var resp = await _itemActivityUnitCostService.AddAsync(itemActivityUnitCost).ConfigureAwait(false);
                    return RedirectToAction("Unitcost");
                }
                var resp2 = await _itemActivityGroupService.ListAsync().ConfigureAwait(false);
                ViewData["ItemActivityGroupId"] = new SelectList(resp2, "ID", "BillNumber", itemActivityUnitCost.ItemActivityGroupId);

                return View("Unitcost");
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController Create Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController Create Page has reloaded");
                return View("Unitcost");
            }
        }


        // GET: ItemActivityUnitCosts/Edit/5
        [Authorize(Claims.Permission.ActivityItem.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ActivityListEdit(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                var resp = await _itemActivityUnitCostService.FindByIdAsync(ID).ConfigureAwait(false);
                var itemActivityUnitCost = resp.ItemActivityUnitCost;
                if (itemActivityUnitCost == null)
                {
                    return NotFound();
                }
                var resp2 = await _itemActivityGroupService.ListAsync().ConfigureAwait(false);
                ViewData["ItemActivityGroupId"] = new SelectList(resp2, "ID", "BillNumber", itemActivityUnitCost.ItemActivityGroupId);
                return View(itemActivityUnitCost);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController Edit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController Edit Page has reloaded");
                return View("Unitcost");
            }
        }

        // POST: ItemActivityUnitCosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.ActivityItem.Change)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ActivityListEdit(long id, [Bind("ID,ItemCode,SubItemCode,SubSubItemCode,Description,OverheadRoutineImprovement,UnitCode,UnitMeasure,UnitDescription,PlannedCost,ItemActivityGroupId")] ItemActivityUnitCost itemActivityUnitCost)
        {
            try
            {
                if (id != itemActivityUnitCost.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var resp = await _itemActivityUnitCostService.Update(id, itemActivityUnitCost).ConfigureAwait(false);
                    return RedirectToAction(nameof(Index));
                }
                var resp2 = await _itemActivityGroupService.ListAsync().ConfigureAwait(false);
                ViewData["ItemActivityGroupId"] = new SelectList(resp2, "ID", "BillNumber", itemActivityUnitCost.ItemActivityGroupId);

                return View("UnitCost");
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController Edit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController Edit Page has reloaded");
                return View("UnitCost");
            }
        }


        // GET: ItemActivityUnitCosts/Delete/5
        [Authorize(Claims.Permission.ActivityItem.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ActivityListDelete(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                int ID;
                bool result = int.TryParse(id.ToString(), out ID);
                var resp = await _itemActivityUnitCostService.FindByIdAsync(ID).ConfigureAwait(false);
                var itemActivityUnitCost = resp.ItemActivityUnitCost;
                if (itemActivityUnitCost == null)
                {
                    return NotFound();
                }

                return View(itemActivityUnitCost);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController Delete Page has reloaded");
                return View("Unitcost");
            }
        }

        // POST: ItemActivityUnitCosts/Delete/5
        [Authorize(Claims.Permission.ActivityItem.Delete)]
        [HttpPost, ActionName("ActivityListDelete")]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ActivityListDeleteConfirmed(long id)
        {
            try
            {
                var resp = await _itemActivityUnitCostService.FindByIdAsync(id).ConfigureAwait(false);
                if (!resp.Success)
                {
                    return NotFound();
                }
                resp = await _itemActivityUnitCostService.RemoveAsync(id).ConfigureAwait(false);
                return RedirectToAction("Unitcost");
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController Delete Page has reloaded");
                return View("Unitcost");
            }
        }

        #endregion

        #region Funding Source
        [Authorize(Claims.Permission.FundingSource.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FundingSources(long? groupId)
        {
            try
            {
                FundingSourceViewModel fundingSourceViewModel = new FundingSourceViewModel();
                //Set Return URL and store in session
                string url;
                if (HttpContext.Session.GetString("RefererToARICSSummary") == null)
                {
                    url = Request.Headers["Referer"].ToString(); /*Request.GetEncodedUrl();*/
                    HttpContext.Session.SetString("RefererToARICSSummary", url);
                }
                //if (groupId != null)
                //{ }
                fundingSourceViewModel.Referer = HttpContext.Session.GetString("RefererToARICSSummary");
                long _groupId;
                bool result = long.TryParse(groupId.ToString(), out _groupId);
                fundingSourceViewModel.FundingSourceId = _groupId;

                return View("funding_sources", fundingSourceViewModel);

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController.FundingSources() Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController.FundingSources() Page has reloaded");
                return View();
            }
        }

        public async Task<IActionResult> FundingSourcesList()
        {
            FundingSourceViewModel fundingSourceViewModel = new FundingSourceViewModel();

            fundingSourceViewModel.FundingSourceList = await _fundingSourceService.ListAsync().ConfigureAwait(false);
            return PartialView("FundingSourcePartialView", fundingSourceViewModel);
        }

        [Authorize(Claims.Permission.FundingSource.View)]
        // GET: FundingSources/Details/5
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]

        public async Task<IActionResult> FundingSourcesDetails(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                var resp = await _fundingSourceService.FindByIdAsync(ID).ConfigureAwait(false);
                var fundingSource = resp.FundingSource;
                if (fundingSource == null)
                {
                    return NotFound();
                }

                return View(fundingSource);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"AdminController.FundingSources() Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController.FundingSources() has reloaded");
                return View("FundingSources");
            }
        }

        // GET: FundingSources/Create
        [Authorize(Claims.Permission.FundingSource.View)]
        public IActionResult FundingSourcesCreate()
        {
            return View();
        }

        // POST: FundingSources/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.FundingSource.Add)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FundingSourcesCreate([Bind("ID,Code,Name,ShortName")] FundingSource fundingSource)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var resp = await _fundingSourceService.AddAsync(fundingSource).ConfigureAwait(false);
                    return RedirectToAction("FundingSources");
                }
                return View(fundingSource);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController.FundingSourcesCreate() Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController.FundingSourcesCreate() Page has reloaded");
                return View(fundingSource);
            }
        }

        [Authorize(Claims.Permission.FundingSource.View)]
        // GET: FundingSources/Edit/5
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FundingSourcesEdit(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                var resp = await _fundingSourceService.FindByIdAsync(ID).ConfigureAwait(false);
                var fundingSource = resp.FundingSource;
                if (fundingSource == null)
                {
                    return NotFound();
                }

                return View(fundingSource);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"AdminController.FundingSourcesEdit() Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController.FundingSourcesEdit() has reloaded");
                return View("FundingSources");
            }
        }

        // POST: FundingSources/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.FundingSource.Change)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FundingSourcesEdit(long id, [Bind("ID,Code,Name,ShortName")] FundingSource fundingSource)
        {
            try
            {
                if (id != fundingSource.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var resp = await _fundingSourceService.Update(id, fundingSource).ConfigureAwait(false);
                    return RedirectToAction("FundingSources");
                }
                return View(fundingSource);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"Road Class Code Units Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "Road Class Code Page has reloaded");
                return View("FundingSourcesEdit");
            }
        }

        [Authorize(Claims.Permission.FundingSource.View)]
        // GET: FundingSources/Delete/5
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FundingSourcesDelete(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                var resp = await _fundingSourceService.FindByIdAsync(ID).ConfigureAwait(false);
                var fundingSource = resp.FundingSource;
                if (fundingSource == null)
                {
                    return NotFound();
                }

                return View(fundingSource);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"AdminController.FundingSourcesDelete() Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController.FundingSourcesDelete() has reloaded");
                return View("FundingSources");
            }
        }

        [Authorize(Claims.Permission.FundingSource.Delete)]
        // POST: FundingSources/Delete/5
        [HttpPost, ActionName("FundingSourcesDelete")]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FundingSourcesDeleteConfirmed(long id)
        {
            try
            {
                var resp = await _fundingSourceService.FindByIdAsync(id).ConfigureAwait(false);
                if (!resp.Success)
                {
                    return NotFound();
                }
                resp = await _fundingSourceService.RemoveAsync(id).ConfigureAwait(false);
                return RedirectToAction("FundingSources");
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController.FundingSourcesDeleteConfirmed() Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController.FundingSourcesDeleteConfirmed() Page has reloaded");
                return View();
            }
        }

        #region Funding sources sub codes

        public async Task<IActionResult> ShowFundingSourcesSubCodesList(long groupId)
        {
            FundingSourceViewModel fundingSourceViewModel = new FundingSourceViewModel();
            //Funding sources Sub codes
            IList<FundingSourceSubCode> fundingSourceSubCodes = null;
            var fundingSourcesubCodeResp = await _fundingSourceSubCodeService.ListAsync().ConfigureAwait(false);
            if (fundingSourcesubCodeResp.Success)
            {
                var objectResult = (ObjectResult)fundingSourcesubCodeResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        fundingSourceSubCodes = (IList<FundingSourceSubCode>)result.Value;
                    }
                }
            }
            if (fundingSourceSubCodes != null)
            {
                fundingSourceViewModel.FundingSourceSubCodeList = fundingSourceSubCodes.Where(g => g.FundingSourceId == groupId);
            }
            fundingSourceViewModel.FundingSourceId = groupId;
            return PartialView("FundingSourceSubCodePartialView", fundingSourceViewModel);
        }

        public async Task FundingSourcesSubCodesDropDown(long FundingSourcesId)
        {
            //Funding sources dropdown
            var fundingSourcesubCodeResp = await _fundingSourceService.ListAsync().ConfigureAwait(false);
            IList<FundingSource> financialYears = (IList<FundingSource>)fundingSourcesubCodeResp;
            var newFundingSourcesList = financialYears
                .OrderByDescending(v => v.Code)
                .Select(
                p => new
                {
                    ID = p.ID,
                    Code = $"{p.Code}-{p.ShortName}"
                }
                    ).ToList();


            if (FundingSourcesId == 0)
            {
                ViewData["FundingSourceId"] = new SelectList(newFundingSourcesList, "ID", "Code");
            }
            else
            {
                ViewData["FundingSourceId"] = new SelectList(newFundingSourcesList, "ID", "Code", FundingSourcesId);
            }

        }

        // GET: FundingSourcesSubCode/Create
        [Authorize(Claims.Permission.FundingSource.View)]
        public async Task<IActionResult> FundingSourcesSubCodeCreate()
        {
            await FundingSourcesSubCodesDropDown(0).ConfigureAwait(false);
            return View();
        }

        // POST: FundingSourcesSubCode/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.FundingSource.Add)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FundingSourcesSubCodeCreate([Bind("ID,Code,Name,ShortName,FundingSourceId")] FundingSourceSubCode fundingSourceSubCode)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var resp = await _fundingSourceSubCodeService.AddAsync(fundingSourceSubCode).ConfigureAwait(false);
                    return Redirect("/Admin/FundingSources?groupId=" + fundingSourceSubCode.FundingSourceId);
                }
                return View(fundingSourceSubCode);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController.FundingSourcesCreate() Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                await FundingSourcesSubCodesDropDown(0).ConfigureAwait(false);
                ModelState.AddModelError(string.Empty, "AdminController.FundingSourcesSubCodeCreate() Page has reloaded");
                return View(fundingSourceSubCode);
            }
        }

        [Authorize(Claims.Permission.FundingSource.View)]
        // GET: FundingSourcesSubCode/Edit/5
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FundingSourcesSubCodeEdit(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                FundingSourceSubCode fundingSourceSubCode = null;
                var resp = await _fundingSourceSubCodeService.FindByIdAsync(ID).ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result2 = (OkObjectResult)objectResult;
                            fundingSourceSubCode = (FundingSourceSubCode)result2.Value;
                        }
                    }
                }

                if (fundingSourceSubCode == null)
                {
                    return NotFound();
                }
                await FundingSourcesSubCodesDropDown(fundingSourceSubCode.FundingSourceId).ConfigureAwait(false);
                return View(fundingSourceSubCode);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"AdminController.FundingSourcesSubCodeEdit() Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController.FundingSourcesSubCodeEdit() has reloaded");
                return View("FundingSources");
            }
        }

        // POST: FundingSourcesSubCode/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.FundingSource.Change)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FundingSourcesSubCodeEdit(long id, [Bind("ID,Code,Name,ShortName,FundingSourceId")] FundingSourceSubCode fundingSourceSubCode)
        {
            try
            {
                if (id != fundingSourceSubCode.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var resp = await _fundingSourceSubCodeService.Update(id, fundingSourceSubCode).ConfigureAwait(false);
                    return RedirectToAction("FundingSources");
                }
                return View(fundingSourceSubCode);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"Road Class Code Units Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                await FundingSourcesSubCodesDropDown(fundingSourceSubCode.FundingSourceId).ConfigureAwait(false);
                ModelState.AddModelError(string.Empty, "Road Class Code Page has reloaded");
                return View("FundingSourcesSubCodeEdit");
            }
        }

        [Authorize(Claims.Permission.FundingSource.View)]
        // GET: FundingSourcesSubCode/Details/5
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]

        public async Task<IActionResult> FundingSourcesSubCodeDetails(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                FundingSourceSubCode fundingSourceSubCode = null;
                var resp = await _fundingSourceSubCodeService.FindByIdAsync(ID).ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result2 = (OkObjectResult)objectResult;
                            fundingSourceSubCode = (FundingSourceSubCode)result2.Value;
                        }
                    }
                }
                if (fundingSourceSubCode == null)
                {
                    return NotFound();
                }

                return View(fundingSourceSubCode);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController.FundingSources() Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController.FundingSources() has reloaded");
                return View("FundingSources");
            }
        }

        [Authorize(Claims.Permission.FundingSource.View)]
        // GET: FundingSourcesSubCode/Delete/5
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FundingSourcesSubCodeDelete(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                FundingSourceSubCode fundingSourceSubCode = null;
                var resp = await _fundingSourceSubCodeService.FindByIdAsync(ID).ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result2 = (OkObjectResult)objectResult;
                            fundingSourceSubCode = (FundingSourceSubCode)result2.Value;
                        }
                    }
                }
                if (fundingSourceSubCode == null)
                {
                    return NotFound();
                }

                return View(fundingSourceSubCode);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"AdminController.FundingSourcesDelete() Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController.FundingSourcesDelete() has reloaded");
                return View("FundingSources");
            }
        }

        [Authorize(Claims.Permission.FundingSource.Delete)]
        // POST: FundingSourcesSubCode/Delete/5
        [HttpPost, ActionName("FundingSourcesSubCodeDelete")]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FundingSourcesSubCodeDeleteConfirmed(long id)
        {
            try
            {
                var resp = await _fundingSourceSubCodeService.FindByIdAsync(id).ConfigureAwait(false);
                if (!resp.Success)
                {
                    return NotFound();
                }
                FundingSourceSubCode fundingSourceSubCode = null;
                var objectResult = (ObjectResult)resp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        fundingSourceSubCode = (FundingSourceSubCode)result.Value;
                    }
                }
                resp = await _fundingSourceSubCodeService.RemoveAsync(id).ConfigureAwait(false);
                return Redirect("/Admin/FundingSources?groupId=" + fundingSourceSubCode.FundingSourceId);

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController.FundingSourcesSubCodeDeleteConfirmed() Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController.FundingSourcesSubCodeDeleteConfirmed() Page has reloaded");
                return View();
            }
        }

        #endregion
        #endregion

        #region Fund Type
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.FundType.View)]
        public async Task<IActionResult> FundTypes()
        {
            try
            {
                var viewModel = await _fundTypeService.ListAsync().ConfigureAwait(false);
                return View("fund_types", viewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController.FundTypes()  Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController.FundTypes() Page has reloaded");
                return View();
            }
        }


        // GET: FundTypes/Details/5
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.FundType.View)]
        public async Task<IActionResult> FundTypesDetails(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                var resp = await _fundTypeService.FindByIdAsync(ID).ConfigureAwait(false);
                var fundType = resp.FundType;
                if (fundType == null)
                {
                    return NotFound();
                }

                return View(fundType);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController.FundTypesDetails()  Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController.FundTypesDetails() Page has reloaded");
                return View();
            }
        }

        // GET: FundTypes/Create
        [Authorize(Claims.Permission.FundType.View)]
        public IActionResult FundTypesCreate()
        {
            return View();
        }

        // POST: FundTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.FundType.Add)]
        public async Task<IActionResult> FundTypesCreate([Bind("ID,Code,Name")] FundType fundType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var resp = await _fundTypeService.AddAsync(fundType).ConfigureAwait(false);
                    return RedirectToAction("FundTypes");
                }
                return View(fundType);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController.FundTypesCreate()  Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController.FundTypesCreate() Page has reloaded");
                return View("FundTypes");
            }
        }


        // GET: FundTypes/Edit/5
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.FundType.View)]
        public async Task<IActionResult> FundTypesEdit(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                var resp = await _fundTypeService.FindByIdAsync(ID).ConfigureAwait(false);
                var fundType = resp.FundType;
                if (fundType == null)
                {
                    return NotFound();
                }

                return View(fundType);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController.FundTypesEdit()  Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController.FundTypesEdit() Page has reloaded");
                return View("FundTypes");
            }
        }

        // POST: FundTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.FundType.Change)]
        public async Task<IActionResult> FundTypesEdit(long id, [Bind("ID,Code,Name")] FundType fundType)
        {
            try
            {
                if (id != fundType.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var resp = await _fundTypeService.Update(id, fundType).ConfigureAwait(false);
                    return RedirectToAction("FundTypes");
                }
                return View(fundType);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController.FundTypesEdit()  Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController.FundTypesEdit() Page has reloaded");
                return View("FundTypes");
            }
        }


        // GET: FundTypes/Delete/5
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.FundType.View)]
        public async Task<IActionResult> FundTypesDelete(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                var resp = await _fundTypeService.FindByIdAsync(ID).ConfigureAwait(false);
                var fundType = resp.FundType;
                if (fundType == null)
                {
                    return NotFound();
                }

                return View(fundType);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController.FundTypesDelete()  Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController.FundTypesDelete() Page has reloaded");
                return View("FundTypes");
            }
        }

        // POST: FundTypes/Delete/5
        [Authorize(Claims.Permission.FundType.Delete)]
        [HttpPost, ActionName("FundTypesDelete")]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FundTypesDeleteConfirmed(long id)
        {
            try
            {
                var resp = await _fundTypeService.FindByIdAsync(id).ConfigureAwait(false);
                if (!resp.Success)
                {
                    return NotFound();
                }
                resp = await _fundTypeService.RemoveAsync(id).ConfigureAwait(false);
                return RedirectToAction("FundTypes");
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController.FundTypesDelete() Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController.FundTypesDelete() Page has reloaded");
                return View("FundTypes");
            }
        }
        #endregion

        #region Authorities
        [Authorize(Claims.Permission.Authority.View)]
        public async Task<IActionResult> Authorities()
        {
            var viewModel = new AuthorityListViewModel();

            viewModel.Authorities = await _authorityService.ListAsync().ConfigureAwait(false);

            return View("authorities", viewModel);
        }


        // GET: Authorities/Details/5
        [Authorize(Claims.Permission.Authority.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AuthoritiesDetails(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                var authority = await _authorityService.FindByIdAsync(ID).ConfigureAwait(false);

                if (authority == null)
                {
                    return NotFound();
                }

                return View(authority.Authority);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController.AuthoritiesDetails Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController.AuthoritiesDetails Page has reloaded");
                return View("Authorities");
            }
        }

        // GET: Authorities/Create
        [Authorize(Claims.Permission.Authority.View)]
        public IActionResult AuthoritiesCreate()
        {
            return View();
        }

        // POST: Authorities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.Authority.Add)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AuthoritiesCreate([Bind("ID,Code,Name,Type,Number,Geom")] Authority authority)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var resp = await _authorityService.AddAsync(authority).ConfigureAwait(false);
                    return RedirectToAction("Authorities");
                }
                return View(authority);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController.AuthoritiesCreate  Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController.AuthoritiesCreate  Page has reloaded");
                return View("AuthoritiesCreate");
            }
        }


        // GET: Authorities/Edit/5
        [Authorize(Claims.Permission.Authority.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AuthoritiesEdit(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                var authority = await _authorityService.FindByIdAsync(ID).ConfigureAwait(false);

                if (authority == null)
                {
                    return NotFound();
                }

                return View(authority.Authority);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController.AuthoritiesEdit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController.AuthoritiesEdit Page has reloaded");
                return View("Authorities");
            }
        }

        // POST: Authorities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.Authority.Change)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AuthoritiesEdit(long id, [Bind("ID,Code,Name,Type,Number,Geom")] Authority authority)
        {
            try
            {
                if (id != authority.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var resp = await _authorityService.Update(id, authority).ConfigureAwait(false);
                    return RedirectToAction("Authorities");
                }
                return View(authority);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController.AuthoritiesEdit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController.AuthoritiesEdit Page has reloaded");
                return View("Authorities");
            }
        }


        // GET: Authorities/Delete/5
        [Authorize(Claims.Permission.Authority.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AuthoritiesDelete(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                var authority = await _authorityService.FindByIdAsync(ID).ConfigureAwait(false);

                if (authority == null)
                {
                    return NotFound();
                }

                return View(authority.Authority);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController.AuthoritiesDelete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController.AuthoritiesDelete Page has reloaded");
                return View("Authorities");
            }
        }

        // POST: Authorities/Delete/5
        [Authorize(Claims.Permission.Authority.Delete)]
        [HttpPost, ActionName("AuthoritiesDelete")]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AuthoritiesDeleteConfirmed(long id)
        {
            try
            {
                var authority = await _authorityService.FindByIdAsync(id).ConfigureAwait(false);
                if (authority == null)
                {
                    return NotFound();
                }
                var resp = await _authorityService.RemoveAsync(id).ConfigureAwait(false);
                return RedirectToAction("Authorities");
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"AdminController.AuthoritiesDelete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "AdminController.AuthoritiesDelete Page has reloaded");
                return View("Authorities");
            }
        }
        #endregion

        #region Contractors
        [Authorize(Claims.Permission.Contractor.View)]
        public async Task<IActionResult> Contractors(long ID)
        {
            var viewModel = new ContractorViewModel();


            viewModel.Contractors = await _contractorService.ListAsync().ConfigureAwait(false);
            if (ID > 0)
            {
                var contractorResp = await _contractorService.FindByIdAsync(ID).ConfigureAwait(false);
                if (contractorResp.Success)
                {
                    viewModel.Contractor = contractorResp.Contractor;
                }
            }
            else
            {
                viewModel.Contractor = new Contractor();
            }
            return View("contractors", viewModel);
        }

        [Authorize(Claims.Permission.Contractor.Save)]
        [HttpPost]
        public async Task<IActionResult> SaveContractor(Contractor contractor)
        {
            //check if contractor is null
            if (contractor != null)
            {
                var user = await GetLoggedInUser().ConfigureAwait(false);
                if (user != null)
                {
                    var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                }
                if (contractor.ID > 0)
                {
                    //edit record
                    var existingRecord = await _contractorService.FindByIdAsync(contractor.ID).ConfigureAwait(false);
                    if (existingRecord.Success)
                    {
                        //update the existing record
                        existingRecord.Contractor.Name = contractor.Name;
                        existingRecord.Contractor.KRAPin = contractor.KRAPin;
                        existingRecord.Contractor.Email = contractor.Email;
                        existingRecord.Contractor.POBox = contractor.POBox;
                        existingRecord.Contractor.Telephone = contractor.Telephone;
                        existingRecord.Contractor.Town = contractor.Town;
                        existingRecord.Contractor.BankAccountNumber = contractor.BankAccountNumber;
                        existingRecord.Contractor.BankBranchCode = contractor.BankBranchCode;
                        existingRecord.Contractor.BankBranchName = contractor.BankBranchName;
                        existingRecord.Contractor.BankName = contractor.BankName;

                        existingRecord.Contractor.UpdateBy = user.UserName;
                        existingRecord.Contractor.UpdateDate = DateTime.UtcNow;

                        var updateResp = await _contractorService.UpdateAsync(existingRecord.Contractor).ConfigureAwait(false);
                        if (updateResp.Success)
                        {
                            return Json(new
                            {
                                Success = true,
                                Message = "Success",
                                Href = Url.Action("Contractors", "Admin", new { ID = contractor.ID })
                            });

                        }
                        else
                        {
                            return Json(new
                            {
                                Success = false,
                                Message = "Error : Could not update the record, please contact administrator"
                            });
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Error : Edit record not found, please contact administrator"
                        });
                    }
                }
                else
                {
                    // new record, insert
                    var saveResp = await _contractorService.AddAsync(contractor).ConfigureAwait(false);
                    if (saveResp.Success)
                    {
                        return Json(new
                        {
                            Success = true,
                            Message = "Success",
                            Href = Url.Action("Contractors", "Admin", new { ID = contractor.ID })
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Error : Could not add the record, please contact administrator"
                        });
                    }
                }

            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "Error : Could not save the record, please contact administrator"
                });
            }
        }

        public async Task<IActionResult> ShowContractorAddEdit(long contractorId)
        {
            if (contractorId > 0)
            {
                //edit record
                var existingRecordResp = await _contractorService.FindByIdAsync(contractorId).ConfigureAwait(false);
                if (existingRecordResp.Success)
                {
                    var viewModel = existingRecordResp.Contractor;
                    return PartialView("ContractorPartialView", viewModel);
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "No record found with the supplied contractor, please contact administrator"
                    });
                }
            }
            else
            {
                //new record
                return PartialView("ContractorPartialView", new Contractor());
            }
        }
        #endregion

        #region Director
        [Authorize(Claims.Permission.Contractor.View)]
        public async Task<IActionResult> ShowDirectorAddEdit(long directorId, long contractorId)
        {
            if (directorId > 0)
            {
                //edit record
                var existingRecordResp = await _directorService.FindByIdAsync(directorId).ConfigureAwait(false);
                if (existingRecordResp.Success)
                {
                    var viewModel = existingRecordResp.Director;
                    return PartialView("DirectorPartialView", viewModel);
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "No record found with the supplied contractor, please contact administrator"
                    });
                }
            }
            else
            {
                //new record
                var viewwModel = new Director();
                viewwModel.ContractorId = contractorId;
                return PartialView("DirectorPartialView", viewwModel);
            }
        }

        [Authorize(Claims.Permission.Contractor.Save)]
        [HttpPost]
        public async Task<IActionResult> SaveDirector(Director director)
        {
            //check if contractor is null
            if (director != null)
            {
                var user = await GetLoggedInUser().ConfigureAwait(false);

                if (director.ID > 0)
                {
                    //edit record
                    var existingRecord = await _directorService.FindByIdAsync(director.ID).ConfigureAwait(false);
                    if (existingRecord.Success)
                    {
                        //update the existing record
                        existingRecord.Director.IdCardNumber = director.IdCardNumber;
                        existingRecord.Director.LastName = director.LastName;
                        existingRecord.Director.MiddleName = director.MiddleName;
                        existingRecord.Director.FirstName = director.FirstName;
                        existingRecord.Director.Gender = director.Gender;
                        existingRecord.Director.CreationDate = DateTime.UtcNow;
                        existingRecord.Director.CreatedBy = user.UserName;

                        var updateResp = await _directorService.UpdateAsync(existingRecord.Director).ConfigureAwait(false);
                        if (updateResp.Success)
                        {
                            return Json(new
                            {
                                Success = true,
                                Message = "Success",
                                Href = Url.Action("Contractors", "Admin", new { ID = director.ContractorId })
                            });

                        }
                        else
                        {
                            return Json(new
                            {
                                Success = false,
                                Message = "Error : Could not update the record, please contact administrator"
                            });
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Error : Edit record not found, please contact administrator"
                        });
                    }
                }
                else
                {
                    // new record, insert
                    var saveResp = await _directorService.AddAsync(director).ConfigureAwait(false);
                    if (saveResp.Success)
                    {
                        return Json(new
                        {
                            Success = true,
                            Message = "Success",
                            Href = Url.Action("Contractors", "Admin", new { ID = director.ContractorId })
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            Message = "Error : Could not add the record, please contact administrator"
                        });
                    }
                }

            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "Error : Could not save the record, please contact administrator"
                });
            }
        }

        #endregion

        #region User Management
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
                }
            }
            return user;
        }
        #endregion

        #region Utilities
        public async Task<IActionResult> ConstituencyList(long? regionId)
        {
            var viewModel = await _constituencyService.ListAsync().ConfigureAwait(false);

            return View("ConstituencyListPartialView", viewModel.ConstituencyList);
        }

        private async Task PopulateDropDowns(int ARICSYearId)
        {
            //ARICS Year
            IList<ARICSYear> aRICSYears = null;
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
            ViewData["ARICSYearId"] = new SelectList(aRICSYears, "ID", "Year", ARICSYearId);

            //Revision list

            var deptList = new List<SelectListItem>();
            deptList.Add(new SelectListItem
            {
                Text = "Please Select",
                Value = ""
            });
            foreach (Revision eVal in Enum.GetValues(typeof(Revision)))
            {
                deptList.Add(new SelectListItem { Text = Enum.GetName(typeof(Revision), eVal), Value = eVal.ToString() });
            }
            ViewBag.Revision = deptList;
        }

        #endregion
    }
}