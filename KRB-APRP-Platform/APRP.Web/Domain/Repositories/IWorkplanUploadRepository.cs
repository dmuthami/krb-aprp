using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IWorkplanUploadRepository
    {
        Task<IEnumerable<WorkplanUpload>> ListAsync();
        Task AddAsync(WorkplanUpload workplanUpload);
        Task<WorkplanUpload> FindByIdAsync(long ID);
        Task<WorkplanUpload> FindByIdSimpleAsync(long ID);
        Task<IEnumerable<WorkplanUpload>> FindByFinancialYearAsync(long financialYearId, long authorityId);
        void Update(WorkplanUpload workplanUpload);
        void Remove(WorkplanUpload workplanUpload);
    }
}
