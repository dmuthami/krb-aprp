using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace APRP.Web.MyPages.WorkCategoryFundingMatrixRx
{
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class IndexModel : PageModel
    {
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly ILogger _logger;
        private readonly IWorkCategoryFundingMatrixService _workCategoryFundingMatrixService;


        public IndexModel( ILogger<IndexModel> logger,
            IApplicationUsersService applicationUsersService, IAuthorityService authorityService,
            IWorkCategoryFundingMatrixService workCategoryFundingMatrixService)
        {
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _logger = logger;
            _workCategoryFundingMatrixService = workCategoryFundingMatrixService;
        }

        public Authority Authority { get; set; }

        public IList<WorkCategoryFundingMatrix> WorkCategoryFundingMatrix { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.WorkCategoryFundingMatrix.View)]
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                //Get logged in user
                ApplicationUser _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);
                Authority = _ApplicationUser.Authority;

                var workCategoryFundingMatrixResp = await _workCategoryFundingMatrixService.ListAsync().ConfigureAwait(false);
                WorkCategoryFundingMatrix = (IList<WorkCategoryFundingMatrix>)workCategoryFundingMatrixResp.WorkCategoryFundingMatrix;

                //Set Return URL and store in session
                HttpContext.Session.SetString("Referer", Request.GetEncodedUrl());
                return Page();
            }
            catch (System.Exception Ex)
            {
                _logger.LogError(Ex, $"Roles.Index Page Error: {Ex.Message} " +
                    $"{Environment.NewLine}");
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


        #endregion
    }
}
