using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class OtherFundItemRepository : BaseRepository, IOtherFundItemRepository
    {
        public OtherFundItemRepository(AppDbContext context) 
            : base(context)
        {
            
        }

        public async Task AddAsync(OtherFundItem otherFundItem)
        {
            await _context.OtherFundItems.AddAsync(otherFundItem);
        }

        public async Task<OtherFundItem> FindByFinancialIdAsync(long FinancialYearId)
        {
            return await _context.OtherFundItems
            .FirstOrDefaultAsync(m =>m.FinancialYearId==FinancialYearId).ConfigureAwait(false);
        }

        public async Task<OtherFundItem> FindByIdAsync(long ID)
        {
            return await _context.OtherFundItems
                .Include(f => f.FinancialYear)
                .FirstOrDefaultAsync(m => m.ID == ID).ConfigureAwait(false);
        }

        public async Task<OtherFundItem> FindByNameAsync(string Description)
        {
            return await _context.OtherFundItems
                .Include(f => f.FinancialYear)
                .FirstOrDefaultAsync(m => m.Description.ToLower() == Description.ToLower()).ConfigureAwait(false);
        }

        public async Task<IEnumerable<OtherFundItem>> ListAsync()
        {
           return await _context.OtherFundItems
                 .Include(f=>f.FinancialYear)
                 .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<OtherFundItem>> ListAsync(long FinancialYearId)
        {
            return await _context.OtherFundItems
                    .Where(w=>w.FinancialYearId== FinancialYearId)
                    .Include(f => f.FinancialYear)
                    .ToListAsync().ConfigureAwait(false);
        }

        public void Remove(OtherFundItem otherFundItem)
        {
            _context.OtherFundItems.Remove(otherFundItem);
        }

        public void Update(OtherFundItem otherFundItem)
        {
            _context.OtherFundItems.Update(otherFundItem);
        }

        public void Update(long ID, OtherFundItem otherFundItem)
        {
            _context.Entry(otherFundItem).State = EntityState.Modified;
        }
    }
}
