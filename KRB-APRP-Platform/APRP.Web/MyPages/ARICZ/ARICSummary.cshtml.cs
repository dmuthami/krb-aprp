using APRP.Web.Domain.Models;
using APRP.Web.Domain.Models.History;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace APRP.Web.MyPages.ARICZ
{
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class ARICSummaryModel : PageModel
    {
        private readonly IRoadSheetService _roadSheetService;
        private readonly IRoadSectionService _roadSectionService;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly IGISRoadService _gISRoadService;
        private readonly IARICSYearService _aRICSYearService;
        private readonly IARICSApprovalService _aRICSApprovalService;


        public IConfiguration Configuration { get; }
        private readonly ILogger _logger;

        public ARICSummaryModel(IRoadSheetService roadSheetService,
            IRoadSectionService roadSectionService,
            ILogger<ARICSHomeModel> logger
            , IConfiguration configuration, IApplicationUsersService applicationUsersService, IAuthorityService authorityService,
            IGISRoadService gISRoadService, IARICSYearService aRICSYearService, IARICSApprovalService aRICSApprovalService)
        {
            _roadSectionService = roadSectionService;
            _roadSheetService = roadSheetService;
            _logger = logger;
            Configuration = configuration;
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _gISRoadService = gISRoadService;
            _aRICSYearService = aRICSYearService;
            _aRICSApprovalService = aRICSApprovalService;
        }

        public IList<RoadSheet> RoadSheet { get; set; }

        public RoadSection RoadSection { get; set; }

        public string Referer { get; set; }

        public string SurfaceType { get; set; }

        public Authority Authority { get; set; }

        public int _Year { get; set; }

        public ARICSYear ARICSYear { get; set; }

        public ARICSApproval ARICSApproval { get; set; }

        public IList<ARICSApprovalh> ARICSApprovalh { get; set; }

        public int NextStatus { get; set; }

        public int ResetStatus { get; set; }

        [Authorize(Claims.Permission.ARICS.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> OnGetAsync(long RoadSectionId, int? Year)
        {
            try
            {
                //Get logged in user
                ApplicationUser user = await GetLoggedInUser().ConfigureAwait(false);
                Authority = user.Authority;

                //Specify Year
                int _Yearr = DateTime.UtcNow.Year;
                bool result = int.TryParse(Year.ToString(), out _Yearr);
                if (result == false)
                {
                    _Year = DateTime.UtcNow.Year;
                }
                else
                {
                    _Year = _Yearr;
                }

                //Populate drop down
                await PopulateDropDown(_Year).ConfigureAwait(false);

                //Get Road Section
                var roadSectionResp = await _roadSectionService.FindByIdAsync(RoadSectionId).ConfigureAwait(false);
                if (roadSectionResp.Success)
                {
                    RoadSection = roadSectionResp.RoadSection;
                }

                var roadSheetListResponse = await _roadSheetService.ListByRoadSectionIdAsync(RoadSectionId, _Year).ConfigureAwait(false);
                RoadSheet = (IList<RoadSheet>)roadSheetListResponse.RoadSheets;

                //Set Surface Type
                var gisRoadResp = await _gISRoadService.GetSurfaceType(RoadSection).ConfigureAwait(false);
                SurfaceType = gisRoadResp.GISRoadViewModel.SurfaceType;

                //Get Details About ARICS Approval
                //var resp = await _aRICSApprovalService.FindByARICSMasterApprovalIdAsync(
                //    RoadSection.ID, _Year).ConfigureAwait(false);
                //if (resp.Success)
                //{
                //    var objectResult = (ObjectResult)resp.IActionResult;
                //    if (objectResult != null)
                //    {
                //        if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                //        {
                //            var result2 = (OkObjectResult)objectResult;
                //            ARICSApproval = (ARICSApproval)result2.Value;
                //        }
                //    }
                //}
                //else
                //{
                //    ARICSApproval = null;
                //}

                //Set nextStatus and PreviousStatus
                await SetStatusParameters().ConfigureAwait(false);

                //get history
                if (ARICSApproval != null)
                {
                    var respHist = await _aRICSApprovalService.ListHistoryAsync(
                    ARICSApproval.ID).ConfigureAwait(false);
                    if (respHist.Success)
                    {
                        var objectResult = (ObjectResult)respHist.IActionResult;
                        if (objectResult != null)
                        {
                            if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var result2 = (OkObjectResult)objectResult;
                                ARICSApprovalh = (IList<ARICSApprovalh>)result2.Value;
                            }
                        }
                    }
                }

                //Set Return URL and store in session
                string url;
                if (HttpContext.Session.GetString("RefererToARICSSummary") == null)
                {
                    url = Request.Headers["Referer"].ToString(); /*Request.GetEncodedUrl();*/
                    HttpContext.Session.SetString("RefererToARICSSummary", url);
                }
                Referer = HttpContext.Session.GetString("RefererToARICSSummary");

                return Page();
            }
            catch (Exception Ex)
            {
                _logger.LogError($" Razor Page Error{Ex.Message}");
                ModelState.AddModelError(string.Empty, Ex.Message);
                return Page();
            }
        }

        #region Utilities
        private async Task PopulateDropDown(int _Year)
        {
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
                        var result = (OkObjectResult)objectResult;
                        aRICSYears = (IList<ARICSYear>)result.Value;
                    }
                }
            }
            ViewData["ARICSYearId"] = new SelectList(aRICSYears, "Year", "Year", _Year);
        }

        private async Task SetStatusParameters()
        {
            //await Task.Run(() =>
            //{

            //}).ConfigureAwait(false);

            //if ARICSApproval is null
            if (ARICSApproval == null)
            {
                //if CG
                if (Authority.Type == 2)
                {
                    NextStatus = 1;
                    ResetStatus = 0;
                }

                //if RA
                if (Authority.Type == 1)
                {
                    NextStatus = 8;
                    ResetStatus = 0;
                }
            }
            else
            {
                //if CG
                if (Authority.Type == 2)
                {
                    ResetStatus = ARICSApproval.ARICSApprovalLevel.Status - 1;
                    NextStatus = ARICSApproval.ARICSApprovalLevel.Status + 1;

                }
                //if RA
                if (Authority.Type == 1)
                {
                    ResetStatus = ARICSApproval.ARICSApprovalLevel.Status - 1;
                    if (ARICSApproval.ARICSApprovalLevel.Status == 10)
                    {
                        NextStatus = 5;//Go to KRB
                    }
                    else
                    {
                        NextStatus = ARICSApproval.ARICSApprovalLevel.Status + 1;
                    }

                }

                //if KRB
                if (Authority.Type == 0)
                {
                    ResetStatus = ARICSApproval.ARICSApprovalLevel.Status - 1;
                    if (ARICSApproval.ARICSApprovalLevel.Status == 7)
                    {
                        NextStatus = 7;//End. Remain there
                    }
                    else
                    {
                        NextStatus = ARICSApproval.ARICSApprovalLevel.Status + 1;
                    }
                }
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