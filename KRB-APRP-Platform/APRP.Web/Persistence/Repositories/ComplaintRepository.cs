using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class ComplaintRepository : BaseRepository, IComplaintRepository
    {

        public ComplaintRepository(AppDbContext appDbContext) : base(appDbContext) { }


        public async Task AddAsync(Complaint complaint)
        {
            await _context.Complaints.AddAsync(complaint);
        }

        public async Task<Complaint> FindByIdAsync(long ID)
        {
            return await _context.Complaints.FindAsync(ID);
        }

        public async Task<IEnumerable<Complaint>> ListAsync()
        {
            return await _context.Complaints.Include(t => t.ComplaintType).Include(a => a.Authority).ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Complaint>> ListUnresolvedAsyc()
        {
            return await _context.Complaints.Include(t => t.ComplaintType).Include(a=>a.Authority).Where(c => c.Status != 2).ToListAsync().ConfigureAwait(false); //0- open, 1-WIP, 2- Closed
        }

        public void Remove(Complaint complaint)
        {
            _context.Complaints.Remove(complaint);
        }

        public void Update(Complaint complaint)
        {
            _context.Complaints.Update(complaint);
        }
    }
}
