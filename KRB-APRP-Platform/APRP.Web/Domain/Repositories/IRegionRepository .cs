using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IRegionRepository
    {
        Task<IEnumerable<Region>> ListAsync();
        Task<Region> GetRegionAsync(long regionId);
        Task<IEnumerable<Region>> ListRegionsByAuthority(long authorityID);
    }
}
