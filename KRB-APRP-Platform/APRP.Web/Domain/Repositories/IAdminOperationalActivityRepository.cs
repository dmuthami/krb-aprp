using APRP.Web.Domain.Models;

namespace APRP.Web.Domain.Repositories
{
    public interface IAdminOperationalActivityRepository
    {
        Task<IEnumerable<AdminOperationalActivity>> ListByAuthorityAsync(long authorityId, long financialYearId);
        Task AddAsync(AdminOperationalActivity adminOperationalActivity);
        Task<AdminOperationalActivity> FindByIdAsync(long ID);
        void Update(AdminOperationalActivity adminOperationalActivity);
        void Remove(AdminOperationalActivity adminOperationalActivity);
    }
}
