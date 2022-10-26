using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Http.Extensions;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Authorization;

namespace APRP.Web.MyPages.QuarterCodeUnitRx
{
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class EditModel : PageModel
    {
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly ILogger _logger;
        private readonly IQuarterCodeUnitService _quarterCodeUnitService;
        private readonly IFinancialYearService _financialYearService;
        private readonly IQuarterCodeListService _quarterCodeListService;

        public EditModel( ILogger<EditModel> logger,
            IApplicationUsersService applicationUsersService, IAuthorityService authorityService,
            IQuarterCodeUnitService quarterCodeUnitService, IFinancialYearService financialYearService,
             IQuarterCodeListService quarterCodeListService)
        {
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _logger = logger;
            _quarterCodeUnitService = quarterCodeUnitService;
            _financialYearService = financialYearService;
            _quarterCodeListService = quarterCodeListService;
        }

        [BindProperty]
        public QuarterCodeUnit QuarterCodeUnit { get; set; }

        public Authority Authority { get; set; }

        public long PreviousQuarterCodeListId { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.QuarterCodeUnit.View)]
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

                var _quarterCodeUnitResp = await _quarterCodeUnitService.FindByIdAsync(myid).ConfigureAwait(false);
                QuarterCodeUnit = _quarterCodeUnitResp.QuarterCodeUnit;
                
                if (QuarterCodeUnit == null)
                {
                    return NotFound();
                }

                //Populate dropdowns
                await PopulateDropDown(QuarterCodeUnit.FinancialYearId).ConfigureAwait(false);

                PreviousQuarterCodeListId = QuarterCodeUnit.QuarterCodeListId;

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
        [Authorize(Claims.Permission.QuarterCodeUnit.Change)]
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var quarterCodeUnitResp = await _quarterCodeUnitService.Update(QuarterCodeUnit.ID, QuarterCodeUnit).ConfigureAwait(false);

                return RedirectToPage("./Index");
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"QuarterCodeUnitRx.Edit Page Error: {Ex.Message} " +
                    $"{Environment.NewLine}");
                string msg = $"Quarter Code Unit Error:{Ex.Message.ToString()}";
                ModelState.AddModelError(string.Empty, msg);
                //Populate dropdowns
                await PopulateDropDown(0).ConfigureAwait(false);
                return Page();
            }
        }

        #region Utilities
        private async Task PopulateDropDown(long FinancialYearId)
        {
            //Set Financial Year
            var finacialResp = await _financialYearService.ListAsync().ConfigureAwait(false);
            IList<FinancialYear> financialYears = (IList<FinancialYear>)finacialResp;
            var newFinancialYearsList = financialYears
                .OrderByDescending(v => v.Code)
                .Select(
                p => new
                {
                    ID = p.ID,
                    Code = $"{p.Code}_{p.Revision}"
                }
                    ).ToList();
            if (FinancialYearId == 0)
            {
                ViewData["FinancialYearId"] = new SelectList(newFinancialYearsList, "ID", "Code");
            }
            else
            {
                ViewData["FinancialYearId"] = new SelectList(newFinancialYearsList, "ID", "Code", FinancialYearId);
            }

            var quarterCodeListResp = await _quarterCodeListService.ListAsync().ConfigureAwait(false);
            ViewData["QuarterCodeListId"] = new SelectList((IList<QuarterCodeList>)quarterCodeListResp.QuarterCodeList, "ID", "Name");

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
