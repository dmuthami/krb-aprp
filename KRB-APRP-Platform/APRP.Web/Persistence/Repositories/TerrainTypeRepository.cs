using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class TerrainTypeRepository : BaseRepository, ITerrainTypeRepository
    {
        public TerrainTypeRepository(AppDbContext context) : base(context)
        {

        }

        public async Task AddAsync(TerrainType terrainType)
        {
            await _context.TerrainTypes.AddAsync(terrainType);
        }

        public async Task<TerrainType> FindByIdAsync(long ID)
        {
            return await _context.TerrainTypes.FindAsync(ID); 
        }

        public async Task<IEnumerable<TerrainType>> ListAsync()
        {
            return await _context.TerrainTypes.ToListAsync();
        }

        public void Remove(TerrainType terrainType)
        {
            _context.TerrainTypes.Remove(terrainType);
        }

        public void Update(TerrainType terrainType)
        {
            _context.TerrainTypes.Update(terrainType);
        }
    }
}
