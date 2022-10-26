using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class ARICSMasterApprovalRepository : BaseRepository, IARICSMasterApprovalRepository
    {
        private readonly ILogger _logger;
        public ARICSMasterApprovalRepository(AppDbContext context,
            ILogger<ARICSMasterApprovalRepository> logger)
        : base(context)
        {
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddAsync(ARICSMasterApproval aRICSMasterApproval)
        {
            try
            {
                await _context.ARICSMasterApprovals.AddAsync(aRICSMasterApproval).ConfigureAwait(false);
                return Ok(aRICSMasterApproval);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSMasterApprovalRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(aRICSMasterApproval);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DetachFirstEntryAsync(ARICSMasterApproval aRICSMasterApproval)
        {
            try
            {
                _context.Entry(aRICSMasterApproval).State = EntityState.Detached;
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSMasterApprovalRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FindByIdAsync(long ID)
        {
            try
            {
                var aricsMasterApproval = await _context.ARICSMasterApprovals
                    .Include(i=>i.ARICSYear)
                    .Where(i=>i.ID==ID)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
                return Ok(aricsMasterApproval);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSMasterApprovalRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ListByAuthorityAndARICSYearAsync(long AuthorityId, int ARICSYearId)
        {
            try
            {
                IQueryable<ARICSMasterApproval> x = null;
                await Task.Run(() =>
                {
                    x = _context.ARICSMasterApprovals
                    .Where(s => /**/s.ARICSYearId == ARICSYearId && s.AuthorityId == AuthorityId);
                }).ConfigureAwait(false);
                return Ok(x);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSMasterApprovalRepository.ListByAuthorityAndARICSYearAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ListAsync()
        {
            try
            {
                var aricsApprovalList = await _context.ARICSMasterApprovals.ToListAsync().ConfigureAwait(false); ;
                return Ok(aricsApprovalList);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSMasterApprovalRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Remove(ARICSMasterApproval aRICSMasterApproval)
        {
            try
            {
                _context.ARICSMasterApprovals.Remove(aRICSMasterApproval);
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSMasterApprovalRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Update(ARICSMasterApproval aRICSMasterApproval)
        {
            try
            {
                _context.ARICSMasterApprovals.Update(aRICSMasterApproval);
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSMasterApprovalRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Update(long ID, ARICSMasterApproval aRICSMasterApproval)
        {
            try
            {
                _context.Entry(aRICSMasterApproval).State = EntityState.Modified;
                return Ok(aRICSMasterApproval);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSMasterApprovalRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }
    }
}
