using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class BudgetCeilingHeaderRepository : BaseRepository , IBudgetCeilingHeaderRepository
    {

        public BudgetCeilingHeaderRepository(AppDbContext context) : base(context)
        {

        }

        public async Task AddAsync(BudgetCeilingHeader  budgetCeilingHeader)
        {
            await _context.BudgetCeilingHeaders.AddAsync(budgetCeilingHeader).ConfigureAwait(false);
        }

        public async Task<BudgetCeilingHeader> FindByFinancialYearAsync(long FinancialYearID)
        {
            return await _context.BudgetCeilingHeaders
                .Include(f => f.FinancialYear)
                .FirstOrDefaultAsync(b => b.FinancialYearId == FinancialYearID).ConfigureAwait(false);
        }

        public async Task<BudgetCeilingHeader> FindByIdAsync(long ID)
        {
            return await _context.BudgetCeilingHeaders
                .Include(f => f.FinancialYear)
                .FirstOrDefaultAsync(b=>b.ID==ID).ConfigureAwait(false);
        }

        public async Task<BudgetCeilingHeader> FindCurrentAsync()
        {
            //isCurrent ==0 for Current Budget
            //isCurrent ==1 for Current Financial year
            return await _context.BudgetCeilingHeaders.Include(l=>l.BudgetCeilings)
                .ThenInclude(a=>a.Authority).Include(f => f.FinancialYear)
                .FirstOrDefaultAsync(b=>(int)b.FinancialYear.IsCurrent==1).ConfigureAwait(false);
        }

        public async Task<IEnumerable<BudgetCeilingHeader>> ListAsync()
        {
            return await _context.BudgetCeilingHeaders.ToListAsync().ConfigureAwait(false);
        }

        public void Remove(BudgetCeilingHeader budgetCeilingHeader)
        {
            _context.BudgetCeilingHeaders.Remove(budgetCeilingHeader);
        }

        public void Update(BudgetCeilingHeader budgetCeilingHeader)
        {
            _context.BudgetCeilingHeaders.Update(budgetCeilingHeader);
        }

        public void Update(long ID, BudgetCeilingHeader budgetCeilingHeader)
        {
            _context.Entry(budgetCeilingHeader).State = EntityState.Modified;
        }
    }
}
