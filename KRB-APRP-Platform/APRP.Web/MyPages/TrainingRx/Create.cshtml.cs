using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Http.Extensions;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Authorization;

namespace APRP.Web.MyPages.TrainingRx
{
    [Authorize]
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class CreateModel : PageModel
    {
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly ILogger _logger;
        private readonly ITrainingService _trainingService;
        private readonly ITrainingCourseService _trainingCourseService;
        private readonly IFinancialYearService _financialYearService;
        private readonly IQuarterCodeListService _quarterCodeListService;
        public CreateModel(ILogger<CreateModel> logger,
            IApplicationUsersService applicationUsersService, IAuthorityService authorityService,
            ITrainingService trainingService,
            IFinancialYearService financialYearService,
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

        public Authority Authority { get; set; }
        public FinancialYear FinancialYear { get; set; }

        public TrainingCourse TrainingCourse { get; set; }

        public string Referer { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.Training.View)]
        public async Task<IActionResult> OnGet()
        {
            try
            {
                //Get logged in user
                ApplicationUser _ApplicationUser = await GetLoggedInUser().ConfigureAwait(false);
                Authority = _ApplicationUser.Authority;

                //Set Previous Return URL
                Referer = Request.Headers["Referer"].ToString();

                var finacialResp = await _financialYearService.ListAsync().ConfigureAwait(false);
                IList<FinancialYear> financialYears = (IList<FinancialYear>)finacialResp;
                ViewData["FinancialYearId"] = new SelectList(financialYears, "ID", "Code");

                //Training Course Title
                var trainingCourseResp = await _trainingCourseService.ListAsync().ConfigureAwait(false);
                IList<TrainingCourse> trainingCourses = (IList<TrainingCourse>)trainingCourseResp.TrainingCourse;
                ViewData["TrainingCourseId"] = new SelectList(trainingCourses, "ID", "Course");

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

        [BindProperty]
        public Training Training { get; set; }


        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.Training.Add)]
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                ApplicationUser loggedInuser = await GetLoggedInUser().ConfigureAwait(false);

                if (!ModelState.IsValid)
                {
                    Authority = loggedInuser.Authority;

                    //Set Previous Return URL
                    Referer = Request.Headers["Referer"].ToString();

                    var finacialResp = await _financialYearService.ListAsync().ConfigureAwait(false);
                    IList<FinancialYear> financialYears = (IList<FinancialYear>)finacialResp;
                    ViewData["FinancialYearId"] = new SelectList(financialYears, "ID", "Code");

                    //Training Course Title
                    var trainingCourseResp = await _trainingCourseService.ListAsync().ConfigureAwait(false);
                    IList<TrainingCourse> trainingCourses = (IList<TrainingCourse>)trainingCourseResp.TrainingCourse;
                    ViewData["TrainingCourseId"] = new SelectList(trainingCourses, "ID", "Course");

                    //Set Return URL and store in session
                    HttpContext.Session.SetString("Referer", Request.GetEncodedUrl());
                    return Page();
                }

                Training.AuthorityId = loggedInuser.Authority.ID;
                var trainingResp = await _trainingService.AddAsync(Training).ConfigureAwait(false);
               
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
                _logger.LogError(Ex, $"TrainingRx.Add Page Error: {Ex.Message} " +
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
