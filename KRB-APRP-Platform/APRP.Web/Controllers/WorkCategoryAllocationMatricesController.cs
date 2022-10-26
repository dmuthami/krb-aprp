using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using APRP.Web.Domain.Models;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Authorization;
using APRP.Web.Domain.Services;
using APRP.Web.ViewModels;

namespace APRP.Web.Controllers
{
    [Authorize]
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutAttribute))]
    public class WorkCategoryAllocationMatricesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;
        private readonly IWorkCategoryAllocationMatrixService _workCategoryAllocationMatrixService;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly IFinancialYearService _financialYearService;
        private readonly IWorkCategoryService _workCategoryService;

        public WorkCategoryAllocationMatricesController(AppDbContext context,
            IWorkCategoryAllocationMatrixService workCategoryAllocationMatrixService,
            ILogger<WorkCategoryAllocationMatricesController> logger,
             IApplicationUsersService applicationUsersService, IAuthorityService authorityService,
             IFinancialYearService financialYearService, IWorkCategoryService workCategoryService)
        {
            _context = context;
            _workCategoryAllocationMatrixService = workCategoryAllocationMatrixService;
            _logger = logger;
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _financialYearService = financialYearService;
            _workCategoryService = workCategoryService;

        }

        #region Ajax
        [HttpPost]
        [Authorize(Roles = "Administrators,RevenueCollection.ApproveBudget")]
        public async Task<IActionResult> GetWorkCategories(long AuthorityID, long FinancialYearId)
        {
            IEnumerable<WorkCategoryAllocationMatrixViewModel> data = Enumerable.Empty<WorkCategoryAllocationMatrixViewModel>();
            //retrieve the currentyear budgetHeader
            var resp = await _workCategoryAllocationMatrixService.GetAuthorityWorkCategoriesAsync(AuthorityID, FinancialYearId).ConfigureAwait(false);
            if (resp.Success)
            {

                 data = resp.WorkCategoryAllocationMatrixViewModel;

            }
            return Json(new
            {
                Success = true,
                Message = "Success",
                data=data,
                Href = Url.Action("Index", "RevenueCollection")
            });
        }
        #endregion
        // GET: WorkCategoryAllocationMatrices
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var workCategoryallocationMatrixResponse = await _workCategoryAllocationMatrixService.ListAsync().ConfigureAwait(false);
                return View(workCategoryallocationMatrixResponse.WorkCategoryAllocationMatrix);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"Work Category allocation Matrices Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "Work Category allocation Matrices Index  has reloaded");
                return View();
            }
        }


        // GET: WorkCategoryAllocationMatrices/Details/5
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                int ID;
                bool result = int.TryParse(id.ToString(), out ID);
                var workCategoryallocationMatrixResponse = await _workCategoryAllocationMatrixService.FindByIdAsync(ID).ConfigureAwait(false);
                var workCategoryAllocationMatrix = workCategoryallocationMatrixResponse.WorkCategoryAllocationMatrix;
                if (workCategoryAllocationMatrix == null)
                {
                    return NotFound();
                }

                return View(workCategoryAllocationMatrix);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"Work Category allocation Matrices Details Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "Work Category allocation Matrices Details  has reloaded");
                return View();
            }
        }


        // GET: WorkCategoryAllocationMatrices/Create
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Create()
        {
            try
            {
                var authorityServiceResp = await _authorityService.ListAsync().ConfigureAwait(false);
                ViewData["AuthorityId"] = new SelectList(authorityServiceResp, "ID", "Code");

                var financialYearResp = await _financialYearService.ListAsync().ConfigureAwait(false);
                ViewData["FinancialYearId"] = new SelectList(financialYearResp, "ID", "Code");

                var workCategoryResp = await _workCategoryService.ListAsync().ConfigureAwait(false);
                ViewData["WorkCategoryId"] = new SelectList(workCategoryResp, "ID", "Code");

                return View();
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"Work Category allocation Matrices Create Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "Work Category allocation Matrices Create  has reloaded");
                return View();
            }
        }

        // POST: WorkCategoryAllocationMatrices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Create([Bind("ID,AuthorityId,FinancialYearId,Percent,WorkCategoryId")] WorkCategoryAllocationMatrix workCategoryAllocationMatrix)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var workCategoryAllocationMatrixResp = await _workCategoryAllocationMatrixService.FindByAuthorityAndFinancialIdAsync(workCategoryAllocationMatrix.AuthorityId, workCategoryAllocationMatrix.FinancialYearId,
                        workCategoryAllocationMatrix.WorkCategoryId).ConfigureAwait(false);
                    if (workCategoryAllocationMatrixResp.Success)
                    {
                        var authorityServiceResp2 = await _authorityService.ListAsync().ConfigureAwait(false);
                        ViewData["AuthorityId"] = new SelectList(authorityServiceResp2, "ID", "Code");

                        var financialYearResp2 = await _financialYearService.ListAsync().ConfigureAwait(false);
                        ViewData["FinancialYearId"] = new SelectList(financialYearResp2, "ID", "Code");

                        var workCategoryResp2 = await _workCategoryService.ListAsync().ConfigureAwait(false);
                        ViewData["WorkCategoryId"] = new SelectList(workCategoryResp2, "ID", "Code");

                        string msg = "Work Category Allocation Matrix item has an existing item with a similar name";
                        _logger.LogError(msg, $"Roles.Index Page Error: {msg} " +
                        $"{Environment.NewLine}");
                        ModelState.AddModelError(string.Empty, msg);
                        return View();
                    }
                    else
                    {
                        workCategoryAllocationMatrixResp = await _workCategoryAllocationMatrixService.AddAsync(workCategoryAllocationMatrix).ConfigureAwait(false);
                    }


                    return RedirectToAction(nameof(Index));
                }
                var authorityServiceResp = await _authorityService.ListAsync().ConfigureAwait(false);
                ViewData["AuthorityId"] = new SelectList(authorityServiceResp, "ID", "Code");

                var financialYearResp = await _financialYearService.ListAsync().ConfigureAwait(false);
                ViewData["FinancialYearId"] = new SelectList(financialYearResp, "ID", "Code");

                var workCategoryResp = await _workCategoryService.ListAsync().ConfigureAwait(false);
                ViewData["WorkCategoryId"] = new SelectList(workCategoryResp, "ID", "Code");

                return View(workCategoryAllocationMatrix);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"Work Category allocation Matrices Create Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "Work Category allocation Matrices Create  has reloaded");
                return View();
            }
        }


        // GET: WorkCategoryAllocationMatrices/Edit/5
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                int ID;
                bool result = int.TryParse(id.ToString(), out ID);
                var workCategoryAllocationMatrixResp = await _workCategoryAllocationMatrixService.FindByIdAsync(ID).ConfigureAwait(false);
                var workCategoryAllocationMatrix = workCategoryAllocationMatrixResp.WorkCategoryAllocationMatrix;
                if (workCategoryAllocationMatrix == null)
                {
                    return NotFound();
                }
                var authorityServiceResp = await _authorityService.ListAsync().ConfigureAwait(false);
                ViewData["AuthorityId"] = new SelectList(authorityServiceResp, "ID", "Code", workCategoryAllocationMatrix.WorkCategoryId);

                var financialYearResp = await _financialYearService.ListAsync().ConfigureAwait(false);
                ViewData["FinancialYearId"] = new SelectList(financialYearResp, "ID", "Code", workCategoryAllocationMatrix.WorkCategoryId);

                var workCategoryResp = await _workCategoryService.ListAsync().ConfigureAwait(false);
                ViewData["WorkCategoryId"] = new SelectList(workCategoryResp, "ID", "Code", workCategoryAllocationMatrix.WorkCategoryId);

                return View(workCategoryAllocationMatrix);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"Work Category allocation Matrices Create Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "Work Category allocation Matrices Create  has reloaded");
                return View();
            }
        }

        // POST: WorkCategoryAllocationMatrices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,AuthorityId,FinancialYearId,Percent,WorkCategoryId")] WorkCategoryAllocationMatrix workCategoryAllocationMatrix)
        {
            try
            {
                if (id != workCategoryAllocationMatrix.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var workCategoryAllocationMatrixResp = await _workCategoryAllocationMatrixService.Update(id,workCategoryAllocationMatrix).ConfigureAwait(false);
                    return RedirectToAction(nameof(Index));
                }
                var authorityServiceResp = await _authorityService.ListAsync().ConfigureAwait(false);
                ViewData["AuthorityId"] = new SelectList(authorityServiceResp, "ID", "Code", workCategoryAllocationMatrix.WorkCategoryId);

                var financialYearResp = await _financialYearService.ListAsync().ConfigureAwait(false);
                ViewData["FinancialYearId"] = new SelectList(financialYearResp, "ID", "Code", workCategoryAllocationMatrix.WorkCategoryId);

                var workCategoryResp = await _workCategoryService.ListAsync().ConfigureAwait(false);
                ViewData["WorkCategoryId"] = new SelectList(workCategoryResp, "ID", "Code", workCategoryAllocationMatrix.WorkCategoryId);
                return View(workCategoryAllocationMatrix);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"Work Category allocation Matrices Create Edit Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "Work Category allocation Matrices Edit  has reloaded");
                return View();
            }
        }


        // GET: WorkCategoryAllocationMatrices/Delete/5
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                int ID;
                bool result = int.TryParse(id.ToString(), out ID);
                var workCategoryAllocationMatrixResp = await _workCategoryAllocationMatrixService.FindByIdAsync(ID).ConfigureAwait(false);
                var workCategoryAllocationMatrix = workCategoryAllocationMatrixResp.WorkCategoryAllocationMatrix;
                if (workCategoryAllocationMatrix == null)
                {
                    return NotFound();
                }
                return View(workCategoryAllocationMatrix);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"Work Category allocation Matrices Create Edit Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "Work Category allocation Matrices Edit  has reloaded");
                return View();
            }
        }

        // POST: WorkCategoryAllocationMatrices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var workCategoryAllocationMatrixResp = await _workCategoryAllocationMatrixService.FindByIdAsync(id).ConfigureAwait(false);
                if (!workCategoryAllocationMatrixResp.Success)
                {
                    return NotFound();
                }
                workCategoryAllocationMatrixResp = await _workCategoryAllocationMatrixService.RemoveAsync(id).ConfigureAwait(false);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"Work Category Allocation Matrices Create Edit Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "Work Category Allocation Matrices Edit  has reloaded");
                return View();
            }
        }

        #region Utility Operations
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

                    if (user != null && user.Authority == null)
                    {
                        var authorityResp = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                        user.Authority = authorityResp.Authority;
                    }
                }
            }
            return user;
        }
        #endregion


    }
}
