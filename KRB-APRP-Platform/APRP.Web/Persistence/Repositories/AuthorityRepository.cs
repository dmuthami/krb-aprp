using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using APRP.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class AuthorityRepository : BaseRepository, IAuthorityRepository
    {
        public AuthorityRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }

        public async Task AddAsync(Authority authority)
        {
            await _context.Authorities.AddAsync(authority).ConfigureAwait(false);
        }

        public async Task<Authority> FindByCodeAsync(string code)
        {
#pragma warning disable CA1304 // Specify CultureInfo
            return await _context.Authorities.FirstOrDefaultAsync(a => a.Code.ToLower().Trim() == code.ToLower().Trim()).ConfigureAwait(false);
#pragma warning restore CA1304 // Specify CultureInfo
        }

        public async Task<Authority> FindByIdAsync(long ID)
        {
            return await _context.Authorities.Include(r=>r.Regions).FirstOrDefaultAsync(a=>a.ID == ID).ConfigureAwait(false);
        }

        public async Task<IActionResult> ListAsync(int PageNumber, int PageSize)
        {
            try
            {
                AuthorityViewModel2 authorityViewModel = new AuthorityViewModel2();
                authorityViewModel.PagedData = await _context.Authorities
                   .Skip((PageNumber - 1) * PageSize)
                   .Take(PageSize)
                   .ToListAsync();

                authorityViewModel.TotalRecords = await _context.Authorities.CountAsync();
                return Ok(authorityViewModel);
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }

        public async Task<IEnumerable<Authority>> ListAsync()
        {
            return await _context.Authorities
            .OrderBy(w => w.Name)
            .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Authority>> ListAsync(string AuthorityType)
        {
            if (AuthorityType == null)
            {
                return await _context.Authorities
                .OrderBy(w => w.Name)
                .ToListAsync().ConfigureAwait(false);
            }
            else
            {
                if (AuthorityType.ToLower() == "RA".ToLower())
                {
                    return await _context.Authorities
                    .Where(x => x.Type == 1 || x.Type == 4 || x.Type == 0)
                    .ToListAsync().ConfigureAwait(false);
                }
                else
                {
                    //counties
                    return await _context.Authorities
                    .Where(x => x.Type == 2)
                    .ToListAsync().ConfigureAwait(false);
                }
            }
        }

        public void Remove(Authority authority)
        {
            _context.Authorities.Remove(authority);
        }
        public void Update(long ID, Authority authority)
        {
            _context.Entry(authority).State = EntityState.Modified; 
        }
    }
}
