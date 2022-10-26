using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface ITerrainTypeRepository
    {
        Task<IEnumerable<TerrainType>> ListAsync();
        Task AddAsync(TerrainType terrainType);
        Task<TerrainType> FindByIdAsync(long ID);
        void Update(TerrainType terrainType);
        void Remove(TerrainType terrainType);
    }
}
