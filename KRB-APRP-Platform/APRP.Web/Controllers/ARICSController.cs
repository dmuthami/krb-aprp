using APRP.Web.ViewModels.CountyVM;
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using APRP.Web.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using APRP.Web.ViewModels.ResponseTypes;
using APRP.Web.ViewModels;
using OfficeOpenXml.Style;
using APRP.Web.Domain.Services.Communication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using APRP.Web.Domain.Models.History;

namespace APRP.Web.Controllers
{
    [Authorize]
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutAttribute))]
    public class ARICSController : Controller
    {
        private readonly IRoadService _roadService;
        private readonly IRoadSectionService _roadSectionService;
        private readonly IRoadSheetService _roadSheetService;
        private readonly ICountyService _countyService;
        private readonly IConstituencyService _constituencyService;
        private readonly IARICSService _aRICSService;
        private readonly IARICSUploadService _aRICSUploadService;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IKenHARoadService _kenHARoadService;
        private readonly IKeRRARoadService _keRRARoadService;
        private readonly IKuRARoadService _kuRARoadService;
        private readonly IKwSRoadService _kwSRoadService;
        private readonly ICountiesRoadService _countiesRoadService;
        private readonly IRoadConditionService _roadConditionService;
        private readonly IGISRoadService _gISRoadService;
        private readonly IARICSApprovalService _aRICSApprovalService;
        private readonly IARICSApprovalLevelService _aRICSApprovalLevelService;
        private readonly IARICSMasterApprovalService _aRICSMasterApprovalService;
        private readonly IARICSYearService _aRICSYearService;
        private readonly IARICSBatchService _aRICSBatchService;




        public IConfiguration Configuration { get; }

        private readonly IMemoryCache _cache;

        private CultureInfo _cultures = new CultureInfo("en-US");

        public ARICSController(IRoadService roadService,
             IRoadSectionService roadSectionService,
             IRoadSheetService roadSheetService,
             ICountyService countyService,
             IConstituencyService constituencyService,
             IARICSService aRICSService,
             IARICSUploadService aRICSUploadService,
             IApplicationUsersService applicationUsersService, IAuthorityService authorityService,
            IConfiguration configuration
            , ILogger<ARICSController> logger
             , IWebHostEnvironment hostingEnvironment, IMemoryCache cache,
            IKenHARoadService kenHARoadService, IKeRRARoadService keRRARoadService,
            IKwSRoadService kwSRoadService, ICountiesRoadService countiesRoadService,
            IKuRARoadService kuRARoadService, IRoadConditionService roadConditionService,
            IGISRoadService gISRoadService, IARICSApprovalService aRICSApprovalService,
            IARICSApprovalLevelService aRICSApprovalLevelService, IARICSMasterApprovalService aRICSMasterApprovalService,
            IARICSYearService aRICSYearService, IARICSBatchService aRICSBatchService)
        {
            Configuration = configuration;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _roadService = roadService;
            _roadSectionService = roadSectionService;
            _roadSheetService = roadSheetService;
            _countyService = countyService;
            _constituencyService = constituencyService;
            _aRICSService = aRICSService;
            _aRICSUploadService = aRICSUploadService;
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _cache = cache;
            _kenHARoadService = kenHARoadService;
            _keRRARoadService = keRRARoadService;
            _kwSRoadService = kwSRoadService;
            _countiesRoadService = countiesRoadService;
            _kuRARoadService = kuRARoadService;
            _roadConditionService = roadConditionService;
            _gISRoadService = gISRoadService;
            _aRICSApprovalService = aRICSApprovalService;
            _aRICSApprovalLevelService = aRICSApprovalLevelService;
            _aRICSMasterApprovalService = aRICSMasterApprovalService;
            _aRICSYearService = aRICSYearService;
            _aRICSBatchService = aRICSBatchService;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region ARICS YEARS CRUD
        // GET: DisbursementController
        [Authorize(Claims.Permission.ARICS.ARICSYear_View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ARICSYearIndex(long? FinancialYearId)
        {
            try
            {
                ARICSYearViewModel aRICSYearViewModel = new ARICSYearViewModel();

                //Get current financial year
                aRICSYearViewModel.Referer = Request.GetEncodedUrl();

                return View(aRICSYearViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSController Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "ARICSController Index Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.ARICS.ARICSYear_Add), Authorize(Claims.Permission.ARICS.ARICSYear_Change)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ARICSYearAddEdit(int? id)
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
                    ARICSYear aRICSYear = new ARICSYear();
                    return View(aRICSYear);
                }
                else
                {
                    ARICSYear aRICSYear = null;
                    var resp = await _aRICSYearService.FindByIdAsync(ID).ConfigureAwait(false);
                    if (resp.Success)
                    {
                        var objectResult = (ObjectResult)resp.IActionResult;
                        if (objectResult != null)
                        {
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var result2 = (OkObjectResult)objectResult;
                                aRICSYear = (ARICSYear)result2.Value;
                            }
                        }
                    }
                    if (aRICSYear == null)
                    {
                        return NotFound();
                    }
                    return View(aRICSYear);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"DisbursementController.AddEdit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "DisbursementController.AddEdit Page has reloaded");
                return View();
            }
        }

        // POST: ARICS/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.ARICS.ARICSYear_Add), Authorize(Claims.Permission.ARICS.ARICSYear_Change)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ARICSYearAddEdit(long id, [Bind("ID,Year," +
            "Description")] ARICSYear aRICSYear)
        {
            try
            {
                bool success = false;
                string msg = null;
                if (id != aRICSYear.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    ApplicationUser loggedInuser = await GetLoggedInUser().ConfigureAwait(false);
                    //if id=0 then create new
                    if (id == 0)
                    {
                        var resp = await _aRICSYearService.FindByYearAsync(aRICSYear.ID).ConfigureAwait(false);

                        ARICSYear aRICSYearSearch = null;
                        if (resp.Success)
                        {
                            var objectResult = (ObjectResult)resp.IActionResult;
                            if (objectResult != null)
                            {
                                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    var result2 = (OkObjectResult)objectResult;
                                    aRICSYearSearch = (ARICSYear)result2.Value;
                                }
                            }
                        }

                        if (aRICSYearSearch != null)
                        {
                            string msgErr = $"A similar ArICSYear entry exists" +
                            $" No duplicate entries may exists for the same ARICSYear";
                            ModelState.AddModelError(string.Empty, msgErr);

                            //Detach the first entry before attaching your updated entry
                            var respDetach = await _aRICSYearService.DetachFirstEntryAsync(aRICSYearSearch).ConfigureAwait(false);
                            return Json(new
                            {
                                Success = false,
                                Message = msgErr,
                                Href = Url.Action("ARICSYearAddEdit", "ARICS", new { id = string.Empty })
                            });
                        }

                        //add disbursement
                        var disbursementResp = await _aRICSYearService.AddAsync(aRICSYear).ConfigureAwait(false);
                        if (disbursementResp.Success)
                        {
                            success = true;
                            msg = "ARICS Year Successfully Added";
                        }
                    }
                    //if id>0 then its an edit
                    if (id > 0)
                    {
                        var disbursementResp = await _aRICSYearService.Update(aRICSYear.ID, aRICSYear).ConfigureAwait(false);
                        if (disbursementResp.Success)
                        {
                            success = true;
                            msg = "ARICSYear Successfully Updated";
                        }
                    }

                    return Json(new
                    {
                        Success = success,
                        Message = msg,
                        Href = Url.Action("ARICSYearIndex", "ARICS")
                    }); ;
                }
                return Json(new
                {
                    Success = false,
                    Message = "Model Error",
                    Href = Url.Action("ARICSYearIndex", "ARICS")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSController AddEdit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "ARICSController AddEdit Page has reloaded");
                return View(aRICSYear);
            }
        }

        [Authorize(Claims.Permission.ARICS.ARICSYear_View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ARICSYearActivities()
        {
            try
            {
                ARICSYearViewModel aRICSYearViewModel = new ARICSYearViewModel();

                //Get current financial year
                aRICSYearViewModel.Referer = Request.GetEncodedUrl();

                //Get list of years
                IList<ARICSYear> aRICSYears = null;
                var resp = await _aRICSYearService.ListAsync().ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result2 = (OkObjectResult)objectResult;
                            aRICSYears = (IList<ARICSYear>)result2.Value;
                        }
                    }
                }
                aRICSYearViewModel.ARICSYears = aRICSYears;

                return PartialView("ARICSYearPartialView", aRICSYearViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSController Index Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "ARICSController Index Page has reloaded");
                return View();
            }
        }

        // GET: ReleaseController/Details/5
        [Authorize(Claims.Permission.ARICS.ARICSYear_View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ARICSYearDelete(long? id)
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
                    ARICSYear aRICSYear = new ARICSYear();
                    return View(aRICSYear);
                }
                else
                {
                    ARICSYear aRICSYear = null;
                    var resp = await _aRICSYearService.FindByIdAsync(ID).ConfigureAwait(false);
                    if (resp.Success)
                    {
                        var objectResult = (ObjectResult)resp.IActionResult;
                        if (objectResult != null)
                        {
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var result2 = (OkObjectResult)objectResult;
                                aRICSYear = (ARICSYear)result2.Value;
                            }
                        }
                    }
                    if (aRICSYear == null)
                    {
                        return NotFound();
                    }
                    return View(aRICSYear);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSController.Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "ARICSController.Delete Page has reloaded");
                return View();
            }
        }

        [Authorize(Claims.Permission.ARICS.ARICSYear_Delete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ARICSYearDelete(long id, [Bind("ID")] ARICSYear aRICSYear)
        {
            try
            {
                bool success = false;
                string msg = "Delete Failed";
                if (id != aRICSYear.ID)
                {
                    return NotFound();
                }
                var resp = await _aRICSYearService.RemoveAsync(aRICSYear.ID).ConfigureAwait(false);
                if (resp.Success)
                {
                    success = true;
                    msg = "ARICSYear Deleted";
                }
                return Json(new
                {
                    Success = success,
                    Message = msg,
                    Href = Url.Action("ARICSYearIndex", "ARICS")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSYearController.Delete Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "ARICSYearController.Delete Page has reloaded");
                return View(aRICSYear);
            }
        }

        #endregion

        #region Utilities
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> GetCounty()
        {
            try
            {
                List<County> _County = (List<County>)await _countyService.ListAsync().ConfigureAwait(false);
                IList<CountyViewModel> countyViewModel;
                countyViewModel = _County.Select(u => new CountyViewModel
                {
                    ID = u.ID,
                    Name = u.Name
                }
                ).ToList();
                return Json(countyViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"ARICSController.GetCounty API Error {Ex.Message}");
                return Json(null);
            }
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> GetConstituenciesForCounty(string selectedCountyID)
        {
            try
            {
                long myid = -1;
                CountyViewModel _CountyViewModel = new CountyViewModel();
                bool result = long.TryParse(selectedCountyID, out myid);
                _CountyViewModel.ID = myid;

                IList<Constituency> _Constituencies = (IList<Constituency>)await _constituencyService.GetConstituenciesForCounty(_CountyViewModel).ConfigureAwait(false);

                IList<ConstituencyViewModel> constituencyList;
                constituencyList = _Constituencies.Select(u => new ConstituencyViewModel
                {
                    ID = u.ID,
                    Name = u.Name
                }
                ).ToList();
                return Json(constituencyList);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"ARICSController.GetConstituenciesForCounty Error{Ex.Message}");
                return Json(null);
            }
        }
        #endregion

        #region Road Sections

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> GetRoads()
        {
            try
            {
                var user = await GetLoggedInUser().ConfigureAwait(false);
                var roadListResponse = await _roadService.GetRoadWithSectionsAsync(user.Authority, null).ConfigureAwait(false);
                var _Road = ((IList<Road>)roadListResponse.Roads).OrderBy(o => o.RoadNumber);

                IList<RoadViewModel> roadViewModel;
                roadViewModel = _Road.Select(u => new RoadViewModel
                {
                    id = u.ID,
                    name = u.RoadNumber
                }
                ).ToList();
                return Json(roadViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"ARICSController.GetRoads API Error {Ex.Message}");
                return Json(null);
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]

        [HttpPost]
        public async Task<JsonResult> OnGetRoadSections(int ARICSYear)
        {
            try
            {
                IQueryable<RoadSectionViewModel> roadSectionData = null;
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
                        //var roadSectionResponse = await _roadSectionService.ListViewAsync().ConfigureAwait(false);
                        //roadSectionData = roadSectionResponse.RoadSections;

                        //Return all Road Sections
                        roadSectionData = await GetARICEDRoadSections(null,ARICSYear).ConfigureAwait(false);
                    }
                    else
                    {
                        //Return for authority that user is placed
                        var authority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                        //var roadSectionResponse = await _roadSectionService.ListViewAsync(authority.Authority).ConfigureAwait(false);
                        //roadSectionData = roadSectionResponse.RoadSections;

                        //Return for authority that user is placed
                        roadSectionData = await GetARICEDRoadSections(authority.Authority, ARICSYear).ConfigureAwait(false);
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
                            roadSectionData = roadSectionData.OrderByDescending(s => s.road_number);
                        }
                        else if (sortColumn == "section_name")
                        {
                            roadSectionData = roadSectionData.OrderByDescending(s => s.section_name);
                        }
                        else if (sortColumn == "length")
                        {
                            roadSectionData = roadSectionData.OrderByDescending(s => s.length);
                        }

                    }
                    else
                    {
                        if (sortColumn == "road_number")
                        {
                            roadSectionData = roadSectionData.OrderByDescending(s => s.road_number);
                        }
                        else if (sortColumn == "section_name")
                        {
                            roadSectionData = roadSectionData.OrderByDescending(s => s.section_name);
                        }
                        else if (sortColumn == "length")
                        {
                            roadSectionData = roadSectionData.OrderByDescending(s => s.length);
                        }
                    }
                }
                int x = 0;
                IEnumerable<RoadSectionViewModel> roadSectionsEnum = roadSectionData.AsEnumerable<RoadSectionViewModel>();
                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    roadSectionsEnum = roadSectionsEnum
                        .Where(
                         m => (m.section_name != null && m.section_name.ToLower().Contains(searchValue.ToLower()))
                        || (m.road_number != null && m.road_number.ToString().ToLower().Contains(searchValue.ToLower()))
                        || (m.length != null && m.length.ToString().ToLower().Contains(searchValue.ToLower()))
                    );
                }

                //total number of rows count 
                recordsTotal = roadSectionsEnum.Count();
                //Paging 
                var data = roadSectionsEnum.Skip(skip).Take(pageSize).ToList();

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

        private async Task<IQueryable<RoadSectionViewModel>> GetARICEDRoadSections(Authority authority,int ARICSYear)
        {
            //get ariced roads
            var resp = await _aRICSService.GetARICEDRoadSections(ARICSYear).ConfigureAwait(false);
            IEnumerable<RoadSection> roadSections = null;
            if (authority == null)
            {
                roadSections = resp.RoadSectionList;
            }
            else
            {
                roadSections = resp.RoadSectionList.Where(x => x.Road.AuthorityId == authority.ID);
            }

            //Return iqueryable
            IQueryable<RoadSection> x = null;
            x = roadSections.AsQueryable<RoadSection>();

            IQueryable<RoadSectionViewModel> RoadSectionsData;
            RoadSectionsData =
             from roadsections
             in x
             select new RoadSectionViewModel()
             {
                 id = roadsections.ID,
                 road_number = roadsections.Road.RoadNumber,
                 section_name = roadsections.SectionName,
                 length = Math.Round(roadsections.Length, 3, MidpointRounding.AwayFromZero)
             };

            return RoadSectionsData;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [HttpPost]
        public async Task<JsonResult> OnGetRoadSectionsKRB(long AuthorityId,int ARICSYear)
        {
            try
            {
                IQueryable<RoadSectionViewModel> roadSectionData = null;
                var user = await GetLoggedInUser().ConfigureAwait(false);

                //Return for authority that user is placed
                var authority = await _authorityService.FindByIdAsync(AuthorityId).ConfigureAwait(false);

                //var roadSectionResponse = await _roadSectionService.ListViewAsync(authority.Authority).ConfigureAwait(false);
                //roadSectionData = roadSectionResponse.RoadSections;

                roadSectionData = await GetARICEDRoadSections(authority.Authority, ARICSYear).ConfigureAwait(false);

                //var roadSectionResponse = await _aRICSService.GetARICEDRoadSection(authority.Authority,null).ConfigureAwait(false);
                //roadSectionData = roadSectionResponse.RoadSections;

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
                            roadSectionData = roadSectionData.OrderByDescending(s => s.road_number);
                        }
                        else if (sortColumn == "section_name")
                        {
                            roadSectionData = roadSectionData.OrderByDescending(s => s.section_name);
                        }
                        else if (sortColumn == "length")
                        {
                            roadSectionData = roadSectionData.OrderByDescending(s => s.length);
                        }

                    }
                    else
                    {
                        if (sortColumn == "road_number")
                        {
                            roadSectionData = roadSectionData.OrderByDescending(s => s.road_number);
                        }
                        else if (sortColumn == "section_name")
                        {
                            roadSectionData = roadSectionData.OrderByDescending(s => s.section_name);
                        }
                        else if (sortColumn == "length")
                        {
                            roadSectionData = roadSectionData.OrderByDescending(s => s.length);
                        }
                    }
                }
                int x = 0;
                IEnumerable<RoadSectionViewModel> roadSectionsEnum = roadSectionData.AsEnumerable<RoadSectionViewModel>();
                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    roadSectionsEnum = roadSectionsEnum
                        .Where(
                         m => (m.section_name != null && m.section_name.ToLower().Contains(searchValue.ToLower()))
                        || (m.road_number != null && m.road_number.ToString().ToLower().Contains(searchValue.ToLower()))
                        || (m.length != null && m.length.ToString().ToLower().Contains(searchValue.ToLower()))
                    );
                }

                //total number of rows count 
                recordsTotal = roadSectionsEnum.Count();
                //Paging 
                var data = roadSectionsEnum.Skip(skip).Take(pageSize).ToList();

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> GetRoadSectionsForSpecificRoad(string SelectedRoadID)
        {
            try
            {
                long myid = -1;
                RoadViewModel _RoadViewModel = new RoadViewModel();
                bool result = long.TryParse(SelectedRoadID, out myid);
                _RoadViewModel.id = myid;


                var roadSectionListResponse = await _roadSectionService.ListByRoadIdAsync(myid).ConfigureAwait(false);
                IList<RoadSection> _RoadSection = (IList<RoadSection>)roadSectionListResponse.RoadSectionList;

                IList<RoadSectionViewModel> roadSectionList;
                roadSectionList = _RoadSection.Select(u => new RoadSectionViewModel
                {
                    id = u.ID,
                    SectionName = u.SectionName,
                    SectionID = u.SectionID,
                    section_surface_type = u.SurfaceType.Name
                }
                ).ToList();
                return Json(roadSectionList);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"ARICSController.GetRoadSectionsForSpecificRoad Error{Ex.Message}");
                return Json(null);
            }
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> GetRoadSectionLength(string RoadSectionID)
        {
            try
            {
                long sectionID = 0;
                bool result = long.TryParse(RoadSectionID, out sectionID);
                var roadSectionResponse = await _roadSectionService.FindByIdAsync(sectionID).ConfigureAwait(false);
                RoadSection _RoadSection = roadSectionResponse.RoadSection;
                return Json(_RoadSection);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"ARICSController.GetRoadSectionLength Error: {Ex.Message}");
                return Json(null);
            }
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> GetRoadSheetsForSpecificYear(long RoadSectionId, int Year)
        {
            try
            {
                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Href = $"/ARICZ/ARICSummary?RoadSectionId={RoadSectionId}&Year={Year}"
                });

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSController.GetRoadSheetsForSpecificYear Error {Environment.NewLine}");
                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Href = $"/ARICZ/ARICSummary?RoadSectionId={RoadSectionId}&Year={Year}"
                });
            }
        }

        #endregion

        #region ARICS Approval Workflows

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [HttpPost]
        public async Task<JsonResult> OnGetARICSMasterApprovalList(long AuthorityId, int ARICSYearId)
        {
            try
            {
                var user = await GetLoggedInUser().ConfigureAwait(false);

                //Return for authority that user is placed
                var authority = await _authorityService.FindByIdAsync(AuthorityId).ConfigureAwait(false);
                IQueryable<ARICSMasterApprovalViewModel> ARICSMasterApprovalData = null;
                var aRICSMasterApprovalResponse = await _aRICSMasterApprovalService.ListByAuthorityAndARICSYearAsync(authority.Authority.ID, ARICSYearId).ConfigureAwait(false);

                var objectResult = (ObjectResult)aRICSMasterApprovalResponse.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result2 = (OkObjectResult)objectResult;
                        ARICSMasterApprovalData = (IQueryable<ARICSMasterApprovalViewModel>)result2.Value;
                    }
                }


                var roadSectionData = ARICSMasterApprovalData;

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
                        if (sortColumn == "batchno")
                        {
                            roadSectionData = roadSectionData.OrderByDescending(s => s.batchno);
                        }
                        else if (sortColumn == "description")
                        {
                            roadSectionData = roadSectionData.OrderByDescending(s => s.description);
                        }

                    }
                    else
                    {
                        if (sortColumn == "batchno")
                        {
                            roadSectionData = roadSectionData.OrderByDescending(s => s.batchno);
                        }
                        else if (sortColumn == "description")
                        {
                            roadSectionData = roadSectionData.OrderByDescending(s => s.description);
                        }
                    }
                }
                int x = 0;
                IEnumerable<ARICSMasterApprovalViewModel> roadSectionsEnum = roadSectionData.AsEnumerable<ARICSMasterApprovalViewModel>();
                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    roadSectionsEnum = roadSectionsEnum
                        .Where(
                         m => (m.batchno != null && m.batchno.ToLower().Contains(searchValue.ToLower()))
                        || (m.description != null && m.description.ToString().ToLower().Contains(searchValue.ToLower()))
                    );
                }

                //total number of rows count 
                recordsTotal = roadSectionsEnum.Count();
                //Paging 
                var data = roadSectionsEnum.Skip(skip).Take(pageSize).ToList();

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

        #endregion

        #region ARICS Reports

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> GetAuthorities()
        {
            try
            {
                var authority = await _authorityService.ListAsync().ConfigureAwait(false);

                IList<AuthorityViewModel> authorityViewModel;
                authorityViewModel = authority.Select(u => new AuthorityViewModel
                {
                    ID = u.ID,
                    Name = u.Name
                }
                ).ToList();
                return Json(authorityViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"ARICSController.GetRoads API Error {Ex.Message}");
                return Json(null);
            }
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> GetPavedRoads(long AuthorityID, string SurfaceType)
        {
            try
            {

                if (AuthorityID == 0)
                {
                    var user = await GetLoggedInUser().ConfigureAwait(false);
                    var roadListResponse = await _roadService.GetRoadWithSectionsAsync(user.Authority, SurfaceType).ConfigureAwait(false);
                    var _Road = ((IList<Road>)roadListResponse.Roads

                        )
                        .OrderBy(o => o.RoadNumber);

                    IList<RoadViewModel> roadViewModel;
                    roadViewModel = _Road.Select(u => new RoadViewModel
                    {
                        id = u.ID,
                        name = u.RoadNumber
                    }
                    ).ToList();
                    return Json(roadViewModel);
                }
                else
                {
                    var authorityResp = await _authorityService.FindByIdAsync(AuthorityID).ConfigureAwait(false);
                    if (authorityResp.Authority != null)
                    {
                        var roadListResponse = await _roadService.GetRoadWithSectionsAsync(authorityResp.Authority, SurfaceType).ConfigureAwait(false);
                        var _Road = ((IList<Road>)roadListResponse.Roads).OrderBy(o => o.RoadNumber);

                        IList<RoadViewModel> roadViewModel;
                        roadViewModel = _Road.Select(u => new RoadViewModel
                        {
                            id = u.ID,
                            name = u.RoadNumber
                        }
                        ).ToList();
                        return Json(roadViewModel);
                    }
                    else
                    {
                        return Json(null);
                    }
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError($"ARICSController.GetRoads API Error {Ex.Message}");
                return Json(null);
            }
        }


        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> ExportARICSSumUnpaved(long SectionID, int? Year)
        {
            try
            {
                //Get roadsheets for the section for the specified year
                var roadSheetListResponse = await _roadSheetService.ListByRoadSectionIdAsync(SectionID, Year).ConfigureAwait(false);
                IEnumerable<RoadSheet> RoadSheet = (IList<RoadSheet>)roadSheetListResponse.RoadSheets;

                //Get Roadsection
                var roadSectionResp = await _roadSectionService.FindByIdAsync(SectionID).ConfigureAwait(false);
                RoadSection roadsection = roadSectionResp.RoadSection;

                //Write to Excel
                MemoryStream stream = new MemoryStream();
                JsonResult myjson = await WriteToExcelARICSSumUnpaved(RoadSheet, roadsection, stream).ConfigureAwait(false);

                return Json(myjson);

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSController.ExportARICSSummaryReport Error {Environment.NewLine}");
                return Json(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<JsonResult> WriteToExcelARICSSumUnpaved(IEnumerable<RoadSheet> roadSheets, RoadSection roadSection, MemoryStream stream)
        {
            //Create a new ExcelPackage  
            string handle = Guid.NewGuid().ToString();
            using (ExcelPackage excelPackage = new ExcelPackage(stream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = "KRB";
                excelPackage.Workbook.Properties.Title = "ARICS_Summary_Report";
                excelPackage.Workbook.Properties.Subject = "ARICS Summary Report";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("ARICS SUM-Unpaved");

                //Add some text to cell A1
                worksheet.Cells["A4"].Value = "ROAD CONDITION SURVEY - SUMMARY SHEET - UNPAVED";
                worksheet.Cells["A4:Q4"].Merge = true;
                worksheet.Cells["A4:Q4"].Style.Font.Size = 14;
                worksheet.Cells["A4:Q4"].Style.Font.Name = "Arial";
                worksheet.Cells["A4:Q4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A4:Q4"].Style.Font.Bold = true;

                //worksheet.Cells["A4:Q4"].Style.Font.UnderLine = true;

                worksheet.Cells["A6"].Value = "RA/County";
                worksheet.Cells["A6:B6"].Merge = true;
                worksheet.Cells["A6:B6"].Style.Font.Size = 10;
                worksheet.Cells["A6:B6"].Style.Font.Name = "Arial";

                //Get county
                var countyResp = await _countyService.FindByIdAsync(roadSection.Constituency.CountyId).ConfigureAwait(false);
                worksheet.Cells["C6"].Value = countyResp.County.Name;
                worksheet.Cells["C6:O6"].Merge = true;
                worksheet.Cells["C6:O6"].Style.Font.Size = 10;
                worksheet.Cells["C6:O6"].Style.Font.Name = "Arial";
                worksheet.Cells["C6:O6"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["C6:O6"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["C6:O6"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["C6:O6"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["S6"].Value = "Constituency";
                worksheet.Cells["S6:U6"].Merge = true;
                worksheet.Cells["S6:U6"].Style.Font.Size = 10;
                worksheet.Cells["S6:U6"].Style.Font.Name = "Arial";

                worksheet.Cells["V6"].Value = roadSection.Constituency.Name;
                worksheet.Cells["V6:AD6"].Merge = true;
                worksheet.Cells["V6:AD6"].Style.Font.Size = 10;
                worksheet.Cells["V6:AD6"].Style.Font.Name = "Arial";
                worksheet.Cells["V6:AD6"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["V6:AD6"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["V6:AD6"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["V6:AD6"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["A8"].Value = "ROAD ID";
                worksheet.Cells["A8:B8"].Merge = true;
                worksheet.Cells["A8:B8"].Style.Font.Size = 10;
                worksheet.Cells["A8:B8"].Style.Font.Name = "Arial";

                worksheet.Cells["C8"].Value = roadSection.Road.RoadNumber;
                worksheet.Cells["C8:E8"].Merge = true;
                worksheet.Cells["C8:E8"].Style.Font.Size = 10;
                worksheet.Cells["C8:E8"].Style.Font.Name = "Arial";
                worksheet.Cells["C8:E8"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["C8:E8"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["C8:E8"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["C8:E8"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["G8"].Value = "ROAD (SECTION) NAME:";
                worksheet.Cells["G8:K8"].Merge = true;
                worksheet.Cells["G8:K8"].Style.Font.Size = 10;
                worksheet.Cells["G8:K8"].Style.Font.Name = "Arial";

                worksheet.Cells["L8"].Value = roadSection.SectionName;
                worksheet.Cells["L8:R8"].Merge = true;
                worksheet.Cells["L8:R8"].Style.Font.Size = 10;
                worksheet.Cells["L8:R8"].Style.Font.Name = "Arial";
                worksheet.Cells["L8:R8"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L8:R8"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L8:R8"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L8:R8"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["S8"].Value = "ROAD (section) LENGTH (km):";
                worksheet.Cells["S8:V8"].Merge = true;
                worksheet.Cells["S8:V8"].Style.Font.Size = 10;
                worksheet.Cells["S8:V8"].Style.Font.Name = "Arial";
                worksheet.Cells["W8"].Value = roadSection.Length;
                worksheet.Cells["W8:X8"].Merge = true;
                worksheet.Cells["W8:X8"].Style.Font.Size = 10;
                worksheet.Cells["W8:X8"].Style.Font.Name = "Arial";
                worksheet.Cells["W8:X8"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["W8:X8"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["W8:X8"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["W8:X8"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                //Year
                worksheet.Cells["Y8"].Value = "Year";
                worksheet.Cells["Y8:AA8"].Merge = true;
                worksheet.Cells["Y8:AA8"].Style.Font.Size = 10;
                worksheet.Cells["Y8:AA8"].Style.Font.Name = "Arial";
                worksheet.Cells["AB8"].Value = ((IList<RoadSheet>)roadSheets)[0].Year;
                worksheet.Cells["AB8:AD8"].Merge = true;
                worksheet.Cells["AB8:AD8"].Style.Font.Size = 10;
                worksheet.Cells["AB8:AD8"].Style.Font.Name = "Arial";
                worksheet.Cells["AB8:AD8"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["AB8:AD8"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["AB8:AD8"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["AB8:AD8"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["E10"].Value = "ON/OFF-CARRIAGEWAY";
                worksheet.Cells["E10:I10"].Merge = true;
                worksheet.Cells["E10:I10"].Style.Font.Size = 10;
                worksheet.Cells["E10:I10"].Style.Font.Name = "Arial";
                worksheet.Cells["E10:I10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells["A11"].Value = "Sheet";
                worksheet.Cells["A11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B11"].Value = "Length";
                worksheet.Cells["B11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B12"].Value = "Km";
                worksheet.Cells["B11:C11"].Merge = true;
                worksheet.Cells["B11:C11"].Style.Font.Size = 10;
                worksheet.Cells["B11:C11"].Style.Font.Name = "Arial";
                worksheet.Cells["E11"].Value = "(Rate of Deterioration)";
                worksheet.Cells["E11:I11"].Merge = true;
                worksheet.Cells["E11:I11"].Style.Font.Size = 10;
                worksheet.Cells["E11:I11"].Style.Font.Name = "Arial";
                worksheet.Cells["E11:I11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["K11"].Value = "Murram";

                worksheet.Cells["L11"].Value = "Culverts";
                worksheet.Cells["L11:Q11"].Merge = true;
                worksheet.Cells["L11:Q11"].Style.Font.Size = 10;
                worksheet.Cells["L11:Q11"].Style.Font.Name = "Arial";
                worksheet.Cells["L11:Q11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells["S11"].Value = "PRIORITY FOR SPOT IMPROVEMENT";
                worksheet.Cells["S11:AD11"].Merge = true;
                worksheet.Cells["S11:AD11"].Style.Font.Size = 10;
                worksheet.Cells["S11:AD11"].Style.Font.Name = "Arial";
                worksheet.Cells["S11:AD11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells["A12"].Value = "No";
                worksheet.Cells["A12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B12"].Value = "Km";
                worksheet.Cells["B12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B12:C12"].Merge = true;
                worksheet.Cells["B12:C12"].Style.Font.Size = 10;
                worksheet.Cells["B12:C12"].Style.Font.Name = "Arial";
                worksheet.Cells["E12"].Value = "1";
                worksheet.Cells["F12"].Value = "2";
                worksheet.Cells["G12"].Value = "3";
                worksheet.Cells["H12"].Value = "4";
                worksheet.Cells["I12"].Value = "5";
                worksheet.Cells["K12"].Value = "Stacks";
                worksheet.Cells["L12"].Value = "N";
                worksheet.Cells["M12"].Value = "RR";
                worksheet.Cells["N12"].Value = "HR";
                worksheet.Cells["O12"].Value = "NH";
                worksheet.Cells["P12"].Value = "G";
                worksheet.Cells["Q12"].Value = "B";
                worksheet.Cells["S12"].Value = "GRAVELLING";
                worksheet.Cells["S12:V12"].Merge = true;
                worksheet.Cells["S12:V12"].Style.Font.Size = 10;
                worksheet.Cells["S12:V12"].Style.Font.Name = "Arial";
                worksheet.Cells["W12"].Value = "RESHAPING";
                worksheet.Cells["W12:Z12"].Merge = true;
                worksheet.Cells["W12:Z12"].Style.Font.Size = 10;
                worksheet.Cells["W12:Z12"].Style.Font.Name = "Arial";
                worksheet.Cells["AA12"].Value = "STRUCTURES";
                worksheet.Cells["AA12:AD12"].Merge = true;
                worksheet.Cells["AA12:AD12"].Style.Font.Size = 10;
                worksheet.Cells["AA12:AD12"].Style.Font.Name = "Arial";


                //You could also use [line, column] notation:
                //worksheet.Cells[1, 2].Value = "This is cell B1!";

                int i = 13;
                double sumKm = 0.0;
                //Loop through Roadsheets
                foreach (var roadsheet in roadSheets)
                {
                    worksheet.Cells[i, 1].Value = roadsheet.SheetNo;
                    worksheet.Cells[i, 2].Value = roadsheet.EndChainage - roadsheet.StartChainage;
                    worksheet.Cells["B" + i + ":" + "C" + i].Merge = true;
                    worksheet.Cells["B" + i + ":" + "C" + i].Style.Font.Size = 10;
                    worksheet.Cells["B" + i + ":" + "C" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["B" + i + ":" + "C" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //Get ARD for Roadsheet
                    Double ARD = await GetARDForSheet(roadsheet.ID).ConfigureAwait(false);

                    if (ARD >= 1 && ARD < 2)
                    {
                        worksheet.Cells["E" + i].Value = ARD;
                    }

                    if (ARD >= 2 && ARD < 3)
                    {
                        worksheet.Cells["F" + i].Value = ARD;
                    }

                    if (ARD >= 3 && ARD < 4)
                    {
                        worksheet.Cells["G" + i].Value = ARD;
                    }

                    if (ARD >= 4 && ARD < 5)
                    {
                        worksheet.Cells["H" + i].Value = ARD;
                    }
                    if (ARD >= 5)
                    {
                        worksheet.Cells["I" + i].Value = ARD;
                    }

                    //Culverts Data
                    ARICSData aRICSData = await GetCulvertsSummaryForSheet(roadsheet.ID).ConfigureAwait(false);

                    //Culverts New Line Required
                    try
                    {
                        worksheet.Cells["L" + i].Value = aRICSData.CulvertN;
                    }
                    catch (Exception Ex)
                    { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }

                    //Culverts Repair of Ring
                    try
                    {
                        worksheet.Cells["M" + i].Value = aRICSData.CulvertRR;
                    }
                    catch (Exception Ex)
                    { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }

                    //Culverts Headwall Repair
                    try
                    {
                        worksheet.Cells["N" + i].Value = aRICSData.CulvertHR;
                    }
                    catch (Exception Ex)
                    { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }

                    //Culverts New Headwall
                    try
                    {
                        worksheet.Cells["O" + i].Value = aRICSData.CulvertNH;
                    }
                    catch (Exception Ex)
                    { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }

                    //Culverts Good
                    try
                    {
                        worksheet.Cells["P" + i].Value = aRICSData.CulvertG;
                    }
                    catch (Exception Ex)
                    { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }

                    //Culverts Blocked
                    try
                    {
                        worksheet.Cells["Q" + i].Value = aRICSData.CulvertB;
                    }
                    catch (Exception Ex)
                    { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }

                    //Culverts Good
                    try
                    {
                        worksheet.Cells["P" + i].Value = aRICSData.CulvertG;
                    }
                    catch (Exception Ex)
                    { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }

                    //Priority for Spot Improvement/Structures
                    try
                    {
                        worksheet.Cells["S" + i].Value = roadsheet.SheetNo + ") Ch:";
                        worksheet.Cells["T" + i].Value = roadsheet.SpotImprovementPriority;
                        worksheet.Cells["T" + i + ":" + "V" + i].Merge = true;
                        worksheet.Cells["T" + i + ":" + "V" + i].Style.Font.Size = 10;
                        worksheet.Cells["T" + i + ":" + "V" + i].Style.Font.Name = "Arial";
                    }
                    catch (Exception Ex) { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }

                    //Add to the summation total
                    try
                    {
                        sumKm += roadsheet.EndChainage - roadsheet.StartChainage;
                    }
                    catch (Exception Ex) { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }


                    //iterate
                    i++;
                }

                //format borders
                worksheet.Cells["A10:" + "C" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A10:" + "C" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["A10:" + "C10"].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                worksheet.Cells["A" + i + ":" + "C" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                worksheet.Cells["C10:" + "C" + i].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                worksheet.Cells["B10:" + "B" + i].Style.Border.Left.Style = ExcelBorderStyle.Thick;


                //Place total km
                try
                {
                    i++;
                    worksheet.Cells["A" + i].Value = "Σ";
                    worksheet.Cells["B" + i].Value = sumKm;
                    worksheet.Cells["B" + i + ":" + "C" + i].Merge = true;
                    worksheet.Cells["B" + i + ":" + "C" + i].Style.Font.Size = 10;
                    worksheet.Cells["B" + i + ":" + "C" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["B" + i + ":" + "C" + i].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["B" + i + ":" + "C" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["B" + i + ":" + "C" + i].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["B" + i + ":" + "C" + i].Style.Border.Left.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["B" + i + ":" + "C" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //On/off carriageway
                    worksheet.Cells["E10:" + "I" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["E10:" + "I" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["E10:" + "I" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["E10:" + "I" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["E10:" + "I10"].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["E12:" + "I12"].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["E10:" + "I" + i].Style.Border.Left.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["I10:" + "I" + i].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["E" + i + ":" + "I" + i].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["E" + i + ":" + "I" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;

                    //Culverts
                    worksheet.Cells["K10:" + "Q" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["K10:" + "Q" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["K10:" + "Q" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["K10:" + "Q" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["K10:" + "Q10"].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["K12:" + "Q12"].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["K10:" + "K" + i].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["K10:" + "K" + i].Style.Border.Left.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["Q10:" + "Q" + i].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["K" + i + ":" + "Q" + i].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["K" + i + ":" + "Q" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;

                    //Priority for Spot Improvement
                    worksheet.Cells["S10:" + "AD" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["S10:" + "AD" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["S10:" + "AD" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["S10:" + "AD" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["S10:" + "S" + i].Style.Border.Left.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["S10:" + "AD10"].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["S12:" + "AD12"].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["V10:" + "V" + i].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["Z10:" + "Z" + i].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["S" + i + ":" + "AD" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["AD10:" + "AD" + i].Style.Border.Right.Style = ExcelBorderStyle.Thick;

                }
                catch (Exception Ex) { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }


                //Road Km
                decimal RoadKM = 0M;
                try
                {
                    //Kenha
                    if (roadSection.Road.AuthorityId == 1)
                    {
                        //Get KenHa road
                        var kenHaResp = await _kenHARoadService.FindByRoadNumberAsync(roadSection.Road.RoadNumber).ConfigureAwait(false);
                        if (kenHaResp.Success)
                        {
                            bool result = decimal.TryParse(kenHaResp.KenhaRoad.Length.ToString(), out RoadKM);
                        }
                    }

                    //Kerra
                    if (roadSection.Road.AuthorityId == 2)
                    {
                        var keRRaResp = await _keRRARoadService.FindByRoadNumberAsync(roadSection.Road.RoadNumber).ConfigureAwait(false);
                        if (keRRaResp.Success)
                        {
                            bool result = decimal.TryParse(keRRaResp.KerraRoad.Length.ToString(), out RoadKM);
                        }

                    }

                    //KURA
                    if (roadSection.Road.AuthorityId == 3)
                    {
                        var kuRAResp = await _kuRARoadService.FindByRoadNumberAsync(roadSection.Road.RoadNumber).ConfigureAwait(false);
                        if (kuRAResp.Success)
                        {
                            bool result = decimal.TryParse(kuRAResp.KuraRoad.Length.ToString(), out RoadKM);
                        }

                    }

                    //KWS
                    if (roadSection.Road.AuthorityId == 4)
                    {
                        var kwSResp = await _kwSRoadService.FindByRoadNumberAsync(roadSection.Road.RoadNumber).ConfigureAwait(false);
                        if (kwSResp.Success)
                        {
                            bool result = decimal.TryParse(kwSResp.KwsRoad.Length.ToString(), out RoadKM);
                        }

                    }

                    //CountiesRoad
                    if (roadSection.Road.AuthorityId > 5)
                    {
                        var countiesRdResp = await _countiesRoadService.FindByRoadNumberAsync(roadSection.Road.RoadNumber).ConfigureAwait(false);
                        if (countiesRdResp.Success)
                        {
                            bool result = decimal.TryParse(countiesRdResp.CountiesRoad.Length.ToString(), out RoadKM);
                        }
                    }

                    i += 2;
                    i++;
                    worksheet.Cells["A" + i].Value = "Road KM";
                    worksheet.Cells["B" + i + ":" + "C" + i].Merge = true;
                    worksheet.Cells["B" + i + ":" + "C" + i].Style.Font.Size = 10;
                    worksheet.Cells["B" + i + ":" + "C" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["E" + i].Value = RoadKM;
                    worksheet.Cells["E" + i + ":" + "F" + i].Merge = true;
                    worksheet.Cells["E" + i + ":" + "F" + i].Style.Font.Size = 10;
                    worksheet.Cells["E" + i + ":" + "F" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["E" + i + ":" + "F" + i].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["E" + i + ":" + "F" + i].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["E" + i + ":" + "F" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["E" + i + ":" + "F" + i].Style.Border.Left.Style = ExcelBorderStyle.Thick;

                    worksheet.Cells["I" + i].Value = "Σ";
                    worksheet.Cells["J" + i].Value = "No";
                }
                catch (Exception Ex) { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }

                //ARD, Summation %, Summation Maintainable Sigma(1-3)
                i += 2;
                try
                {
                    worksheet.Cells["C" + i].Value = "Σ %";
                    worksheet.Cells["M" + i].Value = "Maintainable Σ(1-3)";
                    worksheet.Cells["M" + i + ":" + "O" + i].Merge = true;
                    worksheet.Cells["M" + i + ":" + "O" + i].Style.Font.Size = 10;
                    worksheet.Cells["M" + i + ":" + "O" + i].Style.Font.Name = "Arial";

                    worksheet.Cells["P" + i].Value = "";
                    worksheet.Cells["P" + i + ":" + "Q" + i].Merge = true;
                    worksheet.Cells["P" + i + ":" + "Q" + i].Style.Font.Size = 10;
                    worksheet.Cells["P" + i + ":" + "Q" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["P" + i + ":" + "Q" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["P" + i + ":" + "Q" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["P" + i + ":" + "Q" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["P" + i + ":" + "Q" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["W" + i].Value = "Average Rate of Deterioration:";
                    worksheet.Cells["W" + i + ":" + "AA" + i].Merge = true;
                    worksheet.Cells["W" + i + ":" + "AA" + i].Style.Font.Size = 10;
                    worksheet.Cells["W" + i + ":" + "AA" + i].Style.Font.Name = "Arial";

                    //Compute statitics for the arics data
                    var _aricsDataResponse = await _aRICSService.GetARICSByRoadSection(roadSection.ID).ConfigureAwait(false);
                    if (_aricsDataResponse.Success)
                    {
                        var aricsIRIresp = await _aRICSService.GetIRI((IList<ARICS>)_aricsDataResponse.ARICS).ConfigureAwait(false);
                        if (aricsIRIresp.Success)
                        {
                            worksheet.Cells["AC" + i].Value = aricsIRIresp.ARICSData.RateOfDeterioration;
                        }
                        else
                        {
                            worksheet.Cells["AC" + i].Value = "";
                        }
                    }

                    worksheet.Cells["AC" + i + ":" + "AD" + i].Merge = true;
                    worksheet.Cells["AC" + i + ":" + "AD" + i].Style.Font.Size = 10;
                    worksheet.Cells["AC" + i + ":" + "AD" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["AC" + i + ":" + "AD" + i].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["AC" + i + ":" + "AD" + i].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["AC" + i + ":" + "AD" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["AC" + i + ":" + "AD" + i].Style.Border.Left.Style = ExcelBorderStyle.Thick;

                    //GetARDForSheet(ro);
                    var roadConditionResp = await _roadConditionService.FindByRoadIdAsync(roadSection.RoadId, null).ConfigureAwait(false);

                    worksheet.Cells["X" + i].Value = roadConditionResp.RoadCondtion.ARD;
                }
                catch (Exception Ex) { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }

                i += 2;
                try
                {
                    worksheet.Cells["A" + i].Value = "COMPILED BY:";
                    worksheet.Cells["A" + i + ":" + "C" + i].Merge = true;
                    worksheet.Cells["A" + i + ":" + "C" + i].Style.Font.Size = 10;
                    worksheet.Cells["A" + i + ":" + "C" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["A" + i + ":" + "C" + i].Style.Font.Bold = true;

                    ApplicationUser applicationUser = await GetLoggedInUser().ConfigureAwait(false);
                    worksheet.Cells["E" + i].Value = applicationUser.UserName;
                    worksheet.Cells["E" + i + ":" + "P" + i].Merge = true;
                    worksheet.Cells["E" + i + ":" + "P" + i].Style.Font.Size = 10;
                    worksheet.Cells["E" + i + ":" + "P" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["E" + i + ":" + "P" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["Q" + i].Value = "SIGN:";
                    worksheet.Cells["Q" + i].Merge = true;
                    worksheet.Cells["Q" + i].Style.Font.Size = 10;
                    worksheet.Cells["Q" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["Q" + i].Style.Font.Bold = true;

                    worksheet.Cells["S" + i].Value = "";
                    worksheet.Cells["S" + i + ":" + "X" + i].Merge = true;
                    worksheet.Cells["S" + i + ":" + "X" + i].Style.Font.Size = 10;
                    worksheet.Cells["S" + i + ":" + "X" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["S" + i + ":" + "X" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["Z" + i].Value = "DATE:";
                    worksheet.Cells["Z" + i].Merge = true;
                    worksheet.Cells["Z" + i].Style.Font.Size = 10;
                    worksheet.Cells["Z" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["Z" + i].Style.Font.Bold = true;

                    worksheet.Cells["AA" + i].Value = DateTime.UtcNow.ToString("MM/dd/yyyy"); ;
                    worksheet.Cells["AA" + i + ":" + "AD" + i].Merge = true;
                    worksheet.Cells["AA" + i + ":" + "AD" + i].Style.Font.Size = 10;
                    worksheet.Cells["AA" + i + ":" + "AD" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["AA" + i + ":" + "AD" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;



                }
                catch (Exception Ex) { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }

                _cache.Set(handle, excelPackage.GetAsByteArray(),
                    new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));

                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Href = Url.Action("Download2", "ARICS", new { fileGuid = handle, FileName = "ARCS_Sum_UnPaved.xlsx" })
                });

            }

            //stream.Close();
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> ExportARICSSumPaved(long SectionID, int? Year)
        {
            try
            {
                //Get roadsheets for the section for the specified year
                var roadSheetListResponse = await _roadSheetService.ListByRoadSectionIdAsync(SectionID, Year).ConfigureAwait(false);
                IEnumerable<RoadSheet> RoadSheet = (IList<RoadSheet>)roadSheetListResponse.RoadSheets;

                //Get Roadsection
                var roadSectionResp = await _roadSectionService.FindByIdAsync(SectionID).ConfigureAwait(false);
                RoadSection roadsection = roadSectionResp.RoadSection;
                //Write to Excel
                MemoryStream stream = new MemoryStream();
                JsonResult myjson = await WriteToExcelARICSSumPaved(RoadSheet, roadsection, stream).ConfigureAwait(false);

                return Json(myjson);

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSController.ExportARICSSummaryReport Error {Environment.NewLine}");
                return Json(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<JsonResult> WriteToExcelARICSSumPaved(IEnumerable<RoadSheet> roadSheets, RoadSection roadSection, MemoryStream stream)
        {
            //Create a new ExcelPackage  
            string handle = Guid.NewGuid().ToString();
            using (ExcelPackage excelPackage = new ExcelPackage(stream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = "KRB";
                excelPackage.Workbook.Properties.Title = "ARICS_Summary_Report";
                excelPackage.Workbook.Properties.Subject = "ARICS Summary Report";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("ARICS SUM-Paved");

                //Add some text to cell A1
                worksheet.Cells["A4"].Value = "ROAD CONDITION SURVEY - SUMMARY SHEET - PAVED";
                worksheet.Cells["A4:Q4"].Merge = true;
                worksheet.Cells["A4:Q4"].Style.Font.Size = 14;
                worksheet.Cells["A4:Q4"].Style.Font.Name = "Arial";
                worksheet.Cells["A4:Q4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A4:Q4"].Style.Font.Bold = true;

                //worksheet.Cells["A4:Q4"].Style.Font.UnderLine = true;

                worksheet.Cells["A6"].Value = "RA/County";
                worksheet.Cells["A6:B6"].Merge = true;
                worksheet.Cells["A6:B6"].Style.Font.Size = 10;
                worksheet.Cells["A6:B6"].Style.Font.Name = "Arial";

                //Get county
                var countyResp = await _countyService.FindByIdAsync(roadSection.Constituency.CountyId).ConfigureAwait(false);
                worksheet.Cells["C6"].Value = countyResp.County.Name;
                worksheet.Cells["C6:O6"].Merge = true;
                worksheet.Cells["C6:O6"].Style.Font.Size = 10;
                worksheet.Cells["C6:O6"].Style.Font.Name = "Arial";
                worksheet.Cells["C6:O6"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["C6:O6"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["C6:O6"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["C6:O6"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["S6"].Value = "Constituency";
                worksheet.Cells["S6:U6"].Merge = true;
                worksheet.Cells["S6:U6"].Style.Font.Size = 10;
                worksheet.Cells["S6:U6"].Style.Font.Name = "Arial";

                worksheet.Cells["V6"].Value = roadSection.Constituency.Name;
                worksheet.Cells["V6:AD6"].Merge = true;
                worksheet.Cells["V6:AD6"].Style.Font.Size = 10;
                worksheet.Cells["V6:AD6"].Style.Font.Name = "Arial";
                worksheet.Cells["V6:AD6"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["V6:AD6"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["V6:AD6"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["V6:AD6"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["A8"].Value = "ROAD ID";
                worksheet.Cells["A8:B8"].Merge = true;
                worksheet.Cells["A8:B8"].Style.Font.Size = 10;
                worksheet.Cells["A8:B8"].Style.Font.Name = "Arial";

                worksheet.Cells["C8"].Value = roadSection.Road.RoadNumber;
                worksheet.Cells["C8:E8"].Merge = true;
                worksheet.Cells["C8:E8"].Style.Font.Size = 10;
                worksheet.Cells["C8:E8"].Style.Font.Name = "Arial";
                worksheet.Cells["C8:E8"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["C8:E8"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["C8:E8"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["C8:E8"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["G8"].Value = "ROAD (SECTION) NAME:";
                worksheet.Cells["G8:K8"].Merge = true;
                worksheet.Cells["G8:K8"].Style.Font.Size = 10;
                worksheet.Cells["G8:K8"].Style.Font.Name = "Arial";

                worksheet.Cells["L8"].Value = roadSection.SectionName;
                worksheet.Cells["L8:R8"].Merge = true;
                worksheet.Cells["L8:R8"].Style.Font.Size = 10;
                worksheet.Cells["L8:R8"].Style.Font.Name = "Arial";
                worksheet.Cells["L8:R8"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L8:R8"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L8:R8"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L8:R8"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["S8"].Value = "ROAD (section) LENGTH (km):";
                worksheet.Cells["S8:V8"].Merge = true;
                worksheet.Cells["S8:V8"].Style.Font.Size = 10;
                worksheet.Cells["S8:V8"].Style.Font.Name = "Arial";
                worksheet.Cells["W8"].Value = roadSection.Length;
                worksheet.Cells["W8:X8"].Merge = true;
                worksheet.Cells["W8:X8"].Style.Font.Size = 10;
                worksheet.Cells["W8:X8"].Style.Font.Name = "Arial";
                worksheet.Cells["W8:X8"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["W8:X8"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["W8:X8"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["W8:X8"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                //Year
                worksheet.Cells["Y8"].Value = "Year";
                worksheet.Cells["Y8:AA8"].Merge = true;
                worksheet.Cells["Y8:AA8"].Style.Font.Size = 10;
                worksheet.Cells["Y8:AA8"].Style.Font.Name = "Arial";
                worksheet.Cells["AB8"].Value = ((IList<RoadSheet>)roadSheets)[0].Year;
                worksheet.Cells["AB8:AD8"].Merge = true;
                worksheet.Cells["AB8:AD8"].Style.Font.Size = 10;
                worksheet.Cells["AB8:AD8"].Style.Font.Name = "Arial";
                worksheet.Cells["AB8:AD8"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["AB8:AD8"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["AB8:AD8"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["AB8:AD8"].Style.Border.Left.Style = ExcelBorderStyle.Thin;


                worksheet.Cells["E10"].Value = "ON/OFF-CARRIAGEWAY";
                worksheet.Cells["E10:I10"].Merge = true;
                worksheet.Cells["E10:I10"].Style.Font.Size = 10;
                worksheet.Cells["E10:I10"].Style.Font.Name = "Arial";
                worksheet.Cells["E10:I10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells["A11"].Value = "Sheet";
                worksheet.Cells["A11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B11"].Value = "Length";
                worksheet.Cells["B11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B12"].Value = "Km";
                worksheet.Cells["B11:C11"].Merge = true;
                worksheet.Cells["B11:C11"].Style.Font.Size = 10;
                worksheet.Cells["B11:C11"].Style.Font.Name = "Arial";
                worksheet.Cells["E11"].Value = "(Rate of Deterioration)";
                worksheet.Cells["E11:I11"].Merge = true;
                worksheet.Cells["E11:I11"].Style.Font.Size = 10;
                worksheet.Cells["E11:I11"].Style.Font.Name = "Arial";
                worksheet.Cells["E11:I11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["K11"].Value = "Shoulder";

                worksheet.Cells["L11"].Value = "Culverts";
                worksheet.Cells["L11:Q11"].Merge = true;
                worksheet.Cells["L11:Q11"].Style.Font.Size = 10;
                worksheet.Cells["L11:Q11"].Style.Font.Name = "Arial";
                worksheet.Cells["L11:Q11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells["S11"].Value = "PRIORITY FOR SPOT IMPROVEMENT";
                worksheet.Cells["S11:AD11"].Merge = true;
                worksheet.Cells["S11:AD11"].Style.Font.Size = 10;
                worksheet.Cells["S11:AD11"].Style.Font.Name = "Arial";
                worksheet.Cells["S11:AD11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells["A12"].Value = "No";
                worksheet.Cells["A12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B12"].Value = "Km";
                worksheet.Cells["B12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B12:C12"].Merge = true;
                worksheet.Cells["B12:C12"].Style.Font.Size = 10;
                worksheet.Cells["B12:C12"].Style.Font.Name = "Arial";
                worksheet.Cells["E12"].Value = "1";
                worksheet.Cells["F12"].Value = "2";
                worksheet.Cells["G12"].Value = "3";
                worksheet.Cells["H12"].Value = "4";
                worksheet.Cells["I12"].Value = "5";
                worksheet.Cells["K12"].Value = "Grading";
                worksheet.Cells["L12"].Value = "N";
                worksheet.Cells["M12"].Value = "RR";
                worksheet.Cells["N12"].Value = "HR";
                worksheet.Cells["O12"].Value = "NH";
                worksheet.Cells["P12"].Value = "G";
                worksheet.Cells["Q12"].Value = "B";
                worksheet.Cells["S12"].Value = "GRAVELLING";
                worksheet.Cells["S12:V12"].Merge = true;
                worksheet.Cells["S12:V12"].Style.Font.Size = 10;
                worksheet.Cells["S12:V12"].Style.Font.Name = "Arial";
                worksheet.Cells["W12"].Value = "RESHAPING";
                worksheet.Cells["W12:Z12"].Merge = true;
                worksheet.Cells["W12:Z12"].Style.Font.Size = 10;
                worksheet.Cells["W12:Z12"].Style.Font.Name = "Arial";
                worksheet.Cells["AA12"].Value = "STRUCTURES";
                worksheet.Cells["AA12:AD12"].Merge = true;
                worksheet.Cells["AA12:AD12"].Style.Font.Size = 10;
                worksheet.Cells["AA12:AD12"].Style.Font.Name = "Arial";


                //You could also use [line, column] notation:
                //worksheet.Cells[1, 2].Value = "This is cell B1!";

                int i = 13;
                double sumKm = 0.0;
                //Loop through Roadsheets
                foreach (var roadsheet in roadSheets)
                {
                    worksheet.Cells[i, 1].Value = roadsheet.SheetNo;
                    worksheet.Cells[i, 2].Value = roadsheet.EndChainage - roadsheet.StartChainage;
                    worksheet.Cells["B" + i + ":" + "C" + i].Merge = true;
                    worksheet.Cells["B" + i + ":" + "C" + i].Style.Font.Size = 10;
                    worksheet.Cells["B" + i + ":" + "C" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["B" + i + ":" + "C" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //Get ARD for Roadsheet
                    Double ARD = await GetARDForSheet(roadsheet.ID).ConfigureAwait(false);

                    if (ARD >= 1 && ARD < 2)
                    {
                        worksheet.Cells["E" + i].Value = ARD;
                    }

                    if (ARD >= 2 && ARD < 3)
                    {
                        worksheet.Cells["F" + i].Value = ARD;
                    }

                    if (ARD >= 3 && ARD < 4)
                    {
                        worksheet.Cells["G" + i].Value = ARD;
                    }

                    if (ARD >= 4 && ARD < 5)
                    {
                        worksheet.Cells["H" + i].Value = ARD;
                    }
                    if (ARD >= 5)
                    {
                        worksheet.Cells["I" + i].Value = ARD;
                    }

                    //Culverts Data
                    ARICSData aRICSData = await GetCulvertsSummaryForSheet(roadsheet.ID).ConfigureAwait(false);

                    //Culverts New Line Required
                    try
                    {
                        worksheet.Cells["L" + i].Value = aRICSData.CulvertN;
                    }
                    catch (Exception Ex)
                    { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }

                    //Culverts Repair of Ring
                    try
                    {
                        worksheet.Cells["M" + i].Value = aRICSData.CulvertRR;
                    }
                    catch (Exception Ex)
                    { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }

                    //Culverts Headwall Repair
                    try
                    {
                        worksheet.Cells["N" + i].Value = aRICSData.CulvertHR;
                    }
                    catch (Exception Ex)
                    { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }

                    //Culverts New Headwall
                    try
                    {
                        worksheet.Cells["O" + i].Value = aRICSData.CulvertNH;
                    }
                    catch (Exception Ex)
                    { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }

                    //Culverts Good
                    try
                    {
                        worksheet.Cells["P" + i].Value = aRICSData.CulvertG;
                    }
                    catch (Exception Ex)
                    { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }

                    //Culverts Blocked
                    try
                    {
                        worksheet.Cells["Q" + i].Value = aRICSData.CulvertB;
                    }
                    catch (Exception Ex)
                    { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }

                    //Culverts Good
                    try
                    {
                        worksheet.Cells["P" + i].Value = aRICSData.CulvertG;
                    }
                    catch (Exception Ex)
                    { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }

                    //Priority for Spot Improvement/Structures
                    try
                    {
                        worksheet.Cells["S" + i].Value = roadsheet.SheetNo + ") Ch:";
                        worksheet.Cells["T" + i].Value = roadsheet.SpotImprovementPriority;
                        worksheet.Cells["T" + i + ":" + "V" + i].Merge = true;
                        worksheet.Cells["T" + i + ":" + "V" + i].Style.Font.Size = 10;
                        worksheet.Cells["T" + i + ":" + "V" + i].Style.Font.Name = "Arial";
                    }
                    catch (Exception Ex) { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }

                    //Add to the summation total
                    try
                    {
                        sumKm += roadsheet.EndChainage - roadsheet.StartChainage;
                    }
                    catch (Exception Ex) { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }


                    //iterate
                    i++;
                }

                //format borders
                worksheet.Cells["A10:" + "C" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A10:" + "C" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["A10:" + "C10"].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                worksheet.Cells["A" + i + ":" + "C" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                worksheet.Cells["C10:" + "C" + i].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                worksheet.Cells["B10:" + "B" + i].Style.Border.Left.Style = ExcelBorderStyle.Thick;


                //Place total km
                try
                {
                    i++;
                    worksheet.Cells["A" + i].Value = "Σ";
                    worksheet.Cells["B" + i].Value = sumKm;
                    worksheet.Cells["B" + i + ":" + "C" + i].Merge = true;
                    worksheet.Cells["B" + i + ":" + "C" + i].Style.Font.Size = 10;
                    worksheet.Cells["B" + i + ":" + "C" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["B" + i + ":" + "C" + i].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["B" + i + ":" + "C" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["B" + i + ":" + "C" + i].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["B" + i + ":" + "C" + i].Style.Border.Left.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["B" + i + ":" + "C" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //On/off carriageway
                    worksheet.Cells["E10:" + "I" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["E10:" + "I" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["E10:" + "I" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["E10:" + "I" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["E10:" + "I10"].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["E12:" + "I12"].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["E10:" + "I" + i].Style.Border.Left.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["I10:" + "I" + i].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["E" + i + ":" + "I" + i].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["E" + i + ":" + "I" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;

                    //Culverts
                    worksheet.Cells["K10:" + "Q" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["K10:" + "Q" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["K10:" + "Q" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["K10:" + "Q" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["K10:" + "Q10"].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["K12:" + "Q12"].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["K10:" + "K" + i].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["K10:" + "K" + i].Style.Border.Left.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["Q10:" + "Q" + i].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["K" + i + ":" + "Q" + i].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["K" + i + ":" + "Q" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;

                    //Priority for Spot Improvement
                    worksheet.Cells["S10:" + "AD" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["S10:" + "AD" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["S10:" + "AD" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["S10:" + "AD" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["S10:" + "S" + i].Style.Border.Left.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["S10:" + "AD10"].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["S12:" + "AD12"].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["V10:" + "V" + i].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["Z10:" + "Z" + i].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["S" + i + ":" + "AD" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["AD10:" + "AD" + i].Style.Border.Right.Style = ExcelBorderStyle.Thick;

                }
                catch (Exception Ex) { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }


                //Road Km
                decimal RoadKM = 0M;
                try
                {
                    //Kenha
                    if (roadSection.Road.AuthorityId == 1)
                    {
                        //Get KenHa road
                        var kenHaResp = await _kenHARoadService.FindByRoadNumberAsync(roadSection.Road.RoadNumber).ConfigureAwait(false);
                        if (kenHaResp.Success)
                        {
                            bool result = decimal.TryParse(kenHaResp.KenhaRoad.Length.ToString(), out RoadKM);
                        }
                    }

                    //Kerra
                    if (roadSection.Road.AuthorityId == 2)
                    {
                        var keRRaResp = await _keRRARoadService.FindByRoadNumberAsync(roadSection.Road.RoadNumber).ConfigureAwait(false);
                        if (keRRaResp.Success)
                        {
                            bool result = decimal.TryParse(keRRaResp.KerraRoad.Length.ToString(), out RoadKM);
                        }
                    }

                    //KURA
                    if (roadSection.Road.AuthorityId == 3)
                    {
                        var kuRAResp = await _kuRARoadService.FindByRoadNumberAsync(roadSection.Road.RoadNumber).ConfigureAwait(false);
                        if (kuRAResp.Success)
                        {
                            bool result = decimal.TryParse(kuRAResp.KuraRoad.Length.ToString(), out RoadKM);
                        }

                    }

                    //KWS
                    if (roadSection.Road.AuthorityId == 4)
                    {
                        var kwSResp = await _kwSRoadService.FindByRoadNumberAsync(roadSection.Road.RoadNumber).ConfigureAwait(false);
                        bool result = decimal.TryParse(kwSResp.KwsRoad.Length.ToString(), out RoadKM);
                    }

                    //CountiesRoad
                    if (roadSection.Road.AuthorityId > 5)
                    {
                        var countiesRdResp = await _countiesRoadService.FindByRoadNumberAsync(roadSection.Road.RoadNumber).ConfigureAwait(false);
                        if (countiesRdResp.Success)
                        {
                            bool result = decimal.TryParse(countiesRdResp.CountiesRoad.Length.ToString(), out RoadKM);
                        }
                    }

                    i += 2;
                    i++;
                    worksheet.Cells["A" + i].Value = "Road KM";
                    worksheet.Cells["B" + i + ":" + "C" + i].Merge = true;
                    worksheet.Cells["B" + i + ":" + "C" + i].Style.Font.Size = 10;
                    worksheet.Cells["B" + i + ":" + "C" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["E" + i].Value = RoadKM;
                    worksheet.Cells["E" + i + ":" + "F" + i].Merge = true;
                    worksheet.Cells["E" + i + ":" + "F" + i].Style.Font.Size = 10;
                    worksheet.Cells["E" + i + ":" + "F" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["E" + i + ":" + "F" + i].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["E" + i + ":" + "F" + i].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["E" + i + ":" + "F" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["E" + i + ":" + "F" + i].Style.Border.Left.Style = ExcelBorderStyle.Thick;

                    worksheet.Cells["I" + i].Value = "Σ";
                    worksheet.Cells["J" + i].Value = "No";
                }
                catch (Exception Ex) { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }

                //ARD, Summation %, Summation Maintainable Sigma(1-3)
                i += 2;
                try
                {
                    worksheet.Cells["C" + i].Value = "Σ %";
                    worksheet.Cells["M" + i].Value = "Maintainable Σ(1-3)";
                    worksheet.Cells["M" + i + ":" + "O" + i].Merge = true;
                    worksheet.Cells["M" + i + ":" + "O" + i].Style.Font.Size = 10;
                    worksheet.Cells["M" + i + ":" + "O" + i].Style.Font.Name = "Arial";

                    worksheet.Cells["P" + i].Value = "";
                    worksheet.Cells["P" + i + ":" + "Q" + i].Merge = true;
                    worksheet.Cells["P" + i + ":" + "Q" + i].Style.Font.Size = 10;
                    worksheet.Cells["P" + i + ":" + "Q" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["P" + i + ":" + "Q" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["P" + i + ":" + "Q" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["P" + i + ":" + "Q" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["P" + i + ":" + "Q" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;


                    worksheet.Cells["W" + i].Value = "Average Rate of Deterioration:";
                    worksheet.Cells["W" + i + ":" + "AA" + i].Merge = true;
                    worksheet.Cells["W" + i + ":" + "AA" + i].Style.Font.Size = 10;
                    worksheet.Cells["W" + i + ":" + "AA" + i].Style.Font.Name = "Arial";

                    //Compute statitics for the arics data
                    var _aricsDataResponse = await _aRICSService.GetARICSByRoadSection(roadSection.ID).ConfigureAwait(false);
                    if (_aricsDataResponse.Success)
                    {
                        var aricsIRIresp = await _aRICSService.GetIRI((IList<ARICS>)_aricsDataResponse.ARICS).ConfigureAwait(false);
                        if (aricsIRIresp.Success)
                        {
                            worksheet.Cells["AC" + i].Value = aricsIRIresp.ARICSData.RateOfDeterioration;
                        }
                        else
                        {
                            worksheet.Cells["AC" + i].Value = "";
                        }
                    }
                    worksheet.Cells["AC" + i + ":" + "AD" + i].Merge = true;
                    worksheet.Cells["AC" + i + ":" + "AD" + i].Style.Font.Size = 10;
                    worksheet.Cells["AC" + i + ":" + "AD" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["AC" + i + ":" + "AD" + i].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["AC" + i + ":" + "AD" + i].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["AC" + i + ":" + "AD" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["AC" + i + ":" + "AD" + i].Style.Border.Left.Style = ExcelBorderStyle.Thick;

                    //GetARDForSheet(ro);
                    var roadConditionResp = await _roadConditionService.FindByRoadIdAsync(roadSection.RoadId, null).ConfigureAwait(false);

                    worksheet.Cells["X" + i].Value = roadConditionResp.RoadCondtion.ARD;
                }
                catch (Exception Ex) { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }

                i += 1;
                try
                {
                    worksheet.Cells["A" + i].Value = "POSSIBLE ENVIRONMENTAL";
                    worksheet.Cells["A" + i + ":" + "G" + i].Merge = true;
                    worksheet.Cells["A" + i + ":" + "G" + i].Style.Font.Size = 10;
                    worksheet.Cells["A" + i + ":" + "G" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["A" + i + ":" + "G" + i].Style.Font.Bold = true;

                    worksheet.Cells["H" + i].Value = "";
                    worksheet.Cells["H" + i + ":" + "AD" + i].Merge = true;
                    worksheet.Cells["H" + i + ":" + "AD" + i].Style.Font.Size = 10;
                    worksheet.Cells["H" + i + ":" + "AD" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["H" + i + ":" + "AD" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["H" + i + ":" + "AD" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["H" + i + ":" + "AD" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    i += 1;
                    worksheet.Cells["A" + i].Value = "PROJECTS IN THIS ROAD :";
                    worksheet.Cells["A" + i + ":" + "G" + i].Merge = true;
                    worksheet.Cells["A" + i + ":" + "G" + i].Style.Font.Size = 10;
                    worksheet.Cells["A" + i + ":" + "G" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["A" + i + ":" + "G" + i].Style.Font.Bold = true;

                    worksheet.Cells["H" + i].Value = "";
                    worksheet.Cells["H" + i + ":" + "AD" + i].Merge = true;
                    worksheet.Cells["H" + i + ":" + "AD" + i].Style.Font.Size = 10;
                    worksheet.Cells["H" + i + ":" + "AD" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["H" + i + ":" + "AD" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["H" + i + ":" + "AD" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["H" + i + ":" + "AD" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                }
                catch (Exception Ex) { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }

                i += 2;
                try
                {
                    worksheet.Cells["A" + i].Value = "COMPILED BY:";
                    worksheet.Cells["A" + i + ":" + "C" + i].Merge = true;
                    worksheet.Cells["A" + i + ":" + "C" + i].Style.Font.Size = 10;
                    worksheet.Cells["A" + i + ":" + "C" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["A" + i + ":" + "C" + i].Style.Font.Bold = true;

                    ApplicationUser applicationUser = await GetLoggedInUser().ConfigureAwait(false);
                    worksheet.Cells["E" + i].Value = applicationUser.UserName;
                    worksheet.Cells["E" + i + ":" + "P" + i].Merge = true;
                    worksheet.Cells["E" + i + ":" + "P" + i].Style.Font.Size = 10;
                    worksheet.Cells["E" + i + ":" + "P" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["E" + i + ":" + "P" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["Q" + i].Value = "SIGN:";
                    worksheet.Cells["Q" + i].Merge = true;
                    worksheet.Cells["Q" + i].Style.Font.Size = 10;
                    worksheet.Cells["Q" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["Q" + i].Style.Font.Bold = true;

                    worksheet.Cells["S" + i].Value = "XXXX";
                    worksheet.Cells["S" + i + ":" + "X" + i].Merge = true;
                    worksheet.Cells["S" + i + ":" + "X" + i].Style.Font.Size = 10;
                    worksheet.Cells["S" + i + ":" + "X" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["S" + i + ":" + "X" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["Z" + i].Value = "DATE:";
                    worksheet.Cells["Z" + i].Merge = true;
                    worksheet.Cells["Z" + i].Style.Font.Size = 10;
                    worksheet.Cells["Z" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["Z" + i].Style.Font.Bold = true;

                    worksheet.Cells["AA" + i].Value = DateTime.UtcNow.ToString("MM/dd/yyyy"); ;
                    worksheet.Cells["AA" + i + ":" + "AD" + i].Merge = true;
                    worksheet.Cells["AA" + i + ":" + "AD" + i].Style.Font.Size = 10;
                    worksheet.Cells["AA" + i + ":" + "AD" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["AA" + i + ":" + "AD" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;



                }
                catch (Exception Ex) { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }

                _cache.Set(handle, excelPackage.GetAsByteArray(),
                    new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));

                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Href = Url.Action("Download2", "ARICS", new { fileGuid = handle, FileName = "ARCS_Sum_Paved.xlsx" })
                });

            }

            //stream.Close();
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> ExportARICSMasterSum(long AuthorityID, int? Year)
        {
            try
            {
                //Get Authority
                Authority authority = null;
                if (AuthorityID == 0)
                {
                    ApplicationUser applicationUser = await GetLoggedInUser().ConfigureAwait(false);
                    authority = applicationUser.Authority;
                }
                else
                {
                    var authorityResp = await _authorityService.FindByIdAsync(AuthorityID).ConfigureAwait(false);
                    authority = authorityResp.Authority;
                }

                //Get road conditions for authority
                var resp = await _roadConditionService.ListAsync(authority, Year).ConfigureAwait(false);

                //Write to Excel
                MemoryStream stream = new MemoryStream();
                JsonResult myjson = await WriteToExcelARICSMasterSum(resp.RoadCondtion, authority, stream).ConfigureAwait(false);

                return Json(myjson);

            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSController.ExportARICSSummaryReport Error {Environment.NewLine}");
                return Json(null);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<JsonResult> WriteToExcelARICSMasterSum(IEnumerable<RoadCondition> roadConditions, Authority authority, MemoryStream stream)
        {
            //Create a new ExcelPackage  
            string handle = Guid.NewGuid().ToString();
            using (ExcelPackage excelPackage = new ExcelPackage(stream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = "KRB";
                excelPackage.Workbook.Properties.Title = "ARICS_Master_Sum_Report";
                excelPackage.Workbook.Properties.Subject = "ARICS Master Sum Report";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("ARICS Master Sum");

                //Add some text to cell A1
                worksheet.Cells["A4"].Value = "ANNUAL ROAD CONDITION and INVENTORY SURVEY -  SUMMARY";
                worksheet.Cells["A4:K4"].Merge = true;
                worksheet.Cells["A4:K4"].Style.Font.Size = 16;
                worksheet.Cells["A4:K4"].Style.Font.Name = "Arial";
                worksheet.Cells["A4:K4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A4:K4"].Style.Font.Bold = true;
                worksheet.Cells["A4:K4"].Style.Font.UnderLine = true;

                worksheet.Cells["A6"].Value = "IMPLEMENTATION AGENCY:";
                worksheet.Cells["A6:B6"].Merge = true;
                worksheet.Cells["A6:B6"].Style.Font.Size = 14;
                worksheet.Cells["A6:B6"].Style.Font.Name = "Arial";
                worksheet.Cells["A6:B6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A6:B6"].Style.Font.Bold = true;

                worksheet.Cells["C6"].Value = authority.Name;
                worksheet.Cells["C6"].Style.Font.Size = 11;
                worksheet.Cells["C6"].Style.Font.Name = "Arial";


                worksheet.Cells["G6"].Value = "ARICS CARRIED OUT:";
                worksheet.Cells["G6:I6"].Merge = true;
                worksheet.Cells["G6:I6"].Style.Font.Size = 14;
                worksheet.Cells["G6:I6"].Style.Font.Name = "Arial";
                worksheet.Cells["G6:I6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["G6:I6"].Style.Font.Bold = true;

                worksheet.Cells["J6"].Value = DateTime.UtcNow.Year;
                worksheet.Cells["J6:K6"].Merge = true;
                worksheet.Cells["J6:K6"].Style.Font.Size = 11;
                worksheet.Cells["J6:K6"].Style.Font.Name = "Arial";

                worksheet.Cells["A8"].Value = "ROAD ID";
                worksheet.Cells["A8:A12"].Merge = true;
                worksheet.Cells["A8:A12"].Style.Font.Size = 11;
                worksheet.Cells["A8:A12"].Style.Font.Name = "Arial";

                worksheet.Cells["B8"].Value = "Description:";
                worksheet.Cells["B8:B12"].Merge = true;
                worksheet.Cells["B8:B12"].Style.Font.Size = 11;
                worksheet.Cells["B8:B12"].Style.Font.Name = "Arial";

                worksheet.Cells["C8"].Value = "Road Length(Km)";
                worksheet.Cells["C8:C12"].Merge = true;
                worksheet.Cells["C8:C12"].Style.Font.Size = 11;
                worksheet.Cells["C8:C12"].Style.Font.Name = "Arial";

                worksheet.Cells["E8"].Value = "Detailed Condition Survey";
                worksheet.Cells["E8:J9"].Merge = true;
                worksheet.Cells["E8:J9"].Style.Font.Size = 14;
                worksheet.Cells["E8:J9"].Style.Font.Name = "Arial";
                worksheet.Cells["E8:J9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["E8:J9"].Style.Font.Bold = true;

                worksheet.Cells["K8"].Value = "DATE";
                worksheet.Cells["K8"].Style.Font.Size = 11;

                worksheet.Cells["E10"].Value = "GOOD";
                worksheet.Cells["E10:F10"].Merge = true;
                worksheet.Cells["E10:F10"].Style.Font.Size = 12;
                worksheet.Cells["E10:F10"].Style.Font.Name = "Arial";
                worksheet.Cells["E10:F10"].Style.Font.Bold = true;

                worksheet.Cells["G10"].Value = "FAIR";
                worksheet.Cells["G10:H10"].Merge = true;
                worksheet.Cells["G10:H10"].Style.Font.Size = 12;
                worksheet.Cells["G10:H10"].Style.Font.Name = "Arial";
                worksheet.Cells["G10:H10"].Style.Font.Bold = true;

                worksheet.Cells["I10"].Value = "POOR";
                worksheet.Cells["I10:J10"].Merge = true;
                worksheet.Cells["I10:J10"].Style.Font.Size = 12;
                worksheet.Cells["I10:J10"].Style.Font.Name = "Arial";
                worksheet.Cells["I10:J10"].Style.Font.Bold = true;

                worksheet.Cells["K10"].Value = "Average Rate of Deterioration";
                worksheet.Cells["K10:K11"].Merge = true;
                worksheet.Cells["K10:K11"].Style.Font.Size = 12;
                worksheet.Cells["K10:K11"].Style.Font.Name = "Arial";
                worksheet.Cells["K10:K11"].Style.Font.Bold = true;

                worksheet.Cells["E11"].Value = "1:V. Good/2:Good";
                worksheet.Cells["E11:F11"].Merge = true;
                worksheet.Cells["E11:F11"].Style.Font.Size = 12;
                worksheet.Cells["E11:F11"].Style.Font.Name = "Arial";
                worksheet.Cells["E11:F11"].Style.Font.Bold = true;

                worksheet.Cells["G11"].Value = "3 : Fair";
                worksheet.Cells["G11:H11"].Merge = true;
                worksheet.Cells["G11:H11"].Style.Font.Size = 12;
                worksheet.Cells["G11:H11"].Style.Font.Name = "Arial";
                worksheet.Cells["G11:H11"].Style.Font.Bold = true;

                worksheet.Cells["I11"].Value = "4:Bad/ 5:Very Bad";
                worksheet.Cells["I11:J11"].Merge = true;
                worksheet.Cells["I11:J11"].Style.Font.Size = 12;
                worksheet.Cells["I11:J11"].Style.Font.Name = "Arial";
                worksheet.Cells["I11:J11"].Style.Font.Bold = true;


                worksheet.Cells["E12"].Value = "Km";
                worksheet.Cells["F12"].Value = "%";
                worksheet.Cells["G12"].Value = "Km";
                worksheet.Cells["H12"].Value = "%";
                worksheet.Cells["G12"].Value = "Km";
                worksheet.Cells["H12"].Value = "%";
                worksheet.Cells["I12"].Value = "Km";
                worksheet.Cells["J12"].Value = "%";
                worksheet.Cells["K12"].Value = "ARD";
                worksheet.Cells["E12:K12"].Style.Font.Size = 10;
                worksheet.Cells["E12:K12"].Style.Font.Name = "Arial";

                //You could also use [line, column] notation:
                //worksheet.Cells[1, 2].Value = "This is cell B1!";

                int i = 13;
                double sumKm = 0.0;
                //Loop through Roadsheets
                foreach (var roadCondition in roadConditions)
                {
                    worksheet.Cells[i, 1].Value = roadCondition.Road.RoadNumber;
                    worksheet.Cells[i, 2].Value = roadCondition.Road.RoadName;

                    //Get Road length
                    var roadSections = roadCondition.Road.RoadSections;
                    double roadLength = roadSections.Sum(x => x.Length);
                    worksheet.Cells[i, 3].Value = String.Format("{0:0.00}", roadLength);
                    worksheet.Cells["A" + i + ":" + "C" + i].Style.Font.Size = 11;
                    worksheet.Cells["A" + i + ":" + "C" + i].Style.Font.Name = "Arial";
                    //Get ARD for Roadsheet
                    Double ARD = roadCondition.ARD;
                    worksheet.Cells["K" + i].Value = String.Format("{0:0.00}", ARD);
                    worksheet.Cells["E" + i + ":" + "K" + i].Style.Font.Size = 11;
                    worksheet.Cells["E" + i + ":" + "K" + i].Style.Font.Name = "Arial";

                    //Get analyzed ARICS for Road
                    var dict = await GetARICSRoadConditionSummaries(roadCondition.Road, null);
                    double km; double km2; double sum;
                    bool result = dict.TryGetValue(0, out sum);
                    //Very Good/2 Good
                    result = dict.TryGetValue(1, out km);
                    result = dict.TryGetValue(2, out km2);
                    worksheet.Cells["E" + i].Value = km + km2;
                    worksheet.Cells["F" + i].Value = ((km + km2) / sum).ToString("P", CultureInfo.InvariantCulture);

                    //Fair
                    result = dict.TryGetValue(3, out km);
                    worksheet.Cells["G" + i].Value = km;
                    worksheet.Cells["H" + i].Value = (km / sum).ToString("P", CultureInfo.InvariantCulture);

                    //Very Good/2 Good
                    result = dict.TryGetValue(4, out km);
                    result = dict.TryGetValue(5, out km2);
                    worksheet.Cells["i" + i].Value = km + km2;
                    worksheet.Cells["j" + i].Value = ((km + km2) / sum).ToString("P", CultureInfo.InvariantCulture);

                    //Add to the summation total
                    try
                    {
                        sumKm += roadLength;
                    }
                    catch (Exception Ex) { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }


                    //iterate
                    i++;
                }

                //add some formatting
                worksheet.Cells["A8:C" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A8:C" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A8:C" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A8:C" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["A8:C8"].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                worksheet.Cells["C8:C" + i].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                worksheet.Cells["A" + i + ":" + "C" + i].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                worksheet.Cells["A" + i + ":" + "C" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;



                //Place total km
                try
                {
                    i++;
                    worksheet.Cells["A" + i].Value = "TOTAL E Roads:";
                    worksheet.Cells["A" + i + ":" + "C" + i].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["A" + i + ":" + "C" + i].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["A" + i + ":" + "C" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["C" + i].Value = sumKm;

                    i++;
                    worksheet.Cells["A" + i].Value = "TOTAL ALL UNPAVED ROADS";
                    worksheet.Cells["A" + i + ":" + "C" + i].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["A" + i + ":" + "C" + i].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["A" + i + ":" + "C" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["C" + i].Value = sumKm;

                    i = i + 2;
                    worksheet.Cells["B" + i].Value = "TOTAL Condition";
                    worksheet.Cells["C" + i].Value = 0d;
                }
                catch (Exception Ex) { _logger.LogError(Ex, $"ARICSController.WriteToExcel Error {Environment.NewLine}"); }

                //add some formatting
                worksheet.Cells["E8:K" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["E8:K" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["E8:K" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["E8:K" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["E8:K8"].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                worksheet.Cells["E8:E" + i].Style.Border.Left.Style = ExcelBorderStyle.Thick;
                worksheet.Cells["K8:K" + i].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                worksheet.Cells["K8:K" + i].Style.Border.Left.Style = ExcelBorderStyle.Thick;

                worksheet.Cells["I10:I" + i].Style.Border.Left.Style = ExcelBorderStyle.Thick;

                worksheet.Cells["G10:G" + i].Style.Border.Left.Style = ExcelBorderStyle.Thick;
                //worksheet.Cells["D8:D" + i].Style.Border.Left.Style = ExcelBorderStyle.Thick;

                worksheet.Cells["E" + i + ":" + "K" + i].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                worksheet.Cells["E" + i + ":" + "K" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;

                _cache.Set(handle, excelPackage.GetAsByteArray(),
                    new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));

                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Href = Url.Action("Download2", "ARICS", new { fileGuid = handle, FileName = "ARCS_Master_Sum.xlsx" })
                });

            }

            //stream.Close();
        }

        private async Task<Dictionary<int, double>> GetARICSRoadConditionSummaries(Road road, int? Year)
        {
            var resp = await _aRICSService.GetARICSForRoad(road, Year).ConfigureAwait(false);
            var arics = resp.ARICS;

            double lengthForARD1inKm = arics.Where(s => s.RateOfDeterioration == 1).Count() * 200 / 1000;
            double lengthForARD2inKm = arics.Where(s => s.RateOfDeterioration == 2).Count() * 200 / 1000;
            double lengthForARD3inKm = arics.Where(s => s.RateOfDeterioration == 3).Count() * 200 / 1000;
            double lengthForARD4inKm = arics.Where(s => s.RateOfDeterioration == 4).Count() * 200 / 1000;
            double lengthForARD5inKm = arics.Where(s => s.RateOfDeterioration == 5).Count() * 200 / 1000;
            double sum = lengthForARD1inKm + lengthForARD2inKm + lengthForARD3inKm + lengthForARD4inKm + lengthForARD5inKm;
            var dictionary = new Dictionary<int, double>();
            dictionary.Add(0, sum);
            dictionary.Add(1, lengthForARD1inKm);
            dictionary.Add(2, lengthForARD2inKm);
            dictionary.Add(3, lengthForARD3inKm);
            dictionary.Add(4, lengthForARD4inKm);
            dictionary.Add(5, lengthForARD5inKm);


            return dictionary;
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> ExportARICSSummary(long RoadID, int? Year)
        {
            try
            {
                //Get Authoritty
                ApplicationUser applicationUser = await GetLoggedInUser().ConfigureAwait(false);
                Authority authority = applicationUser.Authority;

                //get road
                var resp = await _roadService.FindByIdAsync(RoadID).ConfigureAwait(false);
                var road = resp.Road;


                //Get road conditions for the road and for year of interest
                var roadConditionResp = await _roadConditionService.GetRoadConditionByYear(road, Year).ConfigureAwait(false);

                if (roadConditionResp != null)
                {
                    //Write to Excel
                    MemoryStream stream = new MemoryStream();
                    JsonResult myjson = await WriteToExcelARICSSummary(roadConditionResp.RoadCondtion, authority, stream).ConfigureAwait(false);

                    return Json(myjson);
                }
                else
                {
                    return Json(null);
                }




            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSController.ExportARICSSummaryReport Error {Environment.NewLine}");
                return Json(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<JsonResult> WriteToExcelARICSSummary(RoadCondition roadCondition, Authority authority, MemoryStream stream)
        {
            //Create a new ExcelPackage  
            string handle = Guid.NewGuid().ToString();
            using (ExcelPackage excelPackage = new ExcelPackage(stream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = "KRB";
                excelPackage.Workbook.Properties.Title = "ARICS_Summary_Report";
                excelPackage.Workbook.Properties.Subject = "ARICS Summary Report";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("ARICS Summary");

                //Add some text to cell A1
                worksheet.Cells["A5"].Value = "ARICS I (F)";
                worksheet.Cells["A5:L5"].Merge = true;
                worksheet.Cells["A5:L5"].Style.Font.Size = 10;
                worksheet.Cells["A5:L5"].Style.Font.Name = "Arial";
                worksheet.Cells["A5:L5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A5:L5"].Style.Font.Bold = true;

                worksheet.Cells["A6"].Value = "ROAD INVENTORY SUMMARY ";
                worksheet.Cells["A6:M6"].Merge = true;
                worksheet.Cells["A6:M6"].Style.Font.Size = 16;
                worksheet.Cells["A6:M6"].Style.Font.Name = "Arial";
                worksheet.Cells["A6:M6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A6:M6"].Style.Font.Bold = true;
                worksheet.Cells["A6:M6"].Style.Font.UnderLine = true;

                worksheet.Cells["A6"].Value = "ROAD INVENTORY SUMMARY ";
                worksheet.Cells["A6:M6"].Merge = true;
                worksheet.Cells["A6:M6"].Style.Font.Size = 16;
                worksheet.Cells["A6:M6"].Style.Font.Name = "Arial";
                worksheet.Cells["A6:M6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A6:M6"].Style.Font.Bold = true;
                worksheet.Cells["A6:M6"].Style.Font.UnderLine = true;

                //Get county
                worksheet.Cells["B8"].Value = "RA/County";
                worksheet.Cells["B8:C8"].Merge = true;
                worksheet.Cells["B8:C8"].Style.Font.Bold = true;
                worksheet.Cells["B8:C8"].Style.Font.Size = 11;
                worksheet.Cells["B8:C8"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B8:C8"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B8:C8"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B8:C8"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B8:C8"].Style.Fill.PatternType = ExcelFillStyle.Gray125;


                worksheet.Cells["D8"].Value = authority.Name;
                worksheet.Cells["G8"].Value = "Constituency";
                worksheet.Cells["G8:H8"].Merge = true;
                worksheet.Cells["G8:H8"].Style.Font.Bold = true;
                worksheet.Cells["G8:H8"].Style.Font.Size = 11;
                worksheet.Cells["G8:H8"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["G8:H8"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["G8:H8"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["G8:H8"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["I8"].Value = "";
                worksheet.Cells["L8"].Value = "Road ID";
                worksheet.Cells["L8"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L8"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L8"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L8"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L8"].Style.Fill.PatternType = ExcelFillStyle.Gray125;
                try
                {
                    worksheet.Cells["M8"].Value = roadCondition.Road.RoadNumber;
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"ARICSController.WriteToExcelARICSSummary Error {Environment.NewLine}");
                }
                worksheet.Cells["M8"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["M8"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["M8"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["M8"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["B10"].Value = "Start Chain.";
                worksheet.Cells["B10:C10"].Merge = true;
                worksheet.Cells["B10:C10"].Style.Font.Bold = true;
                worksheet.Cells["B10:C10"].Style.Font.Size = 11;
                worksheet.Cells["B10:C10"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B10:C10"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B10:C10"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B10:C10"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B10:C10"].Style.Fill.PatternType = ExcelFillStyle.Gray125;

                worksheet.Cells["E10"].Value = "End Chain.";
                worksheet.Cells["E10:F10"].Merge = true;
                worksheet.Cells["E10:F10"].Style.Font.Bold = true;
                worksheet.Cells["E10:F10"].Style.Font.Size = 11;
                worksheet.Cells["E10:F10"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["E10:F10"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["E10:F10"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["E10:F10"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["E10:F10"].Style.Fill.PatternType = ExcelFillStyle.Gray125;

                worksheet.Cells["G22"].Value = "Sandy soil";
                worksheet.Cells["G22:J22"].Merge = true;
                worksheet.Cells["G22:J22"].Style.Font.Size = 11;
                worksheet.Cells["G23"].Value = "Other (define)";
                worksheet.Cells["G23:J23"].Merge = true;
                worksheet.Cells["G23:J23"].Style.Font.Size = 11;

                worksheet.Cells["H10"].Value = "No. of Sections";
                worksheet.Cells["H10:I10"].Merge = true;
                worksheet.Cells["H10:I10"].Style.Font.Bold = true;
                worksheet.Cells["H10:I10"].Style.Font.Size = 11;
                worksheet.Cells["H10:I10"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["H10:I10"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["H10:I10"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["H10:I10"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["H10:I10"].Style.Fill.PatternType = ExcelFillStyle.Gray125;

                worksheet.Cells["L10"].Value = "Road Length";
                worksheet.Cells["L10:M10"].Merge = true;
                worksheet.Cells["L10:M10"].Style.Font.Bold = true;
                worksheet.Cells["L10:M10"].Style.Font.Size = 11;
                worksheet.Cells["L10:M10"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L10:M10"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L10:M10"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L10:M10"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L10:M10"].Style.Fill.PatternType = ExcelFillStyle.Gray125;

                worksheet.Cells["B11"].Value = "";
                worksheet.Cells["B11:C11"].Merge = true;
                worksheet.Cells["B11:C11"].Style.Font.Bold = true;
                worksheet.Cells["B11:C11"].Style.Font.Size = 11;
                worksheet.Cells["B11:C11"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B11:C11"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B11:C11"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B11:C11"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["E11"].Value = "";
                worksheet.Cells["E11:F11"].Merge = true;
                worksheet.Cells["E11:F11"].Style.Font.Bold = true;
                worksheet.Cells["E11:F11"].Style.Font.Size = 11;
                worksheet.Cells["E11:F11"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["E11:F11"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["E11:F11"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["E11:F11"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["E11"].Value = "";
                worksheet.Cells["E11:F11"].Merge = true;
                worksheet.Cells["E11:F11"].Style.Font.Bold = true;
                worksheet.Cells["E11:F11"].Style.Font.Size = 11;
                worksheet.Cells["E11:F11"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["E11:F11"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["E11:F11"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["E11:F11"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["E11"].Value = "";
                worksheet.Cells["E11:F11"].Merge = true;
                worksheet.Cells["E11:F11"].Style.Font.Bold = true;
                worksheet.Cells["E11:F11"].Style.Font.Size = 11;
                worksheet.Cells["E11:F11"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["E11:F11"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["E11:F11"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["E11:F11"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                var roadSections = roadCondition.Road.RoadSections;
                worksheet.Cells["H11"].Value = roadSections.Count;
                worksheet.Cells["H11:I11"].Merge = true;
                worksheet.Cells["H11:I11"].Style.Font.Bold = true;
                worksheet.Cells["H11:I11"].Style.Font.Size = 11;
                worksheet.Cells["H11:I11"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["H11:I11"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["H11:I11"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["H11:I11"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["H11:I11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                double roadLength = roadSections.Sum(x => x.Length);
                worksheet.Cells["L11"].Value = roadLength;
                worksheet.Cells["L11:M11"].Merge = true;
                worksheet.Cells["L11:M11"].Style.Font.Bold = true;
                worksheet.Cells["L11:M11"].Style.Font.Size = 11;
                worksheet.Cells["L11:M11"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L11:M11"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L11:M11"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L11:M11"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L11:M11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells["B13"].Value = "Start Location";
                worksheet.Cells["B13:C13"].Merge = true;
                worksheet.Cells["B13:C13"].Style.Font.Bold = true;
                worksheet.Cells["B13:C13"].Style.Font.Size = 11;
                worksheet.Cells["B13:C13"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B13:C13"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B13:C13"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B13:C13"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["B13:C13"].Style.Fill.PatternType = ExcelFillStyle.Gray125;

                worksheet.Cells["D13"].Value = "";
                worksheet.Cells["D13:F13"].Merge = true;
                worksheet.Cells["D13:F13"].Style.Font.Bold = true;
                worksheet.Cells["D13:F13"].Style.Font.Size = 11;
                worksheet.Cells["D13:F13"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["D13:F13"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["D13:F13"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["D13:F13"].Style.Border.Left.Style = ExcelBorderStyle.Thin;


                worksheet.Cells["H13"].Value = "End Location";
                worksheet.Cells["H13:I13"].Merge = true;
                worksheet.Cells["H13:I13"].Style.Font.Bold = true;
                worksheet.Cells["H13:I13"].Style.Font.Size = 11;
                worksheet.Cells["H13:I13"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["H13:I13"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["H13:I13"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["H13:I13"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["H13:I13"].Style.Fill.PatternType = ExcelFillStyle.Gray125;

                worksheet.Cells["J13"].Value = "";
                worksheet.Cells["J13:K13"].Merge = true;
                worksheet.Cells["J13:K13"].Style.Font.Bold = true;
                worksheet.Cells["J13:K13"].Style.Font.Size = 11;
                worksheet.Cells["J13:K13"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["J13:K13"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["J13:K13"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["J13:K13"].Style.Border.Left.Style = ExcelBorderStyle.Thin;


                worksheet.Cells["L13"].Value = "Road Width";
                worksheet.Cells["L13"].Style.Font.Bold = true;
                worksheet.Cells["L13"].Style.Font.Size = 11;
                worksheet.Cells["L13"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L13"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L13"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L13"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L13"].Style.Fill.PatternType = ExcelFillStyle.Gray125;

                worksheet.Cells["M13"].Value = "";
                worksheet.Cells["M13"].Style.Font.Bold = true;
                worksheet.Cells["M13"].Style.Font.Size = 11;
                worksheet.Cells["M13"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["M13"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["M13"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["M13"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["M13"].Value = "";
                worksheet.Cells["M13"].Style.Font.Bold = true;
                worksheet.Cells["M13"].Style.Font.Size = 11;
                worksheet.Cells["M13"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["M13"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["M13"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["M13"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["A16"].Value = "Surface Type (Select appropriately)";
                worksheet.Cells["A16:F16"].Merge = true;
                worksheet.Cells["A16:F16"].Style.Font.Bold = true;
                worksheet.Cells["A16:F16"].Style.Font.Size = 11;
                worksheet.Cells["A16:F16"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A16:F16"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A16:F16"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A16:F16"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A16:F16"].Style.Fill.PatternType = ExcelFillStyle.Gray125;

                worksheet.Cells["G16"].Value = "Principal Subsoil Type & Location (Chainage)";
                worksheet.Cells["G16:M16"].Merge = true;
                worksheet.Cells["G16:M16"].Style.Font.Bold = true;
                worksheet.Cells["G16:M16"].Style.Font.Size = 11;
                worksheet.Cells["G16:M16"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["G16:M16"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["G16:M16"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["G16:M16"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["G16:M16"].Style.Fill.PatternType = ExcelFillStyle.Gray125;


                worksheet.Cells["A17:M23"].Style.Border.Top.Style = ExcelBorderStyle.Dotted;
                worksheet.Cells["A17:M23"].Style.Border.Right.Style = ExcelBorderStyle.Dotted;
                worksheet.Cells["A17:M23"].Style.Border.Bottom.Style = ExcelBorderStyle.Dotted;
                worksheet.Cells["A17:M23"].Style.Border.Left.Style = ExcelBorderStyle.Dotted;
                worksheet.Cells["D17:D24"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["F17:F24"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["J17:J24"].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["B17"].Value = "Asphaltic Concrete";
                worksheet.Cells["B17:D17"].Merge = true;
                worksheet.Cells["B17:D17"].Style.Font.Size = 11;


                worksheet.Cells["G17"].Value = "Red Coffee/ Friable Clay";
                worksheet.Cells["G17:J17"].Merge = true;
                worksheet.Cells["G17:J17"].Style.Font.Size = 11;

                worksheet.Cells["B18"].Value = "Concrete paving";
                worksheet.Cells["B18:D18"].Merge = true;
                worksheet.Cells["B18:D18"].Style.Font.Size = 11;

                worksheet.Cells["G18"].Value = "Volcanic soil";
                worksheet.Cells["G18:J18"].Merge = true;
                worksheet.Cells["G18:J18"].Style.Font.Size = 11;

                worksheet.Cells["B19"].Value = "Surface Dressing";
                worksheet.Cells["B19:D19"].Merge = true;
                worksheet.Cells["B19:D19"].Style.Font.Size = 11;

                worksheet.Cells["G19"].Value = "Black cotton/Expansive clay";
                worksheet.Cells["G19:J19"].Merge = true;
                worksheet.Cells["G19:J19"].Style.Font.Size = 11;

                worksheet.Cells["B20"].Value = "Gravel";
                worksheet.Cells["B20:D20"].Merge = true;
                worksheet.Cells["B20:D20"].Style.Font.Size = 11;

                worksheet.Cells["G20"].Value = "Rocky";
                worksheet.Cells["G20:J20"].Merge = true;
                worksheet.Cells["G20:J20"].Style.Font.Size = 11;

                worksheet.Cells["B21"].Value = "Earth";
                worksheet.Cells["B21:D21"].Merge = true;
                worksheet.Cells["B21:D21"].Style.Font.Size = 11;

                worksheet.Cells["G21"].Value = "Gravel";
                worksheet.Cells["G21:J21"].Merge = true;
                worksheet.Cells["G21:J21"].Style.Font.Size = 11;

                worksheet.Cells["G22"].Value = "Sandy soil";
                worksheet.Cells["G22:J22"].Merge = true;
                worksheet.Cells["G22:J22"].Style.Font.Size = 11;


                worksheet.Cells["B23"].Value = "";
                worksheet.Cells["B23:D23"].Merge = true;
                worksheet.Cells["B23:D23"].Style.Font.Size = 11;


                worksheet.Cells["G23"].Value = "Other (define)";
                worksheet.Cells["G23:J23"].Merge = true;
                worksheet.Cells["G23:J23"].Style.Font.Size = 11;


                worksheet.Cells["A25"].Value = "Traffic Flow Estimate (in AADT)";
                worksheet.Cells["A25:F25"].Merge = true;
                worksheet.Cells["A25:F25"].Style.Font.Bold = true;
                worksheet.Cells["A25:F25"].Style.Font.Size = 11;
                worksheet.Cells["A25:F25"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A25:F25"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A25:F25"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A25:F25"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A25:F25"].Style.Fill.PatternType = ExcelFillStyle.Gray125;

                worksheet.Cells["G25"].Value = "Transverse gradient ( %)";
                worksheet.Cells["G25:I25"].Merge = true;
                worksheet.Cells["G25:I25"].Style.Font.Bold = true;
                worksheet.Cells["G25:I25"].Style.Font.Size = 11;
                worksheet.Cells["G25:I25"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["G25:I25"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["G25:I25"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["G25:I25"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["G25:I25"].Style.Fill.PatternType = ExcelFillStyle.Gray125;

                worksheet.Cells["J25"].Value = "Rainfall Estimate (in mm)";
                worksheet.Cells["J25:M25"].Merge = true;
                worksheet.Cells["J25:M25"].Style.Font.Bold = true;
                worksheet.Cells["J25:M25"].Style.Font.Size = 11;
                worksheet.Cells["J25:M25"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["J25:M25"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["J25:M25"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["J25:M25"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["J25:M25"].Style.Fill.PatternType = ExcelFillStyle.Gray125;

                worksheet.Cells["G26"].Value = "Road Section";
                worksheet.Cells["G26:H26"].Style.Font.Bold = true;
                worksheet.Cells["G26:H26"].Style.Font.Size = 11;
                worksheet.Cells["G26:H26"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["G26:H26"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["G26:H26"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["G26:H26"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["I26"].Value = "%";
                worksheet.Cells["I26"].Style.Font.Bold = true;
                worksheet.Cells["I26"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["I26"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["I26"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["I26"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["A26:M37"].Style.Border.Top.Style = ExcelBorderStyle.Dotted;
                worksheet.Cells["A26:M37"].Style.Border.Right.Style = ExcelBorderStyle.Dotted;
                worksheet.Cells["A26:M37"].Style.Border.Bottom.Style = ExcelBorderStyle.Dotted;
                worksheet.Cells["A26:M37"].Style.Border.Left.Style = ExcelBorderStyle.Dotted;
                worksheet.Cells["D26:D37"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["F26:F37"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["H26:H37"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["I26:I37"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L26:L37"].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["B27"].Value = "Low (0 - 500)";
                worksheet.Cells["B27:D28"].Merge = true;
                worksheet.Cells["B27:D28"].Style.Font.Size = 11;


                worksheet.Cells["G27"].Value = "Flat (0-5%)";

                worksheet.Cells["G28"].Value = "Ch:";


                worksheet.Cells["J27"].Value = "Low (0 - 500)";
                worksheet.Cells["J27:L28"].Merge = true;
                worksheet.Cells["J27:L28"].Style.Font.Size = 11;


                worksheet.Cells["B31"].Value = "Medium (500 - 1000)";
                worksheet.Cells["B31:D32"].Merge = true;
                worksheet.Cells["B31:D32"].Style.Font.Size = 11;


                worksheet.Cells["G31"].Value = "Rolling (5-25%)";
                worksheet.Cells["G32"].Value = "Ch:";

                worksheet.Cells["J31"].Value = "Medium (250-750)";
                worksheet.Cells["J31:L32"].Merge = true;
                worksheet.Cells["J31:L32"].Style.Font.Size = 11;

                worksheet.Cells["B35"].Value = "High (>1000)";
                worksheet.Cells["B35:D36"].Merge = true;
                worksheet.Cells["B35:D36"].Style.Font.Size = 11;
                worksheet.Cells["G35"].Value = "Hilly (<25%)";
                worksheet.Cells["J35"].Value = "High (>750)";
                worksheet.Cells["J35:L36"].Merge = true;
                worksheet.Cells["J35:L36"].Style.Font.Size = 11;
                worksheet.Cells["G36"].Value = "Ch:";


                worksheet.Cells["A48"].Value = "Re-gravelling / Re-sealing";
                worksheet.Cells["A48:M48"].Merge = true;
                worksheet.Cells["A48:M48"].Style.Font.Bold = true;
                worksheet.Cells["A48:M48"].Style.Font.Size = 11;
                worksheet.Cells["A48:M48"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A48:M48"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A48:M48"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A48:M48"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A48:M48"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A48:M48"].Style.Fill.PatternType = ExcelFillStyle.Gray125;

                worksheet.Cells["I49"].Value = "From (km)";
                worksheet.Cells["L49"].Value = "To (km)";

                worksheet.Cells["C50"].Value = "Date last:";
                worksheet.Cells["D50"].Value = "Re-gravelled";
                worksheet.Cells["E50:G50"].Merge = true;
                worksheet.Cells["E50:G50"].Style.Font.Size = 11;
                worksheet.Cells["E50:G50"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["E50:G50"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["E50:G50"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["E50:G50"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["H50"].Value = "Chainage:";
                worksheet.Cells["I50"].Value = "";
                worksheet.Cells["I50:J50"].Merge = true;
                worksheet.Cells["I50:J50"].Style.Font.Size = 11;
                worksheet.Cells["I50:J50"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["I50:J50"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["I50:J50"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["I50:J50"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["L50"].Value = "";
                worksheet.Cells["L50:M50"].Merge = true;
                worksheet.Cells["L50:M50"].Style.Font.Size = 11;
                worksheet.Cells["L50:M50"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L50:M50"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L50:M50"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L50:M50"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["D52"].Value = "Re-sealed";
                worksheet.Cells["E52:G52"].Merge = true;
                worksheet.Cells["E52:G52"].Style.Font.Size = 11;
                worksheet.Cells["E52:G52"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["E52:G52"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["E52:G52"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["E52:G52"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["H52"].Value = "Chainage";
                worksheet.Cells["I52"].Value = "";
                worksheet.Cells["I52:J52"].Merge = true;
                worksheet.Cells["I52:J52"].Style.Font.Size = 11;
                worksheet.Cells["I52:J52"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["I52:J52"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["I52:J52"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["I52:J52"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L52"].Value = "";
                worksheet.Cells["L52:M52"].Merge = true;
                worksheet.Cells["L52:M52"].Style.Font.Size = 11;
                worksheet.Cells["L52:M52"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L52:M52"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L52:M52"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["L52:M52"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["A54"].Value = "Condition Survey";
                worksheet.Cells["A54:M54"].Merge = true;
                worksheet.Cells["A54:M54"].Style.Font.Bold = true;
                worksheet.Cells["A54:M54"].Style.Font.Size = 11;
                worksheet.Cells["A54:M54"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A54:M54"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A54:M54"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A54:M54"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A54:M54"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A54:M54"].Style.Fill.PatternType = ExcelFillStyle.Gray125;

                worksheet.Cells["B56"].Value = "Average Rate of Deterioration:";
                worksheet.Cells["B56:D56"].Merge = true;
                worksheet.Cells["B56:D56"].Style.Font.Size = 11;

                try
                {
                    worksheet.Cells["E56"].Value = String.Format("{0:0.00}", roadCondition.ARD);
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"ARICSController.WriteToExcelARICSSummary Error {Environment.NewLine}");
                }
                worksheet.Cells["E56:F56"].Merge = true;
                worksheet.Cells["E56:F56"].Style.Font.Size = 11;
                worksheet.Cells["E56:F56"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["E56:F56"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["E56:F56"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["E56:F56"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["I56"].Value = "Date carried out:";
                worksheet.Cells["I56:J56"].Merge = true;
                worksheet.Cells["I56:J56"].Style.Font.Size = 11;

                try
                {
                    worksheet.Cells["K56"].Value = roadCondition.ComputationTime.ToString("MM/dd/yyyy");
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, $"ARICSController.WriteToExcelARICSSummary Error {Environment.NewLine}");
                }
                worksheet.Cells["K56:L56"].Merge = true;
                worksheet.Cells["K56:L56"].Style.Font.Size = 11;
                worksheet.Cells["K56:L56"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["B59"].Value = "Inventory done by:";
                worksheet.Cells["B59:D59"].Merge = true;
                worksheet.Cells["B59:D59"].Style.Font.Bold = true;
                worksheet.Cells["B59:D59"].Style.Font.Size = 11;

                ApplicationUser applicationUser = await GetLoggedInUser().ConfigureAwait(false);
                worksheet.Cells["E59"].Value = applicationUser.UserName;
                worksheet.Cells["E59:G59"].Merge = true;
                worksheet.Cells["E59:G59"].Style.Font.Bold = true;
                worksheet.Cells["E59:G59"].Style.Font.Size = 11;
                worksheet.Cells["E59:G59"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["E59:G59"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["E59:G59"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["E59:G59"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["H59"].Value = "Sign:";

                worksheet.Cells["I59"].Value = "";
                worksheet.Cells["I59:K59"].Merge = true;
                worksheet.Cells["I59:K59"].Style.Font.Bold = true;
                worksheet.Cells["I59:K59"].Style.Font.Size = 11;
                worksheet.Cells["I59:K59"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["I59:K59"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["I59:K59"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["I59:K59"].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["L59"].Value = "Date:";
                worksheet.Cells["L59"].Merge = true;
                worksheet.Cells["L59"].Style.Font.Bold = true;
                worksheet.Cells["L59"].Style.Font.Size = 11;

                worksheet.Cells["M59"].Value = DateTime.UtcNow.ToString("MM/dd/yyyy");

                worksheet.Cells["A4:M4"].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                worksheet.Cells["A58:M58"].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                worksheet.Cells["M4:M60"].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                worksheet.Cells["A60:M60"].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;

                //You could also use [line, column] notation:
                //worksheet.Cells[1, 2].Value = "This is cell B1!";

                _cache.Set(handle, excelPackage.GetAsByteArray(),
                    new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));

                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Href = Url.Action("Download2", "ARICS", new { fileGuid = handle, FileName = "ARCS_Summary.xlsx" })
                });

            }

            //stream.Close();
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> ExportARICSStructure(long RoadSectionID, int? Year)
        {
            try
            {
                //Get Authoritty
                ApplicationUser applicationUser = await GetLoggedInUser().ConfigureAwait(false);
                Authority authority = applicationUser.Authority;

                //get road section
                var resp = await _roadSectionService.FindByIdAsync(RoadSectionID).ConfigureAwait(false);
                var roadSection = resp.RoadSection;

                //Get Road
                var respRoad = await _roadService.FindByIdAsync(roadSection.RoadId).ConfigureAwait(false);
                var road = respRoad.Road;

                //Get road conditions for the road and for year of interest
                var roadConditionResp = await _roadConditionService.GetRoadConditionByYear(road, Year).ConfigureAwait(false);

                if (roadConditionResp != null)
                {
                    //Write to Excel
                    MemoryStream stream = new MemoryStream();
                    JsonResult myjson = await WriteToExcelARICSStructure(roadSection, applicationUser, authority, stream, Year).ConfigureAwait(false);

                    return Json(myjson);
                }
                else
                {
                    return Json(null);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSController.ExportARICSStructureReport Error {Environment.NewLine}");
                return Json(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<JsonResult> WriteToExcelARICSStructure(RoadSection roadSection, ApplicationUser applicationUser, Authority authority, MemoryStream stream
            , int? Year)
        {
            //Create a new ExcelPackage  
            string handle = Guid.NewGuid().ToString();
            using (ExcelPackage excelPackage = new ExcelPackage(stream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = "KRB";
                excelPackage.Workbook.Properties.Title = "ARICS-Structures";
                excelPackage.Workbook.Properties.Subject = "ARICS Structures Report";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                int _Year = DateTime.UtcNow.Year;
                bool result = int.TryParse(Year.ToString(), out _Year);
                if (result == false)
                {
                    _Year = DateTime.UtcNow.Year;
                }

                //Loop through the Roadsheet of the respective roadsection
                var roadsheetResp = await _roadSheetService.ListByRoadSectionIdAsync(roadSection.ID, _Year).ConfigureAwait(false);
                int roadsheetsCount = roadsheetResp.RoadSheets.Count();
                foreach (var roadSheet in roadsheetResp.RoadSheets)
                {
                    //Create the WorkSheet
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add($"RoadSheet {roadSheet.SheetNo}");
                    //Row 5
                    worksheet.Cells["A5:K5"].Style.Border.Top.Style = ExcelBorderStyle.Thick;

                    //Row Number 6
                    worksheet.Cells["A2"].Value = "ARICS FOR STRUCTURES";
                    worksheet.Cells["A2:L2"].Merge = true;
                    worksheet.Cells["A2:L2"].Style.Font.Size = 16;
                    worksheet.Cells["A2:L2"].Style.Font.Name = "Arial";
                    worksheet.Cells["A2:L2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A2:L2"].Style.Font.Bold = true;
                    worksheet.Cells["A2:L2"].Style.Font.UnderLine = true;

                    worksheet.Cells["A6"].Value = "Road ID";
                    worksheet.Cells["A6"].Style.Font.Size = 11;
                    worksheet.Cells["A6"].Style.Font.Name = "Arial";
                    worksheet.Cells["A6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["A6"].Style.Font.Bold = true;
                    worksheet.Cells["A6"].Style.Fill.PatternType = ExcelFillStyle.Gray125;

                    worksheet.Cells["B6"].Value = roadSection.Road.RoadNumber;//Place Road Number
                    worksheet.Cells["A6:B6"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["B6"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A6"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A6:B6"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["D6"].Value = "Road Section ID";
                    worksheet.Cells["D6"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["D6"].Style.Font.Size = 11;
                    worksheet.Cells["D6"].Style.Font.Name = "Arial";
                    worksheet.Cells["D6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["D6"].Style.Font.Bold = true;
                    worksheet.Cells["D6"].Style.Fill.PatternType = ExcelFillStyle.Gray125;


                    worksheet.Cells["E6"].Value = roadSection.SectionID;//Section ID
                    worksheet.Cells["E6"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["E6:G6"].Merge = true;
                    worksheet.Cells["E6:G6"].Style.Font.Size = 11;
                    worksheet.Cells["E6:G6"].Style.Font.Name = "Arial";
                    worksheet.Cells["E6:G6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["E6:G6"].Style.Font.Bold = true;
                    worksheet.Cells["E6:G6"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["E6:G6"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["E6:G6"].Style.Border.Right.Style = ExcelBorderStyle.Thin;



                    worksheet.Cells["H6"].Value = "Sheet No.";
                    worksheet.Cells["H6"].Style.Fill.PatternType = ExcelFillStyle.Gray125;
                    worksheet.Cells["I6"].Value = roadSheet.SheetNo;
                    worksheet.Cells["I6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["J6"].Value = "of";
                    worksheet.Cells["J6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["K6"].Value = roadsheetsCount;
                    worksheet.Cells["K6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["H6:K6"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["H6:K6"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["H6"].Style.Border.Right.Style = ExcelBorderStyle.Thick;

                    //Row 8
                    worksheet.Cells["B8"].Value = "Structures Summary - including culverts";
                    worksheet.Cells["B8:F8"].Merge = true;
                    worksheet.Cells["B8:F8"].Style.Font.Size = 11;
                    worksheet.Cells["B8:F8"].Style.Font.Name = "Arial";
                    worksheet.Cells["B8:F8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["B8:F8"].Style.Font.Bold = true;
                    worksheet.Cells["B8:F8"].Style.Fill.PatternType = ExcelFillStyle.Gray125;

                    worksheet.Cells["G8"].Value = "Major socio-economic features along the road";
                    worksheet.Cells["G8:K8"].Merge = true;
                    worksheet.Cells["G8:K8"].Style.Font.Size = 11;
                    worksheet.Cells["G8:K8"].Style.Font.Name = "Arial";
                    worksheet.Cells["G8:K8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["G8:K8"].Style.Font.Bold = true;
                    worksheet.Cells["G8:K8"].Style.Fill.PatternType = ExcelFillStyle.Gray125;
                    worksheet.Cells["A8:K8"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A8:K8"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    //Row 9
                    worksheet.Cells["B9"].Value = "Chainage";
                    worksheet.Cells["C9"].Value = "Type of Structure / Key data";
                    worksheet.Cells["C9:F9"].Merge = true;
                    worksheet.Cells["C9:F9"].Style.Font.Size = 10;
                    worksheet.Cells["C9:F9"].Style.Font.Name = "Arial";
                    worksheet.Cells["C9:F9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells["C9:F9"].Style.Font.Bold = true;
                    worksheet.Cells["G9"].Value = "position";
                    worksheet.Cells["H9"].Value = "Description : Schools, Clinics, Villages etc ";
                    worksheet.Cells["C9:F9"].Merge = true;
                    worksheet.Cells["C9:F9"].Style.Font.Size = 10;
                    worksheet.Cells["C9:F9"].Style.Font.Name = "Arial";
                    worksheet.Cells["C9:F9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["C9:F9"].Style.Font.Bold = true;
                    worksheet.Cells["A9:K9"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    //Get All ARICS for the RoadSheet
                    ARICSVM aRICSVM = new ARICSVM();
                    aRICSVM.RoadSheetID = roadSheet.ID;
                    var aricsResp = await _aRICSService.GetARICSForSheet(aRICSVM).ConfigureAwait(false);

                    int i = 10;
                    int no = 1;
                    worksheet.Cells["A10:K10"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    foreach (var aric in aricsResp.ARICS.OrderBy(o => o.Chainage))
                    {

                        worksheet.Cells["A" + i].Value = no;
                        worksheet.Cells["B" + i].Value = aric.Chainage;
                        worksheet.Cells["C" + i].Value = $"N={aric.CulvertN}, RR={aric.CulvertRR}, HR={aric.CulvertHR}" +
                            $", NH={aric.CulvertNH}, G={aric.CulvertG}, B={aric.CulvertB}";
                        worksheet.Cells["C" + i + ":F" + i].Merge = true;
                        worksheet.Cells["C" + i + ":F" + i].Style.Font.Size = 10;
                        worksheet.Cells["C" + i + ":F" + i].Style.Font.Name = "Arial";
                        worksheet.Cells["C" + i + ":F" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        worksheet.Cells["G" + i].Value = $"Ch";
                        worksheet.Cells["H" + i].Value = aric.OtherStructureRemarks;
                        worksheet.Cells["H" + i + ":K" + i].Merge = true;
                        worksheet.Cells["H" + i + ":K" + i].Style.Font.Size = 10;
                        worksheet.Cells["H" + i + ":K" + i].Style.Font.Name = "Arial";
                        worksheet.Cells["H" + i + ":K" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["A" + i + ":K" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        i++;
                        no++;
                    }
                    worksheet.Cells["B10:B" + (i - 1).ToString()].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["C9:C" + (i - 1).ToString()].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["G8:G" + (i - 1).ToString()].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    i++;
                    worksheet.Cells["A" + i].Value = "Road Furniture Summary";
                    worksheet.Cells["A" + i + ":F" + i].Merge = true;
                    worksheet.Cells["A" + i + ":F" + i].Style.Font.Size = 11;
                    worksheet.Cells["A" + i + ":F" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["A" + i + ":F" + i].Style.Font.Bold = true;
                    worksheet.Cells["A" + i + ":F" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + i + ":F" + i].Style.Fill.PatternType = ExcelFillStyle.Gray125;
                    worksheet.Cells["A" + i + ":F" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A" + i + ":F" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    i++;
                    worksheet.Cells["B" + i].Value = "Chainage";
                    //i++;
                    worksheet.Cells["C" + i].Value = "Details";
                    worksheet.Cells["C" + i + ":F" + i].Merge = true;
                    worksheet.Cells["C" + i + ":F" + i].Style.Font.Size = 11;
                    worksheet.Cells["C" + i + ":F" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["C" + i + ":F" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + i + ":K" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A" + i + ":K" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    i++;
                    no = 1;
                    worksheet.Cells["A" + i + ":F" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    for (int k = 1; k < 12; k++)
                    {

                        worksheet.Cells["A" + i].Value = no;

                        worksheet.Cells["C" + i].Value = "";
                        worksheet.Cells["C" + i + ":F" + i].Merge = true;
                        worksheet.Cells["C" + i + ":F" + i].Style.Font.Size = 10;
                        worksheet.Cells["C" + i + ":F" + i].Style.Font.Name = "Arial";
                        worksheet.Cells["C" + i + ":K" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells["A" + i + ":K" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        i++;
                        no++;
                    }
                    worksheet.Cells["B" + (i - 14).ToString() + ":B" + (i - 0).ToString()].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["C" + (i - 13).ToString() + ":C" + (i - 0).ToString()].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["G" + (i - 13).ToString() + ":G" + (i - 0).ToString()].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells["H9:H" + i].Style.Border.Left.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["H" + i + ":K" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;

                    //Priority for Structures
                    i++;
                    worksheet.Cells["A" + i].Value = "Priority for Structures";
                    worksheet.Cells["A" + i + ":F" + i].Merge = true;
                    worksheet.Cells["A" + i + ":F" + i].Style.Font.Size = 11;
                    worksheet.Cells["A" + i + ":F" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["A" + i + ":F" + i].Style.Font.Bold = true;
                    worksheet.Cells["A" + i + ":F" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A" + i + ":F" + i].Style.Fill.PatternType = ExcelFillStyle.Gray125;
                    worksheet.Cells["A" + i + ":F" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["G" + i].Value = "Others";
                    worksheet.Cells["G" + i + ":K" + i].Merge = true;
                    worksheet.Cells["G" + i + ":K" + i].Style.Font.Size = 11;
                    worksheet.Cells["G" + i + ":K" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["G" + i + ":K" + i].Style.Font.Bold = true;
                    worksheet.Cells["G" + i + ":K" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["G" + i + ":K" + i].Style.Fill.PatternType = ExcelFillStyle.Gray125;
                    worksheet.Cells["A" + i + ":K" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A" + i + ":K" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    i++;
                    worksheet.Cells["A" + i].Value = "Chainage";
                    worksheet.Cells["A" + i + ":B" + i].Merge = true;
                    worksheet.Cells["A" + i + ":B" + i].Style.Font.Size = 11;
                    worksheet.Cells["A" + i + ":B" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["A" + i + ":B" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["C" + i].Value = "Description";
                    worksheet.Cells["C" + i + ":F" + i].Merge = true;
                    worksheet.Cells["C" + i + ":F" + i].Style.Font.Size = 11;
                    worksheet.Cells["C" + i + ":F" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["C" + i + ":F" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A" + i + ":F" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A" + i + ":F" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A" + i + ":F" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                    string pririorityForStruct = roadSheet.StructurePriority;
                    if (pririorityForStruct != null)
                    {
                        string[] arr = pririorityForStruct.Split(",");

                        i++;
                        no = 1;
                        worksheet.Cells["A" + i + ":F" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        foreach (string s in arr)
                        {

                            worksheet.Cells["A" + i].Value = no;
                            worksheet.Cells["B" + i].Value = s;
                            worksheet.Cells["A" + i + ":F" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells["F" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            i++;
                            no++;
                        }
                    }
                    else
                    {
                        i++;
                        no = 1;
                        worksheet.Cells["A" + i + ":F" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        for (int k = 1; k < 6; k++)
                        {

                            worksheet.Cells["A" + i].Value = no;
                            worksheet.Cells["A" + i + ":F" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            i++;
                            no++;
                        }
                    }

                    //Add Salutation
                    worksheet.Cells["A" + i + ":K" + i].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                    i++;
                    worksheet.Cells["A" + i].Value = "Inventory done by:";
                    worksheet.Cells["A" + i + ":C" + i].Merge = true;
                    worksheet.Cells["A" + i + ":C" + i].Style.Font.Size = 11;
                    worksheet.Cells["A" + i + ":C" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["A" + i + ":C" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["D" + i].Value = applicationUser.UserName;
                    worksheet.Cells["D" + i + ":F" + i].Merge = true;
                    worksheet.Cells["D" + i + ":F" + i].Style.Font.Size = 11;
                    worksheet.Cells["D" + i + ":F" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["D" + i + ":F" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    worksheet.Cells["G" + i].Value = "Sign:";
                    worksheet.Cells["G" + i].Style.Font.Size = 11;
                    worksheet.Cells["G" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["G" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    worksheet.Cells["H" + i].Value = "XXXX";
                    worksheet.Cells["H" + i + ":I" + i].Merge = true;
                    worksheet.Cells["H" + i + ":I" + i].Style.Font.Size = 11;
                    worksheet.Cells["H" + i + ":I" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["H" + i + ":I" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    worksheet.Cells["J" + i].Value = "Date:";
                    worksheet.Cells["J" + i].Style.Font.Size = 11;
                    worksheet.Cells["J" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["J" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    worksheet.Cells["K" + i].Value = DateTime.UtcNow.ToString("MM/dd/yyyy"); ;
                    worksheet.Cells["K" + i].Style.Font.Size = 11;
                    worksheet.Cells["K" + i].Style.Font.Name = "Arial";
                    worksheet.Cells["K" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    i++;
                    worksheet.Cells["A" + i + ":K" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    worksheet.Cells["K5:K" + i].Style.Border.Right.Style = ExcelBorderStyle.Thick;

                }

                //You could also use [line, column] notation:
                //Worksheet.Cells[1, 2].Value = "This is cell B1!";

                _cache.Set(handle, excelPackage.GetAsByteArray(),
                    new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));

                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Href = Url.Action("Download2", "ARICS", new { fileGuid = handle, FileName = "ARCS_Structure.xlsx" })
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

        private async Task<Double> GetARDForSheet(long RoadSheetID)
        {

            ARICSVM aRICSVM = new ARICSVM();
            aRICSVM.RoadSheetID = RoadSheetID;

            //Get aRICS for Sheet
            var aricsResponse = await _aRICSService.GetARICSForSheet(aRICSVM).ConfigureAwait(false);
            IEnumerable<ARICS> aRICs = aricsResponse.ARICS;

            //Get IRI for the ARICS
            var aricsResponse2 = await _aRICSService.GetIRI((IList<ARICS>)aRICs).ConfigureAwait(false);
            return aricsResponse2.ARICSData.RateOfDeterioration;
        }
        private async Task<ARICSData> GetCulvertsSummaryForSheet(long RoadSheetID)
        {

            ARICSVM aRICSVM = new ARICSVM();
            aRICSVM.RoadSheetID = RoadSheetID;

            //Get aRICS for Sheet
            var aricsResponse = await _aRICSService.GetARICSForSheet(aRICSVM).ConfigureAwait(false);
            IEnumerable<ARICS> aRICs = aricsResponse.ARICS;

            //Get Culverts Summary for the ARICS

            var aricsResponse2 = await _aRICSService.GetCulvertsSummaryForSheet((IList<ARICS>)aRICs).ConfigureAwait(false);
            return aricsResponse2.ARICSData;
        }
        #endregion

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
                        var authResp = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                        user.Authority = authResp.Authority;
                    }
                }
            }
            return user;
        }
        #endregion

        #region Direct Data Entry

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<RoadSheet> InternalCheckRoadSheetForRoadSection(RoadSheetVM _RoadSheetVM)
        {
            try
            {
                //Roadsheet Response
                var roadSheetResponse = await _roadSheetService.CheckRoadSheetsForYear(_RoadSheetVM).ConfigureAwait(false);

                //check if roadsheets exists for the particular roadsection_id
                RoadSheet _RoadSheet = roadSheetResponse.RoadSheet;

                return _RoadSheet;

            }
            catch (Exception Ex)
            {
                _logger.LogError($"ARICSController.InternalCheckRoadSheetForRoadSection Error: {Ex.Message}");
                return null;
            }
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> CheckRoadSheetForRoadSection(string RoadSectionID, string Year)
        {
            try
            {
                RoadSheetVM _RoadSheetVM = new RoadSheetVM();

                //Road Section ID
                int _RoadSectionID = 0;
                bool result = int.TryParse(RoadSectionID, out _RoadSectionID);
                _RoadSheetVM.RoadSectionID = _RoadSectionID;

                //Set the Year
                int _Year = 0;
                result = int.TryParse(Year, out _Year);
                _RoadSheetVM.Year = _Year;

                //check if roadsheets exists for the particular roadsection_id
                RoadSheet _RoadSheet = await
                InternalCheckRoadSheetForRoadSection(_RoadSheetVM).ConfigureAwait(false);
                return Json(_RoadSheet);

            }
            catch (Exception Ex)
            {
                _logger.LogError($"ARICSController.CheckRoadSheetForRoadSection Error: {Ex.Message}");
                return Json(null);
            }
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> CreateRoadSheets(string roadLengthKM, string sectionLengthKM, string RoadSectionID, string Year)
        {
            try
            {
                RoadSheetVM _RoadSheetVM = new RoadSheetVM();
                //Section Length(km)
                int sectionlengthkm = 0;
                bool result = int.TryParse(sectionLengthKM, out sectionlengthkm);
                _RoadSheetVM.SectionLengthKM = sectionlengthkm;

                //Road Section ID
                int _RoadSectionID = 0;
                result = int.TryParse(RoadSectionID, out _RoadSectionID);
                _RoadSheetVM.RoadSectionID = _RoadSectionID;

                //Set the road length(km)
                double roadlengthkm = 0.0;
                result = double.TryParse(roadLengthKM, out roadlengthkm);
                _RoadSheetVM.RoadLengthKM = roadlengthkm;

                //Set the Year
                int _Year = 0;
                result = int.TryParse(Year, out _Year);
                _RoadSheetVM.Year = _Year;

                //check if roadsheets exists for the particular roadsection_id
                RoadSheet _RoadSheet = await
                InternalCheckRoadSheetForRoadSection(_RoadSheetVM).ConfigureAwait(false);

                if (_RoadSheet == null)
                {
                    var RoadSheetListResponse = await _roadSheetService.CreateRoadSheets(roadlengthkm, sectionlengthkm, _RoadSectionID, _Year).ConfigureAwait(false);
                    IList<RoadSheet> _RoadSheetList = (IList<RoadSheet>)RoadSheetListResponse.RoadSheets;
                    return Json(_RoadSheetList);
                }
                else //Get roadsheets
                {
                    var RoadSheetListResponse = await _roadSheetService.DisplayRoadsheetsAsync(_RoadSectionID, _Year).ConfigureAwait(false);
                    IList<RoadSheet> _RoadSheetList = (IList<RoadSheet>)RoadSheetListResponse.RoadSheets;
                    return Json(_RoadSheetList);
                }

            }
            catch (Exception Ex)
            {
                _logger.LogError($"ARICSController.GetRoadSectionLength Error: {Ex.Message}");
                return Json(null);
            }
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> GetAdminBnd(long RoadSectionID)
        {
            try
            {
                long? ConstituencyID = null;
                var roadSectionResponce = await _roadSectionService.FindByIdAsync(RoadSectionID).ConfigureAwait(false);
                ConstituencyID = roadSectionResponce.RoadSection.ConstituencyId;

                //Get specific constituency details plus county details
                Constituency _Constituency = null;

                if (ConstituencyID != null)
                {
                    long _ConstituencyID = 0;
                    bool result = long.TryParse(ConstituencyID.ToString(), out _ConstituencyID);

                    var constituencyResponse = await _constituencyService.GetConstituencyAndCounty(_ConstituencyID).ConfigureAwait(false);
                    _Constituency = constituencyResponse.Constituency;
                }
                //Set geometery to Null: parse error
                _Constituency.Geom = null;
                return Json(_Constituency);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"ARICSController.GetAdminBnd Error: {Ex.Message}");
                return Json(null);
            }
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> DisplayRoadSheets(string RoadSectionID, string Year)
        {
            try
            {
                RoadSheetVM _RoadSheetVM = new RoadSheetVM();

                int _Year = 0;
                bool result = int.TryParse(Year, out _Year);
                _RoadSheetVM.Year = _Year;

                int _RoadSectionID = 0;
                result = int.TryParse(RoadSectionID, out _RoadSectionID);
                _RoadSheetVM.RoadSectionID = _RoadSectionID;

                var RoadSheetListResponse = await _roadSheetService.DisplayRoadsheetsAsync(_RoadSectionID, _Year).ConfigureAwait(false);
                IList<RoadSheet> _RoadSheetList = (IList<RoadSheet>)RoadSheetListResponse.RoadSheets;
                return Json(_RoadSheetList);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"ARICSController.DisplayRoadSheets Error: {Ex.Message}");
                return Json(null);
            }
        }

        #endregion

        #region ARICS Uploads

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> DeleteARICSAttachment(long Id, string filename)
        {
            try
            {
                //delete the file
                Boolean FileDelete = DeleteFile(filename, "ARICS");

                var aRICSUploadResponse = await _aRICSUploadService.RemoveAsync(Id).ConfigureAwait(false);
                ARICSUpload _ARICSUpload = aRICSUploadResponse.ARICSUpload;
                return Json(_ARICSUpload);

            }
            catch (Exception Ex)
            {
                _logger.LogError("GetSBP API Error", Ex);
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
                string innerException = null;
                if (IOExp.InnerException != null)
                {
                    innerException = IOExp.InnerException.ToString();
                }
                string msg = $"Error Message: {IOExp.Message.ToString()} \r\n" +
                    $"Inner Exception: {innerException} \r\n" +
                    $"Stack Trace:  {IOExp.StackTrace ?? IOExp.StackTrace.ToString()}";
                _logger.LogError($"ARICSController.DeleteFile: \r\n {msg}");
                return false;
            }

        }

        [HttpPost]
        public async Task<JsonResult> GetARICSUploads()
        {
            try
            {
                var aRICSUploadListResponse = await _aRICSUploadService.ListAsync().ConfigureAwait(false);
                IList<ARICSUpload> ARICSUpload = (IList<ARICSUpload>)aRICSUploadListResponse.ARICSUpload;
                return Json(ARICSUpload);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"ARICSController.GetARICSUploads Error: {Ex.Message}");
                return Json(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Download(string filename, string folder)
        {
            try
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
            catch (Exception Ex)
            {
                _logger.LogError($"ARICSController.Download() Error: {Ex.Message}");
                return new EmptyResult();
            }
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
        public async Task<JsonResult> GetARICSForRoad(string roadID, string startChainage, string endChainage)
        {
            try
            {
                //Get road
                long _RoadID;
                bool result = long.TryParse(roadID, out _RoadID);
                var roadResponse = await _roadService.FindByIdAsync(_RoadID).ConfigureAwait(false);
                Road Road = roadResponse.Road;
                if (Road == null)
                {
                    return Json(null);
                }
                //Convert start chainage to double
                double _StartChainage;
                result = double.TryParse(startChainage, out _StartChainage);
                //Convert end chainage to double
                double _EndChainage;
                result = double.TryParse(endChainage, out _EndChainage);

                double chainageDifference = _EndChainage - _StartChainage;

                if (_StartChainage == 0 || _EndChainage == 0 || chainageDifference <= 0)
                {
                    var aricsResponse = await _aRICSService.GetARICSForRoad(Road, null).ConfigureAwait(false);
                    IList<ARICS> aRICS = (IList<ARICS>)aricsResponse.ARICS;
                    return Json(aRICS);
                }
                else
                {
                    var aricsResponse = await _aRICSService.GetARICSForRoad(Road, _StartChainage, _EndChainage, null).ConfigureAwait(false);
                    IList<ARICS> aRICS = (IList<ARICS>)aricsResponse.ARICS;
                    return Json(aRICS);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError($"ARICSController.GetARICSUploads Error: {Ex.Message}");
                return Json(null);
            }
        }


        #endregion

        #region Roads Register
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> RoadNumberAjaxList(string RoadNumber)
        {
            try
            {
                var roadServiceResponse = await _roadService.RoadNumberAjaxListAsync(RoadNumber).ConfigureAwait(true);
                IList<Road> _Road = (IList<Road>)roadServiceResponse.Roads;
                var _LRNoList = _Road
                    .Select(p => p.RoadNumber).ToList();

                return Json(_LRNoList);
            }
            catch (Exception Ex)
            {
                _logger.LogError("LRNoAuto API Error", Ex);
                return Json(null);
            }
        }
        #endregion

        #region ARICS Approval
        public async Task<IActionResult> ARICSApprovalAgency()
        {
            ARICSApprovalAgencyViewModel aRICSApprovalAgencyViewModel
                = new ARICSApprovalAgencyViewModel();
            try
            {
                //Get logged in user
                ApplicationUser user = await GetLoggedInUser().ConfigureAwait(false);
                aRICSApprovalAgencyViewModel.Authority = user.Authority;

                if (aRICSApprovalAgencyViewModel.Authority.Code.ToLower() == "krb")
                {
                    //redirect to KRB page
                    return RedirectToAction("ARICSApprovalKRB");
                }

                await PopulateDropDown(aRICSApprovalAgencyViewModel).ConfigureAwait(false);

                //Set Return URL and store in session
                HttpContext.Session.SetString("Referer", Request.GetEncodedUrl());

                return View(aRICSApprovalAgencyViewModel);
            }
            catch (Exception Ex)
            {

                _logger.LogError($"ARICS.ARICSApprovalAgency Error : {Ex.Message}");
                ModelState.AddModelError(string.Empty, Ex.Message);
                return View(aRICSApprovalAgencyViewModel);
            }
        }

        public async Task<IActionResult> ARICSApprovalKRB()
        {
            ARICSApprovalAgencyViewModel aRICSApprovalAgencyViewModel
                = new ARICSApprovalAgencyViewModel();
            try
            {
                //Get logged in user
                ApplicationUser user = await GetLoggedInUser().ConfigureAwait(false);
                aRICSApprovalAgencyViewModel.Authority = user.Authority;

                if (aRICSApprovalAgencyViewModel.Authority.Code.ToLower() != "krb")
                {
                    //redirect to KRB page
                    return RedirectToAction("ARICSApprovalAgency");
                }

                await PopulateDropDown(aRICSApprovalAgencyViewModel).ConfigureAwait(false);

                //Set Return URL and store in session
                HttpContext.Session.SetString("Referer", Request.GetEncodedUrl());

                return View(aRICSApprovalAgencyViewModel);
            }
            catch (Exception Ex)
            {

                _logger.LogError($"ARICS.ARICSApprovalKRB Error : {Ex.Message}");
                ModelState.AddModelError(string.Empty, Ex.Message);
                return View(aRICSApprovalAgencyViewModel);
            }
        }
        private async Task PopulateDropDown(ARICSApprovalAgencyViewModel aRICSApprovalAgencyViewModel)
        {
            var authority = await _authorityService.ListAsync().ConfigureAwait(false);

            var CGandRAList = ((IList<Authority>)authority)
                .Where(x => x.Type == 1 || x.Type == 2)
                .OrderBy(o => o.ID)
                .ToList();
            ViewData["AuthorityId"] = new SelectList(CGandRAList, "ID", "Name");

            if (CGandRAList != null)
            {
                if (CGandRAList.Any())
                {
                    ViewData["AuthorityId"] = new SelectList(CGandRAList, "ID", "Name", 0);
                }
            }

            IList<ARICSYear> aRICSYears = null;
            //ARICS Year
            var aricsYearResp = await _aRICSYearService.ListAsync().ConfigureAwait(false);
            if (aricsYearResp.Success)
            {
                var objectResult = (ObjectResult)aricsYearResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result2 = (OkObjectResult)objectResult;
                        aRICSYears = (IList<ARICSYear>)result2.Value;
                    }
                }
            }

            //set current year as selected
            aRICSApprovalAgencyViewModel.ARICSYear = aRICSYears
                .Where(x => x.Year == DateTime.Now.Year)
                .FirstOrDefault();

            //Set drop down to have current year
            ViewData["ARICSYearId"] = new SelectList(aRICSYears, "ID", "Year"
                , aRICSApprovalAgencyViewModel.ARICSYear.Year);



        }

        [HttpPost]
        public async Task<IActionResult> UpdateARICSApproval(long ARICSMasterApprovalID, int Year, int Status, string Comment)
        {
            string msg = "";
            bool status = false;
            //Get logged in user
            var user = await GetLoggedInUser().ConfigureAwait(false);

            var userClaims = HttpContext.User.Claims;

            var hasAnyOfTheClaim = userClaims.Where(x => x.Value == Claims.Permission.ARICS.PreparerOrSubmitter
            || x.Value == Claims.Permission.ARICS.FirstReviewer
            || x.Value == Claims.Permission.ARICS.SecondReviewer
            || x.Value == Claims.Permission.ARICS.InternalApprover
            || x.Value == Claims.Permission.ARICS.Approver)
                .ToList();

            if (hasAnyOfTheClaim.Any() == false)
            {
                msg = $"You do not have permissions to update ARICS Approval status";
                status = false;
                return Json(new
                {
                    Success = status,
                    Message = msg,
                    Href = Url.Action("ARICSBatchDetails", "ARICS", new { ARICSMasterApprovalId = ARICSMasterApprovalID })
                });
            }


            //Get authority type
            int authorityType = Convert.ToInt32(user.Authority.Type);

            //Find by status
            //int Status = 1;//Submit
            var resp = await _aRICSApprovalLevelService.FindByStatusAsync(Status).ConfigureAwait(false);
            ARICSApprovalLevel aRICSApprovalLevel = null;
            if (resp.Success)
            {
                var objectResult = (ObjectResult)resp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        aRICSApprovalLevel = (ARICSApprovalLevel)result.Value;
                    }
                }
            }

            //Check aRICSApprovalLevel is not null
            if (aRICSApprovalLevel == null)
            {
                msg = $"ARICS Approval Levels are null";
                status = false;
                return Json(new
                {
                    Success = status,
                    Message = msg,
                    Href = Url.Action("ARICSBatchDetails", "ARICS", new { ARICSMasterApprovalId = ARICSMasterApprovalID })
                });
            }



            ARICSApproval aRICSApprovalExisting = null;
            resp = await _aRICSApprovalService.FindByARICSMasterApprovalIdAsync(ARICSMasterApprovalID).ConfigureAwait(false);
            if (resp.Success)
            {
                var objectResult = (ObjectResult)resp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        aRICSApprovalExisting = (ARICSApproval)result.Value;
                    }
                }
            }

            if (aRICSApprovalExisting != null)
            {
                //Edit existing ARICS apparoval
                aRICSApprovalExisting.ARICSApprovalLevelId = aRICSApprovalLevel.ID;
                aRICSApprovalExisting.Comment = Comment;
                aRICSApprovalExisting.ARICSMasterApprovalId = ARICSMasterApprovalID;
                aRICSApprovalExisting.Year = Year;

                aRICSApprovalExisting.UserName = user.UserName;
                aRICSApprovalExisting.UpdatedBy = user.UserName;
                aRICSApprovalExisting.UpdatedDate = DateTime.UtcNow;

                resp = await _aRICSApprovalService.Update(aRICSApprovalExisting.ID, aRICSApprovalExisting).ConfigureAwait(false);
                if (resp.Success)
                {
                    //Mark the ARICS Approval as submitted
                    msg = "Update Successful";
                    status = true;
                }
                else
                {
                    msg = "Fail";
                }
            }
            else
            {
                //instantiate ARICS apparoval
                ARICSApproval aRICSApproval = new ARICSApproval();
                aRICSApproval.ARICSApprovalLevelId = aRICSApprovalLevel.ID;
                aRICSApproval.Comment = Comment;
                aRICSApproval.ARICSMasterApprovalId = ARICSMasterApprovalID;
                aRICSApproval.Year = Year;

                aRICSApproval.UserName = user.UserName;
                aRICSApproval.CreatedBy = user.UserName;
                aRICSApproval.CreationDate = DateTime.UtcNow;

                resp = await _aRICSApprovalService.AddAsync(aRICSApproval).ConfigureAwait(false);
                if (resp.Success)
                {
                    //Mark the ARICS Approval as submitted
                    msg = "Success";
                    status = true;
                }
                else
                {
                    msg = "Fail";
                }
            }


            return Json(new
            {
                Success = status,
                Message = msg,
                Href = Url.Action("ARICSBatchDetails", "ARICS", new { ARICSMasterApprovalId = ARICSMasterApprovalID })
            });
        }

        [Authorize(Claims.Permission.ARICS.PreparerOrSubmitter)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddEdit(int id, int ARICSYearId, long AuthorityId)
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
                    ARICSMasterApproval aRICSMasterApproval = new ARICSMasterApproval();
                    aRICSMasterApproval.ARICSYearId = ARICSYearId;
                    aRICSMasterApproval.AuthorityId = AuthorityId;
                    return View(aRICSMasterApproval);
                }
                else
                {
                    var resp = await _aRICSMasterApprovalService.FindByIdAsync(ID).ConfigureAwait(false);
                    ARICSMasterApproval aRICSMasterApproval = null;
                    if (resp.Success)
                    {
                        var objectResult = (ObjectResult)resp.IActionResult;
                        if (objectResult != null)
                        {
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var result2 = (OkObjectResult)objectResult;
                                aRICSMasterApproval = (ARICSMasterApproval)result2.Value;
                            }
                        }
                    }
                    if (aRICSMasterApproval == null)
                    {
                        return NotFound();
                    }
                    return View(aRICSMasterApproval);
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSController.AddEdit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "ARICSController.AddEdit Page has reloaded");
                return View();
            }
        }

        // POST: Disbursement/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Claims.Permission.ARICS.PreparerOrSubmitter)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddEdit(long id, [Bind("ID,BatchNo,Description," +
            "AuthorityId,ARICSYearId")] ARICSMasterApproval aRICSMasterApproval)
        {
            try
            {
                bool success = false;
                string msg = null;
                if (id != aRICSMasterApproval.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    ApplicationUser loggedInuser = await GetLoggedInUser().ConfigureAwait(false);
                    //if id=0 then create new
                    if (id == 0)
                    {

                        //add disbursement
                        var disbursementResp = await _aRICSMasterApprovalService.AddAsync(aRICSMasterApproval).ConfigureAwait(false);
                        if (disbursementResp.Success)
                        {
                            success = true;
                            msg = "ARICS Master Approval Entry Successfully Added";
                        }
                    }
                    //if id>0 then its an edit
                    if (id > 0)
                    {
                        var aRICSMasterApprovalResp = await _aRICSMasterApprovalService.Update(id, aRICSMasterApproval).ConfigureAwait(false);
                        if (aRICSMasterApprovalResp.Success)
                        {
                            success = true;
                            msg = "ARICS Master Approval Entry Successfully Updated";
                        }
                    }

                    return Json(new
                    {
                        Success = success,
                        Message = msg,
                        Href = Url.Action("Index", "Disbursement")
                    }); ;
                }
                return Json(new
                {
                    Success = false,
                    Message = "Model Error",
                    Href = Url.Action("Index", "Disbursement")
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSController AddEdit Page Error: {Ex.Message} " +
                $"{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, "ARICSController AddEdit Page has reloaded");
                return View(aRICSMasterApproval);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ARICSBatchDetails(int ARICSMasterApprovalId)
        {
            ARICSBatchViewModel aRICSBatchViewModel
                = new ARICSBatchViewModel();
            try
            {
                //Get logged in user
                ApplicationUser user = await GetLoggedInUser().ConfigureAwait(false);
                aRICSBatchViewModel.Authority = user.Authority;

                //Get ARICSMasterApprovalId
                ARICSMasterApproval aRICSMasterApproval = null;
                var resp = await _aRICSMasterApprovalService.FindByIdAsync(ARICSMasterApprovalId).ConfigureAwait(false);
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            aRICSMasterApproval = (ARICSMasterApproval)result.Value;
                        }
                    }
                }
                //Set aricsmaster approval
                aRICSBatchViewModel.ARICSMasterApproval = aRICSMasterApproval;
                //specify Year
                aRICSBatchViewModel.ARICSYear = aRICSMasterApproval.ARICSYear;
                aRICSBatchViewModel.Year = aRICSMasterApproval.ARICSYear.Year;

                //Get road sections ariced for the ARICS year
                var aricedRoadSectionList = await _aRICSService.
                    GetARICEDRoadSectionByAuthorityAndYear(aRICSBatchViewModel.Authority.ID, aRICSBatchViewModel.Year)
                    .ConfigureAwait(false);

                IList<RoadSection> roadSections = null;
                if (aricedRoadSectionList.Success)
                {

                    var objectResult = (ObjectResult)aricedRoadSectionList.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result2 = (OkObjectResult)objectResult;
                            roadSections = (IList<RoadSection>)result2.Value;
                        }
                    }
                }

                if (roadSections != null)
                {
                    aRICSBatchViewModel.RoadSections = roadSections;
                }
                else
                {
                    aRICSBatchViewModel.RoadSections = (Enumerable.Empty<RoadSection>()).ToList();
                }

                //Get Details About ARICS Approval
                var resp2 = await _aRICSApprovalService.FindByARICSMasterApprovalIdAsync(
                    aRICSBatchViewModel.ARICSMasterApproval.ID).ConfigureAwait(false);
                if (resp2.Success)
                {
                    var objectResult = (ObjectResult)resp2.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result2 = (OkObjectResult)objectResult;
                            aRICSBatchViewModel.ARICSApproval = (ARICSApproval)result2.Value;
                        }
                    }
                }

                //Set nextStatus and PreviousStatus
                await SetStatusParameters(aRICSBatchViewModel).ConfigureAwait(false);

                //get history
                if (aRICSBatchViewModel.ARICSApproval != null)
                {
                    var respHist = await _aRICSApprovalService.ListHistoryAsync(
                    aRICSBatchViewModel.ARICSApproval.ID).ConfigureAwait(false);
                    if (respHist.Success)
                    {
                        var objectResult = (ObjectResult)respHist.IActionResult;
                        if (objectResult != null)
                        {
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var result2 = (OkObjectResult)objectResult;
                                aRICSBatchViewModel.ARICSApprovalh = (IList<ARICSApprovalh>)result2.Value;
                            }
                        }
                    }
                }
                //Set Return URL and store in session
                HttpContext.Session.SetString("Referer", Request.GetEncodedUrl());

                return View(aRICSBatchViewModel);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"ARICSController.ARICSBatchDetails Error : {Ex.Message}");
                ModelState.AddModelError(string.Empty, Ex.Message);
                return View(aRICSBatchViewModel);
            }
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> OnGetARICSBatchRoadSections(long ARICSMasterApprovalID, int ARICSYearId, int Year)
        {
            try
            {
                var user = await GetLoggedInUser().ConfigureAwait(false);

                //Return for authority that user is placed
                var authority = await _authorityService.FindByIdAsync(user.AuthorityId).ConfigureAwait(false);
                IQueryable<ARICSBatchViewModel> ARICSBatchData = null;
                var aRICSMasterApprovalResponse = await _aRICSBatchService.ListByARICSMasterApprovalIdAndARICSYearIdAsync(ARICSMasterApprovalID, ARICSYearId).ConfigureAwait(false);

                var objectResult = (ObjectResult)aRICSMasterApprovalResponse.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result2 = (OkObjectResult)objectResult;
                        ARICSBatchData = (IQueryable<ARICSBatchViewModel>)result2.Value;
                    }
                }


                var roadSectionData = ARICSBatchData;

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
                        if (sortColumn == "sectionid")
                        {
                            roadSectionData = roadSectionData.OrderByDescending(s => s.sectionid);
                        }
                        else if (sortColumn == "sectionname")
                        {
                            roadSectionData = roadSectionData.OrderByDescending(s => s.sectionname);
                        }

                    }
                    else
                    {
                        if (sortColumn == "sectionid")
                        {
                            roadSectionData = roadSectionData.OrderByDescending(s => s.sectionid);
                        }
                        else if (sortColumn == "sectionname")
                        {
                            roadSectionData = roadSectionData.OrderByDescending(s => s.sectionname);
                        }
                    }
                }
                int x = 0;
                IEnumerable<ARICSBatchViewModel> roadSectionsEnum = roadSectionData.AsEnumerable<ARICSBatchViewModel>();
                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    roadSectionsEnum = roadSectionsEnum
                        .Where(
                         m => (m.sectionid != null && m.sectionid.ToLower().Contains(searchValue.ToLower()))
                        || (m.sectionname != null && m.sectionname.ToString().ToLower().Contains(searchValue.ToLower()))
                    );
                }

                //total number of rows count 
                recordsTotal = roadSectionsEnum.Count();
                //Paging 
                var data = roadSectionsEnum.Skip(skip).Take(pageSize).ToList();

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> AddRoadSectiontoApprovalBatch(long ARICSMasterApprovalID, long RoadSectionId, int Year)
        {
            try
            {

                //find by roadsection id and approval arics id
                var resp = await _aRICSBatchService.FindByRoadSectionIdAndARICSMasterApprovalIdAsync(ARICSMasterApprovalID,
                    RoadSectionId).ConfigureAwait(false);
                ARICSBatch aRICSBatch = null;
                if (resp.Success)
                {
                    var objectResult = (ObjectResult)resp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            aRICSBatch = (ARICSBatch)result.Value;
                        }
                    }
                }

                if (aRICSBatch == null)
                {
                    //Add road section to this batch
                    ARICSBatch aRICSBatch2 = new ARICSBatch();
                    aRICSBatch2.ARICSMasterApprovalId = ARICSMasterApprovalID;
                    aRICSBatch2.RoadSectionId = RoadSectionId;
                    var resp2 = await _aRICSBatchService.AddAsync(aRICSBatch2).ConfigureAwait(false);

                    return Json(new
                    {
                        Success = true,
                        Message = "Success",
                        Href = Url.Action("ARICSBatchDetails", "ARICS", new { ARICSMasterApprovalId = ARICSMasterApprovalID })
                    });
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Entry already exists",
                        Href = Url.Action("ARICSBatchDetails", "ARICS", new { ARICSMasterApprovalId = ARICSMasterApprovalID })
                    });
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"WorkplanController.ExportPrioritized Error {Environment.NewLine}");
                return Json(null);
            }
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<JsonResult> RemoveRoadSectiontoApprovalBatch(long ID, long ARICSMasterApprovalID)
        {
            try
            {
                //remove road section to this batch
                var resp2 = await _aRICSBatchService.RemoveAsync(ID).ConfigureAwait(false);

                return Json(new
                {
                    Success = true,
                    Message = "Success",
                    Href = Url.Action("ARICSBatchDetails", "ARICS", new { ARICSMasterApprovalId = ARICSMasterApprovalID })
                });
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSController.RemoveRoadSectiontoApprovalBatch Error {Environment.NewLine}");
                return Json(null);
            }
        }

        #endregion

        #region
        private async Task SetStatusParameters(ARICSBatchViewModel aRICSBatchViewModel)
        {
            //await Task.Run(() =>
            //{

            //}).ConfigureAwait(false);

            //if ARICSApproval is null
            if (aRICSBatchViewModel.ARICSApproval == null)
            {
                //if CG
                if (aRICSBatchViewModel.Authority.Type == 2)
                {
                    aRICSBatchViewModel.NextStatus = 1;
                    aRICSBatchViewModel.ResetStatus = 0;
                }

                //if RA
                if (aRICSBatchViewModel.Authority.Type == 1)
                {
                    aRICSBatchViewModel.NextStatus = 8;
                    aRICSBatchViewModel.ResetStatus = 0;
                }
            }
            else
            {
                //if CG
                if (aRICSBatchViewModel.Authority.Type == 2)
                {
                    aRICSBatchViewModel.ResetStatus = aRICSBatchViewModel.ARICSApproval.ARICSApprovalLevel.Status - 1;
                    aRICSBatchViewModel.NextStatus = aRICSBatchViewModel.ARICSApproval.ARICSApprovalLevel.Status + 1;

                }
                //if RA
                if (aRICSBatchViewModel.Authority.Type == 1)
                {
                    aRICSBatchViewModel.ResetStatus = aRICSBatchViewModel.ARICSApproval.ARICSApprovalLevel.Status - 1;
                    if (aRICSBatchViewModel.ARICSApproval.ARICSApprovalLevel.Status == 10)
                    {
                        aRICSBatchViewModel.NextStatus = 5;//Go to KRB
                    }
                    else
                    {
                        aRICSBatchViewModel.NextStatus = aRICSBatchViewModel.ARICSApproval.ARICSApprovalLevel.Status + 1;
                    }

                }

                //if KRB
                if (aRICSBatchViewModel.Authority.Type == 0)
                {
                    aRICSBatchViewModel.ResetStatus = aRICSBatchViewModel.ARICSApproval.ARICSApprovalLevel.Status - 1;
                    if (aRICSBatchViewModel.ARICSApproval.ARICSApprovalLevel.Status == 7)
                    {
                        aRICSBatchViewModel.NextStatus = 7;//End. Remain there
                    }
                    else
                    {
                        aRICSBatchViewModel.NextStatus = aRICSBatchViewModel.ARICSApproval.ARICSApprovalLevel.Status + 1;
                    }
                }
            }
        }
        #endregion
    }
}