using Microsoft.AspNetCore.Authorization;

namespace APRP.Web.Claims
{
    internal class PermissionRequirement : IAuthorizationRequirement
    {
        /*
         * Second, we create a class that will hold the permission to be evaluated.
         */
        public string Permission { get; private set; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}
