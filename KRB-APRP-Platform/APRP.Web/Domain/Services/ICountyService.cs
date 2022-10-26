using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface ICountyService 
    {
        Task<IEnumerable<County>> ListAsync();
        Task<IEnumerable<County>> ListByNameAsync(string CountyName);
        Task<CountyResponse> AddAsync(County county);
        Task<CountyResponse> FindByIdAsync(long ID);
        Task<CountyResponse> UpdateAsync(County county);
        Task<CountyResponse> RemoveAsync(long ID);
    }
}
