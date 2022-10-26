using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Controllers
{
    [Authorize]
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutAttribute))]
    public class TrainingCoursesController : Controller
    {
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly ILogger _logger;
        private readonly ITrainingCourseService _trainingCourseService;
        public TrainingCoursesController(IApplicationUsersService applicationUsersService,
             IAuthorityService authorityService, ILogger<TrainingCoursesController> logger,
             ITrainingCourseService trainingCourseService)
        {
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _logger = logger;
            _trainingCourseService = trainingCourseService;
        }


        // GET: TrainingCoursesController
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Index()
        {
            try
            {
                //Return all training courses
                var resp = await _trainingCourseService.ListAsync().ConfigureAwait(false);

                return View(resp.TrainingCourse);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadClassificationsController Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "RoadClassificationsController Index Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.TrainingCourse.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddEditTrainingCourse(int? id)
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
                    TrainingCourse trainingCourse = new TrainingCourse();
                    return View(trainingCourse);
                }else
                {
                    var resp = await _trainingCourseService.FindByIdAsync(ID).ConfigureAwait(false);
                    var trainingCourse = resp.TrainingCourse;
                    if (trainingCourse == null)
                    {
                        return NotFound();
                    }
                    return View(trainingCourse);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TrainingCoursesController.AddRoadtoGIS Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "TrainingCoursesController.AddRoadtoGIS Page has reloaded");
                return View();
            }
        }

        // POST: RoadClassifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.TrainingCourse.Add),Authorize(Claims.Permission.TrainingCourse.Change)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddEditTrainingCourse(long id, [Bind("ID,Course,Description," +
            "UpdateBy,UpdateDate,CreatedBy,CreationDate")] TrainingCourse trainingCourse)
        {
            try
            {
                bool success = false;
                string msg = null;
                if (id != trainingCourse.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    ApplicationUser loggedInuser = await GetLoggedInUser().ConfigureAwait(false);

                    //if id=0 then create new
                    if (id==0)
                    {
                        trainingCourse.CreatedBy = loggedInuser.UserName;
                        trainingCourse.CreationDate = DateTime.UtcNow;
                        trainingCourse.UpdateBy = null;
                        trainingCourse.UpdateDate = null;
                        var trainingResp = await _trainingCourseService.AddAsync(trainingCourse).ConfigureAwait(false);
                        if (trainingResp.Success)
                        {
                            success = true;
                            msg = "Training Course Successfully Added";
                        }
                    }
                    //if id>0 then its an edit
                    if (id > 0)
                    {
                        trainingCourse.UpdateBy = loggedInuser.UserName;
                        trainingCourse.UpdateDate = DateTime.UtcNow;
                        var trainingResp = await _trainingCourseService.
                            Update(id,trainingCourse).ConfigureAwait(false);
                        if (trainingResp.Success)
                        {
                            success = true;
                            msg = "Training Course Successfully Updated";
                        }
                    }

                    return Json(new
                    {
                        Success = success,
                        Message = msg,
                        Href = Url.Action("Index", "TrainingCourses")
                    }); ;
                    //return RedirectToAction(nameof(Index));
                }
                return Json(new
                {
                    Success = false,
                    Message = "Model Error",
                    Href = Url.Action("Index", "TrainingCourses")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TrainingCoursesController Edit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "TrainingCoursesController Edit Page has reloaded");
                return View(trainingCourse);
            }
        }


        // GET: TrainingCoursesController/Details/5
        [Authorize(Claims.Permission.TrainingCourse.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Details(int id)
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
                    TrainingCourse trainingCourse = new TrainingCourse();
                    return View(trainingCourse);
                }
                else
                {
                    var resp = await _trainingCourseService.FindByIdAsync(ID).ConfigureAwait(false);
                    var trainingCourse = resp.TrainingCourse;
                    if (trainingCourse == null)
                    {
                        return NotFound();
                    }
                    return View(trainingCourse);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TrainingCoursesController.Details Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "TrainingCoursesController.Details Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.TrainingCourse.View)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Details(long id, [Bind("ID")] TrainingCourse trainingCourse)
        {
            try
            {
                bool success = false;
                string msg = null;
                if (id != trainingCourse.ID)
                {
                    return NotFound();
                }

                return Json(new
                {
                    Success = true,
                    Message = "Close up",
                    Href = Url.Action("Index", "TrainingCourses")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TrainingCoursesController Details Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "TrainingCoursesController Details Page has reloaded");
                return View(trainingCourse);
            }
        }


        // GET: TrainingCoursesController/Details/5
        [Authorize(Claims.Permission.TrainingCourse.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Delete(int id)
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
                    TrainingCourse trainingCourse = new TrainingCourse();
                    return View(trainingCourse);
                }
                else
                {
                    var resp = await _trainingCourseService.FindByIdAsync(ID).ConfigureAwait(false);
                    var trainingCourse = resp.TrainingCourse;
                    if (trainingCourse == null)
                    {
                        return NotFound();
                    }
                    return View(trainingCourse);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TrainingCoursesController.Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "TrainingCoursesController.Delete Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.TrainingCourse.Delete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Delete(long id, [Bind("ID")] TrainingCourse trainingCourse)
        {
            try
            {
                bool success = false;
                string msg = "Delete Failed";
                if (id != trainingCourse.ID)
                {
                    return NotFound();
                }
                var resp = await _trainingCourseService.RemoveAsync(id).ConfigureAwait(false);
                if (resp.Success)
                {
                    success = true;
                     msg = "Training Course Deleted";
                }
                return Json(new
                {
                    Success = success,
                    Message = msg,
                    Href = Url.Action("Index", "TrainingCourses")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TrainingCoursesController Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "TrainingCoursesController Delete Page has reloaded");
                return View(trainingCourse);
            }
        }


        #region User Access
        private async Task<ApplicationUser> GetLoggedInUser()
        {
            var userResp = await _applicationUsersService.FindByNameAsync(User.Identity.Name).ConfigureAwait(false);
            ApplicationUser user = null;
            if (userResp.Success)
            {

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
    }
}
