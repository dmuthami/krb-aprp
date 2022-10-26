using APRP.Web.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc;

namespace APRP.Web.Persistence.Repositories
{
    public abstract class BaseRepository : ControllerBase
    {
        protected readonly AppDbContext _context;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
        }
    }
}
