using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using APRP.Web.ViewModels;
using APRP.Web.ViewModels.ResponseTypes;
using APRP.Web.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APRP.Web.Controllers
{
    [Authorize]
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutAttribute))]
    public class RoadController : Controller
    {
        private readonly IRoadService _roadService;
        private readonly IRoadSectionService _roadSectionService;
        private readonly ISurfaceTypeService _surfaceTypeService;
        private readonly IConstituencyService _constituencyService;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly IGISRoadService _gISRoadService;
        private readonly ILogger _logger;
        private readonly IKWSParkService _kWSParkService;
        private readonly IMunicipalityService _municipalityService;

        public RoadController(IRoadService roadService, IRoadSectionService roadSectionService, ISurfaceTypeService surfaceTypeService, IConstituencyService constituencyService,
            IApplicationUsersService applicationUsersService, IAuthorityService authorityService, IGISRoadService gISRoadService,
             ILogger<RoadController> logger, IKWSParkService kWSParkService, IMunicipalityService municipalityService)
        {
            _roadService = roadService;
            _roadSectionService = roadSectionService;
            _surfaceTypeService = surfaceTypeService;
            _constituencyService = constituencyService;
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _gISRoadService = gISRoadService;
            _logger = logger;
            _kWSParkService = kWSParkService;
            _municipalityService = municipalityService;
        }

        public async Task<IActionResult> Index()
        {
            IList<Road> roads = null;
            //Get user
            var user = await GetLoggedInUser().ConfigureAwait(false);
            var userResp = await _applicationUsersService.IsInRoleAsync(user, "Administrators").ConfigureAwait(false);
            var objectResult = (ObjectResult)userResp.IActionResult;
            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
            {
                var result2 = (OkObjectResult)objectResult;
                AuthResponse authResponse = (AuthResponse)result2.Value;
                if (authResponse.Success == true)
                {
                    //Return all Roads
                    var roadListResponse = await _roadService.ListAsync().ConfigureAwait(false);
                    roads = (IList<Road>)roadListResponse.Roads;
                }
                else
                {
                    //Return for authority that user is placed
                    var authority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                    var roadListResponse = await _roadService.ListAsync(authority.Authority).ConfigureAwait(false);
                    roads = (IList<Road>)roadListResponse.Roads;
                }
            }
            RoadSectionViewModel viewModel = new RoadSectionViewModel();
            viewModel.Roads = roads;

            //No of roads
            viewModel.RoadCount = roads.Count;
            return View(viewModel);
        }

        #region

        [HttpGet]
        public async Task<IActionResult> RoadSections(long roadId)
        {
            //Get user
            IList<Road> roads = null;
            var user = await GetLoggedInUser().ConfigureAwait(false);
            var userResp = await _applicationUsersService.IsInRoleAsync(user, "Administrators").ConfigureAwait(false);
            var objectResult = (ObjectResult)userResp.IActionResult;
            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
            {
                var result2 = (OkObjectResult)objectResult;
                AuthResponse authResponse = (AuthResponse)result2.Value;
                if (authResponse.Success == true)
                {
                    //Return all Roads
                    var roadListResponse = await _roadService.ListAsync().ConfigureAwait(false);
                    roads = (IList<Road>)roadListResponse.Roads;
                }
                else
                {
                    //Return for authority that user is placed
                    var authority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                    var roadListResponse = await _roadService.ListAsync(authority.Authority).ConfigureAwait(false);
                    roads = (IList<Road>)roadListResponse.Roads;
                }
            }

            var roadService = await _roadService.FindByIdAsync(roadId).ConfigureAwait(false);
            RoadSectionViewModel viewModel = new RoadSectionViewModel();
            Road road = new Road();
            IEnumerable<RoadSection> roadSections = Enumerable.Empty<RoadSection>();
            if (roadService.Success)
            {
                road = roadService.Road;
                roadSections = (await _roadSectionService.ListByRoadIdAsync(roadId).ConfigureAwait(false)).RoadSectionList;
            }
            viewModel.Road = road;
            viewModel.RoadSections = roadSections;
            //No of roads
            viewModel.RoadCount = roads.Count;

            return View(viewModel);
        }

        public async Task<IActionResult> ShowRoadSectionAddEdit(long SectionId, long RoadId)
        {
            RoadSectionEditModel model = new RoadSectionEditModel();
            //retrieving funding source for use in create view
            var SurfaceTypeListResponse = await _surfaceTypeService.ListAsync().ConfigureAwait(false);
            var SurfaceTypeList = (IList<SurfaceType>)SurfaceTypeListResponse.SurfaceType;
            var constituencyListResponse = await _constituencyService.ListAsync().ConfigureAwait(false);
            var ConstituencyList = (IList<Constituency>)constituencyListResponse.ConstituencyList;

            ViewBag.SurfaceTypeList = new SelectList(SurfaceTypeList, "ID", "Name");
            ViewBag.ConstituencyList = new SelectList(ConstituencyList, "ID", "Name");

            Road road = null;
            if (RoadId > 0)
            {
                //get the road
                var roadResp = await _roadService.FindByIdAsync(RoadId).ConfigureAwait(false);
                if (roadResp.Success)
                    model.Road = roadResp.Road;
            }
            else
            {
                model.Road = new Road();
            }

            if (SectionId > 0)
            {
                //get section object
                var roadSectionResp = await _roadSectionService.FindByIdAsync(SectionId).ConfigureAwait(false);
                if (roadSectionResp.Success)
                    model.RoadSection = roadSectionResp.RoadSection;
            }
            else
            {
                //model.RoadSection = new RoadSection();

                //Pull from GIS the road section
                var gISResp = await _gISRoadService.PullRoadSectionFromGISAsync(RoadId).ConfigureAwait(false);
                road = gISResp.Road;
            }

            if (road != null)
            {
                return RedirectToAction("RoadSections", new { roadId = road.ID });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSectionLine(RoadSectionEditModel roadSectionEditModel)
        {

            if (roadSectionEditModel.RoadSection.ID > 0)
            {
                //editing the record
                var updateRecordSection = roadSectionEditModel.RoadSection;
                var roadToAddTo = await _roadService.FindByIdAsync(roadSectionEditModel.Road.ID).ConfigureAwait(false);
                if (roadToAddTo.Success)
                    updateRecordSection.Road = roadToAddTo.Road;

                var response = await _roadSectionService.UpdateAsync(updateRecordSection).ConfigureAwait(false);
                if (response.Success)
                {

                    return Json(Url.Action("RoadSections", "Road", new { roadId = roadSectionEditModel.Road.ID }));
                }
                else
                {
                    return PartialView("RoadSectionPartialView", roadSectionEditModel);
                }
            }
            else
            {
                //new record
                var newRecordSection = roadSectionEditModel.RoadSection;
                var roadToAddTo = await _roadService.FindByIdAsync(roadSectionEditModel.Road.ID).ConfigureAwait(false);
                if (roadToAddTo.Success)
                    newRecordSection.Road = roadToAddTo.Road;

                var response = await _roadSectionService.AddAsync(newRecordSection).ConfigureAwait(false);
                if (response.Success)
                {
                    return Json(Url.Action("RoadSections", "Road", new { roadId = roadSectionEditModel.Road.ID }));
                }
                else
                {
                    return PartialView("RoadSectionPartialView", roadSectionEditModel);
                }
            }
        }

        #endregion

        #region Ajax

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> OnGetRoadsDetails()
        {
            try
            {
                IQueryable<RoadViewModel> roadsData = null;
                var user = await GetLoggedInUser().ConfigureAwait(false);
                var userResp = await _applicationUsersService.IsInRoleAsync(user, "Administrators").ConfigureAwait(false);
                var objectResult = (ObjectResult)userResp.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (OkObjectResult)objectResult;
                    AuthResponse authResponse = (AuthResponse)result2.Value;
                    if (authResponse.Success == true)
                    {
                        //Return all Roads
                        var roadViewResponse = await _roadService.ListViewAsync().ConfigureAwait(false);
                        roadsData = roadViewResponse.Roads;
                    }
                    else
                    {
                        //Return for authority that user is placed
                        var authority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);

                        var roadViewResponse = await _roadService.ListViewAsync(authority.Authority).ConfigureAwait(false);
                        roadsData = roadViewResponse.Roads;
                    }
                }
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
                        if (sortColumn == "road_number")
                        {
                            roadsData = roadsData.OrderByDescending(s => s.road_number);
                        }
                        else if (sortColumn == "road_name")
                        {
                            roadsData = roadsData.OrderByDescending(s => s.road_name);
                        }
                        else if (sortColumn == "authority_name")
                        {
                            roadsData = roadsData.OrderByDescending(s => s.authority_name);
                        }
                        else if (sortColumn == "pulled_from_gis")
                        {
                            roadsData = roadsData.OrderByDescending(s => s.pulled_from_gis);
                        }

                    }
                    else
                    {
                        if (sortColumn == "road_number")
                        {
                            roadsData = roadsData.OrderBy(s => s.road_number);
                        }
                        else if (sortColumn == "road_name")
                        {
                            roadsData = roadsData.OrderBy(s => s.road_name);
                        }
                        else if (sortColumn == "authority_name")
                        {
                            roadsData = roadsData.OrderBy(s => s.authority_name);
                        }
                        else if (sortColumn == "pulled_from_gis")
                        {
                            roadsData = roadsData.OrderBy(s => s.pulled_from_gis);
                        }
                    }
                }
                int x = 0;
                IEnumerable<RoadViewModel> roadsEnum = roadsData.AsEnumerable<RoadViewModel>();
                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    roadsEnum = roadsEnum
                        .Where(
                      m => (m.road_name != null && m.road_name.ToLower().Contains(searchValue.ToLower()))
                    || (m.road_number != null && m.road_number.ToString().ToLower().Contains(searchValue.ToLower()))
                    || (m.authority_name != null && m.authority_name.ToString().ToLower().Contains(searchValue.ToLower()))
                    || (m.pulled_from_gis != null && m.pulled_from_gis.ToString().ToLower().Contains(searchValue.ToLower()))
                    );
                }

                //total number of rows count 
                recordsTotal = roadsEnum.Count();
                //Paging 
                var data = roadsEnum.Skip(skip).Take(pageSize).ToList();

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

        [HttpPost]
        public async Task<JsonResult> PullAllRoadSectionsFromGIS(int? all)
        {
            try
            {
                //Get all roads
                IList<Road> roads = null;
                //Get user
                var user = await GetLoggedInUser().ConfigureAwait(false);
                var userResp = await _applicationUsersService.IsInRoleAsync(user, "Administrators").ConfigureAwait(false);
                var objectResult = (ObjectResult)userResp.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (OkObjectResult)objectResult;
                    AuthResponse authResponse = (AuthResponse)result2.Value;
                    if (authResponse.Success == true)
                    {
                        //Return all Roads
                        var roadListResponse = await _roadService.ListAsync().ConfigureAwait(false);
                        roads = ((IList<Road>)roadListResponse.Roads);
                    }
                    else
                    {
                        //Return for authority that user is placed
                        var authority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                        var roadListResponse = await _roadService.ListAsync(authority.Authority).ConfigureAwait(false);
                        roads = (IList<Road>)roadListResponse.Roads;
                    }
                }
                //Check if road sectons are existing

                //For each road, pull the road section from GIS
                int count = 0;
                IEnumerable<Road> roadsToPullFromGIS = null;
                if (all == '0')
                {
                    //Pull only those that haven't been pulled
                    roadsToPullFromGIS = roads.Where(x => x.PulledSectionsFromGIS == false);
                }
                else
                { //force pull all roads
                    roadsToPullFromGIS = roads;
                }

                foreach (Road road in roadsToPullFromGIS)
                {
                    //Get roadsections for road
                    var roadSectionsResp = await _roadSectionService.ListByRoadIdAsync(road.ID).ConfigureAwait(false);
                    IList<RoadSection> roadSections = (IList<RoadSection>)roadSectionsResp.RoadSectionList;
                    //if (roadSections.Count < 1)//No sections have been pulled from GIS
                    //{
                    //    var gISResp = await _gISRoadService.PullRoadSectionFromGISAsync(road.ID).ConfigureAwait(false);
                    //}
                    var gISResp = await _gISRoadService.PullRoadSectionFromGISAsync(road.ID).ConfigureAwait(false);
                    count++;
                }
                //Returning Json Data
                return new JsonResult(new
                {

                    Success = true,
                    Message = "Success",
                    Href = Url.Action("Index", "Road")
                });
            }
            catch (Exception) { throw; }
        }

        [HttpPost]
        public async Task<JsonResult> ResetPullFromGIS(int? all)
        {
            try
            {
                //Get all roads
                IList<Road> roads = null;
                //Get user
                var user = await GetLoggedInUser().ConfigureAwait(false);
                var userResp = await _applicationUsersService.IsInRoleAsync(user, "Administrators").ConfigureAwait(false);
                var objectResult = (ObjectResult)userResp.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (OkObjectResult)objectResult;
                    AuthResponse authResponse = (AuthResponse)result2.Value;
                    if (authResponse.Success == true)
                    {
                        //Return all Roads
                        var roadListResponse = await _roadService.ListAsync().ConfigureAwait(false);
                        roads = ((IList<Road>)roadListResponse.Roads);
                    }
                    else
                    {
                        //Return for authority that user is placed
                        var authority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                        var roadListResponse = await _roadService.ListAsync(authority.Authority).ConfigureAwait(false);
                        roads = (IList<Road>)roadListResponse.Roads;
                    }
                }
                //Check if road sectons are existing
                IEnumerable<Road> roadsToPullFromGIS = null;
                //Pull only those that haven't been pulled
                roadsToPullFromGIS = roads.Where(x => x.PulledSectionsFromGIS == true);

                //Lop through each record and set pull from GIS = false

                foreach (var rd in roadsToPullFromGIS)
                {

                    //set pulled from gis is false
                    rd.PulledSectionsFromGIS = false;

                    //update
                    var rdResp = await _roadService.UpdateAsync(rd.ID, rd).ConfigureAwait(false);

                    //detach
                    var rdDetachResp = await _roadService.DetachFirstEntryAsync(rd)
                        .ConfigureAwait(false);
                }

                //Returning Json Data
                return new JsonResult(new
                {

                    Success = true,
                    Message = "Success",
                    Href = Url.Action("Index", "Road")
                });
            }
            catch (Exception) { throw; }
        }

        #endregion

        #region CRUD Operations
        [Authorize(Claims.Permission.Road.Add), Authorize(Claims.Permission.Road.Change)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddEdit(int? id)
        {
            AddRoadViewModel addRoadViewModel = new AddRoadViewModel();
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                //Check if user is from KRB
                addRoadViewModel.applicationUser = await GetLoggedInUser().ConfigureAwait(false);

                int ID;
                bool result = int.TryParse(id.ToString(), out ID);
                if (ID == 0)
                {
                    Road road = new Road();
                    addRoadViewModel.Road = road;
                    if (addRoadViewModel.applicationUser.Authority.Code.ToLower() == "krb")
                    {
                        await PopulateDropDowns(0).ConfigureAwait(false);
                    }
                    else
                    {
                        await PopulateDropDowns(addRoadViewModel.applicationUser.Authority.ID).ConfigureAwait(false);
                    }
                    return View(road);
                }
                else
                {
                    var resp = await _roadService.FindByIdAsync(ID).ConfigureAwait(false);
                    var road = resp.Road;
                    if (road == null)
                    {
                        return NotFound();
                    }
                    addRoadViewModel.Road = road;
                    await PopulateDropDowns(addRoadViewModel.applicationUser.Authority.ID).ConfigureAwait(false);
                    return View(road);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadController.AddEdit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "RoadController.AddEdit Page has reloaded");
                return View();
            }
        }

        // POST: Road/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.Road.Add), Authorize(Claims.Permission.Road.Change)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddEdit(long id, [Bind("ID,AuthorityId,RoadNumber,AuthorityId,RoadName")] Road road)
        {
            try
            {
                bool success = false;
                string msg = null;
                if (id != road.ID)
                {
                    return NotFound();
                }

                //Check if user is from KRB
                ApplicationUser user = await GetLoggedInUser().ConfigureAwait(false);
                //if (user.Authority.Code.ToLower() != "krb")
                //{
                //    return Json(new
                //    {
                //        Success = false,
                //        Message = "Only a User from KRB is allowed",
                //        Href = Url.Action("Index", "Road")
                //    });
                //}

                if (ModelState.IsValid)
                {
                    //if id=0 then create new
                    if (id == 0)
                    {
                        var resp = await _roadService.FindByDisbursementEntryAsync(road).ConfigureAwait(false);
                        if (resp.Road != null)
                        {
                            string msgErr = $"A similar road entry exists" +
                            $" No duplicate entries may exists for the same road for the same authority";
                            ModelState.AddModelError(string.Empty, msgErr);
                            //Detach the first entry before attaching your updated entry
                            var respDetach = await _roadService.DetachFirstEntryAsync(resp.Road).ConfigureAwait(false);
                            //return RedirectToAction("AddEdit",new { id=disbursement.ID});
                            return Json(new
                            {
                                Success = false,
                                Message = msgErr,
                                Href = Url.Action("AddEdit", "Disbursement", new { id = string.Empty })
                            });
                        }

                        //add disbursement
                        var disbursementResp = await _roadService.AddAsync(road).ConfigureAwait(false);
                        if (disbursementResp.Success)
                        {
                            success = true;
                            msg = "Road Successfully Added";
                        }
                    }
                    //if id>0 then its an edit
                    if (id > 0)
                    {
                        var roadResp = await _roadService.UpdateAsync(id, road).ConfigureAwait(false);
                        if (roadResp.Success)
                        {
                            success = true;
                            msg = "Road Successfully Updated";
                        }
                    }

                    return Json(new
                    {
                        Success = success,
                        Message = msg,
                        Href = Url.Action("Index", "Road")
                    }); ;
                }
                return Json(new
                {
                    Success = false,
                    Message = "Model Error",
                    Href = Url.Action("Index", "Road")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadController AddEdit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "RoadController AddEdit Page has reloaded");
                await PopulateDropDowns(road.AuthorityId).ConfigureAwait(false);
                return View(road);
            }
        }

        private async Task PopulateDropDowns(long AuthorityId)
        {
            ApplicationUser loggedInuser = await GetLoggedInUser().ConfigureAwait(false);
            //Authority "RA" 
            var authorityResp = await _authorityService.ListAsync().ConfigureAwait(false);
            IList<Authority> authoritylist = (IList<Authority>)authorityResp;
            if (AuthorityId == 0)
            {
                ViewData["AuthorityId"] = new SelectList(authoritylist, "ID", "Name");
            }
            else
            {
                //Loop through list and remove all other authorities if authority id is not KRB
                if (loggedInuser.Authority.Code != "krb")
                {
                    var authorityListArray = new List<Authority>();
                    try
                    {
                        foreach (Authority authority in authoritylist)
                        {
                            if (authority.Code.ToLower() == loggedInuser.Authority.Code.ToLower())
                            {
                                //Add to new array
                                authorityListArray.Add(authority);
                            }
                        }
                    }
                    catch (Exception Ex)
                    {

                        _logger.LogError(Ex, $"roadController>PopulateDropDowns :Removal of authority item Error: {Ex.Message} " +
                        $"{Environment.NewLine}");
                    }
                    ViewData["AuthorityId"] = new SelectList(authorityListArray, "ID", "Name", AuthorityId);
                }
                else
                {
                    ViewData["AuthorityId"] = new SelectList(authoritylist, "ID", "Name", AuthorityId);
                }

            }
        }

        // GET: RoadController/Delete/5
        [Authorize(Claims.Permission.Road.View)]
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
                    Road road = new Road();
                    return View(road);
                }
                else
                {
                    var resp = await _roadService.FindByIdAsync(ID).ConfigureAwait(false);
                    var road = resp.Road;
                    if (road == null)
                    {
                        return NotFound();
                    }
                    return View(road);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadController.Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "RoadController.Delete Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.Road.Delete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Delete(long id, [Bind("ID")] Road road)
        {
            try
            {
                bool success = false;
                string msg = "Delete Failed";
                if (id != road.ID)
                {
                    return NotFound();
                }
                //Check if user is from KRB
                ApplicationUser user = await GetLoggedInUser().ConfigureAwait(false);
                if (user.Authority.Code.ToLower() != "krb")
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Only a User from KRB is allowed",
                        Href = Url.Action("Index", "Road")
                    });
                }
                var resp = await _roadService.RemoveAsync(id).ConfigureAwait(false);
                if (resp.Success)
                {
                    success = true;
                    msg = "Road Deleted";
                }
                return Json(new
                {
                    Success = success,
                    Message = msg,
                    Href = Url.Action("Index", "Road")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementController.Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "DisbursementController.Delete Page has reloaded");
                return View(road);
            }
        }

        /// <summary>
        /// Road Section
        /// </summary>
        /// <returns></returns>
        [Authorize(Claims.Permission.RoadSection.Add), Authorize(Claims.Permission.RoadSection.Change)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddEditRoadSection(long ID, long RoadId)
        {
            try
            {

                if (ID == 0)
                {
                    RoadSection roadSection = new RoadSection();
                    roadSection.RoadId = RoadId;
                    await PopulateRoadSectionDropDowns(0, 0, 0, 0).ConfigureAwait(false);
                    return View(roadSection);
                }
                else
                {
                    var resp = await _roadSectionService.FindByIdAsync(ID).ConfigureAwait(false);
                    var roadSection = resp.RoadSection;
                    if (roadSection == null)
                    {
                        return NotFound();
                    }
                    await PopulateRoadSectionDropDowns(roadSection.SurfaceTypeId, roadSection.ConstituencyId,
                        roadSection.KWSParkId, roadSection.MunicipalityId).ConfigureAwait(false);
                    return View(roadSection);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadController.AddEdit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "RoadController.AddEdit Page has reloaded");
                return View();
            }
        }

        // POST: Road/AddEditRoadSection/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.RoadSection.Add), Authorize(Claims.Permission.RoadSection.Change)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddEditRoadSection(long id, [Bind("ID,SectionName,SectionID,StartChainage," +
            "EndChainage,Length,Interval,SurfaceTypeId,SurfaceType2,CW_Surf_Co,ConstituencyId," +
            "RoadId,KWSParkId,MunicipalityId")] RoadSection roadSection)
        {
            try
            {
                bool success = false;
                string msg = null;
                if (id != roadSection.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    ApplicationUser loggedInuser = await GetLoggedInUser().ConfigureAwait(false);
                    //if id=0 then create new
                    if (id == 0)
                    {
                        var resp = await _roadSectionService.FindByDisbursementEntryAsync(roadSection).ConfigureAwait(false);
                        if (resp.RoadSection != null)
                        {
                            string msgErr = $"A similar road section entry exists" +
                            $" No duplicate entries may exists for the same road section for the same authority";
                            ModelState.AddModelError(string.Empty, msgErr);
                            //Detach the first entry before attaching your updated entry
                            var respDetach = await _roadSectionService.DetachFirstEntryAsync(resp.RoadSection).ConfigureAwait(false);
                            //return RedirectToAction("AddEdit",new { id=disbursement.ID});
                            return Json(new
                            {
                                Success = false,
                                Message = msgErr,
                                Href = Url.Action("AddEditRoadSection", "Road", new { id = string.Empty })
                            });
                        }

                        //add kwspark
                        if (roadSection.KWSParkId == 0)
                        {
                            roadSection.KWSParkId = null;
                        }
                        if (roadSection.MunicipalityId == 0)
                        {
                            roadSection.MunicipalityId = null;
                        }

                        var disbursementResp = await _roadSectionService.AddAsync(roadSection).ConfigureAwait(false);
                        if (disbursementResp.Success)
                        {
                            success = true;
                            msg = "Road Section Successfully Added";
                        }
                    }
                    //if id>0 then its an edit
                    if (id > 0)
                    {
                        //Check that if foreink id = 0 then null them
                        if (roadSection.KWSParkId == 0)
                        {
                            roadSection.KWSParkId = null;
                        }
                        if (roadSection.MunicipalityId == 0)
                        {
                            roadSection.MunicipalityId = null;
                        }
                        var roadResp = await _roadSectionService.UpdateAsync(id, roadSection).ConfigureAwait(false);
                        if (roadResp.Success)
                        {
                            success = true;
                            msg = "Road Section Successfully Updated";
                        }
                    }

                    return Json(new
                    {
                        Success = success,
                        Message = msg,
                        Href = Url.Action("RoadSections", "Road", new { RoadId = roadSection.RoadId })
                    });
                }
                return Json(new
                {
                    Success = false,
                    Message = "Model Error",
                    Href = Url.Action("Index", "Road")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadSectionController AddEdit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "RoadController AddEdit Page has reloaded");
                await PopulateRoadSectionDropDowns(roadSection.SurfaceTypeId, roadSection.ConstituencyId,
                    roadSection.KWSParkId, roadSection.MunicipalityId).ConfigureAwait(false);
                return View(roadSection);
            }
        }

        private async Task PopulateRoadSectionDropDowns(long SurfaceTypeId,
            long? ConstituencyId, long? KWSParkId, long? MunicipalityId)
        {

            //Surface Type
            var surfaceTypeResp = await _surfaceTypeService.ListAsync().ConfigureAwait(false);
            IList<SurfaceType> surfaceTypelist = (IList<SurfaceType>)surfaceTypeResp.SurfaceType;
            if (SurfaceTypeId == 0)
            {
                ViewData["SurfaceTypeId"] = new SelectList(surfaceTypelist, "ID", "Name");
            }
            else
            {
                ViewData["SurfaceTypeId"] = new SelectList(surfaceTypelist, "ID", "Name", SurfaceTypeId);
            }

            //Constituency
            var constituencyResp = await _constituencyService.ListAsync().ConfigureAwait(false);
            IList<Constituency> constituencylist = (IList<Constituency>)constituencyResp.ConstituencyList;
            if (ConstituencyId == 0)
            {
                ViewData["ConstituencyId"] = new SelectList(constituencylist, "ID", "Name");
            }
            else
            {
                ViewData["ConstituencyId"] = new SelectList(constituencylist, "ID", "Name", ConstituencyId);
            }

            //KWS Park
            var kwsParkResp = await _kWSParkService.ListAsync().ConfigureAwait(false);
            IList<KWSPark> kwsParklist = null;
            if (kwsParkResp.Success)
            {
                var objectResult = (ObjectResult)kwsParkResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        kwsParklist = (IList<KWSPark>)result.Value;
                    }
                }
            }

            kwsParklist.Insert(0, new KWSPark());
            if (KWSParkId == 0)
            {
                ViewData["KWSParkId"] = new SelectList(kwsParklist, "ID", "Code");
            }
            else
            {
                ViewData["KWSParkId"] = new SelectList(kwsParklist, "ID", "Code", KWSParkId);
            }

            //Municipality ID
            var municipalityResp = await _municipalityService.ListAsync().ConfigureAwait(false);
            IList<Municipality> municipalitylist = null;
            if (municipalityResp.Success)
            {
                var objectResult = (ObjectResult)municipalityResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        municipalitylist = (IList<Municipality>)result.Value;
                    }
                }
            }
            municipalitylist.Insert(0, new Municipality());
            if (MunicipalityId == 0)
            {
                ViewData["MunicipalityId"] = new SelectList(municipalitylist, "ID", "Name");
            }
            else
            {
                ViewData["MunicipalityId"] = new SelectList(municipalitylist, "ID", "Name", MunicipalityId);
            }
        }

        [Authorize(Claims.Permission.RoadSection.Delete)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DeleteSectionLine(long sectionId)
        {
            bool success = false;
            string msg = "Delete Failed";
            try
            {

                //Check if user is from KRB
                ApplicationUser user = await GetLoggedInUser().ConfigureAwait(false);
                if (user.Authority.Code.ToLower() != "krb")
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Only a User from KRB is allowed",
                        Href = Url.Action("Index", "Road")
                    });
                }

                var resp = await _roadSectionService.RemoveAsync(sectionId).ConfigureAwait(false);
                if (resp.Success)
                {
                    success = true;
                    msg = "Road Section Deleted";
                }
                return Json(new
                {
                    Success = success,
                    Message = msg,
                    Href = Url.Action("Index", "Road")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadController.RoadSectionDelete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                //ModelState.AddModelError(string.Empty, "RoadController.RoadSectionDelete Page has reloaded");
                return Json(new
                {
                    Success = success,
                    Message = msg,
                    Href = Url.Action("Index", "Road")
                });
            }
        }
        #endregion

        #region InHouse Operations
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
