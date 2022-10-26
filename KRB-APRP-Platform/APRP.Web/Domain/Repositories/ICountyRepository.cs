using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface ICountyRepository
    {
        Task<IEnumerable<County>> ListAsync();
        Task<IEnumerable<County>> ListByNameAsync(string CountyName);
        Task AddAsync(County county);
        Task<County> FindByIdAsync(long ID);
        void Update(County county);
        void Remove(County county);
    }
}
