using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface IDisbursementTrancheRepository
    {
        Task<IActionResult> ListAsync();
        Task<IActionResult> AddAsync(DisbursementTranche disbursementTranche);
        Task<IActionResult> FindByIdAsync(long ID);
        Task<IActionResult> Update(DisbursementTranche disbursementTranche);
        Task<IActionResult> Update(long ID,DisbursementTranche disbursementTranche);
        Task<IActionResult> Remove(DisbursementTranche disbursementTranche);
    }
}
