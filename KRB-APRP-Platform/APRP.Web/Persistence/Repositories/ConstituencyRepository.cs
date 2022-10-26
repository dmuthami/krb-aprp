using APRP.Web.ViewModels.CountyVM;
using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class ConstituencyRepository : BaseRepository, IConstituencyRepository
    {
        public ConstituencyRepository(AppDbContext context) : base(context) { }

        public async Task AddAsync(Constituency constituency)
        {
            await _context.Constituencies.AddAsync(constituency);
        }

        public async Task<Constituency> FindByIdAsync(long ID)
        {
            return await _context.Constituencies.FindAsync(ID);
        }

        public async Task<IEnumerable<Constituency>> GetConstituenciesForCounty(CountyViewModel countyViewModel)
        {
            var constituencies = await _context.Constituencies
                 .Where(
                 s =>
                 ((s.CountyId == countyViewModel.ID))
                 )
                 .ToListAsync().ConfigureAwait(false);
            return constituencies;
        }

        public async Task<Constituency> GetConstituencyAndCounty(long ID)
        {
            var constituency = await _context.Constituencies.FindAsync(ID);

            //Explicitly add county
            await _context.Entry(constituency)
            .Reference(b => b.County)
            .LoadAsync().ConfigureAwait(false);

            return constituency;
        }

        public async Task<IEnumerable<Constituency>> ListAsync()
        {
            return await _context.Constituencies.ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Constituency>> ListByNameAsync(string ConstituencyName)
        {
            return await _context.Constituencies.Where(c => c.Name.Contains(ConstituencyName)).ToListAsync().ConfigureAwait(false);
        }

        public void Remove(Constituency constituency)
        {
            _context.Constituencies.Remove(constituency);
        }

        public void Update(Constituency constituency)
        {
            _context.Constituencies.Update(constituency);
        }
    }
}
