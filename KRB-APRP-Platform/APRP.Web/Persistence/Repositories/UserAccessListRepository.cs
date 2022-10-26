using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace APRP.Web.Persistence.Repositories
{
    public class UserAccessListRepository : BaseRepository, IUserAccessListRepository
    {
        public UserAccessListRepository(AppDbContext context) : base(context)
        {

        }

        public async Task AddAsync(UserAccessList userAccessList)
        {
            await _context.UserAccessLists.AddAsync(userAccessList).ConfigureAwait(false);
        }

        public async Task<UserAccessList> FindByEmailAsync(string Email)
        {
            return await _context.UserAccessLists
                .Where(u=>u.EmailAddress.ToLower().Trim()==Email.ToLower().Trim())
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<UserAccessList> FindByIdAsync(long ID)
        {
            return await _context.UserAccessLists
                .Include(a=>a.Authority)
                .Where(w=>w.Id==ID)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false); 
        }


        public async Task<IEnumerable<UserAccessList>> ListAsync()
        {
            return await _context.UserAccessLists
                .Include(u => u.Authority)
                .ToListAsync().ConfigureAwait(false);
        }

        public void Remove(UserAccessList userAccessList)
        {
            _context.UserAccessLists.Remove(userAccessList);
        }

        public void Update(UserAccessList userAccessList)
        {
            _context.UserAccessLists.Update(userAccessList);
        }

        public void Update(long ID, UserAccessList userAccessList)
        {
            _context.Entry(userAccessList).State = EntityState.Modified;
        }
    }
}
