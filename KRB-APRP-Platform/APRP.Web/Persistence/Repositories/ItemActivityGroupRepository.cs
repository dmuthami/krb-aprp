using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class ItemActivityGroupRepository : BaseRepository , IItemActivityGroupRepository
    {
        public ItemActivityGroupRepository(AppDbContext context) : base(context)
        {

        }

        public async Task AddAsync(ItemActivityGroup itemActivityGroup)
        {
            await _context.ItemActivityGroups.AddAsync(itemActivityGroup);
        }

        public async Task<ItemActivityGroup> FindByIdAsync(long ID)
        {
            return await _context.ItemActivityGroups.FindAsync(ID);
        }

        public async Task<IEnumerable<ItemActivityGroup>> ListAsync()
        {
            return await _context.ItemActivityGroups.ToListAsync().ConfigureAwait(false);
        }


        public void Remove(ItemActivityGroup itemActivityGroup)
        {
            _context.ItemActivityGroups.Remove(itemActivityGroup);
        }

        public void Update(ItemActivityGroup itemActivityGroup)
        {
            _context.ItemActivityGroups.Update(itemActivityGroup);
        }

        public void Update(long ID, ItemActivityGroup itemActivityGroup)
        {
            _context.Entry(itemActivityGroup).State = EntityState.Modified;
        }
    }
}
