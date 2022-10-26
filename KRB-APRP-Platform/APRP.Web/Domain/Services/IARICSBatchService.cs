using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IARICSBatchService
    {
        Task<GenericResponse> ListAsync();
        Task<GenericResponse> ListByAuthorityAndARICSYearAsync(long AuthorityId, int ARICSYearId);

        Task<GenericResponse> ListByARICSMasterApprovalIdAndARICSYearIdAsync(long ARICSMasterApprovalId,
            int ARICSYearId);
        Task<GenericResponse> AddAsync(ARICSBatch aRICSBatch);
        Task<GenericResponse> FindByIdAsync(long ID);
        Task<GenericResponse> FindByRoadSectionIdAndARICSMasterApprovalIdAsync(long ARICSMasterApprovalID, long RoadSectionId);
        Task<GenericResponse> Update(ARICSBatch aRICSBatch);
        Task<GenericResponse> Update(long ID, ARICSBatch aRICSBatch);
        Task<GenericResponse> RemoveAsync(long ID);
        Task<GenericResponse> DetachFirstEntryAsync(ARICSBatch aRICSBatch);
    }
}
