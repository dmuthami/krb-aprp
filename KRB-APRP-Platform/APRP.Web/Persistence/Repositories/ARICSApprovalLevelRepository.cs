using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class ARICSApprovalLevelRepository : BaseRepository, IARICSApprovalLevelRepository
    {
        private readonly ILogger _logger;
        public ARICSApprovalLevelRepository(AppDbContext context,
            ILogger<ARICSApprovalLevelRepository> logger)
        : base(context)
        {
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddAsync(ARICSApprovalLevel aRICSApprovalLevel)
        {
            try
            {
                await _context.ARICSApprovalLevels.AddAsync(aRICSApprovalLevel).ConfigureAwait(false);
                return Ok(aRICSApprovalLevel);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSApprovalLevelRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(aRICSApprovalLevel);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DetachFirstEntryAsync(ARICSApprovalLevel aRICSApprovalLevel)
        {
            try
            {
                _context.Entry(aRICSApprovalLevel).State = EntityState.Detached;
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSApprovalLevelRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FindByIdAsync(long ID)
        {
            try
            {
                var aricsApproval = await _context.ARICSApprovalLevels.FindAsync(ID).ConfigureAwait(false);
                return Ok(aricsApproval);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSApprovalLevelRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FindByAuthorityTypeAndStatusAsync(long AuthorityType, int Status)
        {
            try
            {
                var aricsApproval = await _context.ARICSApprovalLevels
                    .Where(s => s.AuthorityType == AuthorityType && s.Status == Status)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
                return Ok(aricsApproval);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSApprovalLevelRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ListAsync()
        {
            try
            {
                var aricsApprovalList = await _context.ARICSApprovalLevels.ToListAsync().ConfigureAwait(false); ;
                return Ok(aricsApprovalList);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSApprovalLevelRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Remove(ARICSApprovalLevel aRICSApprovalLevel)
        {
            try
            {
                _context.ARICSApprovalLevels.Remove(aRICSApprovalLevel);
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSApprovalLevelRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Update(ARICSApprovalLevel aRICSApprovalLevel)
        {
            try
            {
                _context.ARICSApprovalLevels.Update(aRICSApprovalLevel);
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSApprovalLevelRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Update(long ID, ARICSApprovalLevel aRICSApprovalLevel)
        {
            try
            {
                _context.Entry(aRICSApprovalLevel).State = EntityState.Modified;
                return Ok(aRICSApprovalLevel);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSApprovalLevelRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        public async Task<IActionResult> FindByStatusAsync(int Status)
        {
            try
            {
                var aricsApproval = await _context.ARICSApprovalLevels
                    .Where(s => s.Status == Status)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
                return Ok(aricsApproval);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSApprovalLevelRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }
    }
}
