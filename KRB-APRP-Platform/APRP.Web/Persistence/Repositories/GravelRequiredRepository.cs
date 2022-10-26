using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class GravelRequiredRepository : BaseRepository,IGravelRequiredRepository
    {
        private readonly ILogger _logger;
        public GravelRequiredRepository(AppDbContext context,
            ILogger<GravelRequiredRepository> logger)
        : base(context)
        {
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddAsync(GravelRequired gravelRequired)
        {
            try
            {
                await _context.GravelRequireds.AddAsync(gravelRequired).ConfigureAwait(false);
                return Ok(gravelRequired);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ShoulderRequiredRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(gravelRequired);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FindByIdAsync(int ID)
        {
            try
            {
                var gravelRequired = await _context.GravelRequireds.FindAsync(ID).ConfigureAwait(false);
                return Ok(gravelRequired);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ShoulderRequiredRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ListAsync()
        {
            try
            {
                var gravelRequiredList = await _context.GravelRequireds.ToListAsync().ConfigureAwait(false); ;
                return Ok(gravelRequiredList);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ShoulderRequiredRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Remove(GravelRequired gravelRequired)
        {
            try
            {
                 _context.GravelRequireds.Remove(gravelRequired) ;
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ShoulderRequiredRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Update(GravelRequired gravelRequired)
        {
            try
            {
                _context.GravelRequireds.Update(gravelRequired);
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ShoulderRequiredRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Update(int ID, GravelRequired gravelRequired)
        {            
            try
            {
                _context.Entry(gravelRequired).State = EntityState.Modified;
                return Ok(gravelRequired);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ShoulderRequiredRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }
    }
}
