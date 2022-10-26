using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IWorkplanApprovalHistoryService
    {
        Task<WorkplanApprovalHistoryResponse> AddAsync(WorkplanApprovalHistory workplanApprovalHistory);
    }
}
