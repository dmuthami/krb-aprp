using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class DisbursementCodeListRepository : BaseRepository,IDisbursementCodeListRepository
    {
        private readonly ILogger _logger;
        public DisbursementCodeListRepository(AppDbContext context,
            ILogger<DisbursementCodeListRepository> logger)
        : base(context)
        {
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddAsync(DisbursementCodeList disbursementCodeList)
        {
            try
            {
                await _context.DisbursementCodeLists.AddAsync(disbursementCodeList).ConfigureAwait(false);
                return Ok(disbursementCodeList);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"DisbursementCodeListRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(disbursementCodeList);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> DetachFirstEntryAsync(DisbursementCodeList disbursementCodeList)
        {
            try
            {
                _context.Entry(disbursementCodeList).State = EntityState.Detached;
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"DisbursementCodeListRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FindByCodeAsync(string Code)
        {
            try
            {
                var disbursementCodeList = await _context.DisbursementCodeLists
                    .Where(c=>c.Code==Code)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
                return Ok(disbursementCodeList);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"DisbursementCodeListRepository.FindByCodeAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FindByDisbursementCodeListEntryAsync(DisbursementCodeList disbursementCodeList)
        {
            try
            {
                var disbursementCodeListObject = await _context.DisbursementCodeLists
                    .Where(c => c.Code == disbursementCodeList.Code ||
                    c.Name.ToLower()== disbursementCodeList.Name.ToLower())
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
                return Ok(disbursementCodeListObject);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"DisbursementCodeListRepository.FindByCodeAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FindByIdAsync(long ID)
        {
            try
            {
                var disbursementCodeList = await _context.DisbursementCodeLists.FindAsync(ID).ConfigureAwait(false);
                return Ok(disbursementCodeList);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"DisbursementCodeListRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ListAsync()
        {
            try
            {
                var disbursementCodeLists = await _context.DisbursementCodeLists.ToListAsync().ConfigureAwait(false); ;
                return Ok(disbursementCodeLists);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"DisbursementCodeListRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Remove(DisbursementCodeList disbursementCodeList)
        {
            try
            {
                 _context.DisbursementCodeLists.Remove(disbursementCodeList) ;
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"DisbursementCodeListRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Update(DisbursementCodeList disbursementCodeList)
        {
            try
            {
                _context.DisbursementCodeLists.Update(disbursementCodeList);
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"DisbursementCodeListRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Update(long ID, DisbursementCodeList disbursementCodeList)
        {            
            try
            {
                _context.Entry(disbursementCodeList).State = EntityState.Modified;
                return Ok(disbursementCodeList);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"DisbursementCodeListRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }
    }
}
