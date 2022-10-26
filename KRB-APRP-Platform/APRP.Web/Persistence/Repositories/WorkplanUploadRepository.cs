using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class WorkplanUploadRepository : BaseRepository, IWorkplanUploadRepository
    {

        public WorkplanUploadRepository(AppDbContext appDbContext) : base(appDbContext) { }


        public async Task AddAsync(WorkplanUpload workplanUpload)
        {
            await _context.WorkplanUploads.AddAsync(workplanUpload).ConfigureAwait(false);
        }

        public async Task<WorkplanUpload> FindByIdAsync(long ID)
        {
            return await _context.WorkplanUploads.Where(a => a.ID == ID)
                .Include(a=>a.Authority)
                .Include(f=>f.FinancialYear)
                .Include(r=>r.WorkplanUploadSections).ThenInclude(s=>s.WorkplanUploadSectionActivities)
                .SingleOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<WorkplanUpload> FindByIdSimpleAsync(long ID)
        {
            return await _context.WorkplanUploads.FindAsync(ID).ConfigureAwait(false);
        }

        public async Task<IEnumerable<WorkplanUpload>> FindByFinancialYearAsync(long financialYearId, long authorityId)
        {
            return await _context.WorkplanUploads
                .Where(a => a.FinancialYearId == financialYearId && a.AuthorityId  == authorityId)
                .Include(a => a.Authority)
                .Include(f => f.FinancialYear)
                .Include(r => r.WorkplanUploadSections).ThenInclude(s => s.WorkplanUploadSectionActivities)
                .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<WorkplanUpload>> ListAsync()
        {
            return await _context.WorkplanUploads
                .Include(a => a.Authority)
                .Include(f => f.FinancialYear)
                .Include(r => r.WorkplanUploadSections).ThenInclude(s => s.WorkplanUploadSectionActivities)
                .ToListAsync().ConfigureAwait(false);
        }

        public void Remove(WorkplanUpload workplanUpload)
        {
            _context.WorkplanUploads.Remove(workplanUpload);
        }

        public void Update(WorkplanUpload workplanUpload)
        {
            _context.WorkplanUploads.Update(workplanUpload);
        }
    }
}
