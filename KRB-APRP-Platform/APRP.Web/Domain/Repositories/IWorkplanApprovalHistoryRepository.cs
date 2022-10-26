using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IWorkplanApprovalHistoryRepository
    {
        Task AddAsync(WorkplanApprovalHistory workplanApprovalHistory);
    }
}
