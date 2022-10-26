using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;
using System.Security.Claims;

namespace APRP.Web.Domain.Services
{
    public interface IApplicationRolesService
    {
        Task<ApplicationRolesListResponse> ListAsync();

        Task<ApplicationRolesListResponse> ListDefaultRolesAsync();

        Task<ApplicationRolesListResponse> ListRoleClaimsAsync();

        Task<ApplicationRolesResponse> AddAsync(ApplicationRole applicationRole);

        Task<ApplicationRolesResponse> FindByIdAsync(string ID);

        Task<ApplicationRolesResponse> FindByNameAsync(string roleName);

        Task<ApplicationRolesListResponse> FindByName2Async(string roleName);

        Task<bool> RoleExistsAsync(string roleName);

        Task<ApplicationRolesResponse> RemoveAsync(string ID);

        Task<ApplicationRolesResponse> Update(string ID, ApplicationRole applicationRole);

        Task<GenericResponse> GetClaimsAsync(ApplicationRole applicationRole);

        Task<GenericResponse> AddClaimAsync(ApplicationRole applicationRole, Claim claim);

        Task<GenericResponse> RemoveClaimAsync(ApplicationRole applicationRole, Claim claim);

    }
}
