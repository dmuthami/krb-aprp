using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class DisbursementTrancheRepository : BaseRepository,IDisbursementTrancheRepository
    {
        private readonly ILogger _logger;
        public DisbursementTrancheRepository(AppDbContext context,
            ILogger<DisbursementTrancheRepository> logger)
        : base(context)
        {
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddAsync(DisbursementTranche disbursementTranche)
        {
            try
            {
                await _context.DisbursementTranches.AddAsync(disbursementTranche).ConfigureAwait(false);
                return Ok(disbursementTranche);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"DisbursementTrancheRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(disbursementTranche);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FindByIdAsync(long ID)
        {
            try
            {
                var arics= await _context.DisbursementTranches.FindAsync(ID).ConfigureAwait(false);
                return Ok(arics);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"DisbursementTrancheRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ListAsync()
        {
            try
            {
                var aricsYearList = await _context.DisbursementTranches.ToListAsync().ConfigureAwait(false); ;
                return Ok(aricsYearList);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"DisbursementTrancheRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Remove(DisbursementTranche disbursementTranche)
        {
            try
            {
                 _context.DisbursementTranches.Remove(disbursementTranche) ;
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"DisbursementTrancheRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Update(DisbursementTranche disbursementTranche)
        {
            try
            {
                _context.DisbursementTranches.Update(disbursementTranche);
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"DisbursementTrancheRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Update(long ID, DisbursementTranche disbursementTranche)
        {            
            try
            {
                _context.Entry(disbursementTranche).State = EntityState.Modified;
                return Ok(disbursementTranche);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"DisbursementTrancheRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }
    }
}
