using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using APRP.Web.Domain.Services;
using APRP.Web.ViewModels;
using APRP.Web.ViewModels.ResponseTypes;
using APRP.Web.ViewModels.Account;

namespace APRP.Web.Controllers
{
    [Authorize]
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutAttribute))]
    public class RoadClassificationsController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger _logger;
        private readonly IUploadService _uploadService;
        private readonly IRoadClassificationService _roadClassificationService;
        private readonly IAuthorityService _authorityService;
        private readonly ISurfaceTypeService _surfaceTypeService;
        private readonly IRoadClassCodeUnitService _roadClassCodeUnitService;
        private readonly IRoadConditionCodeUnitService _roadConditionCodeUnitService;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IApplicationRolesService _applicationRolesService;
        private readonly ICommentService _commentService;
        private readonly ICommunicationService _communicationService;
        public readonly IRoadService _roadService;

        public RoadClassificationsController(
             ILogger<RoadClassificationsController> logger,
             IWebHostEnvironment hostingEnvironment,
              IUploadService uploadService,
              IRoadClassificationService roadClassificationService,
              IAuthorityService authorityService,
              ISurfaceTypeService surfaceTypeService, IRoadClassCodeUnitService roadClassCodeUnitService, IRoadConditionCodeUnitService roadConditionCodeUnitService,
              IApplicationUsersService applicationUsersService, ICommentService commentService, IApplicationRolesService applicationRolesService,
              ICommunicationService communicationService, IRoadService roadService)
        {
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            _uploadService = uploadService;
            _roadClassificationService = roadClassificationService;
            _authorityService = authorityService;
            _surfaceTypeService = surfaceTypeService;
            _roadClassCodeUnitService = roadClassCodeUnitService;
            _roadConditionCodeUnitService = roadConditionCodeUnitService;
            _applicationUsersService = applicationUsersService;
            _commentService = commentService;
            _applicationRolesService = applicationRolesService;
            _communicationService = communicationService;
            _roadService = roadService;
        }


        // GET: RoadClassifications
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RoadClassification.View)]
        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<RoadClassification> roadClassifications = null;
                var user = await GetLoggedInUser().ConfigureAwait(false);
                var userResp = await _applicationUsersService.IsInRoleAsync(user, "Administrators").ConfigureAwait(false);
                var objectResult = (ObjectResult)userResp.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (OkObjectResult)objectResult;
                    AuthResponse authResponse = (AuthResponse)result2.Value;
                    if (authResponse.Success == true || user.Authority.Code == "KRB")
                    {
                        //Return all Road Classifications
                        var resp = await _roadClassificationService.ListAsync().ConfigureAwait(false);
                        roadClassifications = resp.RoadClassification;
                    }
                    else
                    {
                        //Return for authority that user is placed
                        var authority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                        var resp = await _roadClassificationService.ListAsync(authority.Authority.ID).ConfigureAwait(false);
                        roadClassifications = resp.RoadClassification;
                    }
                }

                return View(roadClassifications);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadClassificationsController Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "RoadClassificationsController Index Page has reloaded");
                return View();
            }
        }


        // GET: RoadClassifications/Details/5
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RoadClassification.View)]
        public async Task<IActionResult> Details(long? id)
        {
            try
            {
                RoadClassificationViewModel roadClassificationViewModel = new RoadClassificationViewModel();
                if (id == null)
                {
                    return NotFound();
                }

                long ID;
                bool result = long.TryParse(id.ToString(), out ID);

                var resp = await _roadClassificationService.FindByIdAsync(ID).ConfigureAwait(false);
                var roadClassification = resp.RoadClassification;
                if (roadClassification == null)
                {
                    return NotFound();
                }
                roadClassificationViewModel.RoadClassification = roadClassification;

                //Get comments
                var respComments = await _commentService.ListAsync("road_classification", roadClassification.ID).ConfigureAwait(false);
                roadClassificationViewModel.Comments = respComments.Comment;

                return View(roadClassificationViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadClassificationsController Details Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "RoadClassificationsController Details Page has reloaded");
                return View();
            }
        }

        [HttpPost]
        [RequestSizeLimit(10000000)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RoadClassification.Add)]
        public async Task<IActionResult> DetailsPost()
        {
            try
            {
                RoadClassification roadClassification = new RoadClassification();
                long AuthorityId;
                bool results = long.TryParse(Request.Form["AuthorityId"].ToString(), out AuthorityId);
                roadClassification.AuthorityId = AuthorityId;

                string RoadId = Request.Form["RoadId"].ToString(); roadClassification.RoadId = RoadId;
                string RoadName = Request.Form["RoadName"].ToString(); roadClassification.RoadName = RoadName;

                int RoadClassCodeUnitId;
                results = int.TryParse(Request.Form["RoadClassCodeUnitId"].ToString(), out RoadClassCodeUnitId); roadClassification.RoadClassCodeUnitId = RoadClassCodeUnitId;
                string StartPoint = Request.Form["StartPoint"].ToString(); roadClassification.StartPoint = StartPoint;
                string EndPont = Request.Form["EndPont"].ToString(); roadClassification.EndPont = EndPont;
                double TotalRoadLength;
                results = double.TryParse(Request.Form["TotalRoadLength"].ToString(), out TotalRoadLength); roadClassification.TotalRoadLength = TotalRoadLength;
                bool TraversesMoreThanOneCounty;
                results = bool.TryParse(Request.Form["TraversesMoreThanOneCounty"].ToString(), out TraversesMoreThanOneCounty); roadClassification.TraversesMoreThanOneCounty = TraversesMoreThanOneCounty;
                bool LinkCountyHQtoAnother;
                results = bool.TryParse(Request.Form["LinkCountyHQtoAnother"].ToString(), out LinkCountyHQtoAnother); roadClassification.LinkCountyHQtoAnother = LinkCountyHQtoAnother;
                string ListCountyHQNames = Request.Form["ListCountyHQNames"].ToString(); roadClassification.ListCountyHQNames = ListCountyHQNames;
                bool AccessToPublicFacility;
                results = bool.TryParse(Request.Form["AccessToPublicFacility"].ToString(), out AccessToPublicFacility); roadClassification.AccessToPublicFacility = AccessToPublicFacility;
                string ListPublicFacility = Request.Form["ListPublicFacility"].ToString(); roadClassification.ListPublicFacility = ListPublicFacility;
                bool AccessToGovernmentOffice;
                results = bool.TryParse(Request.Form["AccessToGovernmentOffice"].ToString(), out AccessToGovernmentOffice); roadClassification.AccessToGovernmentOffice = AccessToGovernmentOffice;
                string ListGovernmentOffice = Request.Form["ListGovernmentOffice"].ToString(); roadClassification.ListGovernmentOffice = ListGovernmentOffice;
                long SurfaceTypeId;
                results = long.TryParse(Request.Form["SurfaceTypeId"].ToString(), out SurfaceTypeId); roadClassification.SurfaceTypeId = SurfaceTypeId;
                int RoadConditionCodeUnitId;
                results = int.TryParse(Request.Form["RoadConditionCodeUnitId"].ToString(), out RoadConditionCodeUnitId); roadClassification.RoadConditionCodeUnitId = RoadConditionCodeUnitId;
                int CwayWidthInMeters;
                results = int.TryParse(Request.Form["CwayWidthInMeters"].ToString(), out CwayWidthInMeters); roadClassification.CwayWidthInMeters = CwayWidthInMeters;
                int RoadReserveSizeInMeters;
                results = int.TryParse(Request.Form["RoadReserveSizeInMeters"].ToString(), out RoadReserveSizeInMeters); roadClassification.RoadReserveSizeInMeters = RoadReserveSizeInMeters;

                //Get logged user
                ApplicationUser loggedInuser = await GetLoggedInUser().ConfigureAwait(false);
                roadClassification.AuthorityId = loggedInuser.Authority.ID;
                roadClassification.CreatedBy = loggedInuser.UserName;
                roadClassification.CreationDate = DateTime.UtcNow;

                var resp = await _roadClassificationService.AddAsync(roadClassification).ConfigureAwait(false);
                if (resp.Success)
                {
                    //Upload the files
                    //Check if files injected in request object
                    if (Request.Form.Files.Count > 0)
                    {
                        try
                        {
                            var files = Request.Form.Files;
                            for (int i = 0; i < files.Count; i++)
                            {
                                var file = files[i];
                                string fname;
                                string x = Request.Headers["User-Agent"].ToString();//.Contains("Edge")
                                if (Request.Headers["User-Agent"].ToString().Contains("IE") || Request.Headers["User-Agent"].ToString().Contains("INTERNETEXPLORER")
                                    || Request.Headers["User-Agent"].ToString().Contains("Edge"))
                                {
                                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                    fname = testfiles[testfiles.Length - 1];
                                }
                                else
                                {
                                    fname = file.FileName;
                                }
                                var path = Path.Combine(
                                _hostingEnvironment.WebRootPath, "uploads", "road_classification", fname);

                                using (var fileStream = new FileStream(path, FileMode.Create))
                                {
                                    await file.CopyToAsync(fileStream).ConfigureAwait(false);
                                }

                                //Register with file uploads
                                Upload upload = new Upload();
                                upload.filename = fname;
                                upload.ForeignId = roadClassification.ID;
                                upload.type = "road_classification";

                                var uploadResp = await _uploadService.AddAsync(upload).ConfigureAwait(false);
                            }

                        }
                        catch (Exception Ex)
                        {
                            _logger.LogError(Ex, $"SubmitBudget() File Upload Fails Error: {Ex.Message} " +
                            $"{Environment.NewLine}");
                        }
                    }

                    return Json(new
                    {
                        Success = true,
                        Message = "Success",
                        Href = Url.Action("Index", "RoadClassifications")
                    });
                }
                else
                {
                    return View(roadClassification);
                }
            }
            catch (Exception Ex)
            {

                _logger.LogError($"RoadClassifications.CreatePost Action {Ex.Message}");
                return Json(new
                {
                    Success = false,
                    Message = "Error",
                    Href = Url.Action("Index", "RoadClassifications")
                });
            }
        }


        // GET: RoadClassifications/Create
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RoadClassification.View)]
        public async Task<IActionResult> Create()
        {
            try
            {
                ApplicationUser user = await GetLoggedInUser().ConfigureAwait(false);

                RoadClassification roadClassification = new RoadClassification();
                roadClassification.Authority = user.Authority;

                var resp = await _authorityService.ListAsync().ConfigureAwait(false);
                ViewData["AuthorityId"] = new SelectList(resp, "ID", "Code");

                var surfaceResp = await _surfaceTypeService.ListAsync().ConfigureAwait(false);
                ViewData["SurfaceTypeId"] = new SelectList(surfaceResp.SurfaceType, "ID", "Name");

                var roadclassResp = await _roadClassCodeUnitService.ListAsync().ConfigureAwait(false);
                ViewData["RoadClassCodeUnitId"] = new SelectList(roadclassResp.RoadClassCodeUnit, "ID", "RoadClass");

                var roadCondResp = await _roadConditionCodeUnitService.ListAsync().ConfigureAwait(false);
                ViewData["RoadConditionCodeUnitId"] = new SelectList(roadCondResp.RoadConditionCodeUnit, "ID", "Rate");

                return View(roadClassification);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadClassificationsController Create Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "RoadClassificationsController Create Page has reloaded");
                return View();
            }
        }

        [HttpPost]
        [RequestSizeLimit(10000000)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RoadClassification.Add)]
        public async Task<IActionResult> CreatePost()
        {
            try
            {
                RoadClassification roadClassification = new RoadClassification();
                long AuthorityId;
                bool results = long.TryParse(Request.Form["AuthorityId"].ToString(), out AuthorityId);
                roadClassification.AuthorityId = AuthorityId;

                string RoadId = Request.Form["RoadId"].ToString(); roadClassification.RoadId = RoadId;
                string RoadName = Request.Form["RoadName"].ToString(); roadClassification.RoadName = RoadName;

                int RoadClassCodeUnitId;
                results = int.TryParse(Request.Form["RoadClassCodeUnitId"].ToString(), out RoadClassCodeUnitId); roadClassification.RoadClassCodeUnitId = RoadClassCodeUnitId;
                string StartPoint = Request.Form["StartPoint"].ToString(); roadClassification.StartPoint = StartPoint;
                string EndPont = Request.Form["EndPont"].ToString(); roadClassification.EndPont = EndPont;
                double TotalRoadLength;
                results = double.TryParse(Request.Form["TotalRoadLength"].ToString(), out TotalRoadLength); roadClassification.TotalRoadLength = TotalRoadLength;
                bool TraversesMoreThanOneCounty;
                results = bool.TryParse(Request.Form["TraversesMoreThanOneCounty"].ToString(), out TraversesMoreThanOneCounty); roadClassification.TraversesMoreThanOneCounty = TraversesMoreThanOneCounty;
                bool LinkCountyHQtoAnother;
                results = bool.TryParse(Request.Form["LinkCountyHQtoAnother"].ToString(), out LinkCountyHQtoAnother); roadClassification.LinkCountyHQtoAnother = LinkCountyHQtoAnother;
                string ListCountyHQNames = Request.Form["ListCountyHQNames"].ToString(); roadClassification.ListCountyHQNames = ListCountyHQNames;
                bool AccessToPublicFacility;
                results = bool.TryParse(Request.Form["AccessToPublicFacility"].ToString(), out AccessToPublicFacility); roadClassification.AccessToPublicFacility = AccessToPublicFacility;
                string ListPublicFacility = Request.Form["ListPublicFacility"].ToString(); roadClassification.ListPublicFacility = ListPublicFacility;
                bool AccessToGovernmentOffice;
                results = bool.TryParse(Request.Form["AccessToGovernmentOffice"].ToString(), out AccessToGovernmentOffice); roadClassification.AccessToGovernmentOffice = AccessToGovernmentOffice;
                string ListGovernmentOffice = Request.Form["ListGovernmentOffice"].ToString(); roadClassification.ListGovernmentOffice = ListGovernmentOffice;
                long SurfaceTypeId;
                results = long.TryParse(Request.Form["SurfaceTypeId"].ToString(), out SurfaceTypeId); roadClassification.SurfaceTypeId = SurfaceTypeId;
                int RoadConditionCodeUnitId;
                results = int.TryParse(Request.Form["RoadConditionCodeUnitId"].ToString(), out RoadConditionCodeUnitId); roadClassification.RoadConditionCodeUnitId = RoadConditionCodeUnitId;
                int CwayWidthInMeters;
                results = int.TryParse(Request.Form["CwayWidthInMeters"].ToString(), out CwayWidthInMeters); roadClassification.CwayWidthInMeters = CwayWidthInMeters;
                int RoadReserveSizeInMeters;
                results = int.TryParse(Request.Form["RoadReserveSizeInMeters"].ToString(), out RoadReserveSizeInMeters); roadClassification.RoadReserveSizeInMeters = RoadReserveSizeInMeters;

                //Get logged user
                ApplicationUser loggedInuser = await GetLoggedInUser().ConfigureAwait(false);
                roadClassification.AuthorityId = loggedInuser.Authority.ID;
                roadClassification.CreatedBy = loggedInuser.UserName;
                roadClassification.CreationDate = DateTime.UtcNow;

                var resp = await _roadClassificationService.AddAsync(roadClassification).ConfigureAwait(false);
                if (resp.Success)
                {
                    //Upload the files
                    //Check if files injected in request object
                    if (Request.Form.Files.Count > 0)
                    {
                        try
                        {
                            var files = Request.Form.Files;
                            for (int i = 0; i < files.Count; i++)
                            {
                                var file = files[i];
                                string fname;
                                string x = Request.Headers["User-Agent"].ToString();//.Contains("Edge")
                                if (Request.Headers["User-Agent"].ToString().Contains("IE") || Request.Headers["User-Agent"].ToString().Contains("INTERNETEXPLORER")
                                    || Request.Headers["User-Agent"].ToString().Contains("Edge"))
                                {
                                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                    fname = testfiles[testfiles.Length - 1];
                                }
                                else
                                {
                                    fname = file.FileName;
                                }
                                var path = Path.Combine(
                                _hostingEnvironment.WebRootPath, "uploads", "road_classification", fname);

                                using (var fileStream = new FileStream(path, FileMode.Create))
                                {
                                    await file.CopyToAsync(fileStream).ConfigureAwait(false);
                                }

                                //Register with file uploads
                                Upload upload = new Upload();
                                upload.filename = fname;
                                upload.ForeignId = roadClassification.ID;
                                upload.type = "road_classification";

                                var uploadResp = await _uploadService.AddAsync(upload).ConfigureAwait(false);
                            }

                        }
                        catch (Exception Ex)
                        {
                            _logger.LogError(Ex, $"SubmitBudget() File Upload Fails Error: {Ex.Message} " +
                            $"{Environment.NewLine}");
                        }
                    }

                    return Json(new
                    {
                        Success = true,
                        Message = "Success",
                        Href = Url.Action("Index", "RoadClassifications")
                    });
                }
                else
                {
                    return View(roadClassification);
                }
            }
            catch (Exception Ex)
            {

                _logger.LogError($"RoadClassifications.CreatePost Action {Ex.Message}");
                return Json(new
                {
                    Success = false,
                    Message = "Error",
                    Href = Url.Action("Index", "RoadClassifications")
                });
            }
        }


        // GET: RoadClassifications/Edit/5
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RoadClassification.View)]
        public async Task<IActionResult> Edit(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                var resp = await _roadClassificationService.FindByIdAsync(ID).ConfigureAwait(false);
                var roadClassification = resp.RoadClassification;
                if (roadClassification == null)
                {
                    return NotFound();
                }
                var authorityResp = await _authorityService.ListAsync().ConfigureAwait(false);
                ViewData["AuthorityId"] = new SelectList(authorityResp, "ID", "Code");

                var surfaceResp = await _surfaceTypeService.ListAsync().ConfigureAwait(false);
                ViewData["SurfaceTypeId"] = new SelectList(surfaceResp.SurfaceType, "ID", "Name");

                var roadclassResp = await _roadClassCodeUnitService.ListAsync().ConfigureAwait(false);
                ViewData["RoadClassCodeUnitId"] = new SelectList(roadclassResp.RoadClassCodeUnit, "ID", "RoadClass");

                var roadCondResp = await _roadConditionCodeUnitService.ListAsync().ConfigureAwait(false);
                ViewData["RoadConditionCodeUnitId"] = new SelectList(roadCondResp.RoadConditionCodeUnit, "ID", "Rate");

                return View(roadClassification);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadClassificationsController Edit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "RoadClassificationsController Edit Page has reloaded");
                return View();
            }
        }

        // POST: RoadClassifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RoadClassification.Change)]
        public async Task<IActionResult> Edit(long id, [Bind("ID,CreatedBy,CreationDate,RoadId,RoadName," +
            "RoadClassCodeUnitId,StartPoint,EndPont,TotalRoadLength,TraversesMoreThanOneCounty," +
            "ListCountyNames,LinkCountyHQtoAnother,ListCountyHQNames,LinkSubCountytoAnother,ListSubCountyNames," +
            "AccessToPublicFacility,ListPublicFacility,AccessToGovernmentOffice,ListGovernmentOffice," +
            "RoadConditionCodeUnitId,SurfaceTypeId,CwayWidthInMeters,RoadReserveSizeInMeters," +
            "ApprovedBy,UpdateBy,UpdateDate")] RoadClassification roadClassification)
        {
            try
            {
                if (id != roadClassification.ID)
                {
                    return NotFound();
                }

                //Validate that edit is allowed 
                var resp2 = await _roadClassificationService.FindByIdAsync(id).ConfigureAwait(false);
                if (resp2.Success)
                {
                    if (resp2.RoadClassification.Status == 6 || resp2.RoadClassification.Status == 4 || resp2.RoadClassification.Status == 5)
                    {///Already  6-Added to Road Register 4-Approved//CS Roads Representative in KRB 5-Rejected//CS Roads Representative in KRB
                        ModelState.AddModelError(string.Empty, "Edit operation not allowed");
                        return View(roadClassification);
                    }
                }

                if (ModelState.IsValid)
                {
                    ApplicationUser loggedInuser = await GetLoggedInUser().ConfigureAwait(false);
                    roadClassification.AuthorityId = loggedInuser.AuthorityId;
                    roadClassification.UpdateBy = loggedInuser.UserName;
                    roadClassification.UpdateDate = DateTime.UtcNow;

                    var resp = await _roadClassificationService.Update(id, roadClassification).ConfigureAwait(false);
                    return RedirectToAction(nameof(Index));
                }


                var authorityResp = await _authorityService.ListAsync().ConfigureAwait(false);
                ViewData["AuthorityId"] = new SelectList(authorityResp, "ID", "Code");
                var surfaceResp = await _surfaceTypeService.ListAsync().ConfigureAwait(false);
                ViewData["SurfaceTypeId"] = new SelectList(surfaceResp.SurfaceType, "ID", "Name");
                var roadclassResp = await _roadClassCodeUnitService.ListAsync().ConfigureAwait(false);
                ViewData["RoadClassCodeUnitId"] = new SelectList(roadclassResp.RoadClassCodeUnit, "ID", "RoadClass");
                var roadCondResp = await _roadConditionCodeUnitService.ListAsync().ConfigureAwait(false);
                ViewData["RoadConditionCodeUnitId"] = new SelectList(roadCondResp.RoadConditionCodeUnit, "ID", "Rate");

                return View(roadClassification);

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadClassificationsController Edit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "RoadClassificationsController Edit Page has reloaded");
                return View(roadClassification);
            }
        }


        // GET: RoadClassifications/Delete/5
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RoadClassification.View)]
        public async Task<IActionResult> Delete(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                long ID;
                bool result = long.TryParse(id.ToString(), out ID);
                var resp = await _roadClassificationService.FindByIdAsync(ID).ConfigureAwait(false);
                var roadClassification = resp.RoadClassification;

                if (roadClassification == null)
                {
                    return NotFound();
                }

                return View(roadClassification);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadClassificationsController Edit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "RoadClassificationsController Edit Page has reloaded");
                return View();
            }
        }

        // POST: RoadClassifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RoadClassification.Delete)]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            try
            {
                var resp = await _roadClassificationService.FindByIdAsync(id).ConfigureAwait(false);
                if (!resp.Success)
                {
                    return NotFound();
                }

                //Validate that delete is allowed 
                if (resp.RoadClassification.Status > 1)
                {//RA/County officer 1 = Submitted                       
                    ModelState.AddModelError(string.Empty, "Delete operation not allowed");
                    return View();
                }


                //Check that the status 
                if (resp.RoadClassification.Status == 4 || resp.RoadClassification.Status == 5)
                {
                    string status = "Rejected";
                    if (resp.RoadClassification.Status == 4)
                    {
                        status = "Approved";

                    }
                    ModelState.AddModelError(string.Empty, $"The Request is {status} at KRB and thus cannot be deleted");
                    return View(resp.RoadClassification);
                }

                resp = await _roadClassificationService.RemoveAsync(id).ConfigureAwait(false);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadClassificationsController Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "RoadClassificationsController Delete Page has reloaded");
                return View();
            }
        }

        #region Approval Process
        [Authorize(Claims.Permission.RoadClassification.Submit)]
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> SubmitRoadRequest(long ID, string Comment)
        {
            string msg = ""; bool result = false;
            var user = await GetLoggedInUser().ConfigureAwait(false);
            //retrieve the currentyear budgetHeader
            var resp = await _roadClassificationService.FindByIdAsync(ID).ConfigureAwait(false); //Todo: Fix need to get budget header for the respective fincial year
            if (resp.Success)
            {
                //get road classification
                var roadClassification = resp.RoadClassification;
                roadClassification.Status = 1;//Submitted
                //roadClassification.Comment = Comment;
                ApplicationUser loggedInuser = await GetLoggedInUser().ConfigureAwait(false);
                roadClassification.UpdateBy = loggedInuser.Id;
                roadClassification.UpdateDate = DateTime.UtcNow;

                var updateResp = await _roadClassificationService.Update(roadClassification.ID, roadClassification).ConfigureAwait(false);
                if (!updateResp.Success)
                {
                    msg = "Could not submit for approval, Please contact system administrator";
                    return RedirectToAction("Upload");
                }
                else
                {
                    //Add  comment
                    if (Comment != null)
                    {
                        Comment comment = new Comment();
                        comment.CreatedBy = loggedInuser.UserName;
                        comment.CreationDate = DateTime.UtcNow;
                        comment.Type = "road_classification";
                        comment.ForeignId = updateResp.RoadClassification.ID;
                        comment.MyComment = Comment;
                        var commentResp = await _commentService.AddAsync(comment).ConfigureAwait(false);

                        //Send notification
                        await SendNotification(roadClassification, comment, loggedInuser, "RoadClassification.Submit").ConfigureAwait(false);
                    }
                    result = true;
                    msg = "Sucess";
                }
            }
            else
            {
                msg = $"Could not find the specified road clasification request :{ID}";
            }

            return Json(new
            {
                Success = result,
                Message = msg,
                Href = Url.Action("Details", "RoadClassifications", new { id = ID })
            });

        }

        [Authorize(Claims.Permission.RoadClassification.Approve)]
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ApproveRoadRequest(long ID, string Comment)
        {
            string msg = ""; bool result = false;
            var user = await GetLoggedInUser().ConfigureAwait(false);
            //retrieve the currentyear budgetHeader
            var resp = await _roadClassificationService.FindByIdAsync(ID).ConfigureAwait(false); //Todo: Fix need to get budget header for the respective fincial year
            if (resp.Success)
            {
                //get road classification
                var roadClassification = resp.RoadClassification;

                if (user != null)
                {
                    var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                    user.Authority = userAuthority.Authority;
                }
                if (user.Authority.Code == "KRB")
                {
                    roadClassification.Status = 4;//Approved
                }
                else
                {
                    roadClassification.Status = 2;//Approved
                }

                //roadClassification.Comment = Comment;
                ApplicationUser loggedInuser = await GetLoggedInUser().ConfigureAwait(false);
                roadClassification.UpdateBy = loggedInuser.Id;
                roadClassification.UpdateDate = DateTime.UtcNow;
                roadClassification.ApprovedBy = loggedInuser.Id;

                var updateResp = await _roadClassificationService.Update(roadClassification.ID, roadClassification).ConfigureAwait(false);
                if (!updateResp.Success)
                {
                    msg = "Could not submit for approval, Please contact system administrator";
                    return RedirectToAction("Upload");
                }
                else
                {
                    //Add  comment
                    if (Comment != null)
                    {
                        Comment comment = new Comment();
                        comment.CreatedBy = loggedInuser.UserName;
                        comment.CreationDate = DateTime.UtcNow;
                        comment.Type = "road_classification";
                        comment.ForeignId = updateResp.RoadClassification.ID;
                        comment.MyComment = Comment;
                        var commentResp = await _commentService.AddAsync(comment).ConfigureAwait(false);

                        //Send notification
                        await SendNotification(roadClassification, comment, loggedInuser, "RoadClassification.Approve").ConfigureAwait(false);
                    }
                    result = true;
                    msg = "Sucess";
                }
            }
            else
            {
                msg = $"Could not find the specified road clasification request :{ID}";
            }

            return Json(new
            {
                Success = result,
                Message = msg,
                Href = Url.Action("Details", "RoadClassifications", new { id = ID })
            });

        }

        [Authorize(Claims.Permission.RoadClassification.Approve)]
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> RejectRoadRequest(long ID, string Comment)
        {
            string msg = ""; bool result = false;
            var user = await GetLoggedInUser().ConfigureAwait(false);
            //retrieve the currentyear budgetHeader
            var resp = await _roadClassificationService.FindByIdAsync(ID).ConfigureAwait(false); //Todo: Fix need to get budget header for the respective fincial year
            if (resp.Success)
            {
                //get road classification
                var roadClassification = resp.RoadClassification;
                if (user != null)
                {
                    var userAuthority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                    user.Authority = userAuthority.Authority;
                }
                if (user.Authority.Code == "KRB")
                {
                    roadClassification.Status = 5;//Rejected
                }
                else
                {
                    roadClassification.Status = 3;//Rejected
                }

                //roadClassification.Comment = Comment;
                ApplicationUser loggedInuser = await GetLoggedInUser().ConfigureAwait(false);
                roadClassification.UpdateBy = loggedInuser.Id;
                roadClassification.UpdateDate = DateTime.UtcNow;

                var updateResp = await _roadClassificationService.Update(roadClassification.ID, roadClassification).ConfigureAwait(false);
                if (!updateResp.Success)
                {
                    msg = "Could not reject the road classification request, Please contact system administrator";
                    return RedirectToAction("Upload");
                }
                else
                {
                    //Add  comment
                    if (Comment != null)
                    {
                        Comment comment = new Comment();
                        comment.CreatedBy = loggedInuser.UserName;
                        comment.CreationDate = DateTime.UtcNow;
                        comment.Type = "road_classification";
                        comment.ForeignId = updateResp.RoadClassification.ID;
                        comment.MyComment = Comment;
                        var commentResp = await _commentService.AddAsync(comment).ConfigureAwait(false);

                        //Send notification
                        await SendNotification(roadClassification, comment, loggedInuser, "RoadClassification.Submit").ConfigureAwait(false);
                    }
                    result = true;
                    msg = "Sucess";
                }
            }
            else
            {
                msg = $"Could not find the specified road clasification request :{ID}";
            }

            return Json(new
            {
                Success = result,
                Message = msg,
                Href = Url.Action("Details", "RoadClassifications", new { id = ID })
            });

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task SendNotification(RoadClassification roadClassification, Comment comment, ApplicationUser loggedInuser, string roleName)
        {
            String Email, Subject, HTMLMessage = null;

            Subject = $"Road classification for Road ID: {roadClassification.RoadId}";
            HTMLMessage = $"Road classification for Road ID: {roadClassification.RoadId}" +
                $"{Environment.NewLine} Details: {comment}";

            //Get Authority
            Authority authority = loggedInuser.Authority;

            //Get Users in respective authority as per logged in user
            List<ApplicationUser> users = null;
            var respUsers = await _applicationUsersService.GetAllUsersAsync(authority).ConfigureAwait(false);
            var objectResult = (ObjectResult)respUsers.IActionResult;
            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
            {
                var result = (OkObjectResult)objectResult;
                users = (List<ApplicationUser>)result.Value;

            }

            //Get users to send notification
            if (users != null)
            {
                var UsersToSendNotificationDictionary = new Dictionary<string, ApplicationUser>();
                foreach (var user in users)
                {
                    var respUser2 = await _applicationUsersService.IsInRoleAsync(user, roleName).ConfigureAwait(false);
                    var objectResult2 = (ObjectResult)respUser2.IActionResult;
                    if (objectResult2.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result2 = (OkObjectResult)objectResult2;
                        AuthResponse authResponse = (AuthResponse)result2.Value;
                        if (authResponse.Success)
                        {
                            try
                            {
                                UsersToSendNotificationDictionary.Add(user.Id, user);
                            }
                            catch (Exception Ex)
                            {

                                _logger.LogError(Ex, $"RoadClassifications.SendNotification :{Environment.NewLine}");
                            }
                        }
                    }
                }
                //Loop through dictionary and send email
                SendEmailModel sendEmailModel = new SendEmailModel();
                sendEmailModel.Subject = Subject;
                sendEmailModel.HTMLMessage = HTMLMessage;
                foreach (KeyValuePair<string, ApplicationUser> pair in UsersToSendNotificationDictionary)
                {
                    var notificationUser = pair.Value;
                    Email = notificationUser.Email;
                    sendEmailModel.Email = Email;
                    var respcomm = await _communicationService.SendEmail2(sendEmailModel).ConfigureAwait(false);

                }

            }
        }

        #endregion

        #region Supporting Documents
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> SupportingDocs(long id)
        {
            try
            {
                //Create Model
                RoadClassificationViewModel roadClassificationViewModel = new RoadClassificationViewModel();

                //Get Road Classification
                var resp = await _roadClassificationService.FindByIdAsync(id).ConfigureAwait(false);
                roadClassificationViewModel.RoadClassification = resp.RoadClassification;

                //Set Referer
                roadClassificationViewModel.Referer = Request.Headers["Referer"].ToString();

                return View(roadClassificationViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"RoadClassificationsController.Supporting Docs Action {Ex.Message}");
                return View();
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> UploadSupportingDocs()
        {
            try
            {
                //Create Model
                RoadClassificationViewModel roadClassificationViewModel = new RoadClassificationViewModel();

                //Get RoadId
                long RoadClassificationId;
                bool results = long.TryParse(Request.Form["RoadClassificationId"].ToString(), out RoadClassificationId);

                //Get Road Classification
                var resp = await _roadClassificationService.FindByIdAsync(RoadClassificationId).ConfigureAwait(false);
                roadClassificationViewModel.RoadClassification = resp.RoadClassification;

                //Upload the files
                //Check if files injected in request object
                if (Request.Form.Files.Count > 0)
                {
                    try
                    {
                        var files = Request.Form.Files;
                        for (int i = 0; i < files.Count; i++)
                        {
                            var file = files[i];
                            string fname;
                            string x = Request.Headers["User-Agent"].ToString();//.Contains("Edge")
                            if (Request.Headers["User-Agent"].ToString().Contains("IE") || Request.Headers["User-Agent"].ToString().Contains("INTERNETEXPLORER")
                                || Request.Headers["User-Agent"].ToString().Contains("Edge"))
                            {
                                string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                fname = testfiles[testfiles.Length - 1];
                            }
                            else
                            {
                                fname = file.FileName;
                            }
                            var path = Path.Combine(
                            _hostingEnvironment.WebRootPath, "uploads", "road_classification", fname);

                            using (var fileStream = new FileStream(path, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream).ConfigureAwait(false);
                            }

                            //Register with file uploads
                            Upload upload = new Upload();
                            upload.filename = fname;
                            upload.ForeignId = roadClassificationViewModel.RoadClassification.ID;
                            upload.type = "road_classification";

                            var uploadResp = await _uploadService.AddAsync(upload).ConfigureAwait(false);
                        }

                    }
                    catch (Exception Ex)
                    {
                        _logger.LogError(Ex, $"RoadClassificationsController.UploadSupportingDocs() File Upload Fails Error: {Ex.Message} " +
                        $"{Environment.NewLine}");
                    }
                }

                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Href = Url.Action("SupportingDocs", "RoadClassifications", new { id = RoadClassificationId })
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError($"RoadClassificationsController.UploadSupportingDocs() Action {Ex.Message}");
                return Json(new
                {
                    Success = false,
                    Message = "Error",
                    Href = Url.Action("Index", "RoadClassifications")
                });
            }
        }
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> GetRoadClassificationUploads(string Type, long RoadClassificationId)
        {
            try
            {
                var UploadListResponse = await _uploadService.ListAsync(Type, RoadClassificationId).ConfigureAwait(false);
                IList<Upload> uploads = (IList<Upload>)UploadListResponse.Upload;
                return Json(uploads);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"RoadClassificationsController.GetRoadClassificationUploads Error: {Ex.Message}");
                return Json(null);
            }
        }
        #endregion

        #region Download
        public async Task<IActionResult> DownloadSupportingDocs(string filename, string folder)
        {
            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(
                           _hostingEnvironment.WebRootPath, "uploads",
                           folder, filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory).ConfigureAwait(false);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> DeleteRoadClassificationAttachment(long Id, string filename, long RoadClassificationId)
        {
            try
            {
                Upload upload = null;
                //delete the file
                Boolean FileDelete = DeleteFile(filename, "road_classification");

                var uploadResponse = await _uploadService.RemoveAsync(Id).ConfigureAwait(false);
                upload = uploadResponse.Upload;
                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    data = upload,
                    Href = Url.Action("SupportingDocs", "RoadClassifications", new { id = RoadClassificationId })
                });

            }
            catch (Exception Ex)
            {
                _logger.LogError($"RoadClassificationsController.DeleteRoadClassificationAttachment() Error: {Ex.Message}");
                return Json(null);
            }
        }

        private Boolean DeleteFile(string filename, string subFolder)
        {
            try
            {
                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", subFolder, filename);
                FileInfo File = new FileInfo(path);

                // Check if file exists with its full path    
                if (File.Exists)
                {
                    // If file found, delete it    
                    File.Delete();
                    return false;

                }
                else return false;
            }
            catch (IOException IOExp)
            {
                string msg = $"Error Message: {IOExp.Message.ToString()} \r\n" +
                    $"Inner Exception: {IOExp.InnerException.ToString()} \r\n" +
                    $"Stack Trace:  {IOExp.StackTrace.ToString()}";
                _logger.LogError($"RoadClassificationsController.DeleteFile() Error: \r\n {msg}");
                return false;
            }

        }
        #endregion

        #region GIS Section
        // GET: RoadClassifications/Details/5
        [Authorize(Claims.Permission.RoadClassification.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddRoadtoGIS(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                long ID;
                bool result = long.TryParse(id.ToString(), out ID);

                var resp = await _roadClassificationService.FindByIdAsync(ID).ConfigureAwait(false);
                var roadClassification = resp.RoadClassification;
                if (roadClassification == null)
                {
                    return NotFound();
                }
                return View(roadClassification);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadClassificationsController.AddRoadtoGIS Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "RoadClassificationsController.AddRoadtoGIS Page has reloaded");
                return View();
            }
        }

        // POST: RoadClassifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.RoadClassification.AddRoadtoGIS)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddRoadtoGIS(long id, [Bind("ID")] RoadClassification roadClassification)
        {
            try
            {
                bool msg = false;
                if (id != roadClassification.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    ApplicationUser loggedInuser = await GetLoggedInUser().ConfigureAwait(false);
                    IEnumerable<RoadClassification> roadClassifications = null;
                    var userResp = await _applicationUsersService.IsInRoleAsync(loggedInuser, "Administrators").ConfigureAwait(false);
                    var objectResult = (ObjectResult)userResp.IActionResult;
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result2 = (OkObjectResult)objectResult;
                        AuthResponse authResponse = (AuthResponse)result2.Value;
                        if (authResponse.Success == true || loggedInuser.Authority.Code == "KRB")
                        {
                            //Return all Road Classifications
                            var resp = await _roadClassificationService.ListAsync().ConfigureAwait(false);
                            roadClassifications = resp.RoadClassification;
                        }
                        else
                        {
                            //Return for authority that user is placed
                            var authority = await _authorityService.FindByIdAsync(loggedInuser.AuthorityId).ConfigureAwait(false);
                            var resp = await _roadClassificationService.ListAsync(authority.Authority.ID).ConfigureAwait(false);
                            roadClassifications = resp.RoadClassification;
                        }
                    }


                    //Get road classification details and Add Road ID to Roads Register
                    var resp2 = await _roadClassificationService.FindByIdAsync(id).ConfigureAwait(false);
                    if (resp2.Success)
                    {
                        roadClassification = resp2.RoadClassification;
                        roadClassification.AuthorityId = resp2.RoadClassification.AuthorityId;
                        roadClassification.UpdateBy = loggedInuser.UserName;
                        roadClassification.UpdateDate = DateTime.UtcNow;
                        if (resp2.RoadClassification.Status == 6)  ///Already  6-Added to Road Register
                        {
                            ModelState.AddModelError(string.Empty, "Road Request Classification has been added to Road Register");
                            return View(roadClassification);
                        }
                        else if (resp2.RoadClassification.Status != 4)
                        {
                            ModelState.AddModelError(string.Empty, "Road Request Classification hasn't been approved at KRB yet");
                            return View(roadClassification);
                        }

                        Road road = new Road();
                        road.RoadNumber = resp2.RoadClassification.RoadId;
                        road.RoadName = resp2.RoadClassification.RoadName;
                        road.AuthorityId = resp2.RoadClassification.AuthorityId;
                        var roadsresp = await _roadService.AddAsync(road).ConfigureAwait(false);
                        if (roadsresp.Success)
                        {
                            msg = true;
                            roadClassification.Status = 6;
                            resp2 = await _roadClassificationService.Update(id, roadClassification).ConfigureAwait(false);
                            //if ()
                            //{

                            //}
                        }
                    }

                    return Json(new
                    {
                        Success = msg,
                        Href = Url.Action("Index", "RoadClassifications")
                    });
                    //return RedirectToAction(nameof(Index));
                }
                return Json(new
                {
                    Success = false,
                    Href = Url.Action("Index", "RoadClassifications")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RoadClassificationsController Edit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "RoadClassificationsController Edit Page has reloaded");
                return View(roadClassification);
            }
        }
        
        #endregion

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
