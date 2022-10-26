using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class ExecutionMethodRepository : BaseRepository, IExecutionMethodRepository
    {
        public ExecutionMethodRepository(AppDbContext appDbContext) : base(appDbContext)
        {
               
        }

        public async Task<IEnumerable<ExecutionMethod>> ListAsync()
        {
            return await _context.ExecutionMethods.ToListAsync();
        }
    }
}
