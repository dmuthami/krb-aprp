using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class TechnologyRepository : BaseRepository, ITechnologyRepository
    {
        public TechnologyRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }

        public async Task<IEnumerable<Technology>> ListAsync()
        {
            return await _context.Technologies.ToListAsync().ConfigureAwait(false);
        }
    }
}
