using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using APRP.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Controllers
{
    [Authorize]
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutAttribute))]
    public class GISController : Controller
    {
        private readonly ILogger _logger;
        private readonly IKenHARoadService _kenHARoadService;
        private readonly IKeRRARoadService _keRRARoadService;
        private readonly IKuRARoadService _kuRARoadService;
        private readonly IKwSRoadService _kwSRoadService;
        private readonly ICountiesRoadService _countiesRoadService;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly IRoadService _roadService;
        public GISController(ILogger<ARICSController> logger, IKenHARoadService kenHARoadService,
            IKeRRARoadService keRRARoadService, IKuRARoadService kuRARoadService,
            IKwSRoadService kwSRoadService, ICountiesRoadService countiesRoadService,
            IApplicationUsersService applicationUsersService, IAuthorityService authorityService,
            IRoadService roadService)
        {
            _logger = logger;
            _kenHARoadService = kenHARoadService;
            _keRRARoadService = keRRARoadService;
            _kuRARoadService = kuRARoadService;
            _kwSRoadService = kwSRoadService;
            _countiesRoadService = countiesRoadService;
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _roadService = roadService;
        }

        #region Ajax
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> GetKenNHARoadDetail(long ID)
        {
            try
            {
                var kenHARoadResp = await _kenHARoadService.FindByIdAsync(ID).ConfigureAwait(false);
                KenhaRoad kenhaRoad = kenHARoadResp.KenhaRoad;
                return Json(kenhaRoad);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"ARICSController.GetCounty API Error {Ex.Message}");
                return Json(null);
            }
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> GetKeRRARoadDetail(long ID)
        {
            try
            {
                var KeRRARoadResp = await _keRRARoadService.FindByIdAsync(ID).ConfigureAwait(false);
                KerraRoad kerraRoad = KeRRARoadResp.KerraRoad;
                return Json(kerraRoad);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"ARICSController.GetKeRRARoadDetail Error {Ex.Message}");
                return Json(null);
            }
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> GetKURARoadDetail(long ID)
        {
            try
            {
                var kURARoadResp = await _kuRARoadService.FindByIdAsync(ID).ConfigureAwait(false);
                KuraRoad kuraRoad = kURARoadResp.KuraRoad;
                return Json(kuraRoad);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"ARICSController.GetKURARoadDetail Error {Ex.Message}");
                return Json(null);
            }
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> GetKWSRoadDetail(long ID)
        {
            try
            {
                var KwSRoadResp = await _kwSRoadService.FindByIdAsync(ID).ConfigureAwait(false);
                KwsRoad kwsRoad = KwSRoadResp.KwsRoad;
                return Json(kwsRoad);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"ARICSController.GetKWSRoadDetail Error {Ex.Message}");
                return Json(null);
            }
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> GetCountiesRoadDetail(long ID)
        {
            try
            {
                var countiesRoadResp = await _countiesRoadService.FindByIdAsync(ID).ConfigureAwait(false);
                CountiesRoad countiesRoad = countiesRoadResp.CountiesRoad;
                return Json(countiesRoad);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"ARICSController.GetKURARoadDetail Error {Ex.Message}");
                return Json(null);
            }
        }
        #endregion

        #region KERRA Section
        [Authorize(Claims.Permission.KERRA.GIS_View)]
        // GET: TrainingCoursesController
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> IndexKERRA()
        {
            try
            {
                //Return all Kerra roads
                //var resp = await _keRRARoadService.ListAsync().ConfigureAwait(false);
                KerraRoadViewModel kerraRoadViewModel = new KerraRoadViewModel
                {
                    Referer = Request.GetEncodedUrl()
                };
                return View(kerraRoadViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController Index Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.KERRA.GIS_View)]
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> OnGetKerraRoads()
        {
            try
            {
                IQueryable<KerraRoadViewModel> kerraRoadsData = null;
                var user = await GetLoggedInUser().ConfigureAwait(false);
                //var userResp = await _applicationUsersService.IsInRoleAsync(user, "Administrators").ConfigureAwait(false);


                //Return all Roads
                var roadViewResponse = await _keRRARoadService.ListViewAsync().ConfigureAwait(false);
                kerraRoadsData = roadViewResponse.Roads;

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
                        if (sortColumn == "rdnum")
                        {
                            kerraRoadsData = kerraRoadsData.OrderByDescending(s => s.rdnum);
                        }
                        else if (sortColumn == "rdname")
                        {
                            kerraRoadsData = kerraRoadsData.OrderByDescending(s => s.rdname);
                        }
                        else if (sortColumn == "sectionid")
                        {
                            kerraRoadsData = kerraRoadsData.OrderByDescending(s => s.sectionid);
                        }
                        else if (sortColumn == "secname")
                        {
                            kerraRoadsData = kerraRoadsData.OrderByDescending(s => s.secname);
                        }
                        else if (sortColumn == "surfacetype")
                        {
                            kerraRoadsData = kerraRoadsData.OrderByDescending(s => s.surfacetype);
                        }
                        else if (sortColumn == "length")
                        {
                            kerraRoadsData = kerraRoadsData.OrderByDescending(s => s.length);
                        }
                    }

                    else
                    {
                        if (sortColumn == "rdnum")
                        {
                            kerraRoadsData = kerraRoadsData.OrderBy(s => s.rdnum);
                        }
                        else if (sortColumn == "rdname")
                        {
                            kerraRoadsData = kerraRoadsData.OrderBy(s => s.rdname);
                        }
                        else if (sortColumn == "sectionid")
                        {
                            kerraRoadsData = kerraRoadsData.OrderBy(s => s.sectionid);
                        }
                        else if (sortColumn == "secname")
                        {
                            kerraRoadsData = kerraRoadsData.OrderBy(s => s.secname);
                        }
                        else if (sortColumn == "surfacetype")
                        {
                            kerraRoadsData = kerraRoadsData.OrderBy(s => s.surfacetype);
                        }
                        else if (sortColumn == "length")
                        {
                            kerraRoadsData = kerraRoadsData.OrderBy(s => s.length);
                        }
                    }
                }

                IEnumerable<KerraRoadViewModel> kerraRoadsDataEnum = kerraRoadsData.AsEnumerable<KerraRoadViewModel>();

                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    kerraRoadsDataEnum = kerraRoadsDataEnum
                        .Where(
                         m => (m.rdnum != null && m.rdnum.ToLower().Contains(searchValue.ToLower()))
                        || (m.sectionid != null && m.sectionid.ToString().ToLower().Contains(searchValue.ToLower()))
                        || (m.rdname != null && m.rdname.ToString().ToLower().Contains(searchValue.ToLower()))
                        || (m.surfacetype != null && m.surfacetype.ToString().ToLower().Contains(searchValue.ToLower()))
                        || (m.length != null && m.length.ToString().ToLower().Contains(searchValue.ToLower()))
                    );
                }


                //total number of rows count 
                recordsTotal = kerraRoadsDataEnum.Count();
                //Paging 
                var data = kerraRoadsDataEnum.Skip(skip).Take(pageSize).ToList();

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

        [Authorize(Claims.Permission.KERRA.GIS_Add), Authorize(Claims.Permission.KERRA.GIS_Change)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddEditKERRA(int? id)
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
                    KerraRoad kerraRoad = new KerraRoad();
                    return View(kerraRoad);
                }
                else
                {
                    var resp = await _keRRARoadService.FindByIdAsync(ID).ConfigureAwait(false);
                    var kerraRoad = resp.KerraRoad;
                    if (kerraRoad == null)
                    {
                        return NotFound();
                    }
                    return View(kerraRoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController.AddEditKERRA Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController.AddEditKERRA Page has reloaded");
                return View();
            }
        }

        // POST: GIS/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.KERRA.GIS_Change), Authorize(Claims.Permission.KERRA.GIS_Add)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddEditKERRA(long id, [Bind("ID,RdNum,RdName,Section_ID," +
            "Sec_Name,SurfaceType,RdClass,Length")] KerraRoad kerraRoad)
        {
            try
            {
                bool success = false;
                string msg = null;
                if (id != kerraRoad.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    ApplicationUser loggedInuser = await GetLoggedInUser().ConfigureAwait(false);

                    //if id=0 then create new
                    if (id == 0)
                    {
                        //Get Road ID by searching by road Number and authority
                        var roadResp = await _roadService.
                            FindByRoadNumberAsync(loggedInuser.AuthorityId, kerraRoad.RdNum).ConfigureAwait(false);
                        if (roadResp.Success)
                        {
                            kerraRoad.RoadId = roadResp.Road.ID;
                            var kerraResp = await _keRRARoadService.AddAsync(kerraRoad).ConfigureAwait(false);
                            if (kerraResp.Success)
                            {
                                success = true;
                                msg = "KERRA GIS Road Successfully Added";
                            }
                        }
                    }
                    //if id>0 then its an edit
                    if (id > 0)
                    {
                        // Get from DB first
                        var resp = await _keRRARoadService.FindByIdAsync(id).ConfigureAwait(false);
                        var kerraRoadFromDB = resp.KerraRoad;
                        if (kerraRoad != null)
                        {
                            kerraRoadFromDB.RdNum = kerraRoad.RdNum;
                            kerraRoadFromDB.RdName = kerraRoad.RdClass;
                            kerraRoadFromDB.Section_ID = kerraRoad.Section_ID;
                            kerraRoadFromDB.Sec_Name = kerraRoad.Sec_Name;
                            kerraRoadFromDB.SurfaceType = kerraRoad.SurfaceType;
                            kerraRoadFromDB.RdClass = kerraRoad.RdClass;
                            kerraRoadFromDB.Length = kerraRoad.Length;
                            var trainingResp = await _keRRARoadService.
                                UpdateAsync(id, kerraRoadFromDB).ConfigureAwait(false);
                            if (trainingResp.Success)
                            {
                                success = true;
                                msg = "KERRA GIS Road Successfully Updated";
                            }
                        }
                    }

                    return Json(new
                    {
                        Success = success,
                        Message = msg,
                        Href = Url.Action("IndexKERRA", "GIS")
                    }); ;
                    //return RedirectToAction(nameof(Index));
                }
                return Json(new
                {
                    Success = false,
                    Message = "Model Error",
                    Href = Url.Action("IndexKERRA", "GIS")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController AddEditKERRA Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController AddEditKERRA Page has reloaded");
                return View(kerraRoad);
            }
        }

        // GET: GISController/Details/5
        [Authorize(Claims.Permission.KERRA.GIS_View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DetailsKERRA(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                if (ID == 0)
                {
                    KerraRoad kerraRoad = new KerraRoad();
                    return View(kerraRoad);
                }
                else
                {
                    var resp = await _keRRARoadService.FindByIdAsync(ID).ConfigureAwait(false);
                    var kerraRoad = resp.KerraRoad;
                    if (kerraRoad == null)
                    {
                        return NotFound();
                    }
                    return View(kerraRoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController.DetailsKERRA Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController.DetailsKERRA Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.KERRA.GIS_View)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DetailsKERRA(long id, [Bind("ID")] KerraRoad kerraRoad)
        {
            try
            {
                bool success = false;
                string msg = null;
                if (id != kerraRoad.ID)
                {
                    return NotFound();
                }

                return Json(new
                {
                    Success = true,
                    Message = "Close up",
                    Href = Url.Action("IndexKERRA", "GIS")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController DetailsKERRA Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController DetailsKERRA Page has reloaded");
                return View(kerraRoad);
            }
        }


        // GET: GISController/Details/5
        [Authorize(Claims.Permission.KERRA.GIS_Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DeleteKERRA(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                if (ID == 0)
                {
                    KerraRoad kerraRoad = new KerraRoad();
                    return View(kerraRoad);
                }
                else
                {
                    var resp = await _keRRARoadService.FindByIdAsync(ID).ConfigureAwait(false);
                    var kerraRoad = resp.KerraRoad;
                    if (kerraRoad == null)
                    {
                        return NotFound();
                    }
                    return View(kerraRoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController.DeleteKERRA Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController.DeleteKERRA Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.KERRA.GIS_Delete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DeleteKERRA(long id, [Bind("ID")] KerraRoad kerraRoad)
        {
            try
            {
                bool success = false;
                string msg = "Delete Failed";
                if (id != kerraRoad.ID)
                {
                    return NotFound();
                }
                var resp = await _keRRARoadService.RemoveAsync(id).ConfigureAwait(false);
                if (resp.Success)
                {
                    success = true;
                    msg = "KERRA GIS Road Deleted";
                }
                return Json(new
                {
                    Success = success,
                    Message = msg,
                    Href = Url.Action("IndexKERRA", "GIS")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController.DeleteKERRA Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController.DeleteKERRAPage has reloaded");
                return View(kerraRoad);
            }
        }

        #endregion

        #region KENHA Section
        // GET: GISController
        [Authorize(Claims.Permission.KENHA.GIS_View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> IndexKENHA()
        {
            try
            {
                //Return all Kerra roads
                //var resp = await _keRRARoadService.ListAsync().ConfigureAwait(false);
                KenhaRoadViewModel kenhaRoadViewModel = new KenhaRoadViewModel();
                kenhaRoadViewModel.Referer = Request.GetEncodedUrl();
                return View(kenhaRoadViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController Index Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.KENHA.GIS_View)]
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> OnGetKENHARoads()
        {
            try
            {
                IQueryable<KenhaRoadViewModel> kenhaRoadsData = null;
                var user = await GetLoggedInUser().ConfigureAwait(false);
                var userResp = await _applicationUsersService.IsInRoleAsync(user, "Administrators").ConfigureAwait(false);


                //Return all Roads
                var roadViewResponse = await _kenHARoadService.ListViewAsync().ConfigureAwait(false);
                kenhaRoadsData = roadViewResponse.Roads;

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
                        if (sortColumn == "rdnum")
                        {
                            kenhaRoadsData = kenhaRoadsData.OrderByDescending(s => s.rdnum);
                        }
                        if (sortColumn == "rdname")
                        {
                            kenhaRoadsData = kenhaRoadsData.OrderByDescending(s => s.rdname);
                        }
                        else if (sortColumn == "sectionid")
                        {
                            kenhaRoadsData = kenhaRoadsData.OrderByDescending(s => s.sectionid);
                        }
                        else if (sortColumn == "secname")
                        {
                            kenhaRoadsData = kenhaRoadsData.OrderByDescending(s => s.secname);
                        }
                        else if (sortColumn == "surfacetype")
                        {
                            kenhaRoadsData = kenhaRoadsData.OrderByDescending(s => s.surfacetype);
                        }
                        else if (sortColumn == "road_class")
                        {
                            kenhaRoadsData = kenhaRoadsData.OrderByDescending(s => s.surfacetype);
                        }
                        else if (sortColumn == "length")
                        {
                            kenhaRoadsData = kenhaRoadsData.OrderByDescending(s => s.length);
                        }
                    }

                    else
                    {
                        if (sortColumn == "rdnum")
                        {
                            kenhaRoadsData = kenhaRoadsData.OrderBy(s => s.rdnum);
                        }
                        if (sortColumn == "rdname")
                        {
                            kenhaRoadsData = kenhaRoadsData.OrderBy(s => s.rdname);
                        }
                        else if (sortColumn == "sectionid")
                        {
                            kenhaRoadsData = kenhaRoadsData.OrderBy(s => s.sectionid);
                        }
                        else if (sortColumn == "secname")
                        {
                            kenhaRoadsData = kenhaRoadsData.OrderBy(s => s.secname);
                        }
                        else if (sortColumn == "surfacetype")
                        {
                            kenhaRoadsData = kenhaRoadsData.OrderBy(s => s.surfacetype);
                        }
                        else if (sortColumn == "road_class")
                        {
                            kenhaRoadsData = kenhaRoadsData.OrderBy(s => s.surfacetype);
                        }
                        else if (sortColumn == "length")
                        {
                            kenhaRoadsData = kenhaRoadsData.OrderBy(s => s.length);
                        }
                    }
                }

                IEnumerable<KenhaRoadViewModel> kenhaRoadsDataEnum = kenhaRoadsData.AsEnumerable<KenhaRoadViewModel>();

                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    kenhaRoadsDataEnum = kenhaRoadsDataEnum
                        .Where(
                         m => (m.rdnum != null && m.rdnum.ToLower().Contains(searchValue.ToLower()))
                        || (m.rdname != null && m.rdname.ToString().ToLower().Contains(searchValue.ToLower()))
                        || (m.surfacetype != null && m.surfacetype.ToString().ToLower().Contains(searchValue.ToLower()))
                    );
                }


                //total number of rows count 
                recordsTotal = kenhaRoadsDataEnum.Count();
                //Paging 
                var data = kenhaRoadsDataEnum.Skip(skip).Take(pageSize).ToList();

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

        [Authorize(Claims.Permission.KENHA.GIS_Add), Authorize(Claims.Permission.KENHA.GIS_Change)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddEditKENHA(int? id)
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
                    KenhaRoad kenhaRoad = new KenhaRoad();
                    return View(kenhaRoad);
                }
                else
                {
                    var resp = await _kenHARoadService.FindByIdAsync(ID).ConfigureAwait(false);
                    var kenhaRoad = resp.KenhaRoad;
                    if (kenhaRoad == null)
                    {
                        return NotFound();
                    }
                    return View(kenhaRoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController.AddEditKENHA Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController.AddEditKENHA Page has reloaded");
                return View();
            }
        }

        // POST: GIS/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.KENHA.GIS_Add), Authorize(Claims.Permission.KENHA.GIS_Change)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddEditKENHA(long id, [Bind("ID,RdNum,RdName,Section_ID," +
            "Sec_Name,SurfaceType,RdClass,Length")] KenhaRoad KenhaRoad)
        {
            try
            {
                bool success = false;
                string msg = null;
                if (id != KenhaRoad.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    ApplicationUser loggedInuser = await GetLoggedInUser().ConfigureAwait(false);

                    //if id=0 then create new
                    if (id == 0)
                    {
                        //Get Road ID by searching by road Number and authority
                        var roadResp = await _roadService.
                            FindByRoadNumberAsync(loggedInuser.AuthorityId, KenhaRoad.RdNum).ConfigureAwait(false);
                        if (roadResp.Success)
                        {
                            KenhaRoad.RoadId = roadResp.Road.ID;
                            var KenhaRoadResp = await _kenHARoadService.AddAsync(KenhaRoad).ConfigureAwait(false);
                            if (KenhaRoadResp.Success)
                            {
                                success = true;
                                msg = "KENHA GIS Road Successfully Added";
                            }
                        }

                    }
                    //if id>0 then its an edit
                    if (id > 0)
                    {
                        // Get from DB first
                        var resp = await _kenHARoadService.FindByIdAsync(id).ConfigureAwait(false);
                        var KenhaRoadFromDB = resp.KenhaRoad;
                        if (KenhaRoad != null)
                        {
                            KenhaRoadFromDB.RdNum = KenhaRoad.RdNum;
                            KenhaRoadFromDB.RdName = KenhaRoad.RdName;
                            KenhaRoadFromDB.Section_ID = KenhaRoad.Section_ID;
                            KenhaRoadFromDB.Sec_Name = KenhaRoad.Sec_Name;
                            KenhaRoadFromDB.SurfaceType = KenhaRoad.SurfaceType;
                            KenhaRoadFromDB.RdClass = KenhaRoad.RdClass;
                            KenhaRoadFromDB.Length = KenhaRoad.Length;
                            var KenhaRoadResp = await _kenHARoadService.
                                UpdateAsync(id, KenhaRoadFromDB).ConfigureAwait(false);
                            if (KenhaRoadResp.Success)
                            {
                                success = true;
                                msg = "KENHA GIS Road Successfully Updated";
                            }
                        }
                    }

                    return Json(new
                    {
                        Success = success,
                        Message = msg,
                        Href = Url.Action("IndexKENHA", "GIS")
                    }); ;
                }
                return Json(new
                {
                    Success = false,
                    Message = "Model Error",
                    Href = Url.Action("IndexKENHA", "GIS")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController AddEditKENHA Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController AddEditKENHA Page has reloaded");
                return View(KenhaRoad);
            }
        }

        // GET: GISController/Details/5
        [Authorize(Claims.Permission.KENHA.GIS_View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DetailsKENHA(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                if (ID == 0)
                {
                    KenhaRoad kenhaRoad = new KenhaRoad();
                    return View(kenhaRoad);
                }
                else
                {
                    var resp = await _kenHARoadService.FindByIdAsync(ID).ConfigureAwait(false);
                    var kenhaRoad = resp.KenhaRoad;
                    if (kenhaRoad == null)
                    {
                        return NotFound();
                    }
                    return View(kenhaRoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController.DetailsKENHA Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController.DetailsKENHA Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.KENHA.GIS_View)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DetailsKENHA(long id, [Bind("ID")] KenhaRoad kenhaRoad)
        {
            try
            {
                bool success = false;
                string msg = null;
                if (id != kenhaRoad.ID)
                {
                    return NotFound();
                }

                return Json(new
                {
                    Success = true,
                    Message = "Close up",
                    Href = Url.Action("IndexKENHA", "GIS")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController DetailsKENHA Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController DetailsKENHA Page has reloaded");
                return View(kenhaRoad);
            }
        }


        // GET: GISController/Details/5
        [Authorize(Claims.Permission.KENHA.GIS_Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DeleteKENHA(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                if (ID == 0)
                {
                    KenhaRoad kenhaRoad = new KenhaRoad();
                    return View(kenhaRoad);
                }
                else
                {
                    var resp = await _kenHARoadService.FindByIdAsync(ID).ConfigureAwait(false);
                    var kenhaRoad = resp.KenhaRoad;
                    if (kenhaRoad == null)
                    {
                        return NotFound();
                    }
                    return View(kenhaRoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController.DeleteKENHA Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController.DeleteKENHAPage has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.KENHA.GIS_Delete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DeleteKENHA(long id, [Bind("ID")] KenhaRoad kenhaRoad)
        {
            try
            {
                bool success = false;
                string msg = "Delete Failed";
                if (id != kenhaRoad.ID)
                {
                    return NotFound();
                }
                var resp = await _kenHARoadService.RemoveAsync(id).ConfigureAwait(false);
                if (resp.Success)
                {
                    success = true;
                    msg = "KENHA GIS Road Deleted";
                }
                return Json(new
                {
                    Success = success,
                    Message = msg,
                    Href = Url.Action("IndexKENHA", "GIS")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController.DeleteKENHAPage Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController.DeleteKENHAPage has reloaded");
                return View(kenhaRoad);
            }
        }

        #endregion

        #region KURA Section
        // GET: GISController
        [Authorize(Claims.Permission.KURA.GIS_View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> IndexKURA()
        {
            try
            {
                //Return all Kerra roads
                //var resp = await _keRRARoadService.ListAsync().ConfigureAwait(false);
                KuraRoadViewModel kuraRoadViewModel = new KuraRoadViewModel();
                kuraRoadViewModel.Referer = Request.GetEncodedUrl();
                return View(kuraRoadViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController KURA Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController KURA Index Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.KURA.GIS_View)]
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> OnGetKURARoads()
        {
            try
            {
                IQueryable<KuraRoadViewModel> kuraRoadsData = null;
                var user = await GetLoggedInUser().ConfigureAwait(false);
                var userResp = await _applicationUsersService.IsInRoleAsync(user, "Administrators").ConfigureAwait(false);


                //Return all Roads
                var roadViewResponse = await _kuRARoadService.ListViewAsync().ConfigureAwait(false);
                kuraRoadsData = roadViewResponse.Roads;

                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();

                // Skiping number of Rows count
                var start = Request.Form["start"].FirstOrDefault();

                // Paging Length 10,20
                var length = Request.Form["length"].FirstOrDefault();


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
                        if (sortColumn == "rdnum")
                        {
                            kuraRoadsData = kuraRoadsData.OrderByDescending(s => s.rdnum);
                        }
                        else if (sortColumn == "rdname")
                        {
                            kuraRoadsData = kuraRoadsData.OrderByDescending(s => s.rdname);
                        }
                        else if (sortColumn == "sectionid")
                        {
                            kuraRoadsData = kuraRoadsData.OrderByDescending(s => s.sectionid);
                        }
                        else if (sortColumn == "secname")
                        {
                            kuraRoadsData = kuraRoadsData.OrderByDescending(s => s.secname);
                        }
                        else if (sortColumn == "surfacetype")
                        {
                            kuraRoadsData = kuraRoadsData.OrderByDescending(s => s.surfacetype);
                        }
                        else if (sortColumn == "length")
                        {
                            kuraRoadsData = kuraRoadsData.OrderByDescending(s => s.length);
                        }
                    }

                    else
                    {
                        if (sortColumn == "rdnum")
                        {
                            kuraRoadsData = kuraRoadsData.OrderBy(s => s.rdnum);
                        }
                        else if (sortColumn == "rdname")
                        {
                            kuraRoadsData = kuraRoadsData.OrderBy(s => s.rdname);
                        }
                        else if (sortColumn == "sectionid")
                        {
                            kuraRoadsData = kuraRoadsData.OrderBy(s => s.sectionid);
                        }
                        else if (sortColumn == "secname")
                        {
                            kuraRoadsData = kuraRoadsData.OrderBy(s => s.secname);
                        }
                        else if (sortColumn == "surfacetype")
                        {
                            kuraRoadsData = kuraRoadsData.OrderBy(s => s.surfacetype);
                        }
                        else if (sortColumn == "length")
                        {
                            kuraRoadsData = kuraRoadsData.OrderBy(s => s.length);
                        }
                    }
                }

                IEnumerable<KuraRoadViewModel> kuraRoadsDataEnum = kuraRoadsData.AsEnumerable<KuraRoadViewModel>();

                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    kuraRoadsDataEnum = kuraRoadsDataEnum
                        .Where(
                         m => (m.rdnum != null && m.rdnum.ToLower().Contains(searchValue.ToLower()))
                        || (m.rdname != null && m.rdname.ToString().ToLower().Contains(searchValue.ToLower()))
                        || (m.sectionid != null && m.sectionid.ToString().ToLower().Contains(searchValue.ToLower()))
                        || (m.surfacetype != null && m.surfacetype.ToString().ToLower().Contains(searchValue.ToLower()))
                        || (m.length != null && m.length.ToString().ToLower().Contains(searchValue.ToLower()))
                    );
                }


                //total number of rows count 
                recordsTotal = kuraRoadsDataEnum.Count();
                //Paging 
                var data = kuraRoadsDataEnum.Skip(skip).Take(pageSize).ToList();

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

        [Authorize(Claims.Permission.KURA.GIS_Add), Authorize(Claims.Permission.KURA.GIS_Change)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddEditKURA(int? id)
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
                    KuraRoad kuraRoad = new KuraRoad();
                    return View(kuraRoad);
                }
                else
                {
                    var resp = await _kuRARoadService.FindByIdAsync(ID).ConfigureAwait(false);
                    var kuraRoad = resp.KuraRoad;
                    if (kuraRoad == null)
                    {
                        return NotFound();
                    }
                    return View(kuraRoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController.AddEditKURA Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController.AddEditKURA Page has reloaded");
                return View();
            }
        }

        // POST: GIS/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.KURA.GIS_Add), Authorize(Claims.Permission.KURA.GIS_Change)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddEditKURA(long id, [Bind("ID,RdNum,RdName,Section_ID," +
            "Sec_Name,SurfaceType,RdClass,Length")] KuraRoad kuraRoad)
        {
            try
            {
                bool success = false;
                string msg = null;
                if (id != kuraRoad.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    ApplicationUser loggedInuser = await GetLoggedInUser().ConfigureAwait(false);

                    //if id=0 then create new
                    if (id == 0)
                    {
                        //Get Road ID by searching by road Number and authority
                        var roadResp = await _roadService.
                            FindByRoadNumberAsync(loggedInuser.AuthorityId, kuraRoad.RdNum).ConfigureAwait(false);
                        if (roadResp.Success)
                        {
                            kuraRoad.RoadId = roadResp.Road.ID;
                            var KuraRoadResp = await _kuRARoadService.AddAsync(kuraRoad).ConfigureAwait(false);
                            if (KuraRoadResp.Success)
                            {
                                success = true;
                                msg = "KURA GIS Road Successfully Added";
                            }
                        }

                    }
                    //if id>0 then its an edit
                    if (id > 0)
                    {
                        // Get from DB first
                        var resp = await _kuRARoadService.FindByIdAsync(id).ConfigureAwait(false);
                        var kuraRoadFromDB = resp.KuraRoad;
                        if (kuraRoad != null)
                        {
                            kuraRoadFromDB.RdNum = kuraRoad.RdNum;
                            kuraRoadFromDB.RdName = kuraRoad.RdName;
                            kuraRoadFromDB.Section_ID = kuraRoad.Section_ID;
                            kuraRoadFromDB.Sec_Name = kuraRoad.Sec_Name;
                            kuraRoadFromDB.SurfaceType = kuraRoad.SurfaceType;
                            kuraRoadFromDB.RdClass = kuraRoad.RdClass;
                            kuraRoadFromDB.Length = kuraRoad.Length;
                            var kuraRoadResp = await _kuRARoadService.
                                UpdateAsync(id, kuraRoadFromDB).ConfigureAwait(false);
                            if (kuraRoadResp.Success)
                            {
                                success = true;
                                msg = "KURA GIS Road Successfully Updated";
                            }
                        }
                    }

                    return Json(new
                    {
                        Success = success,
                        Message = msg,
                        Href = Url.Action("IndexKURA", "GIS")
                    }); ;
                }
                return Json(new
                {
                    Success = false,
                    Message = "Model Error",
                    Href = Url.Action("IndexKURA", "GIS")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController AddEditKURAPage Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController AddEditKURA Page has reloaded");
                return View(kuraRoad);
            }
        }

        // GET: GISController/Details/5
        [Authorize(Claims.Permission.KURA.GIS_View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DetailsKURA(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                if (ID == 0)
                {
                    KuraRoad kuraRoad = new KuraRoad();
                    return View(kuraRoad);
                }
                else
                {
                    var resp = await _kuRARoadService.FindByIdAsync(ID).ConfigureAwait(false);
                    var kuraRoad = resp.KuraRoad;
                    if (kuraRoad == null)
                    {
                        return NotFound();
                    }
                    return View(kuraRoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController.DetailsKURA Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController.DetailsKURA Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.KURA.GIS_View)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DetailsKURA(long id, [Bind("ID")] KuraRoad kuraRoad)
        {
            try
            {
                bool success = false;
                string msg = null;
                if (id != kuraRoad.ID)
                {
                    return NotFound();
                }

                return Json(new
                {
                    Success = true,
                    Message = "Close up",
                    Href = Url.Action("IndexKURA", "GIS")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController DetailsKURA Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController DetailsKENHA Page has reloaded");
                return View(kuraRoad);
            }
        }


        // GET: GISController/Details/5
        [Authorize(Claims.Permission.KURA.GIS_Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DeleteKURA(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                if (ID == 0)
                {
                    KuraRoad kuraRoad = new KuraRoad();
                    return View(kuraRoad);
                }
                else
                {
                    var resp = await _kuRARoadService.FindByIdAsync(ID).ConfigureAwait(false);
                    var kuraRoad = resp.KuraRoad;
                    if (kuraRoad == null)
                    {
                        return NotFound();
                    }
                    return View(kuraRoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController.DeleteKURA Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController.DeleteKURAPage has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.KURA.GIS_Delete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DeleteKURA(long id, [Bind("ID")] KuraRoad kuraRoad)
        {
            try
            {
                bool success = false;
                string msg = "Delete Failed";
                if (id != kuraRoad.ID)
                {
                    return NotFound();
                }
                var resp = await _kuRARoadService.RemoveAsync(id).ConfigureAwait(false);
                if (resp.Success)
                {
                    success = true;
                    msg = "KURA GIS Road Deleted";
                }
                return Json(new
                {
                    Success = success,
                    Message = msg,
                    Href = Url.Action("IndexKURA", "GIS")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController.DeleteKURAPage Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController.DeleteKURAPage has reloaded");
                return View(kuraRoad);
            }
        }

        #endregion

        #region KWS Section
        // GET: GISController
        [Authorize(Claims.Permission.KURA.GIS_View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> IndexKWS()
        {
            try
            {
                //Return all KWS roads
                KwsRoadViewModel kwsRoadViewModel = new KwsRoadViewModel();
                kwsRoadViewModel.Referer = Request.GetEncodedUrl();
                return View(kwsRoadViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController Index Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.KURA.GIS_View)]
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> OnGetKWSRoads()
        {
            try
            {
                IQueryable<KwsRoadViewModel> kwsRoadsData = null;
                var user = await GetLoggedInUser().ConfigureAwait(false);
                var userResp = await _applicationUsersService.IsInRoleAsync(user, "Administrators").ConfigureAwait(false);


                //Return all Roads
                var roadViewResponse = await _kwSRoadService.ListViewAsync().ConfigureAwait(false);
                kwsRoadsData = roadViewResponse.Roads;

                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();

                // Skiping number of Rows count
                var start = Request.Form["start"].FirstOrDefault();

                // Paging Length 10,20
                var length = Request.Form["length"].FirstOrDefault();

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
                        if (sortColumn == "rdnum")
                        {
                            kwsRoadsData = kwsRoadsData.OrderByDescending(s => s.rdnum);
                        }
                        else if (sortColumn == "rdname")
                        {
                            kwsRoadsData = kwsRoadsData.OrderByDescending(s => s.rdname);
                        }
                        else if (sortColumn == "sectionid")
                        {
                            kwsRoadsData = kwsRoadsData.OrderByDescending(s => s.sectionid);
                        }
                        else if (sortColumn == "secname")
                        {
                            kwsRoadsData = kwsRoadsData.OrderByDescending(s => s.secname);
                        }
                        else if (sortColumn == "surfacetype")
                        {
                            kwsRoadsData = kwsRoadsData.OrderByDescending(s => s.surfacetype);
                        }
                        else if (sortColumn == "length")
                        {
                            kwsRoadsData = kwsRoadsData.OrderByDescending(s => s.length);
                        }
                    }

                    else
                    {
                        if (sortColumn == "rdnum")
                        {
                            kwsRoadsData = kwsRoadsData.OrderBy(s => s.rdnum);
                        }
                        else if (sortColumn == "rdname")
                        {
                            kwsRoadsData = kwsRoadsData.OrderBy(s => s.rdname);
                        }
                        else if (sortColumn == "sectionid")
                        {
                            kwsRoadsData = kwsRoadsData.OrderBy(s => s.sectionid);
                        }
                        else if (sortColumn == "secname")
                        {
                            kwsRoadsData = kwsRoadsData.OrderBy(s => s.secname);
                        }
                        else if (sortColumn == "surfacetype")
                        {
                            kwsRoadsData = kwsRoadsData.OrderBy(s => s.surfacetype);
                        }
                        else if (sortColumn == "length")
                        {
                            kwsRoadsData = kwsRoadsData.OrderBy(s => s.length);
                        }
                    }
                }

                IEnumerable<KwsRoadViewModel> kwsRoadsDataDataEnum = kwsRoadsData.AsEnumerable<KwsRoadViewModel>();

                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    kwsRoadsDataDataEnum = kwsRoadsDataDataEnum
                        .Where(
                         m => (m.rdnum != null && m.rdnum.ToLower().Contains(searchValue.ToLower()))
                        || (m.rdname != null && m.rdname.ToString().ToLower().Contains(searchValue.ToLower()))
                        || (m.sectionid != null && m.sectionid.ToString().ToLower().Contains(searchValue.ToLower()))
                        || (m.surfacetype != null && m.surfacetype.ToString().ToLower().Contains(searchValue.ToLower()))
                        || (m.length != null && m.length.ToString().ToLower().Contains(searchValue.ToLower()))
                    );
                }


                //total number of rows count 
                recordsTotal = kwsRoadsDataDataEnum.Count();
                //Paging 
                var data = kwsRoadsDataDataEnum.Skip(skip).Take(pageSize).ToList();

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

        [Authorize(Claims.Permission.KURA.GIS_View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddEditKWS(int? id)
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
                    KwsRoad kwsRoad = new KwsRoad();
                    return View(kwsRoad);
                }
                else
                {
                    var resp = await _kwSRoadService.FindByIdAsync(ID).ConfigureAwait(false);
                    var kwsRoad = resp.KwsRoad;
                    if (kwsRoad == null)
                    {
                        return NotFound();
                    }
                    return View(kwsRoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController.AddEditKWS Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController.AddEditKWS Page has reloaded");
                return View();
            }
        }

        // POST: GIS/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.KURA.GIS_Add), Authorize(Claims.Permission.KURA.GIS_Change)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddEditKWS(long id, [Bind("ID,RdNum,RdName,Section_ID," +
            "Sec_Name,SurfaceType,RdClass,Length")] KwsRoad kwsRoad)
        {
            try
            {
                bool success = false;
                string msg = null;
                if (id != kwsRoad.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    ApplicationUser loggedInuser = await GetLoggedInUser().ConfigureAwait(false);

                    //if id=0 then create new
                    if (id == 0)
                    {
                        //Get Road ID by searching by road Number and authority
                        var roadResp = await _roadService.
                            FindByRoadNumberAsync(loggedInuser.AuthorityId, kwsRoad.RdNum).ConfigureAwait(false);
                        if (roadResp.Success)
                        {
                            kwsRoad.RoadId = roadResp.Road.ID;
                            var KwsRoadResp = await _kwSRoadService.AddAsync(kwsRoad).ConfigureAwait(false);
                            if (KwsRoadResp.Success)
                            {
                                success = true;
                                msg = "KWS GIS Road Successfully Added";
                            }
                        }

                    }
                    //if id>0 then its an edit
                    if (id > 0)
                    {
                        // Get from DB first
                        var resp = await _kwSRoadService.FindByIdAsync(id).ConfigureAwait(false);
                        var kwsRoadFromDB = resp.KwsRoad;
                        if (kwsRoad != null)
                        {
                            kwsRoadFromDB.RdNum = kwsRoad.RdNum;
                            kwsRoadFromDB.RdName = kwsRoad.RdName;
                            kwsRoadFromDB.Section_ID = kwsRoad.Section_ID;
                            kwsRoadFromDB.Sec_Name = kwsRoad.Sec_Name;
                            kwsRoadFromDB.SurfaceType = kwsRoad.SurfaceType;
                            kwsRoadFromDB.RdClass = kwsRoad.RdClass;
                            kwsRoadFromDB.Length = kwsRoad.Length;
                            var KwsRoadResp = await _kwSRoadService.
                                UpdateAsync(id, kwsRoadFromDB).ConfigureAwait(false);
                            if (KwsRoadResp.Success)
                            {
                                success = true;
                                msg = "KWS GIS Road Successfully Updated";
                            }
                        }
                    }

                    return Json(new
                    {
                        Success = success,
                        Message = msg,
                        Href = Url.Action("IndexKWS", "GIS")
                    }); ;
                }
                return Json(new
                {
                    Success = false,
                    Message = "Model Error",
                    Href = Url.Action("IndexKWS", "GIS")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController AddEditKWS Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController AddEditKWS Page has reloaded");
                return View(kwsRoad);
            }
        }

        // GET: GISController/Details/5
        [Authorize(Claims.Permission.KURA.GIS_Add), Authorize(Claims.Permission.KURA.GIS_Change)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DetailsKWS(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                if (ID == 0)
                {
                    KwsRoad kwsRoad = new KwsRoad();
                    return View(kwsRoad);
                }
                else
                {
                    var resp = await _kwSRoadService.FindByIdAsync(ID).ConfigureAwait(false);
                    var kwsRoad = resp.KwsRoad;
                    if (kwsRoad == null)
                    {
                        return NotFound();
                    }
                    return View(kwsRoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController.DetailsKWS Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController.DetailsKWS Page has reloaded");
                return View();
            }
        }

        [Authorize(Roles = "Administrators,GIS.KWS.View")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DetailsKWS(long id, [Bind("ID")] KwsRoad kwsRoad)
        {
            try
            {
                bool success = false;
                string msg = null;
                if (id != kwsRoad.ID)
                {
                    return NotFound();
                }

                return Json(new
                {
                    Success = true,
                    Message = "Close up",
                    Href = Url.Action("IndexKWS", "GIS")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController DetailsKWS Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController DetailsKWS Page has reloaded");
                return View(kwsRoad);
            }
        }


        // GET: GISController/Details/5
        [Authorize(Claims.Permission.KURA.GIS_Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DeleteKWS(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                if (ID == 0)
                {
                    KwsRoad kwsRoad = new KwsRoad();
                    return View(kwsRoad);
                }
                else
                {
                    var resp = await _kwSRoadService.FindByIdAsync(ID).ConfigureAwait(false);
                    var kwsRoad = resp.KwsRoad;
                    if (kwsRoad == null)
                    {
                        return NotFound();
                    }
                    return View(kwsRoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController.DeleteKWS Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController.DeleteKWS Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.KURA.GIS_Delete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DeleteKWS(long id, [Bind("ID")] KwsRoad kwsRoad)
        {
            try
            {
                bool success = false;
                string msg = "Delete Failed";
                if (id != kwsRoad.ID)
                {
                    return NotFound();
                }
                var resp = await _kwSRoadService.RemoveAsync(id).ConfigureAwait(false);
                if (resp.Success)
                {
                    success = true;
                    msg = "KWS GIS Road Deleted";
                }
                return Json(new
                {
                    Success = success,
                    Message = msg,
                    Href = Url.Action("IndexKWS", "GIS")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController.DeleteKWS Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController.DeleteKWS Page has reloaded");
                return View(kwsRoad);
            }
        }

        #endregion

        #region Counties Section
        // GET: GISController
        [Authorize(Claims.Permission.Counties.GIS_View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> IndexCounties()
        {
            try
            {
                //Return all Counties roads
                CountiesRoadViewModel countiesRoadViewModel = new CountiesRoadViewModel();
                countiesRoadViewModel.Referer = Request.GetEncodedUrl();
                return View(countiesRoadViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController Counties Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController Counties Index Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.Counties.GIS_View)]
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> OnGetCountiesRoads()
        {
            try
            {
                IQueryable<CountiesRoadViewModel> countiesRoadsData = null;
                var user = await GetLoggedInUser().ConfigureAwait(false);

                //Return all Roads
                var roadViewResponse = await _countiesRoadService.ListViewAsync(user.AuthorityId).ConfigureAwait(false);
                countiesRoadsData = roadViewResponse.Roads;

                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();

                // Skiping number of Rows count
                var start = Request.Form["start"].FirstOrDefault();

                // Paging Length 10,20
                var length = Request.Form["length"].FirstOrDefault();


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
                        if (sortColumn == "rdnum")
                        {
                            countiesRoadsData = countiesRoadsData.OrderByDescending(s => s.rdnum);
                        }
                        else if (sortColumn == "rdname")
                        {
                            countiesRoadsData = countiesRoadsData.OrderByDescending(s => s.rdname);
                        }
                        else if (sortColumn == "sectionid")
                        {
                            countiesRoadsData = countiesRoadsData.OrderByDescending(s => s.sectionid);
                        }
                        else if (sortColumn == "secname")
                        {
                            countiesRoadsData = countiesRoadsData.OrderByDescending(s => s.secname);
                        }
                        else if (sortColumn == "surfacetype")
                        {
                            countiesRoadsData = countiesRoadsData.OrderByDescending(s => s.surfacetype);
                        }
                        else if (sortColumn == "length")
                        {
                            countiesRoadsData = countiesRoadsData.OrderByDescending(s => s.length);
                        }
                    }

                    else
                    {
                        if (sortColumn == "rdnum")
                        {
                            countiesRoadsData = countiesRoadsData.OrderBy(s => s.rdnum);
                        }
                        else if (sortColumn == "rdname")
                        {
                            countiesRoadsData = countiesRoadsData.OrderBy(s => s.rdname);
                        }
                        else if (sortColumn == "sectionid")
                        {
                            countiesRoadsData = countiesRoadsData.OrderBy(s => s.sectionid);
                        }
                        else if (sortColumn == "secname")
                        {
                            countiesRoadsData = countiesRoadsData.OrderBy(s => s.secname);
                        }
                        else if (sortColumn == "surfacetype")
                        {
                            countiesRoadsData = countiesRoadsData.OrderBy(s => s.surfacetype);
                        }
                        else if (sortColumn == "length")
                        {
                            countiesRoadsData = countiesRoadsData.OrderBy(s => s.length);
                        }
                    }
                }

                IEnumerable<CountiesRoadViewModel> countiesRoadsDataEnum = countiesRoadsData.AsEnumerable<CountiesRoadViewModel>();

                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    countiesRoadsDataEnum = countiesRoadsDataEnum
                        .Where(
                         m => (m.rdnum != null && m.rdnum.ToLower().Contains(searchValue.ToLower()))
                        || (m.rdname != null && m.rdname.ToString().ToLower().Contains(searchValue.ToLower()))
                        || (m.sectionid != null && m.sectionid.ToString().ToLower().Contains(searchValue.ToLower()))
                        || (m.surfacetype != null && m.surfacetype.ToString().ToLower().Contains(searchValue.ToLower()))
                        || (m.length != null && m.length.ToString().ToLower().Contains(searchValue.ToLower()))
                    );
                }


                //total number of rows count 
                recordsTotal = countiesRoadsDataEnum.Count();
                //Paging 
                var data = countiesRoadsDataEnum.Skip(skip).Take(pageSize).ToList();

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

        [Authorize(Claims.Permission.Counties.GIS_Add), Authorize(Claims.Permission.Counties.GIS_Change)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddEditCounties(int? id)
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
                    CountiesRoad countiesRoad = new CountiesRoad();
                    return View(countiesRoad);
                }
                else
                {
                    var resp = await _countiesRoadService.FindByIdAsync(ID).ConfigureAwait(false);
                    var countiesRoad = resp.CountiesRoad;
                    if (countiesRoad == null)
                    {
                        return NotFound();
                    }
                    return View(countiesRoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController.AddEditcountiesRoad Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController.AddEditcountiesRoad Page has reloaded");
                return View();
            }
        }

        // POST: GIS/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.Counties.GIS_Add), Authorize(Claims.Permission.Counties.GIS_Change)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddEditCounties(long id, [Bind("ID,RdNum,RdName,Section_ID," +
            "Sec_Name,SurfaceType,RdClass,Length")] CountiesRoad countiesRoad)
        {
            try
            {
                bool success = false;
                string msg = null;
                if (id != countiesRoad.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    ApplicationUser loggedInuser = await GetLoggedInUser().ConfigureAwait(false);

                    //if id=0 then create new
                    if (id == 0)
                    {
                        //Get Road ID by searching by road Number and authority
                        var roadResp = await _roadService.
                            FindByRoadNumberAsync(loggedInuser.AuthorityId, countiesRoad.RdNum).ConfigureAwait(false);
                        if (roadResp.Success)
                        {
                            countiesRoad.RoadId = roadResp.Road.ID;
                            var countiesRoadResp = await _countiesRoadService.AddAsync(countiesRoad).ConfigureAwait(false);
                            if (countiesRoadResp.Success)
                            {
                                success = true;
                                msg = "Counties GIS Road Successfully Added";
                            }
                        }
                    }
                    //if id>0 then its an edit
                    if (id > 0)
                    {
                        // Get from DB first
                        var resp = await _countiesRoadService.FindByIdAsync(id).ConfigureAwait(false);
                        var countiesRoadFromDB = resp.CountiesRoad;
                        if (countiesRoad != null)
                        {
                            countiesRoadFromDB.RdNum = countiesRoad.RdNum;
                            countiesRoadFromDB.RdName = countiesRoad.RdName;
                            countiesRoadFromDB.Section_ID = countiesRoad.Section_ID;
                            countiesRoadFromDB.Sec_Name = countiesRoad.Sec_Name;
                            countiesRoadFromDB.SurfaceType = countiesRoad.SurfaceType;
                            countiesRoadFromDB.RdClass = countiesRoad.RdClass;
                            countiesRoadFromDB.Length = countiesRoad.Length;
                            var countiesRoadResp = await _countiesRoadService.
                                UpdateAsync(id, countiesRoadFromDB).ConfigureAwait(false);
                            if (countiesRoadResp.Success)
                            {
                                success = true;
                                msg = "Counties GIS Road Successfully Updated";
                            }
                        }
                    }

                    return Json(new
                    {
                        Success = success,
                        Message = msg,
                        Href = Url.Action("IndexCounties", "GIS")
                    }); ;
                }
                return Json(new
                {
                    Success = false,
                    Message = "Model Error",
                    Href = Url.Action("IndexCounties", "GIS")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController AddEditCountiesPage Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController AddEditCounties Page has reloaded");
                return View(countiesRoad);
            }
        }

        // GET: GISController/Details/5
        [Authorize(Claims.Permission.Counties.GIS_View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DetailsCounties(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                if (ID == 0)
                {
                    CountiesRoad countiesRoad = new CountiesRoad();
                    return View(countiesRoad);
                }
                else
                {
                    var resp = await _countiesRoadService.FindByIdAsync(ID).ConfigureAwait(false);
                    var countiesRoad = resp.CountiesRoad;
                    if (countiesRoad == null)
                    {
                        return NotFound();
                    }
                    return View(countiesRoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController.DetailsCounties Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController.DetailsCounties Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.Counties.GIS_View)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DetailsCounties(long id, [Bind("ID")] CountiesRoad countiesRoad)
        {
            try
            {
                bool success = false;
                string msg = null;
                if (id != countiesRoad.ID)
                {
                    return NotFound();
                }

                return Json(new
                {
                    Success = true,
                    Message = "Close up",
                    Href = Url.Action("IndexCounties", "GIS")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController DetailsCounties Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController DetailsCounties Page has reloaded");
                return View(countiesRoad);
            }
        }


        // GET: GISController/Details/5
        [Authorize(Claims.Permission.Counties.GIS_Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DeleteCounties(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                if (ID == 0)
                {
                    CountiesRoad countiesRoad = new CountiesRoad();
                    return View(countiesRoad);
                }
                else
                {
                    var resp = await _countiesRoadService.FindByIdAsync(ID).ConfigureAwait(false);
                    var countiesRoad = resp.CountiesRoad;
                    if (countiesRoad == null)
                    {
                        return NotFound();
                    }
                    return View(countiesRoad);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController.DeleteCounties Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController.DeleteCounties Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.Counties.GIS_Delete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DeleteCounties(long id, [Bind("ID")] CountiesRoad countiesRoad)
        {
            try
            {
                bool success = false;
                string msg = "Delete Failed";
                if (id != countiesRoad.ID)
                {
                    return NotFound();
                }
                var resp = await _countiesRoadService.RemoveAsync(id).ConfigureAwait(false);
                if (resp.Success)
                {
                    success = true;
                    msg = "Counties GIS Road Deleted";
                }
                return Json(new
                {
                    Success = success,
                    Message = msg,
                    Href = Url.Action("IndexCounties", "GIS")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"GISController.DeleteCounties Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "GISController.DeleteCounties Page has reloaded");
                return View(countiesRoad);
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
        #endregion
    }
}
