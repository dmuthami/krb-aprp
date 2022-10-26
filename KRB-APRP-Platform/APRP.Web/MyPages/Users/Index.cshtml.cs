using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace APRP.Web.MyPages.Users
{
    [TypeFilter(typeof(Extensions.Filters.SessionTimeoutRazorFilter))]
    public class IndexModel : PageModel
    {
        private readonly IRegisterService _registerService;
        private readonly ILogger _logger;
        private readonly IAuthorityService _authorityService;

        public IndexModel(IRegisterService registerService,
            ILogger<IndexModel> logger, IAuthorityService authorityService)
        {
            _registerService = registerService;
            _logger = logger;
            _authorityService = authorityService;
        }
        public IList<ApplicationUser> ApplicationUserList { get; set; }

        public IList<Authority> AuthorityList { get; set; }

        private IList<string> _MyRoles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        [Authorize(Claims.Permission.Administrator.Role_View)]
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var applicationUsersLResponse = await _registerService.ListAsync2().ConfigureAwait(false);
                var objectResult = (ObjectResult)applicationUsersLResponse.IActionResult;
                var result2 = (OkObjectResult)objectResult;
                ApplicationUserList = (IList<ApplicationUser>)result2.Value;

                //Get a list of all authorities
                var authResp = await _authorityService.ListAsync().ConfigureAwait(false);
                AuthorityList = (IList<Authority>)authResp;

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
    }
}
