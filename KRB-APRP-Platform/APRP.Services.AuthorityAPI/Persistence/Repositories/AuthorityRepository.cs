using APRP.Services.AuthorityAPI.Models;
using APRP.Services.AuthorityAPI.Models.Dto;
using APRP.Services.AuthorityAPI.Persistence.DbContexts;
using APRP.Services.AuthorityAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APRP.Services.AuthorityAPI.Persistence.Repositories
{
    public class AuthorityRepository : BaseRepository, IAuthorityRepository
    {

        public AuthorityRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IActionResult> AddAsync(Authority authority)
        {
            try
            {
                await _context.Authorities.AddAsync(authority).ConfigureAwait(false);
                return Ok(authority);
            }
            catch (Exception)
            {

                return BadRequest(authority);
            };
        }

        public async Task<IActionResult> FindByCodeAsync(string code)
        {
            try
            {

                var authority = await _context.Authorities.FirstOrDefaultAsync(a => a.Code.ToLower().Trim() == code.ToLower().Trim()).ConfigureAwait(false);
                return Ok(authority);
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }

        public async Task<IActionResult> FindByIdAsync(long ID)
        {
            try
            {
                var authority = await _context.Authorities.FirstOrDefaultAsync(a => a.ID == ID).ConfigureAwait(false);
                return Ok(authority);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        public async Task<IActionResult> ListAsync(int PageNumber, int PageSize)
        {
            try
            {
                AuthorityViewModel authorityViewModel = new AuthorityViewModel();
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


        public async Task<IActionResult> Remove(Authority authority)
        {
            try
            {
                _context.Authorities.Remove(authority);
                return Ok();
            }
            catch (Exception Ex)
            {
                return BadRequest(Ex);
            }
        }
        public async Task<IActionResult> Update(long ID, Authority authority)
        {
            try
            {
                _context.Entry(authority).State = EntityState.Modified;
                return Ok(authority);
            }
            catch (Exception Ex)
            {
                return BadRequest(Ex);
            }
        }
    }
}
