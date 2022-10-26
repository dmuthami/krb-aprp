using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Http.Extensions;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Authorization;

namespace APRP.Web.MyPages.RevenueCollectionRx
{
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class EditModel : PageModel
    {
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly ILogger _logger;
        private readonly IRevenueCollectionService _revenueCollectionService;
        private readonly IFinancialYearService _financialYearService;
        private readonly IRevenueCollectionCodeUnitService _revenueCollectionCodeUnitService;

        public EditModel( ILogger<EditModel> logger,
            IApplicationUsersService applicationUsersService, IAuthorityService authorityService,
            IRevenueCollectionService revenueCollectionService, IFinancialYearService financialYearService,
             IRevenueCollectionCodeUnitService revenueCollectionCodeUnitService)
        {
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _logger = logger;
            _revenueCollectionService = revenueCollectionService;
            _financialYearService = financialYearService;
            _revenueCollectionCodeUnitService = revenueCollectionCodeUnitService;
        }

        [BindProperty]
        public RevenueCollection RevenueCollection { get; set; }

        public Authority Authority { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RevenueCollection.View)]
        public async Task<IActionResult> OnGetAsync(long? id)
        {
            try
            {
                //Get logged in user
                ApplicationUser _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);
                Authority = _ApplicationUser.Authority;

                long myid = 0;
                bool result = long.TryParse(id.ToString(), out myid);

                if (id == null||result==false)
                {
                    return NotFound();
                }

                var _revenuecollectionResp = await _revenueCollectionService.FindByIdAsync(myid).ConfigureAwait(false);
                RevenueCollection = _revenuecollectionResp.RevenueCollection;
                
                if (RevenueCollection == null)
                {
                    return NotFound();
                }

                var revCollectioncodeUnitsResp = await _revenueCollectionCodeUnitService.ListAsync(Authority.ID).ConfigureAwait(false);
                ViewData["RevenueCollectionCodeUnitId"] = new SelectList((IList<RevenueCollectionCodeUnit>)revCollectioncodeUnitsResp.RevenueCollectionCodeUnit, "ID", "RevenueStream");


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


        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.RevenueCollection.Change)]
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var revenueCollectionResp = await _revenueCollectionService.Update(RevenueCollection.ID, RevenueCollection).ConfigureAwait(false);

                return RedirectToPage("./Index");
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"RevenueCollectionRx.Edit Page Error: {Ex.Message} " +
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
