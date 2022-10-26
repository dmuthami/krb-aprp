using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class ContractorRepository : BaseRepository, IContractorRepository
    {

        public ContractorRepository(AppDbContext appDbContext) : base(appDbContext) { }


        public async Task AddAsync(Contractor contractor)
        {
            await _context.Contractors.AddAsync(contractor);
        }

        public async Task<Contractor> FindByIdAsync(long ID)
        {
            // return await _context.Contractors.FindAsync(ID).ConfigureAwait(false);
            return await _context.Contractors.Include(d=>d.Directors).Where(c => c.ID == ID).SingleOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<Contractor> FindByKraPinAsync(string kraPin)
        {
            return await _context.Contractors.Where(c => c.KRAPin == kraPin).SingleOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Contractor>> ListAsync()
        {
            return await _context.Contractors.ToListAsync().ConfigureAwait(false);
        }
        public void Remove(Contractor contractor)
        {
            _context.Contractors.Remove(contractor);
        }

        public void Update(Contractor contractor)
        {
            _context.Contractors.Update(contractor);
        }
    }
}
