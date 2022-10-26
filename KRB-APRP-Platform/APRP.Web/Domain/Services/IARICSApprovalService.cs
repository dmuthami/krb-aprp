using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IARICSApprovalService
    {
        Task<GenericResponse> ListAsync();
        Task<GenericResponse> AddAsync(ARICSApproval aRICSApproval);
        Task<GenericResponse> FindByIdAsync(long ID);
        Task<GenericResponse> FindByARICSMasterApprovalIdAsync(long ARICSMasterApprovalIdId);
        Task<GenericResponse> Update(ARICSApproval aRICSApproval);
        Task<GenericResponse> Update(long ID, ARICSApproval aRICSApproval);
        Task<GenericResponse> RemoveAsync(long ID);
        Task<GenericResponse> DetachFirstEntryAsync(ARICSApproval aRICSApproval);
        Task<GenericResponse> ListHistoryAsync(long ARICSApprovalId);
    }
}
