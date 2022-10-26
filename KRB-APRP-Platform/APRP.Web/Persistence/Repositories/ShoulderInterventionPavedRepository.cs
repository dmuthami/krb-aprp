using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class ShoulderInterventionPavedRepository : BaseRepository, IShoulderInterventionPavedRepository
    {
        public ShoulderInterventionPavedRepository(AppDbContext context) : base(context)
        {

        }

        public async Task AddAsync(ShoulderInterventionPaved shoulderInterventionPaved)
        {
            await _context.ShoulderInterventionPaveds.AddAsync(shoulderInterventionPaved);
        }

        public async Task<ShoulderInterventionPaved> FindByIdAsync(long ID)
        {
            return await _context.ShoulderInterventionPaveds.FindAsync(ID); 
        }

        public async Task<IEnumerable<ShoulderInterventionPaved>> ListAsync()
        {
            return await _context.ShoulderInterventionPaveds.ToListAsync();
        }

        public void Remove(ShoulderInterventionPaved shoulderInterventionPaved)
        {
            _context.ShoulderInterventionPaveds.Remove(shoulderInterventionPaved);
        }

        public void Update(ShoulderInterventionPaved shoulderInterventionPaved)
        {
            _context.ShoulderInterventionPaveds.Update(shoulderInterventionPaved);
        }
    }
}
