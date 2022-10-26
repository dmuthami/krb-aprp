using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface IRegionService
    {
        Task<IEnumerable<Region>> ListAsync();
        Task<RegionResponse> GetRegionAsync(long regionId);
        Task<IEnumerable<Region>> ListRegionsByAuthority(long authorityID);

    }
}
