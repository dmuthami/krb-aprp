using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class RevenueCollectionRepository : BaseRepository, IRevenueCollectionRepository
    {
        public RevenueCollectionRepository(AppDbContext context)
            : base(context)
        {

        }

        public async Task AddAsync(RevenueCollection revenueCollection)
        {
            await _context.RevenueCollections.AddAsync(revenueCollection).ConfigureAwait(false);
        }

        public async Task<RevenueCollection> FindByIdAsync(long ID)
        {
            return await _context.RevenueCollections
            .Include(r => r.RevenueCollectionCodeUnit)
                .ThenInclude(r => r.FinancialYear)
            .FirstOrDefaultAsync(m => m.ID == ID).ConfigureAwait(false);
        }

        public async Task<RevenueCollection> FindByRevenueCollectionCodeUnitIdAsync(long RevenueCollectionCodeUnitId)
        {
            return await _context.RevenueCollections
            .Include(r => r.RevenueCollectionCodeUnit)
                .ThenInclude(r => r.FinancialYear)
            .FirstOrDefaultAsync(m => m.RevenueCollectionCodeUnitId == RevenueCollectionCodeUnitId).ConfigureAwait(false);
        }

        public async Task<RevenueCollection> FindByRevenueStreamAndFinancialYearAsync(long FinancialYearID, RevenueStream RevenueStream)
        {
            return await _context.RevenueCollections
            .Include(r => r.RevenueCollectionCodeUnit)
                .ThenInclude(r => r.FinancialYear)
                .FirstOrDefaultAsync(m => m.RevenueCollectionCodeUnit.FinancialYearId == FinancialYearID &&
                m.RevenueCollectionCodeUnit.RevenueStream== RevenueStream).ConfigureAwait(false);
        }

        public async Task<IEnumerable<RevenueCollection>> ListAsync()
        {
            return await _context.RevenueCollections
                .Include(r => r.RevenueCollectionCodeUnit)
                      .ThenInclude(r => r.FinancialYear)
                .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<RevenueCollection>> ListAsync(long FinancialYearId, string Type)
        {
            if (Type == null)
            {
                return await _context.RevenueCollections
                    .Where(w => w.RevenueCollectionCodeUnit.FinancialYearId == FinancialYearId)
                          .Include(r => r.RevenueCollectionCodeUnit)
                                .ThenInclude(r => r.FinancialYear)
                           .Include(r => r.RevenueCollectionCodeUnit)
                                .ThenInclude(rt => rt.RevenueCollectionCodeUnitType)
                          .ToListAsync()
                          .ConfigureAwait(false);
            }
            else
            {
                return await _context.RevenueCollections
                    .Where(w => w.RevenueCollectionCodeUnit.FinancialYearId == FinancialYearId 
                    && w.RevenueCollectionCodeUnit.RevenueCollectionCodeUnitType.Type==Type)
                          .Include(r => r.RevenueCollectionCodeUnit)
                                .ThenInclude(r => r.FinancialYear)
                           .Include(r => r.RevenueCollectionCodeUnit)
                                .ThenInclude(rt => rt.RevenueCollectionCodeUnitType)
                          .ToListAsync()
                          .ConfigureAwait(false);
            }

        }

        public void Remove(RevenueCollection revenueCollection)
        {
            _context.RevenueCollections.Remove(revenueCollection);
        }

        public void Update(RevenueCollection revenueCollection)
        {
            _context.RevenueCollections.Update(revenueCollection);
        }

        public void Update(long ID, RevenueCollection revenueCollection)
        {
            _context.Entry(revenueCollection).State = EntityState.Modified;
        }
    }
}
