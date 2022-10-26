using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class QuarterCodeUnitRepository : BaseRepository, IQuarterCodeUnitRepository
    {
        public QuarterCodeUnitRepository(AppDbContext context)
            : base(context)
        {

        }

        public async Task AddAsync(QuarterCodeUnit quarterCodeUnit)
        {
            await _context.QuarterCodeUnits.AddAsync(quarterCodeUnit);
        }


        public async Task<QuarterCodeUnit> FindByIdAsync(long ID)
        {
            return await _context.QuarterCodeUnits
                .Include(s => s.FinancialYear)
                .Include(q => q.QuarterCodeList)
                .FirstOrDefaultAsync(m => m.ID == ID).ConfigureAwait(false);
        }

        public async Task<QuarterCodeUnit> FindByQuarterCodeListIdAndFinancialIdAsync(long QuarterCodeListId, long FinancialYearId)
        {
            return await _context.QuarterCodeUnits
            .Include(s => s.FinancialYear)
            .Include(q => q.QuarterCodeList)
            .FirstOrDefaultAsync(m => m.QuarterCodeListId == QuarterCodeListId && m.FinancialYearId== FinancialYearId).ConfigureAwait(false);
        }

        public async Task<IEnumerable<QuarterCodeUnit>> ListAsync()
        {
            return await _context.QuarterCodeUnits
                     .Include(s => s.FinancialYear)
                     .Include(q=>q.QuarterCodeList)
                     .OrderByDescending(x => x.FinancialYear.IsCurrent)
                     .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<QuarterCodeUnit>> ListAsync(long FinancialYearId)
        {
            return await _context.QuarterCodeUnits
                        .Where(w=>w.FinancialYearId==FinancialYearId)
                        .Include(s => s.FinancialYear)
                        .Include(q => q.QuarterCodeList)
                        .ToListAsync().ConfigureAwait(false);
        }

        public void Remove(QuarterCodeUnit quarterCodeUnit)
        {
            _context.QuarterCodeUnits.Remove(quarterCodeUnit);
        }

        public void Update(QuarterCodeUnit quarterCodeUnit)
        {
            _context.QuarterCodeUnits.Update(quarterCodeUnit);
        }

        public void Update(long ID, QuarterCodeUnit quarterCodeUnit)
        {
            _context.Entry(quarterCodeUnit).State = EntityState.Modified;
        }
    }
}
