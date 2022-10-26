using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IWorkplanUploadService
    {
        Task<IEnumerable<WorkplanUpload>> ListAsync();

        Task<WorkplanUploadResponse> AddAsync(WorkplanUpload workplanUpload);
        Task<WorkplanUploadResponse> FindByIdAsync(long ID);
        Task<IEnumerable<WorkplanUpload>> FindByFinancialYearAsync(long financialYearId, long authorityId);
        Task<WorkplanUploadResponse> UpdateAsync(WorkplanUpload workplanUpload);
        Task<WorkplanUploadResponse> RemoveAsync(long ID);
    }
}
