using APRP.Web.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Domain.Repositories
{
    public interface IAuthorityRepository
    {
        Task<IActionResult> ListAsync(int PageNumber, int PageSize);
        Task<IEnumerable<Authority>> ListAsync();
        Task<IEnumerable<Authority>> ListAsync(string AuthorityType);
        Task<Authority> FindByCodeAsync(string code);
        Task<Authority> FindByIdAsync(long iD);
        void Remove(Authority authority);
        Task AddAsync(Authority authority);
        void Update(long ID,Authority authority);
    }
}
