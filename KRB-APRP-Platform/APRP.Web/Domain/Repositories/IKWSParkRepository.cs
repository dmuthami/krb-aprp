using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface IKWSParkRepository
    {
        Task<IActionResult> ListAsync();
        Task<IActionResult> AddAsync(KWSPark kWSPark);
        Task<IActionResult> FindByIdAsync(int ID);
        Task<IActionResult> Update(KWSPark kWSPark);
        Task<IActionResult> Update(int ID,KWSPark kWSPark);
        Task<IActionResult> Remove(KWSPark kWSPark);
    }
}
