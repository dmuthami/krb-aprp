using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IContractorRepository
    {
        Task<IEnumerable<Contractor>> ListAsync();
        Task AddAsync(Contractor contractor);
        Task<Contractor> FindByIdAsync(long ID);
        Task<Contractor> FindByKraPinAsync(string kraPin);
        void Update(Contractor contractor);
        void Remove(Contractor contractor);
    }
}
