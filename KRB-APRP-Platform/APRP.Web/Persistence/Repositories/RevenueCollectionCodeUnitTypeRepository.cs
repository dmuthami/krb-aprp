using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class RevenueCollectionCodeUnitTypeRepository : BaseRepository, IRevenueCollectionCodeUnitTypeRepository
    {
        public RevenueCollectionCodeUnitTypeRepository(AppDbContext context) 
            : base(context)
        {
            
        }

        public async Task AddAsync(RevenueCollectionCodeUnitType revenueCollectionCodeUnitType)
        {
            await _context.RevenueCollectionCodeUnitTypes.AddAsync(revenueCollectionCodeUnitType).ConfigureAwait(false);
        }

        public async Task DetachFirstEntryAsync(RevenueCollectionCodeUnitType revenueCollectionCodeUnitType)
        {
            _context.Entry(revenueCollectionCodeUnitType).State = EntityState.Detached;
        }

        public async Task<RevenueCollectionCodeUnitType> FindByIdAsync(long ID)
        {
            return await _context.RevenueCollectionCodeUnitTypes
            .FirstOrDefaultAsync(m => m.ID == ID).ConfigureAwait(false);
        }

        public async Task<RevenueCollectionCodeUnitType> FindByNameAsync(string Type)
        {
            return await _context.RevenueCollectionCodeUnitTypes
            .FirstOrDefaultAsync(m => m.Type.ToLower() == Type.ToLower()).ConfigureAwait(false);
        }

        public async Task<IEnumerable<RevenueCollectionCodeUnitType>> ListAsync()
        {
           return await _context.RevenueCollectionCodeUnitTypes
                 .ToListAsync().ConfigureAwait(false);
        }

        public void Remove(RevenueCollectionCodeUnitType revenueCollectionCodeUnitType)
        {
            _context.RevenueCollectionCodeUnitTypes.Remove(revenueCollectionCodeUnitType);
        }

        public void Update(RevenueCollectionCodeUnitType revenueCollectionCodeUnitType)
        {
            _context.RevenueCollectionCodeUnitTypes.Update(revenueCollectionCodeUnitType);
        }

        public void Update(long ID, RevenueCollectionCodeUnitType revenueCollectionCodeUnitType)
        {
            _context.Entry(revenueCollectionCodeUnitType).State = EntityState.Modified;
        }
    }
}
