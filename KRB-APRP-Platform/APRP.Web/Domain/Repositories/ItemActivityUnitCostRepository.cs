using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class ItemActivityUnitCostRepository : BaseRepository , IItemActivityUnitCostRepository
    {
        public ItemActivityUnitCostRepository(AppDbContext context) : base(context)
        {

        }

        public async Task AddAsync(ItemActivityUnitCost itemActivityUnitCost)
        {
            await _context.ItemActivityUnitCosts.AddAsync(itemActivityUnitCost).ConfigureAwait(false);
        }

        public async Task<ItemActivityUnitCost> FindByIdAsync(long ID)
        {
            return await _context.ItemActivityUnitCosts
                .Include(r=>r.ItemActivityUnitCostRates).ThenInclude(a=>a.Authority)
                .Include(i => i.ItemActivityGroup)
               .FirstOrDefaultAsync(m => m.ID == ID).ConfigureAwait(false);
        }
        public async Task<ItemActivityUnitCost> FindByActivityCodeAsync(string itemCode)
        {
            return await _context.ItemActivityUnitCosts
                .Include(r => r.ItemActivityUnitCostRates).ThenInclude(a => a.Authority)
                .Include(i => i.ItemActivityGroup)
#pragma warning disable CA1304 // Specify CultureInfo
#pragma warning disable CA1307 // Specify StringComparison
               .FirstOrDefaultAsync(m => (m.ItemCode + '-' + m.SubItemCode + '-' + m.SubSubItemCode).ToLower().Equals(itemCode.ToLower())).ConfigureAwait(false);
#pragma warning restore CA1307 // Specify StringComparison
#pragma warning restore CA1304 // Specify CultureInfo
        }

        public async Task<IEnumerable<ItemActivityUnitCost>> ListAsync()
        {
            return await _context.ItemActivityUnitCosts.ToListAsync().ConfigureAwait(false);
        }


        public void Remove(ItemActivityUnitCost itemActivityUnitCost)
        {
            _context.ItemActivityUnitCosts.Remove(itemActivityUnitCost);
        }

        public void Update(ItemActivityUnitCost itemActivityUnitCost)
        {
            _context.ItemActivityUnitCosts.Update(itemActivityUnitCost);
        }

        public void Update(long ID, ItemActivityUnitCost itemActivityUnitCost)
        {
            _context.Entry(itemActivityUnitCost).State = EntityState.Modified;
        }

        public async Task<ItemActivityUnitCost> FindByCodeAsync(string Code)
        {
            return await _context.ItemActivityUnitCosts
              .Include(r => r.ItemActivityUnitCostRates).ThenInclude(a => a.Authority)
              .Include(i => i.ItemActivityGroup)
               .FirstOrDefaultAsync(m => (m.ItemCode + '.' + m.SubItemCode + '.' + m.SubSubItemCode).ToLower().Equals(Code.ToLower())).ConfigureAwait(false);

        }
    }
}
