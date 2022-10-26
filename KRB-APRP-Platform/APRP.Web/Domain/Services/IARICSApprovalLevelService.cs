using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IARICSApprovalLevelService
    {
        Task<GenericResponse> ListAsync();
        Task<GenericResponse> AddAsync(ARICSApprovalLevel aRICSApprovalLevel);
        Task<GenericResponse> FindByIdAsync(long ID);
        Task<GenericResponse> FindByAuthorityTypeAndStatusAsync(long AuthorityType,int Status);
        Task<GenericResponse> FindByStatusAsync( int Status);
        Task<GenericResponse> Update(ARICSApprovalLevel aRICSApprovalLevel);
        Task<GenericResponse> Update(long ID, ARICSApprovalLevel aRICSApprovalLevel);
        Task<GenericResponse> RemoveAsync(long ID);
        Task<GenericResponse> DetachFirstEntryAsync(ARICSApprovalLevel aRICSApprovalLevel);
    }
}
 