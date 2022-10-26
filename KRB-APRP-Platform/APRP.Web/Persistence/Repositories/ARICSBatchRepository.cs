using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class ARICSBatchRepository : BaseRepository, IARICSBatchRepository
    {
        private readonly ILogger _logger;
        public ARICSBatchRepository(AppDbContext context,
            ILogger<ARICSBatchRepository> logger)
        : base(context)
        {
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddAsync(ARICSBatch aRICSBatch)
        {
            try
            {
                await _context.ARICSBatchs.AddAsync(aRICSBatch).ConfigureAwait(false);
                return Ok(aRICSBatch);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSBatchRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(aRICSBatch);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DetachFirstEntryAsync(ARICSBatch aRICSBatch)
        {
            try
            {
                _context.Entry(aRICSBatch).State = EntityState.Detached;
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSBatchRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FindByIdAsync(long ID)
        {
            try
            {
                var aricsApproval = await _context.ARICSBatchs.FindAsync(ID).ConfigureAwait(false);
                return Ok(aricsApproval);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSBatchRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ListByAuthorityAndARICSYearAsync(long AuthorityId, int ARICSYearId)
        {
            try
            {
                IQueryable<ARICSBatch> x = null;
                await Task.Run(() =>
                {
                    x = _context.ARICSBatchs
                     .Include(i => i.ARICSMasterApproval)
                     .Include(i=>i.RoadSection)
                        .ThenInclude(i=>i.Road)
                    .Where(s => s.ARICSMasterApproval.ARICSYearId == ARICSYearId && s.ARICSMasterApproval.AuthorityId == AuthorityId);
                }).ConfigureAwait(false);
                return Ok(x);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSBatchRepository.ListByAuthorityAndARICSYearAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ListAsync()
        {
            try
            {
                var aricsApprovalList = await _context.ARICSBatchs.ToListAsync().ConfigureAwait(false); ;
                return Ok(aricsApprovalList);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSBatchRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Remove(ARICSBatch aRICSBatch)
        {
            try
            {
                _context.ARICSBatchs.Remove(aRICSBatch);
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSBatchRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Update(ARICSBatch aRICSBatch)
        {
            try
            {
                _context.ARICSBatchs.Update(aRICSBatch);
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSBatchRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Update(long ID, ARICSBatch aRICSBatch)
        {
            try
            {
                _context.Entry(aRICSBatch).State = EntityState.Modified;
                return Ok(aRICSBatch);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSBatchRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        public async Task<IActionResult> FindByRoadSectionIdAndARICSMasterApprovalIdAsync(long ARICSMasterApprovalID, long RoadSectionId)
        {
            try
            {
                var aricsBatch = await _context.ARICSBatchs
                    .Where(i=>i.ARICSMasterApprovalId==ARICSMasterApprovalID
                    && i.RoadSectionId==RoadSectionId
                    ).FirstOrDefaultAsync()
                    .ConfigureAwait(false);
                return Ok(aricsBatch);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSBatchRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        public async Task<IActionResult> ListByARICSMasterApprovalIdAndARICSYearIdAsync(long ARICSMasterApprovalId, int ARICSYearId)
        {
            try
            {
                IQueryable<ARICSBatch> x = null;
                await Task.Run(() =>
                {
                    x = _context.ARICSBatchs
                    .Include(i=>i.RoadSection)
                    .Where(s => s.ARICSMasterApprovalId == ARICSMasterApprovalId && s.ARICSMasterApproval.ARICSYearId == ARICSYearId);
                }).ConfigureAwait(false);

                return Ok(x);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSMasterApprovalRepository.ListByAuthorityAndARICSYearAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }
    }
}
