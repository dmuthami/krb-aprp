using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using APRP.Web.Domain.Services;
using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Authorization;

namespace APRP.Web.MyPages.ARICSheetz
{
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class EditModel : PageModel
    {
        private readonly IRoadSheetService _roadSheetService;
        private readonly IARICSService _aRICSService;
        private readonly IShoulderSurfaceTypePavedService _shoulderSurfaceTypePavedService;
        private readonly IShoulderInterventionPavedService _shoulderInterventionPavedService;
        private readonly ISurfaceTypeUnPavedService _surfaceTypeUnPavedService;
        private readonly IAuthorityService _authorityService;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly ISurfaceTypeService _surfaceTypeService;
        private readonly IGravelRequiredService _gravelRequiredService;
        public IConfiguration Configuration { get; }
        private readonly ILogger _logger;


        public EditModel(IARICSService aRICSService,
            IRoadSheetService roadSheetService,
            IShoulderSurfaceTypePavedService shoulderSurfaceTypePavedService,
            IShoulderInterventionPavedService shoulderInterventionPavedService,
            ISurfaceTypeUnPavedService surfaceTypeUnPavedService
            , ILogger<EditModel> logger
            , IConfiguration configuration,
            IAuthorityService authorityService, IApplicationUsersService applicationUsersService,
            ISurfaceTypeService surfaceTypeService, IGravelRequiredService gravelRequiredService)
        {
   
            _logger = logger;
            Configuration = configuration;
            _roadSheetService = roadSheetService;
            _aRICSService = aRICSService;            
            _shoulderSurfaceTypePavedService = shoulderSurfaceTypePavedService;
            _shoulderInterventionPavedService = shoulderInterventionPavedService;
            _surfaceTypeUnPavedService = surfaceTypeUnPavedService;
            _authorityService = authorityService;
            _applicationUsersService = applicationUsersService;
            _surfaceTypeService = surfaceTypeService;
            _gravelRequiredService = gravelRequiredService;
        }

        [BindProperty]
        public ARICS ARICS { get; set; }

        [BindProperty]
        public string Referer { get; set; }

        [BindProperty]
        public SurfaceType SurfaceType { get; set; }

        public Authority Authority { get; set; }

        private IList<string> _MyRoles { get; set; }

        [Authorize(Claims.Permission.ARICS.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> OnGetAsync(long id,long surfacetypeid)
        {
            try
            {
                //Set Return URL
                Referer = Request.Headers["Referer"].ToString();
                var aRICSResponse = await _aRICSService.FindByIdAsync(id).ConfigureAwait(false);
                ARICS = aRICSResponse.ARICS;

                if (ARICS == null)
                {
                    return NotFound();
                }

                //Get surface type
                await GetSurfaceType(surfacetypeid).ConfigureAwait(false);

                //populate drop downs
                await PopulateDropDown(ARICS).ConfigureAwait(false);

                return Page();
            }
            catch (System.Exception Ex)
            {
                string str = Ex.Message;
                _logger.LogError($"Razor Page Error : {str}");
                ModelState.AddModelError(string.Empty, Ex.Message);
                return Page();
            }
        }

        [Authorize(Claims.Permission.ARICS.Change)]
        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}
            long myID = ARICS.ID;
            var aRICSResponse = await _aRICSService.Update(myID,ARICS).ConfigureAwait(false);
            if (aRICSResponse.Success==true)
            {
                return Redirect(Referer);
            }
            else
            {
                string msg = "Failed to update ARICS";
                _logger.LogError($"ARICSheetz.EditModel Error{msg}");
                //Get surface type
                await GetSurfaceType(SurfaceType.ID).ConfigureAwait(false);

                //populate drop downs
                await PopulateDropDown(ARICS).ConfigureAwait(false);
                ModelState.AddModelError(string.Empty, msg);
                return Page();
            }
        }

        #region Utilities

        private async Task GetSurfaceType(long SurfaceTypeID)
        {
            var resp = await _surfaceTypeService.FindByIdAsync(SurfaceTypeID).ConfigureAwait(false);
            if (resp.Success)
            {
                SurfaceType = resp.SurfaceType;
            }
            else
            {
                SurfaceType = null;
            }
        }
        private async Task PopulateDropDown(ARICS aRICS)
        {
            //Get a list for the Survey Type
            var surveyTypeUnPavedListResponse = await _surfaceTypeUnPavedService.ListAsync().ConfigureAwait(false);
            IList<SurfaceTypeUnPaved> SurfaceTypeUnPavedList = (IList<SurfaceTypeUnPaved>)surveyTypeUnPavedListResponse.SurfaceTypeUnPaved;
            ViewData["SurfaceTypeUnPaved_ID"] = new SelectList(SurfaceTypeUnPavedList, "ID", "Name", aRICS.SurfaceTypeUnPavedId);

            //Get list for ShoulderSufaceType
            IList<ShoulderSurfaceTypePaved> ShoulderSurfaceTypePaved = null;
            var shoulderSurfaceTypePavedListResponse = await _shoulderSurfaceTypePavedService.ListAsync().ConfigureAwait(false);
            ShoulderSurfaceTypePaved = (IList<ShoulderSurfaceTypePaved>)shoulderSurfaceTypePavedListResponse.ShoulderSurfaceTypePaved;
            ViewData["ShoulderSurfaceTypePaved_ID"] = new SelectList(ShoulderSurfaceTypePaved, "ID", "Name", aRICS.ShoulderSurfaceTypePaved);

            //Get list for ShoulderInterventionPaved
            IList<ShoulderInterventionPaved> ShoulderInterventionPaved = null;
            var shoulderInterventionPavedListResponse = await _shoulderInterventionPavedService.ListAsync().ConfigureAwait(false);
            ShoulderInterventionPaved = (IList<ShoulderInterventionPaved>)shoulderInterventionPavedListResponse.ShoulderInterventionPaved;
            ViewData["ShoulderInterventionPaved_ID"] = new SelectList(ShoulderInterventionPaved, "ID", "Name", aRICS.ShoulderInterventionPavedId);

            //_GravelRequiredUnPaved
            IList<GravelRequired> gravelRequireds = null;
            var gravelRequiredResp = await _gravelRequiredService.ListAsync().ConfigureAwait(false);
            if (gravelRequiredResp.Success)
            {
                var objectResult = (ObjectResult)gravelRequiredResp.IActionResult;
                if (objectResult != null)
                {
                    if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var result = (OkObjectResult)objectResult;
                        gravelRequireds = (IList<GravelRequired>)result.Value;
                    }
                }
            }

            ViewData["GravelRequired_ID"] = new SelectList(gravelRequireds, "ID", "Code", aRICS.GravelRequiredId);

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
                }
            }
            return user;
        }

        private async Task<bool> IsApproved(ApplicationUser applicationUser)
        {
            bool _isApproved = false;

            var appUserResp = await _applicationUsersService.GetRolesAsync(applicationUser).ConfigureAwait(false);

            if (appUserResp.Success)
            {
                var objectResult = (ObjectResult)appUserResp.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (OkObjectResult)objectResult;
                    _MyRoles = (IList<string>)result2.Value;

                    if (_MyRoles.Contains("Administrators") || _MyRoles.Contains("ARICS.ConductARICS"))
                    {
                        _isApproved = true;
                    }
                }
            }

            //Check if in Administrator role
            return _isApproved;
        }

        #endregion
    }
}
