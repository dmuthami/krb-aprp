using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace APRP.Web.MyPages.ARICZ
{
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class ConductARICSModel : PageModel
    {
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly ILogger _logger;
        private readonly IRoadSheetLengthService _roadSheetLengthService;
        private readonly IRoadSheetIntervalService _roadSheetIntervalService;
        private readonly IARICSYearService _aRICSYearService;

        public ConductARICSModel(ILogger<ConductARICSModel> logger, IApplicationUsersService applicationUsersService,
             IAuthorityService authorityService, 
             IRoadSheetLengthService roadSheetLengthService,
            IRoadSheetIntervalService roadSheetIntervalService,
            IARICSYearService aRICSYearService)
        {
            _applicationUsersService = applicationUsersService;
            _logger = logger;
            _authorityService = authorityService;
            _roadSheetLengthService = roadSheetLengthService;
            _roadSheetIntervalService = roadSheetIntervalService;
            _aRICSYearService = aRICSYearService;
        }
        public County County { get; set; }

        public ARICSYear ARICSYear { get; set; }

        public Constituency Constituency { get; set; }

        public RoadSection RoadSection { get; set; }

        public string Referer { get; set; }

        private IList<string> _MyRoles { get; set; }
        public Authority Authority { get; set; }

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
                //Get logged in user
                ApplicationUser user = await GetLoggedInUser().ConfigureAwait(false);
                Authority = user.Authority;

                //Populate Dropdowns
                await PopulateDropdowns().ConfigureAwait(false);

                Referer = Request.Headers["Referer"].ToString();

                //Set Return URL and store in session
                HttpContext.Session.SetString("Referer", Request.GetEncodedUrl());
                return Page();
            }
            catch (Exception Ex)
            {
                _logger.LogError($" Razor Page Error{Ex.Message}");
                ModelState.AddModelError(string.Empty, Ex.Message);
                //Populate Dropdowns
                await PopulateDropdowns().ConfigureAwait(false);
                return Page();
            }
        }

        public async Task<IActionResult> OnPostSwitchToUploadARICSAsync()
        {
            return RedirectToPage("/ARICZ/UploadARICS");
        }
        #region Utilities
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task PopulateDropdowns()
        {
            try
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
            catch (Exception Ex)
            {
                _logger.LogError($" Razor Page Error{Ex.Message}");
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