using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace APRP.Web.MyPages.WorkCategoryFundingMatrixRx
{
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class EditModel : PageModel
    {
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly ILogger _logger;
        private readonly IWorkCategoryFundingMatrixService _workCategoryFundingMatrixService;
        private readonly IFinancialYearService _financialYearService;

        public EditModel(ILogger<EditModel> logger,
            IApplicationUsersService applicationUsersService, IAuthorityService authorityService,
            IWorkCategoryFundingMatrixService workCategoryFundingMatrixService,
            IFinancialYearService financialYearService)
        {
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _logger = logger;
            _workCategoryFundingMatrixService = workCategoryFundingMatrixService;
            _financialYearService = financialYearService;
        }

        [BindProperty]
        public WorkCategoryFundingMatrix WorkCategoryFundingMatrix { get; set; }

        public Authority Authority { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.WorkCategoryFundingMatrix.View)]
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
                var workCategoryFundingMatrixResp = await _workCategoryFundingMatrixService.FindByIdAsync(myid).ConfigureAwait(false);
                WorkCategoryFundingMatrix = workCategoryFundingMatrixResp.WorkCategoryFundingMatrix;

                if (WorkCategoryFundingMatrix == null)
                {
                    return NotFound();
                }

                //Set Financial Year
                var finacialResp = await _financialYearService.ListAsync().ConfigureAwait(false);
                IList<FinancialYear> financialYears = (IList<FinancialYear>)finacialResp;
                ViewData["FinancialYearId"] = new SelectList(financialYears, "ID", "Code");

                //Set Authority Year
                var authorityResp = await _authorityService.ListAsync().ConfigureAwait(false);
                IList<Authority> authoritys = (IList<Authority>)authorityResp;
                ViewData["AuthorityId"] = new SelectList(authoritys, "ID", "Name");

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
        [Authorize(Claims.Permission.WorkCategoryFundingMatrix.Change)]
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var workCategoryFundingMatrixResp = await _workCategoryFundingMatrixService.Update(WorkCategoryFundingMatrix.ID, WorkCategoryFundingMatrix).ConfigureAwait(false);

                return RedirectToPage("./Index");
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"WorkCategoryFundingMatrixRx.Edit Page Error: {Ex.Message} " +
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
