using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface IMunicipalityRepository
    {
        Task<IActionResult> ListAsync();
        Task<IActionResult> AddAsync(Municipality municipality);
        Task<IActionResult> FindByIdAsync(int ID);
        Task<IActionResult> Update(Municipality municipality);
        Task<IActionResult> Update(int ID,Municipality municipality);
        Task<IActionResult> Remove(Municipality municipality);
    }
}
