using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface IARICSYearRepository
    {
        Task<IActionResult> ListAsync();
        Task<IActionResult> AddAsync(ARICSYear aRICSYear);
        Task<IActionResult> FindByIdAsync(int ID);
        Task<IActionResult> FindByYearAsync(int ARICSYear);
        Task<IActionResult> Update(ARICSYear aRICSYear);
        Task<IActionResult> Update(int ID,ARICSYear aRICSYear);
        Task<IActionResult> Remove(ARICSYear aRICSYear);
        Task<IActionResult> DetachFirstEntryAsync(ARICSYear aRICSYear);
    }
}
