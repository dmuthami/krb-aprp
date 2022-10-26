using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class CommentRepository : BaseRepository, ICommentRepository
    {
        public CommentRepository(AppDbContext context) : base(context)
        {

        }

        public async Task AddAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment).ConfigureAwait(false);
        }

        public async Task<Comment> FindByIdAsync(long ID)
        {
            return await _context.Comments.FindAsync(ID).ConfigureAwait(false); 
        }

        public async Task<IEnumerable<Comment>> ListAsync()
        {
            return await _context.Comments
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<Comment>> ListAsync(string Type, long ForeignId)
        {
            return await _context.Comments
                .Where(u => u.Type == Type && u.ForeignId == ForeignId)
                .OrderByDescending(y=>y.CreationDate)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public void Remove(Comment comment)
        {
            _context.Comments.Remove(comment);
        }

        public void Update(Comment comment)
        {
            _context.Comments.Update(comment);
        }
    }
}
