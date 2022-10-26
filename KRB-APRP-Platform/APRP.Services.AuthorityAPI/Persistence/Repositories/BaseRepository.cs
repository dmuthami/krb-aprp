using APRP.Services.AuthorityAPI.Persistence.DbContexts;
using Microsoft.AspNetCore.Mvc;
using System;

namespace APRP.Services.AuthorityAPI.Persistence.Repositories
{
    public abstract class BaseRepository: ControllerBase
    {
        protected readonly ApplicationDbContext _context;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
