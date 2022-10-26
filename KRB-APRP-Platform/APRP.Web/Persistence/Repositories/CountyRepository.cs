using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class CountyRepository : BaseRepository, ICountyRepository
    {

        public CountyRepository(AppDbContext appDbContext) : base(appDbContext) { }


        public async Task AddAsync(County county)
        {
            await _context.Counties.AddAsync(county);
        }

        public async Task<County> FindByIdAsync(long ID)
        {
            return await _context.Counties.FindAsync(ID);
        }

        public async Task<IEnumerable<County>> ListAsync()
        {
            return await _context.Counties.ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<County>> ListByNameAsync(string CountyName)
        {
            return await _context.Counties.Where(c => c.Name.Contains(CountyName)).ToListAsync().ConfigureAwait(false);
        }

        public void Remove(County county)
        {
            _context.Counties.Remove(county);
        }

        public void Update(County county)
        {
            _context.Counties.Update(county);
        }
    }
}
