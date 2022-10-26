using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> ListAsync();

        Task<IEnumerable<Comment>> ListAsync(string Type, long ForeignId);

        Task AddAsync(Comment comment);
        Task<Comment> FindByIdAsync(long ID);
        void Update(Comment comment);
        void Remove(Comment comment);
    }
}
