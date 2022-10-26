using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface IDisbursementReleaseRepository
    {
        Task<IActionResult> ListAsync();
        Task<IActionResult> AddAsync(DisbursementRelease disbursementRelease);
        Task<IActionResult> FindbisbursementReleaseAsync(long ReleaseId, long DisbursementId);
        Task<IActionResult> Update(DisbursementRelease disbursementRelease);
        Task<IActionResult> Update(int ID,DisbursementRelease disbursementRelease);
        Task<IActionResult> Remove(DisbursementRelease disbursementRelease);
    }
}
