using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IUploadService
    {
        Task<UploadListResponse> ListAsync();
        Task<UploadListResponse> ListAsync(string Type,long ForeignId);
        Task<UploadResponse> AddAsync(Upload upload);
        Task<UploadResponse> FindByIdAsync(long ID);
        Task<UploadResponse> Update(Upload upload);
        Task<UploadResponse> RemoveAsync(long ID);
    }
}
