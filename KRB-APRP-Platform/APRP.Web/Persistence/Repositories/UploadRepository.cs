using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class UploadRepository : BaseRepository, IUploadRepository
    {
        public UploadRepository(AppDbContext context) : base(context)
        {

        }

        public async Task AddAsync(Upload upload)
        {
            await _context.Uploads.AddAsync(upload);
        }

        public async Task<Upload> FindByIdAsync(long ID)
        {
            return await _context.Uploads.FindAsync(ID); 
        }

        public async Task<IEnumerable<Upload>> ListAsync()
        {
            return await _context.Uploads
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<Upload>> ListAsync(string Type, long ForeignId)
        {
            return await _context.Uploads
                .Where(u=>u.type==Type && u.ForeignId== ForeignId)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public void Remove(Upload upload)
        {
            _context.Uploads.Remove(upload);
        }

        public void Update(Upload upload)
        {
            _context.Uploads.Update(upload);
        }
    }
}
