using APRP.Web.Domain.Models;
using APRP.Web.Domain.Repositories;
using APRP.Web.Persistence.Contexts;

namespace APRP.Web.Persistence.Repositories
{
    public class MessageOutRepository : BaseRepository, IMessageOutRepository
    {
        public MessageOutRepository(AppDbContext context) 
            : base(context)
        {
            
        }

        public async Task AddAsync(MessageOut messageOut)
        {
            await _context.MessageOuts.AddAsync(messageOut);
        }

    }
}
