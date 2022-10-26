using System.Globalization;
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using APRP.Web.Extensions;
using APRP.Web.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;

namespace APRP.Web.MyPages.ARICZ
{
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class UploadARICSModel : PageModel
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger _logger;
        private readonly IARICSService _aRICSService;
        private readonly IRoadSheetService _roadSheetService;
        private readonly IARICSUploadService _aRICSUploadService;
        private readonly IRoadSectionService _roadSectionService;
        private readonly IShoulderSurfaceTypePavedService _shoulderSurfaceTypePavedService;
        private readonly IShoulderInterventionPavedService _shoulderInterventionPavedService;
        private readonly ISurfaceTypeUnPavedService _surfaceTypeUnPavedService;
        private readonly IGISRoadService _gISRoadService;
        private readonly ITerrainTypeService _terrainTypeService;
        private readonly IRoadConditionService _roadConditionService;
        private readonly IAuthorityService _authorityService;
        private readonly IKeRRARoadService _keRRARoadService;
        private readonly IKenHARoadService _kenHARoadService;
        private readonly IKuRARoadService _kuRARoadService;
        private readonly IKwSRoadService _kwSRoadService;
        private readonly ICountiesRoadService _countiesRoadService;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IRoadSheetLengthService _roadSheetLengthService;
        private readonly IRoadSheetIntervalService _roadSheetIntervalService;
        private readonly IARICSYearService _aRICSYearService;
        private readonly IGravelRequiredService _gravelRequiredService;

        private CultureInfo _cultures = new CultureInfo("en-US");

        public UploadARICSModel(IRoadSheetService roadSheetService, IARICSService aRICSService,
            IARICSUploadService aRICSUploadService, ILogger<UploadARICSModel> logger,
            IWebHostEnvironment hostingEnvironment, IConfiguration configuration,
            IRoadSectionService roadSectionService,
            IShoulderSurfaceTypePavedService shoulderSurfaceTypePavedService,
            IShoulderInterventionPavedService shoulderInterventionPavedService,
            ISurfaceTypeUnPavedService surfaceTypeUnPavedService,
            IGISRoadService gISRoadService,
            ITerrainTypeService terrainTypeService,
            IRoadConditionService roadConditionService,
            IAuthorityService authorityService,
            IKeRRARoadService keRRARoadService, IKenHARoadService kenHARoadService, IKuRARoadService kuRARoadService,
            IKwSRoadService kwSRoadService, ICountiesRoadService countiesRoadService,
            IApplicationUsersService applicationUsersService, IRoadSheetLengthService roadSheetLengthService,
            IRoadSheetIntervalService roadSheetIntervalService, IARICSYearService aRICSYearService, IGravelRequiredService gravelRequiredService)
        {
            _roadSheetService = roadSheetService;
            Configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            _aRICSService = aRICSService;
            _aRICSUploadService = aRICSUploadService;
            _roadSectionService = roadSectionService;
            _shoulderSurfaceTypePavedService = shoulderSurfaceTypePavedService;
            _shoulderInterventionPavedService = shoulderInterventionPavedService;
            _surfaceTypeUnPavedService = surfaceTypeUnPavedService;
            _gISRoadService = gISRoadService;
            _terrainTypeService = terrainTypeService;
            _roadConditionService = roadConditionService;
            _authorityService = authorityService;
            _keRRARoadService = keRRARoadService;
            _kenHARoadService = kenHARoadService;
            _kuRARoadService = kuRARoadService;
            _kwSRoadService = kwSRoadService;
            _countiesRoadService = countiesRoadService;
            _applicationUsersService = applicationUsersService;
            _roadSheetLengthService = roadSheetLengthService;
            _roadSheetIntervalService = roadSheetIntervalService;
            _aRICSYearService = aRICSYearService;
            _gravelRequiredService = gravelRequiredService;
        }

        public IConfiguration Configuration { get; }


        public County County { get; set; }

        public Constituency Constituency { get; set; }

        [BindProperty]
        public FileUpload FileUpload { get; set; }

        [BindProperty]
        public IList<ARICSUpload> ARICSUpload { get; set; }

        [BindProperty]
        public ARICSUpload MyARICSUpload { get; set; }

        [BindProperty]
        public string Referer { get; set; }

        public string SN { get; set; } = "UK";

        //Road Section
        private RoadSection _RoadSection { get; set; }

        private ApplicationUser _ApplicationUser { get; set; }

        public ARICSYear ARICSYear { get; set; }

        private string _SectionSurfaceType { get; set; }

        //Lookup tables
        IList<ShoulderSurfaceTypePaved> _ShoulderSurfaceTypePaved { get; set; }
        IList<ShoulderInterventionPaved> _ShoulderInterventionPaved { get; set; }
        IList<SurfaceTypeUnPaved> _SurfaceTypeUnPaved { get; set; }
        IList<GravelRequired> _GravelRequiredUnPaved { get; set; }

        public Authority Authority { get; set; }
        private IList<string> _MyRoles { get; set; }

        [BindProperty]
        public RoadSheetLength RoadSheetLength { get; set; }

        [BindProperty]
        public RoadSheetInterval RoadSheetInterval { get; set; }

        [Authorize(Claims.Permission.ARICS.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                //Call page load
                await PageReload().ConfigureAwait(false);

                //Set Return URL
                Referer = Request.Headers["Referer"].ToString();

                //Set Return URL and store in session
                HttpContext.Session.SetString("Referer", Request.GetEncodedUrl());

                return Page();
            }
            catch (Exception Ex)
            {

                _logger.LogError($" Razor Page Error{Ex.Message}");
                ModelState.AddModelError(string.Empty, Ex.Message);
                return Page();
            }
        }
        FileStream _FileStream = null;
        string _FileName = null;

        [Authorize(Claims.Permission.ARICS.ConductARICS)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> OnPostUploadAsync()
        {
            try
            {
                // Perform an initial check to catch FileUpload class attribute violations.
                if (!ModelState.IsValid)
                {
                    //ModelState.AddModelError(string.Empty, ModelState.);
                    return Page();
                }

                //Get logged in user
                _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);

                ////Validate and verify roadsheet length
                var resp0 = await _roadSheetService.ListByRoadSectionIdAsync(MyARICSUpload.RoadSectionId, MyARICSUpload.Year).ConfigureAwait(false);
                if (resp0.Success)
                {
                    //Take only one response. The first response
                    var list = (from t in resp0.RoadSheets select t).Take(1);
                    if (list.Count() > 0)
                    {
                        int roadSheetLength = (int)Math.Ceiling(list.ElementAt(0).EndChainage - list.ElementAt(0).StartChainage);
                        var respRoadSheetLength = await _roadSheetLengthService.FindByIdAsync(RoadSheetLength.ID).ConfigureAwait(false);
                        if (respRoadSheetLength.Success)
                        {
                            if (roadSheetLength > respRoadSheetLength.RoadSheetLength.LengthInKm)
                            {
                                string msg = $"The current RoadSheet Length is set to {roadSheetLength}" +
                                 $" and is different to the new selected roadsheet length {respRoadSheetLength.RoadSheetLength.LengthInKm}";
                                _logger.LogWarning(msg, $"UploadARICS.Upload: {Environment.NewLine}");
                                ModelState.AddModelError(string.Empty, msg);
                                //Call page load
                                await PageReload().ConfigureAwait(false);
                                return Page();
                            }

                        }
                    }
                }

                //// Validate and verify Interval
                RoadSheetVM _RoadSheetVM = new RoadSheetVM();
                _RoadSheetVM.RoadSectionID = MyARICSUpload.RoadSectionId;
                _RoadSheetVM.Year = MyARICSUpload.Year;

                var roadSheetResponse = await _roadSheetService.CheckRoadSheetsForYear(_RoadSheetVM).ConfigureAwait(false);
                RoadSheet _RoadSheet = roadSheetResponse.RoadSheet;
                if (_RoadSheet != null)
                {
                    ARICSVM aRICSVM = new ARICSVM();
                    aRICSVM.RoadSheetID = _RoadSheet.ID;
                    var resp = await _aRICSService.GetARICSBySheetNo(aRICSVM).ConfigureAwait(false);
                    if (resp.Success)
                    {
                        var list = (from t in resp.ARICS.OrderBy(s => s.Chainage) select t).Take(2);
                        if (list.Count() > 1)
                        {
                            int interval = Math.Abs(list.ElementAt(0).Chainage - list.ElementAt(1).Chainage);//Previous
                            var respRoadSheetInterval = await _roadSheetIntervalService.FindByIdAsync(RoadSheetInterval.ID).ConfigureAwait(false);
                            if (interval != respRoadSheetInterval.RoadSheetInterval.IntervalInMeters)
                            {
                                string msg = $"The current interval is set to {respRoadSheetInterval.RoadSheetInterval.IntervalInMeters}" +
                                    $" and is different to the new selected {interval}";
                                _logger.LogWarning(msg, $"UploadARICS.Upload: {Environment.NewLine}");
                                ModelState.AddModelError(string.Empty, msg);
                                //Call page load
                                await PageReload().ConfigureAwait(false);
                                return Page();
                            }
                        }
                    }
                }

                //Upload the excel
                string fileName = FileUpload.UploadPublicSchedule.GetFilename();
                _FileName = fileName;
                var filePath = Path.Combine(
                        _hostingEnvironment.WebRootPath, "uploads", "ARICS", fileName);

                var cont = GetContentType(filePath);
                if (cont == null)
                {
                    string msg = $"Please ensure the uploaded file is an excel file " +
                    $"";
                    _logger.LogWarning(msg, $"UploadARICS.Upload: {Environment.NewLine}");
                    ModelState.AddModelError(string.Empty, msg);
                    //Call page load
                    await PageReload().ConfigureAwait(false);
                    return Page();
                }

                using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    await FileUpload.UploadPublicSchedule.CopyToAsync(fileStream).ConfigureAwait(false);
                    _FileStream = fileStream;
                }

                //Call function to perform initial chek on the excel upload
                //This initial checks Road ID in the excel matches the Road ID the user selected in the drop down
                var dict = await ValidateExcel().ConfigureAwait(false);

                if (dict.ContainsKey(0))//Off, False
                {
                    _logger.LogWarning(dict[0], $"UploadARICS.Upload: {Environment.NewLine}");
                    ModelState.AddModelError(string.Empty, dict[0]);
                    //Call page load
                    await PageReload().ConfigureAwait(false);
                    return Page();
                }

                //Update Roadsection ARICS Data
                await UpdateRoadSectionARICSData().ConfigureAwait(false);

                //Record uploaded file in the ARICS Upload
                ARICSUpload _ARICSUpload = new ARICSUpload();
                _ARICSUpload.Year = MyARICSUpload.Year;
                _ARICSUpload.filename = fileName;
                _ARICSUpload.RoadSectionId = MyARICSUpload.RoadSectionId;

                var aRICSUploadResponse = await _aRICSUploadService.AddAsync(_ARICSUpload).ConfigureAwait(false);
                if (aRICSUploadResponse.Success == true)
                {
                    //Call function to load ARICS 
                    await GetARICSUploads().ConfigureAwait(false);
                }

                //Call function asynchrously read and write the uploaded arics to DB
                await Task.Run(() => AutoLoadARICS()).ConfigureAwait(false);

                //Dispose
                _FileStream.Dispose();
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"ARICSUploadModel.OnPostAync File UploadError: \r\n {Environment.NewLine}");
                ModelState.AddModelError(string.Empty, Ex.Message.ToString());
                //Call page load
                await PageReload().ConfigureAwait(false);
                return Page();
            }
            //return Redirect(Referer);

            return RedirectToPage("/ARICZ/UploadARICS");
        }

        public async Task<IActionResult> OnPostSwitchToDirectDataEntryAsync()
        {
            return RedirectToPage("/ARICZ/ConductARICS");
        }
        #region Utilities
        private async Task PopulateDropDowns()
        {
            //Road Lengths
            var resp = await _roadSheetLengthService.ListAsync().ConfigureAwait(false);
            IList<RoadSheetLength> roadSheetLengths = (IList<RoadSheetLength>)resp.RoadSheetLength;
            ViewData["RoadSheetLengthId"] = new SelectList(roadSheetLengths, "ID", "LengthInKm");

            //RoadSheet Interval
            var respRoadSheetInterval = await _roadSheetIntervalService.ListAsync().ConfigureAwait(false);
            IList<RoadSheetInterval> roadSheetIntervals = (IList<RoadSheetInterval>)respRoadSheetInterval.RoadSheetInterval;
            ViewData["RoadSheetIntervalId"] = new SelectList(roadSheetIntervals, "ID", "IntervalInMeters");

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
            ViewData["ARICSYearId"] = new SelectList(aRICSYears, "Year", "Year", DateTime.UtcNow.Year);
        }

        private async Task PageReload()
        {
            //Get logged in user
            _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);
            Authority = _ApplicationUser.Authority;

            //Get uploaded ARICS
            await GetARICSUploads().ConfigureAwait(false);

            //Populate dropdowns
            await PopulateDropDowns().ConfigureAwait(false);
        }

        #endregion

        #region Utilities

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
                        //string username
                        string username = user.UserName;
                        SN = username.Substring(0, 2).ToUpper();
                    }
                }
            }
            return user;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<Dictionary<int, string>> ValidateExcel()
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();

            var filePath = Path.Combine(
           _hostingEnvironment.WebRootPath, "uploads", "ARICS", _FileName);
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            //Get Road Section id and ARICS Upload Year 
            long RealRoadSectionID = MyARICSUpload.RoadSectionId;

            //Find Road Section By ID
            if (_RoadSection == null)
            {
                var roadSectionListResponse = await _roadSectionService.FindByIdAsync(RealRoadSectionID).ConfigureAwait(false);
                _RoadSection = (RoadSection)roadSectionListResponse.RoadSection;
            }
            string RoadIDSystem = _RoadSection.Road.RoadNumber;
            string RoadIDFile = null;

            //Get filestream
            if (fs != null)
            {
                //Copy filestream to memory stream
                MemoryStream ms = new MemoryStream();
                await fs.CopyToAsync(ms).ConfigureAwait(true);


                using (var package = new ExcelPackage(ms))
                {
                    try
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        if (worksheet.Cells[5, 2].Value != null)
                        {
                            RoadIDFile = worksheet.Cells[5, 2].Value.ToString().Trim();
                        }
                    }
                    catch (Exception Ex)
                    {
                        _logger.LogInformation($"UploadARICSModel.ValidateExcel: \r\n {Ex.Message}");
                    }
                }
            }

            if (RoadIDSystem != RoadIDFile)
            {
                string msg = $"Road ID from the roads register : {RoadIDSystem}" +
                    $" doesn't match Road ID : {RoadIDFile} in the upload excel file";
                if (dict.ContainsKey(0))
                {
                    dict[0] = msg;
                }
                else
                {
                    dict.Add(0, msg);
                }
            }
            return dict;
        }

        #endregion

        #region Auto Load ARICS
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task GetARICSUploads()
        {
            try
            {
                var aRICSUploadListResponse = await _aRICSUploadService.ListAsync().ConfigureAwait(false);
                ARICSUpload = (IList<ARICSUpload>)aRICSUploadListResponse.ARICSUpload;
            }
            catch (Exception Ex)
            {
                _logger.LogError($" UploadsARICS.GetARICS Razor Page Error" +
                    $"{Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task AutoLoadARICS()
        {
            try
            {
                //Get Road Section id and ARICS Upload Year 
                long RealRoadSectionID = MyARICSUpload.RoadSectionId;
                int Year = MyARICSUpload.Year;

                /*
                 * Check if roadsheets are created. 
                 * if not create the respective roadsheets
                 */
                RoadSheetVM _RoadSheetVM = new RoadSheetVM();
                _RoadSheetVM.RoadSectionID = RealRoadSectionID;
                _RoadSheetVM.Year = Year;

                //Find Road Section By ID
                if (_RoadSection == null)
                {
                    var roadSectionListResponse = await _roadSectionService.FindByIdAsync(RealRoadSectionID).ConfigureAwait(false);
                    _RoadSection = (RoadSection)roadSectionListResponse.RoadSection;
                }

                //Get Surface Type for the road for which road section is based
                await GetSurfaceType().ConfigureAwait(false);

                //Set other required properties
                var respRoadSheetInterval = await _roadSheetIntervalService.FindByIdAsync(RoadSheetInterval.ID).ConfigureAwait(false);
                _RoadSheetVM.Interval = respRoadSheetInterval.RoadSheetInterval.IntervalInMeters;//Meters

                var resp = await _roadSheetLengthService.FindByIdAsync(RoadSheetLength.ID).ConfigureAwait(false);
                _RoadSheetVM.SectionLengthKM = resp.RoadSheetLength.LengthInKm;
                _RoadSheetVM.RoadLengthKM = _RoadSection.Length; //kilometeres

                //Check if roadsheets exists for the particular roadsection_id            
                RoadSheet _RoadSheet = await
                InternalCheckRoadSheetForRoadSection(_RoadSheetVM).ConfigureAwait(false);
                if (_RoadSheet == null)
                {
                    //Call function to create roadsheets
                    await CreateRoadSheets(_RoadSheetVM).ConfigureAwait(false);

                    //Create empty ARICS
                    await LoadARICS(_RoadSheetVM).ConfigureAwait(false);
                }
                else
                {
                    //Load/Update ARICS
                    await LoadARICS(_RoadSheetVM).ConfigureAwait(false);
                }

                //Call function to update road conditions 
                await UpdateRoadConditionTable(_RoadSection.Road, Year).ConfigureAwait(false);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"UploadARICS.OnPostAync  Method AutoLoadARICS: \r\n {Environment.NewLine}");
            }
        }

        private async Task GetSurfaceType()
        {
            //Road section
            if (_RoadSection != null)
            {
                //Get authority
                var authority = await _authorityService.FindByIdAsync(_RoadSection.Road.AuthorityId).ConfigureAwait(false);
                if (authority.Authority != null && authority.Authority.ID == 2)//KeRRA
                {
                    //Get road surface type and set global variable
                    var kerraRdResp = await _keRRARoadService.FindBySectionIdAsync(_RoadSection.SectionID).ConfigureAwait(false);
                    KerraRoad kerraRoad = kerraRdResp.KerraRoad;
                    if (kerraRoad != null)
                    {
                        if (kerraRoad.SurfaceType == "Paved")
                        {
                            _SectionSurfaceType = "Paved";
                        }
                        else
                        {
                            _SectionSurfaceType = "UnPaved";
                        }
                    }
                    else //manually added road
                    {
                        if (_RoadSection.SurfaceType.Name == "Paved")
                        {
                            _SectionSurfaceType = "Paved";
                        }
                        else
                        {
                            _SectionSurfaceType = "UnPaved";
                        }
                    }

                }

                if (authority.Authority != null && authority.Authority.ID == 1)//KenHA
                {
                    //Get road surface type and set global variable
                    var kenHARdResp = await _kenHARoadService.FindBySectionIdAsync(_RoadSection.SectionID).ConfigureAwait(false);
                    KenhaRoad kenhaRoad = kenHARdResp.KenhaRoad;
                    if (kenhaRoad != null)
                    {
                        if (kenhaRoad.SurfaceType == "UnPaved")
                        {
                            _SectionSurfaceType = "UnPaved";
                        }
                        else
                        {
                            _SectionSurfaceType = "Paved";
                        }
                    }
                    else //manually added roads
                    {
                        if (_RoadSection.SurfaceType.Name == "Paved")
                        {
                            _SectionSurfaceType = "Paved";
                        }
                        else
                        {
                            _SectionSurfaceType = "UnPaved";
                        }
                    }

                }

                if (authority.Authority != null && authority.Authority.ID == 3)//KURA
                {
                    //Get road surface type and set global variable
                    var kuRARdResp = await _kuRARoadService.FindByRoadNumberAsync(_RoadSection.Road.RoadNumber).ConfigureAwait(false);
                    KuraRoad kuraRoad = kuRARdResp.KuraRoad;
                    if (kuraRoad != null)
                    {
                        if (kuraRoad.RdClass == "Paved")
                        {
                            _SectionSurfaceType = "Paved";
                        }
                        else
                        {
                            _SectionSurfaceType = "UnPaved";
                        }
                    }
                    else //manually added roads
                    {
                        if (_RoadSection.SurfaceType.Name == "Paved")
                        {
                            _SectionSurfaceType = "Paved";
                        }
                        else
                        {
                            _SectionSurfaceType = "UnPaved";
                        }
                    }

                }

                if (authority.Authority != null && authority.Authority.ID == 4)//KWS
                {
                    //Get road surface type and set global variable
                    var kwSRdResp = await _kwSRoadService.FindBySectionIdAsync(_RoadSection.SectionID).ConfigureAwait(false);
                    KwsRoad kwsRoad = kwSRdResp.KwsRoad;
                    if (kwsRoad != null)
                    {
                        if (kwsRoad.SurfaceType == "Paved")
                        {
                            _SectionSurfaceType = "Paved";
                        }
                        else
                        {
                            _SectionSurfaceType = "UnPaved";
                        }
                    }
                    else //Roads added manually
                    {
                        if (_RoadSection.SurfaceType.Name == "Paved")
                        {
                            _SectionSurfaceType = "Paved";
                        }
                        else
                        {
                            _SectionSurfaceType = "UnPaved";
                        }
                    }
                }

                if (authority.Authority != null && authority.Authority.ID > 4)//Counties
                {
                    //Get road surface type and set global variable
                    var countiesRdResp = await _countiesRoadService.FindByRoadNumberAsync(_RoadSection.Road.RoadNumber).ConfigureAwait(false);
                    CountiesRoad countiesRoad = countiesRdResp.CountiesRoad;
                    if (countiesRoad != null)
                    {
                        if (countiesRoad.SurfaceType == "Paved")
                        {
                            _SectionSurfaceType = "Paved";
                        }
                        else
                        {
                            _SectionSurfaceType = "UnPaved";
                        }
                    }
                    else //manually added
                    {
                        if (_RoadSection.SurfaceType.Name == "Paved")
                        {
                            _SectionSurfaceType = "Paved";
                        }
                        else
                        {
                            _SectionSurfaceType = "UnPaved";
                        }
                    }

                }
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task LoadARICS(RoadSheetVM _RoadSheetVM)
        {

            //Populate arics if not existent
            ARICSVM _ARICSVM = new ARICSVM();
            int _Interval = 0;
            bool result = int.TryParse(_RoadSheetVM.Interval.ToString(_cultures), out _Interval);
            _ARICSVM.Interval = _Interval;
            IList<RoadSheet> roadSheets = await PopulateARICS(_ARICSVM).ConfigureAwait(false);

            var filePath = Path.Combine(
                       _hostingEnvironment.WebRootPath, "uploads", "ARICS", _FileName);
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            //Get filestream
            if (fs != null)
            {
                //Load lookup tables
                await LoadLookUpTable().ConfigureAwait(false);

                //Copy filestream to memory stream
                MemoryStream ms = new MemoryStream();
                await fs.CopyToAsync(ms).ConfigureAwait(true);

                using (var package = new ExcelPackage(ms))
                {
                    //Loop through the roadsheets and 
                    // for each roadsheet populate arics from excel spreadsheets
                    foreach (var roadSheet in roadSheets)
                    {
                        _ARICSVM.RoadSheetID = roadSheet.ID;//Put Roadsheet ID

                        //Loop through the worksheet and update accordingly the arics entries
                        try
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[roadSheet.SheetNo];
                            await ReadExcelAndUpdateARICSTable(worksheet, _ARICSVM, roadSheet).ConfigureAwait(true);

                            //Update Roadsheet
                            var roadSheetResponse = await _roadSheetService.Update(roadSheet.ID, roadSheet).ConfigureAwait(false);
                        }
                        catch (Exception Ex)
                        {
                            _logger.LogInformation($"UploadARICSModel.LoadARICS: \r\n {Ex.Message}");
                        }
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task ReadExcelAndUpdateARICSTable(ExcelWorksheet worksheet, ARICSVM _ARICSVM, RoadSheet _RoadSheet)
        {
            try
            {
                var rowCount = worksheet.Dimension.Rows;
                for (int row = 5; row <= rowCount; row++)
                {
                    int val = 0;
                    //Chainage
                    int Chainage; int Chainage2; string str = null;
                    try
                    {
                        if (worksheet.Cells[row, 1].Value != null)
                        {
                            str = worksheet.Cells[row, 1].Value.ToString().Trim();
                        }

                    }
                    catch (Exception Ex)
                    {

                        _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}");
                    }
                    string[] arr = str.Split("+");
                    if (arr.Length > 1)
                    {
                        /* 
                         * Custom Format : e.g 75+200
                         */
                        string str1 = arr[0];
                        string str2 = arr[1];

                        bool result1 = int.TryParse(str1, out Chainage);
                        Chainage *= 1000; //Convert to meters
                        result1 = int.TryParse(str2, out Chainage2);
                        Chainage += Chainage2; //Get total chainage in meters
                    }
                    else
                    {
                        /*
                         * Mormal string = 75200
                         */
                        bool result1 = int.TryParse(str, out Chainage);
                    }

                    long shoulderSurfaceTypePavedId = 0;
                    long shoulderInterventionPavedId = 0;
                    if (_SectionSurfaceType == "Paved")
                    {
                        /*
                         * -----Start Paved----
                         */
                        //Shoulder Surface Type Paved
                        string Paved = null;
                        try
                        {
                            if (worksheet.Cells[row, 2].Value != null)
                            {
                                Paved = worksheet.Cells[row, 2].Value.ToString().Trim();
                                shoulderSurfaceTypePavedId = _ShoulderSurfaceTypePaved.Where(s => s.Code == Paved.ToUpper(_cultures)).FirstOrDefault().ID;
                            }

                        }
                        catch (Exception Ex)
                        {

                            _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}");
                        }

                        //Shoulder Intervention Required
                        string InterventionPaved = null;
                        try
                        {
                            if (worksheet.Cells[row, 3].Value != null)
                            {
                                InterventionPaved = worksheet.Cells[row, 3].Value.ToString().Trim();
                                shoulderInterventionPavedId = _ShoulderInterventionPaved.Where(s => s.Code == InterventionPaved.ToUpper(_cultures)).FirstOrDefault().ID;
                            }

                        }
                        catch (Exception Ex)
                        {

                            _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}");
                        }

                        /*
                         * -----End Paved----
                         */
                    }

                    long surfaceTypeUnPavedId = 0;
                    long gravelRequiredID = 0;

                    if (_SectionSurfaceType == "Unpaved")
                    {
                        /*
                         * ----Start Unpaved
                         */
                        string SurfaceTypeUnpaved = null;
                        try
                        {
                            if (worksheet.Cells[row, 2].Value != null)
                            {
                                SurfaceTypeUnpaved = worksheet.Cells[row, 2].Value.ToString().Trim();
                                surfaceTypeUnPavedId = _SurfaceTypeUnPaved.Where(s => s.Code == SurfaceTypeUnpaved.ToUpper(_cultures)).FirstOrDefault().ID;
                            }
                        }
                        catch (Exception Ex)
                        {

                            _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}");
                        }
                        string GravelRequired = null;
                        try
                        {
                            if (worksheet.Cells[row, 3].Value != null)
                            {
                                GravelRequired = worksheet.Cells[row, 3].Value.ToString().Trim();
                                gravelRequiredID = _GravelRequiredUnPaved.Where(s => s.Code == GravelRequired.ToUpper(_cultures)).FirstOrDefault().ID;
                            }

                        }
                        catch (Exception Ex)
                        {

                            _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}");
                        }
                        /*
                         * ----End Unpaved
                         */
                    }

                    //Rate of depreciation
                    int RateOfDepreciation = 0; string val1 = null; string val2 = null;
                    string val3 = null; string val4 = null; string val5 = null;
                    try
                    {
                        if (worksheet.Cells[row, 4].Value != null)
                        {
                            val1 = worksheet.Cells[row, 4].Value.ToString().Trim();
                        }

                    }
                    catch (Exception Ex)
                    {

                        _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}");
                    }
                    try
                    {
                        if (worksheet.Cells[row, 5].Value != null)
                        {
                            val2 = worksheet.Cells[row, 5].Value.ToString().Trim();
                        }

                    }
                    catch (Exception Ex)
                    {

                        _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}");
                    }
                    try
                    {
                        if (worksheet.Cells[row, 6].Value != null)
                        {
                            val3 = worksheet.Cells[row, 6].Value.ToString().Trim();
                        }

                    }
                    catch (Exception Ex)
                    {

                        _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}");
                    }
                    try
                    {
                        if (worksheet.Cells[row, 7].Value != null)
                        {
                            val4 = worksheet.Cells[row, 7].Value.ToString().Trim();
                        }

                    }
                    catch (Exception Ex)
                    {
                        _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}");
                    }
                    try
                    {
                        if (worksheet.Cells[row, 8].Value != null)
                        {
                            val5 = worksheet.Cells[row, 8].Value.ToString().Trim();
                        }

                    }
                    catch (Exception Ex)
                    {
                        _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}");
                    }
                    if (String.IsNullOrEmpty(val1) == false) { RateOfDepreciation = 1; }
                    if (String.IsNullOrEmpty(val2) == false) { RateOfDepreciation = 2; }
                    if (String.IsNullOrEmpty(val3) == false) { RateOfDepreciation = 3; }
                    if (String.IsNullOrEmpty(val4) == false) { RateOfDepreciation = 4; }
                    if (String.IsNullOrEmpty(val5) == false) { RateOfDepreciation = 5; }

                    //Spot Improvement
                    string SpotImprovement = null;
                    try
                    {
                        if (worksheet.Cells[row, 9].Value != null)
                        {
                            SpotImprovement = worksheet.Cells[row, 9].Value.ToString().Trim();
                        }

                    }
                    catch (Exception Ex)
                    {

                        _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}");
                    }

                    //New Line Required-N
                    string NewLineRequired = null;
                    try
                    {
                        if (worksheet.Cells[row, 10].Value != null)
                        {
                            NewLineRequired = worksheet.Cells[row, 10].Value.ToString().Trim();
                        }

                    }
                    catch (Exception Ex)
                    {

                        _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}");
                    }


                    //New Repair of Ring-RR
                    string RepairOfRing = null;
                    try
                    {
                        if (worksheet.Cells[row, 11].Value != null)
                        {
                            RepairOfRing = worksheet.Cells[row, 11].Value.ToString().Trim();
                        }

                    }
                    catch (Exception Ex)
                    {

                        _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}");
                    }

                    //New Headwall Repair-HR
                    string HeadwallRepair = null;
                    try
                    {
                        if (worksheet.Cells[row, 12].Value != null)
                        {
                            HeadwallRepair = worksheet.Cells[row, 12].Value.ToString().Trim();
                        }

                    }
                    catch (Exception Ex)
                    {

                        _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}");
                    }

                    //New Headwall-NH
                    string NewHeadwall = null;
                    try
                    {
                        if (worksheet.Cells[row, 13].Value != null)
                        {
                            NewHeadwall = worksheet.Cells[row, 13].Value.ToString().Trim();
                        }

                    }
                    catch (Exception Ex)
                    {

                        _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}");
                    }

                    //Good-G
                    string Good = null;
                    try
                    {
                        if (worksheet.Cells[row, 14].Value != null)
                        {
                            Good = worksheet.Cells[row, 14].Value.ToString().Trim();
                        }

                    }
                    catch (Exception Ex)
                    {

                        _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}");
                    }

                    //Blocked-B
                    string Blocked = null;
                    try
                    {
                        if (worksheet.Cells[row, 15].Value != null)
                        {
                            Blocked = worksheet.Cells[row, 15].Value.ToString().Trim();
                        }

                    }
                    catch (Exception Ex)
                    {
                        _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}");
                    }

                    //Other Structure Remarks
                    string OtherStructureRemarks = null;
                    try
                    {
                        if (worksheet.Cells[row, 16].Value != null)
                        {
                            OtherStructureRemarks = worksheet.Cells[row, 16].Value.ToString().Trim();
                        }

                    }
                    catch (Exception Ex)
                    {

                        _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}");
                    }

                    //Get ARICS Object by RoadSheetID and Chainage
                    var aricsServiceResponse = await _aRICSService.FindByRoadSheetAndChainageAsync(
                        _ARICSVM.RoadSheetID, Chainage).ConfigureAwait(false);
                    ARICS aRICS = (ARICS)aricsServiceResponse.ARICS;

                    if (aRICS != null)
                    {
                        //Paved
                        if (shoulderSurfaceTypePavedId != 0)
                        {
                            try
                            {
                                aRICS.ShoulderSurfaceTypePavedId = shoulderSurfaceTypePavedId;
                            }
                            catch (Exception Ex)
                            {

                                _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}");
                            }
                        }
                        if (shoulderInterventionPavedId != 0)
                        {
                            try
                            {
                                aRICS.ShoulderInterventionPavedId = shoulderInterventionPavedId;
                            }
                            catch (Exception Ex)
                            {

                                _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}");
                            }
                        }


                        //For Unpaved
                        if (surfaceTypeUnPavedId != 0)
                        {
                            try
                            {
                                aRICS.SurfaceTypeUnPavedId = surfaceTypeUnPavedId;
                            }
                            catch (Exception Ex)
                            {

                                _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}");
                            }
                        }
                        if (gravelRequiredID != 0)
                        {
                            try
                            {

                                aRICS.GravelRequiredId = gravelRequiredID;
                            }
                            catch (Exception Ex)
                            {

                                _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}");
                            }
                        }


                        //Set Rate of Depreciation
                        aRICS.RateOfDeterioration = RateOfDepreciation;

                        //Spot Improvement Remarks
                        aRICS.SpotImprovementRemarks = SpotImprovement;

                        //Culvert New Required
                        bool result = int.TryParse(NewLineRequired, out val);
                        aRICS.CulvertN = val;

                        //Culvert Repair of Ring
                        result = int.TryParse(RepairOfRing, out val);
                        aRICS.CulvertRR = val;

                        //Culvert Headwall Ring
                        result = int.TryParse(HeadwallRepair, out val);
                        aRICS.CulvertHR = val;

                        //Culvert New Headwall Wall
                        result = int.TryParse(NewHeadwall, out val);
                        aRICS.CulvertNH = val;

                        //Culvert Good
                        result = int.TryParse(Good, out val);
                        aRICS.CulvertG = val;

                        //Culvert Blocked
                        result = int.TryParse(Blocked, out val);
                        aRICS.CulvertB = val;

                        //Other Structure Remarks
                        aRICS.OtherStructureRemarks = OtherStructureRemarks;

                        //Update ARICS
                        aricsServiceResponse = await _aRICSService.Update(
                            aRICS.ID, aRICS).ConfigureAwait(false);
                    }

                }

                /*----Start---Update Road Sheet Details------*/
                string str3 = null;
                //start location
                try
                {
                    if (worksheet.Cells[1, 10].Value != null)
                    {
                        str3 = worksheet.Cells[1, 10].Value.ToString().Trim();
                        _RoadSheet.StartLocation = str3;
                    }

                }
                catch (Exception Ex)
                { _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}"); }
                //End location
                try
                {
                    if (worksheet.Cells[2, 10].Value != null)
                    {
                        str3 = worksheet.Cells[2, 10].Value.ToString().Trim();
                        _RoadSheet.EndLocation = str3;
                    }

                }
                catch (Exception Ex)
                { _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}"); }

                //Carriage Width
                try
                {
                    double carriageWidth = 0.0;
                    str3 = worksheet.Cells[3, 10].Value.ToString().Trim();
                    bool result = Double.TryParse(str3, out carriageWidth);
                    _RoadSheet.CarriageWidth = carriageWidth;
                }
                catch (Exception Ex)
                { _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}"); }

                //Terrain Type
                try
                {
                    str3 = worksheet.Cells[3, 16].Value.ToString().Trim();
                    var terrainTypeResponse = await _terrainTypeService.ListAsync().ConfigureAwait(false);
                    IList<TerrainType> terrainType = (IList<TerrainType>)
                        terrainTypeResponse.TerrainType;
                    long terrainTypeId = terrainType.Where(s => s.Code == str3).FirstOrDefault().ID;
                    _RoadSheet.TerrainTypeId = terrainTypeId;
                }
                catch (Exception Ex)
                { _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}"); }

                //Priority for spot Improvement in this section
                try
                {
                    string str = null;
                    if (worksheet.Cells["R2"].Value != null)
                    {
                        str = worksheet.Cells["R2"].Value.ToString().Trim();
                    }
                    _RoadSheet.SpotImprovementPriority = str;
                }
                catch (Exception Ex)
                { _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}"); }

                //Priority for structure Improvement in this section
                try
                {
                    str3 = worksheet.Cells["R3"].Value == null ? "" : worksheet.Cells["R3"].Value.ToString().Trim();

                    _RoadSheet.StructurePriority = str3;
                }
                catch (Exception Ex)
                { _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}"); }

                //Compiled by
                try
                {
                    _RoadSheet.CompiledBy = _ApplicationUser.UserName;
                }
                catch (Exception Ex)
                { _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}"); }

                //Date
                try
                {
                    _RoadSheet.Date = DateTime.UtcNow;
                }
                catch (Exception Ex)
                { _logger.LogInformation($"UploadARICSModel.ReadExcelAndUpdateARICSTable: \r\n {Ex.Message}"); }

                /*----Start---Update Road Sheet Details------*/
            }
            catch (Exception Ex)
            {
                throw new Exception("Error Reading ARICS Data from Excel", Ex);
            }


        }

        private async Task<IList<RoadSheet>> PopulateARICS(ARICSVM _ARICSVM)
        {
            IList<ARICS> ARICS = null;
            ARICS myARICS = null;

            //Get Roadsheets for the selected roadsectionid
            var _roadSheetListResponse = await _roadSheetService.ListByRoadSectionIdAsync(
                MyARICSUpload.RoadSectionId, MyARICSUpload.Year).ConfigureAwait(false);

            IList<RoadSheet> roadSheets = (IList<RoadSheet>)_roadSheetListResponse.RoadSheets;
            int incrementalChainage = 0;
            foreach (var roadsheet in roadSheets)
            {
                //get roadsheet id
                _ARICSVM.RoadSheetID = roadsheet.ID;
                //Initialize value for Incrementalchainage
                _ARICSVM.incrementalChainage = incrementalChainage;
                /*--GetSection length for the roadsheet*/
                _ARICSVM.SectionLengthKM = Math.Ceiling(roadsheet.EndChainage - roadsheet.StartChainage);

                var aRICSResponse = await _aRICSService.CheckARICSForSheet(_ARICSVM).ConfigureAwait(false);
                myARICS = aRICSResponse.ARICS;

                if (myARICS == null)
                {
                    var aRICSListResponse = await _aRICSService.CreateARICSForSheet(_ARICSVM).ConfigureAwait(false);
                    //ARICS = (IList<ARICS>)aRICSListResponse.ARICS;
                }
                else
                {
                    var aRICSListResponse = await _aRICSService.GetARICSForSheet(_ARICSVM).ConfigureAwait(false);
                    //ARICS = (IList<ARICS>)aRICSListResponse.ARICS;
                }

                //Inrease incremental chainage by a value equal to section Length
                incrementalChainage += 5; //Todo: Inremental Chainage for ARICS Sheet
            }

            return roadSheets;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task CreateRoadSheets(RoadSheetVM _RoadSheetVM)
        {

            try
            {
                //check if roadsheets exists for the particular roadsection_id
                RoadSheet _RoadSheet = await
                InternalCheckRoadSheetForRoadSection(_RoadSheetVM).ConfigureAwait(false);

                if (_RoadSheet == null)
                {
                    var RoadSheetListResponse = await _roadSheetService.CreateRoadSheets(_RoadSheetVM.RoadLengthKM, _RoadSheetVM.SectionLengthKM, _RoadSheetVM.RoadSectionID, _RoadSheetVM.Year).ConfigureAwait(false);
                    IList<RoadSheet> _RoadSheetList = (IList<RoadSheet>)RoadSheetListResponse.RoadSheets;
                }
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"UploadARICS.CreateRoadSheets Error: {Ex.Message}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<RoadSheet> InternalCheckRoadSheetForRoadSection(RoadSheetVM _RoadSheetVM)
        {
            try
            {
                var roadSheetResponse = await _roadSheetService.CheckRoadSheetsForYear(_RoadSheetVM).ConfigureAwait(false);
                RoadSheet _RoadSheet = roadSheetResponse.RoadSheet;
                //check if roadsheets exists for the particular roadsection_id
                return _RoadSheet;
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"UploadARICS.InternalCheckRoadSheetForRoadSection Error: {Ex.Message}");
                return null;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task LoadLookUpTable()
        {
            try
            {
                //Get default/NA Shoulder Surface Type Paved
                var shoulderSurfaceTypePavedResponse = await _shoulderSurfaceTypePavedService.ListAsync().ConfigureAwait(false);
                _ShoulderSurfaceTypePaved = (IList<ShoulderSurfaceTypePaved>)shoulderSurfaceTypePavedResponse.ShoulderSurfaceTypePaved;


                //Get default/NA Shoulder Intervention Paved
                var shoulderInterventionPavedResponse = await _shoulderInterventionPavedService.ListAsync().ConfigureAwait(false);
                _ShoulderInterventionPaved = (IList<ShoulderInterventionPaved>)shoulderInterventionPavedResponse.ShoulderInterventionPaved;

                //Get default/NA Shoulder Intervention Paved
                var surfaceTypeUnPavedResponse = await _surfaceTypeUnPavedService.ListAsync().ConfigureAwait(false);
                _SurfaceTypeUnPaved = (IList<SurfaceTypeUnPaved>)surfaceTypeUnPavedResponse.SurfaceTypeUnPaved;

                //_GravelRequiredUnPaved
                var gravelRequiredResp = await _gravelRequiredService.ListAsync().ConfigureAwait(false);
                if (gravelRequiredResp.Success)
                {
                    var objectResult = (ObjectResult)gravelRequiredResp.IActionResult;
                    if (objectResult != null)
                    {
                        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var result = (OkObjectResult)objectResult;
                            _GravelRequiredUnPaved = (IList<GravelRequired>)result.Value;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError($"UploadARICS.LoadLookUpTable Error: {Ex.Message}");
                //throw;
            }

        }
        private async Task UpdateRoadConditionTable(Road road, int? Year)
        {
            //Get aRICS for road for year
            var aricsResponse = await _aRICSService.GetARICSForRoad(road, Year).ConfigureAwait(false);
            //Compute IRI
            var aricsDataResp = await _aRICSService.GetIRI((IList<ARICS>)aricsResponse.ARICS).ConfigureAwait(false);

            //Update RoadConditions
            var roadConditionResponse = await _roadConditionService.GetRoadConditionByYear(road, Year).ConfigureAwait(false);
            RoadCondition roadCondition = roadConditionResponse.RoadCondtion;

            //Get Year an int
            int _Year;
            bool result = int.TryParse(Year.ToString(), out _Year);
            if (result == false)
            {
                _Year = DateTime.UtcNow.Year;
            }

            if (roadCondition == null)
            {
                roadCondition = new RoadCondition();
                roadCondition.ComputationTime = DateTime.UtcNow;
                roadCondition.ARD = aricsDataResp.ARICSData.RateOfDeterioration;
                roadCondition.Year = _Year;
                roadCondition.RoadId = road.ID;
                roadCondition.PriorityRate = 999999;//Default 999999 meaning unclassified
                var roadConditionResponse2 = await _roadConditionService.AddAsync(roadCondition).ConfigureAwait(false);

            }
            else
            {
                roadCondition.ComputationTime = DateTime.UtcNow;
                roadCondition.ARD = aricsDataResp.ARICSData.RateOfDeterioration;
                roadCondition.Year = _Year;
                roadCondition.RoadId = road.ID;
                //roadCondition.RoadPrioritizationId = 1;//Default 0 meaning unclassified
                var roadConditionResponse2 = await _roadConditionService.Update(roadCondition.ID, roadCondition).ConfigureAwait(false);
            }
        }

        private async Task UpdateRoadSectionARICSData()
        {
            if (_RoadSection != null)
            {
                //Get file
                var filePath = Path.Combine(
                _hostingEnvironment.WebRootPath, "uploads", "ARICS", _FileName);
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                //Get filestream
                if (fs != null)
                {
                    //Copy filestream to memory stream
                    MemoryStream ms = new MemoryStream();
                    await fs.CopyToAsync(ms).ConfigureAwait(true);


                    using (var package = new ExcelPackage(ms))
                    {
                        try
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                            //ARICS regions
                            try
                            {
                                if (worksheet.Cells[3, 2].Value != null)
                                {
                                    _RoadSection.ARICSRegion = worksheet.Cells[3, 2].Value.ToString().Trim();
                                   
                                }

                            }
                            catch (Exception Ex)
                            {

                                _logger.LogInformation($"UploadARICSModel.UpdateRoadSectionARICSData: \r\n {Ex.Message}");
                            }

                            //ARICS regions
                            try
                            {
                                if (worksheet.Cells[3, 7].Value != null)
                                {
                                    _RoadSection.ARICSCategoryType = worksheet.Cells[3, 7].Value.ToString().Trim();
                                }

                            }
                            catch (Exception Ex)
                            {

                                _logger.LogInformation($"UploadARICSModel.UpdateRoadSectionARICSData: \r\n {Ex.Message}");
                            }

                            //ARICS Road name
                            try
                            {
                                if (worksheet.Cells[5, 7].Value != null)
                                {
                                    _RoadSection.ARICSRoadName = worksheet.Cells[5, 7].Value.ToString().Trim();
                                }

                            }
                            catch (Exception Ex)
                            {_logger.LogInformation($"UploadARICSModel.UpdateRoadSectionARICSData: \r\n {Ex.Message}");}

                            //ARICS Road Length
                            double ARICSRoadLength = 0d;
                            try
                            {
                                if (worksheet.Cells[5, 12].Value != null)
                                {
                                    bool isConverted=double.TryParse(worksheet.Cells[5, 12].Value.ToString().Trim(), out ARICSRoadLength);
                                    _RoadSection.ARICSRoadLength = ARICSRoadLength;
                                }

                            }
                            catch (Exception Ex)
                            { _logger.LogInformation($"UploadARICSModel.UpdateRoadSectionARICSData: \r\n {Ex.Message}"); }

                            //ARICS Section ID
                            string ARICSSectionID = null;
                            try
                            {
                                if (worksheet.Cells[7, 2].Value != null)
                                {
                                    _RoadSection.ARICSSectionID = worksheet.Cells[7, 2].Value.ToString().Trim();
                                    
                                }

                            }
                            catch (Exception Ex)
                            { _logger.LogInformation($"UploadARICSModel.UpdateRoadSectionARICSData: \r\n {Ex.Message}"); }

                            //ARICS Section Name
                            string ARICSSectionName = null;
                            try
                            {
                                if (worksheet.Cells[7, 7].Value != null)
                                {
                                    _RoadSection.ARICSSectionName = worksheet.Cells[7, 7].Value.ToString().Trim();
                                }

                            }
                            catch (Exception Ex)
                            { _logger.LogInformation($"UploadARICSModel.UpdateRoadSectionARICSData: \r\n {Ex.Message}"); }

                            //ARICS Section Length
                            double ARICSSectionLength = 0d;
                            try
                            {
                                if (worksheet.Cells[7, 12].Value != null)
                                {
                                    bool isConverted = double.TryParse(worksheet.Cells[7, 12].Value.ToString().Trim(), out ARICSSectionLength);
                                    _RoadSection.ARICSSectionLength = ARICSSectionLength;
                                }

                            }
                            catch (Exception Ex)
                            { _logger.LogInformation($"UploadARICSModel.UpdateRoadSectionARICSData: \r\n {Ex.Message}"); }

                            //ARICS Sheet No
                            string ARICSSheetNo = null;
                            try
                            {
                                if (worksheet.Cells[9, 2].Value != null)
                                {
                                    _RoadSection.ARICSSheetNo = worksheet.Cells[9, 2].Value.ToString().Trim();
                                }

                            }
                            catch (Exception Ex)
                            { _logger.LogInformation($"UploadARICSModel.UpdateRoadSectionARICSData: \r\n {Ex.Message}"); }

                            //ARICS Sheet Length
                            int ARICSSheetLength = 0;
                            try
                            {
                                if (worksheet.Cells[9, 7].Value != null)
                                {
                                    bool isConverted = int.TryParse(worksheet.Cells[9, 7].Value.ToString().Trim(), out ARICSSheetLength);
                                    _RoadSection.ARICSSheetLength = ARICSSheetLength;

                                }

                            }
                            catch (Exception Ex)
                            { _logger.LogInformation($"UploadARICSModel.UpdateRoadSectionARICSData: \r\n {Ex.Message}"); }

                            //ARICS Sheet Interval
                            int ARICSSheetInterval = 0;
                            try
                            {
                                if (worksheet.Cells[9, 12].Value != null)
                                {
                                    bool isConverted = int.TryParse(worksheet.Cells[9, 12].Value.ToString().Trim(), out ARICSSheetInterval);
                                    _RoadSection.ARICSSheetInterval = ARICSSheetInterval;
                                }
                            }
                            catch (Exception Ex)
                            { _logger.LogInformation($"UploadARICSModel.UpdateRoadSectionARICSData: \r\n {Ex.Message}"); }
                        

                        }
                        catch (Exception Ex)
                        {
                            _logger.LogInformation($"UploadARICSModel.UpdateRoadSectionARICSData: \r\n {Ex.Message}");
                        }
                    }
                }

                //Update
                var resp = await _roadSectionService.UpdateAsync(_RoadSection.ID, _RoadSection).ConfigureAwait(false);
            }
        }
        #endregion

        #region Content Type
        private string GetContentType(string path)
        {
            try
            {
                var types = GetMimeTypes();
                var ext = Path.GetExtension(path).ToLowerInvariant();
                return types[ext];
            }
            catch (Exception Ex)
            {

                return null;
            }
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"}
            };
        }

        #endregion
    }
}