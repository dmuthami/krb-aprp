using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class WorkCategoryRepository : BaseRepository, IWorkCategoryRepository
    {
        public WorkCategoryRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }

        public async Task<IEnumerable<WorkCategory>> ListAsync()
        {
            return await _context.WorkCategories
                .OrderBy(o=>o.Code)
                .ToListAsync().ConfigureAwait(false);
        }
    }
}
