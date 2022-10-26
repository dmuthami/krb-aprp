using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface IGravelRequiredRepository
    {
        Task<IActionResult> ListAsync();
        Task<IActionResult> AddAsync(GravelRequired gravelRequired);
        Task<IActionResult> FindByIdAsync(int ID);
        Task<IActionResult> Update(GravelRequired gravelRequired);
        Task<IActionResult> Update(int ID,GravelRequired gravelRequired);
        Task<IActionResult> Remove(GravelRequired gravelRequired);
    }
}
