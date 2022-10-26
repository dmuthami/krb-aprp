using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APRP.Web.Persistence.Repositories
{
    public class ApplicationRolesRepository : BaseRepository, IApplicationRolesRepository
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger _logger;
        public ApplicationRolesRepository(AppDbContext context, RoleManager<ApplicationRole> roleManager,
            ILogger<ApplicationRolesRepository> logger) : base(context)
        {
            _roleManager = roleManager;
            _logger = logger;
        }
        public async Task AddAsync(ApplicationRole applicationRole)
        {
            //await _context.Roles.AddAsync(new IdentityRole()
            //{
            //    Name= applicationRole.Name,
            //    NormalizedName = applicationRole.Name.ToUpper()
            //});
            await _roleManager.CreateAsync(applicationRole).ConfigureAwait(false);
        }

        public async Task<ApplicationRole> FindByIdAsync(string ID)
        {
            var applicationRole = await _context.Roles.FindAsync(ID).ConfigureAwait(false);
            return applicationRole;
        }

        public async Task<ApplicationRole> FindByNameAsync(string roleName)
        {
            var applicationRole = await _context.Roles
                .Where(s => s.Name == roleName)
                .FirstOrDefaultAsync().ConfigureAwait(false);

            return applicationRole;
        }

        public async Task<IEnumerable<ApplicationRole>> ListAsync()
        {
            return await _context.Roles
                .ToListAsync().ConfigureAwait(false);
        }

        public void Update(ApplicationRole applicationRole)
        {
            _context.Roles.Update(applicationRole);
        }

        public void Remove(ApplicationRole applicationRole)
        {
            _context.Roles.Remove(applicationRole);
        }

        /// <summary>
        /// 0=Role Based authorization
        /// 1=Role Claim authorization coupled with permissions
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ApplicationRole>> ListRoleClaimsAsync()
        {
            return await _context.Roles.
                Where(r => r.RoleType == 1).
                ToListAsync().ConfigureAwait(false);
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _roleManager.RoleExistsAsync(roleName).ConfigureAwait(false);
        }

        public async Task<IEnumerable<ApplicationRole>> FindByName2Async(string roleName)
        {
            var applicationRoles = await _context.Roles
              .Where(s => s.Name == roleName)
              .ToListAsync().ConfigureAwait(false);

            return applicationRoles;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> GetClaimsAsync(ApplicationRole applicationRole)
        {
            try
            {
                var claimsList = await _roleManager.GetClaimsAsync(applicationRole).ConfigureAwait(false);
                return Ok(claimsList);
            }
            catch (System.Exception Ex)
            {
                return BadRequest(Ex);
            }

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddClaimAsync(ApplicationRole applicationRole, Claim claim)
        {
            try
            {
                var result = await _roleManager.
                AddClaimAsync(applicationRole, claim).ConfigureAwait(false);
                return Ok(result);
            }
            catch (System.Exception Ex)
            {
                return BadRequest(Ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> RemoveClaimAsync(ApplicationRole applicationRole, Claim claim)
        {
            try
            {
                var result = await _roleManager.
                RemoveClaimAsync(applicationRole, claim).ConfigureAwait(false);
                return Ok(result);
            }
            catch (System.Exception Ex)
            {
                return BadRequest(Ex);
            }
        }

        public async Task<IEnumerable<ApplicationRole>> ListDefaultRolesAsync()
        {
            return await _context.Roles
            .Where(x => x.RoleType == 0 )/*Show system defined only and not Adminisrators */
            .ToListAsync().ConfigureAwait(false);
        }
    }
}
