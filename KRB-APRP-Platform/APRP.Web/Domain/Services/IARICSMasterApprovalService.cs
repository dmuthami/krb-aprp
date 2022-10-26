using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IARICSMasterApprovalService
    {
        Task<GenericResponse> ListAsync();
        Task<GenericResponse> ListByAuthorityAndARICSYearAsync(long AuthorityId, int ARICSYearId);
        Task<GenericResponse> AddAsync(ARICSMasterApproval aRICSMasterApproval);
        Task<GenericResponse> FindByIdAsync(long ID);
        Task<GenericResponse> Update(ARICSMasterApproval aRICSMasterApproval);
        Task<GenericResponse> Update(long ID, ARICSMasterApproval aRICSMasterApproval);
        Task<GenericResponse> RemoveAsync(long ID);
        Task<GenericResponse> DetachFirstEntryAsync(ARICSMasterApproval aRICSMasterApproval);
    }
}
