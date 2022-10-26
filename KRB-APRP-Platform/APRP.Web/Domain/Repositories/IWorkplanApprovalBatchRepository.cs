using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IWorkplanApprovalBatchRepository
    {
        Task<IEnumerable<WorkplanApprovalBatch>> ListAsync();
        Task AddAsync(WorkplanApprovalBatch workplanApproval);
        Task<WorkplanApprovalBatch> FindByIdAsync(long ID);
        Task<WorkplanApprovalBatch> FindByFinancialYearAsync(long financialYearId, long authorityId);
        void Update(WorkplanApprovalBatch workplanApproval);
        void Remove(WorkplanApprovalBatch workplanApproval);
    }
}
