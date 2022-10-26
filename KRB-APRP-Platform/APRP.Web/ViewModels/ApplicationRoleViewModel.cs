using APRP.Web.Domain.Models;

namespace APRP.Web.ViewModels
{
    public class ApplicationRoleViewModel
    {
        public ApplicationRole ApplicationRole { get; set; }

        public IList<System.Security.Claims.Claim> RoleClaims { get; set; }

        public IList<string> AllPermissions { get; set; }
    }
}
