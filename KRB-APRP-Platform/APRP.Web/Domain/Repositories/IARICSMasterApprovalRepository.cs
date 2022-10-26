using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface IARICSMasterApprovalRepository
    {
        Task<IActionResult> ListAsync();
        Task<IActionResult> AddAsync(ARICSMasterApproval aRICSMasterApproval);
        Task<IActionResult> FindByIdAsync(long ID);
        Task<IActionResult> ListByAuthorityAndARICSYearAsync(long AuthorityId, int ARICSYearId);
        Task<IActionResult> Update(ARICSMasterApproval aRICSMasterApproval);
        Task<IActionResult> Update(long ID, ARICSMasterApproval aRICSMasterApproval);
        Task<IActionResult> Remove(ARICSMasterApproval aRICSMasterApproval);

        Task<IActionResult> DetachFirstEntryAsync(ARICSMasterApproval aRICSMasterApproval);

    }
}
