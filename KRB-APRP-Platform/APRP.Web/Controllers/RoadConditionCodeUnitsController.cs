using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Authorization;

namespace APRP.Web.Controllers
{
    public class RoadConditionCodeUnitsController : Controller
    {
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly ILogger _logger;
        private readonly IRoadConditionCodeUnitService _roadConditionCodeUnitService;
        private readonly ISurfaceTypeService _surfaceTypeService;

        public RoadConditionCodeUnitsController(
            IApplicationUsersService applicationUsersService, IAuthorityService authorityService,
             ILogger<RevenueCollectionController> logger, IRoadConditionCodeUnitService roadConditionCodeUnitService,
             ISurfaceTypeService surfaceTypeService)
        {
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _logger = logger;
            _roadConditionCodeUnitService = roadConditionCodeUnitService;
            _surfaceTypeService = surfaceTypeService;
        }


        // GET: RoadConditionCodeUnits
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RoadConditionCodeUnit.View)]
        public async Task<IActionResult> Index()
        {
            try
            {
                var roadConditionCodeUnitResp = await _roadConditionCodeUnitService.ListAsync().ConfigureAwait(false);
                return View(roadConditionCodeUnitResp.RoadConditionCodeUnit);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"Road Condition Code Units Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "Road Condition Code Page has reloaded");
                return View();
            }
        }


        // GET: RoadConditionCodeUnits/Details/5
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RoadConditionCodeUnit.View)]
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
                var roadConditionCodeUnitResp = await _roadConditionCodeUnitService.FindByIdAsync(ID).ConfigureAwait(false);
                var roadConditionCodeUnit = roadConditionCodeUnitResp.RoadConditionCodeUnit;
                if (roadConditionCodeUnit == null)
                {
                    return NotFound();
                }

                return View(roadConditionCodeUnit);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"Road Condition Code Units Details Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "Road Condition Code Details Page has reloaded");
                return View();
            }
        }


        // GET: RoadConditionCodeUnits/Create
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RoadConditionCodeUnit.View)]
        public async Task<IActionResult> Create()
        {
            try
            {
                var surfaceTypeResp = await _surfaceTypeService.ListAsync().ConfigureAwait(false);
                ViewData["SurfaceTypeId"] = new SelectList(surfaceTypeResp.SurfaceType, "ID", "Code");
                return View();
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"Road Condition Code Units Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "Road Condition Code Page has reloaded");
                return View();
            }
        }

        // POST: RoadConditionCodeUnits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RoadConditionCodeUnit.Add)]
        public async Task<IActionResult> Create([Bind("ID,RoadCondition,Rate,ActivitiesRequired,SurfaceTypeId")] RoadConditionCodeUnit roadConditionCodeUnit)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var roadConditionCodeUnitResp = await _roadConditionCodeUnitService.AddAsync(roadConditionCodeUnit).ConfigureAwait(false);
                    return RedirectToAction(nameof(Index));
                }
                var surfaceTypeResp = await _surfaceTypeService.ListAsync().ConfigureAwait(false);
                ViewData["SurfaceTypeId"] = new SelectList(surfaceTypeResp.SurfaceType, "ID", "Code", roadConditionCodeUnit.SurfaceTypeId);
                return View(roadConditionCodeUnit);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"Road Condition Code Units Create Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "Road Condition Code Create Page has reloaded");
                return View();
            }
        }


        // GET: RoadConditionCodeUnits/Edit/5
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RoadConditionCodeUnit.View)]
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
                var roadConditionCodeUnitResp = await _roadConditionCodeUnitService.FindByIdAsync(ID).ConfigureAwait(false);
                var roadConditionCodeUnit = roadConditionCodeUnitResp.RoadConditionCodeUnit;
                if (roadConditionCodeUnit == null)
                {
                    return NotFound();
                }
                var surfaceTypeResp = await _surfaceTypeService.ListAsync().ConfigureAwait(false);
                ViewData["SurfaceTypeId"] = new SelectList(surfaceTypeResp.SurfaceType, "ID", "Code", roadConditionCodeUnit.SurfaceTypeId);
                return View(roadConditionCodeUnit);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"Road Condition Code Units Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "Road Condition Code Page has reloaded");
                return View();
            }
        }

        // POST: RoadConditionCodeUnits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RoadConditionCodeUnit.Change)]
        public async Task<IActionResult> Edit(int id, [Bind("ID,RoadCondition,Rate,ActivitiesRequired,SurfaceTypeId")] RoadConditionCodeUnit roadConditionCodeUnit)
        {
            try
            {
                if (id != roadConditionCodeUnit.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var roadClassCodeUnitResp = await _roadConditionCodeUnitService.Update(id, roadConditionCodeUnit).ConfigureAwait(false);
                    return RedirectToAction(nameof(Index));
                }
                var surfaceTypeResp = await _surfaceTypeService.ListAsync().ConfigureAwait(false);
                ViewData["SurfaceTypeId"] = new SelectList(surfaceTypeResp.SurfaceType, "ID", "Code", roadConditionCodeUnit.SurfaceTypeId);                
                return View(roadConditionCodeUnit);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"Road Condition Code Units Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "Road Condition Code Page has reloaded");
                return View();
            }
        }


        // GET: RoadConditionCodeUnits/Delete/5
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RoadConditionCodeUnit.View)]
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
                var roadConditionCodeUnitResp = await _roadConditionCodeUnitService.FindByIdAsync(ID).ConfigureAwait(false);
                var roadConditionCodeUnit = roadConditionCodeUnitResp.RoadConditionCodeUnit;
                if (roadConditionCodeUnit == null)
                {
                    return NotFound();
                }

                return View(roadConditionCodeUnit);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"Road Condition Code Units Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "Road Condition Code Delete Page has reloaded");
                return View();
            }
        }

        // POST: RoadConditionCodeUnits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RoadConditionCodeUnit.Delete)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var roadConditionCodeUnitResp = await _roadConditionCodeUnitService.FindByIdAsync(id).ConfigureAwait(false);
                //var roadClassCodeUnit = await _context.RoadClassCodeUnits.FindAsync(id);

                if (!roadConditionCodeUnitResp.Success)
                {
                    return NotFound();
                }

                roadConditionCodeUnitResp = await _roadConditionCodeUnitService.RemoveAsync(id).ConfigureAwait(false);
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
