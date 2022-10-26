using APRP.Web.ViewModels.CountyVM;
using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IConstituencyRepository
    {
        Task<IEnumerable<Constituency>> ListAsync();
        Task<IEnumerable<Constituency>> ListByNameAsync(string ConstituencyName);
        Task AddAsync(Constituency constituency);
        Task<Constituency> FindByIdAsync(long ID);
        void Update(Constituency constituency);
        void Remove(Constituency constituency);
        Task<IEnumerable<Constituency>> GetConstituenciesForCounty(CountyViewModel CVM);
        Task<Constituency> GetConstituencyAndCounty(long ID);
    }
}
