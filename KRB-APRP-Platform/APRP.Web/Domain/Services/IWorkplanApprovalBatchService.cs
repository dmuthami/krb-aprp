using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IWorkplanApprovalBatchService
    {
        Task<IEnumerable<WorkplanApprovalBatch>> ListAsync();

        Task<WorkplanApprovalBatchResponse> AddAsync(WorkplanApprovalBatch workplanApprovalBatch);
        Task<WorkplanApprovalBatchResponse> FindByIdAsync(long ID);
        Task<WorkplanApprovalBatchResponse> FindByFinancialYearAsync(long financialYearId, long authorityId);
        Task<WorkplanApprovalBatchResponse> UpdateAsync(WorkplanApprovalBatch workplanApprovalBatch);
        Task<WorkplanApprovalBatchResponse> RemoveAsync(long ID);
    }
}
