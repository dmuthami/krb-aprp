using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace APRP.Web.MyPages.Roles
{
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class DetailsModel : PageModel
    {
        private readonly IApplicationRolesService _applicationRolesService;
        private readonly ILogger _logger;
        private readonly IApplicationUsersService _applicationUsersService;

        public DetailsModel(IApplicationRolesService applicationRolesService, ILogger<DetailsModel> logger,
            IApplicationUsersService applicationUsersService)
        {
            _applicationRolesService = applicationRolesService;
            _logger = logger;
            _applicationUsersService = applicationUsersService;
        }
        [BindProperty]
        public ApplicationRole ApplicationRole { get; set; }

        private IList<string> _MyRoles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.Administrator.Role_View)]
        public async Task<IActionResult> OnGet(string id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }
                var applicationRolesResponse = await _applicationRolesService.FindByIdAsync(id).ConfigureAwait(false);
                ApplicationRole = applicationRolesResponse.ApplicationRole;

                if (ApplicationRole == null)
                {
                    return NotFound();
                }
                //Set Return URL and store in session
                HttpContext.Session.SetString("Referer", Request.GetEncodedUrl());
                return Page();
            }
            catch (System.Exception Ex)
            {
                _logger.LogError(Ex, $"Roles.Detail: {Ex.Message} {Environment.NewLine}");
                ModelState.AddModelError(string.Empty, Ex.Message);
                return Page();
            }
        }
    }
}
