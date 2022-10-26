using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface IARICSApprovalRepository
    {
        Task<IActionResult> ListAsync();
        Task<IActionResult> AddAsync(ARICSApproval aRICSApproval);
        Task<IActionResult> FindByIdAsync(long ID);
        Task<IActionResult> FindByARICSMasterApprovalIdAsync(long ARICSMasterApprovalIdId);
        Task<IActionResult> Update(ARICSApproval aRICSApproval);
        Task<IActionResult> Update(long ID, ARICSApproval aRICSApproval);
        Task<IActionResult> Remove(ARICSApproval aRICSApproval);

        Task<IActionResult> DetachFirstEntryAsync(ARICSApproval aRICSApproval);

        Task<IActionResult> ListHistoryAsync(long ARICSApprovalId);
    }
}
