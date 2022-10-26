using Microsoft.AspNetCore.Mvc;
using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using APRP.Web.Domain.Services;

namespace APRP.Web.Controllers
{
    [Authorize]
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutAttribute))]
    public class RoadClassCodeUnitsController : Controller
    {
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly ILogger _logger;
        private readonly IRoadClassCodeUnitService _roadClassCodeUnitService;

        public RoadClassCodeUnitsController(
             IApplicationUsersService applicationUsersService, IAuthorityService authorityService,
             ILogger<RevenueCollectionController> logger, IRoadClassCodeUnitService roadClassCodeUnitService)
        {
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _logger = logger;
            _roadClassCodeUnitService = roadClassCodeUnitService;
        }


        // GET: RoadClassCodeUnits
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RoadClassCodeUnit.View)]
        public async Task<IActionResult> Index()
        {
            try
            {
                var roadClassCodeUnitResp = await _roadClassCodeUnitService.ListAsync().ConfigureAwait(false);
                return View(roadClassCodeUnitResp.RoadClassCodeUnit);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"Road Class Code Units Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "Road Class Code Page has reloaded");
                return View();
            }
        }


        // GET: RoadClassCodeUnits/Details/5
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RoadClassCodeUnit.View)]
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
                var roadClassCodeUnitResp = await _roadClassCodeUnitService.FindByIdAsync(ID).ConfigureAwait(false);
                var roadClassCodeUnit = roadClassCodeUnitResp.RoadClassCodeUnit;

                if (roadClassCodeUnit == null)
                {
                    return NotFound();
                }

                return View(roadClassCodeUnit);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"Road Class Code Units Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "Road Class Code Page has reloaded");
                return View();
            }
        }

        // GET: RoadClassCodeUnits/Create
        [Authorize(Claims.Permission.RoadClassCodeUnit.View)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: RoadClassCodeUnits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RoadClassCodeUnit.Add)]
        public async Task<IActionResult> Create([Bind("ID,RoadClass")] RoadClassCodeUnit roadClassCodeUnit)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var roadClassCodeUnitResp = await _roadClassCodeUnitService.AddAsync(roadClassCodeUnit).ConfigureAwait(false);
                    return RedirectToAction(nameof(Index));
                }
                return View(roadClassCodeUnit);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"Road Class Code Units Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "Road Class Code Page has reloaded");
                return View(roadClassCodeUnit);
            }
        }


        // GET: RoadClassCodeUnits/Edit/5
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RoadClassCodeUnit.View)]
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
                var roadClassCodeUnitResp = await _roadClassCodeUnitService.FindByIdAsync(ID).ConfigureAwait(false);
                if (roadClassCodeUnitResp.RoadClassCodeUnit == null)
                {
                    return NotFound();
                }
                return View(roadClassCodeUnitResp.RoadClassCodeUnit);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"Road Class Code Units Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "Road Class Code Page has reloaded");
                return View();
            }
        }

        // POST: RoadClassCodeUnits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RoadClassCodeUnit.Change)]
        public async Task<IActionResult> Edit(int id, [Bind("ID,RoadClass")] RoadClassCodeUnit roadClassCodeUnit)
        {
            try
            {
                if (id != roadClassCodeUnit.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var roadClassCodeUnitResp = await _roadClassCodeUnitService.Update(id, roadClassCodeUnit).ConfigureAwait(false);
                    return RedirectToAction(nameof(Index));
                }
                return View(roadClassCodeUnit);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"Road Class Code Units Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "Road Class Code Page has reloaded");
                return View(roadClassCodeUnit);
            }
        }

        // GET: RoadClassCodeUnits/Delete/5
        [Authorize(Claims.Permission.RoadClassCodeUnit.View)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            int ID;
            bool result = int.TryParse(id.ToString(), out ID);
            var roadClassCodeUnitResp = await _roadClassCodeUnitService.FindByIdAsync(ID).ConfigureAwait(false);
            if (roadClassCodeUnitResp.RoadClassCodeUnit == null)
            {
                return NotFound();
            }

            return View(roadClassCodeUnitResp.RoadClassCodeUnit);
        }

        // POST: RoadClassCodeUnits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RoadClassCodeUnit.Delete)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var roadClassCodeUnitResp = await _roadClassCodeUnitService.FindByIdAsync(id).ConfigureAwait(false);
                //var roadClassCodeUnit = await _context.RoadClassCodeUnits.FindAsync(id);

                if (!roadClassCodeUnitResp.Success)
                {
                    return NotFound();
                }

                roadClassCodeUnitResp = await _roadClassCodeUnitService.RemoveAsync(id).ConfigureAwait(false);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"Road Class Code Units Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "Road Class Code Page has reloaded");
                return View();
            }
        }
    }
}
