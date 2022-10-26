using APRP.Services.AuthorityAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Services.AuthorityAPI.Persistence.Repositories
{
    public interface IAuthorityRepository
    {
        Task<IActionResult> ListAsync(int PageNumber, int PageSize);
        Task<IActionResult> FindByCodeAsync(string code);
        Task<IActionResult> FindByIdAsync(long iD);
        Task<IActionResult> Remove(Authority authority);
        Task<IActionResult> AddAsync(Authority authority);
        Task<IActionResult> Update(long ID, Authority authority);
    }
}
