using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace APRP.Web.Claims
{
    /*
     * 
     * Third, we create an authorization handler
     *  that checks whether a user has the required permission, and if so, access is allowed.
     */
    internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        UserManager<ApplicationUser> _userManager;
        RoleManager<ApplicationRole> _roleManager;
        private readonly IServiceScope _scope;

        public PermissionAuthorizationHandler(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
            IServiceProvider services)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _scope = services.CreateScope();
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            //using (var myDBcontext = new AppDbContext(_scope.ServiceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
            //{
            //    var roleManager = _scope.ServiceProvider.GetService<RoleManager<ApplicationRole>>();
            //    var userManager = _scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
            //}


            var user = await _userManager.GetUserAsync(context.User).ConfigureAwait(false);
            if (user == null)
            {
                return;
            }

            // Get all the roles the user belongs to and check if any of the roles has the permission required
            // for the authorization to succeed.



            var userRoleNames = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            /*
             * NpgsqlOperationInProgressException: A command is already in progress is thrown for postgresql
             * Hence need to enumerate the queryable object
             * No need for this in SQl Server
             */
            var userRoles = _roleManager.Roles.Where(x => userRoleNames.Contains(x.Name)).AsEnumerable<ApplicationRole>().ToList();
            /**/

            foreach (var role in userRoles)
            {
                var roleClaims = await _roleManager.GetClaimsAsync(role).ConfigureAwait(false);
                var permissions = roleClaims.Where(x => x.Type == CustomClaimTypes.Permission &&
                                                        x.Value == requirement.Permission &&
                                                        x.Issuer == "LOCAL AUTHORITY")
                                            .Select(x => x.Value);

                if (permissions.Any())
                {
                    context.Succeed(requirement);
                    return;
                }
            }



        }
    }
}
