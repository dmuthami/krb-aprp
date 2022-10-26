using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace APRP.Web
{
    public class BIKuRAModel : PageModel
    {
        private readonly IApplicationUsersService _applicationUsersService;
        private readonly IAuthorityService _authorityService;
        private readonly ILogger _logger;
        public BIKuRAModel(ILogger<BIKuRAModel> logger, IApplicationUsersService applicationUsersService,
            IAuthorityService authorityService)
        {
            _logger = logger;
            _applicationUsersService = applicationUsersService;
            _authorityService = authorityService;
        }

        public ApplicationUser _ApplicationUser { get; set; }

        public Authority Authority { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                //Get Identity
                var applicationUserResponse = await _applicationUsersService.FindByNameAsync(User.Identity.Name).ConfigureAwait(false);
                var objectResult = (ObjectResult)applicationUserResponse.IActionResult;
                if (objectResult.StatusCode.ToString().Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result2 = (OkObjectResult)objectResult;
                    ApplicationUser applicationUser = (ApplicationUser)result2.Value;
                    if (applicationUser != null)
                    {
                        _ApplicationUser = applicationUser;
                        var authorityResp = await _authorityService.FindByIdAsync(applicationUser.AuthorityId).ConfigureAwait(false);
                        Authority = authorityResp.Authority;
                    }
                    else
                    {
                        _ApplicationUser = null;
                    }
                }
                return Page();
            }
            catch (System.Exception Ex)
            {

                string str = Ex.Message;
                _logger.LogError($"DetailsModel.OnGetAsync Error : {str}");
                ModelState.AddModelError(string.Empty, Ex.Message);
                return Page();
            }
        }
    }
}