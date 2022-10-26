using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IDirectorRepository
    {
        Task<IEnumerable<Director>> ListAsync();
        Task AddAsync(Director director);
        Task<Director> FindByIdAsync(long ID);
        void Update(Director director);
        void Remove(Director director);
    }
}
