using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class RevenueCollectionCodeUnitRepository : BaseRepository, IRevenueCollectionCodeUnitRepository
    {
        public RevenueCollectionCodeUnitRepository(AppDbContext context)
            : base(context)
        {

        }

        public async Task AddAsync(RevenueCollectionCodeUnit revenueCollectionCodeUnit)
        {
            await _context.RevenueCollectionCodeUnits.AddAsync(revenueCollectionCodeUnit).ConfigureAwait(false);
        }

        public async Task<RevenueCollectionCodeUnit> FindByIdAsync(long ID)
        {
            return await _context.RevenueCollectionCodeUnits
                .Include(a => a.Authority)
                .Include(f => f.FinancialYear)
                .Include(r => r.RevenueCollectionCodeUnitType)
                .FirstOrDefaultAsync(m => m.ID == ID).ConfigureAwait(false);
        }

        public async Task<RevenueCollectionCodeUnit> FindByNameAsync(RevenueStream RevenueStream)
        {
            return await _context.RevenueCollectionCodeUnits
                .Include(a => a.Authority)
                .Include(f => f.FinancialYear)
                .Include(r => r.RevenueCollectionCodeUnitType)
                .FirstOrDefaultAsync(/**/m => m.RevenueStream == 0).ConfigureAwait(false);
        }

        public async Task<RevenueCollectionCodeUnit> FindByNameAsync(RevenueStream RevenueStream, long FinancialYearId)
        {
            return await _context.RevenueCollectionCodeUnits
               .Include(a => a.Authority)
               .Include(f => f.FinancialYear)
               .Include(r => r.RevenueCollectionCodeUnitType)
               .FirstOrDefaultAsync(/**/m => m.RevenueStream== 0
                && m.FinancialYearId == FinancialYearId
               ).ConfigureAwait(false);
        }

        public async Task<RevenueCollectionCodeUnit> FindByNameAsync(RevenueStream RevenueStream, long FinancialYearId, long AuthorityId)
        {
            return await _context.RevenueCollectionCodeUnits
           .Include(a => a.Authority)
           .Include(f => f.FinancialYear)
           .Include(r => r.RevenueCollectionCodeUnitType)
           .FirstOrDefaultAsync(/* */m => m.RevenueStream == 0
            && m.FinancialYearId == FinancialYearId && m.AuthorityId == AuthorityId
          ).ConfigureAwait(false);
        }

        public async Task<IEnumerable<RevenueCollectionCodeUnit>> ListAsync(long? AuthorityId)
        {
            long _AuthorityId;
            bool result = long.TryParse(AuthorityId.ToString(), out _AuthorityId);
            if (AuthorityId == null)
            {
                return await _context.RevenueCollectionCodeUnits
                     .Include(a => a.Authority)
                     .Include(f => f.FinancialYear)
                         .Include(r => r.RevenueCollectionCodeUnitType)
                         .Include(rc => rc.RevenueCollection)
                     .OrderByDescending(t => t.FinancialYear.IsCurrent)
                     .ToListAsync().ConfigureAwait(false);
            }
            else
            {
                return await _context.RevenueCollectionCodeUnits
                     .Where(w => (w.AuthorityId == _AuthorityId || w.Authority.Code == "KRB"))
                     .Include(a => a.Authority)
                     .Include(f => f.FinancialYear)
                         .Include(r => r.RevenueCollectionCodeUnitType)
                         .Include(rc => rc.RevenueCollection)
                     .OrderByDescending(t => t.FinancialYear.IsCurrent)
                     .ToListAsync().ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<RevenueCollectionCodeUnit>> ListAsync(long FinancialYearId, string Type)
        {
            if (Type == null)
            {
                return await _context.RevenueCollectionCodeUnits
                      .Where(w => w.FinancialYear.ID == FinancialYearId)
                      .Include(a => a.Authority)
                      .Include(f => f.FinancialYear)
                       .Include(r => r.RevenueCollectionCodeUnitType)
                       .Include(rc => rc.RevenueCollection)
                      .ToListAsync().ConfigureAwait(false);
            }
            else
            {
                return await _context.RevenueCollectionCodeUnits
                        .Where(w => w.FinancialYear.ID == FinancialYearId && w.RevenueCollectionCodeUnitType.Type == Type)
                        .Include(a => a.Authority)
                        .Include(f => f.FinancialYear)
                        .Include(r => r.RevenueCollectionCodeUnitType)
                        .Include(rc => rc.RevenueCollection)
                        .ToListAsync().ConfigureAwait(false);
            }

        }

        public void Remove(RevenueCollectionCodeUnit revenueCollectionCodeUnit)
        {
            _context.RevenueCollectionCodeUnits.Remove(revenueCollectionCodeUnit);
        }

        public void Update(RevenueCollectionCodeUnit revenueCollectionCodeUnit)
        {
            _context.RevenueCollectionCodeUnits.Update(revenueCollectionCodeUnit);
        }

        public void Update(long ID, RevenueCollectionCodeUnit revenueCollectionCodeUnit)
        {
            _context.Entry(revenueCollectionCodeUnit).State = EntityState.Modified;
        }
    }
}
