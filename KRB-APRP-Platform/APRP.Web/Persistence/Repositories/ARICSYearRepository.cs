using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class ARICSYearRepository : BaseRepository,IARICSYearRepository
    {
        private readonly ILogger _logger;
        public ARICSYearRepository(AppDbContext context,
            ILogger<ARICSYearRepository> logger)
        : base(context)
        {
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddAsync(ARICSYear aRICSYear)
        {
            try
            {
                await _context.ARICSYears.AddAsync(aRICSYear).ConfigureAwait(false);
                return Ok(aRICSYear);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSYearRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(aRICSYear);
            }
        }

        public async Task<IActionResult> DetachFirstEntryAsync(ARICSYear aRICSYear)
        {
            try
            {
                _context.Entry(aRICSYear).State = EntityState.Detached;
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSYearRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FindByIdAsync(int ID)
        {
            try
            {
                var arics= await _context.ARICSYears.FindAsync(ID).ConfigureAwait(false);
                return Ok(arics);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSYearRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FindByYearAsync(int ARICSYear)
        {
            try
            {
                var arics = await _context.ARICSYears
                    .Where(s=>s.Year== ARICSYear)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
                return Ok(arics);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSYearRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ListAsync()
        {
            try
            {
                var aricsYearList = await _context.ARICSYears.ToListAsync().ConfigureAwait(false); ;
                return Ok(aricsYearList);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSYearRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Remove(ARICSYear aRICSYear)
        {
            try
            {
                 _context.ARICSYears.Remove(aRICSYear) ;
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSYearRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Update(ARICSYear aRICSYear)
        {
            try
            {
                _context.ARICSYears.Update(aRICSYear);
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSYearRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Update(int ID, ARICSYear aRICSYear)
        {            
            try
            {
                _context.Entry(aRICSYear).State = EntityState.Modified;
                return Ok(aRICSYear);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSYearRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }
    }
}
