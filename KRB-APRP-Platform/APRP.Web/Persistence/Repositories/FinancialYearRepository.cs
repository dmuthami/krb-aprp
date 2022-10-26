using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class FinancialYearRepository : BaseRepository, IFinancialYearRepository
    {
        private readonly ILogger _logger;
        public FinancialYearRepository(AppDbContext appDbContext,
             ILogger<FinancialYearRepository> logger) : base(appDbContext)
        {
            _logger = logger;
        }

        public async Task AddAsync(FinancialYear financialYear)
        {

            await _context.FinancialYears.AddAsync(financialYear).ConfigureAwait(false);
        }

        public async Task<FinancialYear> FindPreviousYearFromCurrentYear(string financialYearCode)
        {
            return await _context.FinancialYears.Where(c => c.Code.Substring(0, 4) == financialYearCode).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<FinancialYear> FindByIdAsync(long ID)
        {
            var financialYear = await _context.FinancialYears.FindAsync(ID)
                .ConfigureAwait(false);
            //Explicitly add Road 
            await _context.Entry(financialYear)
            .Reference(b => b.ARICSYear)
            .LoadAsync().ConfigureAwait(false);
            return financialYear;


        }
        public async Task<FinancialYear> FindCurrentYear()
        {
            return await _context.FinancialYears
                .Include(c => c.ARICSYear)
                .Where(s => (int)s.IsCurrent == 1)
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }
        public async Task<IEnumerable<FinancialYear>> ListAsync()
        {
            return await _context.FinancialYears
                .Include(x => x.ARICSYear)
                .OrderByDescending(y => y.IsCurrent)
                .ToListAsync().ConfigureAwait(false);
        }

        public void Remove(FinancialYear financialYear)
        {
            _context.FinancialYears.Remove(financialYear);
        }

        public void Update(FinancialYear financialYear)
        {
            _context.FinancialYears.Update(financialYear);
        }

        public async Task Update(long ID, FinancialYear financialYear)
        {
            _context.Entry(financialYear).State = EntityState.Modified;
        }

        #region 
        public async Task SetAllToNotCurrent(FinancialYear financialYear)
        {
            if (financialYear != null)
            {
                if (financialYear.IsCurrent == IsCurrent.Current)
                {
                    //Make sure that only this financial year is current
                    //Get list for Iscurrent==1
                    var list = await _context.FinancialYears.Where(y => y.IsCurrent == IsCurrent.Current).ToListAsync().ConfigureAwait(false);

                    if (list.Count() > 0)
                    {
                        foreach (var fyear in list)
                        {
                            if (fyear.ID != financialYear.ID)
                            {
                                fyear.IsCurrent = IsCurrent.Not_Current;
                            }
                        }
                        await _context.SaveChangesAsync().ConfigureAwait(false);
                    }
                }
            }
        }

        public async Task<IActionResult> ListAsync(string Code)
        {
            try
            {
                var budgetCeilingComputationList = await _context.FinancialYears
                    .Where(x => x.Code == Code)
                     .Include(b => b.BudgetCeilingComputations)
                    .ToListAsync().ConfigureAwait(false);
                return Ok(budgetCeilingComputationList);
            }
            catch (Exception Ex)
            {

                _logger.LogError(Ex, $"BudgetCeilingComputationRepository.ListAsync Error: {Environment.NewLine}");
                return BadRequest(null);
            }
        }
        #endregion
    }
}
