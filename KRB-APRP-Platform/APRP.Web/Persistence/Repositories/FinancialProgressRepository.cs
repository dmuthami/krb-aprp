using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class FinancialProgressRepository : BaseRepository, IFinancialProgressRepository
    {

        public FinancialProgressRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }

        public async Task AddAsync(FinancialProgress financialProgress)
        {
            await _context.FinancialProgress.AddAsync(financialProgress).ConfigureAwait(false);
        }

        public async Task<FinancialProgress> FindByIdAsync(long ID)
        {
            return await _context.FinancialProgress
                .Include(a => a.Authority)
                .Include(f=>f.FinancialYear)
                .Include(q=>q.QuarterCodeList)
                .FirstOrDefaultAsync(i => i.ID == ID)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<FinancialProgress>> ListByAuthorityIdAndFinancialYearAsync(long authorityId, long financialYearId)
        {
            return await _context.FinancialProgress.Where(a => a.Authority.ID == authorityId && a.FinancialYear.ID == financialYearId)
                .Include(a => a.Authority)
                .Include(f => f.FinancialYear)
                .Include(q => q.QuarterCodeList)
                .Include(m=>m.MonthCode)
                .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<FinancialProgress>> ListByAuthorityIdAsync(long authorityId)
        {
            return await _context.FinancialProgress.Where(a=>a.Authority.ID == authorityId)
                .Include(a => a.Authority)
                .Include(f=>f.FinancialYear)
                .Include(q => q.QuarterCodeList)
                .ToListAsync().ConfigureAwait(false);
        }

        public void Remove(FinancialProgress financialProgress)
        {
            _context.FinancialProgress.Remove(financialProgress);
        }

        public void Update(FinancialProgress financialProgress)
        {
            _context.FinancialProgress.Update(financialProgress);
        }
    }
}
