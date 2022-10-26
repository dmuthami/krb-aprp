using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class ItemActivityUnitCostRateRepository : BaseRepository, IItemActivityUnitCostRateRepository
    {
        private readonly ILogger _logger;
        public ItemActivityUnitCostRateRepository(AppDbContext context,
            ILogger<ItemActivityUnitCostRateRepository> logger)
        : base(context)
        {
            _logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> AddAsync(ItemActivityUnitCostRate itemActivityUnitCostRate)
        {
            try
            {
                await _context.ItemActivityUnitCostRates.AddAsync(itemActivityUnitCostRate).ConfigureAwait(false);
                return Ok(itemActivityUnitCostRate);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSYearRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(itemActivityUnitCostRate);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FindByFinancialYearAuthorityAndItemUnitCostAsync(long FinancialYearId, long AuthorityId, long ItemActivityUnitCostId)
        {
            try
            {
                var itemActivityUnitCostRate = await _context.ItemActivityUnitCostRates
                    .Where(i=>i.FinancialYearId== FinancialYearId && i.AuthorityId== AuthorityId 
                        && i.ItemActivityUnitCostId== ItemActivityUnitCostId)
                    .FirstOrDefaultAsync().ConfigureAwait(false);
                return Ok(itemActivityUnitCostRate);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ItemActivityUnitCostRateRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> FindByIdAsync(long ID)
        {
            try
            {
                var itemActivityUnitCostRate = await _context.ItemActivityUnitCostRates.FindAsync(ID).ConfigureAwait(false);
                return Ok(itemActivityUnitCostRate);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ItemActivityUnitCostRateRepository.AddAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task<IActionResult> ListAsync()
        {
            try
            {
                var itemActivityUnitCostRateList = await _context.ItemActivityUnitCostRates.ToListAsync().ConfigureAwait(false); ;
                return Ok(itemActivityUnitCostRateList);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"ARICSYearRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }

        public Task<IActionResult> Remove(ItemActivityUnitCostRate itemActivityUnitCostRate)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> Update(ItemActivityUnitCostRate itemActivityUnitCostRate)
        {
            throw new NotImplementedException();
        }
    }
}
