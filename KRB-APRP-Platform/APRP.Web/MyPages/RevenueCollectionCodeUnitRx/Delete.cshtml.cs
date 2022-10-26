using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace APRP.Web.MyPages.RevenueCollectionCodeUnitRx
{
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class DeleteModel : PageModel
    {
        private readonly APRP.Web.Persistence.Contexts.AppDbContext _context;
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly ILogger _logger;
        private readonly IRevenueCollectionCodeUnitService _revenueCollectionCodeUnitService;

        public DeleteModel(APRP.Web.Persistence.Contexts.AppDbContext context, ILogger<DeleteModel> logger,
            IApplicationUsersService applicationUsersService, IAuthorityService authorityService,
            IRevenueCollectionCodeUnitService revenueCollectionCodeUnitService)
        {
            _context = context;
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _logger = logger;
            _revenueCollectionCodeUnitService = revenueCollectionCodeUnitService;
        }

        [BindProperty]
        public RevenueCollectionCodeUnit RevenueCollectionCodeUnit { get; set; }

        public Authority Authority { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RevenueCollectionCodeUnit.View)]
        public async Task<IActionResult> OnGetAsync(long? id)
        {
            try
            {
                //Get logged in user
                ApplicationUser _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);
                Authority = _ApplicationUser.Authority;

                long myid;
                bool result = long.TryParse(id.ToString(), out myid);
                if (id == null || result == false)
                {
                    return NotFound();
                }
                var revenueCollectionCodeUnitResp = await _revenueCollectionCodeUnitService.FindByIdAsync(myid).ConfigureAwait(false);
                RevenueCollectionCodeUnit = revenueCollectionCodeUnitResp.RevenueCollectionCodeUnit;

                if (RevenueCollectionCodeUnit == null)
                {
                    return NotFound();
                }

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RevenueCollectionCodeUnit.Delete)]
        public async Task<IActionResult> OnPostAsync(long? id)
        {
            try
            {
                long myid;
                bool result = long.TryParse(id.ToString(), out myid);
                if (id == null || result == false)
                {
                    return NotFound();
                }

                var revenueCollectionCodeUnitResp = await _revenueCollectionCodeUnitService.RemoveAsync(myid).ConfigureAwait(false);

                return RedirectToPage("./Index");
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionCodeUnitRx.Delete Page Error: {Ex.Message} " +
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
