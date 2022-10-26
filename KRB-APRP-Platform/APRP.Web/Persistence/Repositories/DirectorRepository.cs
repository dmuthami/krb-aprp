using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class DirectorRepository : BaseRepository, IDirectorRepository
    {

        public DirectorRepository(AppDbContext appDbContext) : base(appDbContext) { }


        public async Task AddAsync(Director director)
        {
            await _context.Directors.AddAsync(director);
        }

        public async Task<Director> FindByIdAsync(long ID)
        {
            return await _context.Directors.FindAsync(ID).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Director>> ListAsync()
        {
            return await _context.Directors.ToListAsync().ConfigureAwait(false);
        }
        public void Remove(Director director)
        {
            _context.Directors.Remove(director);
        }

        public void Update(Director director)
        {
            _context.Directors.Update(director);
        }
    }
}
