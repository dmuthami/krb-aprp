using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace APRP.Web.Domain.Repositories
{
    public interface IApplicationRolesRepository
    {
        Task<IEnumerable<ApplicationRole>> ListAsync();

        Task<IEnumerable<ApplicationRole>> ListDefaultRolesAsync();

        Task<IEnumerable<ApplicationRole>> ListRoleClaimsAsync();

        Task AddAsync(ApplicationRole applicationRole);

        Task<ApplicationRole> FindByIdAsync(string ID);

        Task<ApplicationRole> FindByNameAsync(string roleName);

        Task<IEnumerable<ApplicationRole>> FindByName2Async(string roleName);

        Task<bool> RoleExistsAsync(string roleName);

        void Remove(ApplicationRole applicationRole);

        void Update(ApplicationRole applicationRole);

        Task<IActionResult> GetClaimsAsync(ApplicationRole applicationRole);

        Task<IActionResult> AddClaimAsync(ApplicationRole applicationRole, Claim claim);

        Task<IActionResult> RemoveClaimAsync(ApplicationRole applicationRole, Claim claim);
    }
}
