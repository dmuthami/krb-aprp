using APRP.Web.ViewModels.CountyVM;
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IConstituencyService
    {
        Task<ConstituencyListResponse> ListAsync();
        Task<ConstituencyListResponse> ListByNameAsync(string ConstituencyName);
        Task<ConstituencyResponse> AddAsync(Constituency constituency);
        Task<ConstituencyResponse> FindByIdAsync(long ID);
        Task<ConstituencyResponse> UpdateAsync(Constituency constituency);
        Task<ConstituencyResponse> RemoveAsync(long ID);
        Task<ConstituencyListResponse> GetConstituenciesForCounty(CountyViewModel CVM);
        Task<ConstituencyResponse> GetConstituencyAndCounty(long ID);
    }
}
