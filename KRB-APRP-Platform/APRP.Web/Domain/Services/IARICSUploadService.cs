using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IARICSUploadService
    {
        Task<ARICSUploadListResponse> ListAsync();
        Task<ARICSUploadResponse> AddAsync(ARICSUpload aRICSUpload);
        Task<ARICSUploadResponse> FindByIdAsync(long ID);
        Task<ARICSUploadResponse> Update(ARICSUpload aRICSUpload);
        Task<ARICSUploadResponse> RemoveAsync(long ID);
    }
}
