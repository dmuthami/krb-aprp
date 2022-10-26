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
    public class ARICSHomeModel : PageModel
    {
        private readonly IRoadSectionService _roadSectionService;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly IARICSYearService _aRICSYearService;
        public IConfiguration Configuration { get; }
        private readonly ILogger _logger;

        public ARICSHomeModel(IRoadSectionService roadSectionService,
            ILogger<ARICSHomeModel> logger
            , IConfiguration configuration, IApplicationUsersService applicationUsersService,
            IAuthorityService authorityService, IARICSYearService aRICSYearService)
        {
            _logger = logger;
            Configuration = configuration;
            _roadSectionService = roadSectionService;
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _aRICSYearService = aRICSYearService;
        }

        public string Referer { get; set; }

        public Authority Authority { get; set; }
        private IList<string> _MyRoles { get; set; }

        public IList<RoadSection> RoadSection { get; set; }

        public ARICSYear ARICSYear { get; set; }

        [Authorize(Claims.Permission.ARICS.View)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                //Get logged in user
                ApplicationUser user = await GetLoggedInUser().ConfigureAwait(false);
                Authority = user.Authority;

                if (Authority.Code.ToLower()=="krb")
                {
                    //redirect to KRB page
                    return RedirectToPage("/ARICZ/ARICSHomeKRB");
                }

                await PopulateDropDown().ConfigureAwait(false);

                //Set Return URL and store in session
                HttpContext.Session.SetString("Referer", Request.GetEncodedUrl());

                return Page();
            }
            catch (Exception Ex)
            {

                _logger.LogError($"ARICSHomeModel.OnGetAsync Error : {Ex.Message}");
                ModelState.AddModelError(string.Empty, Ex.Message);
                return Page();
            }
           
        }

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

        private async Task PopulateDropDown()
        {
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
        #endregion

    }
}