using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace APRP.Web.MyPages.Roles
{
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class IndexModel : PageModel
    {
        private readonly IApplicationRolesService _applicationRolesService;
        private readonly ILogger _logger;
        private readonly IApplicationUsersService _applicationUsersService;

#pragma warning disable CA2227 // Collection properties should be read only
        public IList<ApplicationRole> ApplicationRole { get; set; }
        #pragma warning restore CA2227 // Collection properties should be read only

        public IndexModel(IApplicationRolesService applicationRolesService, ILogger<IndexModel> logger, 
            IApplicationUsersService applicationUsersService)
        {
            _applicationRolesService = applicationRolesService;
            _applicationUsersService = applicationUsersService;
            _logger = logger;
        }

        private IList<string> _MyRoles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.Administrator.Role_View)]
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {

                var applicationRolesListResponse = await _applicationRolesService.ListDefaultRolesAsync().ConfigureAwait(false);
                ApplicationRole = (IList<ApplicationRole>)applicationRolesListResponse.ApplicationRole;

                //Set Return URL and store in session
                HttpContext.Session.SetString("Referer", Request.GetEncodedUrl());

                return Page();
            }
            catch (System.Exception Ex)
            {
                _logger.LogError(Ex,$"Roles.Index Page Error: {Ex.Message} " +
                    $"{Environment.NewLine}" );
                return Page();
            }
        }

    }
}
