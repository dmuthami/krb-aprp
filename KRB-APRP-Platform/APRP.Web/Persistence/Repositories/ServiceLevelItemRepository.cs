using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class ServiceLevelItemRepository : BaseRepository, IServiceLevelItemRepository
    {
        public ServiceLevelItemRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }

        public async Task<IEnumerable<ServiceLevelItem>> ListAsync()
        {
            return await _context.ServiceLevelItems.Include(g=>g.ServiceLevelGroup).ToListAsync().ConfigureAwait(false);
        }
    }
}
