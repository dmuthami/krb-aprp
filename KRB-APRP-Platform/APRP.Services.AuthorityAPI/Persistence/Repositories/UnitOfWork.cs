using APRP.Services.AuthorityAPI.Persistence.DbContexts;
using APRP.Services.AuthorityAPI.Services;
using Microsoft.EntityFrameworkCore;
using System;

namespace APRP.Services.AuthorityAPI.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CompleteAsync()
        {
            try
            {
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException DBEx)
            {
                throw new DbUpdateConcurrencyException("Update Failed due to DB Concurrency Issues", DBEx);
            }
            catch (Exception Ex)
            {
                throw new Exception("Update Failed", Ex);
            }
        }
    }
}
