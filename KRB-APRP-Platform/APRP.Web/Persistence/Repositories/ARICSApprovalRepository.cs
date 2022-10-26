using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class ARICSApprovalRepository : BaseRepository, IARICSApprovalRepository
    {
        private readonly ILogger _logger;
        public ARICSApprovalRepository(AppDbContext context,
            ILogger<ARICSApprovalRepository> logger)
        : base(context)
        {
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddAsync(ARICSApproval aRICSApproval)
        {
            try
            {
                await _context.ARICSApprovals.AddAsync(aRICSApproval).ConfigureAwait(false);
                return Ok(aRICSApproval);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSApprovalRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(aRICSApproval);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DetachFirstEntryAsync(ARICSApproval aRICSApproval)
        {
            try
            {
                _context.Entry(aRICSApproval).State = EntityState.Detached;
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSApprovalRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FindByIdAsync(long ID)
        {
            try
            {
                var aricsApproval = await _context.ARICSApprovals.FindAsync(ID).ConfigureAwait(false);
                return Ok(aricsApproval);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSApprovalRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FindByARICSMasterApprovalIdAsync(long ARICSMasterApprovalIdId)
        {
            try
            {
                var aricsApproval = await _context.ARICSApprovals
                    .Where(s => s.ARICSMasterApprovalId == ARICSMasterApprovalIdId)
                    .Include(i=>i.ARICSApprovalLevel)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
                return Ok(aricsApproval);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSApprovalRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ListAsync()
        {
            try
            {
                var aricsApprovalList = await _context.ARICSApprovals.ToListAsync().ConfigureAwait(false); ;
                return Ok(aricsApprovalList);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSApprovalRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ListHistoryAsync(long ARICSApprovalId)
        {
            try
            {
                var aricsApprovalhList = await _context.ARICSApprovalhs
                    .Include(i=>i.ARICSApprovalLevel)
                    .Where(x=>x.ARICSApprovalId== ARICSApprovalId)
                    .OrderByDescending(x=>x.BeginLifeSpan)
                    .ToListAsync()
                    .ConfigureAwait(false); ;
                return Ok(aricsApprovalhList);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSApprovalRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Remove(ARICSApproval aRICSApproval)
        {
            try
            {
                _context.ARICSApprovals.Remove(aRICSApproval);
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSApprovalRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Update(ARICSApproval aRICSApproval)
        {
            try
            {
                _context.ARICSApprovals.Update(aRICSApproval);
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSApprovalRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Update(long ID, ARICSApproval aRICSApproval)
        {
            try
            {
                _context.Entry(aRICSApproval).State = EntityState.Modified;
                return Ok(aRICSApproval);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSApprovalRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }
    }
}
