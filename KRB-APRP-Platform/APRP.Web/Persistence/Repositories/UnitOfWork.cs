using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task ApplicationRoleCompleteAsync()
        {
            try
            {
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException DBEx)
            {
                foreach (var entry in DBEx.Entries)
                {
                    if (entry.Entity is ApplicationRole)
                    {
                        var proposedValues = entry.CurrentValues;
                        var databaseValues = entry.GetDatabaseValues();

                        foreach (var property in proposedValues.Properties)
                        {
                            var proposedValue = proposedValues[property];
                            var databaseValue = databaseValues[property];

                            // TODO: decide which value should be written to database
                            // proposedValues[property] = <value to be saved>;

                        }

                        // Refresh original values to bypass next concurrency check
                        entry.OriginalValues.SetValues(databaseValues);
                    }
                    else
                    {
                        throw new NotSupportedException(
                            "Don't know how to handle concurrency conflicts for "
                            + entry.Metadata.Name);
                    }
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Update Failed", Ex);
            }
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
                throw new Exception("Update Failed",Ex); 
            }
        }
    }
}
