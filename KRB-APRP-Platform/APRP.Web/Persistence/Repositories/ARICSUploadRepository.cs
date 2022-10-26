using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class ARICSUploadRepository : BaseRepository, IARICSUploadRepository
    {
        public ARICSUploadRepository(AppDbContext context) : base(context)
        {

        }

        public async Task AddAsync(ARICSUpload aRICSUpload)
        {
            await _context.ARICSUploads.AddAsync(aRICSUpload).ConfigureAwait(false);
        }

        public async Task<ARICSUpload> FindByIdAsync(long ID)
        {
            return await _context.ARICSUploads.FindAsync(ID).ConfigureAwait(false); 
        }

        public async Task<IEnumerable<ARICSUpload>> ListAsync()
        {
            return await _context.ARICSUploads
                .Include(r=>r.RoadSection)
                .ThenInclude(r=>r.Road)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public void Remove(ARICSUpload aRICSUpload)
        {
            _context.ARICSUploads.Remove(aRICSUpload);
        }

        public void Update(ARICSUpload aRICSUpload)
        {
            _context.ARICSUploads.Update(aRICSUpload);
        }
    }
}
