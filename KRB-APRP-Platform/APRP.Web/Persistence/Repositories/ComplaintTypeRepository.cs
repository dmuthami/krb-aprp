using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class ComplaintTypeRepository : BaseRepository, IComplaintTypeRepository
    {
        public ComplaintTypeRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }

        public async Task<IEnumerable<ComplaintType>> ListAsync()
        {
            return await _context.ComplaintTypes.ToListAsync().ConfigureAwait(false);
        }
    }
}
