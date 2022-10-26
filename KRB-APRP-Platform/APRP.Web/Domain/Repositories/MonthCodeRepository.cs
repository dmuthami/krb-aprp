using APRP.Web.Domain.Models;
using APRP.Web.Persistence.Contexts;
using APRP.Web.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Domain.Repositories
{
    public class MonthCodeRepository : BaseRepository, IMonthCodeRepository
    {
        public MonthCodeRepository(AppDbContext appDbContext) :base(appDbContext)
        {
                
        }
        public async Task<IEnumerable<MonthCode>> ListAsync()
        {
            return await _context.MonthCodes
                .OrderBy(o => o.ID)
                .ToListAsync().ConfigureAwait(false);
        }
    }
}
