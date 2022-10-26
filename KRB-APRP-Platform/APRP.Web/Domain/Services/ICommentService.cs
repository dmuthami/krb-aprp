using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface ICommentService
    {
        Task<CommentListResponse> ListAsync();
        Task<CommentListResponse> ListAsync(string Type,long ForeignId);
        Task<CommentResponse> AddAsync(Comment comment);
        Task<CommentResponse> FindByIdAsync(long ID);
        Task<CommentResponse> Update(Comment comment);
        Task<CommentResponse> RemoveAsync(long ID);
    }
}
