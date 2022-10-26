using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class FundingSourceSubCodeRepository : BaseRepository,IFundingSourceSubCodeRepository
    {
        private readonly ILogger _logger;
        public FundingSourceSubCodeRepository(AppDbContext context,
            ILogger<FundingSourceSubCodeRepository> logger)
        : base(context)
        {
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddAsync(FundingSourceSubCode fundingSourceSubCode)
        {
            try
            {
                await _context.FundingSourceSubCodes.AddAsync(fundingSourceSubCode).ConfigureAwait(false);
                return Ok(fundingSourceSubCode);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"FundingSourceSubCodeRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(fundingSourceSubCode);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FindByIdAsync(long ID)
        {
            try
            {
                var arics= await _context.FundingSourceSubCodes.FindAsync(ID).ConfigureAwait(false);
                return Ok(arics);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"FundingSourceSubCodeRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ListAsync()
        {
            try
            {
                var aricsYearList = await _context.FundingSourceSubCodes.ToListAsync().ConfigureAwait(false); ;
                return Ok(aricsYearList);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"FundingSourceSubCodeRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Remove(FundingSourceSubCode fundingSourceSubCode)
        {
            try
            {
                 _context.FundingSourceSubCodes.Remove(fundingSourceSubCode) ;
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"FundingSourceSubCodeRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Update(FundingSourceSubCode fundingSourceSubCode)
        {
            try
            {
                _context.FundingSourceSubCodes.Update(fundingSourceSubCode);
                return Ok();
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"FundingSourceSubCodeRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> Update(long ID, FundingSourceSubCode fundingSourceSubCode)
        {            
            try
            {
                _context.Entry(fundingSourceSubCode).State = EntityState.Modified;
                return Ok(fundingSourceSubCode);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"FundingSourceSubCodeRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }
    }
}
