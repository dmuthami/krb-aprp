using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IUserAccessListRepository
    {
        Task<IEnumerable<UserAccessList>> ListAsync();
        Task AddAsync(UserAccessList userAccessList);
        Task<UserAccessList> FindByIdAsync(long ID);

        Task<UserAccessList> FindByEmailAsync(string Email);

        void Update(UserAccessList userAccessList);
        void Update(long ID, UserAccessList userAccessList);
        void Remove(UserAccessList userAccessList);
    }
}
