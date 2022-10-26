using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class RoadWorkBudgetHeaderRepository : BaseRepository , IRoadWorkBudgetHeaderRepository
    {

        public RoadWorkBudgetHeaderRepository(AppDbContext context) : base(context)
        {

        }

        public async Task AddAsync(RoadWorkBudgetHeader roadWorkBudgetHeader)
        {
            await _context.RoadWorkBudgetHeaders.AddAsync(roadWorkBudgetHeader);
        }

        public async Task<RoadWorkBudgetHeader> FindByAuthorityIdForCurrentYear(long yearId, long authorityId)
        {
            return await _context.RoadWorkBudgetHeaders
                .Include(a => a.Authority)
                .Include(l=>l.RoadWorkBudgetLines).ThenInclude(fs=>fs.FundingSource)
                .Include(l => l.RoadWorkBudgetLines).ThenInclude(ft => ft.FundType)
                .Include(op=>op.RoadWorkOperationalActivitiesBudgets).ThenInclude(fs => fs.FundingSource)
                .Include(op => op.RoadWorkOperationalActivitiesBudgets).ThenInclude(ft => ft.FundType)
                .FirstOrDefaultAsync(y => y.FinancialYearId == yearId && y.Authority.ID == authorityId).ConfigureAwait(false);
        }

        public async Task<RoadWorkBudgetHeader> FindByFinancialYearIdAndAuthorityID(long financialYearId, long authorityID)
        {
            return await _context.RoadWorkBudgetHeaders
                .Include(h => h.RoadWorkBudgetLines)
                .FirstOrDefaultAsync(c => c.FinancialYearId == financialYearId && c.Authority.ID == authorityID)
                .ConfigureAwait(false);
        }

        public async Task<RoadWorkBudgetHeader> FindByIdAsync(long ID)
        {
            return await _context.RoadWorkBudgetHeaders.FindAsync(ID);
        }

        public async Task<IEnumerable<RoadWorkBudgetHeader>> ListAsync()
        {
            return await _context.RoadWorkBudgetHeaders.ToListAsync().ConfigureAwait(false);
        }

        public void Remove(RoadWorkBudgetHeader roadWorkBudgetHeader)
        {
            _context.RoadWorkBudgetHeaders.Remove(roadWorkBudgetHeader);
        }

        public void Update(RoadWorkBudgetHeader roadWorkBudgetHeader)
        {
            _context.RoadWorkBudgetHeaders.Update(roadWorkBudgetHeader);
        }
    }
}
