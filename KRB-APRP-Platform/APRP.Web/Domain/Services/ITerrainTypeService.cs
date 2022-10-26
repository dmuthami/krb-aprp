using APRP.Web.Domain.Models;
using APRP.Web.Domain.Services.Communication;

namespace APRP.Web.Domain.Services
{
    public interface ITerrainTypeService
    {
        Task<TerrainTypeListResponse> ListAsync();
        Task<TerrainTypeResponse> AddAsync(TerrainType terrainType);
        Task<TerrainTypeResponse> FindByIdAsync(long ID);
        Task<TerrainTypeResponse> Update(TerrainType terrainType);
        Task<TerrainTypeResponse> RemoveAsync(long ID);

    }
}
