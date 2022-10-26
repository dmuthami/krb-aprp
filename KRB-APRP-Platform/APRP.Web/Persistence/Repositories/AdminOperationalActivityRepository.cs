using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class AdminOperationalActivityRepository : BaseRepository, IAdminOperationalActivityRepository
    {

        public AdminOperationalActivityRepository(AppDbContext appDbContext) : base(appDbContext) { }


        public async Task AddAsync(AdminOperationalActivity adminOperationalActivity)
        {
            await _context.AdminOperationalActivities.AddAsync(adminOperationalActivity).ConfigureAwait(false);
        }

        public async Task<AdminOperationalActivity> FindByIdAsync(long ID)
        {
            return await _context.AdminOperationalActivities.Where(a => a.ID == ID)
                .SingleOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<AdminOperationalActivity>> ListByAuthorityAsync(long authorityId, long financialYearId)
        {
            return await _context.AdminOperationalActivities
                .Where(a => a.AuthorityId  == authorityId && a.FinancialYearId == financialYearId)
                .Include(a => a.Authority)
                .Include(f => f.FinancialYear)
                .ToListAsync().ConfigureAwait(false);
        }

        public void Remove(AdminOperationalActivity adminOperationalActivity)
        {
            _context.AdminOperationalActivities.Remove(adminOperationalActivity);
        }

        public void Update(AdminOperationalActivity adminOperationalActivity)
        {
            _context.AdminOperationalActivities.Update(adminOperationalActivity);
        }
    }
}
