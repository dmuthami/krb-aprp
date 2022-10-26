using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class PackageProgressEntryRepository : BaseRepository, IPackageProgressEntryRepository
    {
        public PackageProgressEntryRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }

        public async Task AddAsync(PackageProgressEntry packageProgressEntry)
        {
            await _context.PackageProgressEntries.AddAsync(packageProgressEntry);
        }

        public async Task<IEnumerable<PackageProgressEntry>> ListAsync()
        {
            return await _context.PackageProgressEntries.ToListAsync().ConfigureAwait(false);
        }

    }
}
