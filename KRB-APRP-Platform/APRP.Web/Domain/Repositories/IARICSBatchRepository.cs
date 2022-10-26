using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface IARICSBatchRepository
    {
        Task<IActionResult> ListAsync();
        Task<IActionResult> AddAsync(ARICSBatch aRICSBatch);
        Task<IActionResult> FindByIdAsync(long ID);

        Task<IActionResult> FindByRoadSectionIdAndARICSMasterApprovalIdAsync(long ARICSMasterApprovalID, long RoadSectionId);
        Task<IActionResult> ListByAuthorityAndARICSYearAsync(long AuthorityId, int ARICSYearId);

        Task<IActionResult> ListByARICSMasterApprovalIdAndARICSYearIdAsync(long ARICSMasterApprovalId,
            int ARICSYearId);
        Task<IActionResult> Update(ARICSBatch aRICSBatch);
        Task<IActionResult> Update(long ID, ARICSBatch aRICSBatch);
        Task<IActionResult> Remove(ARICSBatch aRICSBatch);

        Task<IActionResult> DetachFirstEntryAsync(ARICSBatch aRICSBatch);

    }
}
