using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IDirectorService
    {
        Task<IEnumerable<Director>> ListAsync();
        Task<DirectorResponse> AddAsync(Director director);
        Task<DirectorResponse> FindByIdAsync(long ID);
        Task<DirectorResponse> UpdateAsync(Director director);
        Task<DirectorResponse> RemoveAsync(long ID);
    }
}
