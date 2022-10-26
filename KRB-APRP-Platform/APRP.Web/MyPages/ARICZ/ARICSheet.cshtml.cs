using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using APRP.Web.Extensions.Filters;
using APRP.Web.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;

namespace APRP.Web.MyPages.ARICZ
{
    [TypeFilter(typeof(SessionTimeoutRazorFilter))]
    public class ARICSheetModel : PageModel
    {
        public IConfiguration Configuration { get; }
        private readonly ILogger _logger;
        private readonly IARICSService _aRICSService;
        private readonly IRoadSheetService _roadSheetService;
        private readonly IConstituencyService _constituencyService;
        private readonly IRoadService _roadService;
        private readonly IRoadSectionService _roadSectionService;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly ISurfaceTypeService _surfaceTypeService;

        private CultureInfo _cultures = new CultureInfo("en-US");

        public ARICSheetModel(ILogger<ARICSheetModel> logger
            , IConfiguration configuration,
            IARICSService aRICSService,
            IRoadSheetService roadSheetService,
            IConstituencyService constituencyService,
            IRoadService roadService,
            IRoadSectionService roadSectionService,
            IApplicationUsersService applicationUsersService, IAuthorityService authorityService
            , ISurfaceTypeService surfaceTypeService)
        {
            _logger = logger;
            Configuration = configuration;
            _aRICSService = aRICSService;
            _roadSheetService = roadSheetService;
            _constituencyService = constituencyService;
            _roadService = roadService;
            _roadSectionService = roadSectionService;
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _surfaceTypeService = surfaceTypeService;

        }
        public IList<ARICS> ARICS { get; set; }

        public RoadSheet RoadSheet { get; set; }

        public SurfaceType SurfaceType { get; set; }

        public Constituency Constituency { get; set; }

        public Road Road { get; set; }

        public string Referer { get; set; }
        public Authority Authority { get; set; }

        [Authorize(Claims.Permission.ARICS.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> OnGetAsync(long SheetId, string IntervalInMeters)
        {
            try
            {
                //Get logged in user
                ApplicationUser _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);
                Authority = _ApplicationUser.Authority;

                //Set Return URL
                Referer = Request.Headers["Referer"].ToString();

                ARICSVM _ARICSVM = new ARICSVM();
                _ARICSVM.RoadSheetID = SheetId;

                //Get the ARICS
                var aRICSListResponse = await _aRICSService.GetARICSBySheetNo(_ARICSVM).ConfigureAwait(false);
                ARICS = (IList<ARICS>)aRICSListResponse.ARICS;

                //Get specific sheet details plus road section details
                var roadSheetResponse = await _roadSheetService.FindByIdAsync(SheetId).ConfigureAwait(false);
                RoadSheet = roadSheetResponse.RoadSheet;
                RoadSheet.RoadSection.Length=Math.Round(RoadSheet.RoadSection.Length, 3, MidpointRounding.AwayFromZero);

                //Get specific constituency details plus county details
                long constID = 0;
                bool result = long.TryParse(RoadSheet.RoadSection.ConstituencyId.ToString(), out constID);
                var constituencyResponse = await _constituencyService.GetConstituencyAndCounty(constID).ConfigureAwait(false);
                Constituency = constituencyResponse.Constituency;


                //Get Specific Road Details
                long roadID = 0;
                result = long.TryParse(RoadSheet.RoadSection.RoadId.ToString(_cultures), out roadID);
                var roadResponse = await _roadService.FindByIdAsync(roadID).ConfigureAwait(false);
                Road = roadResponse.Road;

                //Get surface type
                await GetSurfaceType().ConfigureAwait(false);

                //Pull section length from GIS
                //await GetRoadSectionLength();

                //Populate arics if not existent
                int _Interval = 0;
                if (IntervalInMeters == null)
                {
                    result = int.TryParse(RoadSheet.RoadSection.Interval.ToString(_cultures), out _Interval);
                } else
                {
                    result = int.TryParse(IntervalInMeters, out _Interval);
                }

                _ARICSVM = new ARICSVM();
                _ARICSVM.Interval = _Interval;
                _ARICSVM.SectionLengthKM =RoadSheet.EndChainage - RoadSheet.StartChainage;
                //_ARICSVM.SectionLengthKM = Math.Ceiling(RoadSheet.EndChainage - RoadSheet.StartChainage);
                _ARICSVM.RoadSheetID = SheetId;
                ARICS = await PopulateARICS(_ARICSVM).ConfigureAwait(false);

                try
                {
                    //Compute statitics for the arics data
                    var _aricsDataResponse = await _aRICSService.GetIRI(ARICS).ConfigureAwait(false);
                    ViewData["Average_Rate_Of_Deteroriation"] = _aricsDataResponse.ARICSData.RateOfDeterioration;
                }
                catch (Exception Ex)
                {

                    _logger.LogError(Ex, $"ARICSheetModel.OnGetAsync() Error{Environment.NewLine}");
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

                _logger.LogError(Ex, $"ARICSheetModel.OnGetAsync() Error{Environment.NewLine}");
                ModelState.AddModelError(string.Empty, Ex.Message);
                return Page();
            }
        }

        private async Task<IList<ARICS>> PopulateARICS(ARICSVM _ARICSVM)
        {
            IList<ARICS> ARICS = null;


            ARICS myARICS = null;
            var aRICSResponse = await _aRICSService.CheckARICSForSheet(_ARICSVM).ConfigureAwait(false);
            myARICS = aRICSResponse.ARICS;


            if (myARICS == null)
            {
                var aRICSListResponse = await _aRICSService.CreateARICSForSheet(_ARICSVM).ConfigureAwait(false);
                ARICS = (IList<ARICS>)aRICSListResponse.ARICS;
            }
            else
            {
                var aRICSListResponse = await _aRICSService.GetARICSForSheet(_ARICSVM).ConfigureAwait(false);
                ARICS = (IList<ARICS>)aRICSListResponse.ARICS;
            }



            return ARICS;
        }

        #region Utilities

        private async Task GetSurfaceType()
        {
            var resp = await _surfaceTypeService.FindByIdAsync(RoadSheet.RoadSection.SurfaceTypeId).ConfigureAwait(false);
            if (resp.Success)
            {
                SurfaceType = resp.SurfaceType;
            }else
            {
                SurfaceType = null;
            }
        }

        private void ComputeStatistics()
        {
            if (ARICS != null)
            {
                double iri_1 = 0.0;
                double iri_2 = 0.0;
                double iri_3 = 0.0;
                double iri_4 = 0.0;
                double iri_5 = 0.0;
                iri_1 = ARICS.Count(s => s.RateOfDeterioration == 1) * 0.2;
                iri_2 = ARICS.Count(s => s.RateOfDeterioration == 2) * 0.2;
                iri_3 = ARICS.Count(s => s.RateOfDeterioration == 3) * 0.2;
                iri_4 = ARICS.Count(s => s.RateOfDeterioration == 4) * 0.2;
                iri_5 = ARICS.Count(s => s.RateOfDeterioration == 5) * 0.2;

                double iri = (iri_1 + iri_2 + iri_3 + iri_4 + iri_5) * 5;

                ViewData["Average_Rate_Of_Deteroriation"] = iri;
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