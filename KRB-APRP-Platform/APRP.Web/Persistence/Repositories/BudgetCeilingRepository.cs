using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class BudgetCeilingRepository : BaseRepository, IBudgetCeilingRepository
    {

        public BudgetCeilingRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }

        public async Task AddAsync(BudgetCeiling budgetCeiling)
        {
            await _context.BudgetCeilings.AddAsync(budgetCeiling).ConfigureAwait(false);
        }

        public async Task<BudgetCeiling> FindApprovedByAuthorityForCurrentYear(long financialYear, long authorityID)
        {
            return await _context.BudgetCeilings.Include(h => h.RoadWorkBudgetHeader).FirstOrDefaultAsync(c => c.AuthorityId == authorityID && c.RoadWorkBudgetHeader.FinancialYearId == financialYear && c.RoadWorkBudgetHeader.ApprovalStatus == 2).ConfigureAwait(false);
        }

        public async Task<BudgetCeiling> FindByAuthorityIDAndFinancialYearID(long AuthorityID, long FinancialYearID)
        {
            return await _context.BudgetCeilings
                .Where(c => c.AuthorityId == AuthorityID && c.RoadWorkBudgetHeader.FinancialYearId == FinancialYearID)
                .Include(h => h.RoadWorkBudgetHeader)
                    .ThenInclude(f => f.FinancialYear)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
        }

        public async Task<BudgetCeiling> FindByBudgetHeaderIDAndAuthority(long budgetHeaderID, long authorityID)
        {
            return await _context.BudgetCeilings
                .Include(h => h.RoadWorkBudgetHeader)
                .FirstOrDefaultAsync(c => c.AuthorityId == authorityID && c.BudgetCeilingHeaderId == budgetHeaderID)
                .ConfigureAwait(false);
        }

        public async Task<BudgetCeiling> FindByIdAsync(long ID)
        {
            //return await _context.RoadWorkSectionPlans.Include(s=> s.RoadSection).FirstOrDefaultAsync(s=> s.ID == ID);
            return await _context.BudgetCeilings
                .Include(a=>a.Authority)
                .FirstOrDefaultAsync(b=>b.ID==ID).ConfigureAwait(false);
        }

        public async Task<IEnumerable<BudgetCeiling>> GetCGsByBudgetCeilingHeaderAsync(long BudgetCeilingHeaderID)
        {
            return await _context.BudgetCeilings
            .Include(a => a.Authority)
            .Include(h => h.RoadWorkBudgetHeader).Where(x=>x.BudgetCeilingHeaderId== BudgetCeilingHeaderID)

            .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<BudgetCeiling>> ListAsync()
        {
            return await _context.BudgetCeilings
                .Include(a => a.Authority)
                .Include(h => h.RoadWorkBudgetHeader)
                .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<BudgetCeiling>> ListAsync(string AuthorityType, long FinancialYearID)
        {
            if (AuthorityType == null)
            {
                return await _context.BudgetCeilings
                .Include(a => a.Authority)
                .Include(h => h.RoadWorkBudgetHeader)
                .ToListAsync().ConfigureAwait(false);
            }else
            {
                if (AuthorityType == "RA")
                {
                    return await _context.BudgetCeilings
                    .Include(a => a.Authority)
                    .Include(h => h.RoadWorkBudgetHeader)
                    .Where(x => (x.Authority.Type == 1 || x.Authority.Type == 4 || x.Authority.Type == 0) && x.RoadWorkBudgetHeader.FinancialYearId== FinancialYearID)
                    .ToListAsync().ConfigureAwait(false);
                }
                else
                {
                    return await _context.BudgetCeilings
                    .Include(a => a.Authority)
                    .Include(h => h.RoadWorkBudgetHeader)
                    .Where(x =>( x.Authority.Type == 2) && x.RoadWorkBudgetHeader.FinancialYearId == FinancialYearID)
                    .ToListAsync().ConfigureAwait(false);
                }
            }

        }
        public void Remove(BudgetCeiling budgetCeiling)
        {
            _context.BudgetCeilings.Remove(budgetCeiling);
        }

        public async Task<IEnumerable<BudgetCeiling>> RemoveAllAsync(long? BudgetCeilingHeaderID)
        {
            if (BudgetCeilingHeaderID == null)
            {
                var budgetCeilings = await _context.BudgetCeilings
                    .ToListAsync().ConfigureAwait(false);

                _context.BudgetCeilings.RemoveRange(budgetCeilings);
                return budgetCeilings;
            }
            else
            {
                long _BudgetCeilingHeaderID = 0;
                bool result = long.TryParse(BudgetCeilingHeaderID.ToString(), out _BudgetCeilingHeaderID);
                var budgetCeilings = await _context.BudgetCeilings
                    .Where(f => f.BudgetCeilingHeaderId == _BudgetCeilingHeaderID)
                .ToListAsync().ConfigureAwait(false);

                _context.BudgetCeilings.RemoveRange(budgetCeilings);
                return budgetCeilings;
            }



        }

        public void Update(BudgetCeiling budgetCeiling)
        {
            _context.BudgetCeilings.Update(budgetCeiling);
        }

        public void Update(long ID, BudgetCeiling budgetCeiling)
        {
            _context.Entry(budgetCeiling).State = EntityState.Modified;
        }
    }
}
