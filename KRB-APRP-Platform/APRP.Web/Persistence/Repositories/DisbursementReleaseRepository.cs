using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class DisbursementReleaseRepository : BaseRepository,IDisbursementReleaseRepository
    {
        private readonly ILogger _logger;
        public DisbursementReleaseRepository(AppDbContext context,
            ILogger<DisbursementReleaseRepository> logger)
        : base(context)
        {
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddAsync(DisbursementRelease disbursementRelease)
        {
            try
            {
                await _context.DisbursementReleases.AddAsync(disbursementRelease).ConfigureAwait(false);
                return Ok(disbursementRelease);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"DisbursementReleaseRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(disbursementRelease);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FindbisbursementReleaseAsync(long ReleaseId, long DisbursementId)
        {
            try
            {
                var disbursementRelease= await _context.DisbursementReleases
                    .Where(x=>x.ReleaseId==ReleaseId && x.DisbursementId==DisbursementId)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
                return Ok(disbursementRelease);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"DisbursementReleaseRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ListAsync()
        {
            try
            {
                var aricsYearList = await _context.DisbursementReleases.ToListAsync().ConfigureAwait(false); ;
                return Ok(aricsYearList);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"DisbursementReleaseRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(Ex.Message.ToString());
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Remove(DisbursementRelease disbursementRelease)
        {
            try
            {
                 _context.DisbursementReleases.Remove(disbursementRelease) ;
                return Ok(disbursementRelease);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"DisbursementReleaseRepository.Remove Error: {Environment.NewLine}");
                return BadRequest(disbursementRelease);
            }
        }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        //public async Task<IActionResult> RemoveAsync(long ReleaseId, long DisbursementId)
        //{
        //    try
        //    {
        //        var disbursementRelease = await _context.DisbursementReleases
        //            .Where(x => x.ReleaseId == ReleaseId && x.DisbursementId == DisbursementId)
        //            .FirstOrDefaultAsync()
        //            .ConfigureAwait(false);
        //        return Ok(disbursementRelease);
        //    }
        //    catch (Exception Ex)
        //    {

        //        _logger.LogError(Ex, $"DisbursementReleaseRepository.RemoveAsync Error: {Environment.NewLine}");
        //        return BadRequest(null);
        //    }
        //}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Update(DisbursementRelease disbursementRelease)
        {
            try
            {
                _context.DisbursementReleases.Update(disbursementRelease);
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"DisbursementReleaseRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Update(int ID, DisbursementRelease disbursementRelease)
        {            
            try
            {
                _context.Entry(disbursementRelease).State = EntityState.Modified;
                return Ok(disbursementRelease);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"DisbursementReleaseRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }
    }
}
