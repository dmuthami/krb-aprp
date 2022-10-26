using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Authorization;

namespace APRP.Web.MyPages.ARICSheetz
{
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class DetailsModel : PageModel
    {
        private readonly IARICSService _aRICSService;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly ISurfaceTypeService _surfaceTypeService;
        public IConfiguration Configuration { get; }
        private readonly ILogger _logger;
        public DetailsModel(IARICSService aRICSService
            , ILogger<DetailsModel> logger
            , IConfiguration configuration,
            IApplicationUsersService applicationUsersService, ISurfaceTypeService surfaceTypeService)
        {
            _aRICSService = aRICSService;
            _logger = logger;
            Configuration = configuration;
            _applicationUsersService = applicationUsersService;
            _surfaceTypeService = surfaceTypeService;
        }

        public ARICS ARICS { get; set; }

        private ApplicationUser _ApplicationUser { get; set; }

        public SurfaceType SurfaceType { get; set; }

        public string SN { get; set; } = "UK";

        public string Referer { get; set; }

        [Authorize(Claims.Permission.ARICS.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> OnGetAsync(long id, long surfacetypeid)
        {
            try
            {
                //Get logged in user
                _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);

                //Set Return URL
                Referer = Request.Headers["Referer"].ToString();

                var aRICSResponse = await _aRICSService.GetARICSDetails(id).ConfigureAwait(false);
                ARICS = aRICSResponse.ARICS;

                //Get surface type
                await GetSurfaceType(surfacetypeid).ConfigureAwait(false);

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

                    //string username
                    string username = user.UserName;
                    SN = username.Substring(0, 2).ToUpper();

                }
            }
            return user;
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

        #endregion
    }
}
