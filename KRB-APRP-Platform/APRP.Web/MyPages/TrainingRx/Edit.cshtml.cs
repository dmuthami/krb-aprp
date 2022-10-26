using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Http.Extensions;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Authorization;

namespace APRP.Web.MyPages.TrainingRx
{
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class EditModel : PageModel
    {
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly ILogger _logger;
        private readonly ITrainingService _trainingService;
        private readonly ITrainingCourseService _trainingCourseService;
        private readonly IFinancialYearService _financialYearService;
        private readonly IQuarterCodeListService _quarterCodeListService;

        public EditModel( ILogger<EditModel> logger,
            IApplicationUsersService applicationUsersService, IAuthorityService authorityService,
            ITrainingService trainingService, IFinancialYearService financialYearService,
             IQuarterCodeListService quarterCodeListService,
             ITrainingCourseService trainingCourseService)
        {
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
            _logger = logger;
            _trainingService = trainingService;
            _financialYearService = financialYearService;
            _quarterCodeListService = quarterCodeListService;
            _trainingCourseService = trainingCourseService;
        }

        [BindProperty]
        public Training Training { get; set; }

        public string Referer { get; set; }

        public Authority Authority { get; set; }

        public FinancialYear FinancialYear { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.Training.View)]
        public async Task<IActionResult> OnGetAsync(long? id)
        {
            try
            {
                //Get logged in user
                ApplicationUser _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);
                Authority = _ApplicationUser.Authority;

                //Set Previous Return URL
                Referer = Request.Headers["Referer"].ToString();

                long myid = 0;
                bool result = long.TryParse(id.ToString(), out myid);

                if (id == null||result==false)
                {
                    return NotFound();
                }

                var _trainingResp = await _trainingService.FindByIdAsync(myid).ConfigureAwait(false);
                Training = _trainingResp.Training;
                
                if (Training == null)
                {
                    return NotFound();
                }
                var finacialResp = await _financialYearService.ListAsync().ConfigureAwait(false);
                IList<FinancialYear> financialYears = (IList<FinancialYear>)finacialResp;
                ViewData["FinancialYearId"] = new SelectList(financialYears, "ID", "Code");


                var trainingResp = await _quarterCodeListService.ListAsync().ConfigureAwait(false);
                ViewData["QuarterCodeListId"] = new SelectList((IList<QuarterCodeList>)trainingResp.QuarterCodeList, "ID", "Name");

                //Training Course Title
                var trainingCourseResp = await _trainingCourseService.ListAsync().ConfigureAwait(false);
                IList<TrainingCourse> trainingCourses = (IList<TrainingCourse>)trainingCourseResp.TrainingCourse;
                ViewData["TrainingCourseId"] = new SelectList(trainingCourses, "ID", "Course",Training.TrainingCourseId);

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
        [Authorize(Claims.Permission.Training.Change)]
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                ApplicationUser loggedInuser = await GetLoggedInUser().ConfigureAwait(false);

                if (!ModelState.IsValid)
                {
                    //Set Previous Return URL
                    Referer = Request.Headers["Referer"].ToString();

                    Authority = loggedInuser.Authority;

                    var finacialResp = await _financialYearService.ListAsync().ConfigureAwait(false);
                    IList<FinancialYear> financialYears = (IList<FinancialYear>)finacialResp;
                    ViewData["FinancialYearId"] = new SelectList(financialYears, "ID", "Code");

                    var trainingListResp = await _quarterCodeListService.ListAsync().ConfigureAwait(false);
                    ViewData["QuarterCodeListId"] = new SelectList((IList<QuarterCodeList>)trainingListResp.QuarterCodeList, "ID", "Name",Training.QuarterCodeUnitId);

                    //Training Course Title
                    var trainingCourseResp = await _trainingCourseService.ListAsync().ConfigureAwait(false);
                    IList<TrainingCourse> trainingCourses = (IList<TrainingCourse>)trainingCourseResp.TrainingCourse;
                    ViewData["TrainingCourseId"] = new SelectList(trainingCourses, "ID", "Course", Training.TrainingCourseId);

                    //Set Return URL and store in session
                    HttpContext.Session.SetString("Referer", Request.GetEncodedUrl());
                    return Page();
                }


                Training.AuthorityId = loggedInuser.Authority.ID;

                var trainingResp = await _trainingService.Update(Training.ID, Training).ConfigureAwait(false);

                if (Referer != null)
                {
                    return Redirect(Referer);
                }
                else
                {
                    return RedirectToPage("./Index");
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, $"TrainingRx.Edit Page Error: {Ex.Message} " +
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
                //User is found
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
