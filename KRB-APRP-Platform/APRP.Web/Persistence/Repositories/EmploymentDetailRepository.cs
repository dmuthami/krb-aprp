using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class EmploymentDetailRepository : BaseRepository, IEmploymentDetailRepository
    {
        public EmploymentDetailRepository(AppDbContext context) : base(context)
        {

        }

        public async Task AddAsync(EmploymentDetail employmentDetail)
        {
            await _context.EmploymentDetails.AddAsync(employmentDetail).ConfigureAwait(false);
        }

        public async Task<EmploymentDetail> FindByIdAsync(long ID)
        {
            return await _context.EmploymentDetails.Where(e => e.ID== ID).Include(c=>c.Contract).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<EmploymentDetail>> ListAsync()
        {
            return await _context.EmploymentDetails.ToListAsync().ConfigureAwait(false);
        }


        public void Remove(EmploymentDetail employmentDetail)
        {
            _context.EmploymentDetails.Remove(employmentDetail);
        }

        public void Update(EmploymentDetail employmentDetail)
        {
            _context.EmploymentDetails.Update(employmentDetail);
        }
    }
}
