using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface IARICSApprovalLevelRepository
    {
        Task<IActionResult> ListAsync();
        Task<IActionResult> AddAsync(ARICSApprovalLevel aRICSApprovalLevel);
        Task<IActionResult> FindByIdAsync(long ID);
        Task<IActionResult> FindByAuthorityTypeAndStatusAsync(long AuthorityType, int Status);
        Task<IActionResult> FindByStatusAsync(int Status);
        Task<IActionResult> Update(ARICSApprovalLevel aRICSApprovalLevel);
        Task<IActionResult> Update(long ID, ARICSApprovalLevel aRICSApprovalLevel);
        Task<IActionResult> Remove(ARICSApprovalLevel aRICSApprovalLevel);
        Task<IActionResult> DetachFirstEntryAsync(ARICSApprovalLevel aRICSApprovalLevel);
    }
}
