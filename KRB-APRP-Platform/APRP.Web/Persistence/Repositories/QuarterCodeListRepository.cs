using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class QuarterCodeListRepository : BaseRepository, IQuarterCodeListRepository
    {
        public QuarterCodeListRepository(AppDbContext context) 
            : base(context)
        {
            
        }

        public async Task AddAsync(QuarterCodeList quarterCodeList)
        {
            await _context.QuarterCodeLists.AddAsync(quarterCodeList);
        }


        public async Task<QuarterCodeList> FindByIdAsync(long ID)
        {
            return await _context.QuarterCodeLists
                .FirstOrDefaultAsync(m => m.ID == ID).ConfigureAwait(false);
        }

        public async Task<QuarterCodeList> FindByNameAsync(string Item)
        {
            return await _context.QuarterCodeLists
                .FirstOrDefaultAsync(m => m.Name.ToLower() == Item.ToLower()).ConfigureAwait(false);
        }

        public async Task<IEnumerable<QuarterCodeList>> ListAsync()
        {
           return await _context.QuarterCodeLists
                 .ToListAsync().ConfigureAwait(false);
        }

        public void Remove(QuarterCodeList quarterCodeList)
        {
            _context.QuarterCodeLists.Remove(quarterCodeList);
        }

        public void Update(QuarterCodeList quarterCodeList)
        {
            _context.QuarterCodeLists.Update(quarterCodeList);
        }

        public void Update(long ID, QuarterCodeList quarterCodeList)
        {
            _context.Entry(quarterCodeList).State = EntityState.Modified;
        }
    }
}
