using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IMessageOutRepository
    {
        Task AddAsync(MessageOut messageOut);
 
    }
}
