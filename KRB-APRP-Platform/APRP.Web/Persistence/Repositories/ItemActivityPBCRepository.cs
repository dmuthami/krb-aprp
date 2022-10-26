using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class ItemActivityPBCRepository : BaseRepository, IItemActivityPBCRepository
    {
        public ItemActivityPBCRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }

        public async Task<IEnumerable<ItemActivityPBC>> ListAsync()
        {
            return await _context.ItemActivityPBCs.Include(t=>t.Technology).ToListAsync().ConfigureAwait(false);
        }
    }
}
